Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' About dialog, reproducing the original op2art Viewer's about box and crediting
''' both this re-creation and the original author.
''' </summary>
Public Class fAbout
    Inherits Form

    Public Sub New()
        Me.Text = "About OP2 Art Viewer"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(380, 160)
        Me.ShowInTaskbar = False

        Dim lblTitle As New Label With {
            .Text = "OP2 Art Viewer",
            .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
            .Location = New Point(20, 18),
            .AutoSize = True
        }
        Me.Controls.Add(lblTitle)

        Dim lblBody As New Label With {
            .Text = "A re-creation of the original op2art-Viewer 3.0." & vbCrLf &
                    "Original Copyright (C) 2005 by Cynex." & vbCrLf & vbCrLf &
                    "Outpost 2: Divided Destiny art file viewer." & vbCrLf &
                    "Reads OP2_ART.BMP + op2_art.prt.",
            .Location = New Point(22, 50),
            .AutoSize = True
        }
        Me.Controls.Add(lblBody)

        Dim btnOk As New Button With {
            .Text = "OK",
            .DialogResult = DialogResult.OK,
            .Location = New Point(285, 125),
            .Width = 80
        }
        Me.Controls.Add(btnOk)
        Me.AcceptButton = btnOk
    End Sub
End Class
