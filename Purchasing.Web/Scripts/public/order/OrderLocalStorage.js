///<reference path="Order.js"/>
//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    "use strict";
    var orderform = "orderform";
    var storeOrderFormTimer;

    //Public Method
    purchasing.initLocalStorage = function () {
        if (window.Modernizr.localstorage) {
            attachFormSerializationEvents();
            attachAutosaveEvents();
            attachTourEvents();
        }
    };

    function attachAutosaveEvents() {
        loadExistingForm();

        $("#order-form").delegate(":input", 'change', function () {
            var delay = 1000; //delay the save one second to avoid gratuitous saving
            clearTimeout(storeOrderFormTimer);
            storeOrderFormTimer = setTimeout(purchasing.storeOrderForm, delay);
        });

        $("#order-form").submit(function () {
            localStorage.removeItem(orderform); //On submit, clear the saved temp data
        });

        function loadExistingForm() {
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
            localStorage.setItem('orderform', $("#order-form").serialize());

            localStorage.setItem('orderform-splittype', $("#splitType").val());

            //Store line item count and split count
            localStorage["orderform-lineitems"] = $(".line-item-row").length;
            localStorage["orderform-linesplits"] = $(".sub-line-item-split-row").length;
            localStorage["orderform-ordersplits"] = $(".order-split-line").length;

            console.log("Stored", localStorage[orderform]);
        };

        purchasing.loadOrderForm = function () {
            //Load more line items if we have > 3
            for (var j = 0; j < localStorage["orderform-lineitems"] - 3; j++) {
                $("#add-line-item").trigger('createline');
            }

            //Now we need to create the correct split, if needed
            var splitType = localStorage['orderform-splittype'];

            if (splitType === 'Order') {
                $("#split-order").trigger('click', { automate: true });

                //Load more order splits if we have > 3
                for (var i = 0; i < localStorage["orderform-ordersplits"] - 3; i++) {
                    $("#add-order-split").trigger('createsplit');
                }
            } else if (splitType === 'Line') {
                $("#split-by-line").trigger('click', { automate: true });

                //First add all new line splits to the first line item, and then we'll move them to the proper locations later
                //We start out with 3 line splits (one for each of the original 3 line items)
                var firstLineItemSplit = $(".sub-line-item-split").first();
                var firstSplitButton = $(".add-line-item-split", firstLineItemSplit);
                for (var k = 0; k < localStorage["orderform-linesplits"] - 3; k++) {
                    firstSplitButton.trigger('createsplit');
                }
            }

            var data = localStorage[orderform];

            $("#order-form")
                .unserializeForm(
                    data,
                    {
                        'callback': bindFormValues,
                        'override-values': true
                    });

            //Go through the split rows and move them where they should be
            $('.sub-line-item-split-row').each(function () {
                var el = $(this);
                var splitLineIndex = el.find(".line-item-split-item-id").val();
                var splitTableAtIndex = $(".sub-line-item-split-body[data-line-item-index=" + splitLineIndex + "]");

                splitTableAtIndex.append(el);
            });

            //Now recalculate line items & splits
            $(".quantity").change();
            $(".order-split-account-amount, .line-item-split-account-amount").change();
        };

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
            } else if (el.hasClass("account-subaccount") || el.hasClass("account-number")) {
                if (!el.has("option[value=" + val + "]").length) { //if we don't already have the proper option, add it in
                    var newOption = $("<option>").text(val).val(val);
                    el.append(newOption);
                    el.removeAttr("disabled");
                }
                el.val(val);
                return true;
            }
            return false;
        }
    }

    purchasing.takeTour = function () {
        window.Modernizr.load({ //TODO: update the asset paths to be part of passed options
            load: ['../../Css/guider.css', '../../Scripts/guider.js', '../../Scripts/OrderTour.js'],
            complete: function () {
                //TODO: save their order state and restore when finished
                window.tour.startOverview();
            }
        });
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

            localStorage[userTourToken()] = true;
            $(".tour-message").remove();
            purchasing.takeTour();
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

} (window.purchasing = window.purchasing || {}, jQuery));