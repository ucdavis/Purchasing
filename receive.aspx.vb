Imports emailer
Partial Class receive
    Inherits System.Web.UI.Page
    Private _db As Database = DatabaseFactory.CreateDatabase("OPSConn")

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        sdsOrderRequest.SelectParameters("order_id").DefaultValue = -1
        gvOrders.SelectedIndex = -1
    End Sub
    Protected Sub btnCancel_Click1(ByVal sender As Object, ByVal e As System.EventArgs)
        ResetpnlProcess()
    End Sub

    Protected Sub ResetpnlProcess()
        sdsOrderRequest.SelectParameters("order_id").DefaultValue = 0
        gvOrders.SelectedIndex = -1
    End Sub


    Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btn As Button = sender
        'Response.Write("<BR>" & btn.CommandArgument)
        Dim cmd As DbCommand = _db.GetStoredProcCommand("recieve_detail_items")
        _db.AddInParameter(cmd, "detail_id", DbType.Int32, btn.CommandArgument)
        _db.AddInParameter(cmd, "receiver", DbType.Int32, Session("emp_id"))
        _db.ExecuteNonQuery(cmd)

        fvDetails.DataBind()

    End Sub

    Protected Sub btnRecieveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btn As Button = sender
        'Response.Write("<BR>" & btn.CommandArgument)

        Dim cmd As DbCommand = _db.GetStoredProcCommand("recieve_all_detail_items")
        _db.AddInParameter(cmd, "order_id", DbType.Int32, btn.CommandArgument)
        _db.AddInParameter(cmd, "receiver", DbType.Int32, Session("emp_id"))
        _db.ExecuteNonQuery(cmd)

        fvDetails.DataBind()
    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'Response.Write(fvDetails.DataKey.Value)
        Dim ddl As DropDownList = fvDetails.FindControl("ddlStatus")
        Dim txt As TextBox = fvDetails.FindControl("txtRecdComments")
        Dim cmd As DbCommand = _db.GetStoredProcCommand("update_recd_status")
        _db.AddInParameter(cmd, "order_id", DbType.Int32, fvDetails.DataKey.Value)
        _db.AddInParameter(cmd, "status", DbType.String, ddl.SelectedValue.ToString)
        _db.AddInParameter(cmd, "recd_comments", DbType.String, txt.Text)
        _db.AddInParameter(cmd, "updated_by", DbType.Int32, Session("emp_id"))
        _db.AddOutParameter(cmd, "purch_kerb", DbType.String, 50)
        _db.AddOutParameter(cmd, "tag", DbType.String, 50)


        _db.ExecuteNonQuery(cmd)
        Dim strTo As String = _db.GetParameterValue(cmd, "purch_kerb").ToString & "@ucdavis.edu"
        email_purch_post_receive(Session("name"), strTo, ddl.SelectedValue.ToString, _db.GetParameterValue(cmd, "tag").ToString)

        fvDetails.DataBind()
    End Sub

    
End Class
