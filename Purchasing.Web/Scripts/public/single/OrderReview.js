//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = {};

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing.init = function () {
        $("a[title]").qtip();
        $("input[title]").qtip();
        attachNoteEvents(); //Anyone can add notes and files
        attachPeepsEvents();
        attachFileEvents();

        if (options.CanEdit) {
            attachApprovalEvents();
            attachSubmitEvents();

            if (options.CanComplete) {
                attachCompletionEvents();
            }
        }

        if (options.CanCancel) {
            attachCancelEvents();
        }

        if (options.CanCancelCompletedOrder) {
            attachCancelCompletedOrderEvents();
        }

        if (options.IsKfsOrder) {
            purchasing.loadKfsData();
        }

        if (options.IsComplete) {
            attachReferenceNumberEvents();
            attachPoNumberEvents();
        }
        attachNav();
    };

    function attachNav() {
        $(window).sausage({ page: '.showInNav' });
        $('.orders-nav').addClass('orders-nav-review'); 
        $('.orders-nav').stickyfloat({ duration: 400 });

    }

    function attachReferenceNumberEvents() {
        $("#modify-reference-number-dialog").dialog({
            modal: true,
            autoOpen: false,
            width: 400,
            buttons: {
                "Assign Reference Number": function () {
                    var referenceNumber = $("#new-reference-number").val();

                    var url = options.UpdateReferenceNumberUrl;

                    console.log(options.AntiForgeryToken);

                    $.post(url, { referenceNumber: referenceNumber, __RequestVerificationToken: options.AntiForgeryToken },
                            function (result) {
                                if (result.success === false) {
                                    alert("There was a problem updating the reference number.");
                                }
                                else {
                                    $("#reference-number").html(referenceNumber).effect('highlight', 'slow');

                                    if (options.IsKfsOrder) { //if we change the reference # on a KFS order, requery for new info
                                        purchasing.loadKfsData();
                                    }
                                }
                            }
                        );
                    $(this).dialog("close");
                },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#edit-reference-number").click(function (e) {
            e.preventDefault();

            $("#modify-reference-number-dialog").dialog("open");
        });
    }

    function attachPoNumberEvents() {
        $("#modify-po-number-dialog").dialog({
            modal: true,
            autoOpen: false,
            width: 400,
            buttons: {
                "Assign PO Number": function () {
                    var poNumber = $("#new-po-number").val();

                    var url = options.UpdatePoNumberUrl;

                    console.log(options.AntiForgeryToken);

                    $.post(url, { poNumber: poNumber, __RequestVerificationToken: options.AntiForgeryToken },
                            function (result) {
                                if (result.success === false) {
                                    alert("There was a problem updating the PO number.");
                                }
                                else {
                                    $("#po-number").html(poNumber).effect('highlight', 'slow');
                                }
                            }
                        );
                    $(this).dialog("close");
                },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#edit-po-number").click(function (e) {
            e.preventDefault();

            $("#modify-po-number-dialog").dialog("open");
        });
    }

    purchasing.loadKfsData = function () {
        $.getJSON(options.KfsStatusUrl, function (result) {
            console.log(result);
            if (result === null) {
                $("#kfs-loading").show();
                $("#kfs-data").hide();
                $("#kfs-loading-status").html("A problem was encountered accessing the Campus Financial Information Service. Please try again later.");
            }
            else {
                if (result.DocumentNumber === null) {
                    $("#kfs-loading").show();
                    $("#kfs-data").hide();
                    $("#kfs-loading-status").html("No Campus Financial Information Was Found For This Order. Please Verify That The Reference # Is Valid");
                } else {
                    $("#kfs-docnum").html(result.DocumentNumber);
                    //$("#kfs-ponum").html(result.PoNumber);
                    $("#kfs-potype").html(result.PoTypeCode);
                    $("#kfs-received").html(result.Received);
                    $("#kfs-fullypaid").html(result.FullyPaid);
                    $("#kfs-routelevel").html(result.RouteLevel);

                    if (result.DocUrl != "") {
                        $("#kfs-link").html($("<a>").attr("href", result.DocUrl).html("Click Here"));
                        $("#kfs-link-container").show();
                    } else {
                        $("#kfs-link-container").hide();
                    }

                    $("#kfs-loading").hide();
                    $("#kfs-data").show();
                }
            }
        });
    };

    function attachPeepsEvents() {
        $("#peepsDialog").dialog({
            autoOpen: false,
            height: 610,
            width: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#peepsUl").empty();
                    $(this).dialog("close");
                }
            }
        });

        $(function () {
            $("#approvals").on("click", ".workgroupDetails", function () {
                var el = $(this);
                var role = el.data("role");
                //alert(orderId + role);
                var dialogList = $("#peepsUl");
                dialogList.empty();
                $("#peepsDialog").dialog("open");
                $("#peepsLoaderId").toggle();
                $.getJSON(options.PeepsUrl, { orderStatusCodeId: role }, function (result) {
                    $("#peepsLoaderId").toggle();
                    if (result && result) {
                        $(result.peeps).each(function () {
                            dialogList.append("<li>" + this + "</li>");
                        });

                    } else {
                        alert("There was a problem getting the list of users.");
                    }
                });
            });
        });
    }

    function attachNoteEvents() {
        $("#notes-dialog").dialog({
            modal: true,
            autoOpen: false,
            width: 400,
            buttons: {
                "Confirm": function () {

                    var note = $("#notes-box").val();
                    $("#notes-box").val("");

                    var url = options.AddCommentUrl;

                    var orderid = $("#id").val();

                    $.post(url, { id: orderid, comment: note, __RequestVerificationToken: options.AntiForgeryToken },
                            function (result) {

                                if (result == false) {
                                    alert("There was a problem adding the comment.");
                                }
                                else {
                                    var comment = [{ datetime: result.Date, txt: result.Text, user: result.User}];
                                    $.tmpl($("#comment-template"), comment).appendTo("#notes table tbody");
                                    $(".notes-not-found").empty();
                                }

                            }
                        );

                    $(this).dialog("close");
                },
                "Cancel": function () { $(this).dialog("close"); }
            }
        });

        $("#add-note").click(function () { $("#notes-dialog").dialog("open"); return false; });
    }

    function attachApprovalEvents() {

        $("#reroute-search").dialog({
            autoOpen: false, modal: true,
            buttons: {
                "Confirm": function () {

                    var approvalId = $("#selected-approval").val();
                    var kerbId = $("#selected-person").val();
                    //                    var orderId = $("#selected-orderId").val();

                    // submit the values
                    $.post(options.ReRouteApprovalUrl, { /*id: orderId, */approvalId: approvalId, kerb: kerbId, __RequestVerificationToken: options.AntiForgeryToken }, function (result) {

                        if (result.success === true) {

                            $("a[data-approval-id=" + approvalId + "]").parents("td").siblings("td.name").html(result.name);

                        } else {

                            alert("User could not be assigned.");

                        }

                    });

                    // blank the controls
                    $("#selected-approval").val("");
                    $("#selected-person").val("");
                    $("#reroute-person").val("");

                    // close the dialog
                    $(this).dialog("close");
                },
                "Close": function () {
                    // blank the controls
                    $("#selected-approval").val("");
                    $("#selected-person").val("");
                    $("#reroute-person").val("");

                    // close the dialog
                    $(this).dialog("close");
                }
            }
        });

        $(".reroute").click(function (e) {

            // open the dialog to perform a search
            $("#reroute-search").dialog("open");

            $("#selected-approval").val($(this).data("approval-id"));

            $("#reroute-person").autocomplete({
                source: function (request, response) {
                    $.getJSON(options.UserSearchUrl, { searchTerm: request.term }, function (result) {
                        response($.map(result, function (item) { return { label: item.Label, value: item.Id }; }));
                    });
                },
                select: function (event, ui) {

                    $(event.target).val(ui.item.label);

                    $("#selected-person").val(ui.item.value);

                    return false;

                },
                minLength: 2
            });

            e.preventDefault();
        });
    }

    function attachFileEvents() {
        var uploader = new qq.FileUploader({
            // pass the dom node (ex. $(selector)[0] for jQuery users)
            element: document.getElementById('file-uploader'),
            // path to server-side upload script
            action: options.FileUploadUrl,
            fileTemplate: '<li>' +
            '<span class="qq-upload-file"></span>' +
            '<span class="qq-upload-spinner"></span>' +
            '<span class="qq-upload-size"></span>' +
            '<a class="qq-upload-cancel" href="#">Cancel</a>' +
            '<span class="qq-upload-failed-text">Failed</span>' +
            '<div class="ui-widget" style="display: inline-block"><select id="combobox" class="qq-upload-file-category jcs-combobox"><option value="">Select one...</option><option value="Order Confirmation">Order Confirmation</option><option value="Invoice">Invoice</option><option value="Shipping Notification">Shipping Notification</option><option value="Packing Slip">Packing Slip</option><option value="Licenses and Agreements">Licenses and Agreements</option><option value="Miscellaneous">Miscellaneous</option></select><div class="qq-upload-file-category-message" style="display: inline-block; margin-left: 33px;"></vid></div>' +
            '</li>',
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            onComplete: function (id, fileName, response) {
                if (response.success) {
                    var newFileContainer = $(uploader._getItemByFileId(id));
                    var fileDisplay = $("<a>").attr('href', '/Order/ViewFile?fileId=' + response.id).html(fileName);
                    newFileContainer.find(".qq-upload-file").empty().append(fileDisplay);
                    $(".attachments-not-found").empty();
                    newFileContainer.find(".qq-upload-file-category").attr("data-id", response.id);
                    $(".jcs-combobox").combobox();
                } else {
                    alert("File upload failed. (Missing Extension?)");
                }
            },
            debug: true
        });


        (function ($) {
            $.widget("ui.combobox", {
                _create: function () {
                    var input,
                        self = this,
                        select = this.element.hide(),
                        selected = select.children(":selected"),
                        value = selected.val() ? selected.text() : "",
                        wrapper = this.wrapper = $("<span>")
                            .addClass("ui-combobox")
                            .insertAfter(select);

                    input = $("<input>")
                        .appendTo(wrapper)
                        .attr("title", "Select from the list or type your own description. <strong>Tab off to update.</strong>").qtip()
                        .val(value)
                    //.addClass("ui-state-default ui-combobox-input")
                        .addClass("ui-combobox-input qq-upload-file-category")
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: function (request, response) {
                                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                                response(select.children("option").map(function () {
                                    var text = $(this).text();
                                    if (this.value && (!request.term || matcher.test(text)))
                                        return {
                                            label: text.replace(
                                                new RegExp(
                                                    "(?![^&;]+;)(?!<[^<>]*)(" +
                                                        $.ui.autocomplete.escapeRegex(request.term) +
                                                            ")(?![^<>]*>)(?![^&;]+;)", "gi"
                                                ), "<strong>$1</strong>"),
                                            value: text,
                                            option: this
                                        };
                                }));
                            },
                            //                            select: function (event, ui) {
                            //                                ui.item.option.selected = true;
                            //                                self._trigger("selected", event, {
                            //                                    item: ui.item.option
                            //                                });
                            //                                var fileContainer = $(this).parent().parent();
                            //                                var categeoryText = $(this).val();
                            //                                var attachmentGuid = fileContainer.find("#combobox").data("id");
                            //                                var categoryMessage = fileContainer.find(".qq-upload-file-category-message");

                            //                                categoryMessage.html("Updating...");

                            //                                $.post(options.UpdateAttachmentCategoryUrl, { guidId: attachmentGuid, category: categeoryText, __RequestVerificationToken: options.AntiForgeryToken }, function (result) {
                            //                                    if (result) {
                            //                                        categoryMessage.html(result.message);
                            //                                    } else {
                            //                                        alert("There was a problem updating the Attachment's Category");
                            //                                    }

                            //                                });
                            //                            },
                            change: function (event, ui) {
                                if (!ui.item) {
                                    var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
                                        valid = false;
                                    select.children("option").each(function () {
                                        if ($(this).text().match(matcher)) {
                                            this.selected = valid = true;
                                            return false;
                                        }
                                    });

                                    //Commented out so any value can be entered
                                    //                                    if (!valid) {
                                    //                                        // remove invalid value, as it didn't match anything
                                    //                                        $(this).val("");
                                    //                                        select.val("");
                                    //                                        input.data("autocomplete").term = "";
                                    //                                        return false;
                                    //                                    }
                                }
                                var fileContainer = $(this).parent().parent();
                                var categeoryText = $(this).val();
                                var attachmentGuid = fileContainer.find("#combobox").data("id");
                                var categoryMessage = fileContainer.find(".qq-upload-file-category-message");

                                categoryMessage.html("Updating...");

                                $.post(options.UpdateAttachmentCategoryUrl, { guidId: attachmentGuid, category: categeoryText, __RequestVerificationToken: options.AntiForgeryToken }, function (result) {
                                    if (result) {
                                        categoryMessage.html(result.message);
                                    } else {
                                        alert("There was a problem updating the Attachment's Category");
                                    }

                                });
                            }
                        })
                        .addClass("ui-widget ui-widget-content ui-corner-left");

                    input.data("autocomplete")._renderItem = function (ul, item) {
                        return $("<li></li>")
                            .data("item.autocomplete", item)
                            .append("<a>" + item.label + "</a>")
                            .appendTo(ul);
                    };

                    $("<a>")
                        .attr("tabIndex", -1)
                        .attr("title", "Show All Items")
                        .appendTo(wrapper)
                        .button({
                            icons: {
                                primary: "ui-icon-triangle-1-s"
                            },
                            text: false
                        })
                        .removeClass("ui-corner-all")
                        .addClass("ui-corner-right ui-combobox-toggle")
                        .click(function () {
                            // close if already visible
                            if (input.autocomplete("widget").is(":visible")) {
                                input.autocomplete("close");
                                return;
                            }

                            // work around a bug (likely same cause as #5265)
                            $(this).blur();

                            // pass empty string as value to search for, displaying all results
                            input.autocomplete("search", "");
                            input.focus();
                        });
                },

                destroy: function () {
                    this.wrapper.remove();
                    this.element.show();
                    $.Widget.prototype.destroy.call(this);
                }
            });
        })(jQuery);

        $(function () {
            $(".jcs-combobox").combobox();
            $("#toggle").click(function () {
                $(this).toggle();
            });
        });

    }

    function attachSubmitEvents() {
        $("#deny-order").click(function (e) {
            if (!$("#comment").val()) {
                alert("A comment is required when denying an order");
                e.preventDefault();
            }
        });
    }

    function attachCompletionEvents() {
        //Methods for completing an order, as a purchaser
        $("#orderType").change(function () {
            var el = $(this);

            if (el.val() === "KFS") {
                //show the kfs options box and disable the button
                $("#kfsDocType").val('').show();
                $("#complete-order").button("disable");
            } else {
                //we didn't select kfs, so go ahead and hide the kfs options box and enable complete button
                $("#kfsDocType").hide();
                $("#complete-order").button("enable");
            }
        });

        $("#kfsDocType").change(function () {
            var enableOption = this.value === '' ? "disable" : "enable";

            $("#complete-order").button(enableOption);
        });
    }

    function attachCancelEvents() {
        $("#cancel-form").submit(function (e) {
            var valid = $(this).validate().form();

            if (valid && !confirm("Are you sure you want to cancel this order?")) {
                e.preventDefault();
            }
        });
    }

    function attachCancelCompletedOrderEvents() {
        $("#cancel-completed-order-form").submit(function (e) {
            var valid = $(this).validate().form();

            if (valid && !confirm("Are you sure you want to cancel this order? This action cannot be undone.")) {
                e.preventDefault();
            }
        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));