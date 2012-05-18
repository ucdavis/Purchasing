///<reference path="Order.js"/>
//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    "use strict";
    //Private Property
    var lineItemAndSplitSections = "#line-items-section, #order-split-section, #order-account-section";

    //Public Method
    purchasing.initEdit = function () {
        loadLineItemsAndSplits({ disableModification: true });
        attachModificationEvents();
    };

    //Public Method
    purchasing.initCopy = function () {
        loadLineItemsAndSplits({ disableModification: false });
    };

    function loadLineItemsAndSplits(options) {
        $.getJSON(purchasing._getOption("GetLineItemsAndSplitsUrl"), null, function (result) {
            //manual mapping for now.  can look into mapping plugin later
            var model = purchasing.OrderModel;
            model.items.removeAll();
            model.splitType(result.splitType);
            model.shipping(purchasing.displayAmount(result.orderDetail.Shipping));
            model.freight(purchasing.displayAmount(result.orderDetail.Freight));
            model.tax(purchasing.displayPercent(result.orderDetail.Tax));

            purchasing.OrderModel.disableSubaccountLoading = true; //Don't search subaccounts when loading existing selections
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

                if (model.splitType() === "Line") {
                    var lineSplitCount = model.lineSplitCount(); //keep starting split count, and increment until we push the lineItem
                    $.each(result.splits, function (i, split) {
                        if (split.LineItemId === lineResult.Id) {
                            //Add split because it's for this line
                            var newSplit = new purchasing.LineSplit(lineSplitCount++, lineItem);

                            addAccountIfNeeded(split.Account, split.AccountName);
                            addSubAccountIfNeeded(split.SubAccount, newSplit.subAccounts);

                            newSplit.amountComputed(split.Amount);
                            newSplit.account(split.Account);
                            newSplit.subAccount(split.SubAccount);
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

                    addAccountIfNeeded(split.Account, split.AccountName);
                    addSubAccountIfNeeded(split.SubAccount, newSplit.subAccounts);

                    newSplit.amountComputed(split.Amount);
                    newSplit.account(split.Account);
                    newSplit.subAccount(split.SubAccount);
                    newSplit.project(split.Project);

                    model.splits.push(newSplit);
                });
            }

            //Add the basic account info if there are no splits (aka just one split)
            if (model.splitType() === "None") {
                var singleSplit = result.splits[0];

                if (singleSplit) {
                    addAccountIfNeeded(singleSplit.Account, singleSplit.AccountName);
                    addSubAccountIfNeeded(singleSplit.SubAccount, model.subAccounts);

                    model.account(singleSplit.Account);
                    model.subAccount(singleSplit.SubAccount);
                    model.project(singleSplit.Project);
                }
            }

            $(".account-number").change(); //notify that account numbers were changed to update tip UI

            purchasing.repopulateSubAccounts(model.splitType());

            if (options.disableModification) {
                disableLineItemAndSplitModification();
            }
        });
    }

    //If the account is not in list of accounts, add it
    function addAccountIfNeeded(account, accountName) {
        if (account) {
            var accountIfFound = ko.utils.arrayFirst(purchasing.OrderModel.accounts(), function (item) {
                return item.id === account;
            });

            if (accountIfFound === null) { //not found, add to list
                purchasing.OrderModel.addAccount(account, account, accountName);
            }
        }
    }

    //If the subAccount is not in the associated subAccount list, add it
    function addSubAccountIfNeeded(subAccount, subAccounts) {
        if (subAccount) {
            var subAccountIfFound = ko.utils.arrayFirst(subAccounts(), function (item) {
                return item === subAccount;
            });

            if (subAccountIfFound === null) { //not found, add to list
                purchasing.OrderModel.addSubAccount(subAccounts, subAccount, subAccount, '');
            }
        }
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

    purchasing.repopulateSubAccounts = function (splitType) {
        $.get(purchasing._getOption("GetSubAccounts"), null, function (result) {
            $(result).each(function () { //First load all subaccounts for this order and go through each account
                var account = this.Account;
                var subAccounts = this.SubAccounts;

                //Now for each account look through accounts depending on splits and load matching subAccounts
                if (splitType === "None") { //easy, just load the subaccounts in and don't overwrite any values
                    loadSubAccountsForSplit(purchasing.OrderModel, account, subAccounts);
                }
                else if (splitType === "Order") { //go through line splits and load subaccounts when the account matches
                    $(purchasing.OrderModel.splits()).each(function () {
                        if (this.account() === account) {
                            loadSubAccountsForSplit(this, account, subAccounts);
                        }
                    });
                }
                else if (splitType === "Line") {
                    $(purchasing.OrderModel.items()).each(function () {
                        $(this.splits()).each(function () {
                            if (this.account() === account) {
                                loadSubAccountsForSplit(this, account, subAccounts);
                            }
                        });
                    });
                }
            });

            purchasing.OrderModel.disableSubaccountLoading = false; //Turn auto subaccount loading back on now that we are finished
        });
    };

    function loadSubAccountsForSplit(model, account, subAccounts) { //Add subaccounts to model.subAccounts if they don't exist
        $(subAccounts).each(function () {
            if (model.subAccounts.indexOf(this) === -1) {
                purchasing.OrderModel.addSubAccount(model.subAccounts, this, this, '');
            }
        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));
