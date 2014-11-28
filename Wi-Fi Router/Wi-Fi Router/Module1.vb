Module Module1
    Public Function WrtieLog(TextBoxObject As Object, ByVal InputString As String) As Integer
        If TextBoxObject.Text = "" Then
            TextBoxObject.Text = "[" + TimeOfDay.ToString("h:mm:ss tt") + "] " + InputString
        Else
            TextBoxObject.Text = "[" + TimeOfDay.ToString("h:mm:ss tt") + "] " + InputString & vbCrLf & TextBoxObject.Text
        End If
        WrtieLog = 1
    End Function
    Public Function Encode(ByVal InputString As String) As String
        Dim EncodeSystem As New System.Text.UnicodeEncoding
        Dim EncodeBuffer As Byte() = EncodeSystem.GetBytes(InputString)
        Encode = Convert.ToBase64String(EncodeBuffer)
    End Function
    Public Function Decode(ByVal InputString As String) As String
        Dim EncodeSystem As New System.Text.UnicodeEncoding
        Dim EncodeBuffer As Byte() = Convert.FromBase64String(InputString)
        Decode = EncodeSystem.GetString(EncodeBuffer)
    End Function
    Public Function IsStarted() As Integer
        Dim IsStartedFlag As Integer = 0
        Dim HostedNetworkStatus As String
        Dim IsStartedProcess As Process = New Process
        With IsStartedProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan show hostednetwork"
            .Start()
            HostedNetworkStatus = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        For Each HostedNetworkStatusLine As String In HostedNetworkStatus.Split(vbLf)
            If HostedNetworkStatusLine.Contains("Status                 : Started") Then
                IsStartedFlag = 1
                Exit For
            End If
        Next
        IsStarted = IsStartedFlag
    End Function
    Public Function IsSupported() As Integer
        Dim IsSupportedFlag As Integer = 0
        Dim NetworkDriverOutput As String
        Dim IsSupportedProcess As Process = New Process
        With IsSupportedProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan show driver"
            .Start()
            NetworkDriverOutput = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        For Each NetworkDriverOutputLine As String In NetworkDriverOutput.Split(vbLf)
            If NetworkDriverOutputLine.Contains("Hosted network supported  : Yes") Then
                IsSupportedFlag = 1
                Exit For
            End If
        Next
        IsSupported = IsSupportedFlag
    End Function
    Public Function GetNumberOfClients() As Integer
        Dim NumberOfClients As Integer
        If IsStarted() Then
            Dim Line As Integer = 1
            Dim LineMax As Integer = 16
            Dim GetNumberOfClientsString As String
            Dim GetNumberOfClientsProcess As Process = New Process
            With GetNumberOfClientsProcess
                .StartInfo.CreateNoWindow = True
                .StartInfo.RedirectStandardOutput = True
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = "cmd.exe"
                .StartInfo.Arguments = "/c netsh wlan show hostednetwork"
                .Start()
                GetNumberOfClientsString = .StandardOutput.ReadToEnd
                .WaitForExit()
            End With
            For Each SetNetworkStatusStringLine As String In GetNumberOfClientsString.Split(vbLf)
                If Line > LineMax Then
                    Exit For
                End If
                If Line = 16 Then
                    NumberOfClients = Integer.Parse(SetNetworkStatusStringLine.Substring(SetNetworkStatusStringLine.IndexOf(":") + 2, SetNetworkStatusStringLine.Length - SetNetworkStatusStringLine.IndexOf(":") - 2).Replace("""", ""))
                    Exit For
                End If
                Line = Line + 1
            Next
        Else
            NumberOfClients = 0
        End If
        GetNumberOfClients = NumberOfClients
    End Function
    Public Function GetClient(ByVal ClientNumber As Integer) As String
        Dim ClientString As String = ""
        Dim Index As Integer = 0
        Dim Line As Integer = 1
        Dim LineMax As Integer = 16
        Dim GetClientString As String
        Dim GetClientProcess As Process = New Process
        With GetClientProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan show hostednetwork"
            .Start()
            GetClientString = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        For Each GetClientStringLine As String In GetClientString.Split(vbLf)
            If Line > LineMax Then
                Index = Index + 1
                If Index > GetNumberOfClients() Then
                    Exit For
                Else
                    If Index = ClientNumber Then
                        ClientString = GetClientStringLine.Replace(" ", "")
                        ClientString = ClientString.Replace("Authenticated", "")
                        ClientString = ClientString.ToUpper
                    End If
                End If
            End If
            Line = Line + 1
        Next
        GetClient = ClientString
    End Function
    Public Function ShowClients() As Integer
        Dim Index As Integer = 0
        For Index = 0 To GetNumberOfClients() - 1
            Dim ClientButton As New Button
            With ClientButton
                .Size = New Size(299, 23)
                .Location = New Point(13, 38 + 29 * Index)
                .Text = GetClient(Index + 1)
                .FlatStyle = FlatStyle.Standard
                .BackColor = Color.Empty
                .UseVisualStyleBackColor = True
            End With
            Form4.Controls.Add(ClientButton)
        Next
        ShowClients = 1
    End Function
    Public Function SetNetworkStatus(ByVal NetworkStatusControls() As TextBox) As Integer
        Dim SetNetworkStatusString As String
        Dim Index As Integer = 0
        Dim Line As Integer = 1
        Dim LineMax As Integer = 0
        Dim SetNetworkStatusProcess As Process = New Process
        With SetNetworkStatusProcess
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = "/c netsh wlan show hostednetwork"
            .Start()
            SetNetworkStatusString = .StandardOutput.ReadToEnd
            .WaitForExit()
        End With
        If IsStarted() = 1 Then
            LineMax = 16
        Else
            LineMax = 12
        End If
        For Count = 0 To NetworkStatusControls.GetUpperBound(0)
            NetworkStatusControls(Count).Text = "NULL"
        Next
        For Each SetNetworkStatusStringLine As String In SetNetworkStatusString.Split(vbLf)
            If Line > LineMax Then
                Exit For
            End If
            If (Line = 1 OrElse Line = 2 OrElse Line = 3 OrElse Line = 9 OrElse Line = 10 OrElse Line = 11) Then
                Line = Line + 1
                Continue For
            Else
                If Index > NetworkStatusControls.GetUpperBound(0) Then
                    Exit For
                Else
                    NetworkStatusControls(Index).Text = SetNetworkStatusStringLine.Substring(SetNetworkStatusStringLine.IndexOf(":") + 2, SetNetworkStatusStringLine.Length - SetNetworkStatusStringLine.IndexOf(":") - 2).Replace("""", "")
                    Index = Index + 1
                End If
            End If
            Line = Line + 1
        Next
        SetNetworkStatus = 1
    End Function
End Module