
Partial Class budget_setup
    Inherits System.Web.UI.Page
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") <> "Accounting Assistant" And Session("role") <> "Purchasing Assistant" And Session("role") <> "Department Authorizer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub
End Class
