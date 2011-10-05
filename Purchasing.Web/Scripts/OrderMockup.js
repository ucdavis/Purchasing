///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = { invalidNumberClass: "invalid-number-warning", a: true, b: false };

    //Public Property
    purchasing.splitType = "None"; //Keep track of current split [None,Order,Line]

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
        console.log(options);
    };

    //
    purchasing.init = function () {
        $(".button").button();

        createLineItems();
        attachVendorEvents();
        attachAddressEvents();
        attachLineItemEvents();
        attachSplitOrderEvents();
        attachSplitLineEvents();
        attachFileUploadEvents();
        attachToolTips();
    };

    //Private method
    function attachToolTips() {
        //For all inputs with titles, show the tip
        $('body').delegate('input[title]', 'mouseenter focus', function () {
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
    }

    function attachFileUploadEvents() {
        new qq.FileUploader({
            // pass the dom node (ex. $(selector)[0] for jQuery users)
            element: document.getElementById('file-uploader'),
            // path to server-side upload script
            action: options.UploadUrl,
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            debug: true
        });
    }

    function attachVendorEvents() {
        $("#vendor-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                "Create Vendor": function () { createVendor(this); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#add-vendor").click(function (e) {
            e.preventDefault();

            $("#vendor-dialog").dialog("open");
        });

        function createVendor(dialog) {
            var form = $("#vendor-form");

            if (form.validate().form() == false) {
                return; //don't create the vendor if the form is invalid
            }

            var vendorInfo = {
                name: form.find("#vendor-name").val(),
                address: form.find("#vendor-address").val(),
                city: form.find("#vendor-city").val(),
                state: form.find("#vendor-state").val(),
                zip: form.find(("#vendor-zip")).val(),
                countryCode: form.find("#vendor-country-code").val()
            };

            $.post(options.AddVendorUrl, vendorInfo, function (data) {
                var vendors = $("#vendors");
                //removing existing selected options
                vendors.find("option:selected").removeAttr("selected");

                //Get back the id & add into the vendor select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(vendorInfo.name);
                vendors.append(newAddressOption);
            });

            $(dialog).dialog("close");
        }
    }

    function attachAddressEvents() {
        $("#address-dialog").dialog({
            autoOpen: false,
            height: 500,
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
                var addresses = $("#addresses");
                //removing existing selected options
                addresses.find("option:selected").removeAttr("selected");

                //Get back the id & add into the select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(addressInfo.name);
                addresses.append(newAddressOption);
            });

            $(dialog).dialog("close");
        }
    }

    function createLineItems() {
        for (var i = 0; i < 3; i++) { //Dynamically create 3 line items
            $("#line-item-template").tmpl({}).prependTo("#line-items > tbody").find(".button").button();
        }
    }

    function attachLineItemEvents() {
        $("#add-line-item").click(function (e) {
            e.preventDefault();

            var newLineItem = $("#line-item-template").tmpl({}).prependTo("#line-items > tbody");
            newLineItem.find(".button").button();

            if (purchasing.splitType === "Line") {
                var lineItemSplitTemplate = $("#line-item-split-template");
                var newLineItemSplitTable = newLineItem.find(".sub-line-item-split-body");

                lineItemSplitTemplate.tmpl().appendTo(newLineItemSplitTable);
                lineItemSplitTemplate.tmpl().appendTo(newLineItemSplitTable);

                $(".line-item-splits").show();
            }
            newLineItem.effect('highlight', 5000);
        });

        $(".toggle-line-item-details").live('click', function (e) {

            $(this).parents("tr").next().toggle();

            e.preventDefault();
        });

        $(".quantity, .price, #shipping, #tax", "#line-items").live("focus blur change keyup", function () {
            //First make sure the number is valid
            var el = $(this);

            purchasing.validateNumber(el);

            calculateSubTotal();
            calculateGrandTotal();
        });

        function calculateSubTotal() {
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

            $("#subtotal").html("$" + subTotal.toFixed(2));
        }

        function calculateGrandTotal() {
            var subTotal = parseFloat(purchasing.cleanNumber($("#subtotal").html()));
            var shipping = parseFloat(purchasing.cleanNumber($("#shipping").val()));
            var tax = parseFloat(purchasing.cleanNumber($("#tax").val()));

            var grandTotal = (subTotal * (1 + tax / 100.00)) + shipping;

            if (!isNaN(grandTotal)) {
                displayGrandTotal(grandTotal);
            }
        }
    }

    function attachSplitOrderEvents() {
        $("#add-order-split").click(function (e) {
            e.preventDefault();

            $("#order-split-template").tmpl().prependTo("#order-splits").effect('highlight', 5000);
        });

        $("#cancel-order-split").click(function (e) {
            e.preventDefault();

            if (confirm("Are you sure you want to cancel the current order split? [Description]")) {
                setSplitType("None");
            }
        });

        $("#split-order").click(function (e) {
            e.preventDefault();

            if (confirm("Are you sure you want to split this order across multiple accounts? [Description]")) {
                var splitTemplate = $("#order-split-template");
                splitTemplate.tmpl({}).appendTo("#order-splits");
                splitTemplate.tmpl({}).appendTo("#order-splits");
                splitTemplate.tmpl({}).appendTo("#order-splits");

                $("#order-split-total").html($("#grandtotal").html());

                $("#order-split-section").show();

                setSplitType("Order");
            }
        });

        $(".order-split-account-amount, .order-split-account-percent").live("focus blur change keyup", function (e) {
            e.preventDefault();
            var el = $(this);

            purchasing.validateNumber(el);

            if (el.hasClass(options.invalidNumberClass) == false) { //don't bother doing work on invalid numbers
                var total = purchasing.cleanNumber($("#order-split-total").html());
                var amount = 0, percent = 0;

                if (el.hasClass("order-split-account-amount")) { //update the percent
                    amount = purchasing.cleanNumber(el.val());

                    percent = (amount / total) * 100.0;

                    el.siblings(".order-split-account-percent").val(percent.toFixed(2));
                } else { //update the amount
                    percent = purchasing.cleanNumber(el.val());

                    amount = total * (percent / 100.0);

                    el.siblings(".order-split-account-amount").val(amount.toFixed(2));
                }

                calculateOrderAccountSplits();
            }
        });

        function calculateOrderAccountSplits() {
            var total = 0;

            $(".order-split-account-amount").each(function () {
                var amt = purchasing.cleanNumber(this.value);

                var lineTotal = parseFloat(amt);

                if (!isNaN(lineTotal)) {
                    total += lineTotal;
                }
            });

            var fixedTotal = total.toFixed(2);

            var accountTotal = $("#order-split-account-total");
            accountTotal.html("$" + fixedTotal);

            verifyAccountTotalEqualsGrandTotal(accountTotal);
        }
    }

    function attachSplitLineEvents() {
        $("#split-by-line").click(function (e) {
            e.preventDefault();

            if (confirm("Are you sure you want to split each line item across multiple accounts? [Description]")) {
                var lineItemSplitTemplate = $("#line-item-split-template");
                lineItemSplitTemplate.tmpl().appendTo(".sub-line-item-split-body");
                lineItemSplitTemplate.tmpl().appendTo(".sub-line-item-split-body");

                $(".line-item-splits").show();

                calculateSplitTotals();

                setSplitType("Line");
            }
        });

        $("#cancel-split-by-line").click(function (e) {
            e.preventDefault();

            if (confirm("Are you sure you want to cancel the current line item split? [Description]")) {
                setSplitType("None");
            }
        });

        $(".line-item-split-account-amount, .line-item-split-account-percent").live("focus blur change keyup", function (e) {
            e.preventDefault();
            var el = $(this);

            purchasing.validateNumber(el);

            if (el.hasClass(options.invalidNumberClass) == false) { //don't bother doing work on invalid numbers
                //find the total for this line
                var containingLineItemSplitTable = el.parentsUntil(".line-item-splits", ".sub-line-item-split");
                var total = purchasing.cleanNumber(containingLineItemSplitTable.find(".add-line-item-total").html());

                var amount = 0, percent = 0;

                if (el.hasClass("line-item-split-account-amount")) { //update the percent
                    amount = purchasing.cleanNumber(el.val());

                    percent = (amount / total) * 100.0;

                    el.parent().parent().find(".line-item-split-account-percent").val(percent.toFixed(2));
                } else { //update the amount
                    percent = purchasing.cleanNumber(el.val());

                    amount = total * (percent / 100.0);

                    el.parent().parent().find(".line-item-split-account-amount").val(amount.toFixed(2));
                }

                calculateLineItemAccountSplits();
            }
        });

        $(".add-line-item-split").live("click", function (e) {
            e.preventDefault();

            var containingFooter = $(this).parentsUntil("table.sub-line-item-split", "tfoot");
            var splitBody = containingFooter.prev();

            $("#line-item-split-template").tmpl().appendTo(splitBody).effect('highlight', 2000);
        });

        function calculateLineItemAccountSplits() {
            $(".sub-line-item-split").each(function () {
                var currentLineItemSplitRow = $(this);
                var total = 0;

                currentLineItemSplitRow.find(".line-item-split-account-amount").each(function () {
                    var amt = purchasing.cleanNumber(this.value);

                    var lineTotal = parseFloat(amt);

                    if (!isNaN(lineTotal)) {
                        total += lineTotal;
                    }
                });
                console.log(total);

                var fixedTotal = total.toFixed(2);

                var accountTotal = currentLineItemSplitRow.find(".add-line-item-split-total");
                accountTotal.html("$" + fixedTotal);

                verifyAccountTotalEqualsLineItemTotal(currentLineItemSplitRow);
            });
        }

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

    function displayLineItemTotal(itemRow, total) {
        itemRow.next().next().find(".add-line-item-total").html("$" + total.toFixed(2));
    }

    function displayGrandTotal(total) {
        var formattedTotal = "$" + total.toFixed(2);
        $("#grandtotal").html(formattedTotal);

        if (purchasing.splitType === "Order") {
            $("#order-split-total").html(formattedTotal);
            verifyAccountTotalEqualsGrandTotal($("#order-split-account-total"));
        }
    }

    function verifyAccountTotalEqualsGrandTotal(accountTotal) {
        var grandTotal = $("#grandtotal");
        var difference = $("#order-split-account-difference");

        if (accountTotal.html() != grandTotal.html()) {
            accountTotal.addClass(options.invalidNumberClass);
        }
        else {
            accountTotal.removeClass(options.invalidNumberClass);
        }

        var totalDifference = purchasing.cleanNumber(grandTotal.html()) - purchasing.cleanNumber(accountTotal.html());

        if (totalDifference == 0) {
            difference.html("");
        } else {
            difference.html("($" + totalDifference.toFixed(2) + ")");
        }
    }

    function verifyAccountTotalEqualsLineItemTotal(lineItemSplitRow) {
        var splitTotal = lineItemSplitRow.find(".add-line-item-split-total");
        var lineItemTotal = lineItemSplitRow.find(".add-line-item-total");
        var lineItemDifference = lineItemSplitRow.find(".add-line-item-split-difference");

        if (splitTotal.html() != lineItemTotal.html()) {
            splitTotal.addClass(options.invalidNumberClass);
        }
        else {
            splitTotal.removeClass(options.invalidNumberClass);
        }

        var totalDifference = purchasing.cleanNumber(lineItemTotal.html()) - purchasing.cleanNumber(splitTotal.html());

        if (totalDifference == 0) {
            lineItemDifference.html("");
        }
        else {
            lineItemDifference.html("($" + totalDifference.toFixed(2) + ")");
        }
    }

    function setSplitType(split) {
        purchasing.splitType = split;

        if (split === "Order") {
            $("#order-account-section").hide();
            $(".line-item-splits").hide();
            $(".sub-line-item-split-body").empty(); //clear all line splits
            $("#order-split-section")[0].scrollIntoView(true);

            $("#split-by-line").hide();
            $("#cancel-split-by-line").hide();
        }
        else if (split === "Line") {
            $("#order-account-section").hide();
            $("#order-split-section").hide();
            $("#order-splits").empty();
            $("#line-items-section")[0].scrollIntoView(true);

            $("#split-by-line").hide();
            $("#cancel-split-by-line").show();
        }
        else { //For moving back to "no split" (not implemented)
            $(".line-item-splits").hide(); //if any line splits are showing, hide them
            $("#order-split-section").hide();
            $("#order-account-section").show().get(0).scrollIntoView(true);
            $(".sub-line-item-split-body").empty(); //clear all line splits
            $("#order-splits").empty();

            $("#split-by-line").show();
            $("#cancel-split-by-line").hide();
        }
    }

    purchasing.validateNumber = function (el) {
        //takes a jquery element & validates that it is a number
        var value = purchasing.cleanNumber(el.val());

        if (isNaN(value) && value != '') {
            el.addClass(options.invalidNumberClass); //TODO: return true/false and use that value instead of querying for class
        }
        else {
            el.removeClass(options.invalidNumberClass);
        }
    };

} (window.purchasing = window.purchasing || {}, jQuery));

//Adding a Public Property
purchasing.quantity = "12";

//Adding New Functionality to Purchasing
(function( purchasing, $, undefined ) {
    //Private Property
    var prop = "testing";
    
    //Public Method
    purchasing.cleanNumber = function(n) {
           // Assumes string input, removes all commas, dollar signs, percents and spaces      
            var newValue = n.replace(",","");
            newValue = newValue.replace("$","");
            newValue = newValue.replace("%","");
            newValue = newValue.replace(/ /g,'');
            return newValue;
    };
} (window.purchasing = window.purchasing || {}, jQuery));
