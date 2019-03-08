Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class MyAES256
    Private Const KEY_SIZE As Integer = 256

    Private AES As RijndaelManaged

    Public Sub New(Key As String)
        Dim UnHashedBytes As Byte() = Encoding.UTF8.GetBytes("")
        Dim SHA256 As HMACSHA256 = New HMACSHA256(Encoding.UTF8.GetBytes(Key))
        Dim HashedBytes As Byte() = SHA256.ComputeHash(UnHashedBytes)
        Dim keyBuilder As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Key, HashedBytes)
        AES = New RijndaelManaged()
        AES.Mode = CipherMode.ECB
        AES.KeySize = KEY_SIZE
        AES.IV = keyBuilder.GetBytes(CType(AES.BlockSize / 8, Integer))
        AES.Key = keyBuilder.GetBytes(CType(AES.KeySize / 8, Integer))
        AES.Padding = PaddingMode.Zeros
    End Sub

    Public Sub New(Key As String, Salt As String)
        Dim UnHashedBytes As Byte() = Encoding.UTF8.GetBytes(Salt)
        Dim SHA256 As HMACSHA256 = New HMACSHA256(Encoding.UTF8.GetBytes(Key))
        Dim HashedBytes As Byte() = SHA256.ComputeHash(UnHashedBytes)
        Dim keyBuilder As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Key, HashedBytes)
        AES = New RijndaelManaged()
        AES.Mode = CipherMode.ECB
        AES.KeySize = KEY_SIZE
        AES.IV = keyBuilder.GetBytes(CType(AES.BlockSize / 8, Integer))
        AES.Key = keyBuilder.GetBytes(CType(AES.KeySize / 8, Integer))
        AES.Padding = PaddingMode.Zeros
    End Sub

    Public Function Encrypt(Value As String) As String
        Dim ms As New MemoryStream
        Dim sw As New StreamWriter(ms)
        sw.Write(Value)
        sw.Flush()
        Return Convert.ToBase64String(Encrypt(ms.ToArray))
    End Function

    Public Function Encrypt(Value As Byte()) As Byte()
        Dim Encrypted As Byte()
        Dim Encryptor As ICryptoTransform = AES.CreateEncryptor(AES.Key, AES.IV)
        Using msEncrypt As New MemoryStream()
            Using csEncrypt As New CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write)
                csEncrypt.Write(Value, 0, Value.Length)
                csEncrypt.Flush()
            End Using
            Encrypted = msEncrypt.ToArray()
        End Using
        Return Encrypted
    End Function

    Public Function Decrypt(Value As String) As String
        Dim Encrypted As Byte() = Convert.FromBase64String(Value)
        Dim DecryptedString As String
        Using MS As New MemoryStream(Decrypt(Encrypted))
                Using SR As New StreamReader(MS)
                    DecryptedString = SR.ReadToEnd
                End Using
            End Using
            Dim TempChar As Char = DecryptedString.Chars(DecryptedString.Length - 1)
            While (TempChar = vbNullChar)
                DecryptedString = DecryptedString.Remove(DecryptedString.Length - 1)
                TempChar = DecryptedString.Chars(DecryptedString.Length - 1)
            End While
            Return DecryptedString
    End Function

    Public Function Decrypt(Value As Byte()) As Byte()
        Dim StringBytes As Byte()
        ReDim StringBytes(Value.Length - 1)
        Dim Decryptor As ICryptoTransform = AES.CreateDecryptor(AES.Key, AES.IV)
        Using msDecrypt As New MemoryStream(Value)
            Using csDecrypt As New CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Read)
                csDecrypt.Read(StringBytes, 0, StringBytes.Length)
            End Using
        End Using
        Return StringBytes
    End Function
End Class
