Imports System.Management
Imports System.Security.Cryptography
Imports System.Text

Module GetMachineID

    Private Function identifier(wmiClass As String, wmiProperty As String) As String
        Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher("Select " + wmiProperty + " From " + wmiClass)

        Dim sb As New StringBuilder
        For Each mObject As ManagementObject In searcher.Get()
            For Each pData As PropertyData In mObject.Properties
                If pData.Value Is Nothing Then
                    Continue For
                End If
                sb.Append(pData.Value.ToString)
            Next
        Next
        Return sb.ToString
    End Function

    Private Function biosId() As String
        Return identifier("Win32_BIOS", "Manufacturer") _
        + identifier("Win32_BIOS", "SMBIOSBIOSVersion") _
        + identifier("Win32_BIOS", "SerialNumber") _
        + identifier("Win32_BIOS", "ReleaseDate")
    End Function

    Private Function diskId() As String
        Return identifier("Win32_DiskDrive", "Model") _
            + identifier("Win32_DiskDrive", "SerialNumber")
    End Function

    Private Function baseId() As String
        Return identifier("Win32_BaseBoard", "Manufacturer") _
        + identifier("Win32_BaseBoard", "Name") _
        + identifier("Win32_BaseBoard", "SerialNumber")
    End Function

    Private Function cpuID() As String
        Return identifier("Win32_Processor", "ProcessorId")
    End Function

    Public Function GetMachineID() As String
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim enc As ASCIIEncoding = New ASCIIEncoding
        Return Convert.ToBase64String(md5.ComputeHash(enc.GetBytes(cpuID() + biosId() + diskId() + baseId())))
    End Function
End Module
