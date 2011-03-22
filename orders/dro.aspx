<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" ValidateRequest="false" AutoEventWireup="true" CodeFile="dro.aspx.cs" Inherits="business_purchasing_orders_dro" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../blocks/addedit_vendor.ascx" TagName="addedit_vendor" TagPrefix="uc1" %>
<%@ Register Src="../blocks/dro_items.ascx" TagName="dro_items" TagPrefix="uc2" %>
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
            if (item) {
                item.select();
            }
        }

        function OnSubmit(source, args) {

            //-------- Validate items table ---------//
            var alertString = "";
            for (var i = 0; i <= 10; i = i + 1) {
                var itemDesc = document.getElementById("ctl00_MainContentPlaceHolder_dro_items1_itemDescTextBox" + i).value;
                var repairDesc = document.getElementById("ctl00_MainContentPlaceHolder_dro_items1_repairDescTextBox" + i).value;
                var value = document.getElementById("ctl00_MainContentPlaceHolder_dro_items1_valueRadNumericTextBox" + i + "_text").value;
                var unitPrice = document.getElementById("ctl00_MainContentPlaceHolder_dro_items1_costRadNumericTextBox" + i + "_text").value;

                if (itemDesc || repairDesc || value || unitPrice) {
                    if (itemDesc == "" || repairDesc == "" || value == "" || unitPrice == "")
                        alertString = alertString + "-There is missing information on row #" + (i + 1) + " of the items table.\r\n";
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

        // Handles resetting caret postion on focus of phone/fax textboxes
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
    <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" />
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
                                <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I approve this order and authorize the charges on the
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
        <h1 style="margin: 0px; padding: 0px;">
            <asp:Label ID="sectionTitleLabel" runat="server" Text="Departmental Repair Order"></asp:Label></h1>
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
        <asp:MultiView ID="droMultiView" runat="server">
            <asp:View ID="View_Order" runat="server">
                <uc1:addedit_vendor ID="addedit_vendor1" runat="server" />
                <br />
                <asp:MultiView ID="siteMultiView" runat="server">
                    <asp:View ID="View_ChooseSite" runat="server">
                        <br />
                        <table class="formtable bdrlft" cellspacing="0">
                            <tr>
                                <th>Repair Site</th>
                            </tr>
                            <tr>
                                <td class="smaller alt">Where will the repair take place? </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton ID="onSiteRadioButton" AutoPostBack="true" OnCheckedChanged="onSiteRadioButton_CheckedChanged" runat="server" GroupName="site" Text=" In department" />
                                    <asp:RadioButton ID="offSiteRadioButton" runat="server" Checked="true" GroupName="site" Text=" Off site at vendor's location" />
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="View_Site_List" runat="server">
                        <table cellpadding="0" cellspacing="0" class="formtable bdrlft purchase">
                            <caption class="purchase">
                                <asp:LinkButton ID="siteChooseLinkButton" CausesValidation="false" CssClass="bdrLink" OnClick="siteChooseLinkButton_Click" runat="server">&lArr; Back</asp:LinkButton>
                                |
                                <asp:LinkButton ID="siteNewLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="siteNewLinkButton_Click">New Location</asp:LinkButton></caption>
                            <tr>
                                <th colspan="2">Repair Site</th>
                            </tr>
                            <tr class="alt smaller">
                                <td colspan="2">Select a location below or choose "New Location..." to enter a new address </td>
                            </tr>
                            <tr>
                                <td style="width: 110px;">Tech Contact: </td>
                                <td>
                                    <asp:TextBox ID="siteNameTextBox" BackColor="LemonChiffon" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteRequiredFieldValidator" Text="*" ControlToValidate="siteNameTextBox" runat="server" ErrorMessage="Please enter the name of the primary contact for the location"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Location: </td>
                                <td>
                                    <telerik:RadComboBox ID="siteRadComboBox" Width="500px" CausesValidation="false" OnClientSelectedIndexChanging="AddressChanging" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="address" DataValueField="id" runat="server" OnSelectedIndexChanged="siteRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="siteCompareValidator" ControlToValidate="siteRadComboBox" Text="*" ValueToCompare="Select One..." Operator="NotEqual" runat="server" ErrorMessage="Please select a location in the 'Repair Site' section"></asp:CompareValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="View_Site_New" runat="server">
                        <table cellpadding="0" cellspacing="0" class="formtable bdrlft">
                            <caption class="purchase">
                                <asp:LinkButton ID="siteBackLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="siteBackLinkButton_Click">&lArr; Back to List</asp:LinkButton></caption>
                            <tr>
                                <th colspan="2">New Location</th>
                            </tr>
                            <tr class="alt smaller">
                                <td colspan="2">Please enter the location where the repairs will take place. </td>
                            </tr>
                            <tr>
                                <td style="width: 120px;">Tech Contact: </td>
                                <td>
                                    <asp:TextBox ID="siteNameTextBox2" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteName2RequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteNameTextBox2" ErrorMessage="Please enter the name of the primary contact for the location"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="alt">
                                <td>Location Name/Dept: </td>
                                <td>
                                    <asp:TextBox ID="siteAddressTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteAddressRequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteAddressTextBox" ErrorMessage="Please enter a name or department for the address in the 'New Location' section"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Campus/Center: </td>
                                <td>
                                    <asp:TextBox ID="siteCampusTextBox" Width="300px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="alt">
                                <td>City: </td>
                                <td>
                                    <asp:TextBox ID="siteCityTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteCityRequiredFieldValidator" ControlToValidate="siteCityTextBox" Text="*" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>State/Province: </td>
                                <td>
                                    <telerik:RadComboBox ID="siteStateRadComboBox" runat="server">
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
                                    <asp:TextBox ID="siteStreetTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteStreetRequiredFieldValidator" ControlToValidate="siteStreetTextBox" runat="server" Text="*" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Building: </td>
                                <td>
                                    <asp:TextBox ID="siteBuildingTextBox" BackColor="LemonChiffon" Width="300px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteBuildingRequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteBuildingTextBox" ErrorMessage="Please enter a building name in the 'New Location' section"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="alt">
                                <td>Room: </td>
                                <td>
                                    <asp:TextBox ID="siteRoomTextBox" BackColor="LemonChiffon" Width="100px" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="siteRoomRequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteRoomTextBox" ErrorMessage="Please enter a room number in the 'New Location' section"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Zip: </td>
                                <td>
                                    <telerik:RadNumericTextBox ID="siteZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" Skin="" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                    </telerik:RadNumericTextBox>
                                    -
                                    <telerik:RadNumericTextBox ID="siteZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" Skin="" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                    </telerik:RadNumericTextBox>
                                    <asp:RequiredFieldValidator ID="siteZipRequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteZipRadNumericTextBox" ErrorMessage="Please enter a zip code in the 'New Location' section"></asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="siteZipPlussRequiredFieldValidator" runat="server" Text="*" ControlToValidate="siteZipPlusRadNumericTextBox" ErrorMessage="Please enter the last 4 digits of the zip code in the 'New Location' section. It is used for calculating tax."></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr class="alt">
                                <td>Phone: </td>
                                <td>
                                    <telerik:RadMaskedTextBox ID="sitePhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" Skin="">
                                    </telerik:RadMaskedTextBox>
                                    <asp:CompareValidator ID="sitePhoneCompareValidator" runat="server" Text="*" ControlToValidate="sitePhoneRadMaskedTextBox" ValueToCompare="(___) ___ - ____" Operator="NotEqual" ErrorMessage="Please enter a phone number in the 'New Location' section"></asp:CompareValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
                <br />
                <uc2:dro_items ID="dro_items1" runat="server" />
                <br />
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
                                    <telerik:RadComboBoxItem Text="No shipping required" Value="No shipping required" />
                                    <telerik:RadComboBoxItem Text="We ship items to vendor, vendor ships them back" Value="We ship items to vendor, vendor ships them back" />
                                    <telerik:RadComboBoxItem Text="Vendor ships items from repair site, We pay for return shipping" Value="Vendor ships items from repair site, We pay for return shipping" />
                                    <telerik:RadComboBoxItem Text="Vendor ships items to and from the repair site" Value="Vendor ships items to and from the repair site" />
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
                            <telerik:RadUpload ID="droRadUpload" runat="server" ControlObjectsVisibility="AddButton, RemoveButtons" InitialFileInputsCount="0" InputSize="15" Width="400px">
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
                            <telerik:RadUpload ID="specsRadUpload" InitialFileInputsCount="1" ControlObjectsVisibility="None"  runat="server">
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
</asp:Content>
