Imports Microsoft.VisualBasic
Imports System.Net.Mail

Public Class emailer

    Shared _bolDevel As Boolean = True
    Shared _db As Database = DatabaseFactory.CreateDatabase("OPSConn")
    Public Shared Sub email_purch_post_receive(ByVal strReceiverName As String, ByVal strTo As String, ByVal strStatus As String, ByVal strTag As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "Order receive status updated"
        sbBody.Append("The receive status of Tag " & strTag & " was updated by " & strReceiverName)
        sbBody.Append(". The order is now marked as " & strStatus & ".")
        sbBody.Append("<br /><br />To complete receiving for this request, log in to ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "process.aspx'>Online Purchasing System</a>")
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, False)
    End Sub
    Public Shared Sub email_purch_post_new_order(ByVal urgent As Boolean, ByVal strRequestorName As String, ByVal strTo As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: Order pending final purchasing"
            sbBody.Append("<font color='red'>A new order request was submitted by " & strRequestorName)
            sbBody.Append(" , who is listed as the approver for this order. It has been automatically approved by budget as well.</font>")
            sbBody.Append("<br /><br />To complete purchasing for this request, log in to ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order request pending final purchasing"
            sbBody.Append("A new order request was submitted by " & strRequestorName)
            sbBody.Append(" , who is listed as the approver for this order. It has been automatically approved by budget as well.</font>")
            sbBody.Append("<br /><br />To complete purchasing for this request, log in to ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "process.aspx'>Online Purchasing System</a>")
        End If
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub
    Public Shared Sub email_budget_post_new_order(ByVal urgent As Boolean, ByVal strRequestorName As String, ByVal strTo As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: Order pending budget verification"
            sbBody.Append("<font color='red'>A new order request was submitted by " & strRequestorName)
            sbBody.Append(" , who is listed as the approver for this order, so it was automatically approved.</font>")
            sbBody.Append("<br /><br />To complete purchasing for this request, log in to ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order request pending budget verification"
            sbBody.Append("A new order request was submitted by " & strRequestorName)
            sbBody.Append(" , who is listed as the approver for this order, so it was automatically approved.")
            sbBody.Append("<br /><br />To complete purchasing for this request, log in to ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "budget.aspx'>Online Purchasing System</a>")
        End If
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub

    Public Shared Sub email_approver_post_new_order(ByVal urgent As Boolean, ByVal strRequestorName As String, ByVal strTo As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: Order request pending approval"
            sbBody.Append("<font color='red'>An order request has been submitted by " & strRequestorName & " and is pending your ")
            sbBody.Append("approval.</font><br /><br />To review and act on this request, log in to ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order request pending approval"
            sbBody.Append("An order request has been submitted by " & strRequestorName & " and is pending your ")
            sbBody.Append("approval.<br /><br />To review and act on this request, log in to ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "approve.aspx'>Online Purchasing System</a>")
        End If
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub
    Public Shared Sub email_budget_post_approve(ByVal urgent As Boolean, ByVal strRequestorName As String, ByVal strApprover As String, ByVal strTo As String, ByVal strComments As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: order pending budget approval"
            sbBody.Append("<font color='red'>An order request submitted by " & strRequestorName & " has been approved by " & strApprover & ".")
            If strComments <> "" Then
                sbBody.Append("<br/><b>Approver Comments:</b> <font color='blue'>" & strComments & "</font>")
            End If
            sbBody.Append("<br><br></font>To complete budget approval for this request, log in to the ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order pending budget approval"
            sbBody.Append("An order request submitted by " & strRequestorName & " has been approved by " & strApprover & ".")
            If strComments <> "" Then
                sbBody.Append("<br/><b>Approver Comments:</b> <font color='blue'>" & strComments & "</font>")
            End If
            sbBody.Append("<br><br>To complete budget approval for this request, log in to the ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "budget.aspx'>Online Purchasing System</a>")
        End If
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub
    Public Shared Sub email_purch_post_approve(ByVal urgent As Boolean, ByVal strRequestorName As String, ByVal strApprover As String, ByVal strTo As String, ByVal strComments As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: order pending final purchasing"
            sbBody.Append("<font color='red'>An order request submitted by " & strRequestorName & ", was approved by " & strApprover & " and auto approved by budget.")
            If strComments <> "" Then
                sbBody.Append("<br/><b>Approver Comments:</b>  <font color='blue'>" & strComments & "</font>")
            End If
            sbBody.Append("<br><br></font>To complete purchasing for this request, log in to the ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order pending final purchasing"
            sbBody.Append("An order request submitted by " & strRequestorName & ", was approved by " & strApprover & " and auto approved by budget.")
            If strComments <> "" Then
                sbBody.Append("<br/><b>Approver Comments:</b> <font color='blue'>" & strComments & "</font>")
            End If
            sbBody.Append("<br><br>To complete purchasing for this request, log in to the ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "process.aspx'>Online Purchasing System</a>")
        End If
        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub
    Public Shared Sub email_deny_by_approver(ByVal strApprover As String, ByVal strTo As String, ByVal strComments As String, ByVal strVendor As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "Order request denied by approver"
        sbBody.Append("Your order request to " & strVendor & " was denied by the approver, " & strApprover & ".")
        sbBody.Append("<br /><b>Approver Comments:</b><font color='blue'> " & strComments & "</font><br .><br />")
        sbBody.Append("This request is now closed and no further action will be taken.")
        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "view_orders.aspx'>Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, False)
    End Sub
    Public Shared Sub email_purch_post_budget(ByVal urgent As Boolean, ByVal strBudget As String, ByVal strRequestorName As String, ByVal strTo As String, ByVal strComments As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        If urgent Then
            strSubject = "Rush: order pending final purchasing"
            sbBody.Append("<font color='red'>An order request submitted by " & strRequestorName & ", was approved by a budget.")
            If strComments <> "" Then
                sbBody.Append("<br/><b>BudgetComments:</b><font color='blue'> " & strComments & "</font>")
            End If
            sbBody.Append("<br><br></font>To complete purchasing for this request, log in to the ")
            sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")
        Else
            strSubject = "Order pending final purchasing"
            sbBody.Append("An order request submitted by " & strRequestorName & ", was approved by budget.")
            If strComments <> "" Then
                sbBody.Append("<br/><b>Budget Comments:</b> <font color='blue'>" & strComments & "</font>")
            End If
            sbBody.Append("<br><br>To complete purchasing for this request, log in to the ")
            sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "process.aspx'>Online Purchasing System</a>")
        End If

        Dim maTo As MailAddress = New MailAddress(strTo)
        email_single(sbBody.ToString, maTo, strSubject, urgent)
    End Sub

    Public Shared Sub email_deny_by_budget(ByVal strBudget As String, ByVal strOrderer As String, ByVal strApprover As String, ByVal strComments As String, ByVal strVendor As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "Order request denied by budget"
        sbBody.Append("Your order request to " & strVendor & " was denied for budgetary reasons, " & strBudget & ".")
        sbBody.Append("<br /><b>Budget Comments:</b> <font color='blue'>" & strComments & "</font><br .><br />")
        sbBody.Append("This request is now closed and no further action will be taken.")
        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "view_orders.aspx'>Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strOrderer)
        Dim maCC As MailAddress = New MailAddress(strApprover)
        email_cc(sbBody.ToString, maTo, maCC, strSubject)
    End Sub
    Public Shared Sub email_post_purch_placed(ByVal strTag As String, ByVal strVendor As String, ByVal strRequestorName As String, ByVal strApproverName As String, ByVal strComments As String, ByVal strRequestorEmail As String, ByVal strPurchName As String, ByVal strPO As String, ByVal strPurhPhone As String, ByVal strDept As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "OPS Order completed by purchasing"

        sbBody.Append("The order (Tag# " & strTag & ") to " & strVendor & " submitted by " & strRequestorName & " and approved by " & strApproverName)
        sbBody.Append(" has been completed by " & strPurchName & ".<br />")
        If strComments <> "" Then
            sbBody.Append("Order ETA/Delivery/Purchasing Comments: <font color='blue'>" & strComments & "</font><br />")
        End If
        If strPO <> "" Then
            sbBody.Append("PO/Confirmation #: " & strPO & "<br />")
        End If
        sbBody.Append("Purchaser: " & strPurchName & "<br />")
        If strPurhPhone <> "" Then
            sbBody.Append("Phone: " & strPurhPhone & "<br/>")
        End If
        sbBody.Append("<br/>Once your order is received, please verify that the items have been received ")
        sbBody.Append("and are ok to process for payment, please <b>SIGN & DATE</b> the packing slip ")
        sbBody.Append("and send immediately to: " & strPurchName & ", " & strDept & " Business Office mailbox. ")
        sbBody.Append("If you receive any items that are damaged and/or there are problems ")
        sbBody.Append("with your order, please contact " & strPurchName & ". Thank you.")
        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "view_orders.aspx'>Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strRequestorEmail)
        email_single(sbBody.ToString, maTo, strSubject, False)
    End Sub
    Public Shared Sub email_hr_post_purch(ByVal strOwner As String, ByVal strPhone As String, ByVal strRequestorName As String, ByVal strApproverName As String, ByVal strPurchName As String, ByVal strHREmail As String, ByVal intOrderID As Integer)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        Dim curTotal As Decimal = 0
        strSubject = "OPS Order completed by purchasing: Contains Cell Phone"

        sbBody.Append("An OPS order submitted by " & strRequestorName & " and approved by " & strApproverName)
        sbBody.Append(" has been completed by " & strPurchName & ".<br />")

        sbBody.Append("<br/>This order was marked as containing a cell phone, and therefore needs to be reviewed for tax purposes. ")
        sbBody.Append("<br/><br/>Cell Owner: " & strOwner)
        sbBody.Append("<br/>Cell Number: " & strPhone)
        sbBody.Append("<br/><br/> Item details are as follows: <br /><br />")
        sbBody.Append("<table border='1'><tr><th>qty</th><th>unit</th><th>Description</th><th>Not to exceed</th><th>Unit cost</th><th>Total</th></tr>")
        Dim cmd As DbCommand = _db.GetStoredProcCommand("order_info_details")
        _db.AddInParameter(cmd, "order_id", DbType.Int32, intOrderID)

        Dim dr As SqlDataReader = _db.ExecuteReader(cmd)

        While dr.Read
            sbBody.Append("<tr><td>" & dr("quantity") & "</td><td>" & dr("unit") & "</td><td>" & dr("description") & "</td><td>" & String.Format("{0:c}", dr("exceed")) & "</td><td>" & String.Format("{0:c}", dr("cost")) & "</td><td>" & String.Format("{0:c}", dr("total")) & "</td></tr>")
            curTotal += dr("total")
        End While
        dr.Close()

        sbBody.Append("<tr><th colspan='5' align='right'>Item Total:</th><td>" & String.Format("{0:c}", curTotal) & "</td></tr></table>")
        sbBody.Append("<br /><br />Please review the order details and complete the Imputed Income/Cash Allowance Form found at <a href='http://cellphones.ucdavis.edu/'>cellphones.ucdavis.edu</a>.")

        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href=" & ConfigurationManager.AppSettings("url") & ">Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strHREmail)
        email_single(sbBody.ToString, maTo, strSubject, False)
    End Sub
    Public Shared Sub email_post_purch_campus_purch(ByVal strTag As String, ByVal strVendor As String, ByVal strRequestorName As String, ByVal strApproverName As String, ByVal strComments As String, ByVal strRequestorEmail As String, ByVal strPurchName As String, ByVal strPurhPhone As String, ByVal strDept As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "OPS order submitted to campus purchasing"

        sbBody.Append("The order (Tag# " & strTag & ") to " & strVendor & " submitted by " & strRequestorName & " and approved by " & strApproverName)
        sbBody.Append(" has been submitted to the campus purchasing unit by " & strPurchName & ".<br />")
        If strComments <> "" Then
            sbBody.Append("Order ETA/Delivery/Purchasing Comments: <font color='blue'>" & strComments & "</font><br />")
        End If
        sbBody.Append("Purchaser: " & strPurchName & "<br />")
        If strPurhPhone <> "" Then
            sbBody.Append("Phone: " & strPurhPhone & "<br/>")
        End If

        sbBody.Append("<br/>Your order has been submitted to the UCD campus purchasing department for final ")
        sbBody.Append("processing. The order should be assigned a purchaser at the UCD campus purchasing ")
        sbBody.Append("department and placed within a few weeks. Please direct any ")
        sbBody.Append("questions " & strPurchName & " who can then follow-up with central purchasing ")
        sbBody.Append("on your behalf. Once your order is received, please verify that the items have been ")
        sbBody.Append("received and are ok to process for payment, please <b>SIGN & DATE</b> the packing ")
        sbBody.Append("and send immediately to: " & strPurchName & ", " & strDept & " Business Office mailbox. ")
        sbBody.Append("If you receive any items that are damaged and/or there are problems with your order, ")
        sbBody.Append("please contact " & strPurchName & ". Thank you.")
        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "view_orders.aspx'>Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strRequestorEmail)
        email_single(sbBody.ToString, maTo, strSubject, False)
    End Sub
    Public Shared Sub email_post_purch_denied(ByVal strTag As String, ByVal strVendor As String, ByVal strRequestorName As String, ByVal strApproverName As String, ByVal strComments As String, ByVal strRequestorEmail As String, ByVal strApproverEmail As String, ByVal strPurchName As String)
        Dim strSubject As String
        Dim sbBody As New StringBuilder
        strSubject = "OPS request denied by purchasing"

        sbBody.Append("The order (Tag# " & strTag & ") to " & strVendor & " submitted by " & strRequestorName & " and approved by " & strApproverName)
        sbBody.Append(" has been denied at the purchasing level by " & strPurchName & ".<br />")
        sbBody.Append("Purchasing comments: <font color='blue'>" & strComments & "</font><br />")
        sbBody.Append("This request is now closed and no further action will be taken.<br /><br />")
        sbBody.Append("<br/><br/>To view this order's details, please visit ")
        sbBody.Append("<a href='" & ConfigurationManager.AppSettings("url") & "view_orders.aspx'>Online Purchasing System</a>")

        Dim maTo As MailAddress = New MailAddress(strRequestorEmail)
        Dim maCC As MailAddress = New MailAddress(strApproverEmail)
        email_cc(sbBody.ToString, maTo, maCC, strSubject)
    End Sub
    Private Shared Sub email_single(ByVal sBody As String, ByVal maTo As MailAddress, ByVal sSubject As String, ByVal urgent As Boolean)
        Dim msg As New MailMessage

        If urgent Then
            msg.Priority = MailPriority.High
        Else
            msg.Priority = MailPriority.Normal
        End If
        If _bolDevel Then
            msg.To.Add("jscubbage@ucdavis.edu")
        Else
            msg.To.Add(maTo)
        End If


        msg.Body = sBody
        msg.IsBodyHtml = True
        msg.Subject = sSubject

        Dim smtp As New SmtpClient()
        smtp.Send(msg)

    End Sub
    Private Shared Sub email_cc(ByVal sBody As String, ByVal maTo As MailAddress, ByVal maCC As MailAddress, ByVal sSubject As String)
        Dim msg As New MailMessage

        If _bolDevel Then
            msg.To.Add("jscubbage@ucdavis.edu")
            msg.CC.Add("jscub@ucdavis.edu")
        Else
            msg.To.Add(maTo)
            msg.CC.Add(maCC)
        End If
        msg.Body = sBody
        msg.IsBodyHtml = True
        msg.Subject = sSubject

        Dim smtp As New SmtpClient()
        smtp.Send(msg)

    End Sub
End Class
