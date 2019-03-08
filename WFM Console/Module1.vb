
Module Module1

    Sub Main()
        'Dim IP As String = Console.ReadLine()
        Dim WFS As New WindowsFirewallManagementService.WindowsFirewallManagementService
        WFS.StartCmd(Nothing)
        Console.ReadKey()
    End Sub

End Module
