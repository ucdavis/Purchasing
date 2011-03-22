<%@ Page Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="ba.aspx.cs" Inherits="business_purchasing_orders_ba" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../blocks/ba_items.ascx" TagName="ba_items" TagPrefix="uc1" %>
<%@ Register Src="../blocks/addedit_vendor.ascx" TagName="addedit_vendor" TagPrefix="uc3" %>
<asp:Content ID="Content0" runat="server" ContentPlaceHolderID="HeadContentPlaceHolder">

    <script src="../../../includes/js/jquery.js" type="text/javascript"></script>

    <script type="text/javascript">
    <!--
        // Handles hiding/showing piComboBoxes
        var warnings = 0;
        $(document).ready(function() {

            $('.jAdd').bind('click', function() {

                if (warnings == 0) {
                    alert('PLEASE NOTE: When multiple approvers are selected, your order will not be placed until all approvals are collected. If your primary approver is out of office, adding another approver will not speed up the approval process.');
                    warnings++;
                }

                var found = false;
                var count = 0;

                $('.hide').each(function() {
                    if ($(this).css('display') == 'none' && !found) {
                        $(this).show();
                        found = true;
                        if (count == $('.hide').size() - 1)
                            $('.jAdd').hide()
                    }
                    count++;
                });

                return false;
            });

            $('.jRemove').bind('click', function() {

                var i = parseInt($(this).attr('rel'));
                var combo2 = $find("<%= piRadComboBox2.ClientID %>");
                var combo3 = $find("<%= piRadComboBox3.ClientID %>");
                var combo4 = $find("<%= piRadComboBox4.ClientID %>");
                var combo5 = $find("<%= piRadComboBox5.ClientID %>");

                if (i == 2)
                    ResetComboBox(combo2);
                else if (i == 3)
                    ResetComboBox(combo3);
                else if (i == 4)
                    ResetComboBox(combo4);
                else if (i == 5)
                    ResetComboBox(combo5);

                $(this).parents('tr:first').hide()
                $('.jAdd').show();
                return false;
            });

            if (window.location.href.indexOf('=pi') > -1) {
                $('.jAdd').hide();
                $('.jRemove').hide();
            }

            $('input[id*=accountTextBox]').keyup(function() {
                $('span[id*=acctNameLabel]').text($(this).val());
            });
        });

        // Resets rad combo box item to "Select One..."
        function ResetComboBox(comboBoxObj) {
            var item = comboBoxObj.findItemByText('Select One...');
            if (item)
                item.select();
        }

        function OnFocus(sender, eventArgs) {
            sender.set_caretPosition(1);
        }

        // Handles address combo box changes
        function AddressChanging(sender, eventArgs) {
            if (eventArgs.get_item().get_text() == "New Location..." || eventArgs.get_item().get_text() == "New Vendor...")
                eventArgs.set_cancel(false);
            else {
                sender.set_value(eventArgs.get_item().get_value());
                sender.set_text(eventArgs.get_item().get_text());
                eventArgs.set_cancel(true);
            }
        }

    //-->
    </script>

</asp:Content>
<asp:Content ID="Content1" runat="Server" ContentPlaceHolderID="LeftSidebarContentPlaceHolder">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="MainContentPlaceHolder">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <asp:MultiView ID="approvalMultiView" runat="server">
            <asp:View ID="View_Approve" runat="server">
                <div class="roundedBoxRed">
                    <div class="roundedBoxTopRed">
                        <span></span>
                    </div>
                    <div class="roundedBoxContentRed" style="padding-top: 3px;">
                        <!-- CONTENT BEGIN -->
                        <h1>
                            <asp:Label ID="attnLabel" runat="server" /></h1>
                        <table cellspacing="0" class="standard_tbl" style="width: 100%; font-size: 11px;">
                            <tr class="alt">
                                <td colspan="3" style="padding: 10px;">Please review the order, make changes as necessary, and choose from the options below. By clicking "Approve Order", you agree that these items will be used for official UC Davis business only.</td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                                    <asp:ImageButton ID="receivedImageButton" ToolTip="Approve Order" ImageUrl="~/images/common/approve.gif" OnClick="approveButton_Click" runat="server" />
                                </td>
                                <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                                    <asp:ImageButton ID="piApproveButton" CausesValidation="false" ToolTip="Approve with comments" ImageUrl="~/images/common/approve_comment.gif" runat="server" />
                                </td>
                                <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                                    <asp:ImageButton ID="piRejectButton" CausesValidation="false" ToolTip="Not approved" ImageUrl="~/images/common/not_approved.gif" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I approve this order and authorize the charge of
                                    <asp:Label ID="chargeLabel" Font-Bold="true" runat="server" />
                                    on the
                                    <asp:Label ID="acctNameLabel" Font-Bold="true" ForeColor="Green" runat="server" Text="agreed upon" />
                                    account.</td>
                                <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I approve this order and would like to attach a comment.</td>
                                <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I do not approve this order.</td>
                            </tr>
                        </table>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" Display="None" ControlToValidate="accountTextBox" runat="server" ErrorMessage="No account was specified for this order. Please enter an account name in the 'Order Preferences' section."></asp:RequiredFieldValidator>
                        <!-- CONTENT END -->
                    </div>
                    <div class="roundedBoxBottomRed">
                        <span></span>
                    </div>
                </div>
                <br />
                <!-- APPROVE MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="approveModalPanel" BackgroundCssClass="modalBackground" CancelControlID="approveCloseButton" TargetControlID="piApproveButton" ID="approveModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="approveModalPanel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th>Approval Message</th>
                        </tr>
                        <tr class="alt smaller">
                            <td style="padding: 8px !important;">By clicking "Submit", you agree that these items will be used for official UC Davis business only. Please enter a short message such as "I approve." into the textbox below. </td>
                        </tr>
                        <tr>
                            <td style="padding: 8px !important;">
                                <asp:TextBox ID="approveMessageTextBox" CssClass="tmp" ValidationGroup="approve" Width="358px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td style="padding: 8px !important;">
                                <asp:Button ID="approveButton" ValidationGroup="approve" OnClick="approveButton_Click" runat="server" Text=" Submit " OnClientClick="if(Page_ClientValidate('') && Page_ClientValidate('approve')) { return true; } else return false;" />
                                <asp:Button ID="approveCloseButton" CausesValidation="false" runat="server" Text=" Close " />
                            </td>
                        </tr>
                    </table>
                    <asp:RequiredFieldValidator ID="approveRequiredFieldValidator" ValidationGroup="approve" Display="None" ControlToValidate="approveMessageTextBox" runat="server" ErrorMessage="To ensure that your approval is legally binding, you must enter a message."></asp:RequiredFieldValidator>
                </asp:Panel>
                <!-- END: APPROVE MODAL -->
                <!-- REJECT MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="rejectModalPanel" CancelControlID="rejectCloseButton" BackgroundCssClass="modalBackground" TargetControlID="piRejectButton" ID="rejectModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="rejectModalPanel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th>Not Approved Message</th>
                        </tr>
                        <tr class="alt smaller">
                            <td style="padding: 8px !important;">Please enter a short message describing why you do not approve this order.</td>
                        </tr>
                        <tr>
                            <td style="padding: 8px !important;">
                                <asp:TextBox ID="rejectMessageTextBox" CssClass="tmp" Width="358px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td style="padding: 8px !important;">
                                <asp:Button ID="rejectButton" ValidationGroup="reject" OnClick="rejectButton_Click" runat="server" Text=" Submit " />
                                <asp:Button ID="rejectCloseButton" CausesValidation="false" runat="server" Text=" Close " />
                            </td>
                        </tr>
                    </table>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ValidationGroup="reject" Display="None" ControlToValidate="rejectMessageTextBox" runat="server" ErrorMessage="Please enter a short message describing why you are rejecting this order."></asp:RequiredFieldValidator>
                    <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="reject" ShowSummary="false" ShowMessageBox="true" runat="server" />
                </asp:Panel>
                <!-- END: REJECT MODAL -->
            </asp:View>
        </asp:MultiView>
        <h1 style="padding: 0px; margin: 0px;">
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <asp:MultiView ID="subTitleMultiView" runat="server">
            <asp:View ID="View_LastOrder" runat="server">
                <asp:ImageButton ID="lastOrderImageButton" CausesValidation="false" OnClick="lastOrder_Click" ToolTip="Populate with last order" ImageUrl="~/images/10/edit.gif" ImageAlign="AbsMiddle" runat="server" />&nbsp;<asp:LinkButton ID="lastOrderLinkButton" OnClick="lastOrder_Click" ForeColor="#5e9734" CausesValidation="false" runat="server" ToolTip="Populate with last order" Text="Populate with last order" />
            </asp:View>
            <asp:View ID="View_Prompt" runat="server">
                <asp:Label ID="promptLabel" runat="server" />
            </asp:View>
            <asp:View ID="View_Blank" runat="server">
            </asp:View>
        </asp:MultiView>
        <asp:MultiView ID="baMultiView" runat="server">
            <asp:View ID="View_Order" runat="server">
                <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" />
                <uc3:addedit_vendor ID="addedit_vendor1" runat="server" />
                <br />
                <br />
                <table cellspacing="0" class="formtable bdrlft">
                    <tr>
                        <th colspan="4">Agreement Details</th>
                    </tr>
                    <tr class="alt smaller">
                        <td colspan="4">Please enter the details for the business agreement below </td>
                    </tr>
                    <tr>
                        <td>Agreement Type: </td>
                        <td colspan="3">
                            <telerik:RadComboBox ID="baTypeRadComboBox" runat="server" AutoPostBack="true" CausesValidation="false" CollapseAnimation-Type="None" Width="353px" OnSelectedIndexChanged="baTypeRadComboBox_SelectedIndexChanged">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                    <telerik:RadComboBoxItem Text="One-time Service" Value="One-time Service" />
                                    <telerik:RadComboBoxItem Text="Ongoing Service" Value="Ongoing Service" />
                                    <telerik:RadComboBoxItem Text="No-cost Agreement" Value="No-cost Agreement" />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:CompareValidator ID="baTypeCompareValidator" runat="server" ControlToValidate="baTypeRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" Text="*" ErrorMessage="Please select an agreement type"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td style="width: 120px;">Agreement Contact: </td>
                        <td style="width: 150px;">
                            <asp:TextBox ID="baAgreementContactTextBox" runat="server" MaxLength="64"></asp:TextBox>
                        </td>
                        <td style="width: 60px;">Phone: </td>
                        <td>
                            <telerik:RadMaskedTextBox ID="baAgreementContactRadMaskedTextBox" runat="server" ClientEvents-OnFocus="OnFocus" Skin="" InvalidStyle-BackColor="Red" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px">
                            </telerik:RadMaskedTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>UCD Contact: </td>
                        <td>
                            <asp:TextBox ID="baContactTextBox" runat="server" BackColor="LemonChiffon" MaxLength="64"></asp:TextBox>
                        </td>
                        <td>Phone: </td>
                        <td>
                            <telerik:RadMaskedTextBox ID="baContactPhoneRadMaskedTextBox" runat="server" BackColor="LemonChiffon" ClientEvents-OnFocus="OnFocus" Skin="" InvalidStyle-BackColor="Red" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px">
                            </telerik:RadMaskedTextBox>
                            <asp:CompareValidator ID="baContactPhoneCompareValidator" runat="server" ControlToValidate="baContactPhoneRadMaskedTextBox" ErrorMessage="Please enter a phone number in the 'Deliver To' section" Operator="NotEqual" Text="*" ValueToCompare="(___) ___ - ____"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Effective From: </td>
                        <td>
                            <asp:TextBox ID="baFromTextBox" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="fromCalendarExtender" runat="server" TargetControlID="baFromTextBox">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                        <td>To: </td>
                        <td>
                            <asp:TextBox ID="baToTextBox" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="toCalendarExtender" runat="server" TargetControlID="baToTextBox">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:MultiView ID="agreementMultiView" runat="server">
                    <asp:View ID="View_Blanket" runat="server">
                        <asp:MultiView ID="shiptoMultiView" runat="server">
                            <asp:View ID="View_ShipTo_List" runat="server">
                                <table cellpadding="0" cellspacing="0" class="formtable bdrlft">
                                    <caption class="purchase">
                                        <asp:LinkButton ID="shiptoNewLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="shiptoNewLinkButton_Click">New Location</asp:LinkButton></caption>
                                    <tr>
                                        <th colspan="2">Deliver To</th>
                                    </tr>
                                    <tr class="alt smaller">
                                        <td colspan="2">Select a delivery location below or choose "New Location..." to enter a new address </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px;">Location Contact: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoNameTextBox" BackColor="LemonChiffon" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" Text="*" ControlToValidate="shiptoNameTextBox" runat="server" ErrorMessage="Please enter the name of the person who will be receiving the items"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>Location: </td>
                                        <td>
                                            <telerik:RadComboBox ID="shiptoRadComboBox" Width="500px" CausesValidation="false" OnClientSelectedIndexChanging="AddressChanging" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="address" DataValueField="id" runat="server" OnSelectedIndexChanged="shiptoRadComboBox_SelectedIndexChanged">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                                </Items>
                                            </telerik:RadComboBox>
                                            <asp:CompareValidator ID="CompareValidator6" ControlToValidate="shiptoRadComboBox" Text="*" ValueToCompare="Select One..." Operator="NotEqual" runat="server" ErrorMessage="Please select a delivery address in the 'Deliver To' section"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View_ShipTo_New" runat="server">
                                <table cellpadding="0" cellspacing="0" class="formtable bdrlft purchase">
                                    <caption class="purchase">
                                        <asp:LinkButton ID="shiptoBackLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="shiptoBackLinkButton_Click">&lArr; Back to List</asp:LinkButton></caption>
                                    <tr>
                                        <th colspan="2">New Delivery Location</th>
                                    </tr>
                                    <tr class="alt smaller">
                                        <td colspan="2">The location where the items should be delivered. </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px;">Name: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoNameTextBox2" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoName2RequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoNameTextBox2" ErrorMessage="Please enter a name in the 'Deliver To' section"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>Location Name/Dept: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoAddressTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoAddressRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoAddressTextBox" ErrorMessage="Please enter a display address in the 'Deliver To' section"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Campus/Center: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoCampusTextBox" Width="300px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>City: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoCityTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoCityRequiredFieldValidator" ControlToValidate="shiptoCityTextBox" Text="*" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>State/Province: </td>
                                        <td>
                                            <telerik:RadComboBox ID="shiptoStateRadComboBox" runat="server">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="Alabama" Value="AL" />
                                                    <telerik:RadComboBoxItem Text="Alaska" Value="AK" />
                                                    <telerik:RadComboBoxItem Text="Arizona" Value="AZ" />
                                                    <telerik:RadComboBoxItem Text="Arkansas" Value="AR" />
                                                    <telerik:RadComboBoxItem Text="California" Value="CA" />
                                                    <telerik:RadComboBoxItem Text="Canada" Value="CANADA" />
                                                    <telerik:RadComboBoxItem Text="Colorado" Value="CO" />
                                                    <telerik:RadComboBoxItem Text="Connecticut" Value="CT" />
                                                    <telerik:RadComboBoxItem Text="Delaware" Value="DE" />
                                                    <telerik:RadComboBoxItem Text="Florida" Value="FL" />
                                                    <telerik:RadComboBoxItem Text="Georgia" Value="GA" />
                                                    <telerik:RadComboBoxItem Text="Hawaii" Value="HI" />
                                                    <telerik:RadComboBoxItem Text="Idaho" Value="ID" />
                                                    <telerik:RadComboBoxItem Text="Illinois" Value="IL" />
                                                    <telerik:RadComboBoxItem Text="Indiana" Value="IN" />
                                                    <telerik:RadComboBoxItem Text="Iowa" Value="IA" />
                                                    <telerik:RadComboBoxItem Text="Kansas" Value="KS" />
                                                    <telerik:RadComboBoxItem Text="Kentucky" Value="KY" />
                                                    <telerik:RadComboBoxItem Text="Louisiana" Value="LA" />
                                                    <telerik:RadComboBoxItem Text="Maine" Value="ME" />
                                                    <telerik:RadComboBoxItem Text="Maryland" Value="MD" />
                                                    <telerik:RadComboBoxItem Text="Massachusetts" Value="MA" />
                                                    <telerik:RadComboBoxItem Text="Michigan" Value="MI" />
                                                    <telerik:RadComboBoxItem Text="Minnesota" Value="MN" />
                                                    <telerik:RadComboBoxItem Text="Mississippi" Value="MS" />
                                                    <telerik:RadComboBoxItem Text="Missouri" Value="MO" />
                                                    <telerik:RadComboBoxItem Text="Montana" Value="MT" />
                                                    <telerik:RadComboBoxItem Text="Nebraska" Value="NE" />
                                                    <telerik:RadComboBoxItem Text="Nevada" Value="NV" />
                                                    <telerik:RadComboBoxItem Text="New Hampshire" Value="NH" />
                                                    <telerik:RadComboBoxItem Text="New Jersey" Value="NJ" />
                                                    <telerik:RadComboBoxItem Text="New Mexico" Value="NM" />
                                                    <telerik:RadComboBoxItem Text="New York" Value="NY" />
                                                    <telerik:RadComboBoxItem Text="North Carolina" Value="NC" />
                                                    <telerik:RadComboBoxItem Text="North Dakota" Value="ND" />
                                                    <telerik:RadComboBoxItem Text="Ohio" Value="OH" />
                                                    <telerik:RadComboBoxItem Text="Oklahoma" Value="OK" />
                                                    <telerik:RadComboBoxItem Text="Oregon" Value="OR" />
                                                    <telerik:RadComboBoxItem Text="Pennsylvania" Value="PA" />
                                                    <telerik:RadComboBoxItem Text="Rhode Island" Value="RI" />
                                                    <telerik:RadComboBoxItem Text="South Carolina" Value="SC" />
                                                    <telerik:RadComboBoxItem Text="South Dakota" Value="SD" />
                                                    <telerik:RadComboBoxItem Text="Tennessee" Value="TN" />
                                                    <telerik:RadComboBoxItem Text="Texas" Value="TX" />
                                                    <telerik:RadComboBoxItem Text="Utah" Value="UT" />
                                                    <telerik:RadComboBoxItem Text="Vermont" Value="VT" />
                                                    <telerik:RadComboBoxItem Text="Virginia" Value="VA" />
                                                    <telerik:RadComboBoxItem Text="Washington" Value="WA" />
                                                    <telerik:RadComboBoxItem Text="West Virginia" Value="WV" />
                                                    <telerik:RadComboBoxItem Text="Wisconsin" Value="WI" />
                                                    <telerik:RadComboBoxItem Text="Wyoming" Value="WY" />
                                                </Items>
                                            </telerik:RadComboBox>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>Street: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoStreetTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoStreetRequiredFieldValidator" ControlToValidate="shiptoStreetTextBox" runat="server" Text="*" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Building: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoBuildingTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoBuildingRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoBuildingTextBox" ErrorMessage="Please enter a building name in the 'Deliver To' section"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>Room: </td>
                                        <td>
                                            <asp:TextBox ID="shiptoRoomTextBox" BackColor="LemonChiffon" Width="100px" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="shiptoRoomRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoRoomTextBox" ErrorMessage="Please enter a room number in the 'Deliver To' section"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Zip: </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="shiptoZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" Skin="" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                            </telerik:RadNumericTextBox>
                                            -
                                            <telerik:RadNumericTextBox ID="shiptoZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" Skin="" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                            </telerik:RadNumericTextBox>
                                            <asp:RequiredFieldValidator ID="shiptoZipRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoZipRadNumericTextBox" ErrorMessage="Please enter a zip code in the 'Deliver To' section"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="shiptoZipPlussRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoZipPlusRadNumericTextBox" ErrorMessage="Please enter the last 4 digits of the zip code in the 'Deliver To' section. It is used for calculating tax."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td>Phone: </td>
                                        <td>
                                            <telerik:RadMaskedTextBox ID="shiptoPhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" Skin="">
                                            </telerik:RadMaskedTextBox>
                                            <asp:CompareValidator ID="shiptoPhoneCompareValidator" runat="server" Text="*" ControlToValidate="shiptoPhoneRadMaskedTextBox" ValueToCompare="(___) ___ - ____" Operator="NotEqual" ErrorMessage="Please enter a phone number in the 'Deliver To' section"></asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                        <br />
                        <uc1:ba_items ID="ba_items1" runat="server" />
                        <br />

                        <script type="text/javascript">
                        <!--
                            function OnSubmit(source, args) {

                                //-------- Shipping warning --------//
                                var combo = $find("<%= shippingRadComboBox.ClientID %>");
                                if (combo.get_text() != "Standard")
                                    alert("WARNING: 2nd Day and Next Business Day shipping can potentially double or triple shipping fees.")

                                //-------- Validate items table ---------//
                                var alertString = "";
                                for (var i = 0; i <= 14; i = i + 1) {
                                    try {
                                        var quantity = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_qtyRadNumericTextBox" + i + "_text").value;
                                        var unit = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_unitTextBox" + i).value;
                                        var service = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_serviceTextBox" + i).value;
                                        var desc = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_descTextBox" + i).value;
                                        var unitPrice = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_costRadNumericTextBox" + i + "_text").value;

                                        if (quantity || service || desc || unitPrice) {
                                            if (quantity == null || quantity == "" || unit == null || unit == "" || service == null || service == "" || desc == null || desc == "" || unitPrice == null || unitPrice == "")
                                                alertString = alertString + "-There is missing information on row #" + (i + 1) + " of the items table.\r\n";
                                        }
                                    }
                                    catch (err) {
                                        args.IsValid = false;
                                    }
                                }

                                if (alertString != "") {
                                    alert(alertString);
                                    args.IsValid = false;
                                }
                                else {
                                    args.IsValid = true;
                                }
                            }
                        //-->
                        </script>

                    </asp:View>
                    <asp:View ID="View_NoCost" runat="server">

                        <script type="text/javascript">
                        <!--
                            function OnSubmit(source, args) {

                                //-------- Shipping warning --------//
                                var combo = $find("<%= shippingRadComboBox.ClientID %>");
                                if (combo.get_text() != "Standard")
                                    alert("WARNING: 2nd Day and Next Business Day shipping can potentially double or triple shipping fees.")
                            }
                        //-->
                        </script>

                    </asp:View>
                </asp:MultiView>
                <br />
                <table class="formtable bdrlft" cellpadding="0" cellspacing="0">
                    <tr>
                        <th colspan="2">Order Preferences</th>
                    </tr>
                    <tr class="alt smaller">
                        <td colspan="2">Help us proccess your order by configuring the following preferences </td>
                    </tr>
                    <tr runat="server" id="Tr1">
                        <td style="width: 120px;">Your PI/Approver
                            <asp:CompareValidator ID="piCompareValidator" runat="server" Text="*" ControlToValidate="piRadComboBox1" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select your P.I. in the 'Order Preferences' section"></asp:CompareValidator>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="piRadComboBox1" Height="300px" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;<a href="#" title="This order requires multiple approvers, add another." class="bdrLink jAdd">+ Add another PI/Approver</a> </td>
                    </tr>
                    <tr class="hide" runat="server" id="Tr2" style="display: none;">
                        <td>&nbsp;</td>
                        <td>
                            <telerik:RadComboBox ID="piRadComboBox2" Height="300px" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;<a href="#" class="jRemove bdrLink">Remove</a> </td>
                    </tr>
                    <tr class="hide" runat="server" id="Tr3" style="display: none;">
                        <td>&nbsp;</td>
                        <td>
                            <telerik:RadComboBox ID="piRadComboBox3" Height="300px" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;<a href="#" class="jRemove bdrLink">Remove</a> </td>
                    </tr>
                    <tr class="hide" runat="server" id="Tr4" style="display: none;">
                        <td>&nbsp;</td>
                        <td>
                            <telerik:RadComboBox ID="piRadComboBox4" Height="300px" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;<a href="#" class="jRemove bdrLink">Remove</a> </td>
                    </tr>
                    <tr class="hide" runat="server" id="Tr5" style="display: none;">
                        <td>&nbsp;</td>
                        <td>
                            <telerik:RadComboBox ID="piRadComboBox5" Height="300px" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;<a href="#" class="jRemove bdrLink">Remove</a> </td>
                    </tr>
                    <tr class="alt">
                        <td style="width: 120px;">Your Purchaser </td>
                        <td>
                            <telerik:RadComboBox ID="purchaserRadComboBox" Width="202px" DataTextField="ListName" DataValueField="UserName" AppendDataBoundItems="true" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:CompareValidator ID="purchaserCompareValidator" runat="server" Text="*" ControlToValidate="purchaserRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select your purchaser in the 'Order Preferences' section"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Shipping: </td>
                        <td>
                            <telerik:RadComboBox Width="202px" ID="shippingRadComboBox" EnableTextSelection="false" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Standard" Value="Standard" />
                                    <telerik:RadComboBoxItem Text="2nd Day" Value="2nd Day" />
                                    <telerik:RadComboBoxItem Text="Next Business Day" Value="Next Business Day" />
                                    <telerik:RadComboBoxItem Text="No Shipping Required (Pickup)" Value="No Shipping Required (Pickup)" />
                                </Items>
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td id="accountCell1" runat="server">Account: </td>
                        <td id="accountCell2" runat="server">
                            <asp:TextBox ID="accountTextBox" BackColor="LemonChiffon" MaxLength="64" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="accountRequiredFieldValidator" ControlToValidate="accountTextBox" Text="*" runat="server" ErrorMessage="No account was specified. If you're not sure what account to use, please enter a brief message into the 'Account' field explaining who will be receiving this order and how they intend to use it."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Date Needed: </td>
                        <td>
                            <asp:TextBox ID="neededTextBox" Width="200px" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="neededCalendarExtender" TargetControlID="neededTextBox" PopupPosition="Right" runat="server">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>OK to Backorder </td>
                        <td>
                            <asp:RadioButton ID="backorderOKRadioButton" Text=" Yes" GroupName="backorder" runat="server" />&nbsp;<asp:RadioButton ID="backorderNoRadioButton" Text=" No" GroupName="backorder" Checked="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top;">Special Instructions: </td>
                        <td>
                            <div style="width: 500px; float: left;">
                                <asp:TextBox ID="notesTextBox" Style="overflow: auto;" Width="500px" Rows="6" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 210px; float: left; padding-left: 10px; font-size: 9px;">
                                <strong>Examples:</strong><br />
                                - Please double check the account name<br />
                                - This order will be used for _____<br />
                            </div>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td style="vertical-align: top">Attachments: </td>
                        <td>
                            <telerik:RadUpload ID="baRadUpload" runat="server" ControlObjectsVisibility="AddButton, RemoveButtons" InitialFileInputsCount="0" InputSize="15" Width="400px">
                                <Localization Add="Add" Select="Browse" />
                            </telerik:RadUpload>
                        </td>
                    </tr>
                </table>
                <asp:MultiView ID="submitMultiView" runat="server">
                    <asp:View ID="View_Submit" runat="server">
                        <div class="tableButtons ctrls">
                            <asp:LinkButton ID="submitLinkButton" OnClick="submitLinkButton_Click" runat="server"><img src="../../../images/15/check.gif" alt="" />Submit Request</asp:LinkButton>
                        </div>
                        <br class="clear" />
                    </asp:View>
                    <asp:View ID="View_Update" runat="server">
                        <div class="tableButtons">
                            <div class="ctrls">
                                <asp:LinkButton ID="radWinUpdateButton" CausesValidation="false" runat="server"><img src="../../../images/15/check.gif" alt="" /> Update Request</asp:LinkButton>
                            </div>
                            <br class="clear" />
                            <!-- UPDATE MODAL -->
                            <ajaxToolkit:ModalPopupExtender PopupControlID="updatePanel" CancelControlID="updateCancelButton" BackgroundCssClass="modalBackground" TargetControlID="radWinUpdateButton" ID="updateModalPopupExtender" runat="server">
                            </ajaxToolkit:ModalPopupExtender>
                            <asp:Panel ID="updatePanel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
                                <table class="formtable bdrlft" cellspacing="0">
                                    <tr>
                                        <th>Update Order</th>
                                    </tr>
                                    <tr class="alt smaller">
                                        <td style="padding: 8px 10px !important;">Please enter a short message describing what changes you've made to the order and why. Please note that updated orders will need to be re-approved by the selected PI(s).</td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px !important;">
                                            <asp:TextBox ID="updateNotesTextBox" CssClass="tmp" Width="355px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td style="padding: 8px 10px !important;">
                                            <asp:Button ID="updateButton" runat="server" Text=" Update Request " OnClick="updateButton_Click" />
                                            <asp:Button ID="updateCancelButton" CausesValidation="false" runat="server" Text=" Cancel " />
                                        </td>
                                    </tr>
                                </table>
                                <asp:RequiredFieldValidator ID="updateRequiredFieldValidator" Display="None" ControlToValidate="updateNotesTextBox" runat="server" ErrorMessage="Please enter a short message describing the changes you've made to this order."></asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary3" ShowSummary="false" ShowMessageBox="true" runat="server" />
                            </asp:Panel>
                            <!-- END: UPDATE MODAL -->
                        </div>
                    </asp:View>
                    <asp:View ID="View_Submit_Blank" runat="server">
                    </asp:View>
                </asp:MultiView>
                <asp:CustomValidator ID="CustomValidator1" Display="None" runat="server" ClientValidationFunction="OnSubmit" ErrorMessage=""></asp:CustomValidator>
                <asp:ValidationSummary ID="ValidationSummary2" ShowMessageBox="true" ShowSummary="false" runat="server" />
            </asp:View>
            <asp:View ID="View_Requisition" runat="server">
                <p style="padding-top: 10px;">This request exceeds $5,000 and must be processed as a Purchase Requisition (PR) by the UC Davis Purchasing Department. PRs take an <strong>average of 30 days</strong> to process and require special handling by your purchasing agent,
                    <asp:Label ID="purchaserLabel" runat="server" />. </p>
                <p>A purchase requisition may require a <a class="bdrLink" href="../admin/uploads/PUR_SoleSourceJust.doc">sole source justification form</a> and up to three vendor quotes. Originals of these supporting documents should be faxed to your purchasing agent or attached using the form below.</p>
                <table class="formtable bdr" cellspacing="0">
                    <tr class="alt">
                        <td>If you understand and agree to the terms above, click the "Submit Requisition" button below. </td>
                    </tr>
                    <tr class="alt">
                        <td>
                            <asp:Button ID="submitReqButton" runat="server" Text=" Submit Requisition " OnClick="submitReqButton_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <table class="formtable bdr" cellspacing="0">
                    <tr class="alt">
                        <td colspan="2">If you would like your purchasing agent to submit the supporting documents to the UC Davis Purchasing Department for you, attach the documents <strong>in PDF or MS Word format</strong> below and click the "Submit Requisition with Supporting Documents" button. </td>
                    </tr>
                    <tr class="alt">
                        <td>Purchase Specifications (required) </td>
                        <td>
                            <telerik:RadUpload ID="specsRadUpload" InitialFileInputsCount="1" ControlObjectsVisibility="None" runat="server">
                            </telerik:RadUpload>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Sole Source Justification Form </td>
                        <td>
                            <telerik:RadUpload ID="soleSourceRadUpload" InitialFileInputsCount="1" ControlObjectsVisibility="None" runat="server">
                            </telerik:RadUpload>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Vendor Quotes </td>
                        <td>
                            <telerik:RadUpload ID="quotesRadUpload" InitialFileInputsCount="3" ControlObjectsVisibility="None" runat="server">
                            </telerik:RadUpload>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td colspan="2">
                            <asp:Label ID="validationLabel" runat="server"></asp:Label>
                            <asp:Button ID="submitReqWithDocsButton" runat="server" Text=" Submit Requisition with Supporting Documents " OnClick="submitReqWithDocsButton_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
                <telerik:RadProgressArea ID="RadProgressArea1" runat="server">
                </telerik:RadProgressArea>
                <br />
                <asp:Button ID="reqBackButton" runat="server" Text=" << Back to order " OnClick="reqBackButton_Click" />
            </asp:View>
            <asp:View ID="View_Confirm" runat="server">
                <br />
                <asp:Label ID="confirmLabel" runat="server" />
            </asp:View>
        </asp:MultiView>
        <iframe id="defib" src="../blocks/defibrillator.aspx" frameborder="no" width="0" height="0" runat="server"></iframe>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
