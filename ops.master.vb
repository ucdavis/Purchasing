Partial Class opsmaster
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("app") <> "ops" Then
            Dim app_id As Integer = ConfigurationManager.AppSettings("app_id")

            Dim db As Database = DatabaseFactory.CreateDatabase("OPSConn")
            Dim bolError As Boolean = False
            Dim cmd As DbCommand = db.GetStoredProcCommand("check_auth")
            db.AddInParameter(cmd, "id", DbType.String, Context.User.Identity.Name)
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


        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblName.Text = Session("name")
        lblRole.Text = Session("role")
    End Sub
End Class

