//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = {};
    

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing.init = function () {
       
        attachRecordEvents();
        attachHistoryEvents();
        attachFileEvents();
        attachNoteEvent();
    };

    function attachRecordEvents () {

        $("#RecordButton").click(function () {
            $(".receiveLineItem").each(function () {
                var receiveVal = $(this).val();
                var lineItemId = $(this).data("id");
                var initial = $("#original" + lineItemId).val();
                if (initial != receiveVal) {
                    if (!$.isNumeric(receiveVal)) {
                        alert("Must be a number");
                    } else {

                        var id = $("#id").val();
                        var antiforgery = $("input[name='__RequestVerificationToken']").val();
                        var url = options.ReceiveItemUrl;
                        var loader = $("#" + lineItemId + " .quantity-loader");
                        var outstanding = $("#" + lineItemId + " .unaccounted");

                        $("#" + lineItemId + " .quantity-update-message").html("Updating...");
                        loader.show();
                        $.post(url, { id: id, lineItemId: lineItemId, receivedQuantity: receiveVal, __RequestVerificationToken: antiforgery }, function (result) {
                            loader.hide();
                            if (result) {
                                $("#" + lineItemId + " .receiveQuantity").html(result.receivedQuantityReturned);
                                if (result.success == true) {
                                    $("#" + lineItemId + " .quantity-update-message").html(result.message);
                                    $("#original" + lineItemId).val(receiveVal);
                                    outstanding.html(result.unaccounted);
                                    $("#" + lineItemId + " .user-name").html(result.lastUpdatedBy);
                                    $("#" + lineItemId + " .update-date").html("Just Now");
                                    if (result.showRed) {
                                        outstanding.removeClass("green");
                                        outstanding.addClass("red");
                                    } else {
                                        outstanding.removeClass("red");
                                        outstanding.addClass("green");
                                    }
                                } else {
                                    $("#" + lineItemId + " .quantity-update-message").html(result.message);
                                }
                            } else {
                                alert("There was a problem updating the received quantity.");
                            }

                        });
                    }
                }
            });

        });

        $(".receiveNotes").change(function () {
            var id = $("#id").val();
            var notesVal = $(this).val();
            var lineItemId = $(this).data("id");
            var antiforgery = $("input[name='__RequestVerificationToken']").val();
            var url = options.ReceiveNotesUrl;

            var updateMessage = $("#notesLineId" + lineItemId + " .notes-update-message");
            var loader = $("#notesLineId" + lineItemId + " .notes-loader");

            loader.show();
            updateMessage.html("Updating...");
            $.post(url, { id: id, lineItemId: lineItemId, note: notesVal, __RequestVerificationToken: antiforgery }, function (result) {
                loader.hide();
                if (result) {
                    if (result.success == true) {
                        $("#" + lineItemId + " .user-name").html(result.lastUpdatedBy);
                        $("#" + lineItemId + " .update-date").html("Just Now");
                        updateMessage.html(result.message);
                    } else {
                        updateMessage.html(result.message);
                    }
                } else {
                    alert("There was a problem updating the Notes.");
                }

            });
        });

        $(".toggle-line-item-details").click(function () {
            var data = $(this).data("id");
            var idToToggle = "#notesLineId" + data;
            $(idToToggle).toggle();
        });

    };

    function attachHistoryEvents () {
        $(".userDetails").click(function () {
            var temp = $(this);
            var orderId = $("#id").val();
            var lineItemId = temp.data("id");
            var url = options.HistoryUrl;
            var dialogList = $("#historyBody");

            dialogList.empty();
            $("#peepsDialog").dialog("open");
            $("#peepsLoaderId").show();
            $.getJSON(url, { id: orderId, lineItemId: lineItemId, payInvoice: false }, function (result) {
                $("#peepsLoaderId").hide();
                if (result == null || result.success == false) {
                    alert("There was a problem getting the list of users.");
                } else {

                    $(result.history).each(function () {
                        dialogList.append("<tr>" + "<td>" + this.FullName + "</td><td>" + this.updateDate + "</td><td>" + this.whatWasUpdated + "</td></tr>");
                    });
                }
            });
        });
    };

    function attachFileEvents() {
        var uploader = new qq.FileUploader({
            // pass the dom node (ex. $(selector)[0] for jQuery users)
            element: document.getElementById('file-uploader'),
            // path to server-side upload script
            action: options.UploadFileUrl,
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
                    var viewAttachmentUrl = options.ViewAttachment + "?fileId=" + response.id;
                    var fileDisplay = $("<a>").attr('href', viewAttachmentUrl).html(fileName);
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

        (function () {
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
                                }
                                var fileContainer = $(this).parent().parent();
                                var categeoryText = $(this).val();
                                var attachmentGuid = fileContainer.find("#combobox").data("id");
                                var categoryMessage = fileContainer.find(".qq-upload-file-category-message");

                                categoryMessage.html("Updating...");

                                $.post( options.UpdateAttachmentCategoryUrl, { guidId: attachmentGuid, category: categeoryText, __RequestVerificationToken: $('#forgery-token > input[name="__RequestVerificationToken"]').val() }, function (result) {
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

    function attachNoteEvent () {
        $("#add-note").click(function () {
            $("#notes-dialog").dialog({
                modal: true,
                autoOpen: true,
                width: 400,
                buttons: {
                    "Confirm": function () {

                        var note = $("#notes-box").val();
                        $("#notes-box").val("");

                        var url = options.AttachNoteUrl;

                        var orderid = $("#id").val();
                        var antiforgery = $('#forgery-token > input[name="__RequestVerificationToken"]').val();


                        $.post(url, { id: orderid, comment: note, __RequestVerificationToken: antiforgery },
                            function (result) {

                                if (result == false) {
                                    alert("There was a problem adding the comment.");
                                } else {
                                    var comment = [{ datetime: result.Date, txt: result.Text, user: result.User }];
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

            $("#add-note").click(function () {
                $("#notes-dialog").dialog("open");
                return false;
            });
        });
    };

}(window.purchasing = window.purchasing || {}, jQuery));

