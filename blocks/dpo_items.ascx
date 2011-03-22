<%@ Control Language="C#" AutoEventWireup="true" CodeFile="dpo_items.ascx.cs" Inherits="business_purchasing_blocks_dpo_items" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="dpoItemsPanel" runat="server">
    <asp:HiddenField ID="subTotalHiddenField" Value="0" runat="server" />
    <asp:HiddenField ID="taxRateHiddenField" Value="8.25" runat="server" />
    <table cellpadding="0" cellspacing="0" class="formtable bdrlft purchase">
        <caption class="purchase"><a href="javascript:AddItem();" class="bdrLink">+ Add Another</a> | <a href="javascript:RemoveItem()" class="bdrLink">Remove</a></caption>
        <tr>
            <th colspan="6">Items</th>
        </tr>
        <tr class="alt smaller">
            <td style="width: 10%">Quantity</td>
            <td style="width: 10%">Unit</td>
            <td style="width: 15%">Catalog #</td>
            <td style="width: 50%">Complete Description</td>
            <td style="width: 10%">Unit Price</td>
            <td style="width: 5%">Notes</td>
        </tr>
        <tr id="Tr0" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox0" row="0" NumberFormat-AllowRounding="true" Skin="" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox0" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox0" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox0" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox0" row="0" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField0" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes0Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr1" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox1" row="1" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox1" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox1" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox1" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox1" row="1" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField1" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes1Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr2" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox2" row="2" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox2" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox2" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox2" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox2" row="2" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField2" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes2Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr3" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox3" row="3" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox3" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox3" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox3" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox3" row="3" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField3" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes3Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr4" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox4" row="4" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox4" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox4" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox4" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox4" row="4" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField4" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes4Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr5" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox5" row="5" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox5" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox5" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox5" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox5" row="5" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField5" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes5Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr6" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox6" row="6" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox6" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox6" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox6" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox6" row="6" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField6" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes6Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr7" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox7" row="7" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox7" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox7" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox7" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox7" row="7" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField7" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes7Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr8" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox8" row="8" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox8" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox8" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox8" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox8" row="8" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField8" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes8Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr9" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox9" row="9" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox9" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox9" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox9" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox9" row="9" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField9" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes9Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr10" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox10" row="10" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox10" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox10" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox10" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox10" row="10" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField10" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes10Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr11" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox11" row="11" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox11" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox11" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox11" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox11" row="11" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField11" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes11Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr12" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox12" row="12" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox12" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox12" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox12" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox12" row="12" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField12" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes12Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr13" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox13" row="13" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox13" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox13" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox13" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox13" row="13" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField13" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes13Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr id="Tr14" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox14" row="14" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox14" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="catalogTextBox14" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox14" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox14" ClientEvents-OnLoad="InitTotals" row="14" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField14" runat="server" />
            </td>
            <td style="text-align: center; padding: 3px !important">
                <asp:Image ID="notes14Image" ToolTip="Attach notes to this item" Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
            </td>
        </tr>
        <tr class="alt smaller">
            <td colspan="4" style="text-align: right">Estimated Total (including
                <asp:Label ID="taxLabel" runat="server" Text="8.25" />% tax but not including shipping fees):</td>
            <td colspan="2" style="font-weight: bold;">
                <asp:Label ID="dpoTotalLabel" runat="server" Text="" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- MODAL POPUPS -->
<ajaxToolkit:ModalPopupExtender PopupControlID="notes0Panel" CancelControlID="notes0Button" BackgroundCssClass="modalBackground" TargetControlID="notes0Image" ID="notes0ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes1Panel" CancelControlID="notes1Button" BackgroundCssClass="modalBackground" TargetControlID="notes1Image" ID="notes1ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes2Panel" CancelControlID="notes2Button" BackgroundCssClass="modalBackground" TargetControlID="notes2Image" ID="notes2ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes3Panel" CancelControlID="notes3Button" BackgroundCssClass="modalBackground" TargetControlID="notes3Image" ID="notes3ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes4Panel" CancelControlID="notes4Button" BackgroundCssClass="modalBackground" TargetControlID="notes4Image" ID="notes4ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes5Panel" CancelControlID="notes5Button" BackgroundCssClass="modalBackground" TargetControlID="notes5Image" ID="notes5ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes6Panel" CancelControlID="notes6Button" BackgroundCssClass="modalBackground" TargetControlID="notes6Image" ID="notes6ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes7Panel" CancelControlID="notes7Button" BackgroundCssClass="modalBackground" TargetControlID="notes7Image" ID="notes7ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes8Panel" CancelControlID="notes8Button" BackgroundCssClass="modalBackground" TargetControlID="notes8Image" ID="notes8ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes9Panel" CancelControlID="notes9Button" BackgroundCssClass="modalBackground" TargetControlID="notes9Image" ID="notes9ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes10Panel" CancelControlID="notes10Button" BackgroundCssClass="modalBackground" TargetControlID="notes10Image" ID="notes10ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes11Panel" CancelControlID="notes11Button" BackgroundCssClass="modalBackground" TargetControlID="notes11Image" ID="notes11ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes12Panel" CancelControlID="notes12Button" BackgroundCssClass="modalBackground" TargetControlID="notes12Image" ID="notes12ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes13Panel" CancelControlID="notes13Button" BackgroundCssClass="modalBackground" TargetControlID="notes13Image" ID="notes13ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<ajaxToolkit:ModalPopupExtender PopupControlID="notes14Panel" CancelControlID="notes14Button" BackgroundCssClass="modalBackground" TargetControlID="notes14Image" ID="notes14ModalPopupExtender" runat="server">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel ID="notes0Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes0TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes0Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes1Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes1TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes1Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes2Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes2TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes2Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes3Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes3TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes3Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes4Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes4TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes4Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes5Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes5TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes5Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes6Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes6TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes6Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes7Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes7TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes7Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes8Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes8TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes8Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes9Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes9TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes9Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes10Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes10TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes10Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes11Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes11TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes11Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes12Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes12TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes12Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes13Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes13TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes13Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="notes14Panel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
    <table class="formtable bdrlft" cellspacing="0">
        <tr>
            <th>Item Notes</th>
        </tr>
        <tr class="alt smaller">
            <td>Enter your notes in the textbox below and click "Okay".</td>
        </tr>
        <tr>
            <td style="padding: 8px !important;">
                <asp:TextBox ID="notes14TextBox" Width="360px" Height="100px" TextMode="MultiLine" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="alt">
            <td style="padding: 8px !important;">
                <asp:Button ID="notes14Button" CausesValidation="false" OnClientClick="CheckNotes();" runat="server" Text=" Okay " />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- END: MODAL POPUPS -->

<script type="text/javascript">
    <!--

    $(document).keyup(function(event) {
        if (event.keyCode == 27) {
            $find("<%= notes0ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes1ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes2ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes3ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes4ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes5ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes6ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes7ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes8ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes9ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes10ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes11ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes12ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes13ModalPopupExtender.ClientID %>").hide();
            $find("<%= notes14ModalPopupExtender.ClientID %>").hide();
        }
    });

    $(function() {
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(CheckNotes);
    });

    function CheckNotes() {
        for (var i = 0; i <= 14; i++) {
            if (jQuery.trim($('[id*=notes' + i + 'TextBox]').val()).length > 0)
                $('img[id*=notes' + i + 'Image]').attr('src', 'http://envnet.ucdavis.edu/images/15/note_new.gif');
            else
                $('img[id*=notes' + i + 'Image]').attr('src', 'http://envnet.ucdavis.edu/images/15/note_add.gif');
        }
    };

    var rowTotals = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    var runningTotal = 0;
    var formattedNumber = 0;
    var hasInnerText = (document.getElementsByTagName("body")[0].innerText != undefined) ? true : false;

    function InitTotals() {
        for (var r = 0; r <= 14; r = r + 1)
            rowTotals[r] = parseFloat(document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_rowHiddenField" + r).value);
        runningTotal = (document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_subTotalHiddenField").value);
        OutputTotal(AddRowTotals());
    }

    // Handles quanitiy changes
    function QuanitiyChanged(sender, eventArgs) {
        var row = GetRowAtrb(sender) // Determine the row in which the quanitiy was changed
        var unitPrice = document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_costRadNumericTextBox" + row).value; // Get the unit price for this row
        if (unitPrice == "")
            unitPrice = 0;

        rowTotals[row] = Math.round(eventArgs.get_newValue()) * unitPrice; // Insert the row's total (int quanitiy * unitPrice) into the row position in the array
        OutputTotal(AddRowTotals());

        if (eventArgs.get_newValue() == "")
            eventArgs.set_cancel(true);
    }

    // Handles value (unit price) changes
    function ValueChanged(sender, eventArgs) {
        var row = GetRowAtrb(sender) // Determine the row in which the unit price was changed

        var qty = document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_qtyRadNumericTextBox" + row + "_text").value; // Get the quanitiy for the unit price
        if (qty == "")
            qty = 0;

        rowTotals[row] = eventArgs.get_newValue() * qty; // Insert the row's total (unit price * quanitiy) into the row position in the array
        OutputTotal(AddRowTotals());

        if (eventArgs.get_newValue() == "")
            eventArgs.set_cancel(true);
    }

    // Outputs total to label element
    function OutputTotal(runningTotal) {
        var lbl = document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_dpoTotalLabel");
        var chargeLbl = document.getElementById("ctl00_MainContentPlaceHolder_chargeLabel");

        formattedNumber = formatNumber(runningTotal, 2); // Add tax and decimal places

        if (formattedNumber > 4999) {
            lbl.style.color = "Red";
            if (chargeLbl)
                chargeLbl.style.color = "Red";
        }
        else if (formattedNumber == 0.00) {
            lbl.style.color = "";
            if (chargeLbl)
                chargeLbl.style.color = "";
        }
        else {
            lbl.style.color = "Green";
            if (chargeLbl)
                chargeLbl.style.color = "Green";
        }

        if (!hasInnerText) {
            // FireFox doesn't like innerText
            lbl.textContent = "$" + formattedNumber;
            if (chargeLbl)
                chargeLbl.textContent = "$" + formattedNumber;
        }
        else {
            lbl.innerText = "$" + formattedNumber;
            if (chargeLbl)
                chargeLbl.innerText = "$" + formattedNumber;
        }
    }

    // Adds up the row totals
    function AddRowTotals() {
        var total = 0;
        for (var i = 0; i <= 14; i = i + 1)
            total = total + rowTotals[i]; // Add up all the row totals
        return total;
    }

    // Returns which row the sender is from
    function GetRowAtrb(sender) {
        var target = document.getElementById(sender.get_element().id + '_text');
        if (target.getAttribute('row'))
            return target.getAttribute('row');
        if (target.row)
            return target.row;
    }

    // Returns taxed and padded number
    function formatNumber(myNum, numOfDec) {
        var decimal = 1;
        for (i = 1; i <= numOfDec; i++)
            decimal = decimal * 10;

        var taxRate = (document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_taxRateHiddenField").value);

        var myFormattedNum = ((Math.round(myNum * decimal) / decimal) * ((taxRate / 100) + 1)).toFixed(numOfDec);
        return myFormattedNum;
    }

    // Resets caret to start on focus (phone and fax masked texboxes)
    function OnFocus(sender, eventArgs) {
        sender.set_caretPosition(1);
    }

    // Adds DPO row
    function AddItem() {
        for (var r = 1; r <= 14; ++r) {
            var row = document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_Tr" + r);
            if (row.style.display == "none") {
                row.style.display = "";
                return;
            }
        }
        // none available?
    }

    // Removes DPO row
    function RemoveItem() {
        for (var r = 14; r > 0; --r) {
            var row = document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_Tr" + r);
            if (row.style.display == "") {
                row.style.display = "none";
                ClearTB(r);
                rowTotals[r] = 0;
                OutputTotal(AddRowTotals());
                return;
            }
        }
    }

    function ClearTB(which) {
        switch (which) {
            case 0:
                var cost = $find("<%= costRadNumericTextBox0.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox0.ClientID %>"); qty.clear();
                break;
            case 1:
                var cost = $find("<%= costRadNumericTextBox1.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox1.ClientID %>"); qty.clear();
                break;
            case 2:
                var cost = $find("<%= costRadNumericTextBox2.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox2.ClientID %>"); qty.clear();
                break;
            case 3:
                var cost = $find("<%= costRadNumericTextBox3.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox3.ClientID %>"); qty.clear();
                break;
            case 4:
                var cost = $find("<%= costRadNumericTextBox4.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox4.ClientID %>"); qty.clear();
                break;
            case 5:
                var cost = $find("<%= costRadNumericTextBox5.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox5.ClientID %>"); qty.clear();
                break;
            case 6:
                var cost = $find("<%= costRadNumericTextBox6.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox6.ClientID %>"); qty.clear();
                break;
            case 7:
                var cost = $find("<%= costRadNumericTextBox7.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox7.ClientID %>"); qty.clear();
                break;
            case 8:
                var cost = $find("<%= costRadNumericTextBox8.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox8.ClientID %>"); qty.clear();
                break;
            case 9:
                var cost = $find("<%= costRadNumericTextBox9.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox9.ClientID %>"); qty.clear();
                break;
            case 10:
                var cost = $find("<%= costRadNumericTextBox10.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox10.ClientID %>"); qty.clear();
                break;
            case 11:
                var cost = $find("<%= costRadNumericTextBox11.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox11.ClientID %>"); qty.clear();
                break;
            case 12:
                var cost = $find("<%= costRadNumericTextBox12.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox12.ClientID %>"); qty.clear();
                break;
            case 13:
                var cost = $find("<%= costRadNumericTextBox13.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox13.ClientID %>"); qty.clear();
                break;
            case 14:
                var cost = $find("<%= costRadNumericTextBox14.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox14.ClientID %>"); qty.clear();
                break;
            default:
                break;
        }

        document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_unitTextBox" + which).value = 'each';
        document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_catalogTextBox" + which).value = '';
        document.getElementById("ctl00_MainContentPlaceHolder_dpo_items1_descTextBox" + which).value = '';
    }
    //-->
</script>

