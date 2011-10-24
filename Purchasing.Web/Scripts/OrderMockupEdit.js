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
                $("#split-by-line").trigger('click', { automate: false });
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

                    var numNewSplitsNeeded = splitsForThisLine.length - startingLineItemSplitCount;

                    if (numNewSplitsNeeded > 0) { //Add the number of splits to this line item so we have enough
                        var splitButton = $(".sub-line-item-split-body[data-line-item-id='" + i + "']").next().find(".add-line-item-split");

                        for (var k = 0; k < numNewSplitsNeeded; k++) {
                            splitButton.trigger('createsplit');
                        }
                    }

                    //Now bind all of the splits for this line
                    bindSplitsForLine(i, result.lineItems[i], splitsForThisLine);
                }
            }

            purchasing.calculateSubTotal(); //TODO: maybe move these somewhere better? or refactor the get method? or use defer/await?
            purchasing.calculateGrandTotal();
            
            if (isLineItemSplit) purchasing.calculateLineItemAccountSplits();
        });
    }

    //Create splits for order splits and no split cases
    function createSplits(data) {
        if (data.splitType === "Order") {
            $("#split-order").trigger('click', { automate: false });

            var newSplitsNeeded = data.splits.length - startingOrderSplitCount;

            if (newSplitsNeeded > 0) {
                for (var j = 0; j < newSplitsNeeded; j++) {
                    $("#add-order-split").trigger('createsplit');
                }
            }

            bindOrderSplits(data);
        }
        else if (data.splitType === "None") {
            bindSplitlessOrder(data);
        }
    }

    function bindSplitsForLine(rowIndex, line, splits) {
        console.log(rowIndex);

        var splitsToBind = $(".sub-line-item-split-body[data-line-item-id='" + rowIndex + "'] > tr");

        splitsToBind.each(function (index, row) {
            var $splitRow = $(row);
            console.log("splits for " + splits[index].LineItemId, splits[index]);

            var $splitAccountSelect = $splitRow.find("select.account-number");
            var lineItemId = splits[index].LineItemId;
            var account = splits[index].Account;
            var subAccount = splits[index].SubAccount;
            var amount = splits[index].Amount;

            if (!purchasing.selectListContainsValue($splitAccountSelect, account)) {
                //Add the account to the list if it is not already in the select
                $("#select-option-template").tmpl({ id: account, name: account }).appendTo($splitAccountSelect);
            }

            $splitAccountSelect.val(account);

            $splitRow.find("input.account-projectcode").val(splits[index].Project);
            $splitRow.find("input.line-item-split-account-amount").val(amount);
            $splitRow.find("input.line-item-split-item-id").val(lineItemId);

            if (subAccount != null) {
                var $splitSubAccountSelect = $splitRow.find("select.account-subaccount");
                loadSubAccountsAndBind(account, subAccount, $splitSubAccountSelect);
            }
        });
    }

    function bindSplitlessOrder(data) {
        var singleSplit = data.splits[0];

        if (singleSplit.Account !== null) {//we have account info, bind
            var $accountSelect = $("select.account-number");

            if (!purchasing.selectListContainsValue($accountSelect, singleSplit.Account)) {
                //Add the account to the list if it is not already in the select
                $("#select-option-template").tmpl({ id: singleSplit.Account, name: singleSplit.Account }).appendTo($accountSelect);
            }

            $accountSelect.val(singleSplit.Account);
            $("input.account-projectcode").val(singleSplit.Project);

            if (singleSplit.SubAccount != null) {
                var $subAccountSelect = $("select.account-subaccount");
                loadSubAccountsAndBind(singleSplit.Account, singleSplit.SubAccount, $subAccountSelect);
            }
        }
    }

    function bindOrderSplits(data) {
        for (var i = 0; i < data.splits.length; i++) {
            var splitPrefix = "splits[" + i + "].";
            var $splitAccountSelect = $("select.account-number").filter("[name='" + splitPrefix + "Account']");
            var account = data.splits[i].Account;
            var subAccount = data.splits[i].SubAccount;
            var amount = data.splits[i].Amount;

            if (!purchasing.selectListContainsValue($splitAccountSelect, account)) {
                //Add the account to the list if it is not already in the select
                $("#select-option-template").tmpl({ id: account, name: account }).appendTo($splitAccountSelect);
            }

            $splitAccountSelect.val(account);

            $("input.account-projectcode").filter("[name='" + splitPrefix + "Project']").val(data.splits[i].Project);
            $("input.order-split-account-amount").filter("[name='" + splitPrefix + "amount']").val(amount);
            //TODO: figure out how to create percentages, hopefully without looping through each split again.

            if (subAccount != null) {
                var $splitSubAccountSelect = $("select.account-subaccount").filter("[name='" + splitPrefix + "SubAccount']");
                loadSubAccountsAndBind(account, subAccount, $splitSubAccountSelect);
            }
        }

        purchasing.calculateOrderAccountSplits();
    }

    //TODO: only call if subaccount != null, maybe refactor to move redundant code to OrderMockup.js
    function loadSubAccountsAndBind(account, subAccount, $subAccountSelect) {
        $.getJSON(purchasing._getOption("KfsSearchSubAccountsUrl"), { accountNumber: account }, function (result) {
            $subAccountSelect.find("option:not(:first)").remove();

            if (result.length > 0) {
                var data = $.map(result, function (n, i) { return { name: n.Name, id: n.Id }; });

                $("#select-option-template").tmpl(data).appendTo($subAccountSelect);
            }

            if (!purchasing.selectListContainsValue($subAccountSelect, subAccount)) {
                //If we still don't have the necessary value in sub account, add it in (shouldn't happen?)
                $("#select-option-template").tmpl({ id: subAccount, name: subAccount }).appendTo($subAccountSelect);
            }

            $subAccountSelect.val(subAccount);
            $subAccountSelect.removeAttr("disabled");
        });
    }

    purchasing.lowerCaseFirstLetter = function (w) {
        return w.charAt(0).toLowerCase() + w.slice(1);
    };

    purchasing.selectListContainsValue = function ($select, val) {
        var exists = false;
        $select.find("option").each(function () {
            if (this.value === val) {
                exists = true;
            }
        });

        return exists;
    };

} (window.purchasing = window.purchasing || {}, jQuery));
