Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Text.RegularExpressions

Public Class Form1
    <Serializable()>
    Private Structure Data
        Dim RunCount As Integer
        Dim SetupCount As Integer
        Dim UserName As String
        Dim PassWord As String
        Dim TimerEnabled As Integer
        Dim TimerInterval As Integer
        Dim TimerUnit As Integer
    End Structure
    Dim SetupCount As Integer = 0
    Public RunCount As Integer = 0
    Dim RunCountFlag As Integer = 0
    Dim TimerEnableFlag As Integer
    Dim NetworkStatusControls() As TextBox
    Dim ProgramPath As String = System.IO.Directory.GetCurrentDirectory()
    Dim DataFilePath As String = ProgramPath + "\resources\data.bin"
    Dim DataVar As Data
    Private Function UpdateDataFile() As Integer
        DataVar = New Data With {.RunCount = RunCount, .SetupCount = SetupCount, .UserName = TextBox1.Text, .PassWord = Encode(TextBox2.Text), .TimerEnabled = CheckBox1.CheckState, .TimerInterval = ComboBox1.Text, .TimerUnit = ComboBox2.SelectedIndex}
        Dim UpdateFormatter As BinaryFormatter = New BinaryFormatter()
        Dim DataFileStream As FileStream
        DataFileStream = File.Open(DataFilePath, FileMode.OpenOrCreate, FileAccess.Write)
        DataFileStream.Flush()
        UpdateFormatter.Serialize(DataFileStream, DataVar)
        DataFileStream.Close()
        UpdateDataFile = 1
    End Function
    Private Function UpdateTimeInterval() As Integer
        Dim TimerInterval As Integer
        Dim TimerIntervalValue As Integer = ComboBox1.Text
        Dim TimerIntervalUnit As Integer = ComboBox2.SelectedIndex
        TimerInterval = TimerIntervalValue * (60 ^ TimerIntervalUnit) * 1000
        Timer1.Interval = TimerInterval
        UpdateTimeInterval = 1
    End Function
    Private Function LoadForm(ByVal LogText As String) As Integer
        Dim InputFormatter As BinaryFormatter = New BinaryFormatter()
        Dim DataFileStream As FileStream
        If Not File.Exists(DataFilePath) Then
            DataVar = New Data With {.RunCount = 0, .SetupCount = 0, .UserName = "", .PassWord = "", .TimerEnabled = 0, .TimerInterval = 5, .TimerUnit = 0}
            TextBox1.Text = ""
            TextBox2.Text = ""
            ComboBox1.Text = 5
            ComboBox2.SelectedIndex = 0
            CheckBox1.CheckState = 0
        Else
            DataFileStream = File.Open(DataFilePath, FileMode.Open, FileAccess.Read)
            DataVar = InputFormatter.Deserialize(DataFileStream)
            RunCount = DataVar.RunCount
            SetupCount = DataVar.SetupCount
            TextBox1.Text = DataVar.UserName
            TextBox2.Text = Decode(DataVar.PassWord)
            ComboBox1.Text = DataVar.TimerInterval
            ComboBox2.SelectedIndex = DataVar.TimerUnit
            CheckBox1.CheckState = DataVar.TimerEnabled
            DataFileStream.Close()
        End If
        If RunCountFlag = 0 Then
            RunCount = RunCount + 1
            RunCountFlag = 1
            UpdateDataFile()
        End If
        MyBase.Show()
        RichTextBox1.Focus()
        If CheckBox1.CheckState = 0 Then
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False
            Button8.Enabled = False
            Label18.Enabled = False
            Label17.Enabled = False
        Else
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True
            Button8.Enabled = True
            Label18.Enabled = True
            Label17.Enabled = True
        End If
        UpdateTimeInterval()
        TimerEnableFlag = CheckBox1.CheckState
        If IsStarted() = 1 Then
            Button1.Enabled = False
            Button1.Text = "Started"
            Button3.Enabled = False
            TextBox1.Enabled = False
            TextBox2.Enabled = False
            PictureBox1.Image = Wi_Fi_Router.My.Resources.Resources.running
            If TimerEnableFlag = 1 Then
                Timer1.Start()
            End If

            Button2.Enabled = True
            Button2.Text = "Stop"
            TextBox8.Show()
            TextBox9.Show()
            TextBox10.Show()
            TextBox12.Show()
            Label11.Show()
            Label12.Show()
            Label13.Show()
            Label16.Show()
            If GetNumberOfClients() <> 0 Then
                Button9.Show()
            Else
                Button9.Hide()
            End If
        Else
            Button2.Enabled = False
            Button2.Text = "Stopped"
            PictureBox1.Image = Wi_Fi_Router.My.Resources.Resources.stopped
            If TimerEnableFlag = 1 Then
                Timer1.Stop()
            End If
            TextBox8.Hide()
            TextBox9.Hide()
            TextBox10.Hide()
            TextBox12.Hide()
            Label11.Hide()
            Label12.Hide()
            Label13.Hide()
            Label16.Hide()
            Button9.Hide()

            Button1.Enabled = True
            Button1.Text = "Start"
            Button3.Enabled = True
            TextBox1.Enabled = True
            TextBox2.Enabled = True
        End If
        NetworkStatusControls = {TextBox3, TextBox4, TextBox7, TextBox5, TextBox6, TextBox11, TextBox10, TextBox9, TextBox8, TextBox12}
        SetNetworkStatus(NetworkStatusControls)
        WrtieLog(RichTextBox1, LogText)
        LoadForm = 1
    End Function
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form3.Show()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Form2.Show()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Button3.Text = "Loading..."
        Button3.Enabled = False
        If ((TextBox1.Text = "") And (TextBox2.Text <> "")) Then
            Dim AboutMsg = "Please provide a User Name!"
            Dim AboutTitle = "Wi-Fi Router - Alert!"
            MsgBox(AboutMsg, , AboutTitle)
            Button3.Text = "Setup"
            Button3.Enabled = True
            LoadForm("Network Setup Aborted - No User Name")
        ElseIf ((TextBox1.Text <> "") And (TextBox2.Text = "")) Then
            Dim AboutMsg = "Please provide a Password!"
            Dim AboutTitle = "Wi-Fi Router - Alert!"
            MsgBox(AboutMsg, , AboutTitle)
            Button3.Text = "Setup"
            Button3.Enabled = True
            LoadForm("Network Setup Aborted - No Password")
        ElseIf ((TextBox1.Text = "") And (TextBox2.Text = "")) Then
            Dim AboutMsg = "Please provide a User Name and Password!"
            Dim AboutTitle = "Wi-Fi Router - Alert!"
            MsgBox(AboutMsg, , AboutTitle)
            Button3.Text = "Setup"
            Button3.Enabled = True
            LoadForm("Network Setup Aborted - No User Name and Password")
        Else
            If Len(TextBox1.Text) < 8 Or Len(TextBox2.Text) > 63 Then
                Dim AboutMsg = "Please provide Password with minimum 8 characters and maximum 63 characters!"
                Dim AboutTitle = "Wi-Fi Router - Alert!"
                MsgBox(AboutMsg, , AboutTitle)
                Button3.Text = "Setup"
                Button3.Enabled = True
                LoadForm("Network Setup Aborted - Invalid Password")
            Else
                Dim SetupNetworkProcessOutPut As String
                Dim SetupNetworkProcess As Process = New Process
                With SetupNetworkProcess
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.RedirectStandardOutput = True
                    .StartInfo.UseShellExecute = False
                    .StartInfo.FileName = "cmd.exe"
                    .StartInfo.Arguments = "/c netsh wlan set hostednetwork mode=allow  ssid=""" + TextBox1.Text + """ key=""" + TextBox2.Text + """  keyUsage=persistent"
                    .Start()
                    SetupNetworkProcessOutPut = .StandardOutput.ReadToEnd
                    .WaitForExit()
                End With
                If SetupNetworkProcessOutPut.Contains("The hosted network mode has been set to allow.") Then
                    Button3.Text = "Setup"
                    Button3.Enabled = True
                    SetupCount = SetupCount + 1
                    UpdateDataFile()
                    LoadForm("Network Setup Complete")
                    If SetupCount = 1 Then
                        Dim FirstSetupHelp As Integer = MessageBox.Show("It looks like you have setup the connection for the first time!" & vbCrLf & "Do you want to know What To Do Next?", "Launch What To Do?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        If FirstSetupHelp = DialogResult.No Then
                            MessageBox.Show("You can launch ""What To Do?"" from the main screen anytime if you want.")
                        ElseIf FirstSetupHelp = DialogResult.Yes Then
                            Form2.Show()
                        End If
                    End If
                Else
                    MessageBox.Show(SetupNetworkProcessOutPut, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Button3.Text = "Setup"
                    Button3.Enabled = True
                    LoadForm("Network Setup Error - Internal Error!")
                End If
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Text = "Loading..."
        Button1.Enabled = False
        Dim StartNetworkProcessOutPut As String
        Dim StartNetworkProcess As Process = New Process
        With StartNetworkProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan start hostednetwork"
            .Start()
            StartNetworkProcessOutPut = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        If StartNetworkProcessOutPut.Contains("The hosted network started.") Then
            Button1.Text = "Started"
            Button1.Enabled = False
            LoadForm("Network Started")
        Else
            Dim ErrorMsg As String = StartNetworkProcessOutPut
            If StartNetworkProcessOutPut.Contains("The wireless local area network interface is powered down and doesn't support the requested operation.") Then
                ErrorMsg = ErrorMsg + "Please start Wi-Fi of your machine."
            End If
            MessageBox.Show(ErrorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Button1.Text = "Start"
            Button1.Enabled = True
            LoadForm("Network Start Error - Internal Error!")
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button2.Text = "Loading..."
        Button2.Enabled = False
        Dim StopNetworkProcessOutPut As String
        Dim StopNetworkProcess As Process = New Process
        With StopNetworkProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan stop hostednetwork"
            .Start()
            StopNetworkProcessOutPut = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        If StopNetworkProcessOutPut.Contains("The hosted network stopped.") Then
            Button2.Text = "Stopped"
            Button2.Enabled = False
            LoadForm("Network Stopped")
        Else
            MessageBox.Show(StopNetworkProcessOutPut, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Button2.Text = "Stop"
            Button2.Enabled = True
            LoadForm("Network Stop Error - Internal Error!")
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IsSupported() = 1 Then
            TimerEnableFlag = 0
            RichTextBox1.ForeColor = Color.Black
            LoadForm("Main Form Loaded")
        Else
            MessageBox.Show("Your Wi-Fi adapter doesn't support Hosted Network functionality. Program will now terminate.", "Wi-Fi Router", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Me.Close()
            Application.Exit()
        End If
    End Sub

    Private Sub Form1_Leave(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If IsStarted() = 1 Then
            If e.CloseReason <> CloseReason.TaskManagerClosing AndAlso e.CloseReason <> CloseReason.WindowsShutDown Then
                e.Cancel = MessageBox.Show("Do you really want to exit? Wi-Fi Router is still connected. Closing this program will not disconnect your Wi-Fi router.", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No
            End If
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim PassWordChar As String = TextBox2.PasswordChar
        If PassWordChar = "•" Then
            TextBox2.PasswordChar = ""
            Button7.Text = "Hide"
        Else
            TextBox2.PasswordChar = "•"
            Button7.Text = "Show"
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        LoadForm("Status Updated")
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If TimerEnableFlag = 1 And IsStarted() = 1 Then
            SetNetworkStatus(NetworkStatusControls)
            If GetNumberOfClients() <> 0 Then
                Button9.Show()
            Else
                Button9.Hide()
            End If
            WrtieLog(RichTextBox1, "Auto Status Update")
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.CheckState = 0 Then
            WrtieLog(RichTextBox1, "Auto Status Updater Stopped")
        Else
            WrtieLog(RichTextBox1, "Auto Status Updater Started")
        End If
    End Sub

    Private Sub CheckBox1_Click(sender As Object, e As EventArgs) Handles CheckBox1.Click
        UpdateDataFile()
        If CheckBox1.CheckState = 0 Then
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False
            Button8.Enabled = False
            Label18.Enabled = False
            Label17.Enabled = False
            Timer1.Stop()
        Else
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True
            Button8.Enabled = True
            Label18.Enabled = True
            Label17.Enabled = True
            Timer1.Start()
        End If
        TimerEnableFlag = CheckBox1.CheckState
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If ComboBox1.Text <> "" Then
            If Regex.IsMatch(ComboBox1.Text, "^[0-9 ]+$") Then
                If ComboBox1.Text = "0" Then
                    MessageBox.Show("Time interval value cannot be 0!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ComboBox1.Text = DataVar.TimerInterval
                    ComboBox2.SelectedIndex = DataVar.TimerUnit
                    WrtieLog(RichTextBox1, "Auto Status Updater Interval Change Error - Zero Value")
                ElseIf (Integer.Parse(ComboBox1.Text) < 5 And ComboBox2.SelectedIndex = 0) Then
                    MessageBox.Show("Time interval value cannot be less than 5!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ComboBox1.Text = DataVar.TimerInterval
                    ComboBox2.SelectedIndex = DataVar.TimerUnit
                    WrtieLog(RichTextBox1, "Auto Status Updater Interval Change Error - Invalid Value")
                Else
                    UpdateDataFile()
                    UpdateTimeInterval()
                    WrtieLog(RichTextBox1, "Auto Status Updater Interval Changed - " + ComboBox1.Text + " " + ComboBox2.Text)
                End If
            Else
                MessageBox.Show("Please enter a valid numeric time interval value!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ComboBox1.Text = DataVar.TimerInterval
                ComboBox2.SelectedIndex = DataVar.TimerUnit
                WrtieLog(RichTextBox1, "Auto Status Updater Interval Change Error - Non-Numeric Value")
            End If
        Else
            MessageBox.Show("Please enter a valid time interval value!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ComboBox1.Text = DataVar.TimerInterval
            ComboBox2.SelectedIndex = DataVar.TimerUnit
            WrtieLog(RichTextBox1, "Auto Status Updater Interval Change Error - No Value")
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Button9.Text = "Loading..."
        Button9.Enabled = False
        Form4.Show()
    End Sub
End Class
