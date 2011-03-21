Imports emailer
Partial Class hr_emailer
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        email_hr_post_purch("Fisher", "Lauri", "2-3902", "James", "Rob", "Sharon", "jscubbage@ucdavis.edu", 42)
    End Sub
End Class
