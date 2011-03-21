
Partial Class home
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("role") <> "approver" Then
            trApprover.Visible = False
        End If
        If Session("role") <> "reviewer" Then
            trReviewer.Visible = False
        End If
        If Session("role") <> "approver" And Session("role") <> "orderer" And Session("role") <> "reviewer" Then
            pnlStaff.Visible = True
        End If
    End Sub


  
End Class
