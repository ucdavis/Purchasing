﻿///<reference path="jquery-1.6.2.js"/>
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
            attachUserTrackingEvents();
            attachFormSerializationEvents();
            attachAutosaveEvents();
        }
    };

    function attachAutosaveEvents() {
        loadExistingForm();

        $("#order-form").delegate(":input", 'change', function () {
            var delay = 1000; //delay the save one second to avoid gratuitous saving
            clearTimeout(storeOrderFormTimer);
            storeOrderFormTimer = setTimeout(purchasing.storeOrderForm, delay);
        });

        function loadExistingForm() {
            var savedFormExists = localStorage[orderform] !== undefined;

            if (savedFormExists) {
                if (window.confirm("Looks like you were working on a form... do you want to load it back up?")) {
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

    function attachUserTrackingEvents() {
        checkFirstTime();

        $("#clear-user-history").live('click', function (e) {
            e.preventDefault();
            var usertoken = "user-" + $("#userid").html();
            localStorage.removeItem(usertoken);
            window.alert("cleared!  Refresh the page to appear like a first timer");
        });

        function checkFirstTime() {
            var usertoken = "user-" + $("#userid").html();
            var message;

            if (localStorage.getItem(usertoken) === null) {
                message = "it's your first time here";
                localStorage[usertoken] = 1;
            } else {
                message = "you've been here before " + localStorage[usertoken]++ + " times";
            }

            var statusMessage = $("<div id='status-message'>" + message + "<a id='clear-user-history' href='#' style='float:right'>Clear History</a></div>");
            $(".main > header").prepend(statusMessage);
        }
    }

} (window.purchasing = window.purchasing || {}, jQuery));