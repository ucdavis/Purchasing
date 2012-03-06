///<reference path="Order.js"/>
//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    "use strict";
    //Private Property
    var routingAdjusted = false;
    var startingLineItemCount = 3;
    var startingOrderSplitCount = 3; //TODO: move these into a public var?  or options or something?  Statics maybe?
    var startingLineItemSplitCount = 1;
    var lineItemAndSplitSections = "#line-items-section, #order-split-section, #order-account-section";

    //Public Method
    purchasing.initEdit = function () {
        //loadLineItemsAndSplits({ disableModification: true }); //TODO: better name?
        //attachModificationEvents();
    };

    //Public Method
    purchasing.initCopy = function () {
        //loadLineItemsAndSplits({ disableModification: false });
    };

    purchasing.initKoEdit = function () {
        koLoadLineItemsAndSplits({ disableModification: true });
        attachModificationEvents();
    };

    purchasing.initKoCopy = function () {
        koLoadLineItemsAndSplits({ disableModification: false });
    };

    function koLoadLineItemsAndSplits(options) {
        console.log($("#shipping").val());
        $.getJSON(purchasing._getOption("GetLineItemsAndSplitsUrl"), null, function (result) {
            //manual mapping for now.  can look into mapping plugin later
            var model = purchasing.OrderModel;
            model.items.removeAll();
            model.splitType(result.splitType);
            model.shipping(purchasing.displayAmount(result.orderDetail.Shipping));
            model.freight(purchasing.displayAmount(result.orderDetail.Freight));
            model.tax(purchasing.displayPercent(result.orderDetail.Tax));
            
            $.each(result.lineItems, function (index, lineResult) {
                var lineItem = new purchasing.LineItem(index, model);

                lineItem.quantity(lineResult.Quantity);
                lineItem.unit(lineResult.Units);
                lineItem.desc(lineResult.Description);
                lineItem.price(lineResult.Price);
                lineItem.catalogNumber(lineResult.CatalogNumber);

                lineItem.commodity(lineResult.CommodityCode);
                lineItem.url(lineResult.Url);
                lineItem.note(lineResult.Notes);

                if (lineItem.hasDetails()) {
                    lineItem.showDetails(true);
                }

                //Do we need to look at line splits?
                if (model.splitType() === "Line") {
                    $.each(result.splits, function (i, split) {
                        if (split.LineItemId === lineResult.Id) {
                            //Add split because it's for this line
                            var newSplit = new purchasing.LineSplit(model.lineSplitCount(), lineItem);

                            newSplit.amountComputed(split.Amount);
                            newSplit.account(split.Account);
                            newSplit.subAccount(split.subAccount);
                            newSplit.project(split.Project);

                            lineItem.splits.push(newSplit);
                        }
                    });
                }

                model.items.push(lineItem);
            });

            //Lines are in, now add Order splits if needed
            if (model.splitType() === "Order") {
                $.each(result.splits, function (i, split) {
                    //Create a new split, index starting at 0, and the model is the order/$root
                    var newSplit = new purchasing.OrderSplit(i, model);

                    newSplit.amountComputed(split.Amount);
                    newSplit.account(split.Account);
                    newSplit.subAccount(split.subAccount);
                    newSplit.project(split.Project);

                    model.splits.push(newSplit);
                });
            }

            //Add the basic account info if there are no splits
            if (model.splitType() === "None") {
                bindSplitlessOrder(result); //TODO: for now just use the splitless order binding
            }

            if (options.disableModification) {
                disableLineItemAndSplitModification();
            }
        });
    }

    function attachModificationEvents() {
        $("#item-modification-template").tmpl({}).insertBefore("#line-items-section");

        purchasing.updateNav(); //Update navigation now that we inserted a new section

        $("#item-modification-button").click(function (e, data) {
            e.preventDefault();

            var automate = data === undefined ? false : data.automate;

            if (!automate && !confirm(purchasing.getMessage("ConfirmModification"))) {
                return;
            }

            enableLineItemAndSplitModification();
        });
    }

    function disableLineItemAndSplitModification() {
        $(":input", lineItemAndSplitSections).attr("disabled", "disabled");
        $("a.button, a.biggify", lineItemAndSplitSections).hide();
        purchasing.OrderModel.adjustRouting("False");
    }

    function enableLineItemAndSplitModification() {
        $(":input", lineItemAndSplitSections).removeAttr("disabled");
        $("a.button, a.biggify", lineItemAndSplitSections).show();
        $("#item-modification-section").hide();

        //adjust the routing and for a reevaluation of the the split type so the UI updates
        purchasing.OrderModel.adjustRouting("True");
        purchasing.OrderModel.splitType.valueHasMutated();
    }

    function bindSplitlessOrder(data) {
        var singleSplit = data.splits[0];

        if (singleSplit && singleSplit.Account !== null) {//we have account info, bind
            var $accountSelect = $("select.account-number");

            if (!purchasing.selectListContainsValue($accountSelect, singleSplit.Account)) {
                //Add the account to the list if it is not already in the select
                $("#select-option-template").tmpl({ id: singleSplit.Account, name: singleSplit.Account, title: singleSplit.AccountName }).appendTo($accountSelect);
            }

            $accountSelect.val(singleSplit.Account);
            $("input.account-projectcode").val(singleSplit.Project);

            if (singleSplit.SubAccount !== null) {
                var $subAccountSelect = $("select.account-subaccount");
                loadSubAccountsAndBind(singleSplit.Account, singleSplit.SubAccount, $subAccountSelect);
            }
        }
    }

    //TODO: only call if subaccount != null, maybe refactor to move redundant code to Order.js
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

            if (routingAdjusted) {
                $subAccountSelect.removeAttr("disabled");
            }
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
