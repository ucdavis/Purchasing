///<reference path="jquery.unserializeform.js"/>
///<reference path="Order.js"/>
//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    "use strict";
    var orderform = "orderform";
    var orderfinancial = "orderfinancial";
    var storeOrderFormTimer;

    //Public Method
    purchasing.initLocalStorage = function () {
        if (window.Modernizr.localstorage) {
            attachFormSerializationEvents();
            attachAutosaveEvents();
        }
    };

    purchasing.initTourNotification = function () {
        if (window.Modernizr.localstorage) {
            attachTourEvents();
        }
    };

    purchasing.initSaveOrderRequest = function (url) {
        attachSaveEvents(url);
    };


    function attachAutosaveEvents() {
        loadExistingForm();

        $("#order-form").delegate(":input", 'change', function () {
            var delay = 1000; //delay the save one second to avoid gratuitous saving
            clearTimeout(storeOrderFormTimer);
            storeOrderFormTimer = setTimeout(purchasing.storeOrderForm, delay);
        });

        $("#order-form").submit(function () { //TODO: maybe subscribe to an event so only valid orders clear the localstorage
            localStorage.removeItem(orderform); //On submit, clear the saved temp data
        });

        function loadExistingForm() {
            if (window.location.toString().indexOf("?loadform=true") !== -1) {
                purchasing.loadOrderForm();
                return;
            }

            var savedForm = localStorage[orderform];

            if (savedForm !== undefined && savedForm !== null) {
                if (window.confirm(purchasing.getMessage('LoadForm'))) {
                    purchasing.loadOrderForm();
                } else {
                    localStorage.removeItem(orderform);
                }
            }
        }
    }

    function attachFormSerializationEvents() {
        purchasing.storeOrderForm = function () {
            localStorage.setItem(orderform, $("#order-form").serialize());
            localStorage.setItem(orderfinancial, ko.toJSON(purchasing.OrderModel));
        };

        purchasing.loadOrderForm = function () {
            $("#order-form").unserializeForm(localStorage[orderform],
                {
                    'callback': bindFormValues,
                    'override-values': true
                }
            );

            loadFinancialData(localStorage[orderfinancial]);
        };

        function loadFinancialData(financialData) {
            purchasing.OrderModel.disableSubaccountLoading = true;

            var data = JSON.parse(financialData); //TODO: checkout plugin for IE7 and older

            var model = purchasing.OrderModel;
            model.items.removeAll();
            model.splitType(data.splitType);
            model.shipping(data.shipping);
            model.freight(data.freight);
            model.tax(data.tax);

            purchasing.OrderModel.disableSubaccountLoading = true; //Don't search subaccounts when loading existing selections
            $.each(data.items, function (index, item) {
                var lineItem = new purchasing.LineItem(index, model);

                lineItem.quantity(item.quantity);
                lineItem.unit(item.unit);
                lineItem.desc(item.desc);
                lineItem.price(item.price);
                lineItem.catalogNumber(item.catalogNumber);

                lineItem.commodity(item.commodityCode);
                lineItem.url(item.url);
                lineItem.note(item.notes);

                if (lineItem.hasDetails()) {
                    lineItem.showDetails(true);
                }

                if (model.splitType() === "Line") {
                    var lineSplitCount = model.lineSplitCount(); //keep starting split count, and increment until we push the lineItem
                    $.each(item.splits, function (i, split) {
                        var newSplit = new purchasing.LineSplit(lineSplitCount++, lineItem);

                        addAccountIfNeeded(split.account, split.account);
                        addSubAccountIfNeeded(split.subAccount, newSplit.subAccounts);

                        newSplit.amountComputed(split.amount);
                        newSplit.account(split.account);
                        newSplit.subAccount(split.subAccount);
                        newSplit.project(split.project);

                        lineItem.splits.push(newSplit);
                    });
                }

                model.items.push(lineItem);
            });

            //Lines are in, now add Order splits if needed
            if (model.splitType() === "Order") {
                $.each(data.splits, function (i, split) {
                    //Create a new split, index starting at 0, and the model is the order/$root
                    var newSplit = new purchasing.OrderSplit(i, model);

                    addAccountIfNeeded(split.account, split.account);
                    addSubAccountIfNeeded(split.subAccount, newSplit.subAccounts);

                    newSplit.amountComputed(split.amount);
                    newSplit.account(split.account);
                    newSplit.subAccount(split.subAccount);
                    newSplit.project(split.project);

                    model.splits.push(newSplit);
                });
            }

            //Add the basic account info if there are no splits (aka just one split)
            if (model.splitType() === "None") {
                addAccountIfNeeded(data.account, data.account);
                addSubAccountIfNeeded(data.subAccount, model.subAccounts);

                model.account(data.account);
                model.subAccount(data.subAccount);
                model.project(data.project);
            }

            purchasing.OrderModel.disableSubaccountLoading = false; //Turn auto subaccount loading back on now that we are finished
        }

        //If the account is not in list of accounts, add it
        function addAccountIfNeeded(account, accountName) {
            if (account === undefined) {
                return;
            }

            var accountIfFound = ko.utils.arrayFirst(purchasing.OrderModel.accounts(), function (item) {
                return item.id === account;
            });

            if (accountIfFound === null) { //not found, add to list
                purchasing.OrderModel.addAccount(account, account, accountName);
            }
        }

        //If the subAccount is not in the associated subAccount list, add it
        function addSubAccountIfNeeded(subAccount, subAccounts) {
            if (subAccount === undefined) {
                return;
            }

            var subAccountIfFound = ko.utils.arrayFirst(subAccounts(), function (item) {
                return item === subAccount;
            });

            if (subAccountIfFound === null) { //not found, add to list
                subAccounts.push(subAccount);
            }
        }

        function bindFormValues(key, val, el) {
            if (el.is(":checkbox")) {
                if (key === 'restricted.status') {
                    el.attr('checked', 'checked');
                    $("#order-restricted-fields").show();
                    return true;
                } else if (key === 'conditionalApprovals') {
                    //find the checkbox with the correct value, and check it
                    $(":checkbox[value=" + val + "]", "#conditional-approval-section").attr("checked", "checked");
                    return true;
                }
                return false; //handle all other checkboxes regularly
            } else if (el.attr("name") === "__RequestVerificationToken") {
                return true; //don't replace the request verification token
            }

            return false;
        }
    }

    // Before the tour, save off the form if desired and put the split back into its original state
    purchasing.preTour = function (saveForm) {
        if (saveForm) {
            purchasing.storeOrderForm();

            localStorage["tour" + orderform] = localStorage[orderform];
            localStorage["tour" + orderfinancial] = localStorage[orderfinancial];
        }
    };

    //Reset the orderform to the pre tour state if desired
    purchasing.postTour = function (restoreForm) {
        if (restoreForm) {
            localStorage[orderform] = localStorage["tour" + orderform];
            localStorage[orderfinancial] = localStorage["tour" + orderfinancial];

            var append = window.location.toString().indexOf("?loadform=true") === -1 ? "?loadform=true" : "";
            window.location = window.location + append;
        } else {
            window.location = window.location; //just reload the page, losing changes
        }
    };

    function attachTourEvents() {
        checkFirstTime();

        $(".tour-message").on('click', '#hide-tour', function (e) {
            e.preventDefault();

            localStorage[userTourToken()] = true;
            $(".tour-message").remove();
        });

        $(".tour-message").on('click', '#take-tour', function (e) {
            e.preventDefault();

            //TODO: don't hide the take tour stuff while testing
            //localStorage[userTourToken()] = true;
            //$(".tour-message").remove();
            purchasing.takeTour("intro"); //take the intro tour
        });

        function checkFirstTime() {
            var usertoken = userTourToken();

            if (localStorage[usertoken] === undefined || localStorage[usertoken] === null) {
                localStorage[usertoken] = false;
            }

            var message;

            if (localStorage[usertoken] === 'false') {
                message = "Check out our guided tour for this page: ";
                var statusMessage = $("<div id='status-message' class='tour-message'>" + message +
                    "<span style='float:right'><a id='take-tour' href='#'>Take The Tour</a> | <a id='hide-tour' href='#'>No Thanks</a></span></div>");
                $(".main > header").prepend(statusMessage);
            }
        }

        function userTourToken() {
            return "user-tour-" + $("#userid").html();
        }
    }

    function attachSaveEvents(url) {
        $("#save-btn").click(function () {

            var formData = $("#order-form").serialize();
            var accountData = ko.toJSON(purchasing.OrderModel);
            var saveId = $("#saveId").data("save-id");
            var token = $("input[name='__RequestVerificationToken']").val();

            $.post(url, { saveId: saveId, formData: formData, accountData: accountData, workgroupId: 5, __RequestVerificationToken: token }, function (result) {
                $("#saveId").data("save-id", result);
            });

        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));