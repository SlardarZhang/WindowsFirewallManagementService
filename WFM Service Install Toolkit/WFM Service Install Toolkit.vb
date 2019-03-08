Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.ServiceProcess
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.Win32

Public Class WFM_Service_Install_Toolkit
    Private CurrentX509 As X509Certificate2
    Private MoveForm As Boolean
    Private MoveForm_MousePosition As Point
    Private AES256 As MyAES256
    Private Steps As Integer
    Private Sub Close_Button_Click(sender As Object, e As EventArgs) Handles Close_Button.Click
        Application.Exit()
    End Sub

    Private Sub Close_Button_MouseHover(sender As Object, e As EventArgs) Handles Close_Button.MouseHover
        Close_Button.Image = My.Resources.close_hover
    End Sub

    Private Sub Close_Button_MouseDown(sender As Object, e As MouseEventArgs) Handles Close_Button.MouseDown
        Close_Button.Image = My.Resources.close_press
    End Sub

    Private Sub Close_Button_MouseLeave(sender As Object, e As EventArgs) Handles Close_Button.MouseLeave
        Close_Button.Image = My.Resources.close_normal
    End Sub

    Private Sub Close_Button_MouseUp(sender As Object, e As MouseEventArgs) Handles Close_Button.MouseUp
        If Close_Button.DisplayRectangle.Contains(e.Location) Then
            Close_Button.Image = My.Resources.close_hover
        Else
            Close_Button.Image = My.Resources.close_normal
        End If
    End Sub

    Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
        Application.Exit()
    End Sub

    Private Sub CenterGroup(ByRef group As GroupBox)
        Try
            For Each item As GroupBox In MainPanel.Controls.OfType(Of GroupBox)()
                item.Visible = False
            Next
        Catch ex As Exception

        End Try
        group.Left = (MainPanel.Width - group.Width) / 2
        group.Top = (MainPanel.Height - group.Height) / 2
        group.Visible = True
    End Sub

    Private Sub WFM_Service_Install_Toolkit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If LoadConfig("Language") IsNot Nothing Then
            Try
                Thread.CurrentThread.CurrentUICulture = New CultureInfo(CType(LoadConfig("Language"), String))
            Catch
                Thread.CurrentThread.CurrentUICulture = New CultureInfo("en")
            End Try
        Else
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en")
        End If
        Reload()

        For Each Service As ServiceController In ServiceController.GetServices()
            If Service.ServiceName = "WFMService" Then
                If MessageBox.Show(Me, My.Resources.ResourceManager.GetString("UninstallMsg"), My.Resources.ResourceManager.GetString("Uninstall"), vbYesNo, vbQuestion) = vbYes Then
                    Dim InstallUtilEXE As String = GetDotNetFrameworkPath() + "\InstallUtil.exe"
                    Dim DDNSPath As String = Directory.GetCurrentDirectory
                    If DDNSPath.Chars(DDNSPath.Length - 1) <> "\" Then
                        DDNSPath = DDNSPath + "\"
                    End If
                    DDNSPath = """" + DDNSPath + "Windows Firewall Management Service.exe" + """"
                    DDNSPath = DDNSPath + " /u"
                    Dim InstallPSI As New ProcessStartInfo(InstallUtilEXE, DDNSPath)
                    Dim InstallProcess As New Process
                    InstallPSI.CreateNoWindow = True
                    InstallPSI.WindowStyle = ProcessWindowStyle.Hidden
                    InstallPSI.UseShellExecute = False
                    InstallPSI.RedirectStandardOutput = True
                    InstallPSI.RedirectStandardError = True
                    InstallProcess.StartInfo = InstallPSI

                    InstallProcess.Start()
                    InstallProcess.WaitForExit()
                    Dim Result As String = InstallProcess.StandardOutput.ReadToEnd
                    If InstallProcess.ExitCode <> 0 Then
                        MessageBox.Show(Me, Result, My.Resources.ResourceManager.GetString("InstallError"), vbOKOnly, MessageBoxIcon.Error)
                    Else
                        MessageBox.Show(Me, Result, My.Resources.ResourceManager.GetString("InstallSuccessful"), vbOKOnly, MessageBoxIcon.Information)
                    End If
                Else
                    Application.Exit()
                End If
                Exit For
            End If
        Next
        Steps = 0
        If My.Application.UICulture.Name = "zh-CHS" Then
            Chs.Checked = True
            Eng.Checked = False
        Else
            Eng.Checked = True
            Chs.Checked = False
        End If
        AES256 = New MyAES256(GetMachineID.GetMachineID())
    End Sub

    Private Sub Reload()
        Controls.Clear()
        My.Application.ChangeCulture(My.Application.UICulture.Name)
        InitializeComponent()
        TitleLabel.Text = Me.Text
        If My.Application.UICulture.Name = "zh-CHS" Then
            Eng.Checked = False
            Chs.Checked = True
        Else
            Eng.Checked = True
            Chs.Checked = False
        End If
        CenterGroup(LanguageGroup)
    End Sub
    Private Sub SaveStringConfig(ByVal Key As String, Value As String)
        Dim Reg As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Slardar\WFMService", True)
        Reg.SetValue(Key, Value)
        Reg.Close()
    End Sub

    Private Sub SaveBooleanConfig(ByVal Key As String, Value As Boolean)
        Dim Reg As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Slardar\WFMService", True)
        Reg.SetValue(Key, Value, RegistryValueKind.DWord)
        Reg.Close()
    End Sub

    Private Sub SaveBytesConfig(ByVal Key As String, Value As Byte())
        Dim Reg As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Slardar\WFMService", True)
        Reg.SetValue(Key, Value)
        Reg.Close()
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

    Private Sub Eng_Click(sender As Object, e As EventArgs) Handles Eng.Click
        If Eng.Checked Then
            Chs.Checked = False
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en")
        Else
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("zh-CHS")
        End If
        Reload()
    End Sub

    Private Sub Chs_Click(sender As Object, e As EventArgs) Handles Chs.Click
        If Chs.Checked Then
            Eng.Checked = False
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("zh-CHS")
        Else
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en")
        End If
        Reload()
    End Sub

    Private Function GetDotNetFrameworkPath() As String
        Dim Reg As RegistryKey
        If Environment.Is64BitOperatingSystem Then
            Reg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\Microsoft\.NetFramework", False)
        Else
            Reg = Registry.LocalMachine.OpenSubKey("Software\Microsoft\.NetFramework", False)
        End If
        If Reg Is Nothing Then
            Return Nothing
        Else
            Dim DotNetFrameworkPath As String = Reg.GetValue("InstallRoot")
            If DotNetFrameworkPath Is Nothing Then
                Return Nothing
            Else
                Dim DotNetFramewrokDir As String() = Directory.GetDirectories(DotNetFrameworkPath)
                Dim DotDirsCollection As New List(Of DirectoryInfo)
                For Each Dir As String In DotNetFramewrokDir
                    If Regex.IsMatch(Dir, "v\d\.\d|v\d\.\d\.\d*") Then
                        DotDirsCollection.Add(New DirectoryInfo(Dir))
                    End If
                Next
                DotDirsCollection.Sort(Function(A As DirectoryInfo, B As DirectoryInfo)
                                           Return (0 - A.Name.CompareTo(B.Name))
                                       End Function)
                Return (DotDirsCollection.Item(0).FullName)
            End If
        End If
    End Function

    Private Sub ShowCertInfo(sender As Object, e As EventArgs) Handles CertShow.Click
        Try
            Dim CertInfo As String = ""
            If File.Exists(CertFilename.Text) Then
                If CertShowPassword.Text.Length = 0 Then
                    CurrentX509 = New X509Certificate2(CertFilename.Text)
                Else
                    CurrentX509 = New X509Certificate2(CertFilename.Text, CertPassword.Text)
                End If
            End If
            CertInfo = My.Resources.ResourceManager.GetString("Issuer") + CurrentX509.GetNameInfo(X509NameType.DnsName, True) + vbCrLf
            CertInfo = CertInfo + My.Resources.ResourceManager.GetString("IssuedTo") + CurrentX509.GetNameInfo(X509NameType.SimpleName, False) + vbCrLf
            CertInfo = CertInfo + My.Resources.ResourceManager.GetString("DNSDomain") + vbCrLf + String.Join(vbCrLf, GetDNSNames(CurrentX509).ToArray)
            MessageBox.Show(Me, CertInfo, My.Resources.ResourceManager.GetString("CertInfo"), vbOKOnly, vbInformation)
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, My.Resources.ResourceManager.GetString("ReadCertError"), vbOKOnly, vbCritical)
        End Try
    End Sub

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

    Private Sub CertShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles CertShowPassword.CheckedChanged
        If CertShowPassword.Checked = True Then
            CertPassword.PasswordChar = ""
        Else
            CertPassword.PasswordChar = "●"
        End If
    End Sub

    Private Sub WFM_Service_Install_Toolkit_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawRectangle(New Pen(Color.FromArgb(128, 0, 0, 0)), New Rectangle(0, 0, Width - 1, Height - 1))
    End Sub

    Private Sub TitlePanel_MouseUp(sender As Object, e As MouseEventArgs) Handles TitlePanel.MouseUp
        If e.Button = MouseButtons.Left Then
            MoveForm = False
        End If
    End Sub

    Private Sub TitlePanel_MouseDown(sender As Object, e As MouseEventArgs) Handles TitlePanel.MouseDown
        If e.Button = MouseButtons.Left Then
            MoveForm = True
            MoveForm_MousePosition = e.Location
        End If
    End Sub

    Private Sub TitlePanel_MouseMove(sender As Object, e As MouseEventArgs) Handles TitlePanel.MouseMove
        If MoveForm Then
            Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
        End If
    End Sub

    Private Sub Next_Button_Click(sender As Object, e As EventArgs) Handles Next_Button.Click
        Select Case Steps
            Case 0
                SaveStringConfig("Language", My.Application.UICulture.Name)
                If LoadConfig("isServer") IsNot Nothing Then
                    If LoadConfig("isServer") Then
                        Server.Checked = True
                        Client.Checked = False
                    Else
                        Server.Checked = False
                        Client.Checked = True
                    End If
                Else
                    Server.Checked = True
                    Client.Checked = False
                End If
                CenterGroup(ServiceTypeGroup)
                Proviers_Button.Enabled = True
            Case 1
                If Server.Checked Then
                    SaveBooleanConfig("isServer", True)
                Else
                    SaveBooleanConfig("isServer", False)
                End If
                CenterGroup(CertGroup)
                If LoadConfig("Cert") IsNot Nothing And LoadConfig("Key") IsNot Nothing Then
                    Try
                        CurrentX509 = New X509Certificate2(Convert.FromBase64String(AES256.Decrypt((LoadConfig("Cert")))))
                        CertFilename.Text = "[Saved Cert:" + CurrentX509.GetNameInfo(X509NameType.SimpleName, False) + "]"
                        Dim Params As CspParameters = New CspParameters()
                        Params.KeyContainerName = "KeyContainer"
                        Dim PrivateKey As RSACryptoServiceProvider = New RSACryptoServiceProvider(Params)
                        PrivateKey.FromXmlString(AES256.Decrypt(LoadConfig("Key")))
                        CurrentX509.PrivateKey = PrivateKey
                        CertFilename.Enabled = False
                        CertPassword.Enabled = False
                        CertShowPassword.Enabled = False
                    Catch ex As CryptographicException
                        CertFilename.Text = ""
                        CertFilename.Enabled = True
                    End Try
                Else
                    CertFilename.Text = ""
                    CertFilename.Enabled = True
                End If
            Case 2
                If CertFilename.Enabled Then
                    Try
                        Dim X509Certificate2 As X509Certificate2
                        If CertPassword.Text.Length = 0 Then
                            X509Certificate2 = New X509Certificate2(CertFilename.Text, X509KeyStorageFlags.Exportable)
                        Else
                            X509Certificate2 = New X509Certificate2(CertFilename.Text, CertPassword.Text, X509KeyStorageFlags.Exportable)
                        End If
                        If Not X509Certificate2.HasPrivateKey Then
                            Throw New Exception(My.Resources.ResourceManager.GetString("NoPrivateKey"))
                        End If
                        Dim PrivateKey As RSACryptoServiceProvider = X509Certificate2.PrivateKey

                        SaveStringConfig("Cert", AES256.Encrypt(Convert.ToBase64String(X509Certificate2.GetRawCertData)))
                        SaveStringConfig("Key", AES256.Encrypt(PrivateKey.ToXmlString(True)))
                        CurrentX509 = X509Certificate2
                    Catch ex As Exception
                        MessageBox.Show(Me, ex.Message, My.Resources.ResourceManager.GetString("ReadCertError"), vbOKOnly, vbCritical)
                        Exit Sub
                    End Try
                End If
                CenterGroup(IPPortGroup)
                If Server.Checked Then
                    IPText.Visible = False
                    IPLabel.Visible = False
                Else
                    IPText.Visible = True
                    IPLabel.Visible = True
                End If
                If LoadConfig("ServerIP") IsNot Nothing Then
                    IPText.Text = LoadConfig("ServerIP")
                End If
                If LoadConfig("Port") IsNot Nothing Then
                    PortText.Text = LoadConfig("Port")
                End If
            Case 3
                SaveStringConfig("ServerIP", IPText.Text)
                SaveStringConfig("Port", PortText.Text)
                CenterGroup(ConfigSumGroup)
                ConfigListBox.Items.Clear()
                If Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("InstallType") + "	  " + My.Resources.ResourceManager.GetString("Server"))
                Else
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("InstallType") + "	  " + My.Resources.ResourceManager.GetString("Client"))
                End If
                ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("UsingCert") + "	  " + CurrentX509.GetNameInfo(X509NameType.SimpleName, False))
                If Not Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("Issuer") + " " + CurrentX509.GetNameInfo(X509NameType.DnsName, True))
                End If
                If Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("ListeningPort") + "	  " + PortText.Text)
                Else
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("ServerIP") + "	  " + IPText.Text.Replace(" ", ""))
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("RemotePort") + "	  " + PortText.Text)
                End If
                Next_Button.Text = My.Resources.ResourceManager.GetString("Install")
            Case 4
                Dim InstallUtilEXE As String = GetDotNetFrameworkPath() + "\InstallUtil.exe"
                Dim DDNSPath As String = Directory.GetCurrentDirectory
                If DDNSPath.Chars(DDNSPath.Length - 1) <> "\" Then
                    DDNSPath = DDNSPath + "\"
                End If
                DDNSPath = """" + DDNSPath + "Windows Firewall Management Service.exe" + """"
                Dim InstallPSI As New ProcessStartInfo(InstallUtilEXE, DDNSPath)
                Dim InstallProcess As New Process
                InstallPSI.CreateNoWindow = True
                InstallPSI.WindowStyle = ProcessWindowStyle.Hidden
                InstallPSI.UseShellExecute = False
                InstallPSI.RedirectStandardOutput = True
                InstallPSI.RedirectStandardError = True
                InstallProcess.StartInfo = InstallPSI
                InstallProcess.Start()
                InstallProcess.WaitForExit()
                Dim Result As String = InstallProcess.StandardOutput.ReadToEnd
                If InstallProcess.ExitCode <> 0 Then
                    MessageBox.Show(Me, Result, My.Resources.ResourceManager.GetString("InstallError"), vbOKOnly, MessageBoxIcon.Error)
                    Application.Exit()
                Else
                    MessageBox.Show(Me, Result, My.Resources.ResourceManager.GetString("InstallSuccessful"), vbOKOnly, MessageBoxIcon.Information)
                    For Each Service As ServiceController In ServiceController.GetServices()
                        If Service.ServiceName = "WFMService" Then
                            Service.Start()
                        End If
                    Next
                    Application.Exit()
                End If
            Case Else
                Steps = 4
                CenterGroup(ConfigSumGroup)
                ConfigListBox.Items.Clear()
                If Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("InstallType") + "	  " + My.Resources.ResourceManager.GetString("Server"))
                Else
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("InstallType") + "	  " + My.Resources.ResourceManager.GetString("Client"))
                End If
                ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("UsingCert") + "	  " + CurrentX509.GetNameInfo(X509NameType.SimpleName, False))
                If Not Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("Issuer") + " " + CurrentX509.GetNameInfo(X509NameType.DnsName, True))
                End If
                If Server.Checked Then
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("ListeningPort") + "	  " + PortText.Text)
                Else
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("ServerIP") + "	  " + IPText.Text.Replace(" ", ""))
                    ConfigListBox.Items.Add(My.Resources.ResourceManager.GetString("RemotePort") + "	  " + PortText.Text)
                End If
                Next_Button.Text = My.Resources.ResourceManager.GetString("Install")
        End Select
        Steps = Steps + 1
    End Sub

    Private Sub Proviers_Button_Click(sender As Object, e As EventArgs) Handles Proviers_Button.Click
        Select Case Steps
            Case 0
            Case 1
                If My.Application.UICulture.Name = "zh-CHS" Then
                    Eng.Checked = False
                    Chs.Checked = True
                Else
                    Eng.Checked = True
                    Chs.Checked = False
                End If
                CenterGroup(LanguageGroup)
                Steps = 0
            Case Else
                Next_Button.Text = My.Resources.ResourceManager.GetString("Next")
                Steps = Steps - 2
                Next_Button_Click(sender, e)
        End Select

    End Sub

    Private Sub CertBrowse_Click(sender As Object, e As EventArgs) Handles CertBrowse.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = Directory.GetCurrentDirectory
        OpenFileDialog.Filter = "PKCS #12 Certificate (*.p12;*.pfx)|*.p12;*.pfx"
        OpenFileDialog.Title = My.Resources.ResourceManager.GetString("PickCert")
        OpenFileDialog.Multiselect = False
        OpenFileDialog.ValidateNames = True
        OpenFileDialog.RestoreDirectory = False
        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            If OpenFileDialog.CheckFileExists Then
                CertFilename.Text = OpenFileDialog.FileName
                CertFilename.Enabled = True
                CertPassword.Enabled = True
                CertShowPassword.Enabled = True
            Else
                MessageBox.Show(Me, My.Resources.ResourceManager.GetString("CertFileNotExistsMsg"), My.Resources.ResourceManager.GetString("CertFileNotExistsTitle"), vbOKOnly, vbCritical)
                CertBrowse_Click(sender, e)
            End If
        End If
    End Sub
End Class