<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSaveCurrent = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileBatchSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuFileSep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelect = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelPicture = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelFrame = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelGroup = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSelSep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuShowInfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuDrawBorders = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuDrawLights = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuBackground = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuBgGray = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuBgOrange = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlCanvas = New System.Windows.Forms.Panel()
        Me.picCanvas = New System.Windows.Forms.PictureBox()
        Me.pnlZoom = New System.Windows.Forms.Panel()
        Me.picZoom = New System.Windows.Forms.PictureBox()
        Me.lblZoom = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabImage = New System.Windows.Forms.TabPage()
        Me.TabPicture = New System.Windows.Forms.TabPage()
        Me.TabFrame = New System.Windows.Forms.TabPage()
        Me.TabGroup = New System.Windows.Forms.TabPage()
        Me.lblLoadedImages = New System.Windows.Forms.Label()
        Me.lblPictures = New System.Windows.Forms.Label()
        Me.lblFrames = New System.Windows.Forms.Label()
        Me.lblGroups = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblImageInfo = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MenuStrip1.SuspendLayout()
        Me.pnlCanvas.SuspendLayout()
        CType(Me.picCanvas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlZoom.SuspendLayout()
        CType(Me.picZoom, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile, Me.mnuSelect, Me.mnuView, Me.mnuHelp})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(914, 24)
        Me.MenuStrip1.TabIndex = 0
        '
        'mnuFile
        '
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFileOpen, Me.mnuFileSaveCurrent, Me.mnuFileBatchSave, Me.mnuFileSep1, Me.mnuFileExit})
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(37, 20)
        Me.mnuFile.Text = "File"
        '
        'mnuFileOpen
        '
        Me.mnuFileOpen.Name = "mnuFileOpen"
        Me.mnuFileOpen.Size = New System.Drawing.Size(186, 22)
        Me.mnuFileOpen.Text = "Open OP2 Folder..."
        '
        'mnuFileSaveCurrent
        '
        Me.mnuFileSaveCurrent.Name = "mnuFileSaveCurrent"
        Me.mnuFileSaveCurrent.Size = New System.Drawing.Size(186, 22)
        Me.mnuFileSaveCurrent.Text = "Save Current Image..."
        '
        'mnuFileBatchSave
        '
        Me.mnuFileBatchSave.Name = "mnuFileBatchSave"
        Me.mnuFileBatchSave.Size = New System.Drawing.Size(186, 22)
        Me.mnuFileBatchSave.Text = "Batch Save..."
        '
        'mnuFileSep1
        '
        Me.mnuFileSep1.Name = "mnuFileSep1"
        Me.mnuFileSep1.Size = New System.Drawing.Size(183, 6)
        '
        'mnuFileExit
        '
        Me.mnuFileExit.Name = "mnuFileExit"
        Me.mnuFileExit.Size = New System.Drawing.Size(186, 22)
        Me.mnuFileExit.Text = "Exit"
        '
        'mnuSelect
        '
        Me.mnuSelect.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelImage, Me.mnuSelPicture, Me.mnuSelFrame, Me.mnuSelGroup, Me.mnuSelSep1, Me.mnuShowInfo, Me.mnuDrawBorders, Me.mnuDrawLights})
        Me.mnuSelect.Name = "mnuSelect"
        Me.mnuSelect.Size = New System.Drawing.Size(50, 20)
        Me.mnuSelect.Text = "Select"
        '
        'mnuSelImage
        '
        Me.mnuSelImage.Name = "mnuSelImage"
        Me.mnuSelImage.Size = New System.Drawing.Size(180, 22)
        Me.mnuSelImage.Text = "Image"
        '
        'mnuSelPicture
        '
        Me.mnuSelPicture.Name = "mnuSelPicture"
        Me.mnuSelPicture.Size = New System.Drawing.Size(180, 22)
        Me.mnuSelPicture.Text = "Picture"
        '
        'mnuSelFrame
        '
        Me.mnuSelFrame.Name = "mnuSelFrame"
        Me.mnuSelFrame.Size = New System.Drawing.Size(180, 22)
        Me.mnuSelFrame.Text = "Frame"
        '
        'mnuSelGroup
        '
        Me.mnuSelGroup.Name = "mnuSelGroup"
        Me.mnuSelGroup.Size = New System.Drawing.Size(180, 22)
        Me.mnuSelGroup.Text = "Group"
        '
        'mnuSelSep1
        '
        Me.mnuSelSep1.Name = "mnuSelSep1"
        Me.mnuSelSep1.Size = New System.Drawing.Size(177, 6)
        '
        'mnuShowInfo
        '
        Me.mnuShowInfo.CheckOnClick = True
        Me.mnuShowInfo.Name = "mnuShowInfo"
        Me.mnuShowInfo.Size = New System.Drawing.Size(180, 22)
        Me.mnuShowInfo.Text = "Show Info"
        '
        'mnuDrawBorders
        '
        Me.mnuDrawBorders.CheckOnClick = True
        Me.mnuDrawBorders.Name = "mnuDrawBorders"
        Me.mnuDrawBorders.Size = New System.Drawing.Size(180, 22)
        Me.mnuDrawBorders.Text = "Draw Group Borders"
        '
        'mnuDrawLights
        '
        Me.mnuDrawLights.CheckOnClick = True
        Me.mnuDrawLights.Name = "mnuDrawLights"
        Me.mnuDrawLights.Size = New System.Drawing.Size(180, 22)
        Me.mnuDrawLights.Text = "Draw Group Lights"
        '
        'mnuView
        '
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBackground})
        Me.mnuView.Name = "mnuView"
        Me.mnuView.Size = New System.Drawing.Size(44, 20)
        Me.mnuView.Text = "View"
        '
        'mnuBackground
        '
        Me.mnuBackground.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBgGray, Me.mnuBgOrange})
        Me.mnuBackground.Name = "mnuBackground"
        Me.mnuBackground.Size = New System.Drawing.Size(170, 22)
        Me.mnuBackground.Text = "Background Color"
        '
        'mnuBgGray
        '
        Me.mnuBgGray.Name = "mnuBgGray"
        Me.mnuBgGray.Size = New System.Drawing.Size(113, 22)
        Me.mnuBgGray.Text = "Gray"
        '
        'mnuBgOrange
        '
        Me.mnuBgOrange.Name = "mnuBgOrange"
        Me.mnuBgOrange.Size = New System.Drawing.Size(113, 22)
        Me.mnuBgOrange.Text = "Orange"
        '
        'mnuHelp
        '
        Me.mnuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuHelpAbout})
        Me.mnuHelp.Name = "mnuHelp"
        Me.mnuHelp.Size = New System.Drawing.Size(44, 20)
        Me.mnuHelp.Text = "Help"
        '
        'mnuHelpAbout
        '
        Me.mnuHelpAbout.Name = "mnuHelpAbout"
        Me.mnuHelpAbout.Size = New System.Drawing.Size(107, 22)
        Me.mnuHelpAbout.Text = "About"
        '
        'pnlCanvas
        '
        Me.pnlCanvas.AutoScroll = True
        Me.pnlCanvas.BackColor = System.Drawing.Color.FromArgb(CType(CType(96, Byte), Integer), CType(CType(96, Byte), Integer), CType(CType(96, Byte), Integer))
        Me.pnlCanvas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlCanvas.Controls.Add(Me.picCanvas)
        Me.pnlCanvas.Location = New System.Drawing.Point(12, 30)
        Me.pnlCanvas.Name = "pnlCanvas"
        Me.pnlCanvas.Size = New System.Drawing.Size(450, 217)
        Me.pnlCanvas.TabIndex = 1
        '
        'picCanvas
        '
        Me.picCanvas.BackColor = System.Drawing.Color.Transparent
        Me.picCanvas.Location = New System.Drawing.Point(5, 5)
        Me.picCanvas.Name = "picCanvas"
        Me.picCanvas.Size = New System.Drawing.Size(100, 50)
        Me.picCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.picCanvas.TabIndex = 0
        Me.picCanvas.TabStop = False
        '
        'pnlZoom
        '
        Me.pnlZoom.AutoScroll = True
        Me.pnlZoom.BackColor = System.Drawing.Color.FromArgb(CType(CType(96, Byte), Integer), CType(CType(96, Byte), Integer), CType(CType(96, Byte), Integer))
        Me.pnlZoom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlZoom.Controls.Add(Me.picZoom)
        Me.pnlZoom.Location = New System.Drawing.Point(12, 268)
        Me.pnlZoom.Name = "pnlZoom"
        Me.pnlZoom.Size = New System.Drawing.Size(450, 258)
        Me.pnlZoom.TabIndex = 12
        '
        'picZoom
        '
        Me.picZoom.BackColor = System.Drawing.Color.Transparent
        Me.picZoom.Location = New System.Drawing.Point(0, 0)
        Me.picZoom.Name = "picZoom"
        Me.picZoom.Size = New System.Drawing.Size(100, 50)
        Me.picZoom.TabIndex = 0
        Me.picZoom.TabStop = False
        '
        'lblZoom
        '
        Me.lblZoom.AutoSize = True
        Me.lblZoom.Location = New System.Drawing.Point(12, 250)
        Me.lblZoom.Name = "lblZoom"
        Me.lblZoom.Size = New System.Drawing.Size(71, 13)
        Me.lblZoom.TabIndex = 11
        Me.lblZoom.Text = "Zoomed view"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabImage)
        Me.TabControl1.Controls.Add(Me.TabPicture)
        Me.TabControl1.Controls.Add(Me.TabFrame)
        Me.TabControl1.Controls.Add(Me.TabGroup)
        Me.TabControl1.Location = New System.Drawing.Point(478, 130)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(424, 396)
        Me.TabControl1.TabIndex = 2
        '
        'TabImage
        '
        Me.TabImage.Location = New System.Drawing.Point(4, 22)
        Me.TabImage.Name = "TabImage"
        Me.TabImage.Padding = New System.Windows.Forms.Padding(3)
        Me.TabImage.Size = New System.Drawing.Size(416, 370)
        Me.TabImage.TabIndex = 0
        Me.TabImage.Text = "Image"
        Me.TabImage.UseVisualStyleBackColor = True
        '
        'TabPicture
        '
        Me.TabPicture.Location = New System.Drawing.Point(4, 22)
        Me.TabPicture.Name = "TabPicture"
        Me.TabPicture.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPicture.Size = New System.Drawing.Size(416, 334)
        Me.TabPicture.TabIndex = 1
        Me.TabPicture.Text = "Picture"
        Me.TabPicture.UseVisualStyleBackColor = True
        '
        'TabFrame
        '
        Me.TabFrame.Location = New System.Drawing.Point(4, 22)
        Me.TabFrame.Name = "TabFrame"
        Me.TabFrame.Padding = New System.Windows.Forms.Padding(3)
        Me.TabFrame.Size = New System.Drawing.Size(416, 334)
        Me.TabFrame.TabIndex = 2
        Me.TabFrame.Text = "Frame"
        Me.TabFrame.UseVisualStyleBackColor = True
        '
        'TabGroup
        '
        Me.TabGroup.Location = New System.Drawing.Point(4, 22)
        Me.TabGroup.Name = "TabGroup"
        Me.TabGroup.Padding = New System.Windows.Forms.Padding(3)
        Me.TabGroup.Size = New System.Drawing.Size(416, 334)
        Me.TabGroup.TabIndex = 3
        Me.TabGroup.Text = "Group"
        Me.TabGroup.UseVisualStyleBackColor = True
        '
        'lblLoadedImages
        '
        Me.lblLoadedImages.AutoSize = True
        Me.lblLoadedImages.Location = New System.Drawing.Point(478, 35)
        Me.lblLoadedImages.Name = "lblLoadedImages"
        Me.lblLoadedImages.Size = New System.Drawing.Size(89, 13)
        Me.lblLoadedImages.TabIndex = 3
        Me.lblLoadedImages.Text = "Loaded Images: -"
        '
        'lblPictures
        '
        Me.lblPictures.AutoSize = True
        Me.lblPictures.Location = New System.Drawing.Point(478, 55)
        Me.lblPictures.Name = "lblPictures"
        Me.lblPictures.Size = New System.Drawing.Size(54, 13)
        Me.lblPictures.TabIndex = 4
        Me.lblPictures.Text = "Pictures: -"
        '
        'lblFrames
        '
        Me.lblFrames.AutoSize = True
        Me.lblFrames.Location = New System.Drawing.Point(478, 75)
        Me.lblFrames.Name = "lblFrames"
        Me.lblFrames.Size = New System.Drawing.Size(50, 13)
        Me.lblFrames.TabIndex = 5
        Me.lblFrames.Text = "Frames: -"
        '
        'lblGroups
        '
        Me.lblGroups.AutoSize = True
        Me.lblGroups.Location = New System.Drawing.Point(478, 95)
        Me.lblGroups.Name = "lblGroups"
        Me.lblGroups.Size = New System.Drawing.Size(50, 13)
        Me.lblGroups.TabIndex = 6
        Me.lblGroups.Text = "Groups: -"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblImageInfo})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 540)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(914, 22)
        Me.StatusStrip1.TabIndex = 7
        '
        'lblImageInfo
        '
        Me.lblImageInfo.Name = "lblImageInfo"
        Me.lblImageInfo.Size = New System.Drawing.Size(39, 17)
        Me.lblImageInfo.Text = "Ready"
        '
        'fMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(914, 562)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.lblGroups)
        Me.Controls.Add(Me.lblFrames)
        Me.Controls.Add(Me.lblPictures)
        Me.Controls.Add(Me.lblLoadedImages)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.lblZoom)
        Me.Controls.Add(Me.pnlZoom)
        Me.Controls.Add(Me.pnlCanvas)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "fMain"
        Me.Text = "Outpost 2 Art Viewer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.pnlCanvas.ResumeLayout(False)
        Me.pnlCanvas.PerformLayout()
        CType(Me.picCanvas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlZoom.ResumeLayout(False)
        CType(Me.picZoom, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents mnuFile As ToolStripMenuItem
    Friend WithEvents mnuFileOpen As ToolStripMenuItem
    Friend WithEvents mnuFileSaveCurrent As ToolStripMenuItem
    Friend WithEvents mnuFileBatchSave As ToolStripMenuItem
    Friend WithEvents mnuFileSep1 As ToolStripSeparator
    Friend WithEvents mnuFileExit As ToolStripMenuItem
    Friend WithEvents mnuSelect As ToolStripMenuItem
    Friend WithEvents mnuSelImage As ToolStripMenuItem
    Friend WithEvents mnuSelPicture As ToolStripMenuItem
    Friend WithEvents mnuSelFrame As ToolStripMenuItem
    Friend WithEvents mnuSelGroup As ToolStripMenuItem
    Friend WithEvents mnuSelSep1 As ToolStripSeparator
    Friend WithEvents mnuShowInfo As ToolStripMenuItem
    Friend WithEvents mnuDrawBorders As ToolStripMenuItem
    Friend WithEvents mnuDrawLights As ToolStripMenuItem
    Friend WithEvents mnuView As ToolStripMenuItem
    Friend WithEvents mnuBackground As ToolStripMenuItem
    Friend WithEvents mnuBgGray As ToolStripMenuItem
    Friend WithEvents mnuBgOrange As ToolStripMenuItem
    Friend WithEvents mnuHelp As ToolStripMenuItem
    Friend WithEvents mnuHelpAbout As ToolStripMenuItem
    Friend WithEvents pnlCanvas As Panel
    Friend WithEvents picCanvas As PictureBox
    Friend WithEvents pnlZoom As Panel
    Friend WithEvents picZoom As PictureBox
    Friend WithEvents lblZoom As Label
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabImage As TabPage
    Friend WithEvents TabPicture As TabPage
    Friend WithEvents TabFrame As TabPage
    Friend WithEvents TabGroup As TabPage
    Friend WithEvents lblLoadedImages As Label
    Friend WithEvents lblPictures As Label
    Friend WithEvents lblFrames As Label
    Friend WithEvents lblGroups As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblImageInfo As ToolStripStatusLabel
End Class
