///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Private Property
    var routingAdjusted = false;

    //Public Method
    purchasing.initEdit = function () {
        loadLineItems();
    };

    //Loads up the line litem info from ajax for this order
    function loadLineItems() {
        //Place a 'loading line items' ui block
        $.getJSON(purchasing._getOption("LoadLineItemsUrl"), null, function (result) {
            console.log(result);
            //var lineItemsContainer = $("#line-items-body");

            //Go through each line item and bind it to the ui
            for (var i = 0; i < result.lineItems.length; i++) {
                var prefix = "items[" + i + "].";

                /*
                //Now make sure there is an available line item on screen, if now trigger the creation of a new one
                if (document.getElementsByName(prefix + 'id').length == 0) {
                    $("#add-line-item").click();
                }*/

                for (var prop in result.lineItems[i]) {
                    var inputName = prefix + purchasing.lowerCaseFirstLetter(prop);

                    $(document.getElementsByName(inputName)).val(result.lineItems[i][prop]);
                }
            }
        });
    }

    purchasing.lowerCaseFirstLetter = function (w) {
        return w.charAt(0).toLowerCase() + w.slice(1);
    };

} (window.purchasing = window.purchasing || {}, jQuery));
