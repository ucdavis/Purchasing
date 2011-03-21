Imports Microsoft.VisualBasic

Public Class InsertFile

    Public Shared Sub file_upload(ByVal fu As FileUpload, ByVal orderid As Int32)
        Dim sbRoot As New StringBuilder(1000)
        Dim dirExists As Boolean
        Dim db As Database = DatabaseFactory.CreateDatabase("OPSConn")

        Dim stE As String = fu.FileName
        If Len(fu.FileName) <= 5 Then
            stE = Right(fu.FileName, Len(fu.FileName) - InStr(fu.FileName, "."))
        Else
            stE = Right(fu.FileName, Len(fu.FileName) - InStr(Len(fu.FileName) - 5, fu.FileName, "."))
        End If
        stE = "." & LCase(stE)

        sbRoot.Append(ConfigurationManager.AppSettings("root_folder").ToString())

        Dim cmd As DbCommand = db.GetStoredProcCommand("insert_file")
        db.AddInParameter(cmd, "order_id", DbType.Int32, orderid)
        db.AddInParameter(cmd, "attach_name", DbType.String, fu.FileName)
        db.AddOutParameter(cmd, "attach_id", DbType.Int32, Int32.MaxValue)
        db.AddOutParameter(cmd, "year", DbType.Int16, Int16.MaxValue)
        db.AddOutParameter(cmd, "app_id", DbType.Int32, Int32.MaxValue)
        db.AddInParameter(cmd, "file_ext", DbType.String, stE)

        db.ExecuteNonQuery(cmd)

        sbRoot.Append(db.GetParameterValue(cmd, "app_id") & "\")
        dirExists = My.Computer.FileSystem.DirectoryExists(sbRoot.ToString)

        If Not dirExists Then
            My.Computer.FileSystem.CreateDirectory(sbRoot.ToString)
        End If

        sbRoot.Append(db.GetParameterValue(cmd, "year") & "\")
        dirExists = My.Computer.FileSystem.DirectoryExists(sbRoot.ToString)

        If Not dirExists Then
            My.Computer.FileSystem.CreateDirectory(sbRoot.ToString)
        End If

        sbRoot.Append(orderid & "\")
        dirExists = My.Computer.FileSystem.DirectoryExists(sbRoot.ToString)

        If Not dirExists Then
            My.Computer.FileSystem.CreateDirectory(sbRoot.ToString)
        End If

        sbRoot.Append(db.GetParameterValue(cmd, "attach_id") & stE)
        fu.SaveAs(sbRoot.ToString)
    End Sub

    Public Shared Function check_extension(ByVal fu As FileUpload, ByVal Cval As CustomValidator) As Boolean
        If fu.HasFile Then
            Dim stE As String = ""
            If Len(fu.FileName) <= 5 Then
                stE = Right(fu.FileName, Len(fu.FileName) - InStr(fu.FileName, "."))
            Else
                stE = Right(fu.FileName, Len(fu.FileName) - InStr(Len(fu.FileName) - 5, fu.FileName, "."))
            End If
            stE = LCase(stE)

            If stE = "pdf" Or stE = "htm" Or stE = "html" Or stE = "doc" Or stE = "docx" Or stE = "xls" Or stE = "xlsx" Or stE = "txt" Or stE = "wpd" Or stE = "jpg" Or stE = "msg" Or stE = "bmp" Then
                If fu.PostedFile.ContentLength.ToString > 5242880 Then
                    Cval.ErrorMessage = "File size is to large. Please reduce to less then 5 mb and try again"
                    Return False
                Else
                    Return True
                End If
            Else
                Cval.ErrorMessage = "'" & stE & "' is not a recognized valid file extension."
                Return False
            End If
        Else
            Return True
        End If
    End Function

End Class
