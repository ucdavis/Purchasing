
Partial Class view
    Inherits System.Web.UI.Page
  

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.SelectedIndex = -1
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") <> "Accounting Assistant" And Session("role") <> "Purchasing Assistant" And Session("role") <> "Department Authorizer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub
End Class
