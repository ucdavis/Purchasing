
Partial Class no_access
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblDept.Text = ConfigurationManager.AppSettings("Dept_Name")
        hlemail.NavigateUrl = "mailto:" & ConfigurationManager.AppSettings("access_email")
    End Sub
End Class
