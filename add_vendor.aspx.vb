
Partial Class add_vendor
    Inherits System.Web.UI.Page

    Protected Sub CustomValidator1_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        Dim txtWeb As TextBox = FormView1.FindControl("websiteTextBox")
        args.IsValid = False
        If txtWeb.Text <> "" Then
            args.IsValid = True
        Else
            Dim txtAd1 As TextBox = FormView1.FindControl("Address1TextBox")
            Dim txtCity As TextBox = FormView1.FindControl("cityTextBox")
            Dim txtState As TextBox = FormView1.FindControl("stateTextBox")
            Dim txtZip As TextBox = FormView1.FindControl("zipTextBox")
            Dim txtPhone As TextBox = FormView1.FindControl("phoneTextBox")
            If txtAd1.Text = "" Or txtCity.Text = "" Or txtState.Text = "" Or txtZip.Text = "" Or txtPhone.Text = "" Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End If
    End Sub

    Protected Sub CustomValidator2_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        Dim txtWeb As TextBox = FormView1.FindControl("websiteTextBox")
        args.IsValid = False
        If Left(txtWeb.Text, 4) <> "http" Then
            args.IsValid = True
        End If
    End Sub

    Protected Sub CustomValidator3_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        args.IsValid = False
        GridView1.DataBind()
    End Sub

    Protected Sub vendor_nameTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.DataBind()
        Dim cval As CustomValidator = FormView1.FindControl("CustomValidator3")
        cval.Validate()
    End Sub

    Protected Sub sdsVendorConfirm_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles sdsVendorConfirm.Selecting
        Dim txt As TextBox = FormView1.FindControl("vendor_nameTextBox")
        If txt.Text <> "" Then
            e.Command.Parameters("@vendor_name").Value = txt.Text
        Else
            e.Command.Parameters("@vendor_name").Value = "XXXX"
        End If

    End Sub

    Protected Sub InsertButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim cval As CustomValidator = FormView1.FindControl("CustomValidator3")
        Dim btnvalid As LinkButton = FormView1.FindControl("InsertButtonConfirm")
        Dim btnInsert As LinkButton = FormView1.FindControl("InsertButton")
        If Not cval.IsValid Then
            btnvalid.Visible = True
            btnInsert.Visible = False
        Else
            btnvalid.Visible = False
            btnInsert.Visible = True
        End If

    End Sub

    Protected Sub sdsVendor_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles sdsVendor.Inserted
        If e.AffectedRows > 0 Then
            Response.Redirect("home.aspx")
        End If
    End Sub
End Class
