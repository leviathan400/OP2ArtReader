Imports System.IO
Imports System.Text
Imports System.Globalization

''' <summary>
''' Minimal INI reader/writer, used to persist settings in the same spirit as the
''' original op2art.ini (sections: Main / Art Select / Batch Save).
''' Section and key lookups are case-insensitive.
''' </summary>
Public Class CIni

    Private ReadOnly sections As New Dictionary(Of String, Dictionary(Of String, String))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly path As String

    Public Sub New(filePath As String)
        path = filePath
        Load()
    End Sub

    Private Sub Load()
        Try
            If Not File.Exists(path) Then Return
            Dim current As String = ""
            For Each rawLine In File.ReadAllLines(path)
                Dim line = rawLine.Trim()
                If line.Length = 0 OrElse line.StartsWith(";") OrElse line.StartsWith("#") Then Continue For
                If line.StartsWith("[") AndAlso line.EndsWith("]") Then
                    current = line.Substring(1, line.Length - 2).Trim()
                    If Not sections.ContainsKey(current) Then sections(current) = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                Else
                    Dim eq = line.IndexOf("="c)
                    If eq > 0 AndAlso current.Length > 0 Then
                        Dim k = line.Substring(0, eq).Trim()
                        Dim v = line.Substring(eq + 1).Trim()
                        sections(current)(k) = v
                    End If
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("CIni load error: " & ex.Message)
        End Try
    End Sub

    Public Sub Save()
        Try
            Dim sb As New StringBuilder()
            For Each sec In sections
                sb.AppendLine($"[{sec.Key}]")
                For Each kv In sec.Value
                    sb.AppendLine($"{kv.Key}={kv.Value}")
                Next
                sb.AppendLine()
            Next
            File.WriteAllText(path, sb.ToString())
        Catch ex As Exception
            Debug.WriteLine("CIni save error: " & ex.Message)
        End Try
    End Sub

    Public Function GetString(section As String, key As String, Optional def As String = "") As String
        Dim d As Dictionary(Of String, String) = Nothing
        If sections.TryGetValue(section, d) Then
            Dim v As String = Nothing
            If d.TryGetValue(key, v) Then Return v
        End If
        Return def
    End Function

    Public Function GetInt(section As String, key As String, Optional def As Integer = 0) As Integer
        Dim v As Integer
        If Integer.TryParse(GetString(section, key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, v) Then Return v
        Return def
    End Function

    Public Function GetBool(section As String, key As String, Optional def As Boolean = False) As Boolean
        Dim s = GetString(section, key, "").Trim()
        If s = "1" OrElse s.Equals("true", StringComparison.OrdinalIgnoreCase) Then Return True
        If s = "0" OrElse s.Equals("false", StringComparison.OrdinalIgnoreCase) Then Return False
        Return def
    End Function

    Public Sub SetValue(section As String, key As String, value As String)
        Dim d As Dictionary(Of String, String) = Nothing
        If Not sections.TryGetValue(section, d) Then
            d = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            sections(section) = d
        End If
        d(key) = value
    End Sub

    Public Sub SetValue(section As String, key As String, value As Integer)
        SetValue(section, key, value.ToString(CultureInfo.InvariantCulture))
    End Sub

    Public Sub SetValue(section As String, key As String, value As Boolean)
        SetValue(section, key, If(value, "1", "0"))
    End Sub
End Class
