
Partial Class receipt
    Inherits System.Web.UI.Page

    Dim price_sum As Decimal
    Dim bolFirst As Boolean = True
    Dim ship_mat As Decimal
    Dim ship_cost As Decimal
    Dim tax As Decimal
    Dim taxvalue As Decimal


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session("role") = "approver" Or Session("role") = "orderer" Then
            Response.Redirect("home.aspx")
        End If
    End Sub
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
                    If e.Item.DataItem("tax") Then
                        taxvalue = ConfigurationManager.AppSettings("tax_value")
                    Else
                        tax = 0
                    End If
                End If
            ElseIf e.Item.ItemType = ListItemType.Footer Then
                lbl = e.Item.FindControl("lblShMat")
                lbl.Text = String.Format("{0:c}", ship_mat)


                lbl = e.Item.FindControl("lblsum")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat)

                tax = taxvalue * (price_sum + ship_mat)

                lbl = e.Item.FindControl("lblTax")
                lbl.Text = String.Format("{0:c}", tax)

                lbl = e.Item.FindControl("lblSandH")
                lbl.Text = String.Format("{0:c}", ship_cost)

                lbl = e.Item.FindControl("lblTotal")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat + tax + ship_cost)

                lbl = FormView1.FindControl("lblBalance")
                lbl.Text = String.Format("{0:c}", price_sum + ship_mat + tax + ship_cost)

            End If
        End If
    End Sub
End Class
