///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = { a: true, b: false };

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

        $(".toggle-lineitem-details").live('click', function (e) {

            $(this).parents("tr").next().toggle();

            e.preventDefault();

        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));

//Adding a Public Property
purchasing.quantity = "12";

//Adding New Functionality to Purchasing
(function( purchasing, $, undefined ) {
    //Private Property
    var prop = "testing";
    
    //Public Method
    purchasing.toString = function() {
        console.log(purchasing.ingredient);
    };    
}( window.purchasing = window.purchasing || {}, jQuery ));