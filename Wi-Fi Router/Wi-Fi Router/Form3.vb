Public Class Form3
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RichTextBox1.ForeColor = Color.Black
        Label1.Text = "Program Run - " + Convert.ToString(Form1.RunCount) + " Time(s)"
        Label2.Text = "Program Version - " & Application.ProductVersion
    End Sub
End Class