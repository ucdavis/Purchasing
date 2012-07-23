///<reference path="fileuploader.js"/>
///<reference path="SausageCustom.js"/>
///<reference path="jquery.stickyfloat.js"/>
///<reference path="jquery.tmpl.min.js"/>
///<reference path="knockout-2.0.0.0.js"/>
//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    "use strict";
    //Private Property
    var options = { invalidNumberClass: "invalid-number-warning", invalidLineItemClass: "line-item-warning", validLineItemClass: "line-item-ok", lineItemDataIndex: "line-item-index", lineAddedEvent: "lineadded", lineItemIndex: 0, splitIndex: 0 };

    //Public Property
    purchasing.splitType = "None"; //Keep track of current split [None,Order,Line]

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing._getOption = function (prop) {
        return options[prop];
    };

    purchasing.getMessage = function (key) {
        return options.Messages[key];
    };

    purchasing.init = function () {
        $(".button").button();

        attachFormEvents();
        attachVendorEvents();
        attachAddressEvents();

        //Hookup line items, splits, and account info to knockout
        extendKnockout();
        createSplitModels();
        createLineModel();
        createOrderModel();
        ko.applyBindings(purchasing.OrderModel);

        purchasing.setupCommoditySearch();
        attachKeyboardShortcutEvents();
        attachAccountSearchEvents();
        attachRestrictedItemsEvents();
        attachFileUploadEvents();
        attachCalculatorEvents();
        attachToolTips();
        attachShippingWarnings();
        attachTour();
        attachNav();
    };

    function extendKnockout() {
        function isBlank(val) {
            if (val) {
                return false;
            }
            return true;
        }

        ko.bindingHandlers['itemName'] = {
            'update': function (element, valueAccessor, all, model) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                element.name = "items[" + model.index() + "]." + value;
            }
        };

        ko.bindingHandlers['splitName'] = {
            'update': function (element, valueAccessor, all, model) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                element.name = "splits[" + model.index() + "]." + value;
            }
        };

        ko.extenders.verifyNumber = function (target, option) {
            target.hasError = ko.observable(false);

            target.subscribe(function (val) {
                if (isNaN(val)) {
                    target.hasError(true);
                } else {
                    target.hasError(false);
                }
            });
            return target;
        };

        ko.bindingHandlers.lineValid = {
            update: function (element, valueAccessor, allBindingsAccessor, model) {
                var row = $(element).parentsUntil("#line-items-body", ".line-item-row"); //find the row

                var quantityBlank = isBlank(model.quantity());
                var priceBlank = isBlank(model.price());
                var descriptionBlank = isBlank(model.desc());

                if (quantityBlank && descriptionBlank & priceBlank) {
                    row.find(":input").removeClass(options.invalidLineItemClass).removeClass(options.validLineItemClass);
                }
                else {
                    row.find(".quantity").toggleClass(options.invalidLineItemClass, quantityBlank).toggleClass(options.validLineItemClass, !quantityBlank);
                    row.find(".description").toggleClass(options.invalidLineItemClass, descriptionBlank).toggleClass(options.validLineItemClass, !descriptionBlank);
                    row.find(".price").toggleClass(options.invalidLineItemClass, priceBlank).toggleClass(options.validLineItemClass, !priceBlank);
                }

                if (!quantityBlank && !descriptionBlank && !priceBlank &&
                    !model.quantity.hasError() && !model.price.hasError()) {
                    model.valid(true); //valid if none of these are blank AND price/quantity do not have errors
                } else {
                    model.valid(false);
                }
            }
        };
    }

    function createSplitModels() {
        purchasing.LineSplit = function (index, item) {
            var self = this;

            self.lineId = ko.observable(item.id());
            self.index = ko.observable(index);
            self.amount = ko.observable();
            self.percent = ko.observable();

            //Accounts
            self.account = ko.observable();
            self.subAccount = ko.observable();
            self.project = ko.observable();
            self.subAccounts = ko.observableArray();

            //Subscriptions
            self.loadSubaccountsSubscription = createLoadSubaccountsSubscription(self);

            //Methods
            self.valid = ko.computed(function () { //valid if there is an account selected and a positive amount
                return (self.account() !== '' && self.amount() > 0);
            });

            self.empty = ko.computed(function () { //empty != invalid, but it means no account or amount is selected
                return (self.account() === '' && !parseFloat(self.amount()));
            });

            self.amountComputed = ko.computed({
                read: function () { return self.amount(); },
                write: function (value) {
                    if (isNaN(value) || value === '') {
                        self.percent('');
                        self.amount('');
                        self.amountComputed.notifySubscribers('');
                        return;
                    }

                    var amount = parseFloat(purchasing.cleanNumber(value));
                    var lineTotal = parseFloat(purchasing.cleanNumber(item.lineTotal()));

                    if (!isNaN(amount) && lineTotal !== 0) {
                        var percent = 100 * (amount / lineTotal);
                        self.percent(percent.toFixed(3));
                        self.amount(amount);
                    }
                }
            });

            self.percentComputed = ko.computed({
                read: function () {
                    return self.percent();
                },
                write: function (value) {
                    if (isNaN(value) || value === '') {
                        self.percent('');
                        self.amount('');
                        self.percentComputed.notifySubscribers('');
                        return;
                    }

                    var lineTotal = parseFloat(purchasing.cleanNumber(item.lineTotal()));
                    var percent = value / 100;
                    var amount = lineTotal * percent;

                    if (!isNaN(amount)) {
                        self.amount(amount.toFixed(3));
                    }
                }
            });
        };

        purchasing.OrderSplit = function (index, order) {
            var self = this;

            self.index = ko.observable(index);
            self.amount = ko.observable();
            self.percent = ko.observable();

            //Accounts
            self.account = ko.observable();
            self.subAccount = ko.observable();
            self.project = ko.observable();
            self.subAccounts = ko.observableArray();

            //Subscriptions
            self.loadSubaccountsSubscription = createLoadSubaccountsSubscription(self);

            //Methods
            self.valid = ko.computed(function () { //valid if there is an account selected and a positive amount
                return (self.account() !== '' && self.amount() > 0);
            });

            self.empty = ko.computed(function () { //empty != invalid, but it means no account or amount is selected
                return (self.account() === '' && !parseFloat(self.amount()));
            });

            self.amountComputed = ko.computed({
                read: function () { return self.amount(); },
                write: function (value) {
                    if (isNaN(value) || value === '') {
                        self.percent(null);
                        self.amount(null);
                        self.amountComputed.notifySubscribers(null);
                        return;
                    }

                    var amount = parseFloat(purchasing.cleanNumber(value));
                    var total = parseFloat(purchasing.cleanNumber(order.grandTotal()));

                    if (!isNaN(amount) && total !== 0 && amount !== 0) {
                        var percent = 100 * (amount / total);
                        self.percent(percent.toFixed(3));
                        self.amount(amount);
                    }
                }
            });

            self.percentComputed = ko.computed({
                read: function () {
                    return self.percent();
                },
                write: function (value) {
                    if (isNaN(value) || value === '') {
                        self.percent(null);
                        self.amount(null);
                        self.percentComputed.notifySubscribers(null);
                        return;
                    }

                    var total = parseFloat(purchasing.cleanNumber(order.grandTotal()));
                    var percent = value / 100;
                    var amount = total * percent;

                    if (!isNaN(amount)) {
                        self.amount(amount.toFixed(3));
                    }
                }
            });
        };
    }

    function createLineModel() {
        purchasing.LineItem = function (index, order) {
            var self = this;
            self.id = ko.observable(index + 1);
            self.index = ko.observable(index);

            self.valid = ko.observable(false);
            self.showDetails = ko.observable(false);

            self.quantity = ko.observable().extend({ verifyNumber: [] });
            self.unit = ko.observable("EA");
            self.catalogNumber = ko.observable();
            self.desc = ko.observable().extend({ lineStatus: [] });
            self.price = ko.observable().extend({ verifyNumber: [] });

            self.commodity = ko.observable();
            self.url = ko.observable();
            self.note = ko.observable();

            self.splits = ko.observableArray([]);

            self.url.subscribe(function () {
                var orderValidate = $("#order-form").validate();
                $(".url").each(function () {
                    orderValidate.element(this);
                });
            });

            self.hasDetails = function () {
                return self.commodity() || self.url() || self.note();
            };

            self.splitTotal = ko.computed(function () {
                var splitTotal = 0;
                $.each(self.splits(), function (i, val) {
                    splitTotal += parseFloat(purchasing.cleanNumber(val.amount()));
                });

                return purchasing.displayAmount(splitTotal);
            });

            self.total = ko.computed(function () {
                var total = self.quantity() * self.price();
                return purchasing.displayAmount(total);
            });

            self.lineTotal = ko.computed(function () {
                var taxPercent = purchasing.cleanNumber(order.tax());
                var taxRate = taxPercent / 100.00;

                var lineTotal = purchasing.cleanNumber(self.total()) * (1 + taxRate);

                return purchasing.displayAmount(lineTotal);
            });

            self.hasLineTotal = ko.computed(function () {
                var lineTotal = parseFloat(purchasing.cleanNumber(self.lineTotal()));

                return lineTotal !== 0;
            });

            self.lineUnaccounted = ko.computed(function () {
                var lineTotal = parseFloat(purchasing.cleanNumber(self.lineTotal()));
                var splitTotal = parseFloat(purchasing.cleanNumber(self.splitTotal()));

                return purchasing.displayAmount(lineTotal - splitTotal);
            });

            self.hasUnaccounted = ko.computed(function () {
                var unaccounted = parseFloat(purchasing.cleanNumber(self.lineUnaccounted()));

                return unaccounted !== 0;
            });

            self.lineTaxCost = ko.computed(function () {
                var taxPercent = purchasing.cleanNumber(order.tax());
                var taxRate = taxPercent / 100.00;

                var taxCost = purchasing.cleanNumber(self.total()) * (taxRate);

                return purchasing.displayAmount(taxCost);
            });

            self.toggleDetails = function () {
                self.showDetails(!self.showDetails());
            };

            self.addSplit = function () {
                self.splits.push(new purchasing.LineSplit(order.lineSplitCount(), self));
            };
        };
    }

    function createOrderModel() {
        purchasing.Account = function (id, text, title) {
            this.id = id;
            this.text = text;
            this.title = title;
        };

        purchasing.OrderModel = new function () {
            var self = this;
            self.showLines = true; //hides lines until KO loads
            self.adjustRouting = ko.observable('True'); //true if we are editing and want to adjust the lines/splits
            self.splitType = ko.observable("None");  //ko.observable("None");
            self.disableSubaccountLoading = false;

            //Order details
            self.orderSplitType = ko.observable("account"); //Default to account order split type

            self.shipping = ko.observable('$0.00');
            self.freight = ko.observable('$0.00');
            self.tax = ko.observable('7.25%');

            //Account info
            self.account = ko.observable();
            self.subAccount = ko.observable();
            self.project = ko.observable();

            self.accounts = ko.observableArray([new purchasing.Account('', "-- Account --", "No Account Selected")]);
            self.subAccounts = ko.observableArray();

            //Items & Splits
            self.items = ko.observableArray(
                [new purchasing.LineItem(0, self),
                    new purchasing.LineItem(1, self),
                    new purchasing.LineItem(2, self)
                ]); //default to 3 line items

            self.splits = ko.observableArray(); //for order-level splits

            //Subscriptions
            self.items.subscribe(function () {
                purchasing.setupCommoditySearch(); //When items get updated, setup commodity code search
            });

            self.splitType.subscribe(function () {
                //when split type changes we are adding/removing sections, so update the nav
                setTimeout(purchasing.updateNav, 100);
            });

            self.loadSubaccountsSubscription = createLoadSubaccountsSubscription(self);

            //Methods
            self.addAccount = function (value, text, title) {
                self.accounts.push(new purchasing.Account(value, text, title));
            };

            self.addSubAccount = function (subAccounts, value, text, title) {
                if (value) {
                    subAccounts.push(new purchasing.Account(value, text, title));
                }
            };

            self.addLine = function () {
                self.items.push(new purchasing.LineItem(self.items().length, self));
            };

            self.resetOrderRouting = function () {
                $("#approvers, #accountmanagers", "#account-manager").val(""); //Set AM routing options back to defaults
                self.account(""); //set account routing back to default
                self.subAccount("");
            };

            self.setOrderAccountRouting = function () {
                self.setOrderRoutingType("account");
            };

            self.setOrderApproverRouting = function () {
                self.setOrderRoutingType("approver");
            };

            self.setOrderRoutingType = function (type) {
                self.orderSplitType(type);
            };

            self.shouldEnableSubAccounts = function (subAccounts) {
                return this.adjustRouting() === 'True' && subAccounts().length > 1; //default option always present
            };

            self.clearSubAccounts = function (subAccounts) {
                subAccounts.removeAll();
                subAccounts.push(new purchasing.Account('', '--Sub Account', 'No Sub Account'));
            };

            self.splitByOrder = function () {
                if (confirm(options.Messages.ConfirmOrderSplit)) {
                    //Add 2 splits by default
                    self.splits.push(new purchasing.OrderSplit(0, self));
                    self.splits.push(new purchasing.OrderSplit(1, self));
                    self.splitType("Order");
                }
            };

            self.cancelSplitByOrder = function () {
                if (confirm(options.Messages.ConfirmCancelOrderSplit)) {
                    self.splits.removeAll();
                    self.splitType("None");
                }
            };

            self.addOrderSplit = function () {
                var index = self.splits().length;
                self.splits.push(new purchasing.OrderSplit(index, self));
            };

            self.splitByLine = function () {
                if (confirm(options.Messages.ConfirmLineSplit)) {
                    $.each(self.items(), function (index, item) {
                        item.splits.push(new purchasing.LineSplit(index, this));
                    });
                    self.splitType('Line');
                }
            };

            self.cancelLineSplit = function () {
                if (confirm(options.Messages.ConfirmCancelLineSplit)) {
                    $.each(self.items(), function (index, item) {
                        item.splits.removeAll();
                    });
                    self.splitType('None');
                }
            };

            self.lineSplitCount = function () {
                var splitCount = 0;
                $.each(self.items(), function (index, item) {
                    splitCount += item.splits().length;
                });

                return splitCount;
            };

            self.lineSplits = function () {
                var lineItemSplits = [];
                $.each(self.items(), function (index, item) {
                    $.each(item.splits(), function (i, split) {
                        lineItemSplits.push(split);
                    });
                });

                return lineItemSplits;
            };

            self.showForSplit = function (splitType) {
                return splitType === self.splitType();
            };

            self.subTotal = ko.computed(function () {
                var subTotal = 0;
                $.each(self.items(), function (index, item) {
                    subTotal += parseFloat(purchasing.cleanNumber(item.total())); //just add each line total to get subtotal
                });

                return purchasing.displayAmount(subTotal);
            });

            self.grandTotal = ko.computed(function () {
                var subTotal = parseFloat(purchasing.cleanNumber(self.subTotal()));
                var shipping = parseFloat(purchasing.cleanNumber(self.shipping()));
                var freight = parseFloat(purchasing.cleanNumber(self.freight()));
                var tax = parseFloat(purchasing.cleanNumber(self.tax()));

                var grandTotal = ((subTotal + freight) * (1 + tax / 100.00)) + shipping;

                return purchasing.displayAmount(grandTotal);
            });

            self.orderSplitTotal = ko.computed(function () {
                var splitTotal = 0;
                $.each(self.splits(), function (i, val) {
                    splitTotal += parseFloat(purchasing.cleanNumber(val.amount()));
                });

                return purchasing.displayAmount(splitTotal);
            });

            self.orderSplitUnaccounted = ko.computed(function () {
                var total = parseFloat(purchasing.cleanNumber(self.grandTotal()));
                var splitTotal = parseFloat(purchasing.cleanNumber(self.orderSplitTotal()));

                return purchasing.displayAmount(total - splitTotal);
            });

            self.hasUnaccounted = ko.computed(function () {
                var unaccounted = parseFloat(purchasing.cleanNumber(self.orderSplitUnaccounted()));

                return unaccounted !== 0;
            });

            self.insertUnaccountedValue = function (elAmount) {
                var context = ko.contextFor(elAmount);

                var unaccounted = self.splitType() === "Order" ? context.$parent.orderSplitUnaccounted() : context.$parent.lineUnaccounted();
                var current = parseFloat(purchasing.cleanNumber(context.$data.amount()));
                var remaining = parseFloat(purchasing.cleanNumber(unaccounted));

                context.$data.amount(remaining + current);
            };

            self.status = ko.computed(function () {
                var hasValidLine = false;
                $.each(self.items(), function (index, item) {
                    if (item.valid()) {
                        hasValidLine = true;
                        return false; //breaks the each
                    }
                    return true;
                });

                return hasValidLine ? "There is at least one valid line!" : "No valid lines yet...";
            });

            self.valid = function () {
                //Always make sure there are >0 valid line items
                var validLines = $.map(self.items(), function (item) {
                    return item.valid() ? item : null;
                });

                if (validLines.length === 0) {
                    alert(options.Messages.LineItemRequired);
                    return false;
                }

                //If modification is not enabled, don't check the split info
                if (self.adjustRouting() === 'False') {
                    return true;
                }

                //If no spit is chosen, make sure either account or AM are chosen
                if (purchasing.OrderModel.splitType() === "None") {
                    var hasAccount = self.account();
                    var hasAccountManager = $("#accountmanagers").val();

                    console.log("acct/am", hasAccount, hasAccountManager);

                    if ((hasAccount && hasAccountManager) || !(hasAccount || hasAccountManager)) { //XOR
                        if (hasAccountManager === undefined) {
                            alert(options.Messages.AccountRequired);
                        }
                        else {
                            alert(options.Messages.AccountOrManagerRequired);
                        }
                        return false;
                    }
                }
                else if (purchasing.OrderModel.splitType() === "Order") {
                    //If order is split, make sure all order money is accounted for
                    var splitUnaccounted = parseFloat(purchasing.cleanNumber(self.orderSplitUnaccounted()));

                    var invalidSplits = $.map(self.splits(), function (split) {
                        if (!split.valid() && !split.empty()) { //return if split is not valid and not empty
                            return split;
                        }

                        return null;
                    });

                    if (invalidSplits.length) {
                        alert(options.Messages.OrderSplitWithNoAccount);
                        return false;
                    }

                    if (splitUnaccounted !== 0) {
                        alert(options.Messages.TotalAmountRequired);
                        return false;
                    }
                }
                else if (purchasing.OrderModel.splitType() === "Line") {
                    //if line items are split, make sure #1 all money is accounted for, #2 every line item has at least one split
                    var linesWithNonMatchingAmounts = $.map(self.items(), function (item) {
                        var unaccounted = parseFloat(purchasing.cleanNumber(item.lineUnaccounted()));

                        return unaccounted !== 0 ? item : null;
                    });

                    //Make sure each split is valid or empty
                    var invalidLineSplits = $.map(self.lineSplits(), function (split) {
                        if (!split.valid() && !split.empty()) { //return if split is not valid and not empty
                            return split;
                        }

                        return null;
                    });

                    if (invalidLineSplits.length !== 0) {
                        alert(options.Messages.LineSplitNoAccount);
                        return false;
                    }

                    if (linesWithNonMatchingAmounts.length !== 0) {
                        alert(options.Messages.LineSplitTotalAmountRequired);
                        return false;
                    }
                }

                return true;
            };

            //Defaults
            self.clearSubAccounts(self.subAccounts);

            $("#defaultAccounts>option").each(function (index, account) { //setup the default accounts
                self.addAccount(account.value, account.text, account.title);
            });
        } ();
    }

    function createLoadSubaccountsSubscription(obj) {
        obj.account.subscribe(function () {
            if (purchasing.OrderModel.disableSubaccountLoading === true) {
                return;
            }

            //account changed, clear out existing subAccount info and do an ajax search for new values
            obj.subAccount();
            purchasing.OrderModel.clearSubAccounts(obj.subAccounts);

            $.getJSON(options.KfsSearchSubAccountsUrl, { accountNumber: obj.account() }, function (result) {
                $.each(result, function (index, subaccount) {
                    purchasing.OrderModel.addSubAccount(obj.subAccounts, subaccount.Id, subaccount.Name, subaccount.Title);
                });

                //notify subaccount update
                $(".account-subaccount").change();
            });
        });
    }

    purchasing.resetFinancials = function () {
        var model = purchasing.OrderModel;
        model.items.removeAll();
        model.items.push(new purchasing.LineItem(0, model));
        model.items.push(new purchasing.LineItem(1, model));
        model.items.push(new purchasing.LineItem(2, model));
        model.splits.removeAll();
        model.splitType("None");
        model.setOrderAccountRouting();
    };

    //Private method
    function attachFormEvents() {
        $("#order-form").submit(function (e) {
            if ($(this).valid() && purchasing.OrderModel.valid()) {
                if (confirm(options.Messages.ConfirmSubmit)) {
                    return;
                }
            }

            e.preventDefault();
        });
    }

    function attachTour() {
        purchasing.initTourNotification(); //attach tour notifications

        purchasing.takeTour = function (startId) {
            window.Modernizr.load({
                load: options.Guider.Assets,
                complete: function () {
                    window.tour.startTour(startId);
                }
            });
        };

        $("#order-form").on('click', ".tour-help", function (e) {
            e.preventDefault();

            purchasing.takeTour($(this).data("tourid"));
        });
    }

    purchasing.setupCommoditySearch = function () {
        var delay = 1000; //delay attaching events for one second to avoid gratuitous setup
        clearTimeout(purchasing.commoditySetupTimer);
        purchasing.commoditySetupTimer = setTimeout(attachCommoditySearchEvents, delay);

        function attachCommoditySearchEvents() {
            $(".search-commodity-code").autocomplete({
                source: function (request, response) {
                    var el = this.element[0]; //grab the element that caused the autocomplete
                    var searchTerm = el.value;

                    $.getJSON(options.SearchCommodityCodeUrl, { searchTerm: searchTerm }, function (results) {
                        if (!results.length) {
                            response([{ label: options.Messages.NoCommodityCodesMatch + ' "' + searchTerm + '"', value: searchTerm}]);
                        } else {
                            response($.map(results, function (item) {
                                return {
                                    label: item.DisplayNameAndId,
                                    value: item.Id
                                };
                            }));
                        }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    event.preventDefault();

                    ko.dataFor(this).commodity(ui.item.value);
                    $(this).attr("title", ui.item.label);
                }
            });
        }
    };

    function attachKeyboardShortcutEvents() {
        $("#order-form").on("keydown", ".line-item-split-account-amount, .order-split-account-amount", function (event) {
            if (event.ctrlKey && event.which == 13 /*enter*/) {
                event.preventDefault();
                this.blur();
                purchasing.OrderModel.insertUnaccountedValue(this);
            }
        });
    }

    function attachAccountSearchEvents() {
        $("#accounts-search-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true,
            buttons: {
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#order-form").on("click", ".search-account", function (e) {
            e.preventDefault();
            if (purchasing.OrderModel.adjustRouting() === "True") {
                //clear out inputs and empty the results table
                $("input", "#accounts-search-form").val("");
                $("#accounts-search-dialog-results > tbody").empty();

                $("#accounts-search-dialog").data("container", $(this).parents(".account-container")).dialog("open");
            } else {
                alert("You must Enable Modification before changing the account information.");
            }
        });

        $("#accounts-search-dialog-searchbox-btn").click(function (e) {
            e.preventDefault();
            searchKfsAccounts();
        });

        $("#accounts-search-dialog-searchbox").keypress(function (e) {
            if (e.which === 13) { //handle the enter key
                e.preventDefault();
                searchKfsAccounts();
            }
        });

        // trigger for selecting an account
        $("#accounts-search-dialog-results").on("click", ".result-select-btn", function () {
            var $container = $("#accounts-search-dialog").data('container');
            var row = $(this).parents("tr");
            var account = row.find(".result-account").html();
            var title = row.find(".result-name").html();

            var context = ko.contextFor($container[0]);

            //push the new choice into the accounts array
            context.$root.addAccount(account, account, title);

            //select it
            context.$data.account(account);

            $container.find(".account-number").change(); //notify the UI that we change the account

            $("#accounts-search-dialog").dialog("close");
        });

        function searchKfsAccounts() {
            var searchTerm = $("#accounts-search-dialog-searchbox").val();
            $("#accounts-search-dialog-results tbody").empty();

            $.getJSON(options.KfsSearchUrl, { searchTerm: searchTerm }, function (result) {
                if (result.length > 0) {

                    var rowData = $.map(result, function (n, i) { return { name: n.Name, account: n.Id }; });

                    $("#accounts-search-dialog-results-template").tmpl(rowData).appendTo("#accounts-search-dialog-results tbody");

                    $(".result-select-btn").button();
                }
                else {
                    var tr = $("<tr>").append($("<tr>").attr("colspan", 3).html(options.Messages.NoResults));
                    $("#accounts-search-dialog-results tbody").append(tr);
                }
            });
        }
    }

    function attachToolTips() {
        //For all inputs with titles, show the tip
        $('#order-form').on('mouseenter focus', 'input[title], select[title], a[title]', function () {
            $(this).qtip({
                overwrite: false,
                show: {
                    event: 'mouseenter focus',
                    ready: true
                },
                hide: {
                    event: 'mouseleave blur'
                },
                position: {
                    my: 'bottom center',
                    at: 'top center'
                }
            });
        });

        $("#order-form").on('change', ".account-number, .account-subaccount", function () {
            var el = $(this);
            var selectedOption = $("option:selected", el);
            var title = selectedOption.attr('title');

            if (!title) {
                el.qtip('destroy');
                el.removeAttr("title");
            } else {
                el.attr('title', title);
            }
        });
    }

    function attachNav() {
        $(window).sausage({ page: '.ui-widget-header:visible' });
        $('.orders-nav').stickyfloat({ duration: 400 });
    }

    function attachFileUploadEvents() {
        var uploader = new qq.FileUploader({
            // pass the dom node (ex. $(selector)[0] for jQuery users)
            element: document.getElementById('file-uploader'),
            // path to server-side upload script
            action: options.UploadUrl,
            fileTemplate: '<li>' +
                '<span class="qq-upload-file"></span>' +
                '<span class="qq-upload-spinner"></span>' +
                '<span class="qq-upload-size"></span>' +
                '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                '<span class="qq-upload-failed-text">Failed</span>' +
                '<a href="#" class="qq-upload-file-remove">[Remove]</a>' +
                '<input type="hidden" class="qq-upload-file-id" name="fileIds" value="" />' +
            '</li>',
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            onComplete: function (id, fileName, response) {
                var newFileContainer = $(uploader._getItemByFileId(id));
                newFileContainer.find(".qq-upload-file-id").val(response.id);
                $("#file-uploader").trigger("fileuploaded");
            },
            debug: true
        });

        $(".qq-upload-file-remove").live("click", function (e) {
            e.preventDefault();

            var fileContainer = $(this).parent();
            fileContainer.remove();
        });
    }

    function attachVendorEvents() {
        $("#vendor-dialog").dialog({
            autoOpen: false,
            height: 610,
            width: 500,
            modal: true,
            buttons: {
                "Create Vendor": function () { createVendor(this); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#search-vendor-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 550,
            modal: true,
            buttons: {
                "Select Vendor": addKfsVendor,
                Cancel: function () { $(this).dialog("close"); }
            }
        });

        $("#vendor-name").change(function () {
            checkDuplicates($("#loader-name"));
        });
        $("#vendor-address").change(function () {
            checkDuplicates($("#loader-line1"));
        });

        $("#vendor-form").on("click", "#toggle-address", function (e) {
            e.preventDefault();

            var buttonSpan = $(this).children();
            var addressSet = $("#vendor-address-fieldset");

            if (addressSet.data("enabled")) {
                $("#vendor-address", addressSet).val("N/A");
                $("#vendor-city", addressSet).val("N/A");
                $("#vendor-state", addressSet).val("CA");
                $("#vendor-zip", addressSet).val("95616");

                $("input", addressSet).prop("disabled", true);

                buttonSpan.html("Needs Address?");
                addressSet.data("enabled", false);
            } else {
                $("input", addressSet).val("");
                $("#vendor-country-code", addressSet).val("US");
                $("input", addressSet).prop("disabled", false);

                buttonSpan.html("No Address?");
                addressSet.data("enabled", true);
            }

            console.log(this);
        });

        function checkDuplicates(loader) {
            var id = $("#workgroup").val();
            var name = $("#vendor-name").val();
            var line1 = $("#vendor-address").val();
            loader.toggle();
            $.get(options.CheckDuplicateVendorUrl, { workgrougpId: id, name: name, line1: line1 }, function (result) {
                loader.toggle();
                if (result) {
                    $("#duplicateCheck").html(result.message);
                } else {
                    alert("There was a problem checking for duplicate vendors");
                }
            });
        }

        $("#add-vendor").click(function (e) {
            e.preventDefault();

            $("#vendor-dialog").dialog("open");
        });

        $("#search-vendor").click(function (e) {
            e.preventDefault();

            $("#search-vendor-dialog-searchbox").val("");
            $("#search-vendor-dialog-vendor-address option:not(:first)").remove();

            $("#search-vendor-dialog").dialog("open");
        });

        $("#search-vendor-dialog-searchbox").autocomplete({
            source: function (request, response) {
                var searchTerm = $("#search-vendor-dialog-searchbox").val();

                $.getJSON(options.SearchVendorUrl, { searchTerm: searchTerm }, function (results) {
                    response($.map(results, function (item) {
                        return {
                            label: item.Name,
                            value: item.Id
                        };
                    }));
                });
            },
            minLength: 3,
            select: function (event, ui) {
                event.preventDefault();

                $("#search-vendor-dialog-selectedvendor").val(ui.item.value);
                $(this).val(ui.item.label);

                searchVendorAddress(ui.item.value);
            }
        });

        function searchVendorAddress(vendorId) {
            $.getJSON(options.SearchVendorAddressUrl, { vendorId: vendorId }, function (results) {
                $("#search-vendor-dialog-vendor-address option:not(:first)").remove();
                var select = $("#search-vendor-dialog-vendor-address");

                if (results.length > 0) {
                    var data = $.map(results, function (n, i) { return { id: n.Id, name: n.Name }; });

                    $("#select-option-template").tmpl(data).appendTo("#search-vendor-dialog-vendor-address");

                    select.removeAttr("disabled");
                }
                else {
                    select.attr("disabled", "disabled");
                }
            });
        }

        function addKfsVendor() {
            var vendorId = $("#search-vendor-dialog-selectedvendor").val();
            var typeCode = $("#search-vendor-dialog-vendor-address").val();
            var workgroupId = $("#workgroup").val();

            if (vendorId && typeCode) {
                $.post(
                    options.AddKfsVendorUrl,
                    { workgroupId: workgroupId, vendorId: vendorId, addressTypeCode: typeCode, __RequestVerificationToken: options.AntiForgeryToken },
                    function (result) {
                        if (result.added == true) {
                            $("#select-option-template").tmpl({ id: result.id, name: result.name }).appendTo("#vendor");
                        }
                        if (result.duplicate) {
                            alert("That vendor already exists in this workgroup.");
                        }
                        if (result.wasInactive) {
                            alert("That vendor was previously removed from this workgroup. It has been added back.");
                        }
                        if (result.errorMessage != null) {
                            alert(result.errorMessage);
                        }
                        $("#vendor").val(result.id);

                        $("#search-vendor-dialog").dialog("close");
                    }
                );
            } else {
                alert("You must enter a vendor and choose an address for that vendor into order to continue");
            }
        }

        function createVendor(dialog) {
            var form = $("#vendor-form");

            if (form.validate().form() === false) {
                return; //don't create the vendor if the form is invalid
            }

            var vendorInfo = {
                name: form.find("#vendor-name").val(),
                line1: form.find("#vendor-address").val(),
                city: form.find("#vendor-city").val(),
                state: form.find("#vendor-state").val(),
                zip: form.find(("#vendor-zip")).val(),
                countryCode: form.find("#vendor-country-code").val(),
                phone: form.find("#vendor-phone").val(),
                fax: form.find("#vendor-fax").val(),
                email: form.find("#vendor-email").val(),
                url: form.find("#vendor-url").val(),
                __RequestVerificationToken: options.AntiForgeryToken
            };


            $.post(options.AddVendorUrl, vendorInfo, function (data) {
                if (data.success === false) {
                    alert("Unable to save vendor");
                    return;
                }
                var vendor = $("#vendor");

                //removing existing selected options
                vendor.find("option:selected").removeAttr("selected");

                //Get back the id & add into the vendor select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(vendorInfo.name);
                vendor.append(newAddressOption);

                //Clear out the dialog options now that we are done
                $("input", form).val("");
            });

            $(dialog).dialog("close");
        }
    }


    function attachAddressEvents() {
        $("#address-dialog").dialog({
            autoOpen: false,
            height: 610,
            width: 500,
            modal: true,
            buttons: {
                "Create Shipping Address": function () { createAddress(this); },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#address-building").autocomplete({
            source: options.SearchBuildingUrl,
            minLength: 2,
            select: function (event, ui) {
                $("#address-buildingcode").val(ui.item.id);
            },
            change: function (event, ui) {
                if (ui.item == null) {
                    $("#address-buildingcode").val("");
                }
            }
        });

        $("#add-address").click(function (e) {
            e.preventDefault();

            $("#address-dialog").dialog("open");
        });

        function createAddress(dialog) {
            var form = $("#address-form");

            if (form.validate().form() === false) {
                return; //don't create the address if the form is invalid
            }

            var addressInfo = {
                name: form.find("#address-name").val(),
                building: form.find("#address-building").val(),
                buildingCode: form.find("#address-buildingcode").val(),
                room: form.find("#address-room").val(),
                address: form.find("#address-address").val(),
                city: form.find("#address-city").val(),
                state: form.find("#address-state").val(),
                zip: form.find(("#address-zip")).val(),
                phone: form.find(("#address-phone")).val(),
                __RequestVerificationToken: options.AntiForgeryToken
            };

            $.post(options.AddAddressUrl, addressInfo, function (data) {
                var addresses = $("#shipAddress");
                //removing existing selected options
                addresses.find("option:selected").removeAttr("selected");

                //Get back the id & add into the select
                var newAddressOption = $("<option>", { selected: 'selected', value: data.id }).html(addressInfo.name);
                addresses.append(newAddressOption);
            });

            $(dialog).dialog("close");
        }
    }

    function attachShippingWarnings() {
        $("#shippingType").on("change", function () {
            var warning = $("#shippingType > option:selected").data("warning");
            $("#shipping-warning").html(warning);
        });
    }

    function attachCalculatorEvents() {
        $("#calculator-dialog").dialog({
            autoOpen: false,
            height: 500,
            width: 500,
            modal: true
        });

        function enterLineValues(dialog, lineItem) {
            var data = ko.dataFor(lineItem[0]);

            //enter the values into the associated line item
            data.quantity($("#calculator-quantity").val());
            data.price($("#calculator-price").val());

            dialog.dialog("close");
        }

        $("#line-items-body").on("click", ".price-calculator", function (e) {
            e.preventDefault();
            if (purchasing.OrderModel.adjustRouting() === "True") {
                //fill in the values from the line item
                var el = $(this);
                var lineItem = el.parentsUntil("#line-items-body", ".line-item-row");

                $("#calculator-quantity").val(lineItem.find(".quantity").val());
                $("#calculator-price, #calculator-total").val("");

                $("#calculator-dialog").dialog("option",
                    {
                        buttons: {
                            "Accept Values": function () { enterLineValues($(this), lineItem); },
                            "Cancel": function () { $(this).dialog("close"); }
                        }
                    }
                );
                $("#calculator-dialog").dialog("open");
            }
            else {
                alert("You must Enable Modification before changing the Line Items.");
            }
        });

        $("#calculator-quantity, #calculator-total").bind("focus blur keyup", function () {
            var quantity = purchasing.cleanNumber($("#calculator-quantity").val());
            var total = purchasing.cleanNumber($("#calculator-total").val());

            var price = total / quantity;

            $("#calculator-price").val(purchasing.formatNumber(price));
        });
    }

    function attachRestrictedItemsEvents() {
        $("#order-restricted-checkbox").change(function () {
            var fields = $("#order-restricted-fields");
            if (!this.checked) {
                fields.show("highlight", "slow");
            }
            else {
                fields.hide();
            }
        });
    }

    purchasing.validateNumber = function (el) {
        //takes a jquery element & validates that it is a number
        var value = purchasing.cleanNumber(el.val());

        if (isNaN(value) && value !== '') {
            el.addClass(options.invalidNumberClass); //TODO: return true/false and use that value instead of querying for class
        }
        else {
            el.removeClass(options.invalidNumberClass);
        }
    };

    purchasing.formatNumber = function (n) {
        return n.toFixed(3);
    };

    purchasing.displayAmount = function (n) {
        return '$' + (isNaN(n) ? 0.00 : n.toFixed(3));
    };

    purchasing.displayPercent = function (n) {
        return (isNaN(n) ? 0.00 : n.toFixed(3)) + '%';
    };

    purchasing.cleanNumber = function (n) {
        if (!n) {
            return 0; //just return 0 if n is falsy, like undefined, null, empty
        }

        if (isNaN(n) === false) {
            return n; //return the param if it's already a number
        }

        // Assumes string input, removes all commas, dollar signs, percents and spaces      
        var newValue = n.replace(",", "");
        newValue = newValue.replace("$", "");
        newValue = newValue.replace("%", "");
        newValue = newValue.replace(/ /g, '');
        return newValue;
    };

    purchasing.updateNav = function () {
        $(window).sausage("draw");
    };

} (window.purchasing = window.purchasing || {}, jQuery));