///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = { invalidNumberClass: "invalid-number-warning", a: true, b: false };

    //Public Property
    purchasing.ingredient = "Public Property";

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
        console.log(options);
    };

    //
    purchasing.init = function () {
        attachVendorEvents();
        attachAddressEvents();
        attachLineItemEvents();
    };

    //Private method
    function attachVendorEvents() {
        $("#vendor-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                "Create Vendor": function () { $(this).dialog("close"); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#add-vendor").click(function (e) {
            e.preventDefault();

            $("#vendor-dialog").dialog("open");
        });
    }

    function attachAddressEvents() {
        $("#address-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                "Create Shipping Address": function () { $(this).dialog("close"); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#add-address").click(function (e) {
            e.preventDefault();

            $("#address-dialog").dialog("open");
        });
    }

    function attachLineItemEvents() {
        $("#add-line-item").button();

        $("#add-line-item").click(function (e) {
            e.preventDefault();

            $("#line-item-template").tmpl({}).prependTo("#line-items > tbody");
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

            var grandTotal = (subTotal * (1+tax/100.00)) + shipping;

            if (!isNaN(grandTotal)) {
                $("#grandtotal").html("$" + grandTotal.toFixed(2));
            }
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
