
Partial Class order_reuse
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ph As ContentPlaceHolder = CType(PreviousPage.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)

        Dim gv As GridView = CType(ph.FindControl("GridView1"), GridView)
        If Not gv Is Nothing Then
            Label1.Text = gv.SelectedValue
        End If
    End Sub
End Class
