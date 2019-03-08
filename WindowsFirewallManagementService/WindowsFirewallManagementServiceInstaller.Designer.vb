<System.ComponentModel.RunInstaller(True)> Partial Class WindowsFirewallManagementServicetInstaller
    Inherits System.Configuration.Install.Installer

    'Installer 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '组件设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是组件设计器所必需的
    '可使用组件设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.WindowsFirewallManagementServiceProcessInstaller = New System.ServiceProcess.ServiceProcessInstaller()
        Me.WindowsFirewallManagementServiceInstaller = New System.ServiceProcess.ServiceInstaller()
        '
        'WindowsFirewallManagementServiceProcessInstaller
        '
        Me.WindowsFirewallManagementServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.WindowsFirewallManagementServiceProcessInstaller.Password = Nothing
        Me.WindowsFirewallManagementServiceProcessInstaller.Username = Nothing
        '
        'WindowsFirewallManagementServiceInstaller
        '
        Me.WindowsFirewallManagementServiceInstaller.Description = "Windows Firewall Management Service program."
        Me.WindowsFirewallManagementServiceInstaller.DisplayName = "Windows Firewall Management Service"
        Me.WindowsFirewallManagementServiceInstaller.ServiceName = "WFMService"
        Me.WindowsFirewallManagementServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'WindowsFirewallManagementServicetInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.WindowsFirewallManagementServiceProcessInstaller, Me.WindowsFirewallManagementServiceInstaller})

    End Sub

    Friend WithEvents WindowsFirewallManagementServiceProcessInstaller As ServiceProcess.ServiceProcessInstaller
    Friend WithEvents WindowsFirewallManagementServiceInstaller As ServiceProcess.ServiceInstaller
End Class
