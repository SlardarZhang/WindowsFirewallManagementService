Imports System.IO
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Security
Imports System.Net.Sockets
Imports System.Security.Authentication
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Text.RegularExpressions
Imports System.Threading
Imports DotRas
Imports Microsoft.Win32
Imports NetFwTypeLib

Public Class WindowsFirewallManagementService
    Private isServer As Boolean
    Private Serverlog As EventLog
    Private LastUpdate As Date
    Private cert As X509Certificate2
    Private Port As Integer
    Private ServerListener As TcpListener
    Private ServerLoopThread As Thread = Nothing
    Private Running As Boolean = True
    Private ClientThreadPool As ArrayList
    Private RemoteIP As IPAddress

    Public Sub StartCmd(ByVal args() As String)
        OnStart(args)
        Dim trying As Integer = 0
        Console.ForegroundColor = ConsoleColor.Red
        While (Not SyncFireWall(True))
            trying = trying + 1
            Console.WriteLine("Try:" + trying.ToString)
        End While
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("SUCCESS")
        Console.ReadKey()
    End Sub

    Protected Overrides Sub OnStart(ByVal args() As String)
        Dim PingTester As New Ping
        While (PingTester IsNot Nothing)
            Try
                If PingTester.Send("8.8.8.8").Status = IPStatus.Success Then
                    PingTester = Nothing
                End If
            Catch ex As Exception
            End Try
        End While
        Try
            If Not EventLog.SourceExists("Windows Firewall Management Service") Then
                EventLog.CreateEventSource("Windows Firewall Management Service", "WFM Service")
            End If
            Serverlog = New EventLog()
            Serverlog.Source = "Windows Firewall Management Service"
            LastUpdate = New DateTime(0)

            isServer = LoadConfig("isServer")
            Dim MyKey As String = GetMachineID.GetMachineID
            Dim AES256 As MyAES256 = New MyAES256(MyKey)

            cert = New X509Certificate2(Convert.FromBase64String(AES256.Decrypt((LoadConfig("Cert")))))
            Dim Params As CspParameters = New CspParameters()
            Params.KeyContainerName = "KeyContainer"
            Dim PrivateKey As RSACryptoServiceProvider = New RSACryptoServiceProvider(Params)
            PrivateKey.FromXmlString(AES256.Decrypt(LoadConfig("Key")))
            cert.PrivateKey = PrivateKey

            If Not cert.HasPrivateKey Then
                Throw New Exception("Load Private Key failed")
            End If

            Port = Integer.Parse(CType(LoadConfig("Port"), String))
            Prepare(True)
            Serverlog.WriteEntry("Windows Firewall Management service started successfully.", EventLogEntryType.Information)
        Catch ex As Exception
            Serverlog.WriteEntry("Windows Firewall Management service start failed." + vbCrLf + ex.Message, EventLogEntryType.Error)
            Environment.Exit(1)
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Running = False
        If ServerLoopThread IsNot Nothing Then
            Try
                ServerLoopThread.Interrupt()
                ServerLoopThread.Join(1)
                ServerLoopThread.Abort()
                ServerLoopThread = Nothing
            Catch
            End Try
        End If

        'If MySqlConnection IsNot Nothing Then
        'MySqlConnection.Close()
        'End If
        Serverlog.WriteEntry("Windows Firewall Management service stopped successfully.", EventLogEntryType.Information)


    End Sub

    Private Function LoadConfig(ByVal Key As String) As Object
        Dim Reg As RegistryKey
        If Environment.Is64BitOperatingSystem Then
            Reg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Slardar\WFMService", False)
        Else
            Reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\Slardar\WFMService", False)
        End If
        Dim ReturnObj As Object
        Try
            If Reg Is Nothing Then
                Registry.LocalMachine.CreateSubKey("SOFTWARE\Slardar\WFMService")
                ReturnObj = Nothing
            Else
                If Reg.GetValue(Key) Is Nothing Then
                    ReturnObj = Nothing
                Else
                    ReturnObj = Reg.GetValue(Key)
                End If
            End If
        Catch ex As Exception
            ReturnObj = Nothing
        End Try
        If Reg IsNot Nothing Then Reg.Close()
        Return ReturnObj
    End Function

    Private Sub Prepare(ByVal PublicIP As Boolean)
        If isServer Then
            Dim LocalAddress As IPAddress = Nothing

            For Each IPA As IPAddress In Array.FindAll(Dns.GetHostEntry(String.Empty).AddressList, Function(add) add.AddressFamily = AddressFamily.InterNetwork)
                If (isPublicIP(IPA.ToString()) Or (Not PublicIP)) And IPA.AddressFamily = AddressFamily.InterNetwork Then
                    LocalAddress = IPA
                    Exit For
                End If
            Next
            If LocalAddress Is Nothing Then
                Throw New Exception("No IP Address can be use!")
            Else
                ServerListener = New TcpListener(LocalAddress, Port)
                Running = True
                ServerLoopThread = New Thread(AddressOf HandleClient)
                ServerLoopThread.IsBackground = True
                ServerLoopThread.Start()
            End If
        Else
            Dim ServerAddStr As String = LoadConfig("ServerIP")
            If ServerAddStr Is Nothing Then
                Throw New Exception("Server Address is not configured.")
            End If
            If Not IPAddress.TryParse(ServerAddStr, RemoteIP) Then
                Dim ServerAddArray As IPAddress() = Dns.GetHostAddresses(ServerAddStr)
                For Each ServerAddress As IPAddress In ServerAddArray
                    If ServerAddress.AddressFamily = AddressFamily.InterNetwork And isPublicIP(ServerAddress.ToString) Then
                        RemoteIP = ServerAddress
                        Exit For
                    End If
                Next
                If RemoteIP Is Nothing Then
                    Throw New Exception("Unknow server address")
                End If
            End If

            Dim watcher As RasConnectionWatcher = New RasConnectionWatcher
            AddHandler watcher.Connected, Sub(sender As Object, EventArgs As RasConnectionEventArgs)
                                              Serverlog.WriteEntry("PPPoE reconnected, Syncing Firewall data.", EventLogEntryType.Information)
                                              Dim Wating As Integer = 0
                                              While isConnected() And Wating < 10
                                                  Thread.Sleep(500)
                                              End While
                                              If SyncFireWall(False) = False Then
                                                  LastUpdate = Now - New TimeSpan(0, 29, 55)
                                              Else
                                                  LastUpdate = Now
                                              End If
                                          End Sub
            watcher.EnableRaisingEvents = True

            ServerLoopThread = New Thread(Sub()
                                              While (Running)
                                                  If (Now - LastUpdate) >= New TimeSpan(0, 30, 0) Then
                                                      If SyncFireWall(False) = False Then
                                                          LastUpdate = Now - New TimeSpan(0, 29, 55)
                                                      Else
                                                          LastUpdate = Now
                                                      End If
                                                  End If
                                              End While
                                          End Sub)
            ServerLoopThread.IsBackground = True
            ServerLoopThread.Start()
        End If
    End Sub

    Private Sub HandleClient()
        ClientThreadPool = New ArrayList
        ServerListener.Start()
        While (Running)
            Try
                Dim Client As TcpClient = ServerListener.AcceptTcpClient
                Dim ProcessThread As Thread = New Thread(New ThreadStart(Sub()
                                                                             ProcessClient(Client)
                                                                             Try
                                                                                 ClientThreadPool.Remove(ProcessThread)
                                                                             Catch ex As Exception
                                                                             End Try

                                                                         End Sub))

                ClientThreadPool.Add(ProcessThread)
                ProcessThread.Start()

            Catch ex As Exception
                Serverlog.WriteEntry("Handle Client Error:" + vbCrLf + ex.Message, EventLogEntryType.Error)
            End Try
        End While
        For Each thread As Thread In ClientThreadPool
            Try
                thread.Interrupt()
                thread.Join(1)
                thread.Abort()
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub ProcessClient(client As TcpClient)
        Try
            Dim sslStream As New SslStream(client.GetStream, False, New RemoteCertificateValidationCallback(Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
                                                                                                                For Each status As X509ChainStatus In chain.ChainStatus
                                                                                                                    If (status.Status = X509ChainStatusFlags.RevocationStatusUnknown) Or (status.Status = X509ChainStatusFlags.OfflineRevocation) Then
                                                                                                                        Continue For
                                                                                                                    Else
                                                                                                                        Serverlog.WriteEntry(status.StatusInformation + vbCrLf + status.Status.ToString, EventLogEntryType.Error)
                                                                                                                        Return False
                                                                                                                    End If
                                                                                                                Next
                                                                                                                Return True
                                                                                                            End Function))
            sslStream.ReadTimeout = 10000
            sslStream.WriteTimeout = 10000
            sslStream.AuthenticateAsServer(cert, True, SslProtocols.Tls12, True)
            Dim SR As New StreamReader(sslStream)
            Dim SW As New StreamWriter(sslStream)
            SW.AutoFlush = True
            If sslStream.IsAuthenticated Then
                Dim RemoteDomainList As List(Of String) = GetDNSNames(New X509Certificate2(sslStream.RemoteCertificate.GetRawCertData))
                Dim LocalDomainList As List(Of String) = GetDNSNames(cert)

                Dim isTemp As Boolean = SR.ReadLine

                If isTemp Then
                    Serverlog.WriteEntry("Update Firewall temporary.IP:" + CType(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString, EventLogEntryType.SuccessAudit)
                    Dim RemoteIP As String = GetRemoteIP()
                    UpdateFireWall(CType(client.Client.RemoteEndPoint, IPEndPoint).Address, False, Nothing)
                    UpdateFireWall(IPAddress.Parse(RemoteIP), True, New TimeSpan(0, 10, 0))
                Else
                    Serverlog.WriteEntry("Update Firewall permanently.IP:" + CType(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString, EventLogEntryType.SuccessAudit)
                    UpdateFireWall(CType(client.Client.RemoteEndPoint, IPEndPoint).Address, False, Nothing)
                End If
                SW.WriteLine("OK")
            Else
                SW.WriteLine("Certification authenticated failed.")
                Throw New Exception("Certification authenticated failed.")
            End If

            Try
                sslStream.Close()
            Catch
            End Try
        Catch ex As Exception
            Serverlog.WriteEntry(ex.Message, EventLogEntryType.Error)
        End Try
        Try
            client.Close()
        Catch
        End Try
    End Sub

    Private Function isPublicIP(IPString As String) As Boolean
        Return Regex.IsMatch(IPString, "^([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))(?<!127)(?<!^10)(?<!^0)\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!192\.168)(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!\.255$)$")
    End Function

    Private Function ContainsDomain(DomainList As List(Of String), CheckingDomain As String) As Boolean
        For Each Domain As String In DomainList
            If Domain.IndexOf("*") = -1 Then
                If (Domain = CheckingDomain) Then
                    Return True
                End If
            Else
                Dim MCollection As MatchCollection = Regex.Matches(CheckingDomain, Domain.Replace(".", "\.").Replace("*", "."))
                If MCollection.Count = 1 Then
                    If MCollection.Item(0).Index = (CheckingDomain.Length - Domain.Length) Then
                        Return True
                    End If
                End If
            End If
        Next
        Return False
    End Function

    Private Function GetDomainArray(DomainList As List(Of String), CheckingDomain As String) As String()
        For Each Domain As String In DomainList
            If Domain.IndexOf("*") = -1 Then
                If (Domain = CheckingDomain) Then
                    Return New String() {Domain}
                End If
            Else
                Dim MCollection As MatchCollection = Regex.Matches(CheckingDomain, Domain.Replace(".", "\.").Replace("*", "."))
                If MCollection.Count = 1 Then
                    If MCollection.Item(0).Index = (CheckingDomain.Length - Domain.Length) Then
                        Dim MainDomain, SubDomain As String
                        MainDomain = CheckingDomain.Substring(CheckingDomain.Length - Domain.Length + 2)
                        SubDomain = CheckingDomain.Substring(0, CheckingDomain.Length - Domain.Length + 1)
                        Return New String() {MainDomain, SubDomain}
                    End If
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Function GetDNSNames(ByVal cert As X509Certificate2) As List(Of String)
        Dim ext As X509Extension = cert.Extensions("2.5.29.17")
        Dim DNSNames As New List(Of String)
        If ext IsNot Nothing Then
            For Each nvp As String In ext.Format(True).Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                DNSNames.Add(nvp.Replace("DNS Name=", ""))
            Next
        End If

        Return DNSNames
    End Function

    Private Sub UpdateFireWall(IPAddress As IPAddress, delay As Boolean, dalayTime As TimeSpan)
        If (delay) Then
            Dim updateFWThread As New Thread(Sub()
                                                 Thread.Sleep(dalayTime)
                                                 Dim RuleList As String() = My.Resources.Rules.Split(vbCrLf)
                                                 For Each Rule As String In RuleList
                                                     Try
                                                         Dim Policy As INetFwPolicy2
                                                         Policy = DirectCast(Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")), INetFwPolicy2)
                                                         Policy.Rules.Item(Rule.Replace(vbLf, "")).RemoteAddresses = IPAddress.ToString
                                                     Catch ex As Exception
                                                     End Try
                                                 Next
                                             End Sub)
            updateFWThread.Start()
        Else
            Dim RuleList As String() = My.Resources.Rules.Split(vbCrLf)
            For Each Rule As String In RuleList
                Try
                    Dim Policy As INetFwPolicy2
                    Policy = DirectCast(Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")), INetFwPolicy2)
                    Policy.Rules.Item(Rule.Replace(vbLf, "")).RemoteAddresses = IPAddress.ToString
                Catch ex As Exception
                End Try
            Next
        End If
    End Sub

    Private Function GetRemoteIP() As String

        Dim RuleList As String() = My.Resources.Rules.Split(vbCrLf)
        For Each Rule As String In RuleList
            Try
                Dim Policy As INetFwPolicy2
                Policy = DirectCast(Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")), INetFwPolicy2)
                Dim INetRule As INetFwRule = Policy.Rules.Item(Rule.Replace(vbLf, ""))
                If INetRule IsNot Nothing Then
                    IPAddress.Parse(INetRule.RemoteAddresses.Split("/")(0))
                    Return INetRule.RemoteAddresses.Split("/")(0)
                End If
            Catch ex As Exception
            End Try
        Next
        Return Nothing
    End Function

    Protected Friend Function SyncFireWall(isTemp As Boolean) As Boolean
        Try
            Dim tcpClient As New TcpClient(RemoteIP.ToString, Port)
            Dim sslStream As SslStream = New SslStream(tcpClient.GetStream,
                                                       False,
                    New RemoteCertificateValidationCallback(Function(sender As Object, certificate As X509Certificate2, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
                                                                If sslPolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch And chain.ChainStatus.Length = 0 Then
                                                                    If certificate.Extensions("2.5.29.14").Format(False) = cert.Extensions("2.5.29.35").Format(False).Replace("KeyID=", "") Then
                                                                        Return True
                                                                    Else
                                                                        Return False
                                                                    End If
                                                                ElseIf sslPolicyErrors = SslPolicyErrors.None Then
                                                                    Return True
                                                                Else
                                                                    For Each status As X509ChainStatus In chain.ChainStatus
                                                                        If (status.Status = X509ChainStatusFlags.RevocationStatusUnknown) Or (status.Status = X509ChainStatusFlags.OfflineRevocation) Then
                                                                            Continue For
                                                                        Else
                                                                            Serverlog.WriteEntry(status.StatusInformation + vbCrLf + status.Status.ToString, EventLogEntryType.Error)
                                                                            Return False
                                                                        End If
                                                                    Next
                                                                    Return True
                                                                End If
                                                            End Function),
                    New LocalCertificateSelectionCallback(Function(sender As Object, targetHost As String, localCertificates As X509CertificateCollection, remoteCertificate As X509Certificate, acceptableIssuers As String())
                                                              Return cert
                                                          End Function))

            sslStream.ReadTimeout = 10000
            sslStream.WriteTimeout = 10000

            sslStream.AuthenticateAsClient("Slardar.net", New X509Certificate2Collection(cert), SslProtocols.Tls12, True)
            If sslStream.IsMutuallyAuthenticated Then
                Dim SR As New StreamReader(sslStream)
                Dim SW As New StreamWriter(sslStream)
                SW.AutoFlush = True
                SW.WriteLine(isTemp)
                Dim ReturnStr As String = SR.ReadLine
                If ReturnStr = "OK" Then
                    Serverlog.WriteEntry("UPDATE Firewall Successful.", EventLogEntryType.SuccessAudit)
                Else
                    Serverlog.WriteEntry("UPDATE Firewall Error." + vbCrLf + "Reason:" + ReturnStr, EventLogEntryType.FailureAudit)
                End If

            Else
                Throw New Exception("Authenticate Failed")
            End If
            Return True
        Catch ex As Exception
            Serverlog.WriteEntry(ex.Message, EventLogEntryType.Error)
            Return False
        End Try
    End Function

    Private Function isConnected() As Boolean
        Try
            Dim client As New TcpClient()
            client.ReceiveTimeout = 100
            client.SendTimeout = 100
            client.Connect(RemoteIP.ToString, Port)
            Dim WS As New StreamWriter(client.GetStream)
            WS.Write(0)
            WS.Flush()
            WS.Close()
            client.Close()
            Return True
        Catch
            Return False
        End Try
    End Function
End Class
