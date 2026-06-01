# Outpost 2 Art Format (`OP2_ART.BMP` + `op2_art.prt`)

This document describes the file format used by **Outpost 2: Divided Destiny** for
its sprite art, as read by this viewer. It combines the original community format
notes with details recovered by decompiling the original `op2art.exe` / `op2art.dll`
(2005, Cynex) and verifying them against the real data files.

All of the game's art is built from **two files**:

| File | Contents |
|------|----------|
| `OP2_ART.BMP` | Raw pixel data only — **no** size or palette information. A single huge bitmap that every sprite indexes into. |
| `op2_art.prt` | Palettes, image headers (size / type / palette / pointer into the BMP), and the picture / frame / group animation tables. |

> `op2_art.prt` can be extracted from `maps.vol` (e.g. with reVOLver). In OPU
> installs it lives at `OPU\base\sprites\op2_art.prt`.

For the real file, the counts are:

| Object | Count |
|--------|------:|
| Palettes | 18 |
| Images | 5 390 |
| Pictures | 160 922 |
| Frames | 24 185 |
| Groups | 2 079 |

---

## 1. The four levels

Art is built up in four levels, smallest to largest:

1. **Image** — the lowest level: **pixel data + palette + size**, but *no* placement.
   An image is usually just a small piece of a unit or building, so it can be
   reused across similar structures and animation frames (saves space).
2. **Picture** — an image **plus its (x, y) position** within a frame.
3. **Frame** — a set of pictures drawn transparently over each other in a defined
   order (lowest order first → drawn at the bottom; highest order on top). A frame
   is everything shown for a unit at one instant.
4. **Group** — an **animation**: a sequence of frames, plus a selection rectangle,
   a centre point, and optional extra data ("appendices" / "extended info").

```
Image  ──referenced by──►  Picture  ──collected into──►  Frame  ──sequenced into──►  Group
(pixels+palette+size)      (+ x,y offset)                (composited)                (animation + selection box)
```

---

## 2. `OP2_ART.BMP`

A standard Windows BMP whose **pixel data only** is used — the BMP's own header
size/palette are ignored for individual sprites. Each image header in the PRT
stores a byte offset (`DataPtr`) into this pixel data, a per-row byte width
(`ScanLineByteWidth`), and the image dimensions. Reading starts at `DataPtr` and
walks `Height` rows of `ScanLineByteWidth` bytes.

- **8bpp images**: one palette index per pixel; `ScanLineByteWidth` is the row
  pitch (width rounded up to a 4-byte boundary).
- **1bpp shadow images** (types 4 & 5): see §6. Their rows are packed to a 32-bit
  boundary and the stored `ScanLineByteWidth` is **not** the 1bpp pitch.

---

## 3. `op2_art.prt` structure

The PRT is read top-to-bottom in three sections: **palettes**, **image headers**,
then the **animation tables**. C-like layout (little-endian):

```c
struct op2art_prt {
    char  id[4] = "CPAL";
    int   pal_count;
    ppal  palettes[pal_count];

    int   image_count;
    op2image images[image_count];

    // animation section: four counts, then the group data
    int   group_count;       // "groups" / animations
    int   frame_count;       // total frames across all groups
    int   picture_count;     // total pictures across all frames
    int   extinfo_count;     // total optional/"extended info" entries
    imagegroup groups[group_count];   // frames, pictures, appendices are nested inline
};
```

### 3.1 Palettes (`ppal`)

Each palette is a small RIFF-like chunked block. 256 colours, stored **B, G, R,
reserved** per entry (the viewer swaps B↔R to get RGB):

```c
struct rgbquad { char red, green, blue, reserved; };  // on disk: blue,green,red,reserved

struct ppal {
    char id[4]    = "PPAL";   int size;        // size ≈ 0x0418
    char head[4]  = "head";   int head_size;   // 4; followed by an int (entry/flags)
    char data[4]  = "data";   int data_size;   // 0x0400 = 1024
    rgbquad data[256];        // data_size / 4 entries
};
```

A robust reader parses these as tagged chunks (`PPAL` / `head` / `data`) rather
than a fixed struct, since other tags (`RIFF`, `pspl`, `ptpl`) can appear.

### 3.2 Image headers (`op2image`)

One 20-byte record per image:

```c
struct op2image {
    int   ScanLineByteWidth;  // row pitch in bytes (8bpp: width rounded up to 4)
    int   DataPtr;            // byte offset into OP2_ART.BMP pixel data
    int   Height;
    int   Width;
    short Type;               // see below
    short PaletteNum;         // index into the palette array
};
```

**Image `Type`:**

| Type | Meaning |
|-----:|---------|
| 0 | Menu / UI graphic (8bpp) |
| 1 | In-game graphic (8bpp) |
| 2, 3 | Other 8bpp graphics |
| 4 | Unit shadow (**1bpp** mask) |
| 5 | Unit shadow (**1bpp** mask) |

### 3.3 Animation tables

After the image headers come four `int` counts (`group_count`, `frame_count`,
`picture_count`, `extinfo_count`), then the group records. **Frames, pictures and
appendices are stored inline inside each group**, in order — there are no separate
top-level frame/picture arrays on disk; the counts are totals for validation and
for the in-memory tables the game builds.

```c
struct imagegroup {                 // a "group" / animation
    int   unknown1;                 // usually 1
    int   sel_left, sel_top, sel_right, sel_bottom;   // selection rectangle
    int   center_x, center_y;       // centre / pixel displacement
    int   unknown8;                 // usually 0
    int   frame_count;
    op2frame  frames[frame_count];  // inline, variable-size (see below)
    int   appendix_count;
    group_ext appendices[appendix_count];   // 4 ints each
};

struct op2frame {
    char  pic_count;                // low 7 bits = number of pictures; high bit = flag
    char  unknown;                  // high bit = flag
    // optional "extended info", present only when the matching high bit is set:
    char  ext_1_2[2];               // if (pic_count  & 0x80)
    char  ext_3_4[2];               // if (unknown    & 0x80)
    op2picture pictures[pic_count & 0x7F];
};

struct op2picture {                 // a "picture" = image + position
    short img_number;               // index into images[]
    char  reserved;                 // 0xFF, ignored by the game
    char  pic_order;                // draw order within the frame (low first)
    short pos_x, pos_y;             // offset within the frame
};

struct group_ext { int a, b, c, d; };   // appendix — purpose unknown
```

**Frame flags / extended info:** the two high bits of the frame header signal
optional 2-byte blocks. When `pic_count & 0x80` is set, a first extra pair is
present; when `unknown & 0x80` is set, a second pair follows. The game allocates
these "extended info" entries for *all* frames of a group if *any* frame uses them
(unset fields are zeroed). Their meaning is not known.

---

## 4. Rendering semantics

These details are required to reproduce the original output exactly.

### 4.1 Transparency — palette index 0

When drawing a sprite, pixels whose **palette index is 0 are skipped** (transparent).
This is *index*-based, not colour-based: do **not** colour-key by the RGB of entry 0,
because that colour can legitimately reappear at other indices inside the sprite
(doing so leaves halos / holes). The standalone "Image" view shows index 0 opaque
(its palette colour); compositing (pictures / frames / groups) treats it as transparent.

### 4.2 Frame composition

Within a frame, pictures are drawn **in ascending `pic_order`** (lowest first, so it
ends up at the bottom; highest on top), each at its `(pos_x, pos_y)` offset.
A frame's bounding box is the union of all its pictures' placed rectangles.

### 4.3 Shadows (types 4 & 5)

Shadow images are **1bpp masks that darken the background**, not coloured sprites.
In the original, each set bit halves the destination pixel's R/G/B
(`dest.rgb >>= 1`) — a ~50% darkening. This viewer renders them as semi-transparent
black (alpha ≈ 128).

Critical packing detail: shadow rows are padded to a **32-bit boundary**, so the
1bpp stride is `((width + 31) / 32) * 4` bytes. The `ScanLineByteWidth` field in the
image header holds the 8bpp-style pixel width instead and must **not** be used as the
shadow stride (using it produces horizontal-streak artifacts). Bit order is
**MSB-first** within each byte (pixel 0 = bit `0x80`).

### 4.4 Group selection border

When "Draw Borders" is on, the group's selection rectangle
(`sel_left/top/right/bottom`) is drawn as **white corner-brackets**, *not* a full
rectangle: each side is split, leaving the middle third open. The split inset from
each corner is `side_length / 6`.

### 4.5 Group lights

When "Draw Lights" is on, the original overlays a **single fixed "lights" group —
group index 47** (a small green marker) — at `(sel_left + 2, sel_top + 2)` relative
to the group being drawn, animated by a global frame counter. (In the binary this is
`groupBase + 0xEB0`, and `0xEB0 = 47 × 0x50`, the group stride.)

---

## 5. In-memory layout (reference)

At runtime the game expands the inline disk tables into flat arrays plus a fixed
group struct (stride `0x50` bytes):

```c
struct mem_group {   // 0x50 bytes
    int unknown1;                       // +0x00
    int sel_left, sel_top, sel_right, sel_bottom;  // +0x04 .. +0x10
    int center_x, center_y;             // +0x14, +0x18
    int unknown8;                       // +0x1C
    int frame_count;                    // +0x20
    void* frame_table;                  // +0x24
    int   global_extinfo_index;         // +0x28
    int   appendix_count;               // +0x2C
    void* appendices;                   // +0x30
    // ...padding to 0x50
};
```

The image stride in the runtime image array is `0x16` bytes; frames `6` bytes;
pictures `8` bytes.

---

## 6. Summary cheat-sheet

- Two files: **BMP = pixels**, **PRT = everything else**.
- Levels: **Image → Picture → Frame → Group**.
- Palette entries are stored **BGR(+reserved)**; **index 0 = transparent**.
- Image types **4 / 5 = 1bpp shadows** that *darken*; rows packed to 32 bits.
- Frame header high bits flag optional 2-byte "extended info" blocks.
- Pictures draw in ascending `pic_order`; group selection box is drawn as
  white corner-brackets; "lights" = fixed **group 47**.

## References

- Original community notes: `artinfo.txt`, `prt-type_def.txt`, `op2_art_mem.txt`.
- Outpost 2 forum: <https://forum.outpost2.net/>.
- Behavioural details verified by decompiling `op2art.exe` / `op2art.dll`
  (`mem_image::draw`, `mem_group::drawFlags`).
