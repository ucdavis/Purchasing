Imports emailer
Partial Class approve
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
                lbl = e.Item.FindControl("lblShMat")
                lbl.Text = String.Format("{0:c}", ship_mat)


                lbl = e.Item.FindControl("lblsum")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat)

                tax = ConfigurationManager.AppSettings("tax_value") * (price_sum + ship_mat)
                lbl = e.Item.FindControl("lblTax")
                lbl.Text = String.Format("{0:c}", tax)

                lbl = e.Item.FindControl("lblSandH")
                lbl.Text = String.Format("{0:c}", ship_cost)

                lbl = e.Item.FindControl("lblTotal")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat + tax + ship_cost)

            End If
        End If
    End Sub

   

    Protected Sub FormView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles FormView1.DataBound

        Dim lbl As Label = FormView1.FindControl("statusLabel")
        Dim pnl As Panel = FormView1.FindControl("pnlActive")
        

        If Not lbl Is Nothing Then

            If lbl.Text = "Pending Approval" Then
                pnl.Visible = True
            Else
                pnl.Visible = False
            End If
        End If
    End Sub

    Protected Sub btnApprove_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Page.IsValid Then
            Dim bolUrgent As Boolean = False

            Dim cmd As DbCommand = _db.GetStoredProcCommand("approve_order")

            Dim txt As TextBox = FormView1.FindControl("txtID")
            _db.AddInParameter(cmd, "account", DbType.String, txt.Text)

            Dim ddlShip As DropDownList = FormView1.FindControl("ddlShip")
            _db.AddInParameter(cmd, "need_by", DbType.String, ddlShip.SelectedValue)

            _db.AddInParameter(cmd, "approved_by", DbType.String, Session("emp_id"))

            txt = FormView1.FindControl("txtComments")
            _db.AddInParameter(cmd, "approver_comments", DbType.String, txt.Text)

            _db.AddInParameter(cmd, "order_id", DbType.Int32, GridView1.SelectedValue)
            _db.AddOutParameter(cmd, "auto_budget", DbType.Boolean, 1)

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)

            dr.Read()

            If ddlShip.SelectedValue = "Pick Up" Or ddlShip.SelectedValue = "Overnight" Or ddlShip.SelectedValue = "2-day" Or ddlShip.SelectedValue = "3-day" Then
                bolUrgent = True
            End If

            Dim lbl As Label = FormView1.FindControl("contactLabel")

            If _db.GetParameterValue(cmd, "auto_budget") Then
                email_purch_post_approve(bolUrgent, Session("name"), lbl.Text, dr("kerberos_id") & "@ucdavis.edu", txt.Text)
            Else
                email_budget_post_approve(bolUrgent, Session("name"), lbl.Text, dr("kerberos_id") & "@ucdavis.edu", txt.Text)
            End If

            dr.Close()

        GridView1.SelectedIndex = -1
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        GridView1.SelectedIndex = -1
    End Sub

    Protected Sub btnDeny_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Page.IsValid Then
           
            Dim cmd As DbCommand = _db.GetStoredProcCommand("approver_deny_order")
            Dim txt As TextBox = FormView1.FindControl("txtComments")
            Dim lbl As Label = FormView1.FindControl("vendor_nameLabel")


            _db.AddInParameter(cmd, "approved_by", DbType.String, Session("emp_id"))

            _db.AddInParameter(cmd, "approver_comments", DbType.String, txt.Text)
            _db.AddInParameter(cmd, "order_id", DbType.Int32, GridView1.SelectedValue)

            Dim dr As SqlDataReader = _db.ExecuteReader(cmd)
            dr.Read()

            email_deny_by_approver(Session("name"), dr("kerberos_id") & "@ucdavis.edu", txt.Text, lbl.Text)


            dr.Close()


            GridView1.SelectedIndex = -1
        End If
    End Sub
End Class
