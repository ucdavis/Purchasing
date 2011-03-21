<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false"
    CodeFile="home.aspx.vb" Inherits="home"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="pnlStaff" runat="server" Visible="False" Width="100%">
        <table align="center" border="0" cellpadding="0" cellspacing="0">
            <tbody>
                <tr>
                    <td>
                        <h2>
                            Admin MENU</h2>   
                                     
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <b><a href="view.aspx">View All Order Requests </a></b>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <a href="admin_approve.aspx"><strong>Approve Pending Order Requests </strong></a>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <strong><a href="budget.aspx">Budget Verification</a></strong>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <strong><a href="process.aspx">Process Order Requests</a></strong>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <b><a href="vendors.aspx">List of Vendors</a></b>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">                       
                            <b><a href="approvers.aspx">List of Approvers</a>&#160;</b>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <b><a href="members.aspx">List of Members</a></b></td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <b><a href="building_list.aspx">List of Buildings</a></b></td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">                        
                            <a href="budget_setup.aspx"><strong>Budget Asst Setup</strong></a>
                    </td>
                </tr>
                <tr>
                    <td style="height: 40px" valign="top">
                        <b><a href="history_admin.aspx">Detailed Histories</a>&nbsp;</b>
                    </td>
                </tr>
            </tbody>
        </table>
        <hr />
    </asp:Panel>
    &nbsp;&nbsp;<br />
    <table align="center" border="0" cellpadding="0" cellspacing="0">
        <tbody>
            <tr>
                <td style="height: 40px" valign="top">
                    <strong><a href="order.aspx">Submit New Order Request</a></strong>
                </td>
            </tr>
            <tr>
                <td style="height: 40px" valign="top">
                    <strong><a href="view_orders.aspx">View Your Order Requests</a></strong>
                </td>
            </tr>
            <tr>
                <td style="height: 40px" valign="top">
                   <strong><a href="edit_orders.aspx">Edit Orders</a></strong>
                </td>
            </tr>
            <tr id="trApprover" runat="server">
                <td style="height: 40px" valign="top">
                    <strong><a href="approve.aspx">Order Submitted for Your Approval</a></strong>
                </td>
            </tr>
            <tr>
                <td style="height: 40px" valign="top">
                    <strong><a href="history.aspx">View All Detailed Histories</a></strong>
                </td>
            </tr>
            <tr id="trReviewer" runat="server">
                <td style="height: 40px" valign="top">
                    <strong><a href="view.aspx">Review All Order Request</a></strong>
                </td>
            </tr>
        </tbody>
    </table>
    <br />
</asp:Content>
