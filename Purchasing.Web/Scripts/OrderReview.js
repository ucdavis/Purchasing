///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = {};

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing.init = function () {
        if (options.CanEdit) {
            attachNoteEvents();
            attachApprovalEvents();
            attachFileEvents();
            attachSubmitEvents();
        }

        if (options.CanCancel) {
            attachCancelEvents();
        }
    };

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

                        if (result.success) {

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
                    $.getJSON(options.UserSearchUrl, { searchTerm: request.term }, function (result) { response($.map(result, function (item) { return { label: item.Label, value: item.Id }; })); });
                },
                select: function (event, ui) {

                    $(event.target).val(ui.item.label);

                    $("#selected-person").val(ui.item.value);

                    return false;

                },
                minLength: 2
            });

            //            var el = $(this);
            //            var approvalId = el.data("approval-id");

            //            var buttonContainer = el.parent();
            //            var nameContainer = buttonContainer.siblings(".name");
            //            var nameContents = nameContainer.html(); //Save in case of cancel

            //            nameContainer.html($("<input type='text' placeholder='new approver kerberos'/>"));

            //            buttonContainer.children().toggle();

            //            $(".cancel", buttonContainer).click(function (event) {
            //                nameContainer.html(nameContents);

            //                buttonContainer.children().toggle();
            //                event.preventDefault();

            //                $(this).unbind();
            //            });

            //            $(".update", buttonContainer).click(function (event) {
            //                var url = options.ReRouteApprovalUrl;
            //                var kerb = nameContainer.find("input").val();

            //                $.post(url, { id: approvalId, kerb: kerb, __RequestVerificationToken: options.AntiForgeryToken }, function (result) {
            //                    console.log(result);

            //                    if (result.success) {
            //                        nameContainer.html(result.name);
            //                        nameContainer.animate('highlight');
            //                    } else {
            //                        alert("sorry, I don't recognize that kerb, they probably aren't in the system yet.... I'll deal with that later");
            //                    }

            //                    buttonContainer.children().toggle();
            //                });

            //                event.preventDefault();
            //                $(this).unbind();
            //            });

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
            '</li>',
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            onComplete: function (id, fileName, response) {
                var newFileContainer = $(uploader._getItemByFileId(id));
                var fileDisplay = $("<a>").attr('href', '/Order/ViewFile?fileId=' + response.id).html(fileName);
                newFileContainer.find(".qq-upload-file").empty().append(fileDisplay);
                $(".attachments-not-found").empty();
            },
            debug: true
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

    function attachCancelEvents() {
        $("#cancel-form").submit(function (e) {
            var valid = $(this).validate().form();

            if (valid && !confirm("Are you sure you want to cancel this order?")) {
                e.preventDefault();
            }
        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));