///<reference path="jquery-1.6.2-vsdoc.js"/>
///<reference path="OrderMockup.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Private Property
    var routingAdjusted = false;
    var startingLineItemCount = 3;

    //Public Method
    purchasing.initEdit = function () {
        loadLineItems();
        loadSplits(); //TODO: maybe join into one ajax call
    };

    //Loads up the splits from ajax and injects into the page
    function loadSplits() {
        $.getJSON(purchasing._getOption("LoadSplitsUrl"), null, function (result) {
            console.log(result);

            bindSplits(result.splitType);
        });
    }

    function bindSplits(result) {
        if (result.splitType === "Order") {
            $("#split-order").trigger('click', { prompt: false });
        }
        else if (result.splitType === "Line") {
            $("#split-by-line").trigger('click', { prompt: false });
        }
        else { //No split, bind directly to the available account
            var singleSplit = result.splits[0];

            if (singleSplit.Account !== null) {//we have account info, bind
                
            }
        }
    }

    //Loads up the line litem info from ajax for this order
    function loadLineItems() {
        //Place a 'loading line items' ui block
        $.getJSON(purchasing._getOption("LoadLineItemsUrl"), null, function (result) {
            console.log(result);
            var newLineItemsNeeded = result.lineItems.length - startingLineItemCount;

            if (newLineItemsNeeded > 0) { //Add the number of new line items needed so we have enough
                for (var j = 0; j < newLineItemsNeeded; j++) {
                    $("#add-line-item").trigger('createline');
                }
            }

            //TODO: do this with the unserialize plugin
            //Go through each line item and bind it to the ui
            for (var i = 0; i < result.lineItems.length; i++) {
                var prefix = "items[" + i + "].";

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
