///<reference path="jquery-1.6.2-vsdoc.js"/>
///<reference path="OrderMockup.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Private Property
    var routingAdjusted = false;
    var startingLineItemCount = 3;
    var startingOrderSplitCount = 3; //TODO: move these into a public var?  or options or something?  Statics maybe?
    var startingLineItemSplitCount = 2;

    //Public Method
    purchasing.initEdit = function () {
        loadLineItemsAndSplits(); //TODO: better name?
    };

    function loadLineItemsAndSplits() {
        //Place a 'loading line items' ui block
        $.getJSON(purchasing._getOption("GetLineItemsAndSplitsUrl"), null, function (result) {
            console.log(result);
            var newLineItemsNeeded = result.lineItems.length - startingLineItemCount;

            if (newLineItemsNeeded > 0) { //Add the number of new line items needed so we have enough
                for (var j = 0; j < newLineItemsNeeded; j++) {
                    $("#add-line-item").trigger('createline');
                }
            }

            var isLineItemSplit = result.splitType === "Line";

            if (isLineItemSplit) {
                $("#split-by-line").trigger('click', { prompt: false });
            } else {
                createSplits(result);
            }

            //TODO: do this with the unserialize plugin??
            //Go through each line item and bind it to the ui
            for (var i = 0; i < result.lineItems.length; i++) {
                var prefix = "items[" + i + "].";

                for (var prop in result.lineItems[i]) {
                    var inputName = prefix + purchasing.lowerCaseFirstLetter(prop);

                    $(document.getElementsByName(inputName)).val(result.lineItems[i][prop]);
                }

                if (isLineItemSplit) {
                    var splitsForThisLine = $.map(result.splits, function (val) {
                        return val.LineItemId === result.lineItems[i]["Id"] ? val : null;
                    });

                    console.log("splits for line", splitsForThisLine);

                    var numNewSplitsNeeded = splitsForThisLine.length - startingLineItemSplitCount;

                    if (numNewSplitsNeeded > 0) { //Add the number of splits to this line item so we have enough
                        var splitButton = $(".sub-line-item-split-body[data-line-item-id='" + i + "']").next().find(".add-line-item-split");

                        for (var k = 0; k < numNewSplitsNeeded; k++) {
                            splitButton.trigger('createsplit');
                        }
                    }
                }
            }
        });
    }

    //Create splits for order splits and no split cases
    function createSplits(data) {
        if (data.splitType === "Order") {
            $("#split-order").trigger('click', { prompt: false });

            var newSplitsNeeded = data.splits.length - startingOrderSplitCount;

            if (newSplitsNeeded > 0) {
                for (var j = 0; j < newSplitsNeeded; j++) {
                    $("#add-order-split").trigger('createsplit');
                }
            }

            //TODO: actually bind the data
        }
        else if (data.splitType === "None") {
            var singleSplit = data.splits[0];

            if (singleSplit.Account !== null) {//we have account info, bind
                var $accountSelect = $("select[name=Account]"); //TODO: maybe use class instead?

                if (!purchasing.selectListContainsValue($accountSelect, singleSplit.Account)) {
                    //Add the account to the list if it is not already in the select
                    $("#select-option-template").tmpl({ id: singleSplit.Account, name: singleSplit.Account }).appendTo($accountSelect);
                }

                $accountSelect.val(singleSplit.Account);
                $("select[name=SubAccount]").val(singleSplit.SubAccount); //TODO: handle sub account
                $("select[name=Project]").val(singleSplit.Project);
            }
        }
    }

    purchasing.lowerCaseFirstLetter = function (w) {
        return w.charAt(0).toLowerCase() + w.slice(1);
    };

    purchasing.selectListContainsValue = function ($select, val) {
        var exists = false;
        $select.find("option").each(function () {
            if (this.value === val) {
                exists = true;
                console.log("value of option", this.value);
            }
        });

        return exists;
    };

} (window.purchasing = window.purchasing || {}, jQuery));
