<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class fBatchSave
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.rbImages = New System.Windows.Forms.RadioButton()
        Me.rbFrames = New System.Windows.Forms.RadioButton()
        Me.rbGroups = New System.Windows.Forms.RadioButton()
        Me.lblImagesN = New System.Windows.Forms.Label()
        Me.lblPicturesN = New System.Windows.Forms.Label()
        Me.lblFramesN = New System.Windows.Forms.Label()
        Me.lblGroupsN = New System.Windows.Forms.Label()
        Me.lblRange = New System.Windows.Forms.Label()
        Me.txtRange = New System.Windows.Forms.TextBox()
        Me.lblDir = New System.Windows.Forms.Label()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.lblDepth = New System.Windows.Forms.Label()
        Me.rb32 = New System.Windows.Forms.RadioButton()
        Me.rb24 = New System.Windows.Forms.RadioButton()
        Me.rb16 = New System.Windows.Forms.RadioButton()
        Me.rb8 = New System.Windows.Forms.RadioButton()
        Me.chkAdjust = New System.Windows.Forms.CheckBox()
        Me.bar = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'rbImages
        '
        Me.rbImages.AutoSize = True
        Me.rbImages.Checked = True
        Me.rbImages.Location = New System.Drawing.Point(14, 14)
        Me.rbImages.Name = "rbImages"
        Me.rbImages.TabStop = True
        Me.rbImages.Text = "Save Images"
        '
        'rbFrames
        '
        Me.rbFrames.AutoSize = True
        Me.rbFrames.Location = New System.Drawing.Point(14, 38)
        Me.rbFrames.Name = "rbFrames"
        Me.rbFrames.Text = "Save Frames"
        '
        'rbGroups
        '
        Me.rbGroups.AutoSize = True
        Me.rbGroups.Location = New System.Drawing.Point(14, 62)
        Me.rbGroups.Name = "rbGroups"
        Me.rbGroups.Text = "Save Groups"
        '
        'lblImagesN
        '
        Me.lblImagesN.AutoSize = True
        Me.lblImagesN.Location = New System.Drawing.Point(250, 14)
        Me.lblImagesN.Name = "lblImagesN"
        Me.lblImagesN.Text = "Images:"
        '
        'lblPicturesN
        '
        Me.lblPicturesN.AutoSize = True
        Me.lblPicturesN.Location = New System.Drawing.Point(250, 34)
        Me.lblPicturesN.Name = "lblPicturesN"
        Me.lblPicturesN.Text = "Pictures:"
        '
        'lblFramesN
        '
        Me.lblFramesN.AutoSize = True
        Me.lblFramesN.Location = New System.Drawing.Point(250, 54)
        Me.lblFramesN.Name = "lblFramesN"
        Me.lblFramesN.Text = "Frames:"
        '
        'lblGroupsN
        '
        Me.lblGroupsN.AutoSize = True
        Me.lblGroupsN.Location = New System.Drawing.Point(250, 74)
        Me.lblGroupsN.Name = "lblGroupsN"
        Me.lblGroupsN.Text = "Groups:"
        '
        'lblRange
        '
        Me.lblRange.AutoSize = True
        Me.lblRange.Location = New System.Drawing.Point(14, 96)
        Me.lblRange.Name = "lblRange"
        Me.lblRange.Text = "Select Image Range:"
        '
        'txtRange
        '
        Me.txtRange.Location = New System.Drawing.Point(14, 114)
        Me.txtRange.Name = "txtRange"
        Me.txtRange.Size = New System.Drawing.Size(442, 20)
        '
        'lblDir
        '
        Me.lblDir.AutoSize = True
        Me.lblDir.Location = New System.Drawing.Point(14, 146)
        Me.lblDir.Name = "lblDir"
        Me.lblDir.Text = "Save Directory:"
        '
        'txtFolder
        '
        Me.txtFolder.Location = New System.Drawing.Point(14, 164)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(360, 20)
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(380, 162)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(76, 23)
        Me.btnBrowse.Text = "Browse"
        '
        'lblDepth
        '
        Me.lblDepth.AutoSize = True
        Me.lblDepth.Location = New System.Drawing.Point(14, 198)
        Me.lblDepth.Name = "lblDepth"
        Me.lblDepth.Text = "Save Color Depth:"
        '
        'rb32
        '
        Me.rb32.AutoSize = True
        Me.rb32.Checked = True
        Me.rb32.Location = New System.Drawing.Point(120, 196)
        Me.rb32.Name = "rb32"
        Me.rb32.Size = New System.Drawing.Size(46, 17)
        Me.rb32.TabStop = True
        Me.rb32.Text = "32"
        '
        'rb24
        '
        Me.rb24.AutoSize = True
        Me.rb24.Location = New System.Drawing.Point(170, 196)
        Me.rb24.Name = "rb24"
        Me.rb24.Size = New System.Drawing.Size(46, 17)
        Me.rb24.Text = "24"
        '
        'rb16
        '
        Me.rb16.AutoSize = True
        Me.rb16.Location = New System.Drawing.Point(220, 196)
        Me.rb16.Name = "rb16"
        Me.rb16.Size = New System.Drawing.Size(46, 17)
        Me.rb16.Text = "16"
        '
        'rb8
        '
        Me.rb8.AutoSize = True
        Me.rb8.Location = New System.Drawing.Point(270, 196)
        Me.rb8.Name = "rb8"
        Me.rb8.Size = New System.Drawing.Size(70, 17)
        Me.rb8.Text = "8 or 1"
        '
        'chkAdjust
        '
        Me.chkAdjust.AutoSize = True
        Me.chkAdjust.Location = New System.Drawing.Point(14, 222)
        Me.chkAdjust.Name = "chkAdjust"
        Me.chkAdjust.Text = "Adjust frames to upper-left corner"
        '
        'bar
        '
        Me.bar.Location = New System.Drawing.Point(14, 248)
        Me.bar.Name = "bar"
        Me.bar.Size = New System.Drawing.Size(442, 16)
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(14, 268)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Text = "Ready"
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(300, 266)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 23)
        Me.btnStart.Text = "Start"
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(381, 266)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.Text = "Cancel"
        '
        'fBatchSave
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(470, 300)
        Me.Controls.Add(Me.rbImages)
        Me.Controls.Add(Me.rbFrames)
        Me.Controls.Add(Me.rbGroups)
        Me.Controls.Add(Me.lblImagesN)
        Me.Controls.Add(Me.lblPicturesN)
        Me.Controls.Add(Me.lblFramesN)
        Me.Controls.Add(Me.lblGroupsN)
        Me.Controls.Add(Me.lblRange)
        Me.Controls.Add(Me.txtRange)
        Me.Controls.Add(Me.lblDir)
        Me.Controls.Add(Me.txtFolder)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.lblDepth)
        Me.Controls.Add(Me.rb32)
        Me.Controls.Add(Me.rb24)
        Me.Controls.Add(Me.rb16)
        Me.Controls.Add(Me.rb8)
        Me.Controls.Add(Me.chkAdjust)
        Me.Controls.Add(Me.bar)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "fBatchSave"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Batch Save Objects"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents rbImages As RadioButton
    Friend WithEvents rbFrames As RadioButton
    Friend WithEvents rbGroups As RadioButton
    Friend WithEvents lblImagesN As Label
    Friend WithEvents lblPicturesN As Label
    Friend WithEvents lblFramesN As Label
    Friend WithEvents lblGroupsN As Label
    Friend WithEvents lblRange As Label
    Friend WithEvents txtRange As TextBox
    Friend WithEvents lblDir As Label
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents btnBrowse As Button
    Friend WithEvents lblDepth As Label
    Friend WithEvents rb32 As RadioButton
    Friend WithEvents rb24 As RadioButton
    Friend WithEvents rb16 As RadioButton
    Friend WithEvents rb8 As RadioButton
    Friend WithEvents chkAdjust As CheckBox
    Friend WithEvents bar As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents btnStart As Button
    Friend WithEvents btnClose As Button
End Class
