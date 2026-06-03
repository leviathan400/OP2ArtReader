Imports System.IO
Imports System.Drawing

' OP2ArtReader
' https://github.com/leviathan400/OP2ArtReader
'
' Outpost 2 art reader. Application to view all of the sprites within the OP2_ART files.
' Re-creation of the original op2art Viewer v3.0 (2005, Cynex).
'
' Follow on from OP2Graphics project        https://forum.outpost2.net/index.php?topic=1593
' and Recreating the op2art Viewer:         https://forum.outpost2.net/index.php/topic=1635
'
'
' Outpost 2: Divided Destiny is a real-time strategy video game released in 1997.

Public Class fMain

    Public Const ApplicationName As String = "OP2ArtReader"
    Public Const ApplicationVersion As String = "0.8.0"
    Public Const ApplicationBuild As String = "0020"
    Public Const ApplicationDescription As String = "Outpost 2: Divided Destiny art reader."
    Public Const ApplicationCopyright As String = "Outpost Universe © 2025-2026"
    Public Const ApplicationWebsite As String = "https://github.com/leviathan400/OP2ArtReader"

#Region "Class Declarations"
    ' Art file paths
    Private bmpFilePath As String
    Private prtFilePath As String

    ' Class member for the PRT file
    Private prtFile As New CPrtFile()

    ' The four selectable entity kinds map 1:1 to the tab indices.
    Private Enum EntityKind
        Image = 0
        Picture = 1
        Frame = 2
        Group = 3
    End Enum

    ' Per-tab controls, built programmatically so the four tabs stay in sync.
    Private numSel(3) As NumericUpDown
    Private chkAuto(3) As CheckBox
    Private btnApply(3) As Button
    Private treeInfo(3) As TreeView

    ' Extra controls on the Group tab.
    Private numGroupFrame As NumericUpDown
    Private chkBorders As CheckBox
    Private chkLights As CheckBox
    Private chkAnimate As CheckBox

    ' Drives group frame animation on the Group tab (op2art animates by default).
    Private WithEvents animTimer As New Timer With {.Interval = 120}

    ' The original op2art hardcodes the "lights" overlay to group index 47
    ' (a small green marker), drawn at every group's selection origin.
    ' Confirmed by decompilation: art.groupBase + 0xEB0 == group[47] (stride 0x50).
    Private Const LightsGroupIndex As Integer = 47

    Private filesLoaded As Boolean = False
    Private suppressEvents As Boolean = False

    ' Bitmap currently shown in the main canvas, mirrored into the zoomed second view.
    Private zoomSource As Bitmap

    ' Magnification of the second (zoomed) render: 2x the original pixel size.
    Private Const ZoomFactor As Integer = 2

    ' Settings persisted to op2art.ini next to the executable.
    Private ini As CIni
    Private ReadOnly iniPath As String = IO.Path.Combine(Application.StartupPath, "op2art.ini")

    ' Optional group -> unit/building name map (op2art_names.ini, [GroupNames]
    ' section: "<group>=<label>"). Generated from the Outpost2.exe decompile; lets
    ' the viewer show which unit/building a group belongs to. Absent file = no names.
    Private groupNames As CIni
    Private ReadOnly groupNamesPath As String = IO.Path.Combine(Application.StartupPath, "op2art_names.ini")
#End Region

#Region "Initialization"
    Private Sub fMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Debug.WriteLine("--- " & ApplicationName & " Started ---")
        Me.Icon = My.Resources.dynamics2
        Me.Text = "Outpost 2 Art Viewer"

        BuildTabs()

        lblLoadedImages.Text = "Loaded Images: -"
        lblPictures.Text = "Pictures: -"
        lblFrames.Text = "Frames: -"
        lblGroups.Text = "Groups: -"

        ' Load persisted settings (op2art.ini) and apply view toggles.
        ini = New CIni(iniPath)
        LoadIniSettings()

        ' Load the optional group->name map (units/buildings).
        groupNames = New CIni(groupNamesPath)

        If String.IsNullOrEmpty(My.Settings.OP2Path) Then
            Debug.WriteLine("First Run")
            MessageBox.Show("Browse for the Outpost 2 (1.4.1) folder.", ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information)
            SetOP2Path()
        End If

        If Not Directory.Exists(My.Settings.OP2Path) Then
            MessageBox.Show("Can't find Outpost 2 folder. Browse for the Outpost 2 (1.4.1) folder.", ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information)
            SetOP2Path()
        End If

        UpdateFilePaths()
        LoadOP2ART()
        RestoreSelections()
    End Sub

    ''' <summary>Reads settings from op2art.ini and applies them to the UI.</summary>
    Private Sub LoadIniSettings()
        ' Prefer the path stored in the INI; fall back to .NET user settings.
        Dim iniPathVal As String = ini.GetString("Main", "OP2Path", "")
        If iniPathVal.Length > 0 Then My.Settings.OP2Path = iniPathVal

        mnuShowInfo.Checked = ini.GetBool("Art Select", "ShowInfo", True)
        mnuDrawBorders.Checked = ini.GetBool("Art Select", "DrawGroupBorders", False)
        mnuDrawLights.Checked = ini.GetBool("Art Select", "DrawGroupLights", False)
        SyncToggleCheckboxes()

        Dim autoUpd As Boolean = ini.GetBool("Art Select", "AutoUpdate", True)
        For i As Integer = 0 To 3
            chkAuto(i).Checked = autoUpd
        Next
        If chkAnimate IsNot Nothing Then chkAnimate.Checked = ini.GetBool("Art Select", "Animate", True)

        ' Canvas background colour (Gray default; Orange / Brown / Green).
        SetBackground(BackgroundByName(ini.GetString("Art Select", "Background", "Gray")))

        ' Window position, if previously saved and on-screen.
        Dim wp = ini.GetString("Main", "WindowPos", "")
        Dim parts = wp.Split(","c)
        If parts.Length = 2 Then
            Dim x As Integer, y As Integer
            If Integer.TryParse(parts(0), x) AndAlso Integer.TryParse(parts(1), y) Then
                Dim pt As New Point(x, y)
                For Each scr In Screen.AllScreens
                    If scr.WorkingArea.Contains(pt) Then
                        Me.StartPosition = FormStartPosition.Manual
                        Me.Location = pt
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>Restores the last-selected indices for each tab after the files load.</summary>
    Private Sub RestoreSelections()
        If Not filesLoaded Then Return
        suppressEvents = True
        SetSelector(EntityKind.Image, ini.GetInt("Art Select", "Image", 1))
        SetSelector(EntityKind.Picture, ini.GetInt("Art Select", "Picture", 0))
        SetSelector(EntityKind.Frame, ini.GetInt("Art Select", "Frame", 0))
        SetSelector(EntityKind.Group, ini.GetInt("Art Select", "Group", 0))
        UpdateGroupFrameRange()
        TabControl1.SelectedIndex = Math.Min(3, Math.Max(0, ini.GetInt("Art Select", "Tab", 0)))
        suppressEvents = False
        RenderCurrent()
        UpdateAnimation()
    End Sub

    Private Sub SetSelector(kind As EntityKind, value As Integer)
        Dim n = numSel(kind)
        n.Value = Math.Min(n.Maximum, Math.Max(n.Minimum, value))
    End Sub

    ''' <summary>Writes current settings back to op2art.ini.</summary>
    Private Sub SaveIniSettings()
        If ini Is Nothing Then Return
        ini.SetValue("Main", "OP2Path", If(My.Settings.OP2Path, ""))
        Dim loc As Point = If(Me.WindowState = FormWindowState.Normal, Me.Location, Me.RestoreBounds.Location)
        ini.SetValue("Main", "WindowPos", $"{loc.X},{loc.Y}")

        ini.SetValue("Art Select", "Tab", TabControl1.SelectedIndex)
        ini.SetValue("Art Select", "Image", CInt(numSel(EntityKind.Image).Value))
        ini.SetValue("Art Select", "Picture", CInt(numSel(EntityKind.Picture).Value))
        ini.SetValue("Art Select", "Frame", CInt(numSel(EntityKind.Frame).Value))
        ini.SetValue("Art Select", "Group", CInt(numSel(EntityKind.Group).Value))
        ini.SetValue("Art Select", "AutoUpdate", chkAuto(EntityKind.Image).Checked)
        ini.SetValue("Art Select", "Animate", If(chkAnimate IsNot Nothing, chkAnimate.Checked, True))
        ini.SetValue("Art Select", "Background", CurrentBackgroundName)
        ini.SetValue("Art Select", "ShowInfo", mnuShowInfo.Checked)
        ini.SetValue("Art Select", "DrawGroupBorders", mnuDrawBorders.Checked)
        ini.SetValue("Art Select", "DrawGroupLights", mnuDrawLights.Checked)
        ini.Save()
    End Sub

    Private Sub fMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        animTimer.Stop()
        SaveIniSettings()
    End Sub

    ''' <summary>
    ''' Builds the selector/checkbox/button/treeview set on each of the four tabs.
    ''' </summary>
    Private Sub BuildTabs()
        Dim pages() As TabPage = {TabImage, TabPicture, TabFrame, TabGroup}

        For i As Integer = 0 To 3
            Dim page As TabPage = pages(i)
            Dim idx As Integer = i

            Dim num As New NumericUpDown With {
                .Location = New Point(10, 12),
                .Width = 120,
                .Minimum = 0,
                .Maximum = 0
            }
            AddHandler num.ValueChanged, Sub() OnSelectorChanged(idx)
            page.Controls.Add(num)
            numSel(i) = num

            Dim chk As New CheckBox With {
                .Text = "Automatic Update",
                .Location = New Point(145, 13),
                .AutoSize = True,
                .Checked = True
            }
            page.Controls.Add(chk)
            chkAuto(i) = chk

            Dim btn As New Button With {
                .Text = "Apply",
                .Location = New Point(295, 9),
                .Width = 100
            }
            AddHandler btn.Click, Sub() RenderCurrent()
            page.Controls.Add(btn)
            btnApply(i) = btn

            ' The Group tab gets an extra frame-offset selector.
            Dim treeTop As Integer = 45
            If i = EntityKind.Group Then
                Dim lblF As New Label With {.Text = "Frame in group:", .Location = New Point(10, 47), .AutoSize = True}
                page.Controls.Add(lblF)
                numGroupFrame = New NumericUpDown With {.Location = New Point(110, 44), .Width = 70, .Minimum = 0, .Maximum = 0}
                AddHandler numGroupFrame.ValueChanged, AddressOf OnGroupFrameChanged
                page.Controls.Add(numGroupFrame)

                chkAnimate = New CheckBox With {.Text = "Animate", .Location = New Point(200, 46), .AutoSize = True, .Checked = True}
                AddHandler chkAnimate.CheckedChanged, AddressOf OnAnimateChanged
                page.Controls.Add(chkAnimate)

                chkBorders = New CheckBox With {.Text = "Draw Borders", .Location = New Point(10, 72), .AutoSize = True}
                AddHandler chkBorders.CheckedChanged, AddressOf OnGroupToggleChanged
                page.Controls.Add(chkBorders)
                chkLights = New CheckBox With {.Text = "Draw Lights", .Location = New Point(130, 72), .AutoSize = True}
                AddHandler chkLights.CheckedChanged, AddressOf OnGroupToggleChanged
                page.Controls.Add(chkLights)
                treeTop = 100
            End If

            Dim tree As New TreeView With {
                .Location = New Point(10, treeTop),
                .Size = New Size(395, page.Height - treeTop - 12),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right,
                .HideSelection = False
            }
            page.Controls.Add(tree)
            treeInfo(i) = tree
        Next
    End Sub
#End Region

#Region "File Management"
    Private Sub SetOP2Path()
        Using folderDialog As New FolderBrowserDialog()
            folderDialog.Description = "Select Outpost 2 Game Directory"
            folderDialog.ShowNewFolderButton = False

            If Not String.IsNullOrWhiteSpace(My.Settings.OP2Path) AndAlso Directory.Exists(My.Settings.OP2Path) Then
                folderDialog.SelectedPath = My.Settings.OP2Path
            Else
                folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            End If

            If folderDialog.ShowDialog() = DialogResult.OK Then
                Dim selectedPath As String = folderDialog.SelectedPath

                If Not File.Exists(Path.Combine(selectedPath, "outpost2.exe")) Then
                    MessageBox.Show("Could not find outpost2.exe in the selected folder. Please select a valid Outpost 2 installation folder.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    SetOP2Path()
                    Return
                End If

                My.Settings.OP2Path = selectedPath
                My.Settings.Save()
            End If
        End Using
    End Sub

    Private Sub UpdateFilePaths()
        bmpFilePath = Path.Combine(My.Settings.OP2Path, "OP2_ART.BMP")
        prtFilePath = Path.Combine(My.Settings.OP2Path, "OPU", "base", "sprites", "op2_art.prt")
    End Sub

    Private Function LoadOP2ART() As Boolean
        filesLoaded = False

        If prtFile.OpenBitmapFile(bmpFilePath) <> 0 Then
            Debug.WriteLine("Error loading BMP file from: " & bmpFilePath)
            MessageBox.Show("Could not load OP2_ART.BMP file." & vbCrLf & bmpFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If prtFile.OpenPrtFile(prtFilePath) <> 0 Then
            Debug.WriteLine("Error loading PRT file from: " & prtFilePath)
            MessageBox.Show("Could not load op2_art.prt file." & vbCrLf & prtFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Debug.WriteLine("Loaded OP2_ART files.")
        filesLoaded = True

        UpdateSpriteInfoLabels()
        ConfigureSelectors()

        ' Show the first image.
        TabControl1.SelectedIndex = EntityKind.Image
        RenderCurrent()
        Return True
    End Function

    ''' <summary>
    ''' Sets the min/max range of each tab's selector from the loaded counts.
    ''' </summary>
    Private Sub ConfigureSelectors()
        suppressEvents = True
        numSel(EntityKind.Image).Maximum = Math.Max(0, prtFile.NumImages - 1)
        numSel(EntityKind.Picture).Maximum = Math.Max(0, prtFile.NumPictures - 1)
        numSel(EntityKind.Frame).Maximum = Math.Max(0, prtFile.NumFrames - 1)
        numSel(EntityKind.Group).Maximum = Math.Max(0, prtFile.NumGroups - 1)
        suppressEvents = False
    End Sub
#End Region

#Region "UI Interaction"
    Private Sub UpdateSpriteInfoLabels()
        lblLoadedImages.Text = $"Loaded Images: {prtFile.NumImages}"
        lblPictures.Text = $"Pictures: {prtFile.NumPictures}"
        lblFrames.Text = $"Frames: {prtFile.NumFrames}"
        lblGroups.Text = $"Groups: {prtFile.NumGroups}"
    End Sub

    Private Sub OnSelectorChanged(tabIndex As Integer)
        If suppressEvents Then Return
        ' If the group selection changed, re-range the frame-offset control and
        ' (re)start animation for the newly selected group.
        If tabIndex = EntityKind.Group Then
            UpdateGroupFrameRange()
            UpdateAnimation()
        End If
        If chkAuto(tabIndex).Checked Then RenderCurrent()
    End Sub

    ''' <summary>Handles manual changes to the "Frame in group" selector.</summary>
    Private Sub OnGroupFrameChanged(sender As Object, e As EventArgs)
        If suppressEvents Then Return
        If chkAuto(EntityKind.Group).Checked Then RenderCurrent()
    End Sub

    Private Sub UpdateGroupFrameRange()
        Dim g As Integer = CInt(numSel(EntityKind.Group).Value)
        Dim n As Integer = prtFile.NumFramesInGroup(g)
        suppressEvents = True
        numGroupFrame.Maximum = Math.Max(0, n - 1)
        If numGroupFrame.Value > numGroupFrame.Maximum Then numGroupFrame.Value = numGroupFrame.Maximum
        ' Animation only makes sense with more than one frame.
        chkAnimate.Enabled = (n > 1)
        suppressEvents = False
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        RenderCurrent()
        UpdateAnimation()
    End Sub

    ''' <summary>
    ''' The Group-tab Draw Borders / Draw Lights checkboxes are the on-screen mirror of the
    ''' Select-menu toggles; keep the two in sync and re-render.
    ''' </summary>
    Private Sub OnGroupToggleChanged(sender As Object, e As EventArgs)
        If suppressEvents Then Return
        suppressEvents = True
        mnuDrawBorders.Checked = chkBorders.Checked
        mnuDrawLights.Checked = chkLights.Checked
        suppressEvents = False
        RenderCurrent()
    End Sub

    ''' <summary>Pushes the menu toggle states onto the Group-tab checkboxes.</summary>
    Private Sub SyncToggleCheckboxes()
        If chkBorders Is Nothing Then Return
        suppressEvents = True
        chkBorders.Checked = mnuDrawBorders.Checked
        chkLights.Checked = mnuDrawLights.Checked
        suppressEvents = False
    End Sub

    Private Sub OnAnimateChanged(sender As Object, e As EventArgs)
        UpdateAnimation()
    End Sub

    ''' <summary>Starts the frame timer when the Group tab is active, Animate is on,
    ''' and the current group has more than one frame; stops it otherwise.</summary>
    Private Sub UpdateAnimation()
        Dim shouldRun As Boolean = filesLoaded AndAlso chkAnimate IsNot Nothing AndAlso chkAnimate.Checked AndAlso
                                   TabControl1.SelectedIndex = EntityKind.Group AndAlso
                                   prtFile.NumFramesInGroup(CInt(numSel(EntityKind.Group).Value)) > 1
        If shouldRun Then
            If Not animTimer.Enabled Then animTimer.Start()
        Else
            If animTimer.Enabled Then animTimer.Stop()
        End If
    End Sub

    ''' <summary>Advances to the next frame of the current group, wrapping around.</summary>
    Private Sub animTimer_Tick(sender As Object, e As EventArgs) Handles animTimer.Tick
        If Not filesLoaded OrElse TabControl1.SelectedIndex <> EntityKind.Group Then Return
        Dim g As Integer = CInt(numSel(EntityKind.Group).Value)
        Dim n As Integer = prtFile.NumFramesInGroup(g)
        If n <= 1 Then Return
        suppressEvents = True
        numGroupFrame.Value = (CInt(numGroupFrame.Value) + 1) Mod n
        suppressEvents = False
        RenderCurrent()
    End Sub
#End Region

#Region "Rendering"
    ''' <summary>
    ''' Renders whichever entity is selected on the active tab, and refreshes its info tree.
    ''' </summary>
    Private Sub RenderCurrent()
        If Not filesLoaded Then Return

        Dim kind As EntityKind = CType(TabControl1.SelectedIndex, EntityKind)

        ' Dispose previous bitmap.
        If picCanvas.Image IsNot Nothing Then
            picCanvas.Image.Dispose()
            picCanvas.Image = Nothing
        End If

        Try
            Select Case kind
                Case EntityKind.Image
                    Dim n As Integer = CInt(numSel(EntityKind.Image).Value)
                    picCanvas.Image = prtFile.GetImage(n)
                    lblImageInfo.Text = $"Image {n}: {prtFile.ImageWidth(n)}x{prtFile.ImageHeight(n)}  Type {prtFile.ImageType(n)}  Palette {prtFile.ImagePalette(n)}"

                Case EntityKind.Picture
                    Dim n As Integer = CInt(numSel(EntityKind.Picture).Value)
                    picCanvas.Image = prtFile.GetPictureImage(n)
                    lblImageInfo.Text = $"Picture {n}: Image {prtFile.PictureImageIndex(n)}  Offset ({prtFile.PictureXOffset(n)},{prtFile.PictureYOffset(n)})  Order {prtFile.PictureOrder(n)}"

                Case EntityKind.Frame
                    Dim n As Integer = CInt(numSel(EntityKind.Frame).Value)
                    Dim comp = prtFile.GetFrameComposite(n)
                    If comp IsNot Nothing Then picCanvas.Image = comp.Bitmap
                    lblImageInfo.Text = $"Frame {n}: {prtFile.NumPicturesInFrame(n)} pictures"

                Case EntityKind.Group
                    Dim g As Integer = CInt(numSel(EntityKind.Group).Value)
                    Dim fo As Integer = CInt(numGroupFrame.Value)
                    Dim comp = prtFile.GetGroupComposite(g, fo)
                    If comp IsNot Nothing Then
                        DrawGroupOverlays(comp, g)
                        picCanvas.Image = comp.Bitmap
                    End If
                    Dim gName As String = If(groupNames IsNot Nothing, groupNames.GetString("GroupNames", g.ToString()), "")
                    lblImageInfo.Text = $"Group {g}: frame {fo + 1}/{prtFile.NumFramesInGroup(g)}  Sel({prtFile.GroupSelLeft(g)},{prtFile.GroupSelTop(g)},{prtFile.GroupSelRight(g)},{prtFile.GroupSelBottom(g)})" & If(gName <> "", "   —   " & gName, "")
            End Select
        Catch ex As Exception
            Debug.WriteLine("RenderCurrent error: " & ex.Message)
            lblImageInfo.Text = "Render error: " & ex.Message
        End Try

        ' Refresh the zoomed second view (shares the same bitmap as the main canvas).
        zoomSource = picCanvas.Image
        Dim zw As Integer = If(zoomSource IsNot Nothing, zoomSource.Width, 1) * ZoomFactor
        Dim zh As Integer = If(zoomSource IsNot Nothing, zoomSource.Height, 1) * ZoomFactor
        picZoom.Size = New Size(Math.Max(1, zw), Math.Max(1, zh))
        lblZoom.Text = $"Zoomed view — {ZoomFactor}x"
        picZoom.Invalidate()

        BuildInfoTree(kind)
    End Sub

    ''' <summary>
    ''' Paints the magnified second view at a fixed integer zoom (2x), crisp
    ''' nearest-neighbour. The panel scrolls when the result is larger than it.
    ''' </summary>
    Private Sub picZoom_Paint(sender As Object, e As PaintEventArgs) Handles picZoom.Paint
        If zoomSource Is Nothing Then Return
        Dim artW As Integer = zoomSource.Width, artH As Integer = zoomSource.Height
        If artW <= 0 OrElse artH <= 0 Then Return

        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        e.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.Half
        Try
            e.Graphics.DrawImage(zoomSource, New Rectangle(0, 0, artW * ZoomFactor, artH * ZoomFactor))
        Catch
            ' bitmap may have been disposed mid-repaint; ignore, next render repaints
        End Try
    End Sub

    ''' <summary>
    ''' Draws the selection border / centre marker over a group composite when enabled.
    ''' The canvas is first grown so a selection rectangle that extends beyond the frame
    ''' pixels (very common) is not clipped. Mutates the passed CompositeImage in place.
    ''' (Group "lights" are added in a later pass once their format is confirmed.)
    ''' </summary>
    Private Sub DrawGroupOverlays(comp As CPrtFile.CompositeImage, groupIndex As Integer)
        If Not (mnuDrawBorders.Checked OrElse mnuDrawLights.Checked) Then Return
        If comp Is Nothing OrElse comp.Bitmap Is Nothing Then Return

        ' Current frame extent in logical coordinates (pixel = logical + origin).
        Dim minX As Integer = -comp.OriginX
        Dim minY As Integer = -comp.OriginY
        Dim maxX As Integer = comp.Bitmap.Width - comp.OriginX
        Dim maxY As Integer = comp.Bitmap.Height - comp.OriginY

        ' Union with the selection rectangle and centre marker so nothing is clipped.
        Dim l As Integer = prtFile.GroupSelLeft(groupIndex)
        Dim t As Integer = prtFile.GroupSelTop(groupIndex)
        Dim r As Integer = prtFile.GroupSelRight(groupIndex)
        Dim b As Integer = prtFile.GroupSelBottom(groupIndex)
        Dim cx As Integer = prtFile.GroupCenterX(groupIndex)
        Dim cy As Integer = prtFile.GroupCenterY(groupIndex)
        minX = Math.Min(minX, Math.Min(l, cx - 3))
        minY = Math.Min(minY, Math.Min(t, cy - 3))
        maxX = Math.Max(maxX, Math.Max(r + 1, cx + 4))
        maxY = Math.Max(maxY, Math.Max(b + 1, cy + 4))

        Dim newOriginX As Integer = -minX
        Dim newOriginY As Integer = -minY
        Dim newW As Integer = Math.Max(1, maxX - minX)
        Dim newH As Integer = Math.Max(1, maxY - minY)

        ' Re-host the frame in a canvas large enough for the overlays.
        Dim canvas As New Bitmap(newW, newH, Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(canvas)
            g.Clear(Color.Transparent)
            ' Old logical top-left (-OriginOld) maps to new pixel = logical + newOrigin.
            g.DrawImageUnscaled(comp.Bitmap, -comp.OriginX + newOriginX, -comp.OriginY + newOriginY)

            If mnuDrawBorders.Checked Then DrawSelectionBrackets(g, l + newOriginX, t + newOriginY, r + newOriginX, b + newOriginY)

            ' Lights: overlay the fixed lights group (47) at (sel_left+2, sel_top+2),
            ' exactly as op2art.dll (mem_group::drawFlags / FUN_11002b90 lights branch).
            If mnuDrawLights.Checked Then
                Dim lg As Integer = LightsGroupIndex
                If lg <> groupIndex AndAlso lg >= 0 AndAlso lg < prtFile.NumGroups AndAlso prtFile.NumFramesInGroup(lg) > 0 Then
                    Dim light = prtFile.GetGroupComposite(lg, 0)   ' frame 0 of the lights group
                    If light IsNot Nothing AndAlso light.Bitmap IsNot Nothing Then
                        Dim px As Integer = prtFile.GroupSelLeft(groupIndex) + 2 + newOriginX - light.OriginX
                        Dim py As Integer = prtFile.GroupSelTop(groupIndex) + 2 + newOriginY - light.OriginY
                        g.DrawImageUnscaled(light.Bitmap, px, py)
                        light.Bitmap.Dispose()
                    End If
                End If
            End If
        End Using

        comp.Bitmap.Dispose()
        comp.Bitmap = canvas
        comp.OriginX = newOriginX
        comp.OriginY = newOriginY
    End Sub

    ''' <summary>
    ''' Draws the selection rectangle as white corner-brackets, exactly as the original
    ''' op2art.dll (mem_group::drawFlags): each side is split, leaving the middle third
    ''' open. The split inset is one sixth of the side length from each corner.
    ''' </summary>
    Private Shared Sub DrawSelectionBrackets(g As Graphics, x0 As Integer, y0 As Integer, x1 As Integer, y1 As Integer)
        Dim w As Integer = x1 - x0
        Dim h As Integer = y1 - y0
        If w = 0 OrElse h = 0 Then Return
        Dim xa As Integer = x0 + w \ 6           ' near-left split
        Dim xb As Integer = x1 - w \ 6           ' near-right split
        Dim ya As Integer = y0 + h \ 6           ' near-top split
        Dim yb As Integer = y1 - h \ 6           ' near-bottom split
        Using pen As New Pen(Color.White)
            ' Horizontal corner segments (top & bottom).
            g.DrawLine(pen, x0, y0, xa, y0)
            g.DrawLine(pen, x0, y1, xa, y1)
            g.DrawLine(pen, xb, y0, x1, y0)
            g.DrawLine(pen, xb, y1, x1, y1)
            ' Vertical corner segments (left & right).
            g.DrawLine(pen, x0, y0, x0, ya)
            g.DrawLine(pen, x1, y0, x1, ya)
            g.DrawLine(pen, x0, yb, x0, y1)
            g.DrawLine(pen, x1, yb, x1, y1)
        End Using
    End Sub
#End Region

#Region "Info Tree"
    Private Sub BuildInfoTree(kind As EntityKind)
        Dim tree As TreeView = treeInfo(kind)
        tree.BeginUpdate()
        tree.Nodes.Clear()

        If mnuShowInfo.Checked AndAlso filesLoaded Then
            Select Case kind
                Case EntityKind.Image
                    Dim n As Integer = CInt(numSel(EntityKind.Image).Value)
                    Dim root As TreeNode = tree.Nodes.Add($"Image: {n} [{n:X8}]")
                    Dim rect As TreeNode = root.Nodes.Add("Rect")
                    rect.Nodes.Add("left: 0")
                    rect.Nodes.Add("top: 0")
                    rect.Nodes.Add($"right: {prtFile.ImageWidth(n)}")
                    rect.Nodes.Add($"bottom: {prtFile.ImageHeight(n)}")
                    root.Nodes.Add($"Palette: {prtFile.ImagePalette(n)}")
                    root.Nodes.Add($"Type: {prtFile.ImageType(n)}")
                    root.ExpandAll()

                Case EntityKind.Picture
                    Dim n As Integer = CInt(numSel(EntityKind.Picture).Value)
                    Dim root As TreeNode = tree.Nodes.Add($"Picture: {n} [{n:X8}]")
                    root.Nodes.Add($"Image: {prtFile.PictureImageIndex(n)}")
                    root.Nodes.Add($"Order: {prtFile.PictureOrder(n)}")
                    root.Nodes.Add($"PosX: {prtFile.PictureXOffset(n)}")
                    root.Nodes.Add($"PosY: {prtFile.PictureYOffset(n)}")
                    root.ExpandAll()

                Case EntityKind.Frame
                    Dim n As Integer = CInt(numSel(EntityKind.Frame).Value)
                    Dim root As TreeNode = tree.Nodes.Add($"Frame: {n} [{n:X8}]")
                    Dim cnt As Integer = prtFile.NumPicturesInFrame(n)
                    root.Nodes.Add($"Pictures: {cnt}")
                    root.Nodes.Add($"Unknown: {prtFile.FrameUnknownByte(n)}")
                    If prtFile.FrameHasOpt1(n) OrElse prtFile.FrameHasOpt2(n) Then
                        Dim o = prtFile.FrameOptBytes(n)
                        Dim opt As TreeNode = root.Nodes.Add("Extended Info")
                        opt.Nodes.Add($"opt1: {o(0)}, {o(1)}")
                        opt.Nodes.Add($"opt2: {o(2)}, {o(3)}")
                    End If
                    Dim start As Integer = prtFile.GetFrameStartPicture(n)
                    For p As Integer = 0 To cnt - 1
                        Dim pi As Integer = start + p
                        Dim pn As TreeNode = root.Nodes.Add($"[{p}] Pic {pi}")
                        pn.Nodes.Add($"Image: {prtFile.PictureImageIndex(pi)}")
                        pn.Nodes.Add($"Order: {prtFile.PictureOrder(pi)}")
                        pn.Nodes.Add($"Pos: ({prtFile.PictureXOffset(pi)},{prtFile.PictureYOffset(pi)})")
                    Next
                    root.Expand()

                Case EntityKind.Group
                    Dim g As Integer = CInt(numSel(EntityKind.Group).Value)
                    Dim root As TreeNode = tree.Nodes.Add($"Group: {g} [{g:X8}]")
                    Dim sel As TreeNode = root.Nodes.Add("Selection Rect")
                    sel.Nodes.Add($"left: {prtFile.GroupSelLeft(g)}")
                    sel.Nodes.Add($"top: {prtFile.GroupSelTop(g)}")
                    sel.Nodes.Add($"right: {prtFile.GroupSelRight(g)}")
                    sel.Nodes.Add($"bottom: {prtFile.GroupSelBottom(g)}")
                    root.Nodes.Add($"Center: ({prtFile.GroupCenterX(g)},{prtFile.GroupCenterY(g)})")
                    root.Nodes.Add($"Frames: {prtFile.NumFramesInGroup(g)}")
                    root.Nodes.Add($"Appendices: {prtFile.GroupNumAppendices(g)}")
                    root.ExpandAll()
            End Select
        End If

        tree.EndUpdate()
    End Sub
#End Region

#Region "Menu Handlers"
    Private Sub mnuFileOpen_Click(sender As Object, e As EventArgs) Handles mnuFileOpen.Click
        SetOP2Path()
        UpdateFilePaths()
        LoadOP2ART()
    End Sub

    Private Sub mnuFileSaveCurrent_Click(sender As Object, e As EventArgs) Handles mnuFileSaveCurrent.Click
        If picCanvas.Image Is Nothing Then Return
        Using sfd As New SaveFileDialog()
            sfd.Filter = "PNG Image|*.png|Bitmap|*.bmp"
            sfd.FileName = "op2art_export.png"
            If sfd.ShowDialog() = DialogResult.OK Then
                Dim fmt As Imaging.ImageFormat = If(sfd.FilterIndex = 2, Imaging.ImageFormat.Bmp, Imaging.ImageFormat.Png)
                picCanvas.Image.Save(sfd.FileName, fmt)
            End If
        End Using
    End Sub

    Private Sub mnuFileBatchSave_Click(sender As Object, e As EventArgs) Handles mnuFileBatchSave.Click
        If Not filesLoaded Then
            MessageBox.Show("Load the OP2 art files first.", ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Using dlg As New fBatchSave(prtFile, ini)
            dlg.ShowDialog(Me)
        End Using
    End Sub

    Private Sub mnuFileExit_Click(sender As Object, e As EventArgs) Handles mnuFileExit.Click
        Me.Close()
    End Sub

    Private Sub mnuSelImage_Click(sender As Object, e As EventArgs) Handles mnuSelImage.Click
        TabControl1.SelectedIndex = EntityKind.Image
    End Sub
    Private Sub mnuSelPicture_Click(sender As Object, e As EventArgs) Handles mnuSelPicture.Click
        TabControl1.SelectedIndex = EntityKind.Picture
    End Sub
    Private Sub mnuSelFrame_Click(sender As Object, e As EventArgs) Handles mnuSelFrame.Click
        TabControl1.SelectedIndex = EntityKind.Frame
    End Sub
    Private Sub mnuSelGroup_Click(sender As Object, e As EventArgs) Handles mnuSelGroup.Click
        TabControl1.SelectedIndex = EntityKind.Group
    End Sub

    Private Sub mnuShowInfo_Click(sender As Object, e As EventArgs) Handles mnuShowInfo.Click
        RenderCurrent()
    End Sub
    Private Sub mnuDrawBorders_Click(sender As Object, e As EventArgs) Handles mnuDrawBorders.Click
        SyncToggleCheckboxes()
        RenderCurrent()
    End Sub
    Private Sub mnuDrawLights_Click(sender As Object, e As EventArgs) Handles mnuDrawLights.Click
        SyncToggleCheckboxes()
        RenderCurrent()
    End Sub

    Private Sub mnuHelpAbout_Click(sender As Object, e As EventArgs) Handles mnuHelpAbout.Click
        Using AboutForm As New fAbout()
            AboutForm.ApplicationTitle = ApplicationName
            AboutForm.ApplicationVersion = ApplicationVersion
            AboutForm.ApplicationBuild = ApplicationBuild
            AboutForm.ApplicationDescription = ApplicationDescription
            AboutForm.ApplicationCopyright = ApplicationCopyright
            AboutForm.ApplicationWebsite = ApplicationWebsite
            AboutForm.ShowDialog(Me)
        End Using
    End Sub

    ' Canvas background colours. Orange (0xFF7F00) is the fill the original op2art
    ' used behind group/image renders; Gray is this viewer's default.
    Private Shared ReadOnly BgGray As Color = Color.FromArgb(96, 96, 96)
    Private Shared ReadOnly BgOrange As Color = Color.FromArgb(255, 127, 0)
    Private Shared ReadOnly BgBrown As Color = Color.FromArgb(120, 80, 40)
    Private Shared ReadOnly BgGreen As Color = Color.FromArgb(40, 110, 40)

    Private Sub mnuBgGray_Click(sender As Object, e As EventArgs) Handles mnuBgGray.Click
        SetBackground(BgGray)
    End Sub
    Private Sub mnuBgOrange_Click(sender As Object, e As EventArgs) Handles mnuBgOrange.Click
        SetBackground(BgOrange)
    End Sub
    Private Sub mnuBgBrown_Click(sender As Object, e As EventArgs) Handles mnuBgBrown.Click
        SetBackground(BgBrown)
    End Sub
    Private Sub mnuBgGreen_Click(sender As Object, e As EventArgs) Handles mnuBgGreen.Click
        SetBackground(BgGreen)
    End Sub

    ''' <summary>Maps a saved background name to its colour (defaults to Gray).</summary>
    Private Shared Function BackgroundByName(name As String) As Color
        Select Case If(name, "").Trim().ToLowerInvariant()
            Case "orange" : Return BgOrange
            Case "brown" : Return BgBrown
            Case "green" : Return BgGreen
            Case Else : Return BgGray
        End Select
    End Function

    ''' <summary>The name of the currently selected background colour, for persistence.</summary>
    Private ReadOnly Property CurrentBackgroundName As String
        Get
            If mnuBgOrange.Checked Then Return "Orange"
            If mnuBgBrown.Checked Then Return "Brown"
            If mnuBgGreen.Checked Then Return "Green"
            Return "Gray"
        End Get
    End Property

    ''' <summary>Applies the canvas background colour and updates the menu check marks.</summary>
    Private Sub SetBackground(c As Color)
        pnlCanvas.BackColor = c
        pnlZoom.BackColor = c
        picZoom.BackColor = c
        picZoom.Invalidate()
        Dim argb As Integer = c.ToArgb()
        mnuBgGray.Checked = (argb = BgGray.ToArgb())
        mnuBgOrange.Checked = (argb = BgOrange.ToArgb())
        mnuBgBrown.Checked = (argb = BgBrown.ToArgb())
        mnuBgGreen.Checked = (argb = BgGreen.ToArgb())
    End Sub
#End Region

End Class
