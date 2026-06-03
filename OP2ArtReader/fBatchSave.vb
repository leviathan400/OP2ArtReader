Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

''' <summary>
''' Batch export dialog, modelled on the original op2art v3 "Batch Save Objects":
''' saves a range of Images / Frames / Groups as BMP files at a chosen colour depth
''' (32 / 24 / 16 / "8 or 1"), with an option to adjust frames to the upper-left corner.
''' </summary>
Public Class fBatchSave

    Private ReadOnly prt As CPrtFile
    Private ReadOnly ini As CIni
    Private cancelRequested As Boolean = False
    Private running As Boolean = False

    Private Shared ReadOnly TransparentKey As Color = Color.FromArgb(255, 255, 0, 255) ' magenta

    ''' <summary>Parameterless constructor required by the Windows Forms designer.</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(prtFile As CPrtFile, settings As CIni)
        InitializeComponent()
        prt = prtFile
        ini = settings
        WireEvents()

        ' Counts (right side), like the original.
        lblImagesN.Text = $"Images:   0 - {prt.NumImages - 1}"
        lblPicturesN.Text = $"Pictures: 0 - {prt.NumPictures - 1}"
        lblFramesN.Text = $"Frames:   0 - {prt.NumFrames - 1}"
        lblGroupsN.Text = $"Groups:   0 - {prt.NumGroups - 1}"

        OnWhatChanged(Nothing, EventArgs.Empty)
        LoadSettings()
    End Sub

    ''' <summary>Attaches event handlers at runtime (kept out of the designer).</summary>
    Private Sub WireEvents()
        AddHandler rbImages.CheckedChanged, AddressOf OnWhatChanged
        AddHandler rbFrames.CheckedChanged, AddressOf OnWhatChanged
        AddHandler rbGroups.CheckedChanged, AddressOf OnWhatChanged
        AddHandler btnBrowse.Click, AddressOf OnBrowse
        AddHandler btnStart.Click, AddressOf OnStart
        AddHandler btnClose.Click, Sub() If running Then cancelRequested = True Else Me.Close()
    End Sub

    ''' <summary>Restores the last-used folder, depth and adjust option from op2art.ini.</summary>
    Private Sub LoadSettings()
        If ini Is Nothing Then Return
        txtFolder.Text = ini.GetString("Batch Save", "SaveFolder", "")
        Select Case ini.GetInt("Batch Save", "ColorDepth", 0)
            Case 1 : rb24.Checked = True
            Case 2 : rb16.Checked = True
            Case 3 : rb8.Checked = True
            Case Else : rb32.Checked = True
        End Select
        chkAdjust.Checked = ini.GetBool("Batch Save", "AdjustGroups", False)
    End Sub

    Private Sub SaveSettings()
        If ini Is Nothing Then Return
        ini.SetValue("Batch Save", "SaveFolder", txtFolder.Text)
        Dim depth As Integer = If(rb24.Checked, 1, If(rb16.Checked, 2, If(rb8.Checked, 3, 0)))
        ini.SetValue("Batch Save", "ColorDepth", depth)
        ini.SetValue("Batch Save", "AdjustGroups", chkAdjust.Checked)
        ini.Save()
    End Sub

    Private Enum SaveKind
        Images
        Frames
        Groups
    End Enum

    Private ReadOnly Property CurrentKind As SaveKind
        Get
            If rbFrames.Checked Then Return SaveKind.Frames
            If rbGroups.Checked Then Return SaveKind.Groups
            Return SaveKind.Images
        End Get
    End Property

    Private Function MaxIndex() As Integer
        Select Case CurrentKind
            Case SaveKind.Frames : Return prt.NumFrames - 1
            Case SaveKind.Groups : Return prt.NumGroups - 1
            Case Else : Return prt.NumImages - 1
        End Select
    End Function

    Private Sub OnWhatChanged(sender As Object, e As EventArgs)
        Dim name As String = If(CurrentKind = SaveKind.Frames, "Frame", If(CurrentKind = SaveKind.Groups, "Group", "Image"))
        lblRange.Text = $"Select {name} Range:"
        txtRange.Text = $"0-{MaxIndex()}"
        chkAdjust.Enabled = (CurrentKind <> SaveKind.Images)
    End Sub

    Private Sub OnBrowse(sender As Object, e As EventArgs)
        Using fbd As New FolderBrowserDialog()
            If fbd.ShowDialog() = DialogResult.OK Then txtFolder.Text = fbd.SelectedPath
        End Using
    End Sub

    Private Function ParseRange(text As String, max As Integer) As List(Of Integer)
        Dim result As New List(Of Integer)
        Dim seen As New HashSet(Of Integer)
        For Each part In text.Split(","c)
            Dim p = part.Trim()
            If p.Length = 0 Then Continue For
            Dim a As Integer, b As Integer
            If p.Contains("-") Then
                Dim halves = p.Split("-"c)
                If Not (Integer.TryParse(halves(0).Trim(), a) AndAlso Integer.TryParse(halves(1).Trim(), b)) Then Continue For
            Else
                If Not Integer.TryParse(p, a) Then Continue For
                b = a
            End If
            If a > b Then Dim tmp = a : a = b : b = tmp
            For v As Integer = Math.Max(0, a) To Math.Min(max, b)
                If seen.Add(v) Then result.Add(v)
            Next
        Next
        Return result
    End Function

    ''' <summary>Produces the source 32bpp bitmap for a frame/group, honouring "adjust".</summary>
    Private Function CompositeFor(kind As SaveKind, index As Integer) As Bitmap
        Dim comp As CPrtFile.CompositeImage = If(kind = SaveKind.Frames, prt.GetFrameComposite(index), prt.GetGroupComposite(index, 0))
        If comp Is Nothing OrElse comp.Bitmap Is Nothing Then Return Nothing
        If chkAdjust.Checked Then
            ' Tight crop: content already sits at the upper-left of the composite.
            Return DirectCast(comp.Bitmap.Clone(), Bitmap)
        End If
        ' Preserve registration: include the logical origin (0,0) in the canvas.
        Dim left As Integer = -comp.OriginX, top As Integer = -comp.OriginY
        Dim right As Integer = left + comp.Bitmap.Width, bottom As Integer = top + comp.Bitmap.Height
        Dim cl As Integer = Math.Min(0, left), ct As Integer = Math.Min(0, top)
        Dim cr As Integer = Math.Max(0, right), cb As Integer = Math.Max(0, bottom)
        Dim canvas As New Bitmap(Math.Max(1, cr - cl), Math.Max(1, cb - ct), PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(canvas)
            g.Clear(Color.Transparent)
            g.DrawImageUnscaled(comp.Bitmap, left - cl, top - ct)
        End Using
        Return canvas
    End Function

    ''' <summary>Flattens a 32bpp source over the magenta transparent key into 24bpp.</summary>
    Private Shared Function To24(src As Bitmap) As Bitmap
        Dim flat As New Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb)
        Using g As Graphics = Graphics.FromImage(flat)
            g.Clear(TransparentKey)
            g.DrawImageUnscaled(src, 0, 0)
        End Using
        Return flat
    End Function

    ''' <summary>
    ''' Converts a 32bpp source to an 8bpp indexed bitmap, reserving index 0 as the
    ''' magenta transparent key. Returns Nothing if the image needs more than 255
    ''' distinct opaque colours (caller then falls back to 24bpp).
    ''' </summary>
    Private Shared Function To8bppIndexed(src As Bitmap) As Bitmap
        Dim w As Integer = src.Width, h As Integer = src.Height
        Dim sd As BitmapData = src.LockBits(New Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
        Dim sbuf(sd.Stride * h - 1) As Byte
        Marshal.Copy(sd.Scan0, sbuf, 0, sbuf.Length)
        src.UnlockBits(sd)

        Dim map As New Dictionary(Of Integer, Byte)
        Dim colors As New List(Of Color) From {TransparentKey}   ' index 0 = transparent
        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                Dim o As Integer = y * sd.Stride + x * 4
                If sbuf(o + 3) < 128 Then Continue For            ' transparent
                Dim rgb As Integer = (CInt(sbuf(o + 2)) << 16) Or (CInt(sbuf(o + 1)) << 8) Or sbuf(o + 0)
                If Not map.ContainsKey(rgb) Then
                    If colors.Count >= 256 Then Return Nothing
                    map(rgb) = CByte(colors.Count)
                    colors.Add(Color.FromArgb(255, sbuf(o + 2), sbuf(o + 1), sbuf(o + 0)))
                End If
            Next
        Next

        Dim bmp As New Bitmap(w, h, PixelFormat.Format8bppIndexed)
        Dim pal As ColorPalette = bmp.Palette
        For i As Integer = 0 To colors.Count - 1
            pal.Entries(i) = colors(i)
        Next
        bmp.Palette = pal
        Dim bd As BitmapData = bmp.LockBits(New Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed)
        Try
            Dim dbuf(bd.Stride * h - 1) As Byte
            For y As Integer = 0 To h - 1
                For x As Integer = 0 To w - 1
                    Dim o As Integer = y * sd.Stride + x * 4
                    If sbuf(o + 3) >= 128 Then
                        Dim rgb As Integer = (CInt(sbuf(o + 2)) << 16) Or (CInt(sbuf(o + 1)) << 8) Or sbuf(o + 0)
                        dbuf(y * bd.Stride + x) = map(rgb)
                    End If
                Next
            Next
            Marshal.Copy(dbuf, 0, bd.Scan0, dbuf.Length)
        Finally
            bmp.UnlockBits(bd)
        End Try
        Return bmp
    End Function

    ''' <summary>Renders one object at the selected colour depth, ready to save as BMP.</summary>
    Private Function RenderForSave(kind As SaveKind, index As Integer) As Bitmap
        ' "8 or 1": images export in their native indexed format (lossless).
        If rb8.Checked AndAlso kind = SaveKind.Images Then
            Return prt.GetImageIndexed(index)
        End If

        ' Get a 32bpp source.
        Dim src As Bitmap
        If kind = SaveKind.Images Then
            src = prt.GetImageTransparent(index)
        Else
            src = CompositeFor(kind, index)
        End If
        If src Is Nothing Then Return Nothing

        Try
            If rb32.Checked Then
                Return DirectCast(src.Clone(), Bitmap)            ' 32bpp ARGB (alpha kept)
            ElseIf rb24.Checked Then
                Return To24(src)
            ElseIf rb16.Checked Then
                Return src.Clone(New Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format16bppRgb565)
            Else ' 8 or 1 for a composite
                Dim idx As Bitmap = To8bppIndexed(src)
                Return If(idx, To24(src))                          ' fall back to 24bpp if > 255 colours
            End If
        Finally
            src.Dispose()
        End Try
    End Function

    Private Sub OnStart(sender As Object, e As EventArgs)
        If running Then Return
        If Not Directory.Exists(txtFolder.Text) Then
            MessageBox.Show("Choose an existing output folder.", "Batch Save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim kind As SaveKind = CurrentKind
        Dim indices = ParseRange(txtRange.Text, MaxIndex())
        If indices.Count = 0 Then
            MessageBox.Show("Range is empty or invalid.", "Batch Save", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim prefix As String = If(kind = SaveKind.Frames, "frame", If(kind = SaveKind.Groups, "group", "image"))
        SaveSettings()

        running = True : cancelRequested = False
        btnStart.Enabled = False : btnClose.Text = "Cancel"
        bar.Minimum = 0 : bar.Maximum = indices.Count : bar.Value = 0
        Dim ok As Integer = 0, fail As Integer = 0

        Try
            For i As Integer = 0 To indices.Count - 1
                If cancelRequested Then Exit For
                Dim idx = indices(i)
                Try
                    Using outBmp As Bitmap = RenderForSave(kind, idx)
                        If outBmp IsNot Nothing Then
                            outBmp.Save(Path.Combine(txtFolder.Text, $"{prefix}{idx:00000}.bmp"), ImageFormat.Bmp)
                            ok += 1
                        Else
                            fail += 1
                        End If
                    End Using
                Catch ex As Exception
                    Debug.WriteLine($"Batch save failed for {idx}: {ex.Message}")
                    fail += 1
                End Try

                bar.Value = i + 1
                If (i And &H1F) = 0 Then
                    lblStatus.Text = $"Saving {prefix} {idx}...  ({ok} ok, {fail} failed)"
                    Application.DoEvents()
                End If
            Next
        Finally
            running = False
            btnStart.Enabled = True : btnClose.Text = "Close"
            lblStatus.Text = If(cancelRequested, $"Cancelled. {ok} saved, {fail} failed.", $"Done. {ok} saved, {fail} failed.")
        End Try
    End Sub
End Class
