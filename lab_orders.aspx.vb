
Partial Class lab_orders
    Inherits System.Web.UI.Page
   

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.SelectedIndex = -1
    End Sub
End Class