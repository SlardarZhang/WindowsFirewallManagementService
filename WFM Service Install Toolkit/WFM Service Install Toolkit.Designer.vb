Imports System.IO

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WFM_Service_Install_Toolkit
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WFM_Service_Install_Toolkit))
        Me.TitleLabel = New System.Windows.Forms.Label()
        Me.TitlePanel = New System.Windows.Forms.Panel()
        Me.Proviers_Button = New System.Windows.Forms.PictureBox()
        Me.Close_Button = New System.Windows.Forms.PictureBox()
        Me.NavigationPanel = New System.Windows.Forms.Panel()
        Me.Next_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.MainPanel = New System.Windows.Forms.Panel()
        Me.ConfigSumGroup = New System.Windows.Forms.GroupBox()
        Me.ConfigListBox = New System.Windows.Forms.ListBox()
        Me.IPPortGroup = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.IPLabel = New System.Windows.Forms.Label()
        Me.IPText = New System.Windows.Forms.TextBox()
        Me.PortLabel = New System.Windows.Forms.Label()
        Me.PortText = New System.Windows.Forms.TextBox()
        Me.ServiceTypeGroup = New System.Windows.Forms.GroupBox()
        Me.Client = New System.Windows.Forms.RadioButton()
        Me.Server = New System.Windows.Forms.RadioButton()
        Me.CertGroup = New System.Windows.Forms.GroupBox()
        Me.CertShow = New System.Windows.Forms.Button()
        Me.CertShowPassword = New System.Windows.Forms.CheckBox()
        Me.CertBrowse = New System.Windows.Forms.Button()
        Me.CertPassword = New System.Windows.Forms.TextBox()
        Me.CertFilename = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.LanguageGroup = New System.Windows.Forms.GroupBox()
        Me.Chs = New System.Windows.Forms.RadioButton()
        Me.Eng = New System.Windows.Forms.RadioButton()
        Me.TitlePanel.SuspendLayout()
        CType(Me.Proviers_Button, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Close_Button, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.NavigationPanel.SuspendLayout()
        Me.MainPanel.SuspendLayout()
        Me.ConfigSumGroup.SuspendLayout()
        Me.IPPortGroup.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.ServiceTypeGroup.SuspendLayout()
        Me.CertGroup.SuspendLayout()
        Me.LanguageGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'TitleLabel
        '
        resources.ApplyResources(Me.TitleLabel, "TitleLabel")
        Me.TitleLabel.Name = "TitleLabel"
        '
        'TitlePanel
        '
        resources.ApplyResources(Me.TitlePanel, "TitlePanel")
        Me.TitlePanel.Controls.Add(Me.TitleLabel)
        Me.TitlePanel.Controls.Add(Me.Proviers_Button)
        Me.TitlePanel.Controls.Add(Me.Close_Button)
        Me.TitlePanel.Name = "TitlePanel"
        '
        'Proviers_Button
        '
        resources.ApplyResources(Me.Proviers_Button, "Proviers_Button")
        Me.Proviers_Button.Image = Global.WFM_Service_Install_Toolkit.My.Resources.Resources.back_normal
        Me.Proviers_Button.Name = "Proviers_Button"
        Me.Proviers_Button.TabStop = False
        '
        'Close_Button
        '
        resources.ApplyResources(Me.Close_Button, "Close_Button")
        Me.Close_Button.Image = Global.WFM_Service_Install_Toolkit.My.Resources.Resources.close_normal
        Me.Close_Button.Name = "Close_Button"
        Me.Close_Button.TabStop = False
        '
        'NavigationPanel
        '
        resources.ApplyResources(Me.NavigationPanel, "NavigationPanel")
        Me.NavigationPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.NavigationPanel.Controls.Add(Me.Next_Button)
        Me.NavigationPanel.Controls.Add(Me.Cancel_Button)
        Me.NavigationPanel.Name = "NavigationPanel"
        '
        'Next_Button
        '
        resources.ApplyResources(Me.Next_Button, "Next_Button")
        Me.Next_Button.Name = "Next_Button"
        Me.Next_Button.UseVisualStyleBackColor = True
        '
        'Cancel_Button
        '
        resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'MainPanel
        '
        resources.ApplyResources(Me.MainPanel, "MainPanel")
        Me.MainPanel.BackColor = System.Drawing.Color.White
        Me.MainPanel.Controls.Add(Me.ConfigSumGroup)
        Me.MainPanel.Controls.Add(Me.IPPortGroup)
        Me.MainPanel.Controls.Add(Me.ServiceTypeGroup)
        Me.MainPanel.Controls.Add(Me.CertGroup)
        Me.MainPanel.Controls.Add(Me.LanguageGroup)
        Me.MainPanel.Name = "MainPanel"
        '
        'ConfigSumGroup
        '
        resources.ApplyResources(Me.ConfigSumGroup, "ConfigSumGroup")
        Me.ConfigSumGroup.Controls.Add(Me.ConfigListBox)
        Me.ConfigSumGroup.Name = "ConfigSumGroup"
        Me.ConfigSumGroup.TabStop = False
        '
        'ConfigListBox
        '
        resources.ApplyResources(Me.ConfigListBox, "ConfigListBox")
        Me.ConfigListBox.FormattingEnabled = True
        Me.ConfigListBox.Name = "ConfigListBox"
        Me.ConfigListBox.SelectionMode = System.Windows.Forms.SelectionMode.None
        '
        'IPPortGroup
        '
        resources.ApplyResources(Me.IPPortGroup, "IPPortGroup")
        Me.IPPortGroup.Controls.Add(Me.FlowLayoutPanel1)
        Me.IPPortGroup.Name = "IPPortGroup"
        Me.IPPortGroup.TabStop = False
        '
        'FlowLayoutPanel1
        '
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.Controls.Add(Me.IPLabel)
        Me.FlowLayoutPanel1.Controls.Add(Me.IPText)
        Me.FlowLayoutPanel1.Controls.Add(Me.PortLabel)
        Me.FlowLayoutPanel1.Controls.Add(Me.PortText)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'IPLabel
        '
        resources.ApplyResources(Me.IPLabel, "IPLabel")
        Me.IPLabel.Name = "IPLabel"
        '
        'IPText
        '
        resources.ApplyResources(Me.IPText, "IPText")
        Me.IPText.Name = "IPText"
        '
        'PortLabel
        '
        resources.ApplyResources(Me.PortLabel, "PortLabel")
        Me.PortLabel.Name = "PortLabel"
        '
        'PortText
        '
        resources.ApplyResources(Me.PortText, "PortText")
        Me.PortText.Name = "PortText"
        '
        'ServiceTypeGroup
        '
        resources.ApplyResources(Me.ServiceTypeGroup, "ServiceTypeGroup")
        Me.ServiceTypeGroup.Controls.Add(Me.Client)
        Me.ServiceTypeGroup.Controls.Add(Me.Server)
        Me.ServiceTypeGroup.Name = "ServiceTypeGroup"
        Me.ServiceTypeGroup.TabStop = False
        '
        'Client
        '
        resources.ApplyResources(Me.Client, "Client")
        Me.Client.Name = "Client"
        Me.Client.UseVisualStyleBackColor = False
        '
        'Server
        '
        resources.ApplyResources(Me.Server, "Server")
        Me.Server.Checked = True
        Me.Server.Name = "Server"
        Me.Server.TabStop = True
        Me.Server.UseVisualStyleBackColor = False
        '
        'CertGroup
        '
        resources.ApplyResources(Me.CertGroup, "CertGroup")
        Me.CertGroup.Controls.Add(Me.CertShow)
        Me.CertGroup.Controls.Add(Me.CertShowPassword)
        Me.CertGroup.Controls.Add(Me.CertBrowse)
        Me.CertGroup.Controls.Add(Me.CertPassword)
        Me.CertGroup.Controls.Add(Me.CertFilename)
        Me.CertGroup.Controls.Add(Me.Label4)
        Me.CertGroup.Controls.Add(Me.Label5)
        Me.CertGroup.Name = "CertGroup"
        Me.CertGroup.TabStop = False
        '
        'CertShow
        '
        resources.ApplyResources(Me.CertShow, "CertShow")
        Me.CertShow.Name = "CertShow"
        Me.CertShow.UseVisualStyleBackColor = True
        '
        'CertShowPassword
        '
        resources.ApplyResources(Me.CertShowPassword, "CertShowPassword")
        Me.CertShowPassword.Name = "CertShowPassword"
        Me.CertShowPassword.UseVisualStyleBackColor = True
        '
        'CertBrowse
        '
        resources.ApplyResources(Me.CertBrowse, "CertBrowse")
        Me.CertBrowse.Name = "CertBrowse"
        Me.CertBrowse.UseVisualStyleBackColor = True
        '
        'CertPassword
        '
        resources.ApplyResources(Me.CertPassword, "CertPassword")
        Me.CertPassword.Name = "CertPassword"
        '
        'CertFilename
        '
        resources.ApplyResources(Me.CertFilename, "CertFilename")
        Me.CertFilename.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.CertFilename.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem
        Me.CertFilename.Name = "CertFilename"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'LanguageGroup
        '
        resources.ApplyResources(Me.LanguageGroup, "LanguageGroup")
        Me.LanguageGroup.Controls.Add(Me.Chs)
        Me.LanguageGroup.Controls.Add(Me.Eng)
        Me.LanguageGroup.Name = "LanguageGroup"
        Me.LanguageGroup.TabStop = False
        '
        'Chs
        '
        resources.ApplyResources(Me.Chs, "Chs")
        Me.Chs.Name = "Chs"
        Me.Chs.UseVisualStyleBackColor = False
        '
        'Eng
        '
        resources.ApplyResources(Me.Eng, "Eng")
        Me.Eng.Checked = True
        Me.Eng.Name = "Eng"
        Me.Eng.TabStop = True
        Me.Eng.UseVisualStyleBackColor = False
        '
        'WFM_Service_Install_Toolkit
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(203, Byte), Integer), CType(CType(127, Byte), Integer), CType(CType(166, Byte), Integer))
        Me.Controls.Add(Me.MainPanel)
        Me.Controls.Add(Me.NavigationPanel)
        Me.Controls.Add(Me.TitlePanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WFM_Service_Install_Toolkit"
        Me.TitlePanel.ResumeLayout(False)
        Me.TitlePanel.PerformLayout()
        CType(Me.Proviers_Button, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Close_Button, System.ComponentModel.ISupportInitialize).EndInit()
        Me.NavigationPanel.ResumeLayout(False)
        Me.MainPanel.ResumeLayout(False)
        Me.MainPanel.PerformLayout()
        Me.ConfigSumGroup.ResumeLayout(False)
        Me.IPPortGroup.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ServiceTypeGroup.ResumeLayout(False)
        Me.CertGroup.ResumeLayout(False)
        Me.CertGroup.PerformLayout()
        Me.LanguageGroup.ResumeLayout(False)
        Me.LanguageGroup.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Close_Button As PictureBox
    Friend WithEvents Proviers_Button As PictureBox
    Friend WithEvents TitleLabel As Label
    Friend WithEvents TitlePanel As Panel
    Friend WithEvents NavigationPanel As Panel
    Friend WithEvents Next_Button As Button
    Friend WithEvents Cancel_Button As Button
    Friend WithEvents MainPanel As Panel
    Friend WithEvents LanguageGroup As GroupBox
    Friend WithEvents Eng As RadioButton
    Friend WithEvents Chs As RadioButton
    Friend WithEvents CertGroup As GroupBox
    Friend WithEvents CertShow As Button
    Friend WithEvents CertShowPassword As CheckBox
    Friend WithEvents CertBrowse As Button
    Friend WithEvents CertPassword As TextBox
    Friend WithEvents CertFilename As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents ServiceTypeGroup As GroupBox
    Friend WithEvents Client As RadioButton
    Friend WithEvents Server As RadioButton
    Friend WithEvents ConfigSumGroup As GroupBox
    Friend WithEvents ConfigListBox As ListBox
    Friend WithEvents IPPortGroup As GroupBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents IPLabel As Label
    Friend WithEvents IPText As TextBox
    Friend WithEvents PortLabel As Label
    Friend WithEvents PortText As TextBox
End Class
