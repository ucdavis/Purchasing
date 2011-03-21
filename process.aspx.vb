Imports emailer
Partial Class process
    Inherits System.Web.UI.Page
    Dim price_sum As Decimal
    Dim bolFirst As Boolean = True
    Dim ship_mat As Decimal
    Dim ship_cost As Decimal
    Dim tax As Decimal
    Private _db As Database = DatabaseFactory.CreateDatabase("OPSConn")

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") <> "Accounting Assistant" And Session("role") <> "Purchasing Assistant" And Session("role") <> "Department Authorizer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub

    Protected Sub gvUrgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvUrgent.SelectedIndexChanged
        gvYour.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvUrgent.SelectedValue
        pnlProcess.Visible = True
    End Sub

    Protected Sub gvYour_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvYour.SelectedIndexChanged
        gvUrgent.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvYour.SelectedValue
        pnlProcess.Visible = True
    End Sub

    Protected Sub gvOther_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvOther.SelectedIndexChanged
        gvUrgent.SelectedIndex = -1
        gvYour.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvOther.SelectedValue
        pnlProcess.Visible = True
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        gvUrgent.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        gvYour.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = -1
        pnlProcess.Visible = True
    End Sub

    Protected Sub btnCancel_Click1(ByVal sender As Object, ByVal e As System.EventArgs)
        ResetpnlProcess()
    End Sub

    Protected Sub ResetpnlProcess()
        pnlProcess.Visible = False
        gvUrgent.SelectedIndex = -1
        gvYour.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = 0
        rblTax.SelectedIndex = 0
        txtShip.Text = ""
        txtShipMat.Text = ""
        ddlPaymentType.SelectedIndex = 0
        ddlStatus.SelectedIndex = 0
        txtPO.Text = ""
        txtComment.Text = ""
    End Sub


    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles FormView1.ModeChanged
        If FormView1.CurrentMode = FormViewMode.ReadOnly Then
            pnlProcess.Visible = True
        Else
            pnlProcess.Visible = False
        End If
    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        ResetpnlProcess()
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Page.IsValid Then
            Dim cmd As DbCommand = _db.GetStoredProcCommand("process_order")

            _db.AddInParameter(cmd, "tax", DbType.Boolean)
            If rblTax.SelectedValue = 1 Then
                _db.SetParameterValue(cmd, "tax", True)
            Else
                _db.SetParameterValue(cmd, "tax", False)
            End If
            _db.AddInParameter(cmd, "ship_cost", DbType.Currency, txtShip.Text)
            _db.AddInParameter(cmd, "ship_mat", DbType.Currency, txtShipMat.Text)
            _db.AddInParameter(cmd, "payment_type", DbType.String, ddlPaymentType.SelectedValue)
            _db.AddInParameter(cmd, "status", DbType.String, ddlStatus.SelectedValue)
            _db.AddInParameter(cmd, "po", DbType.String, txtPO.Text)
            _db.AddInParameter(cmd, "purchasing_comments", DbType.String, txtComment.Text)
            _db.AddInParameter(cmd, "purchasing_id", DbType.Int32, Session("emp_id"))
            _db.AddInParameter(cmd, "order_id", DbType.Int32, FormView1.SelectedValue)

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)

            dr.Read()

            If ddlStatus.SelectedValue = "Denied by Purchasing" Then
                email_post_purch_denied(dr("full_tag"), dr("vendor_name"), dr("o_name"), dr("a_name"), txtComment.Text, dr("o_email"), dr("a_email"), Session("name"))
            ElseIf ddlStatus.SelectedValue = "Order Placed" Then
                email_post_purch_placed(dr("full_tag"), dr("vendor_name"), dr("o_name"), dr("a_name"), txtComment.Text, dr("o_email"), Session("name"), txtPO.Text, dr("p_phone"), ConfigurationManager.AppSettings("dept_name"))
            Else
                email_post_purch_campus_purch(dr("full_tag"), dr("vendor_name"), dr("o_name"), dr("a_name"), txtComment.Text, dr("o_email"), Session("name"), dr("p_phone"), ConfigurationManager.AppSettings("dept_name"))
            End If




            dr.Close()

            Response.Redirect("receipt.aspx?order_id=" & FormView1.DataKey.Value)
        End If
    End Sub

    Protected Sub CustomValidator1_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator1.ServerValidate
        args.IsValid = False
        If ddlStatus.SelectedValue = "Denied by Purchasing" Then
            If txtComment.Text = "" Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub

    Protected Sub CustomValidator2_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator2.ServerValidate
        args.IsValid = False
        If ddlStatus.SelectedValue <> "Denied by Purchasing" Then
            If txtPO.Text = "" Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub

    Protected Sub sdsOrderRequest_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceCommandEventArgs) Handles sdsOrderRequest.Updating
        For x As Integer = 0 To e.Command.Parameters.Count - 1
            Trace.Write(e.Command.Parameters(x).ParameterName & ": " & e.Command.Parameters(x).Value)
        Next
    End Sub
End Class
