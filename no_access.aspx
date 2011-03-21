<%@ Page Language="VB"  AutoEventWireup="false" CodeFile="no_access.aspx.vb" Inherits="no_access" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p>
    <span style="COLOR: #ff0000">ACCESS DENIED<br />
    <br />
    </span><span style="COLOR: #000000">These pages are only for authorized, current
    <asp:Label ID="lblDept" runat="server" Text="Label"></asp:Label>
&nbsp;employees with a kerberos account. To gain access to the Online Purchasing 
    System (OPS) you must contact
    <asp:HyperLink ID="hlemail" runat="server"><span style="COLOR: #000000">business 
    office</span></asp:HyperLink>
    . Please include your Kerberos ID with new user requests. Do NOT send your 
    password!</span><br />
</p>
    </div>
    </form>
</body>
</html>
    

