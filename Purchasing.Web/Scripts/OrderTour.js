//Self-Executing Anonymous Function
(function (tour, $, undefined) {
    var closeButton = { name: "Quit Tour", onclick: function () { tour.complete(); } };
    var hasApprovers = $("#approvers").length !== 0;

    tour.isOriginalRequest = function () {
        return window.location.toString().indexOf("/Request/") !== -1;
    };

    tour.startTour = function (startId) {
        purchasing.preTour(tour.isOriginalRequest());

        guiders.show(startId);
    };

    tour.complete = function () {
        guiders.hideAll();
        purchasing.postTour(tour.isOriginalRequest());
    };

    function resetPage() { //just going to reset the line/split/account info, can update to reset all data if needed
        $("#item-modification-button").trigger("click", { automate: true }); //if we have an edit lock, unlock it
        purchasing.resetFinancials();
        //scrollTo(0, 0); //reset at the top of the page
    }

    function loadTourInfo() {
        loadOverviewTour();
        loadLineItemTour();
        loadLineItemSplitTour();
        loadOrderDetailsTour();
    }

    function loadLineItemTour() {
        var intro = tour.isOriginalRequest() === true
            ? "Don't panic!  Your current form will be saved and restored whenever you choose to quit the tour"
            : "Once this tour is over your page will reload and any unsaved modifications will be lost. If this is not ok, please click close now";

        guiders.createGuider({
            buttons: [{ name: "Close" }, {
                name: "Let's get started",
                onclick: function () {
                    resetPage(); //We have chosen to enter the tour, so reset the page
                    configureLineItemTour();
                    guiders.next();
                }
            }],
            description: intro + "<br/><br/>Here we're going to look at how to enter line items in depth",
            id: "lineitem-intro",
            next: "lineitem-quantity",
            position: 0,
            overlay: true,
            title: "Line Item Tour"
        });
    }

    function configureLineItemTour() {
        guiders.createGuider({
            attachTo: "input[name='items[0].quantity']",
            buttons: [closeButton, { name: "Next"}],
            description: "Today we are going to buy three dozen apples.<br/><br/>First, enter a quantity for the first line item.  This can be any number, including fractional numbers."
                + "<br/><br/>Note how the 'description' and 'unit$' fields turn are <span class='line-item-warning'>red</span> indicating that they need to be filled out for this line item to be valid",
            onShow: function (guider) {
                console.log($(guider.attachTo).val());
                purchasing.OrderModel.items()[0].quantity(3);
                console.log($(guider.attachTo).val());
            },
            id: "lineitem-quantity",
            next: "lineitem-unit",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Quantity"
        });

        guiders.createGuider({
            attachTo: "select[name='items[0].units']",
            buttons: [closeButton, { name: "Next"}],
            description: "Select a unit: the default is 'Each'. In our case we are going to select 'Dozen'",
            onShow: function (guider) {
                $(guider.attachTo).val("DZ");
            },
            id: "lineitem-unit",
            next: "lineitem-description",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Unit"
        });

        guiders.createGuider({
            attachTo: "input[name='items[0].description']",
            buttons: [closeButton, { name: "Next"}],
            description: "Now enter a description of what you want to buy, and optionally enter an associated catalog number on the previous field.<br/> Note! You may enter a model number here.",
            onShow: function (guider) {
                $(guider.attachTo).val("Fresh Organic Fuji Apples").change();
            },
            id: "lineitem-description",
            next: "lineitem-price",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Description"
        });

        guiders.createGuider({
            attachTo: "input[name='items[0].price']",
            buttons: [closeButton, { name: "Next"}],
            description: "Now enter the unit price (this will be multiplied by quantity to determine the line total).  In our example, each dozen is $5, so we'll enter 5.",
            onShow: function (guider) {
                $(guider.attachTo).val(5).change();
            },
            id: "lineitem-price",
            next: "lineitem-total",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Price"
        });

        guiders.createGuider({
            attachTo: ".line-total:first",
            buttons: [closeButton, { name: "Next"}],
            description: "At this point you have filled out the required information for a valid line item.  The total for this line has been calculated automatically, along with the order totals."
                + "<br/><br/>Now we'll go in depth to check out some advanced features.",
            onShow: function (guider) {
                $(guider.attachTo).val(5).change();
            },
            id: "lineitem-total",
            next: "lineitem-calculatorbutton",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour"
        });

        guiders.createGuider({
            attachTo: ".line-total:first",
            buttons: [closeButton, { name: "Next"}],
            description: "If you do not know the unit price for a line item, the price calculator can help you fill out the line item information given only the quantity and total price"
                + "<a class=\"ui-icon ui-icon-calculator\"></a>",
            id: "lineitem-calculatorbutton",
            next: "lineitem-calculatordialog",
            position: 1,
            overlay: true,
            highlight: '.price-calculator:first',
            title: "Price Calculator"
        });

        guiders.createGuider({
            attachTo: "#calculator-quantity",
            buttons: [closeButton, { name: "Next"}],
            description: "Enter the quantity and total price for this order, and the unit price will be calculated automatically"
            + "<br/><br/>Click 'Accept Values' and the quantity & unit $ fields will be filled in for you",
            onShow: function () {
                $(".price-calculator:first").click();
                $("#calculator-quantity").val(3);
                $("#calculator-total").val(15).blur();
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
            },
            id: "lineitem-calculatordialog",
            next: "lineitem-morebutton",
            position: 3,
            overlay: true,
            highlight: '.ui-dialog',
            title: "Price Calculator"
        });

        guiders.createGuider({
            attachTo: ".toggle-line-item-details:first",
            buttons: [closeButton, { name: "Next"}],
            description: "You can include additional optional information about a line item by clicking the 'More' icon"
            + "<a class=\"ui-icon ui-icon-comment\"></a>",
            onHide: function (guider) {
                $(guider.attachTo).click();
            },
            id: "lineitem-morebutton",
            next: "lineitem-commodity",
            position: 1,
            overlay: true,
            highlight: ".toggle-line-item-details:first",
            title: "More Information"
        });

        guiders.createGuider({
            attachTo: "input[name='items[0].commodityCode']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally, you may enter a Commodity code. A lookup against KFS Commodity Codes and Names will be performed, and you can select the correct match if it appears.  You will be notified if no match is found. If you do not know the commodity code, leave it blank and it will be completed at the purchase stage.",
            onShow: function (guider) {
                $(guider.attachTo).val("13165").change();
            },
            id: "lineitem-commodity",
            next: "lineitem-url",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Commodity"
        });

        guiders.createGuider({
            attachTo: "input[name='items[0].url']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally, you may enter a URL for your line item.",
            onShow: function (guider) {
                $(guider.attachTo).val("http://www.store.com/chairs/1").change();
            },
            id: "lineitem-url",
            next: "lineitem-notes",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Item URL"
        });

        guiders.createGuider({
            attachTo: "textarea[name='items[0].notes']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally, you may enter notes for your line item. You can make the text area larger or smaller by dragging on the right corner. You may enter multiple lines of text with the enter key. You may press the tab key to move to the next field.",
            onShow: function (guider) {
                $(guider.attachTo).val("Please select the blue chair.").change();
            },
            id: "lineitem-notes",
            next: "lineitem-finish",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Item Notes"
        });

        guiders.createGuider({
            buttons: [{ name: "Thanks for the tour, I'll take it from here!", onclick: function () { tour.complete(); } }],
            description: "That's it!  Pretty easy huh?",
            id: "lineitem-finish",
            overlay: true,
            position: 0,
            title: "Line Item Tour"
        });
    }

    function loadOrderDetailsTour() {
        var intro = tour.isOriginalRequest() === true
            ? "Don't panic!  Your current form will be saved and restored whenever you choose to quit the tour"
            : "Once this tour is over your page will reload and any unsaved modifications will be lost. If this is not ok, please click close now";

        //#1
        guiders.createGuider({
            buttons: [{ name: "Close" }, {
                name: "Let's get started",
                onclick: function () {
                    resetPage(); //We have chosen to enter the tour, so reset the page
                    guiders.next();
                }
            }],
            description: intro + "<br/><br/>Here we're going to look at how to enter the order details. To submit an order you must enter either an account, an account manager, or split the order between 2 or more accounts.<br/><br/><strong>NOTE!</strong> you may only make one of those choices. If you select and account, then pick an approver and account manager, the account choice will be cleared.",
            id: "orderDetails-intro",
            next: "orderDetails-account",
            position: 0,
            overlay: true,
            highlight: "#order-account-section",
            title: "Order Details Tour"
        });

        //#2
        guiders.createGuider({
            attachTo: "#Account",
            buttons: [closeButton, { name: "Next"}],
            description: "If you know the account that this purchase should use, you may pick it from the list of accounts in the workgroup.<br/>If there are any related sub accounts, they will be available from the drop down list once the account is selected.",
            onShow: function (guider) {
                var lineItems = purchasing.OrderModel.items();
                lineItems[0].quantity(12);
                lineItems[0].desc("lawn chairs");
                lineItems[0].price(20.25);
                lineItems[1].quantity(3);
                lineItems[1].unit("DZ");
                lineItems[1].desc("apples");
                lineItems[1].price(5);
                $(guider.attachTo).val($(guider.attachTo + " option:nth-child(2)").val());
            },
            id: "orderDetails-account",
            next: "orderDetails-project",
            position: 1,
            overlay: true,
            highlight: '#order-account-section',
            title: "Order Details Tour: Account"
        });

        //#3
        guiders.createGuider({
            attachTo: "input[name='Project']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally enter a project.",
            onShow: function (guider) {
                $(guider.attachTo).val("Proj");
            },
            id: "orderDetails-project",
            next: "orderDetails-searchAccount",
            position: 1,
            overlay: true,
            highlight: '#order-account-section',
            title: "Order Details Tour: Account Project"
        });

        //#4
        guiders.createGuider({
            attachTo: ".search-account",
            buttons: [closeButton, { name: "Next"}],
            description: "If the account you need is not in the drop down list for the workgroup, you may search for it by clicking here.<br/><br/><strong>NOTE!</strong> If you search for and pick an account not in the drop down list it will change the approval routing. The account manager for that account in KFS will be added as an account manager approver for the order and the workgroup approver may be bypassed.",
            id: "orderDetails-searchAccount",
            next: "orderDetails-searchAccount2",
            position: 2,
            offset: { top: -36, left: null },
            overlay: true,
            highlight: '#order-account-section',
            title: "Order Details Tour: Search for a KFS Account"
        });


        //#5
        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#accounts-search-dialog-searchbox",
            onShow: function () {
                $(".search-account:first").click();  //TODO: either turn off animation or wait until complete
                $("#accounts-search-dialog-searchbox").val("3-3136");
            },

            description: "Enter an account or account name.",
            id: "orderDetails-searchAccount2",
            next: "orderDetails-searchAccount3",
            overlay: true,
            highlight: ".ui-dialog",
            position: 1,
            title: "Order Details Tour: Search for a KFS Account"
        });

        //#6
        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#accounts-search-dialog-searchbox-btn",
            onShow: function () {
                $("#accounts-search-dialog-searchbox-btn").click();
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
                if (hasApprovers) { purchasing.OrderModel.setOrderApproverRouting(); }
            },
            description: "Click on the search icon. Once the results are found, click on the Select button, search again or cancel.",
            id: "orderDetails-searchAccount3",
            next: hasApprovers ? "orderDetails-accountManager" : "orderDetails-split",
            overlay: true,
            highlight: ".ui-dialog",
            position: 2,
            offset: { top: -35, left: null },
            title: "Order Details Tour: Search for a KFS Account"
        });

        if (hasApprovers) {
            //#7
            guiders.createGuider({
                attachTo: "#accountmanagers",
                buttons: [closeButton, { name: "Next"}],
                description: "If you are unsure of the account to use, you must select an account manager from the drop down list for a valid order. When it gets to the account manager stage, they will choose the correct account to use.",
                onShow: function (guider) {
                    $(guider.attachTo).val($(guider.attachTo + " option:nth-child(2)").val());
                },
                id: "orderDetails-accountManager",
                next: "orderDetails-approver",
                position: 1,
                overlay: true,
                highlight: '#order-account-section',
                title: "Order Details Tour: Account Manager"
            });

            //#8
            guiders.createGuider({
                attachTo: "#approvers",
                buttons: [closeButton, { name: "Next"}],
                description: "If you choose to select an account manager, you may optionally select the approver for this order. If you do not choose an approver, this order will be available to all the approvers in the workgroup.",
                id: "orderDetails-approver",
                next: "orderDetails-split",
                position: 1,
                overlay: true,
                highlight: '#order-account-section',
                title: "Order Details Tour: Approver"
            });
        }

        //#9
        guiders.createGuider({
            attachTo: "#split",
            buttons: [closeButton, {
                name: "Next",
                onclick: function () {
                    var model = purchasing.OrderModel; //split by order and add 2 splits by default
                    model.splitType("Order");
                    model.splits.push(new purchasing.OrderSplit(0, model));
                    model.splits.push(new purchasing.OrderSplit(1, model));
                    configureSplitAccounts();
                    guiders.next();
                }
            }],
            description: "If you know the accounts to use for this order and want to split the total order between 2 or more accounts you would click on Split Order Request",
            id: "orderDetails-split",
            next: "orderDetails-account1",
            position: 1,
            offset: { top: 0, left: 40 },
            overlay: true,
            highlight: '#order-account-section',
            title: "Order Details Tour: Split Between Accounts"
        });

    }

    function configureSplitAccounts() {
        //#10
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "Pick an account from the list of accounts in the workgroup.<br/>If there are any related sub accounts, they will be available from the drop down list once the account is selected.",
            onShow: function (guider) {
                $(guider.attachTo).val($(guider.attachTo + " option:nth-child(2)").val());
            },
            id: "orderDetails-account1",
            next: "orderDetails-project1",
            position: 1,
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Split Between Accounts"
        });

        //#11
        guiders.createGuider({
            attachTo: "input[name='splits[0].Project']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally enter a project.",
            id: "orderDetails-project1",
            next: "orderDetails-searchAccount3",
            onShow: function (guider) {
                $(guider.attachTo).val("Proj");
            },
            position: 1,
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Split Between Accounts"
        });

        //#12
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "If the account you need is not in the drop down list for the workgroup, you may search for it by clicking here.",
            id: "orderDetails-searchAccount3",
            next: "orderDetails-percent",
            position: 2,
            offset: { top: -25, left: 20 },
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Search for a KFS Account"
        });

        //#13
        guiders.createGuider({
            attachTo: "input[name='splits[0].percent']",
            buttons: [closeButton, { name: "Next"}],
            description: "You must enter either an amount or a percentage. When you enter one, the other is updated.",
            id: "orderDetails-percent",
            next: "orderDetails-addSplit",
            onShow: function (guider) {
                $(guider.attachTo).val("60").change();
            },
            position: 1,
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Percent"
        });

        //#14
        guiders.createGuider({
            attachTo: "#add-order-split",
            buttons: [closeButton, { name: "Next"}],
            description: "If you need to split between more accounts, click on Add Split.<br/>If you have added an account by mistake, all you need to do is choose <strong>--Account--</strong> from the drop down, and clear out the rest of the fields for that account line. When you save the order that line will be ignored.",
            id: "orderDetails-addSplit",
            next: "orderDetails-percent2",
            onShow: function () {
                purchasing.OrderModel.addOrderSplit();
            },
            position: 2,
            offset: { top: -32, left: null },
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Add Split"
        });

        //#15
        guiders.createGuider({
            attachTo: "#order-split-account-total",
            buttons: [closeButton, { name: "Next"}],
            description: "No we have entered 2 more percentages. That takes care of all the unaccounted amounts.",
            id: "orderDetails-percent2",
            next: "orderDetails-finish",
            onShow: function () {
                $("select[name='splits[1].Account']").val($("select[name='splits[1].Account'] option:nth-child(2)").val());
                $("select[name='splits[2].Account']").val($("select[name='splits[2].Account'] option:nth-child(2)").val());
                $("input[name='splits[1].percent']").val("25").change();
                $("input[name='splits[2].percent']").val("15").change();
            },
            position: 1,
            overlay: true,
            highlight: '#order-split-section',
            title: "Order Details Tour: Totals"
        });

        //#16 and End
        guiders.createGuider({
            buttons: [{ name: "Thanks for the tour, I'll take it from here!", onclick: function () { tour.complete(); } }],
            description: "That's it!  Pretty easy huh?",
            id: "orderDetails-finish",
            overlay: true,
            position: 0,
            title: "Line Item Tour"
        });

    }

    function loadLineItemSplitTour() {
        var intro = tour.isOriginalRequest() === true
            ? "Don't panic!  Your current form will be saved and restored whenever you choose to quit the tour"
            : "Once this tour is over your page will reload and any unsaved modifications will be lost. If this is not ok, please click close now";

        //#1
        guiders.createGuider({
            buttons: [{ name: "Close" }, {
                name: "Let's get started",
                onclick: function () {
                    resetPage(); //We have chosen to enter the tour, so reset the page
                    guiders.next();
                    resetPage();
                }
            }],
            description: intro + "<br/><br/>Here we're going to look at how to enter line items with account splits in depth. We will assume that you are familiar with entering the line item without splits.",
            id: "lineitemsplit-intro",
            next: "lineitemsplit-intro2",
            position: 0,
            overlay: true,
            title: "Line Item Split Tour"
        });

        //#2
        guiders.createGuider({
            attachTo: ".lineitemsplit",
            buttons: [closeButton, {
                name: "Next",
                onclick: function () {
                    var model = purchasing.OrderModel;
                    $.each(model.items(), function (index, item) { //add one split to each line
                        item.splits.push(new purchasing.LineSplit(index, item));
                    });
                    model.splitType("Line");
                    configureLineItemSplitTour();
                    guiders.next();
                }
            }],
            description: "To split the line items by account you would click on this link.",
            id: "lineitemsplit-intro2",
            next: "lineitemsplit-item1",
            position: 2,
            offset: { top: -15, left: null },
            overlay: true,
            highlight: '.lineitemsplit',
            title: "Line Item Split Tour"
        });

    }

    function configureLineItemSplitTour() {
        //#3
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "Using a line item split for your order will require knowing all account information with which to split your line items.",
            onShow: function (guider) {
                var lineItems = purchasing.OrderModel.items();
                lineItems[0].quantity(12);
                lineItems[0].desc("lawn chairs");
                lineItems[0].price(20.25);
                lineItems[1].quantity(3);
                lineItems[1].unit("DZ");
                lineItems[1].desc("apples");
                lineItems[1].price(5);
            },
            id: "lineitemsplit-item1",
            next: "lineitemsplit-unaccounted1",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Note"
        });

        //#4
        guiders.createGuider({
            attachTo: ".amount-difference:first",
            buttons: [closeButton, { name: "Next"}],
            description: "The unaccounted field shows total unaccounted for the line amount this includes the estimated tax for the line item that has not been assigned to an account.",
            id: "lineitemsplit-unaccounted1",
            next: "lineitemsplit-account1",
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Line Item Unaccounted"
        });

        //#5
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "Pick an account from the list of accounts in the workgroup.<br/>If there are any related sub accounts, they will be available from the drop down list once the account is selected.",
            id: "lineitemsplit-account1",
            next: "lineitemsplit-project1",
            onShow: function (guider) {
                $(guider.attachTo).val($(guider.attachTo + " option:nth-child(2)").val());
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Select Account"
        });

        //#6
        guiders.createGuider({
            attachTo: "input[name='splits[0].Project']",
            buttons: [closeButton, { name: "Next"}],
            description: "Optionally enter a project.",
            id: "lineitemsplit-project1",
            next: "lineitemsplit-searchAccount1",
            onShow: function (guider) {
                $(guider.attachTo).val("Proj");
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Select Account"
        });

        //#7
        guiders.createGuider({
            attachTo: ".account-container:first",
            buttons: [closeButton, { name: "Next"}],
            description: "If the account you need is not in the drop down list for the workgroup, you may search for it by clicking here.",
            id: "lineitemsplit-searchAccount1",
            next: "lineitemsplit-amount1",
            position: 2,
            overlay: true,
            highlight: '#line-items-section',
            offset: { top: -26, left: null },
            title: "Line Item Tour: Search for a KFS Account"
        });

        //#8
        guiders.createGuider({
            attachTo: "input[name='splits[0].amount']",
            buttons: [closeButton, { name: "Next"}],
            description: "You must enter either an amount or a percentage. When you enter one, the other is updated. So, I've entered $50.50, you will notice the percentage is updated.",
            id: "lineitemsplit-amount1",
            next: "lineitemsplit-percent1",
            onShow: function (guider) {
                $(guider.attachTo).val(50.50).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Enter Amount"
        });

        //#9
        guiders.createGuider({
            attachTo: "input[name='splits[0].percent']",
            buttons: [closeButton, { name: "Next"}],
            description: "You must enter either an amount or a percentage. When you enter one, the other is updated. So, I've entered 100%, you will notice the amount is updated and the Unaccounted no longer has a value.",
            id: "lineitemsplit-percent1",
            next: "lineitemsplit-start2",
            onShow: function (guider) {
                $(guider.attachTo).val(100).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Enter Percentage"
        });

        //#10
        guiders.createGuider({
            attachTo: ".add-line-item-split:first",
            buttons: [closeButton, { name: "Next"}],
            description: "To split an item between two or more accounts, click Add Split to add another account to this line item.",
            id: "lineitemsplit-start2",
            next: "lineitemsplit-addsplit2",
            onShow: function () {
                purchasing.OrderModel.items()[0].addSplit(); //Add another line split to the first item
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Add Split"
        });

        //#11
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "You would select or find your accounts as described previously.",
            id: "lineitemsplit-addsplit2",
            next: "lineitemsplit-percent2a",
            onShow: function (guider) {
                $(guider.attachTo).val($(guider.attachTo + " option:nth-child(2)").val());
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Select Account"
        });

        //#12
        guiders.createGuider({
            attachTo: "input[name='splits[0].percent']",
            buttons: [closeButton, { name: "Next"}],
            description: "For the first account we will enter 50%.",
            id: "lineitemsplit-percent2a",
            next: "lineitemsplit-percent2a",
            onShow: function (guider) {
                $(guider.attachTo).val(50).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Select Percent"
        });

        //#13
        guiders.createGuider({
            attachTo: ".add-line-item-split-total:first",
            buttons: [closeButton, { name: "Next"}],
            description: "You will notice the following have changed:<br/><strong>Split Total:</strong> The total amount split so far.<br/><br/><strong>Unaccounted:</strong> The amount that has not been assigned to an account yet (or if too much has been assigned).<br/><br/><strong>Line Total:</strong> The total for this line item including estimated tax.",
            id: "lineitemsplit-percent2a",
            next: "lineitemsplit-percent2b",
            onShow: function (guider) {
                $("input[name='splits[0].percent']").val(50).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Updated Values"
        });

        //#14
        guiders.createGuider({
            attachTo: "input[name='splits[0].percent']",
            buttons: [closeButton, { name: "Next"}],
            description: "For the second account we will enter 49%.<br/><br/>If you look to the right you will see that the unaccounted amount is 2.606. Sometimes when you use the percentages to determine the amount, things don't always add up to 100% because of rounding.<br/>",
            id: "lineitemsplit-percent2b",
            next: "lineitemsplit-amount2",
            onShow: function (guider) {
                $("input[name='splits[0].percent']").val(50).change();
                $("input[name='splits[3].percent']").val(49).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            offset: { top: 50, left: null },
            title: "Line Item Tour: Select Percent"
        });

        //#15
        guiders.createGuider({
            attachTo: "input[name='splits[0].amount']",
            buttons: [closeButton, { name: "Next"}],
            description: "To fix this we will add the $2.606 to the amount field of the first account. To the right you will now notice that the unaccounted amount is gone and the percentage is changed (51%) in this case.",
            id: "lineitemsplit-amount2",
            next: "lineitemsplit-extraAccountSplit",
            onShow: function () {
                $("input[name='splits[0].amount']").val(132.915).change();
                $("input[name='splits[3].percent']").val(49).change();
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            title: "Line Item Tour: Select Amount"
        });

        //#16
        guiders.createGuider({
            attachTo: "select[name='splits[0].Account']",
            buttons: [closeButton, { name: "Next"}],
            description: "If you have added an account by mistake, all you need to do is choose <strong>--Account--</strong> from the drop down, and clear out the rest of the fields for that account line. When you save the order that line will be ignored.",
            id: "lineitemsplit-extraAccountSplit",
            next: "lineitemsplit-finish",
            onShow: function () {
                purchasing.OrderModel.items()[0].addSplit(); //Add another line split to the first item
                $("input[name='splits[0].amount']").val(132.915).change();
                $("input[name='splits[3].percent']").val('').change(); //clear out the value
            },
            position: 1,
            overlay: true,
            highlight: '#line-items-section',
            offset: { top: 100, left: null },
            title: "Line Item Tour: To Remove Account"
        });

        //#17
        guiders.createGuider({
            attachTo: ".lineitemsplit",
            buttons: [closeButton, { name: "Next"}],
            description: "To cancel the split line items by account you would click on this link.",
            id: "lineitemsplit-finish",
            next: "lineitemsplit-finish2",
            position: 2,
            offset: { top: -15, left: null },
            overlay: true,
            overlay: true,
            highlight: '#line-items-section',
            highlight: '.lineitemsplit',
            title: "Line Item Split Tour"
        });

        //#18 and End
        guiders.createGuider({
            buttons: [{ name: "Thanks for the tour, I'll take it from here!", onclick: function () { tour.complete(); } }],
            description: "That's it!  Pretty easy huh?",
            id: "lineitemsplit-finish2",
            overlay: true,
            position: 0,
            title: "Line Item Tour"
        });
    }

    function loadOverviewTour() {
        var intro = tour.isOriginalRequest() === true
            ? "Don't panic!  Your current form will be saved and restored whenever you choose to quit the tour"
            : "Once this tour is over your page will reload and any unsaved modifications will be lost. If this is not ok, please click close now";

        guiders.createGuider({
            buttons: [{ name: "Close" }, {
                name: "Let's get started",
                onclick: function () {
                    resetPage(); //We have chosen to enter the tour, so reset the page
                    configureOverviewTour(); //Process the rest of the tour
                    guiders.next();
                }
            }],
            description: intro + "<br/><br/>Prepare to be taken to a dreamworld of magic",
            id: "intro",
            next: "justification",
            //next: "lineitemsoverview",
            position: 0,
            overlay: true,
            title: "Guided Tour"
        });
    }

    function configureOverviewTour() {
        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "textarea[name=justification]",
            description: "Note the required <span class='required'>*</span> for required fields.",
            id: "justification",
            next: "vendor",
            onShow: function (guider) {
                $(guider.attachTo).val("Enter some text");
            },
            position: 11,
            highlight: ".orders-nav",
            title: "Fill Out Fields"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#vendor_chzn",
            description: "Select a vendor or leave the vendor 'unspecified'. If you do not see your desired vendor in the drop down list, you can add a new vendor or lookup an existing vendor from the campus financial data source",
            id: "vendor",
            next: "addvendorbutton",
            overlay: true,
            highlight: "#vendor-section",
            position: 1,
            title: "Vendor Selection Options"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#add-vendor",
            description: "Let's look at adding a new vendor first <br/><br/>Clicking on the 'Add New Vendor' button will bring up the Add Vendor dialog",
            id: "addvendorbutton",
            next: "addvendor",
            overlay: true,
            highlight: "#add-vendor",
            position: 1,
            title: "Add A New Vendor"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#vendor-name",
            onShow: function (guider) {
                $("#add-vendor").click(); //TODO: either turn off animation or wait until complete
                $("#vendor-name").val("New Vendor Name");
                $("#vendor-address").val("One Infinite Loop");
                $("#vendor-city").val("Coopertino");
                $("#vendor-state").val("CA");
                $("#vendor-zip").val("90210");
                $("#vendor-phone").val("555-5555");
                $("#vendor-email").val("contact@buymore.com");
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
            },
            description: "Fill out the required<span class='required'>*</span> fields and then click the 'Create Vendor' button at the bottom of the dialog. "
            + "This will create a new vendor and automatically select it for you in the vendor list"
            + "<br/><br/>Click cancel if you want to close the dialog without saving your changes",
            id: "addvendor",
            next: "searchvendorbutton",
            overlay: true,
            highlight: ".ui-dialog",
            position: 3,
            title: "Add A New Vendor"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#search-vendor",
            description: "Now let's look at searching for an existing vendor (from KFS)<br/><br/>Clicking on the 'Search Vendor' button will bring up the Search Vendor dialog",
            id: "searchvendorbutton",
            next: "searchvendor",
            overlay: true,
            highlight: "#search-vendor",
            position: 1,
            title: "Vendor Search"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#search-vendor",
            description: "Watch below to see an example of searching for a vendor.  Click continue to move on with the tour<br/><br/><img src='" + purchasing._getOption("Guider").KfsVendorSearchUrl + "' alt='Looking up a KFS vendor' />",
            id: "searchvendor",
            next: "address",
            position: 0,
            width: 520,
            title: "Vendor Search"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#shipAddress_chzn",
            description: "Select an address where the order should be shipped. If you do not see your desired address in the drop down list, you can add a new address at any time by clicking the 'Add New Shipping Address' button",
            id: "address",
            next: "addaddressbutton",
            overlay: true,
            highlight: "#shipping-address-section",
            position: 1,
            title: "Address Selection Options"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#add-address",
            description: "Let's quickly look at adding a new address <br/><br/>Clicking on the 'Add New Shipping Address' button will bring up the Add Shipping Address dialog",
            id: "addaddressbutton",
            next: "addaddress",
            overlay: true,
            highlight: "#add-address",
            position: 1,
            title: "Add A New Address"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#address-name",
            onShow: function (guider) {
                $("#add-address").click(); //TODO: either turn off animation or wait until complete
                $("#address-name").val("New Address Name");
                $("#address-building").val("Mrak Hall");
                $("#address-room").val("38");
                $("#address-address").val("One Shields Ave");
                $("#address-city").val("Davis");
                $("#address-state").val("CA");
                $("#address-zip").val("95616");
                $("#address-phone").val("754-5555");
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
            },
            description: "Fill out the required<span class='required'>*</span> fields and then click the 'Create Address' button at the bottom of the dialog. "
            + "This will create a new address and automatically select it for you in the address list"
            + "<br/><br/>Click cancel if you want to close the dialog without saving your changes",
            id: "addaddress",
            next: "lineitemsoverview",
            overlay: true,
            highlight: ".ui-dialog",
            position: 3,
            title: "Add A New Address"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "body",
            description: "Now let's look at a quick overview of adding line items <br/><br/>"
            + "For in depth information on different line item features, look for the "
            + "<span class=\"ui-icon ui-icon-help\"></span>"
            + "icons which will take you on an in depth guided tour of relevant features at any time",
            id: "lineitemsoverview",
            next: "lineitems",
            overlay: true,
            position: 0,
            title: "Line Items Overview"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#line-items",
            description: "Fill in at least the quantity, description, and unit price of each line item in your order.  All price calculations will update automatically as you type."
            + "<br/><br/>You can add new line item rows as needed by clicking 'Add New Item'",
            onShow: function () {
                var lineItems = purchasing.OrderModel.items();
                lineItems[0].quantity(12);
                lineItems[0].desc("lawn chairs");
                lineItems[0].price(20.25);
                lineItems[1].quantity(3);
                lineItems[1].unit("DZ");
                lineItems[1].desc("apples");
                lineItems[1].price(5);
            },
            id: "lineitems",
            next: "orderdetailsoverview",
            overlay: true,
            highlight: "#line-items-section",
            position: 1,
            title: "Line Items Overview"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#line-items",
            description: "When you are finished adding line items, you need to select how to account for the costs. "
            + "We'll start by looking at <strong>Account Selection</strong>"
            + (hasApprovers ? " and <strong>Approver/Manager Selection</strong>, the simplest choices" : " ")
            + "<br/><br/>For advanced features like splitting orders across multiple accounts, or even splitting each line item across multiple accounts, please click on the relevant"
            + "<span class=\"ui-icon ui-icon-help\"></span>"
            + "icons to get a targeted tour at any time",
            id: "orderdetailsoverview",
            next: "orderdetails-accountselection",
            overlay: true,
            position: 0,
            title: "Order Details Overview"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#Account",
            description: "If you know the account this order should be charged against, select it from the account drop down.  If there are any subaccounts related to the selected account, they will appear in the '--Sub Account--' drop down."
            + "<br/><br/>If you do not see the desired account in the list, you can use the <img src=\"/Images/details.png\"> icon to lookup any account",
            onShow: function () {
                var model = purchasing.OrderModel;
                if (model.accounts().length > 1) {
                    model.account(model.accounts()[1].id); //just pick the first non-default choice (by id)
                }
            },
            onHide: function () {
                if (hasApprovers) {
                    purchasing.OrderModel.setOrderApproverRouting();
                }
            },
            id: "orderdetails-accountselection",
            next: hasApprovers ? "orderdetails-approverselection" : "orderformcompletion",
            overlay: true,
            highlight: '#order-account-section',
            position: 1,
            title: "Account Selection"
        });

        if (hasApprovers) {
            guiders.createGuider({
                buttons: [closeButton, { name: "Next"}],
                attachTo: "#approvers",
                description: "Alternately, you can select a Purchasing Agent to route this order to. An approver can also be specified."
                + "<br/><br/>This option would be useful if you do not know the account this order should be charged to",
                onShow: function () {
                    var firstChoice = $("#accountmanagers :nth-child(2)").val();
                    $("#accountmanagers").val(firstChoice);
                },
                id: "orderdetails-approverselection",
                next: "orderformcompletion",
                overlay: true,
                highlight: '#order-account-section',
                position: 1,
                title: "Purchasing Agent Selection"
            });
        }

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#order-preferences-section",
            description: "Fill out the rest of the form as desired, including uploading associated files",
            id: "orderformcompletion",
            next: "submit",
            position: 6,
            title: "Complete The Order Form"
        });

        var submitType = $("input.order-submit").val();
        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "input.order-submit",
            description: "When you are ready, click '" + submitType + "' to " + submitType.toString().toLowerCase() + " the order!",
            id: "submit",
            next: "coda",
            overlay: true,
            highlight: 'input.order-submit',
            position: 12,
            title: "Submit"
        });

        guiders.createGuider({
            buttons: [{ name: "Thanks for the tour, I'll take it from here!", onclick: function () { tour.complete(); } }],
            description: "That's it!  Pretty easy huh?"
            + "<br/><br/>If you want to learn more about a specific topic, click any of the "
            + "<span class=\"ui-icon ui-icon-help\"></span> icons to get an in depth tour on the related feature. ",
            id: "coda",
            overlay: true,
            position: 0,
            title: "Coda"
        });
    }

    loadTourInfo(); //load tour info when this object is loaded, and wait for start to be called

} (window.tour = window.tour || {}, jQuery));