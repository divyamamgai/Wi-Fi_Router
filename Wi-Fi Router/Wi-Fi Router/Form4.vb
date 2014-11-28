Public Class Form4
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(340, 112 + 29 * GetNumberOfClients())
        ShowClients()
        Dim DoneButton As New Button
        With DoneButton
            .Location = New Point(237, 38 + 29 * GetNumberOfClients())
            .Size = New Size(75, 23)
            .Text = "Ok, Done!"
            .FlatStyle = FlatStyle.Standard
            .BackColor = Color.Empty
            .UseVisualStyleBackColor = True
        End With
        AddHandler DoneButton.Click, AddressOf DoneButton_Click
        Me.Controls.Add(DoneButton)
        Form1.Button9.Text = "Show Clients"
    End Sub
    Private Sub DoneButton_Click(sender As Object, e As EventArgs)
        Form1.Button9.Enabled = True
        Me.Close()
    End Sub
End Class