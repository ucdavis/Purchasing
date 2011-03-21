Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Xml

Partial Class login
    Inherits System.Web.UI.Page
    Private CASHOST As String = "https://cas.ucdavis.edu/cas/"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim tkt As String = Request.QueryString("ticket")
        Dim service As String = Request.Url.ToString

        If tkt Is Nothing Then
            Dim redir As String = CASHOST & "login?" & "service=" & service
            Response.Redirect(redir)
        End If

        Dim validateurl As String = CASHOST & "serviceValidate?ticket=" & tkt & "&service=" & service
        Dim sreader As StreamReader = New StreamReader(New WebClient().OpenRead(validateurl))
        Dim resp As String = sreader.ReadToEnd

        Dim nt As NameTable = New NameTable()
        Dim nsmgr As XmlNamespaceManager = New XmlNamespaceManager(nt)
        Dim context As XmlParserContext = New XmlParserContext(nt, nsmgr, "", XmlSpace.None)
        Dim xreader As XmlTextReader = New XmlTextReader(resp, XmlNodeType.Element, context)

        Dim netid As String = ""

        While xreader.Read()
            If xreader.IsStartElement() Then
                Dim tag As String = xreader.LocalName
                If tag = "user" Then
                    netid = xreader.ReadString()
                End If
            End If
        End While
        xreader.Close()
        If netid = "" Then
            Label1.Text = "CAS returned to this application, but then refused to validate your identity"
        Else
            Label1.Text = "Welcome " & netid
        End If
        FormsAuthentication.RedirectFromLoginPage(netid, False)





    End Sub
End Class
