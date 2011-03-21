<%@ Import Namespace="System.IO"%>
<script language="VB" runat="server">
Sub Page_Load(sender As Object, e As EventArgs)
 
        If Session("app") <> "ops" Then
            Response.Redirect("home.aspx")
        End If
        
        Dim sbRoot As New StringBuilder
        Dim strFile As String = ""
        Dim strName As String = ""
        
        sbRoot.Append(ConfigurationManager.AppSettings("root_folder"))
        
        Dim db As Database = DatabaseFactory.CreateDatabase("OPSConn")
        Dim cmd As DbCommand = db.GetStoredProcCommand("download_file")
        db.AddInParameter(cmd, "emp_id", DbType.String, Session("emp_id"))
        db.AddInParameter(cmd, "order_id", DbType.Int32, Request.QueryString("order_id"))
        db.AddInParameter(cmd, "attach_id", DbType.Int32, Request.QueryString("attach_id"))
        
        Dim dr As SqlDataReader = db.ExecuteReader(cmd)
        
        While dr.Read
            sbRoot.Append(dr("app_id") & "\" & dr("fiscal_year") & "\" & dr("order_id") & "\")
            strFile = dr("attach_id") & dr("file_ext")
            strName = dr("attach_name")
        End While
        
        
            
       
        Dim filepath As String = sbRoot.ToString & "\" & strFile
        If Not filepath Is Nothing Then
            If File.Exists(filepath) And filepath.StartsWith(sbRoot.ToString) Then
                Dim filename As String = Path.GetFileName(filepath)
                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Response.AddHeader("Content-Disposition", "attachment; filename=""" & strName & """")
                Response.Flush()
                Response.WriteFile(filepath)
            End If
        Else
            Response.Write("Help")
        End If

End Sub
</script>