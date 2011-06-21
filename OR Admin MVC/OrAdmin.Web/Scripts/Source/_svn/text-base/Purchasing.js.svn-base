var pur = pur || {}, preSelectedVendorId, preSelectedAddressId;

pur.Purchasing = function (o) {

    this._options = {
        moreItemInfoAction: '',
        vendorRequiredWarning: 'You must select a vendor.',
        deliveryLocationRequiredWarning: 'You must select a "Delivery Location" address.',
        multipleApproverWarning: 'PLEASE NOTE: When multiple approvers are selected, your request will not be placed until all approvals are collected. If your primary approver is out of office, adding another approver will not speed up the approval process.',
        onBeforeUnloadWarning: 'If you continue without saving, you may lose your work.',
        noItemsWarning: 'You must enter at least one item.',
        taxRate: 8.25,
        unloadOkay: false
    };

    pur.extend(this._options, o);
    this._init();
};

pur.Purchasing.prototype = {

    _init: function () {

        var self = this, uploadIndex = 0;

        /* Various onLoad tasks
        ----------------------------------------------------------------------- */
        // Prevents unsaved changes
        window.onbeforeunload = function () {
            if (!self._options.unloadOkay)
                return self._options.onBeforeUnloadWarning;
        };

        // Check whether an item has extra info attached, display icon if necessary
        this._updateItemMoreInfoLink();

        /* On form submit
        ----------------------------------------------------------------------- */
        // Handle form submissions
        $('#submit-request input, #save-request input').click(function (e) {
            self._onSubmit(e);
        });

        /* Attach approver events
        ----------------------------------------------------------------------- */
        // Adds approver row
        $('#add-approver').click(function (e) {
            self._onAddApprover(this, e);
        });

        /* Attach vendor events
        ----------------------------------------------------------------------- */
        // Handles opening "Add vendor" window
        $('#add-vendor').click(function (e) {
            self._onAddVendor(e);
        });

        /* Attach address events
        ----------------------------------------------------------------------- */
        // Handles opening "Add address" window
        $('#add-address').click(function (e) {
            self._onAddAddress(e);
        });

        /* Attach item events
        ----------------------------------------------------------------------- */
        // Set initial item subtotal
        $(window).load(function () {
            self._outputTotal();
        });

        // Handles adding new item rows
        $('#add-item').click(function (e) {
            self._onAddItem(this, e);
        });

        // Handles opening item "more info" window
        $('.more-item-info').click(function (e) {
            self._onMoreItemOpening(this, e);
        });

        // Handles price changes
        $('input[name$=PricePerUnit-text]').live('blur', function () {
            self._outputTotal();
        });

        // Handles incremental price changes
        $('div[id$=PricePerUnit] a').live('click', function () {
            self._outputTotal();
        });

        // Handles quantity changes
        $('input[name$=Quantity-text]').live('blur', function () {
            self._outputTotal();
        });

        // Handles incremental quantity changes
        $('div[id$=Quantity] a').live('click', function () {
            self._outputTotal();
        });
    },

    _onSubmit: function (e) {

        var self = this, iframe, src;

        // Check to make sure a vendor was selected
        var vendor = $('#Request_VendorId').data('tComboBox').value();
        if (vendor.length == 0) {
            alert(self._options.vendorRequiredWarning);
            e.preventDefault();
            return;
        }

        // Additionally, check to see if new vendor was entered (not an integer). If so, launch the "new vendor" window
        else if (/^ *[0-9]+ *$/.test(vendor) == false) {
            iframe = $('#add-vendor-frame')[0];
            src = iframe.src.toString();

            if (src.indexOf('?') != -1)
                src = src.substr(0, src.indexOf('?'));

            iframe.src = src + "?name=" + escape(vendor);
            $('#AddVendor').data('tWindow').center().open();
            e.preventDefault();
            return;
        }

        // Check to make sure a shipto location was selected
        var shipTo = $('#Request_ShipToId').data('tComboBox').value();
        if (shipTo.length == 0) {
            alert(self._options.deliveryLocationRequiredWarning);
            e.preventDefault();
            return;
        }

        // Additionally, check to see if new address was entered (not an integer). If so, launch the "new address" window
        else if (/^ *[0-9]+ *$/.test(shipTo) == false) {
            iframe = $('#add-address-frame')[0];
            src = iframe.src.toString();

            if (src.indexOf('?') != -1)
                src = src.substr(0, src.indexOf('?'));

            iframe.src = src + "?name=" + escape(shipTo);
            $('#AddAddress').data('tWindow').center().open();
            e.preventDefault();
            return;
        }

        var allItemRowsEmpty = true;
        $('.item-row').each(function () {
            if (!self._isItemRowEmpty($(this)))
                allItemRowsEmpty = false;
        });
        if (allItemRowsEmpty) {
            alert(self._options.noItemsWarning);
            e.preventDefault();
            return;
        }

        // Finally, remove empty item rows starting with the last row and working your way back
        $($('.item-row').get().reverse()).each(function (i) {
            if (self._isItemRowEmpty($(this)))
                $(this).remove();
            else
                return false;
        });

        self._options.unloadOkay = true;
        e.stopPropagation();
    },

    _onAddApprover: function (self, e) {

        if (confirm(this._options.multipleApproverWarning)) {
            $('.approver-hidden:first').fadeIn().removeClass('approver-hidden');
            if ($('.approver-hidden').size() == 0)
                $(self).hide();
        }
        e.preventDefault();
        e.stopPropagation();

    },

    _onAddVendor: function (e) {

        var iframe = $('#add-vendor-frame')[0],
            src = iframe.src.toString();

        if (src.indexOf('?') != -1)
            src = src.substr(0, src.indexOf('?'));
        iframe.src = src;

        $('#AddVendor').data('tWindow').center().open();
        e.preventDefault();
        e.stopPropagation();

    },

    _reloadVendors: function (selectedValue) {
        // Reloads vendors and closes all popups
        preSelectedVendorId = selectedValue;
        var combobox = $('#Request_VendorId').data('tComboBox');
        combobox.reload();

        // Close popups
        $('.t-window').each(function () {
            $('#' + $(this).attr('id')).data('tWindow').close();
        });
    },

    _onVendorsDataBound: function (e) {
        // Triggered on vendor data binding
        var combobox = $(this).data('tComboBox');
        var selectItem = function (dataItem) {
            if (!preSelectedVendorId)
                return false;
            return dataItem.Value == preSelectedVendorId;
        }
        combobox.select(selectItem);
        preSelectedVendorId = null;
    },

    _onAddAddress: function (e) {
        var iframe = $('#add-address-frame')[0],
            src = iframe.src.toString();

        if (src.indexOf('?') != -1)
            src = src.substr(0, src.indexOf('?'));
        iframe.src = src;

        $('#AddAddress').data('tWindow').center().open();
        e.preventDefault();
        e.stopPropagation();
    },

    _reloadAddresses: function (selectedValue) {
        // Reloads addresses and closes all popups
        preSelectedAddressId = selectedValue;
        var combobox = $('#Request_ShipToId').data('tComboBox');
        combobox.reload();

        // Close popups
        $('.t-window').each(function () {
            $('#' + $(this).attr('id')).data('tWindow').close();
        });
    },

    _onAddressesDataBound: function (e) {
        // Triggered on address list data binding
        var combobox = $(this).data('tComboBox');
        var selectItem = function (dataItem) {
            if (!preSelectedAddressId)
                return false;
            return dataItem.Value == preSelectedAddressId;
        }
        combobox.select(selectItem);
        preSelectedAddressId = null;
    },

    _onAddItem: function (self, e) {
        $('.item-hidden:first').fadeIn().removeClass('item-hidden');
        if ($('.item-hidden').size() == 0)
            $(self).hide();
        e.preventDefault();
        e.stopPropagation();
    },

    _onMoreItemOpening: function (self, e) {

        var itemIndex = $('.more-item-info').index($(self)),
            url = $(self).parent().find('input[name$=Url]').val(),
            notes = $(self).parent().find('input[name$=Notes]').val(),
            promoCode = $(self).parent().find('input[name$=PromoCode]').val(),
            commodityTypeId = $(self).parent().find('input[name$=CommodityTypeId]').val();

        $("#MoreItemInfoWindow .t-window-content").html('<iframe src="' + this._options.moreItemInfoAction + '?Index=' + itemIndex + '&Url=' + escape(url) + '&Notes=' + escape(notes) + '&CommodityTypeId=' + commodityTypeId + '&PromoCode=' + escape(promoCode) + '" style="width: 100%; height: 100%;" frameborder="0"></iframe>');
        $('#MoreItemInfoWindow').data('tWindow').center().open();

    },

    _isItemRowEmpty: function (itemRow) {

        var qty = $(itemRow).find('div[id$=Quantity]').data('tTextBox').value(),
                unit = $(itemRow).find('input[name$=Unit]').val(),
                catalog = $(itemRow).find('input[name$=CatalogNumber]').val(),
                desc = $(itemRow).find('input[name$=Description]').val(),
                price = $(itemRow).find('div[id$=PricePerUnit]').data('tTextBox').value();

        if (qty == null && price == null && unit === 'each' && (catalog + desc).length == 0)
            return true;
        else
            return false;
    },

    _updateItemMoreInfoLink: function () {

        $('.item-row').each(function () {

            // If any fields are populated, show the corresponding icon
            var itemIndex = $('.item-row').index($(this));
            var url = $(this).find('input[name$=Url]').val();
            var notes = $(this).find('input[name$=Notes]').val();
            var promoCode = $(this).find('input[name$=PromoCode]').val();

            if ((url + notes + promoCode).length > 0)
                $(this).find('.more-item-info').attr('src', '/Content/Img/Fugue/document--pencil.png').attr('title', 'Edit additional information for this item');
            else
                $(this).find('.more-item-info').attr('src', '/Content/Img/Fugue/document--plus.png').attr('title', 'Add additional information to this item'); ;

            // If any validation errors are present, highlight the icon
            if ($(this).find('.more-item-info').parent('td').find('.input-validation-error').size() > 0)
                $(this).find('.more-item-info').addClass('invalid-icon');
            else
                $(this).find('.more-item-info').removeClass('invalid-icon');
        });

    },

    _onItemChange: function (e) {
        // Handles updating item subtotal on quantity or price change
        // Set to new value
        $(this).data("tTextBox").value(e.newValue);
        pur.Purchasing.prototype._outputTotal();
    },

    _outputTotal: function () {
        // Outputs the item subtotal
        var subTotal = 0;
        $('.item-row').each(function () {
            var qty = $(this).find('div[id$=Quantity]').data('tTextBox').value();
            var price = $(this).find('div[id$=PricePerUnit]').data('tTextBox').value();
            if (qty && price)
                subTotal += qty * price;
        });
        $('#subtotal').html(this._formatNumber(subTotal, 2));
    },

    // Returns taxed and padded number
    _formatNumber: function (myNum, numOfDec) {
        var decimal = 1;
        for (i = 1; i <= numOfDec; i++)
            decimal = decimal * 10;
        return '$' + ((Math.round(myNum * decimal) / decimal) * ((this._options.taxRate / 100) + 1)).toFixed(numOfDec);
    }
};

pur.extend = function (obj1, obj2) {
    for (var prop in obj2) {
        obj1[prop] = obj2[prop];
    }
};