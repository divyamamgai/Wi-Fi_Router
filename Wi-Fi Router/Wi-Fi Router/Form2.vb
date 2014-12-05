Imports System.Drawing.Text
Public Class Form2
    Dim ProgramPath As String = System.IO.Directory.GetCurrentDirectory()
    Dim CurrentStep As Integer
    Dim StepStrings() As String
    Dim StepImages() As Image
    Private Function NextButton() As Integer
        CurrentStep = CurrentStep + 1
        If CurrentStep > 7 Then
            Label1.Hide()
            Label2.Hide()
            Label3.Hide()
            PictureBox1.Hide()
            PictureBox2.Hide()
            PictureBox3.Hide()
            PictureBox4.Hide()
            Button1.Hide()
            Button2.Hide()
            Button3.Show()
            Label4.Show()
        Else
            Label2.Text = "Step " + Convert.ToString(CurrentStep) + " :"
            Label3.Text = StepStrings(CurrentStep - 1)
            PictureBox1.BackgroundImage = StepImages(CurrentStep - 1)
        End If
        If CurrentStep > 1 Then
            Button2.Enabled = True
        End If
        NextButton = 1
    End Function
    Private Function PreviousButton() As Integer
        CurrentStep = CurrentStep - 1
        Label2.Text = "Step " + Convert.ToString(CurrentStep) + " :"
        Label3.Text = StepStrings(CurrentStep - 1)
        PictureBox1.BackgroundImage = StepImages(CurrentStep - 1)
        If CurrentStep = 1 Then
            Button2.Enabled = False
        End If
        PreviousButton = 1
    End Function
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim CustomFontRes As New PrivateFontCollection
        'CustomFontRes.AddFontFile(ProgramPath + "\resources\Rockwell.ttf")
        'Dim CustomFontBoldRes As New PrivateFontCollection
        'CustomFontBoldRes.AddFontFile(ProgramPath + "\resources\RockwellBold.ttf")
        'Dim CustomFont28B As Font = New Font(CustomFontBoldRes.Families(0), 28, FontStyle.Bold)
        'Dim CustomFont14B As Font = New Font(CustomFontBoldRes.Families(0), 14, FontStyle.Bold)
        'Dim CustomFont14R As Font = New Font(CustomFontRes.Families(0), 14, FontStyle.Regular)
        'Dim CustomFont12B As Font = New Font(CustomFontBoldRes.Families(0), 12, FontStyle.Bold)
        'Label1.Font = CustomFont28B
        'Label2.Font = CustomFont14B
        'Label3.Font = CustomFont14R
        'Label4.Font = CustomFont28B
        'Button1.Font = CustomFont12B
        'Button2.Font = CustomFont12B
        'Button3.Font = CustomFont12B
        StepStrings = {
            "Right-Click the Network Icon at the bottom bar.",
            "Left-Click ""Open Network and Sharing Center"".",
            "Left-Click ""Change Adapter Settings"" on the left pane of the window opened.",
            "Right-Click on the connection through which you connect to internet and Left-Click on ""Properties"".",
            "Swtich to ""Sharing"" tab in the connection properties.",
            "Tick the first checkbox as shown in the image below.",
            "Select the newly created connection, usually of the type - ""Local Area Connection* #""."
        }
        StepImages = {
            Wi_Fi_Router.My.Resources.Resources.Step_1,
            Wi_Fi_Router.My.Resources.Resources.Step_2,
            Wi_Fi_Router.My.Resources.Resources.Step_3,
            Wi_Fi_Router.My.Resources.Resources.Step_4,
            Wi_Fi_Router.My.Resources.Resources.Step_5,
            Wi_Fi_Router.My.Resources.Resources.Step_6,
            Wi_Fi_Router.My.Resources.Resources.Step_7
        }
        Label3.Text = StepStrings(0)
        Button2.Enabled = False
        Button3.Hide()
        Label4.Hide()
        CurrentStep = 1
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        NextButton()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        PreviousButton()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
End Class