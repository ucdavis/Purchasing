Imports emailer
Partial Class budget
    Inherits System.Web.UI.Page
    Dim price_sum As Decimal
    Dim bolFirst As Boolean = True
    Dim ship_mat As Decimal
    Dim ship_cost As Decimal
    Dim tax As Decimal
    Private _db As Database = DatabaseFactory.CreateDatabase("OPSConn")

    Protected Sub Repeater1_ItemDataBound(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim lbl As Label

        If e.Item.ItemType = ListItemType.Header Then
            price_sum = 0
        Else
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                price_sum += e.Item.DataItem("total")
                If bolFirst Then
                    bolFirst = False
                    ship_mat = e.Item.DataItem("ship_mat")
                    ship_cost = e.Item.DataItem("ship_cost")

                End If
            ElseIf e.Item.ItemType = ListItemType.Footer Then


                lbl = e.Item.FindControl("lblsum")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat)

                tax = ConfigurationManager.AppSettings("tax_value") * (price_sum + ship_mat)
                lbl = e.Item.FindControl("lblTax")
                lbl.Text = String.Format("{0:c}", tax)


                lbl = e.Item.FindControl("lblTotal")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat + tax + ship_cost)

            End If
        End If
    End Sub
    Protected Sub gvUrgent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvUrgent.SelectedIndexChanged
        gvYour.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvUrgent.SelectedValue
    End Sub

    Protected Sub gvLog_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvYour.SelectedIndexChanged
        gvUrgent.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvYour.SelectedValue
    End Sub

    Protected Sub gvNotLog_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvOther.SelectedIndexChanged
        gvUrgent.SelectedIndex = -1
        gvYour.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = gvOther.SelectedValue
    End Sub

    Protected Sub btnApprove_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Page.IsValid Then
            Dim cmd As DbCommand = _db.GetStoredProcCommand("budget_approve_order")
            Dim txt As TextBox = FormView1.FindControl("txtID")
            Dim ship As Label = FormView1.FindControl("need_byLabel")
            Dim bolUrgent As Boolean = False

            ship.Text = FormView1.SelectedValue
            _db.AddInParameter(cmd, "account", DbType.String, txt.Text)
            _db.AddInParameter(cmd, "approved_by", DbType.Int32, Session("emp_id"))
            txt = FormView1.FindControl("txtComments")
            _db.AddInParameter(cmd, "approver_comments", DbType.String, txt.Text)
            _db.AddInParameter(cmd, "order_id", DbType.Int32, FormView1.SelectedValue)

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)

            dr.Read()

            If ship.Text = "Pick Up" Or ship.Text = "Overnight" Or ship.Text = "2-day" Or ship.Text = "3-day" Then
                bolUrgent = True
            End If

            Dim lbl As Label = FormView1.FindControl("contactLabel")

            email_purch_post_budget(bolUrgent, Session("name"), lbl.Text, dr("kerberos_id") & "@ucdavis.edu", txt.Text)

            gvUrgent.SelectedIndex = -1
            gvYour.SelectedIndex = -1
            gvOther.SelectedIndex = -1
            sdsOrderRequest.SelectParameters("order_id").DefaultValue = -1
            gvUrgent.DataBind()
            gvYour.DataBind()
            gvOther.DataBind()
        End If
    End Sub

    Protected Sub btnDeny_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Page.IsValid Then

            Dim cmd As DbCommand = _db.GetStoredProcCommand("budget_deny_order")

            _db.AddInParameter(cmd, "approved_by", DbType.Int32, Session("emp_id"))

            Dim txt As TextBox = FormView1.FindControl("txtComments")
            _db.AddInParameter(cmd, "approver_comments", DbType.String, txt.Text)

            _db.AddInParameter(cmd, "order_id", DbType.Int32, FormView1.SelectedValue)

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)
            Dim orderer As String
            Dim approver As String
            Dim lbl As Label = FormView1.FindControl("vendor_nameLabel")


            dr.Read()

            orderer = dr("kerberos_id") & "@ucdavis.edu"

            dr.NextResult()
            dr.Read()

            approver = dr("kerberos_id") & "@ucdavis.edu"

            email_deny_by_budget(Session("name"), orderer, approver, txt.Text, lbl.Text)

            dr.Close()

            gvUrgent.SelectedIndex = -1
            gvYour.SelectedIndex = -1
            gvOther.SelectedIndex = -1
            sdsOrderRequest.SelectParameters("order_id").DefaultValue = -1
            gvUrgent.DataBind()
            gvYour.DataBind()
            gvOther.DataBind()
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        gvUrgent.SelectedIndex = -1
        gvOther.SelectedIndex = -1
        gvYour.SelectedIndex = -1
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = -1
    End Sub

 
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") <> "Accounting Assistant" And Session("role") <> "Purchasing Assistant" And Session("role") <> "Department Authorizer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub
End Class
