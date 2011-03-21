Imports emailer
Imports InsertFile
Partial Class order
    Inherits System.Web.UI.Page
    Dim _db As Database = DatabaseFactory.CreateDatabase("OPSConn")
    Protected Sub ddlVendor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlVendor.SelectedIndexChanged
        If ddlVendor.SelectedIndex > 0 Then
            pnlSelectVendor.Visible = False
            pnlOrder.Visible = True
        Else
            pnlSelectVendor.Visible = True
            pnlOrder.Visible = False
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim cmd As DbCommand = _db.GetStoredProcCommand("page_info")
            _db.AddInParameter(cmd, "emp_id", DbType.Int32, Session("emp_id"))

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)
            dr.Read()

            lblAuthorizerName.Text = dr("approver_name")
            txtContact.Text = Session("name")
            txtPhone.Text = dr("campus_phone")
            lblDept.Text = ConfigurationManager.AppSettings("Dept_Name")
        End If
    End Sub
    Protected Sub CustomValidator1_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator1.ServerValidate
        If txtExceed1.Text = "" And txtCost1.Text = "" Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator2_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator2.ServerValidate
        If txtQty2.Text <> "" And txtUnit2.Text <> "" And txtDescription2.Text <> "" And (txtExceed2.Text <> "" Or txtCost2.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty2.Text <> "" Or txtUnit2.Text <> "" Or txtDescription2.Text <> "" Or txtExceed2.Text <> "" Or txtCost2.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #2: ")
                If txtQty2.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit2.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription2.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed2.Text = "" And txtCost2.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator2.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator3_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator3.ServerValidate
        If txtQty3.Text <> "" And txtUnit3.Text <> "" And txtDescription3.Text <> "" And (txtExceed3.Text <> "" Or txtCost3.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty3.Text <> "" Or txtUnit3.Text <> "" Or txtDescription3.Text <> "" Or txtExceed3.Text <> "" Or txtCost3.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #3: ")
                If txtQty3.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit3.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription3.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed3.Text = "" And txtCost3.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator3.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator4_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator4.ServerValidate
        If txtQty4.Text <> "" And txtUnit4.Text <> "" And txtDescription4.Text <> "" And (txtExceed4.Text <> "" Or txtCost4.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty4.Text <> "" Or txtUnit4.Text <> "" Or txtDescription4.Text <> "" Or txtExceed4.Text <> "" Or txtCost4.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #4: ")
                If txtQty4.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit4.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription4.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed4.Text = "" And txtCost4.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator4.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator5_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator5.ServerValidate
        If txtQty5.Text <> "" And txtUnit5.Text <> "" And txtDescription5.Text <> "" And (txtExceed5.Text <> "" Or txtCost5.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty5.Text <> "" Or txtUnit5.Text <> "" Or txtDescription5.Text <> "" Or txtExceed5.Text <> "" Or txtCost5.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #5: ")
                If txtQty5.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit5.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription5.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed5.Text = "" And txtCost5.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator5.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator6_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator6.ServerValidate
        If txtQty6.Text <> "" And txtUnit6.Text <> "" And txtDescription6.Text <> "" And (txtExceed6.Text <> "" Or txtCost6.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty6.Text <> "" Or txtUnit6.Text <> "" Or txtDescription6.Text <> "" Or txtExceed6.Text <> "" Or txtCost6.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #6: ")
                If txtQty6.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit6.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription6.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed6.Text = "" And txtCost6.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator6.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator7_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator7.ServerValidate
        If txtQty7.Text <> "" And txtUnit7.Text <> "" And txtDescription7.Text <> "" And (txtExceed7.Text <> "" Or txtCost7.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty7.Text <> "" Or txtUnit7.Text <> "" Or txtDescription7.Text <> "" Or txtExceed7.Text <> "" Or txtCost7.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #7: ")
                If txtQty7.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit7.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription7.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed7.Text = "" And txtCost7.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator7.ErrorMessage = strError.ToString
            End If
        End If

    End Sub
    Protected Sub CustomValidator8_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator8.ServerValidate
        If txtQty8.Text <> "" And txtUnit8.Text <> "" And txtDescription8.Text <> "" And (txtExceed8.Text <> "" Or txtCost8.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty8.Text <> "" Or txtUnit8.Text <> "" Or txtDescription8.Text <> "" Or txtExceed8.Text <> "" Or txtCost8.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #8: ")
                If txtQty8.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit8.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription8.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed8.Text = "" And txtCost8.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator8.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator9_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator9.ServerValidate
        If txtQty9.Text <> "" And txtUnit9.Text <> "" And txtDescription9.Text <> "" And (txtExceed9.Text <> "" Or txtCost9.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty9.Text <> "" Or txtUnit9.Text <> "" Or txtDescription9.Text <> "" Or txtExceed9.Text <> "" Or txtCost9.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #9: ")
                If txtQty9.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit9.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription9.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed9.Text = "" And txtCost9.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator9.ErrorMessage = strError.ToString
            End If
        End If
    End Sub
    Protected Sub CustomValidator10_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator10.ServerValidate
        If txtQty10.Text <> "" And txtUnit10.Text <> "" And txtDescription10.Text <> "" And (txtExceed10.Text <> "" Or txtCost10.Text <> "") Then
            args.IsValid = True
        Else
            If txtQty10.Text <> "" Or txtUnit10.Text <> "" Or txtDescription10.Text <> "" Or txtExceed10.Text <> "" Or txtCost10.Text <> "" Then
                args.IsValid = False
                Dim strError As New StringBuilder
                strError.Append("An error occurred with item #10: ")
                If txtQty10.Text = "" Then
                    strError.Append("quantity can not be blank;")
                End If
                If txtUnit10.Text = "" Then
                    strError.Append("units can not be blank;")
                End If
                If txtDescription10.Text = "" Then
                    strError.Append("description can not be blank;")
                End If
                If txtExceed10.Text = "" And txtCost10.Text = "" Then
                    strError.Append("either 'not to exceed' or 'unit cost' must be supplied;")
                End If
                CustomValidator10.ErrorMessage = strError.ToString
            End If
        End If
    End Sub

    Protected Sub CustomValidator12_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator12.ServerValidate
        If txtCost1.Text <> "" And txtQty1.Text <> "" Then
            If (CInt(txtCost1.Text) * CInt(txtQty1.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator13_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator13.ServerValidate
        If txtCost2.Text <> "" And txtQty2.Text <> "" Then
            If (CInt(txtCost2.Text) * CInt(txtQty2.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator14_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator14.ServerValidate
        If txtCost3.Text <> "" And txtQty3.Text <> "" Then
            If (CInt(txtCost3.Text) * CInt(txtQty3.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator15_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator15.ServerValidate
        If txtCost4.Text <> "" And txtQty4.Text <> "" Then
            If (CInt(txtCost4.Text) * CInt(txtQty4.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator16_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator16.ServerValidate
        If txtCost5.Text <> "" And txtQty5.Text <> "" Then
            If (CInt(txtCost5.Text) * CInt(txtQty5.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator17_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator17.ServerValidate
        If txtCost6.Text <> "" And txtQty6.Text <> "" Then
            If (CInt(txtCost6.Text) * CInt(txtQty6.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator18_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator18.ServerValidate
        If txtCost7.Text <> "" And txtQty7.Text <> "" Then
            If (CInt(txtCost7.Text) * CInt(txtQty7.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator19_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator19.ServerValidate
        If txtCost8.Text <> "" And txtQty9.Text <> "" Then
            If (CInt(txtCost9.Text) * CInt(txtQty9.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub CustomValidator20_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator20.ServerValidate
        If txtCost10.Text <> "" And txtQty10.Text <> "" Then
            If (CInt(txtCost10.Text) * CInt(txtQty10.Text)) > 214748.36 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            args.IsValid = True
        End If
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Page.IsValid Then

            pnlOrder.Visible = False
            pnlReview.Visible = True
            lblContact.Text = txtContact.Text
            lblPhone.Text = txtPhone.Text
            lblAuthorizer.Text = ddlAuthorizer.SelectedItem.Text
            lblAcct.Text = txtAcct.Text
            lblShipping.Text = ddlShipping.SelectedValue
            lblShipto.Text = ddlShipStation.SelectedValue
            If ckbxRadioactive.Checked Then
                lblRadioactive.Text = "Yes"
                lblRUA.Text = txtRUA.Text
            Else
                lblRadioactive.Text = "No"
                lblRUA.Text = ""
            End If

            If ckbxControlledSubstance.Checked Then
                lblControlledSubstance.Text = "Yes"
                lblCSClass.Text = txtCSClass.Text
                lblCSUse.Text = txtCSUse.Text
                lblCSStore.Text = txtCSStore.Text
                lblCSCustodian.Text = txtCSCustodian.Text
                lblCSUsers.Text = txtCSUser.Text
            Else
                lblControlledSubstance.Text = "No"
                lblCSClass.Text = ""
                lblCSUse.Text = ""
                lblCSStore.Text = ""
                lblCSCustodian.Text = ""
                lblCSUsers.Text = ""
            End If

            If ckbxQuote.Checked Then
                lblQuote.Text = "Yes"
            Else
                lblQuote.Text = "No"
            End If
            lblVContact.Text = txtVendor.Text
            lblComments.Text = txtComment.Text



            Dim i As Integer

            Dim tqty As TextBox
            Dim tunit As TextBox
            Dim titem As TextBox
            Dim tdesc As TextBox
            Dim texceed As TextBox
            Dim tcost As TextBox

            Dim lqty As Label
            Dim lunit As Label
            Dim litem As Label
            Dim ldesc As Label
            Dim lexceed As Label
            Dim lcost As Label
            Dim ltotal As Label
            Dim rtotal As Decimal
            Dim tax As Decimal
            Dim tr As HtmlTableRow

            For i = 1 To 10
                tqty = Master.FindControl("ContentPlaceHolder1").FindControl("txtQty" & i)
                tunit = Master.FindControl("ContentPlaceHolder1").FindControl("txtUnit" & i)
                titem = Master.FindControl("ContentPlaceHolder1").FindControl("txtItem" & i)
                tdesc = Master.FindControl("ContentPlaceHolder1").FindControl("txtDescription" & i)
                texceed = Master.FindControl("ContentPlaceHolder1").FindControl("txtExceed" & i)
                tcost = Master.FindControl("ContentPlaceHolder1").FindControl("txtCost" & i)
                lqty = Master.FindControl("ContentPlaceHolder1").FindControl("lblQty" & i)
                lunit = Master.FindControl("ContentPlaceHolder1").FindControl("lblUnit" & i)
                litem = Master.FindControl("ContentPlaceHolder1").FindControl("lblItem" & i)
                ldesc = Master.FindControl("ContentPlaceHolder1").FindControl("lblDescription" & i)
                lexceed = Master.FindControl("ContentPlaceHolder1").FindControl("lblExceed" & i)
                lcost = Master.FindControl("ContentPlaceHolder1").FindControl("lblCost" & i)
                tr = Master.FindControl("ContentPlaceHolder1").FindControl("tr" & i)
                ltotal = Master.FindControl("ContentPlaceHolder1").FindControl("lblTotal" & i)

                If tqty.Text <> "" Then
                    tr.Visible = True
                    lqty.Text = tqty.Text
                    lunit.Text = tunit.Text
                    litem.Text = titem.Text
                    ldesc.Text = tdesc.Text
                    lexceed.Text = texceed.Text

                    If tcost.Text <> "" Then
                        ltotal.Text = FormatNumber(tcost.Text * tqty.Text, 2)
                        lcost.Text = tcost.Text
                    Else
                        ltotal.Text = "0"
                        lcost.Text = "0"
                    End If

                    rtotal = rtotal + ltotal.Text
                Else
                    tr.Visible = False
                End If

            Next
            lblSubTotal.Text = FormatNumber(rtotal, 2)
            tax = FormatNumber(rtotal * ConfigurationManager.AppSettings("tax_value"), 2)
            lblTax.Text = tax.ToString
            lblTotal.Text = FormatNumber(rtotal, 2) + tax
        End If
    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        sdsInsertOrderRequest.Insert()
    End Sub

    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        pnlOrder.Visible = True
        pnlReview.Visible = False
    End Sub

    Protected Sub sdsInsertOrderRequest_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs) Handles sdsInsertOrderRequest.Inserted
        If e.AffectedRows > 0 Then



            Dim bolUrgent As Boolean = False

            Dim i As Integer

            Dim tqty As TextBox
            Dim tunit As TextBox
            Dim titem As TextBox
            Dim tdesc As TextBox
            Dim texceed As TextBox
            Dim tcost As TextBox
            Dim intRows As Integer

            Dim cmd As DbCommand = _db.GetStoredProcCommand("insert_details")
            _db.AddInParameter(cmd, "order_id", DbType.Int32, e.Command.Parameters("@order_id").Value)
            _db.AddInParameter(cmd, "quantity", DbType.Int32)
            _db.AddInParameter(cmd, "unit", DbType.String)
            _db.AddInParameter(cmd, "item", DbType.String)
            _db.AddInParameter(cmd, "description", DbType.String)
            _db.AddInParameter(cmd, "exceed", DbType.Currency)
            _db.AddInParameter(cmd, "cost", DbType.Currency)

            For i = 1 To 10
                tqty = Master.FindControl("ContentPlaceHolder1").FindControl("txtQty" & i)
                tunit = Master.FindControl("ContentPlaceHolder1").FindControl("txtUnit" & i)
                titem = Master.FindControl("ContentPlaceHolder1").FindControl("txtItem" & i)
                tdesc = Master.FindControl("ContentPlaceHolder1").FindControl("txtDescription" & i)
                texceed = Master.FindControl("ContentPlaceHolder1").FindControl("txtExceed" & i)
                tcost = Master.FindControl("ContentPlaceHolder1").FindControl("txtCost" & i)


                If tqty.Text <> "" Then
                    _db.SetParameterValue(cmd, "quantity", tqty.Text)
                    _db.SetParameterValue(cmd, "unit", tunit.Text)
                    _db.SetParameterValue(cmd, "item", titem.Text)
                    _db.SetParameterValue(cmd, "description", tdesc.Text)
                    If texceed.Text = "" Then
                        _db.SetParameterValue(cmd, "exceed", DBNull.Value)
                    Else
                        _db.SetParameterValue(cmd, "exceed", texceed.Text)
                    End If
                    If tcost.Text = "" Then
                        _db.SetParameterValue(cmd, "cost", DBNull.Value)
                    Else
                        _db.SetParameterValue(cmd, "cost", tcost.Text)
                    End If
                    intRows = _db.ExecuteNonQuery(cmd)
                    If intRows <> 1 Then
                        Throw New ArgumentException("Error submitting row # " & i)
                    End If
                End If
            Next

            pnlReview.Visible = False

            lblTag.Text = e.Command.Parameters("@full_tag").Value
            lblTag1.Text = e.Command.Parameters("@full_tag").Value
            HyperLink1.NavigateUrl = "mailto:" & e.Command.Parameters("@pemail_address").Value
            HyperLink2.NavigateUrl = "mailto:" & e.Command.Parameters("@pemail_address").Value
            HyperLink1.Text = e.Command.Parameters("@pemail_name").Value
            HyperLink2.Text = e.Command.Parameters("@pemail_name").Value
            lblREmailAddress.Text = e.Command.Parameters("@remail_address").Value
            lblREmailName.Text = e.Command.Parameters("@remail_name").Value
            lblTest.Text = e.Command.Parameters("@remail_address").Value
            lblEmailAddress.Text = e.Command.Parameters("@email_address").Value
            lblEmailName.Text = e.Command.Parameters("@email_name").Value
            lblOrderID.Text = e.Command.Parameters("@order_id").Value

            If e.Command.Parameters("@approved").Value Then
                If e.Command.Parameters("@auto_budget").Value Then
                    lblApproved.Text = "Purch"
                Else
                    lblApproved.Text = "Budget"
                End If
            Else
                lblApproved.Text = "No"
            End If

            Dim sbBody As New StringBuilder

            If ddlShipping.SelectedValue = "Pick Up" Or ddlShipping.SelectedValue = "Overnight" Or ddlShipping.SelectedValue = "2-day" Or ddlShipping.SelectedValue = "3-day" Then
                bolUrgent = True
            End If

            If lblApproved.Text = "Purch" Then
                pnlApproverConfirm.Visible = True
                email_purch_post_new_order(bolUrgent, Session("name"), e.Command.Parameters("@email_address").Value)
            ElseIf lblApproved.Text = "Budget" Then
                pnlApproverConfirm.Visible = True
                email_budget_post_new_order(bolUrgent, Session("name"), e.Command.Parameters("@email_address").Value)
            Else
                pnlSubmitterConfirm.Visible = True
                email_approver_post_new_order(bolUrgent, Session("name"), e.Command.Parameters("@email_address").Value)
            End If


            If ckbxQuote.Checked Then
                pnlQuote.Visible = True
            End If
        End If
    End Sub

    Protected Sub sdsInsertOrderRequest_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceCommandEventArgs) Handles sdsInsertOrderRequest.Inserting
        e.Command.Parameters("@fiscal_year").Value = ConfigurationManager.AppSettings("fiscal_year")
        e.Command.Parameters("@app_id").Value = ConfigurationManager.AppSettings("app_id")

        For x As Integer = 0 To e.Command.Parameters.Count - 1
            Trace.Write(e.Command.Parameters(x).ParameterName & ": " & e.Command.Parameters(x).Value)
        Next
    End Sub

    Protected Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
        If Page.IsValid Then
            If fuQuote.HasFile Then
                file_upload(fuQuote, lblOrderID.Text)
            End If
        End If
        gvAttachments.DataBind()
    End Sub

    Protected Sub CustomValidator22_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvalQuote.ServerValidate
        args.IsValid = False
        
        args.IsValid = InsertFile.check_extension(fuQuote, cvalQuote)

    End Sub

    Protected Sub sdsFiles_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceSelectingEventArgs) Handles sdsFiles.Selecting
        If lblOrderID.Text <> "" Then
            e.Command.Parameters("@order_id").Value = lblOrderID.Text
        End If
    End Sub

    Protected Sub ckbxControlledSubstance_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ckbxControlledSubstance.CheckedChanged
        If ckbxControlledSubstance.Checked Then
            pnlCS.Visible = True
        Else
            pnlCS.Visible = False
        End If
    End Sub

    Protected Sub CustomValidator11_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator11.ServerValidate
        If lblAuthorizerName.Text = Left(ddlAuthorizer.SelectedItem.Text, Len(lblAuthorizerName.Text)) And txtAcct.Text = "" Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub
End Class
