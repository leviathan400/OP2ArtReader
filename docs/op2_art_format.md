# Outpost 2 Art Format (`OP2_ART.BMP` + `op2_art.prt`)

This document describes the file format used by **Outpost 2: Divided Destiny** for
its sprite art, as read by this viewer. It combines the original community format
notes, details recovered by decompiling the original `op2art.exe` / `op2art.dll`
(2005, Cynex), and the op2-formats reference (`docs/sprites.md`), all checked
against the real data files (OPU 1.4.1).

Where the behaviour of the **game** (`Outpost2.exe`) and the original **op2art
viewer** differ, the text says which one it means.

All of the game's art is built from **two files**:

| File | Contents |
|------|----------|
| `OP2_ART.BMP` | A BMP header followed by the packed pixels of every image. The header does **not** describe the data; each image's size and position come from the PRT. |
| `op2_art.prt` | Palettes, image headers (size / type / palette / pointer into the BMP), and the picture / frame / group animation tables. |

> `op2_art.prt` can be extracted from `maps.vol` (e.g. with reVOLver). In OPU
> installs it also lives loose at `OPU\base\sprites\op2_art.prt` (byte-for-byte the
> same). `OP2_ART.BMP` is only shipped loose in the game folder.

For the shipped file (`op2_art.prt`, 1,555,938 bytes), the counts are:

| Object | Count |
|--------|------:|
| Palettes | 9 |
| Images | 5 390 |
| Pictures | 160 922 |
| Frames | 24 185 |
| Groups | 2 079 |
| Optional-info slots | 6 012 |

---

## 1. The four levels

Art is built up in four levels, smallest to largest:

1. **Image** — the lowest level: **pixel data + palette + size**, but *no* placement.
   An image is usually just a small piece of a unit or building, so it can be
   reused across similar structures and animation frames (saves space).
2. **Picture** (a "layer") — an image **plus its (x, y) position** within a frame.
3. **Frame** — a set of pictures drawn transparently over each other in order
   (first drawn at the bottom, last on top). A frame is everything shown for a unit
   at one instant.
4. **Group** — an **animation**: a sequence of frames, plus a selection rectangle,
   a reference point, and up to four clip rectangles.

```
Image  ──referenced by──►  Picture  ──collected into──►  Frame  ──sequenced into──►  Group
(pixels+palette+size)      (+ x,y offset)                (composited)                (animation + selection box)
```

---

## 2. `OP2_ART.BMP`

A Windows BMP header (`BM`, 40-byte `BITMAPINFOHEADER`, a 256-entry all-zero
palette) in front of a block of packed image pixels. The header's width/height
(2732 × −2731) do not describe a usable picture; only the **pixel data offset**
(`bfOffBits`, 1078 in the shipped file) matters. Each image header in the PRT
stores a byte offset (`DataPtr`) counted from that pixel data offset.

Images are stored top-down, row after row, back to back:

- **8bpp images**: one palette index per pixel; each row is `ScanLineByteWidth`
  bytes (width rounded up to a multiple of 4).
- **1bpp shadow images** (type bit 2, see §4.3): one bit per pixel, MSB first; rows
  are padded to a **32-bit boundary**, and the stored `ScanLineByteWidth` is **not**
  the 1bpp row size.

426 images share their `DataPtr` with another image (the same pixels described twice).

---

## 3. `op2_art.prt` structure

The PRT is read top-to-bottom in three sections: **palettes**, **image headers**,
then the **animation tables**. There is no padding and no offset table.
C-like layout (little-endian):

```c
struct op2art_prt {
    char  id[4] = "CPAL";
    int   pal_count;                 // 9
    ppal  palettes[pal_count];       // 1052 bytes each

    int   image_count;               // 5390
    op2image images[image_count];    // 20 bytes each

    // animation section: four totals, then the group records
    int   group_count;               // 2079
    int   frame_count;               // total frames across all groups
    int   picture_count;             // total pictures across all frames
    int   optional_count;            // optional-info slots (see §3.3)
    imagegroup groups[group_count];  // frames, pictures, clip rects are nested inline

    // followed by 12,172 trailing bytes that belong to no record (ignored)
};
```

### 3.1 Palettes (`ppal`)

Each palette is a small RIFF-like chunked block of exactly **1052 bytes**.
256 colours, stored **red, green, blue, flags** per entry — the *reverse* of a BMP's
blue, green, red order. (This viewer reads the first byte into the BMP-style
`rgbBlue` slot and then swaps blue↔red, which gives the correct RGB.) The flags
byte is 4 (`PC_NOCOLLAPSE`) in every entry and is ignored.

```c
struct ppal_entry { unsigned char red, green, blue, flags; };   // on-disk order

struct ppal {
    char id[4]    = "PPAL";   int size;        // 1048 (4 more than the bytes that follow)
    char head[4]  = "head";   int head_size;   // 4
    int  version;                              // 1
    char data[4]  = "data";   int data_size;   // 1024
    ppal_entry entries[256];
};
```

Because the `PPAL` size field is off by 4, step from one palette to the next by
1052 bytes, not by the size. This viewer parses the chunks by tag; it does not
skip the contents of unrecognised tags (`RIFF`, `pspl`, `ptpl`), none of which
occur in the shipped file.

**Player colours:** slots 1–23 of palettes 1–8 hold player 0's (blue) colour ramp.
The game swaps them for another player's ramp from `color.bmp` when drawing an
image with type bit 0 set. Palette 0 has ordinary colours there. (This viewer
always shows the stored blue ramp.)

### 3.2 Image headers (`op2image`)

One 20-byte record per image:

```c
struct op2image {
    int   ScanLineByteWidth;  // width rounded up to 4 — for shadows too (not their real stride)
    int   DataPtr;            // byte offset from OP2_ART.BMP's pixel data offset
    int   Height;             // note: height comes before width
    int   Width;
    short Type;               // bit field, see below
    short PaletteNum;         // index into the palette array
};
```

**Image `Type` is a bit field:**

| Bits | Meaning |
|-----:|---------|
| 0 | Drawn in the player's colour (OP2Utility calls it "in-game graphic") |
| 2 | **1bpp shadow** mask (otherwise 8bpp indexed) |
| 5–9 | Clip index *k*: drawn only inside the group's *k*-th clip rectangle (0 = none) |
| 1, 3–4, 10–15 | Never set |

Values that occur in the shipped file:

| Type | Count | Meaning |
|-----:|------:|---------|
| 0x0 | 83 | 8bpp, no player colour (mostly menu/UI art) |
| 0x1 | 4 871 | 8bpp, player colour |
| 0x4 | 86 | 1bpp shadow |
| 0x5 | 322 | 1bpp shadow (bit 0 set, no visible effect) |
| 0x21 / 0x41 / 0x61 / 0x81 | 3 / 20 / 3 / 2 | 8bpp, player colour, clip index 1 / 2 / 3 / 4 |

So "types 4 and 5 are shadows" holds for the shipped file, but the correct test
is `Type & 0x4`.

### 3.3 Animation tables

After the image headers come four `int` totals, then the group records.
**Frames, pictures and clip rectangles are stored inline inside each group**, in
order — there are no separate top-level frame/picture arrays on disk. The game uses
the totals as allocation sizes, so the frame and picture totals must match the
records.

```c
struct imagegroup {                 // a "group" / animation
    int   unknown1;                 // 1 in 2071 groups, 0 in 8
    int   sel_left, sel_top;        // selection rectangle, frame-local pixels
    int   sel_right, sel_bottom;    // right/bottom are EXCLUSIVE (not width/height)
    int   ref_x, ref_y;             // reference point: the frame-local pixel placed on
                                    // the object's map position ("centre")
    int   unknown2;                 // 0 in 2060 groups, 60 in 18, 13 in 1
    int   frame_count;              // 1 .. 263
    op2frame  frames[frame_count];  // inline, variable-size (see below)
    int   clip_count;               // 0 .. 4
    cliprect  clips[clip_count];
};

struct op2frame {
    unsigned char b0;               // bits 0-6: picture count; bit 7: optional pair 1 follows
    unsigned char b1;               // bits 0-6: unknown (ignored by the game); bit 7: pair 2 follows
    signed char   pair1_x, pair1_y; // only if (b0 & 0x80)
    signed char   pair2_x, pair2_y; // only if (b1 & 0x80)
    op2picture pictures[b0 & 0x7F];
};

struct op2picture {                 // a "picture" / layer = image + position
    unsigned short img_number;      // index into images[]
    unsigned char  unknown;         // 255 in most; 0-42 in 1,800 layers. Ignored by the game
    unsigned char  layer_index;     // position within the frame: 0, 1, 2, ... (draw order)
    short pos_x, pos_y;             // top-left of the image, frame-local pixels
};

struct cliprect { int left, top, right, bottom; };   // frame-local, edges included
```

**Optional pairs** ("extended info", TethysAPI `FrameOptionalInfo`) are **signed**
pixel offsets from the reference point. On a vehicle chassis, pair 1 is where the
turret attaches (Lynx facing east, group 1202: (0, −5)); on a Tiger turret, pair 2
is the second barrel. 1,663 frames carry pair 1 and 647 carry pair 2.

**`optional_count`** is *not* the number of pairs: the game reserves one 4-byte
slot for **every** frame of a group as soon as any of its frames carries a pair. So
it is the total frame count of those groups (6,012 in the shipped file).

**Clip rectangles** (earlier notes called them "appendices", purpose unknown): an
image whose type has clip index *k* is drawn only inside rectangle *k* (counting
from 1) of the group it is drawn in. This is how a Cargo Truck sinks into a
Smelter's dock pit (group 497 has rectangles (12, 9, 41, 38) and (12, 5, 41, 38)).
90 groups have clip rectangles.

**Trailing bytes:** the 12,172 bytes after the last group look like leftover frame
records but are never read by the game.

---

## 4. Rendering semantics

### 4.1 Transparency — palette index 0

When drawing a sprite, pixels whose **palette index is 0 are skipped** (transparent),
whatever colour the palette holds there (black, magenta or dark grey depending on
the palette). This is *index*-based, not colour-based: do **not** colour-key by the
RGB of entry 0, because that colour can legitimately reappear at other indices
inside the sprite (doing so leaves halos / holes). This viewer treats index 0 as
transparent in every view, including the standalone Image view.

### 4.2 Frame composition

1. Take the frame's pictures in `layer_index` order (the game files each picture at
   `layers[layer_index]`; in the shipped file this equals file order).
2. Draw each image with its top-left at `(pos_x, pos_y)` in frame-local pixels,
   skipping transparent pixels; with a clip index *k*, also skip pixels outside clip
   rectangle *k*.
3. Later pictures draw over earlier ones; a shadow darkens what is already drawn.
4. To place a frame on the map, put the frame-local reference point `(ref_x, ref_y)`
   on the object's pixel position.

A frame's bounding box is the union of its pictures' placed rectangles. To keep an
animation from shifting, use the union over **all** frames of the group (this
viewer does so on the Group tab).

> **Viewer limitation:** this viewer does not yet apply clip rectangles, so the
> 28 clip-indexed images draw unclipped.

### 4.3 Shadows (type bit 2)

Shadow images are **1bpp masks that darken the background**, not coloured sprites;
a set bit is shadow, a clear bit is transparent. The palette number is ignored.

- **The game** darkens each pixel under a set bit to **three quarters** (each 5-bit
  channel `c` becomes `c/2 + c/4` on its 15-bit screen) — a 25% darkening.
- **The original op2art viewer** halves the destination (`dest.rgb >>= 1`) — 50%.
- **This viewer** renders them as semi-transparent black (alpha 128, ≈50%), following
  op2art.

Critical packing detail: shadow rows are padded to a **32-bit boundary**, so the
1bpp stride is `((width + 31) / 32) * 4` bytes. The `ScanLineByteWidth` field holds
the 8bpp-style pitch instead and must **not** be used as the shadow stride (using it
produces horizontal-streak artifacts). Bit order is **MSB-first** within each byte
(pixel 0 = bit `0x80`).

### 4.4 Group selection border (op2art viewer)

When "Draw Borders" is on, the group's selection rectangle
(`sel_left/top/right/bottom`) is drawn as **white corner-brackets**, *not* a full
rectangle: each side is split, leaving the middle third open. The split inset from
each corner is `side_length / 6`. (In the game the selection box is the click
area; 29 groups have an all-zero box.)

### 4.5 Group lights (op2art viewer)

When "Draw Lights" is on, the original viewer overlays a **single fixed "lights"
group — group index 47** (a small green marker) — at `(sel_left + 2, sel_top + 2)`
relative to the group being drawn, animated by a global frame counter. (In
op2art.dll this is `groupBase + 0xEB0`, and `0xEB0 = 47 × 0x50`, its group stride.)

---

## 5. In-memory layouts (reference)

These are the **original op2art.dll** viewer's runtime structures, not the game's.
It expands the inline disk tables into flat arrays plus a fixed group struct
(stride `0x50` bytes):

```c
struct mem_group {   // 0x50 bytes (op2art.dll)
    int unknown1;                       // +0x00
    int sel_left, sel_top, sel_right, sel_bottom;  // +0x04 .. +0x10
    int ref_x, ref_y;                   // +0x14, +0x18
    int unknown2;                       // +0x1C
    int frame_count;                    // +0x20
    void* frame_table;                  // +0x24
    int   global_optional_index;        // +0x28
    int   clip_count;                   // +0x2C
    void* clips;                        // +0x30
    // ...padding to 0x50
};
```

In op2art.dll the image stride is `0x16` bytes, frames `6` bytes and pictures `8`
bytes. **The game** (`Outpost2.exe`) instead allocates 14 bytes per frame, 6 per
picture and 4 per optional slot, and has fixed room for 5,608 images and 2,176
groups. The game also numbers groups from **101** (game id = group index + 101).

---

## 6. Summary cheat-sheet

- Two files: **BMP = pixels** (offsets from `bfOffBits`), **PRT = everything else**.
- Levels: **Image → Picture → Frame → Group**.
- 9 palettes, entries stored **R, G, B, flags**; **index 0 = transparent**.
- Image type is a **bit field**: bit 0 player colour, **bit 2 = 1bpp shadow**,
  bits 5–9 clip index. Shadow rows are packed to 32 bits.
- Frame header high bits flag optional **signed** (x, y) pairs.
- Each group ends with up to 4 **clip rectangles** (formerly "appendices").
- Selection rect right/bottom are exclusive; `ref_x/ref_y` is the map anchor.
- Pictures draw in `layer_index` order; op2art draws the selection box as white
  corner-brackets and "lights" as fixed **group 47**.

## References

- Original community notes: `artinfo.txt`, `prt-type_def.txt`, `op2_art_mem.txt`.
- op2-formats `docs/sprites.md` (format, game behaviour, measured values) and
  OP2Utility / OP2ArtUtils / TethysAPI (OutpostUniverse).
- Outpost 2 forum: <https://forum.outpost2.net/>.
- Behavioural details of the original viewer verified by decompiling
  `op2art.exe` / `op2art.dll` (`mem_image::draw`, `mem_group::drawFlags`).
