Imports InsertFile
Partial Class edit_orders
    Inherits System.Web.UI.Page

    Protected Sub sdsOrderRequest_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceCommandEventArgs) Handles sdsOrderRequest.Updating
        For x As Integer = 0 To e.Command.Parameters.Count - 1
            Trace.Write(e.Command.Parameters(x).ParameterName & ": " & e.Command.Parameters(x).Value)
        Next
    End Sub

    Protected Sub btnCancel0_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.SelectedIndex = -1
    End Sub

    Protected Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Page.IsValid Then
            Dim fu As FileUpload = FormView1.FindControl("fuQuote")
            If fu.HasFile Then
                file_upload(fu, FormView1.DataKey.Value)
            End If
        End If
        Dim gv As GridView = FormView1.FindControl("gvAttachments")
        gv.DataBind()
    End Sub

    Protected Sub cvalQuote_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        args.IsValid = False
        Dim fu As FileUpload = FormView1.FindControl("fuQuote")
        Dim cval As CustomValidator = FormView1.FindControl("cvalQuote")
        args.IsValid = InsertFile.check_extension(fu, cval)
    End Sub
End Class
