
Partial Class members
    Inherits System.Web.UI.Page

    Protected Sub FormView1_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertedEventArgs) Handles FormView1.ItemInserted
        FormView1.ChangeMode(FormViewMode.Edit)
        GridView1.SelectedIndex = -1
        GridView1.DataBind()
    End Sub

    Protected Sub FormView1_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdatedEventArgs) Handles FormView1.ItemUpdated
        GridView1.SelectedIndex = -1
        GridView1.DataBind()
    End Sub


    Protected Sub btnNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNew.Click
        FormView1.ChangeMode(FormViewMode.Insert)
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") <> "Accounting Assistant" And Session("role") <> "Purchasing Assistant" And Session("role") <> "Department Authorizer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.SelectedIndex = -1
    End Sub
End Class
