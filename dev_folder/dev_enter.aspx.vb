
Partial Class dev_folder_dev_enter
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim app_id As Integer = ConfigurationManager.AppSettings("app_id")

        Dim db As Database = DatabaseFactory.CreateDatabase("OPSConn")
        Dim bolError As Boolean = False
        Dim cmd As DbCommand = db.GetStoredProcCommand("check_auth")
        db.AddInParameter(cmd, "id", DbType.String, TextBox1.Text)
        db.AddInParameter(cmd, "app_id", DbType.Int16, app_id)

        Dim dr As SqlDataReader = db.ExecuteReader(cmd)


        dr.Read()

        If dr.HasRows Then
            Session("app_id") = app_id
            Session("app") = "ops"
            Session("emp_id") = dr("emp_id")
            Session("name") = dr("name")


            Select Case dr("admin_role")
                Case "dept_auth"
                    Session("role") = "Department Authorizer"
                Case "acct_asst"
                    Session("role") = "Accounting Assistant"
                Case "purch_asst"
                    Session("role") = "Purchasing Assistant"
                Case "reviewer"
                    Session("role") = "reviewer"
                Case Else
                    Session("role") = "orderer"
            End Select

        Else
            Session("name") = ""
            Session("app") = ""
            bolError = True
        End If
        If Session("role") = "orderer" Then
            dr.NextResult()
            If dr.HasRows Then
                Session("role") = "approver"
            End If
        End If


        dr.Close()

        If bolError Then
            Response.Redirect("no_access.aspx")
        End If
    End Sub
End Class
