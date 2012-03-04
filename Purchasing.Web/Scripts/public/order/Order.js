//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    "use strict";
    //Private Property
    var options = { invalidNumberClass: "invalid-number-warning", invalidLineItemClass: "line-item-warning", validLineItemClass: "line-item-ok", lineItemDataIndex: "line-item-index", lineAddedEvent: "lineadded", lineItemIndex: 0, splitIndex: 0 };

    //Public Property
    purchasing.splitType = "None"; //Keep track of current split [None,Order,Line]

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing._getOption = function (prop) {
        return options[prop];
    };

    purchasing.getMessage = function (key) {
        return options.Messages[key];
    };

    purchasing.init = function () {
        $(".button").button();

        attachFormEvents();
        attachVendorEvents();
        attachAddressEvents();

        //attachLineItemEvents();
        //attachSplitOrderEvents();
        //attachSplitLineEvents();

        attachKoEvents();

        attachCommoditySearchEvents();
        attachAccountSearchEvents();
        attachRestrictedItemsEvents();
        attachFileUploadEvents();
        attachCalculatorEvents();
        attachToolTips();
        attachShippingWarnings();
        attachTour();
        attachNav();

        //createLineItems();
    };

    function attachKoEvents() {
        ko.bindingHandlers['itemName'] = {
            'update': function (element, valueAccessor, all, model) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                element.name = "items[" + model.index() + "]." + value;
            }
        };

        ko.extenders.verifyNumber = function (target, option) {
            target.hasError = ko.observable(false);

            target.subscribe(function (val) {
                if (isNaN(val)) {
                    target.hasError(true);
                } else {
                    target.hasError(false);
                }
            });
            return target;
        };

        var lineItem = function (index) {
            var self = this;
            self.id = ko.observable(); //start at index+1?
            self.index = ko.observable(index);
            self.quantity = ko.observable().extend({ verifyNumber: [] });
            self.unit = ko.observable("EA");
            self.catalogNumber = ko.observable();
            self.desc = ko.observable();
            self.price = ko.observable().extend({ verifyNumber: [] });
            self.total = ko.computed(function () {
                var total = self.quantity() * self.price();
                return isNaN(total) ? '$0.00' : '$' + total.toFixed(3);
            });
        };

        purchasing.orderModel = new function () {
            var self = this;
            self.name = "Fake name for testing";

            self.items = ko.observableArray([new lineItem(0), new lineItem(1), new lineItem(2)]); //default to 3 line items
        } ();

        ko.applyBindings(purchasing.orderModel);
    }

    //Private method
    function attachFormEvents() {
        $("form").submit(function (e) {
            if ($(this).valid() && purchasing.orderValid()) {
                if (confirm(options.Messages.ConfirmSubmit)) {
                    return;
                }
            }

            e.preventDefault();
        });
    }

    function attachTour() {
        purchasing.initTourNotification(); //attach tour notifications

        purchasing.takeTour = function (startId) {
            window.Modernizr.load({
                load: options.Guider.Assets,
                complete: function () {
                    window.tour.startTour(startId);
                }
            });
        };

        $("#order-form").on('click', ".tour-help", function (e) {
            e.preventDefault();

            purchasing.takeTour($(this).data("tourid"));
        });
    }

    function attachCommoditySearchEvents() {
        $(".line-item-details").live(options.lineAddedEvent, function () {
            var $el = $(this).find(".search-commodity-code");

            createCommodityCodeAutoComplete($el);
        });

        function createCommodityCodeAutoComplete($el) {
            $el.autocomplete({
                source: function (request, response) {
                    var el = this.element[0]; //grab the element that caused the autocomplete
                    var searchTerm = el.value;

                    $.getJSON(options.SearchCommodityCodeUrl, { searchTerm: searchTerm }, function (results) {
                        if (!results.length) {
                            response([{ label: options.Messages.NoCommodityCodesMatch + ' "' + searchTerm + '"', value: searchTerm}]);
                        } else {
                            response($.map(results, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Id
                                };
                            }));
                        }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    event.preventDefault();

                    var el = $(this);
                    el.val(ui.item.value).attr("title", ui.item.label);
                }
            });
        }
    }

    function attachAccountSearchEvents() {
        $("#accounts-search-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $(".search-account").live("click", function (e) {
            e.preventDefault();

            //clear out inputs and empty the results table
            $("input", "#accounts-search-form").val("");
            $("#accounts-search-dialog-results > tbody").empty();

            $("#accounts-search-dialog").data("container", $(this).parents(".account-container")).dialog("open");
        });

        $("#accounts-search-dialog-searchbox-btn").click(function (e) {
            e.preventDefault();
            searchKfsAccounts();
        });

        $("#accounts-search-dialog-searchbox").keypress(function (e) {
            if (e.which === 13) { //handle the enter key
                e.preventDefault();
                searchKfsAccounts();
            }
        });

        // trigger for selecting an account
        $(".result-select-btn").live("click", function () {
            var $container = $("#accounts-search-dialog").data('container');
            var row = $(this).parents("tr");
            var account = row.find(".result-account").html();
            var title = row.find(".result-name").html();

            var select = $container.find(".account-number");

            $("#select-option-template").tmpl({ id: account, name: account, title: title }).appendTo(select);
            select.val(account).change();

            $("#accounts-search-dialog").dialog("close");

            var selectCtl = $container.find(".account-subaccount");

            loadSubAccounts(account, selectCtl);
        });

        // change of account in drop down, check to load subaccounts
        purchasing.bindAccountChange = function () {
            $(".account-number").live("change", function () {
                var $account = $(this);
                var select = $account.siblings(".account-subaccount");
                loadSubAccounts($account.val(), select);
            });
        };

        purchasing.unBindAccountChange = function () {
            $(".account-number").die("change");
        };

        purchasing.bindAccountChange();

        // load subaccounts into the subaccount select
        function loadSubAccounts(account, $selectCtrl) {
            $.getJSON(options.KfsSearchSubAccountsUrl, { accountNumber: account }, function (result) {

                $selectCtrl.find("option:not(:first)").remove();
                $selectCtrl.change();

                if (result.length > 0) {
                    var data = $.map(result, function (n, i) { return { name: n.Name, id: n.Id }; });

                    $("#select-option-template").tmpl(data).appendTo($selectCtrl);

                    $selectCtrl.removeAttr("disabled");
                }
                else {
                    $selectCtrl.attr("disabled", "disabled");
                }
            });
        }

        function searchKfsAccounts() {
            var searchTerm = $("#accounts-search-dialog-searchbox").val();
            $("#accounts-search-dialog-results tbody").empty();

            $.getJSON(options.KfsSearchUrl, { searchTerm: searchTerm }, function (result) {
                if (result.length > 0) {

                    var rowData = $.map(result, function (n, i) { return { name: n.Name, account: n.Id }; });

                    $("#accounts-search-dialog-results-template").tmpl(rowData).appendTo("#accounts-search-dialog-results tbody");

                    $(".result-select-btn").button();
                }
                else {
                    var tr = $("<tr>").append($("<tr>").attr("colspan", 3).html(options.Messages.NoResults));
                    $("#accounts-search-dialog-results tbody").append(tr);
                }
            });
        }
    }

    function attachToolTips() {
        //For all inputs with titles, show the tip
        $('#order-form').on('mouseenter focus', 'input[title], select[title], a[title]', function () {
            $(this).qtip({
                overwrite: false,
                show: {
                    event: 'mouseenter focus',
                    ready: true
                },
                hide: {
                    event: 'mouseleave blur'
                },
                position: {
                    my: 'bottom center',
                    at: 'top center'
                }
            });
        });

        $(".account-number").live('change', function () {
            var el = $(this);
            var selectedOption = $("option:selected", el);
            var title = selectedOption.attr('title');

            if (!title) {
                el.qtip('destroy');
                el.removeAttr("title");
            } else {
                el.attr('title', title);
            }
        });
    }

    function attachNav() {
        $(window).sausage({ page: '.ui-widget-header:visible' });
        $('.orders-nav').stickyfloat({ duration: 400 });
    }

    function attachFileUploadEvents() {
        var uploader = new qq.FileUploader({
            // pass the dom node (ex. $(selector)[0] for jQuery users)
            element: document.getElementById('file-uploader'),
            // path to server-side upload script
            action: options.UploadUrl,
            fileTemplate: '<li>' +
                '<span class="qq-upload-file"></span>' +
                '<span class="qq-upload-spinner"></span>' +
                '<span class="qq-upload-size"></span>' +
                '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                '<span class="qq-upload-failed-text">Failed</span>' +
                '<a href="#" class="qq-upload-file-remove">[Remove]</a>' +
                '<input type="hidden" class="qq-upload-file-id" name="fileIds" value="" />' +
            '</li>',
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            onComplete: function (id, fileName, response) {
                var newFileContainer = $(uploader._getItemByFileId(id));
                newFileContainer.find(".qq-upload-file-id").val(response.id);
            },
            debug: true
        });

        $(".qq-upload-file-remove").live("click", function (e) {
            e.preventDefault();

            var fileContainer = $(this).parent();
            fileContainer.remove();
        });
    }

    function attachVendorEvents() {
        $("#vendor-dialog").dialog({
            autoOpen: false,
            height: 610,
            width: 500,
            modal: true,
            buttons: {
                "Create Vendor": function () { createVendor(this); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#search-vendor-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                Confirm: addKfsVendor,
                Cancel: function () { $(this).dialog("close"); }
            }
        });

        $("#add-vendor").click(function (e) {
            e.preventDefault();

            $("#vendor-dialog").dialog("open");
        });

        $("#search-vendor").click(function (e) {
            e.preventDefault();

            $("#search-vendor-dialog-searchbox").val("");
            $("#search-vendor-dialog-vendor-address option:not(:first)").remove();

            $("#search-vendor-dialog").dialog("open");
        });

        $("#search-vendor-dialog-searchbox").autocomplete({
            source: function (request, response) {
                var searchTerm = $("#search-vendor-dialog-searchbox").val();

                $.getJSON(options.SearchVendorUrl, { searchTerm: searchTerm }, function (results) {
                    response($.map(results, function (item) {
                        return {
                            label: item.Name,
                            value: item.Id
                        };
                    }));
                });
            },
            minLength: 3,
            select: function (event, ui) {
                event.preventDefault();

                $("#search-vendor-dialog-selectedvendor").val(ui.item.value);
                $(this).val(ui.item.label);

                searchVendorAddress(ui.item.value);
            }
        });

        function searchVendorAddress(vendorId) {
            $.getJSON(options.SearchVendorAddressUrl, { vendorId: vendorId }, function (results) {
                $("#search-vendor-dialog-vendor-address option:not(:first)").remove();
                var select = $("#search-vendor-dialog-vendor-address");

                if (results.length > 0) {
                    var data = $.map(results, function (n, i) { return { id: n.Id, name: n.Name }; });

                    $("#select-option-template").tmpl(data).appendTo("#search-vendor-dialog-vendor-address");

                    select.removeAttr("disabled");
                }
                else {
                    select.attr("disabled", "disabled");
                }
            });
        }

        function addKfsVendor() {
            var vendorId = $("#search-vendor-dialog-selectedvendor").val();
            var typeCode = $("#search-vendor-dialog-vendor-address").val();

            //TODO: add in correct workgroup, add in error handling
            $.post(
                options.AddKfsVendorUrl,
                { id: 1, vendorId: vendorId, addressTypeCode: typeCode },
                function (result) {
                    $("#select-option-template").tmpl({ id: result.id, name: result.name }).appendTo("#vendor");
                    $("#vendor").val(result.id);

                    $("#search-vendor-dialog").dialog("close");
                }
            );
        }

        function createVendor(dialog) {
            var form = $("#vendor-form");

            if (form.validate().form() === false) {
                return; //don't create the vendor if the form is invalid
            }

            var vendorInfo = {
                name: form.find("#vendor-name").val(),
                line1: form.find("#vendor-address").val(),
                city: form.find("#vendor-city").val(),
                state: form.find("#vendor-state").val(),
                zip: form.find(("#vendor-zip")).val(),
                countryCode: form.find("#vendor-country-code").val(),
                phone: form.find("#vendor-phone").val(),
                fax: form.find("#vendor-fax").val(),
                email: form.find("#vendor-email").val()
            };

            $.post(options.AddVendorUrl, vendorInfo, function (data) {
                var vendor = $("#vendor");
                //removing existing selected options
                vendor.find("option:selected").removeAttr("selected");

                //Get back the id & add into the vendor select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(vendorInfo.name);
                vendor.append(newAddressOption);
            });

            $(dialog).dialog("close");
        }
    }

    function attachAddressEvents() {
        $("#address-dialog").dialog({
            autoOpen: false,
            height: 610,
            width: 500,
            modal: true,
            buttons: {
                "Create Shipping Address": function () { createAddress(this); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#add-address").click(function (e) {
            e.preventDefault();

            $("#address-dialog").dialog("open");
        });

        function createAddress(dialog) {
            var form = $("#address-form");

            if (form.validate().form() === false) {
                return; //don't create the address if the form is invalid
            }

            var addressInfo = {
                name: form.find("#address-name").val(),
                building: form.find("#address-building").val(),
                room: form.find("#address-room").val(),
                address: form.find("#address-address").val(),
                city: form.find("#address-city").val(),
                state: form.find("#address-state").val(),
                zip: form.find(("#address-zip")).val(),
                phone: form.find(("#address-phone")).val()
            };

            $.post(options.AddAddressUrl, addressInfo, function (data) {
                var addresses = $("#shipAddress");
                //removing existing selected options
                addresses.find("option:selected").removeAttr("selected");

                //Get back the id & add into the select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(addressInfo.name);
                addresses.append(newAddressOption);
            });

            $(dialog).dialog("close");
        }
    }

    function attachShippingWarnings() {
        $("#shippingType").on("change", function () {
            var warning = $("#shippingType > option:selected").data("warning");
            $("#shipping-warning").html(warning);
        });
    }

    function attachCalculatorEvents() {
        $("#calculator-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true
        });

        function enterLineValues(dialog, lineItem) {
            //enter the values into the associated line item
            lineItem.find(".price").val($("#calculator-price").val());
            lineItem.find(".quantity").val($("#calculator-quantity").val()).keyup();

            dialog.dialog("close");
        }

        $(".price-calculator").live("click", function (e) {
            e.preventDefault();

            //fill in the values from the line item
            var el = $(this);
            var lineItem = el.parentsUntil("#line-items-body", ".line-item-row");

            $("#calculator-quantity").val(lineItem.find(".quantity").val());
            $("#calculator-price, #calculator-total").val("");

            $("#calculator-dialog").dialog("option",
                {
                    buttons: {
                        "Accept Values": function () { enterLineValues($(this), lineItem); },
                        "Cancel": function () { $(this).dialog("close"); }
                    }
                }
            );
            $("#calculator-dialog").dialog("open");
        });

        $("#calculator-quantity, #calculator-total").bind("focus blur keyup", function () {
            var quantity = purchasing.cleanNumber($("#calculator-quantity").val());
            var total = purchasing.cleanNumber($("#calculator-total").val());

            var price = total / quantity;

            $("#calculator-price").val(purchasing.formatNumber(price));
        });
    }

    function createLineItems() {
        for (var i = 0; i < 3; i++) { //Dynamically create 3 line items
            $("#line-item-template").tmpl({ index: options.lineItemIndex++ }).appendTo("#line-items > tbody")
                .trigger(options.lineAddedEvent)
                .find(".button").button();
        }
    }

    purchasing.addLineItem = function () {
        var newLineItemId = options.lineItemIndex++;
        var newLineItem = $("#line-item-template").tmpl({ index: newLineItemId }).appendTo("#line-items > tbody").trigger(options.lineAddedEvent);
        newLineItem.find(".button").button();

        if (purchasing.splitType === "Line") {
            var lineItemSplitTemplate = $("#line-item-split-template");
            var newLineItemSplitTable = newLineItem.find(".sub-line-item-split-body");

            lineItemSplitTemplate.tmpl({ index: options.splitIndex++, lineItemId: newLineItemId }).appendTo(newLineItemSplitTable);
            lineItemSplitTemplate.tmpl({ index: options.splitIndex++, lineItemId: newLineItemId }).appendTo(newLineItemSplitTable);

            $(".line-item-splits").show();
        }

        return newLineItem;
    };

    function attachLineItemEvents() {
        $("#add-line-item").bind('click createline', function (e) {
            e.preventDefault();
            var newLineItem = purchasing.addLineItem();

            if (e.type === 'click') {
                newLineItem.find("td").effect('highlight', 3000);
            }
        });

        $(".toggle-line-item-details").live('click', function (e) {

            $(this).parents("tr").next().toggle();

            e.preventDefault();
        });

        $(".quantity, .price, #shipping, #tax, #freight", "#line-items").live("focus blur change keyup", function () {

            //First make sure the number is valid
            var el = $(this);

            purchasing.validateNumber(el);

            purchasing.calculateSubTotal();
            purchasing.calculateGrandTotal();
        });

        $(".quantity, .price, .description").live("focus blur change keyup", function () {
            purchasing.validateLineItem();
        });


    }

    function attachSplitOrderEvents() {
        $("#add-order-split").bind('click createsplit', function (e) {
            e.preventDefault();

            var newSplit = $("#order-split-template").tmpl({ index: options.splitIndex++ }).appendTo("#order-splits");

            if (e.type === 'click') {
                newSplit.effect('highlight', 5000);
            }
        });

        $("#cancel-order-split").click(function (e) {
            e.preventDefault();

            if (confirm(options.Messages.ConfirmCancelOrderSplit)) {
                purchasing.resetSplits();
                purchasing.setSplitType("None", true);
            }
        });

        $("#split-order").click(function (e, data) {
            e.preventDefault();

            var automate = data === undefined ? false : data.automate;

            if (!automate && !confirm(options.Messages.ConfirmOrderSplit)) {
                return;
            }

            var splitTemplate = $("#order-split-template");
            splitTemplate.tmpl({ index: options.splitIndex++ }).appendTo("#order-splits");
            splitTemplate.tmpl({ index: options.splitIndex++ }).appendTo("#order-splits");
            splitTemplate.tmpl({ index: options.splitIndex++ }).appendTo("#order-splits");

            $("#order-split-total").html($("#grandtotal").html());

            $("#order-split-section").show();

            purchasing.setSplitType("Order", automate);
        });

        $(".order-split-account-amount, .order-split-account-percent").live("focus blur change keyup", function (e) {
            e.preventDefault();
            var el = $(this);

            purchasing.validateNumber(el);

            if (el.hasClass(options.invalidNumberClass) === false) { //don't bother doing work on invalid numbers
                var total = purchasing.cleanNumber($("#order-split-total").html());
                var amount = 0, percent = 0;

                if (el.hasClass("order-split-account-amount")) { //update the percent
                    amount = purchasing.cleanNumber(el.val());

                    percent = (amount / total) * 100.0;

                    el.siblings(".order-split-account-percent").val(purchasing.formatNumber(percent));
                } else { //update the amount
                    percent = purchasing.cleanNumber(el.val());

                    amount = total * (percent / 100.0);

                    el.siblings(".order-split-account-amount").val(purchasing.formatNumber(amount));
                }

                purchasing.calculateOrderAccountSplits();
            }
        });
    }

    function attachSplitLineEvents() {
        $("#split-by-line").click(function (e, data) {
            e.preventDefault();

            var automate = data === undefined ? false : data.automate;

            if (!automate && !confirm(options.Messages.ConfirmLineSplit)) {
                return;
            }

            var lineItemSplitTemplate = $("#line-item-split-template");

            $(".sub-line-item-split-body").each(function () {
                var splitBody = $(this);
                var lineItemId = splitBody.data(options.lineItemDataIndex);

                lineItemSplitTemplate.tmpl({ index: options.splitIndex++, lineItemId: lineItemId }).appendTo(splitBody);
                //Only default to one line item split, since you only need >= 1 split per line
                //lineItemSplitTemplate.tmpl({ index: options.splitIndex++, lineItemId: lineItemId }).appendTo(splitBody);
            });

            $(".line-item-splits").show();

            calculateSplitTotals();

            purchasing.setSplitType("Line", automate);
        });

        $("#cancel-split-by-line").click(function (e) {
            e.preventDefault();

            if (confirm(options.Messages.ConfirmCancelLineSplit)) {
                purchasing.resetSplits();
                purchasing.setSplitType("None", true);
            }
        });

        $(".line-item-split-account-amount, .line-item-split-account-percent").live("focus blur change keyup", function (e) {
            e.preventDefault();
            var el = $(this);

            purchasing.validateNumber(el);

            if (el.hasClass(options.invalidNumberClass) === false) { //don't bother doing work on invalid numbers
                //find the total for this line
                var containingLineItemSplitTable = el.parentsUntil("#line-items-body", ".line-item-splits");
                var total = purchasing.cleanNumber(containingLineItemSplitTable.find(".add-line-item-total").html());

                var amount = 0, percent = 0;

                if (el.hasClass("line-item-split-account-amount")) { //update the percent
                    amount = purchasing.cleanNumber(el.val());

                    percent = (amount / total) * 100.0;

                    el.parent().parent().find(".line-item-split-account-percent").val(purchasing.formatNumber(percent));
                } else { //update the amount
                    percent = purchasing.cleanNumber(el.val());

                    amount = total * (percent / 100.0);

                    el.parent().parent().find(".line-item-split-account-amount").val(purchasing.formatNumber(amount));
                }

                purchasing.calculateLineItemAccountSplits();
            }
        });

        $(".quantity, .price, #tax", "#line-items").live("focus blur change keyup", function () {
            if (purchasing.splitType === "Line") { //For a line split, changes to this values must force recalculation
                purchasing.calculateLineItemAccountSplits();
            }
        });

        $(".add-line-item-split").live("click createsplit", function (e) {
            e.preventDefault();

            var splitContainer = $(this).parents(".line-item-splits", ".line-item-splits");
            var splitBody = splitContainer.find(".sub-line-item-split-body");
            var lineItemId = splitContainer.find(".line-item-ids").val();

            var newSplit = $("#line-item-split-template").tmpl({ index: options.splitIndex++, lineItemId: lineItemId }).appendTo(splitBody);

            if (e.type === 'click') {
                newSplit.effect('highlight', 2000);
            }
        });

        function calculateSplitTotals() {
            $(".line-item-row").each(function () {
                var row = $(this);
                var quantity = purchasing.cleanNumber(row.find(".quantity").val());
                var price = purchasing.cleanNumber(row.find(".price").val());

                var lineTotal = parseFloat(quantity) * parseFloat(price);

                if (!isNaN(lineTotal)) { //place on sibling line total 
                    displayLineItemTotal(row, lineTotal);
                }
            });
        }
    }

    function attachRestrictedItemsEvents() {
        $("#order-restricted-checkbox").change(function () {
            var fields = $("#order-restricted-fields");
            if (!this.checked) {
                fields.show("highlight", "slow");
            }
            else {
                fields.hide();
            }
        });
    }

    function displayLineItemTotal(itemRow, total) {

        // set the line total value
        itemRow.find(".line-total").html("$" + purchasing.formatNumber(total));

        var taxPercent = purchasing.cleanNumber($("#tax").val());
        var taxRate = taxPercent / 100.00;

        var totalWithTax = total * (1 + taxRate);
        var totalFromTax = total * taxRate;

        var splitsRow = itemRow.next().next();
        splitsRow.find(".add-line-item-total").html("$" + purchasing.formatNumber(totalWithTax));
        splitsRow.find(".add-line-item-split-tax-shipping").html("$" + purchasing.formatNumber(totalFromTax));
    }

    function displayGrandTotal(total) {
        var formattedTotal = "$" + purchasing.formatNumber(total);
        $("#grandtotal").html(formattedTotal);

        if (purchasing.splitType === "Order") {
            $("#order-split-total").html(formattedTotal);
            verifyAccountTotalEqualsGrandTotal($("#order-split-account-total"));
        }
    }

    function verifyAccountTotalEqualsGrandTotal(accountTotal) {
        var grandTotal = $("#grandtotal");
        var difference = $("#order-split-account-difference");

        if (accountTotal.html() !== grandTotal.html()) {
            accountTotal.addClass(options.invalidNumberClass);
        }
        else {
            accountTotal.removeClass(options.invalidNumberClass);
        }

        var totalDifference = purchasing.cleanNumber(grandTotal.html()) - purchasing.cleanNumber(accountTotal.html());

        if (totalDifference === 0) {
            difference.html("");
        } else {
            difference.html("($" + purchasing.formatNumber(totalDifference) + ")");
        }
    }

    function verifyAccountTotalEqualsLineItemTotal(lineItemSplitRow) {
        var splitTotal = lineItemSplitRow.find(".add-line-item-split-total");
        var lineItemTotal = lineItemSplitRow.find(".add-line-item-total");
        var lineItemDifference = lineItemSplitRow.find(".add-line-item-split-difference");

        if (splitTotal.html() !== lineItemTotal.html()) {
            splitTotal.addClass(options.invalidNumberClass);
        }
        else {
            splitTotal.removeClass(options.invalidNumberClass);
        }

        var totalDifference = purchasing.cleanNumber(lineItemTotal.html()) - purchasing.cleanNumber(splitTotal.html());

        if (totalDifference === 0) {
            lineItemDifference.html("");
        }
        else {
            lineItemDifference.html("($" + purchasing.formatNumber(totalDifference) + ")");
        }
    }

    purchasing.orderValid = function () {
        //Always make sure there are >0 line items
        var linesWithNonZeroValues = $(".line-total").filter(function () {
            var rowValue = purchasing.cleanNumber($(this).html());
            return rowValue > 0;
        });

        if (linesWithNonZeroValues.length === 0) {
            alert(options.Messages.LineItemRequired);
            return false;
        }

        //If modification is not enabled, don't check the split info
        if ($("#item-modification-button").is(":visible")) {
            return true;
        }

        //If no spit is chosen, make sure either account or AM are chosen
        if (purchasing.splitType === "None") {
            var hasAccount = $("#Account").val() !== "";
            var hasAccountManager = $("#accountmanagers").val();

            if ((hasAccount && hasAccountManager) || !(hasAccount || hasAccountManager)) { //XOR
                if (hasAccountManager === undefined) {
                    alert(options.Messages.AccountRequired);
                }
                else {
                    alert(options.Messages.AccountOrManagerRequired);
                }
                return false;
            }
        }
        else if (purchasing.splitType === "Order") {
            //If order is split, make sure all order money is accounted for
            var splitTotal = purchasing.cleanNumber($("#order-split-account-total").html());
            var orderTotal = purchasing.cleanNumber($("#order-split-total").html());

            //Make sure each split with an amount has an account chosen
            var splitsWithAmountsButNoAccounts = $(".order-split-line").filter(function () {
                var split = $(this);
                var hasAccountChosen = split.find(".account-number").val() !== "";
                var amount = split.find(".order-split-account-amount").val();

                if (amount != 0 && !hasAccountChosen) {
                    return true;
                }
                else {
                    return false;
                }
            });

            if (splitsWithAmountsButNoAccounts.length) {
                alert(options.Messages.OrderSplitWithNoAccount);
                return false;
            }

            if (splitTotal !== orderTotal) {
                alert(options.Messages.TotalAmountRequired);
                return false;
            }
        }
        else if (purchasing.splitType === "Line") {
            //if line items are split, make sure #1 all money is accounted for, #2 every line item has at least one split

            var lineSplitsWithNonMatchingAmounts = $(".line-item-splits-totals").filter(function () {
                var split = $(this);
                var lineSplitTotal = purchasing.cleanNumber(split.find(".add-line-item-split-total").html());
                var lineTotal = purchasing.cleanNumber(split.find(".add-line-item-total").html());

                return parseFloat(lineSplitTotal) !== parseFloat(lineTotal);
            });

            //Make sure each split with an amount has an account chosen
            var lineSplitsWithAmountsButNoAccounts = $(".sub-line-item-split-body > tr").filter(function () {
                var split = $(this);
                var hasAccountChosen = split.find(".account-number").val() !== "";
                var amount = split.find(".line-item-split-account-amount").val();

                if (amount != 0 && !hasAccountChosen) {
                    return true;
                }
                else {
                    return false;
                }
            });

            if (lineSplitsWithAmountsButNoAccounts.length !== 0) {
                alert(options.Messages.LineSplitNoAccount);
                return false;
            }

            if (lineSplitsWithNonMatchingAmounts.length !== 0) {
                alert(options.Messages.LineSplitTotalAmountRequired);
                return false;
            }
        }

        return true;
    };

    purchasing.resetSplits = function () {
        options.splitIndex = 0; //Reset split index
    };

    purchasing.setSplitType = function (split, automate) {
        purchasing.splitType = split;
        $("#splitType").val(split);
        var scroll = !automate; //Only scroll if the split setting should not be automated
        var scrollToLocation;

        if (split === "Order") {
            $("#order-account-section").hide();
            $(".line-item-splits").hide();
            $(".sub-line-item-split-body").empty(); //clear all line splits
            scrollToLocation = $("#order-split-section")[0];

            $("#split-by-line").hide();
            $("#cancel-split-by-line").hide();
            $("#line-item-help-display").hide();
        }
        else if (split === "Line") {
            $("#order-account-section").hide();
            $("#order-split-section").hide();
            $("#order-splits > tbody").empty();
            scrollToLocation = $("#line-items-section")[0];

            $("#split-by-line").hide();
            $("#cancel-split-by-line").show();
            $("#line-item-help-display").show();
        }
        else { //For moving back to "no split"
            $(".line-item-splits").hide(); //if any line splits are showing, hide them
            $("#order-split-section").hide();
            scrollToLocation = $("#order-account-section").show().get(0);
            $(".sub-line-item-split-body").empty(); //clear all line splits
            $("#order-splits > tbody").empty();

            $("#split-by-line").show();
            $("#cancel-split-by-line").hide();
            $("#line-item-help-display").show();
        }

        purchasing.updateNav();

        if (scroll) {
            scrollToLocation.scrollIntoView(true);
        }
    };

    purchasing.calculateLineItemAccountSplits = function () {
        $(".line-item-splits").each(function () {
            var currentLineItemSplitRow = $(this);
            var total = 0;

            currentLineItemSplitRow.find(".line-item-split-account-amount").each(function () {
                var amt = purchasing.cleanNumber(this.value);

                var lineTotal = parseFloat(amt);

                if (!isNaN(lineTotal)) {
                    total += lineTotal;
                }
            });

            var fixedTotal = purchasing.formatNumber(total);

            var accountTotal = currentLineItemSplitRow.find(".add-line-item-split-total");
            accountTotal.html("$" + fixedTotal);

            verifyAccountTotalEqualsLineItemTotal(currentLineItemSplitRow);
        });
    };

    purchasing.validateLineItem = function () {
        $(".line-item-row").each(function () {
            var row = $(this);
            var hasQuantity = purchasing.cleanNumber(row.find(".quantity").val()) !== '';
            var hasPrice = purchasing.cleanNumber(row.find(".price").val()) !== '';
            var hasDescription = row.find(".description").val().trim() !== '';

            if (hasQuantity || hasPrice || hasDescription) {
                row.find(".quantity").toggleClass(options.invalidLineItemClass, !hasQuantity).toggleClass(options.validLineItemClass, hasQuantity);
                row.find(".description").toggleClass(options.invalidLineItemClass, !hasDescription).toggleClass(options.validLineItemClass, hasDescription);
                row.find(".price").toggleClass(options.invalidLineItemClass, !hasPrice).toggleClass(options.validLineItemClass, hasPrice);
            }
            else {
                row.find(":input").removeClass(options.invalidLineItemClass).removeClass(options.validLineItemClass);
            }
        });
    };

    purchasing.calculateSubTotal = function () {
        var subTotal = 0;

        $(".line-item-row").each(function () {
            var row = $(this);
            var quantity = purchasing.cleanNumber(row.find(".quantity").val());
            var price = purchasing.cleanNumber(row.find(".price").val());

            var lineTotal = parseFloat(quantity) * parseFloat(price);

            if (!isNaN(lineTotal)) {
                subTotal += lineTotal;
                displayLineItemTotal(row, lineTotal);
            }
        });

        $("#subtotal").html("$" + purchasing.formatNumber(subTotal));
    };

    purchasing.calculateGrandTotal = function () {
        var subTotal = parseFloat(purchasing.cleanNumber($("#subtotal").html()));
        var shipping = parseFloat(purchasing.cleanNumber($("#shipping").val()));
        var freight = parseFloat(purchasing.cleanNumber($("#freight").val()));
        var tax = parseFloat(purchasing.cleanNumber($("#tax").val()));

        var grandTotal = ((subTotal + freight) * (1 + tax / 100.00)) + shipping;

        if (!isNaN(grandTotal)) {
            displayGrandTotal(grandTotal);
        }
    };

    purchasing.calculateOrderAccountSplits = function () {
        var total = 0;

        $(".order-split-account-amount").each(function () {
            var amt = purchasing.cleanNumber(this.value);

            var lineTotal = parseFloat(amt);

            if (!isNaN(lineTotal)) {
                total += lineTotal;
            }
        });

        var fixedTotal = purchasing.formatNumber(total);

        var accountTotal = $("#order-split-account-total");
        accountTotal.html("$" + fixedTotal);

        verifyAccountTotalEqualsGrandTotal(accountTotal);
    };

    purchasing.validateNumber = function (el) {
        //takes a jquery element & validates that it is a number
        var value = purchasing.cleanNumber(el.val());

        if (isNaN(value) && value !== '') {
            el.addClass(options.invalidNumberClass); //TODO: return true/false and use that value instead of querying for class
        }
        else {
            el.removeClass(options.invalidNumberClass);
        }
    };

    purchasing.formatNumber = function (n) {
        return n.toFixed(3);
    };

    purchasing.cleanNumber = function (n) {
        // Assumes string input, removes all commas, dollar signs, percents and spaces      
        var newValue = n.replace(",", "");
        newValue = newValue.replace("$", "");
        newValue = newValue.replace("%", "");
        newValue = newValue.replace(/ /g, '');
        return newValue;
    };

    purchasing.updateNav = function () {
        $(window).sausage("draw");
    };

} (window.purchasing = window.purchasing || {}, jQuery));