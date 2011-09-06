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
        
        attachVendorEvents();
        attachAddressEvents();
        attachLineItemEvents();
        attachSplitOrderEvents();
        attachSplitLineEvents();
    };

    //Private method
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

    function attachLineItemEvents() {
        $("#add-line-item").click(function (e) {
            e.preventDefault();

            var newLineItem = $("#line-item-template").tmpl({}).prependTo("#line-items > tbody");

            if (purchasing.splitType === "Line") {
                var lineItemSplitTemplate = $("#line-item-split-template");
                var newLineItemSplitTable = newLineItem.find(".sub-line-item-split-body");

                lineItemSplitTemplate.tmpl().appendTo(newLineItemSplitTable);
                lineItemSplitTemplate.tmpl().appendTo(newLineItemSplitTable);

                $(".line-item-splits").show();
            }
        });

        $(".toggle-line-item-details").live('click', function (e) {

            $(this).parents("tr").next().toggle();

            e.preventDefault();
        });

        $(".quantity, .price, #shipping, #tax", "#line-items").live("focus blur change keyup", function () {
            //First make sure the number is valid
            var el = $(this);
            var value = purchasing.cleanNumber(el.val());

            if (isNaN(value) && value != '') {
                el.addClass(options.invalidNumberClass);
            }
            else {
                el.removeClass(options.invalidNumberClass);
            }

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
                $("#grandtotal").html("$" + grandTotal.toFixed(2));
            }
        }
    }

    function attachSplitOrderEvents() {
        $("#add-order-split").click(function (e) {
            e.preventDefault();

            $("#order-split-template").tmpl().prependTo("#order-splits");
        });

        $("#split-order").click(function (e) {
            e.preventDefault();

            if (confirm("Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat")) {
                var splitTemplate = $("#order-split-template");
                splitTemplate.tmpl({}).appendTo("#order-splits");
                splitTemplate.tmpl({}).appendTo("#order-splits");
                splitTemplate.tmpl({}).appendTo("#order-splits");

                $("#order-split-section").show();

                setSplitType("Order");
            }
        });
    }

    function attachSplitLineEvents() {
        $("#split-by-line").click(function (e) {
            e.preventDefault();

            if (confirm("Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat")) {
                var lineItemSplitTemplate = $("#line-item-split-template");
                lineItemSplitTemplate.tmpl().appendTo(".sub-line-item-split-body");
                lineItemSplitTemplate.tmpl().appendTo(".sub-line-item-split-body");

                $(".line-item-splits").show();

                setSplitType("Line");
            }
        });

        $(".add-line-item-split").live("click", function (e) {
            e.preventDefault();

            var containingFooter = $(this).parentsUntil("table.sub-line-item-split", "tfoot");
            var splitBody = containingFooter.prev();

            $("#line-item-split-template").tmpl().appendTo(splitBody);
        });
    }

    function setSplitType(split) {
        purchasing.splitType = split;

        if (split === "Order") {
            $("#order-account-section").addClass("ui-state-disabled"); //TODO: this is just fake disabled
            $(".line-item-splits").hide();
            $(".sub-line-item-split-body").empty(); //clear all line splits
        }
        else if (split === "Line") {
            $("#order-account-section").addClass("ui-state-disabled"); //TODO: this is just fake disabled
            $("#order-split-section").hide();
            $("#order-splits").empty();
        }
        else { //For moving back to "no split" (not implemented)
            $(".line-item-splits").hide(); //if any line splits are showing, hide them
            $("#order-split-section").hide();
            $("#order-account-section").removeClass("ui-state-disabled"); //TODO: this is just fake disabled
            $(".sub-line-item-split-body").empty(); //clear all line splits
            $("#order-splits").empty();
        }
    }

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
