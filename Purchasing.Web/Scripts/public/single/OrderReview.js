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

        if (options.IsKfsOrder) {
            purchasing.loadKfsData();
        }

        if (options.IsComplete) {
            attachReferenceNumberEvents();
        }
    };

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

    purchasing.loadKfsData = function () {
        $.getJSON(options.KfsStatusUrl, function (result) {
            console.log(result);
            if (result.PoNumber === null) {
                $("#kfs-loading").show();
                $("#kfs-data").hide();
                $("#kfs-loading-status").html("No Campus Financial Information Was Found For This Order. Please Verify That The PO Number Is Valid");
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
                }
                else {
                    $("#kfs-link-container").hide();
                }

                $("#kfs-loading").hide();
                $("#kfs-data").show();
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
            $(".workgroupDetails").click(function () {
                var temp = $(this);
                var orderId = temp.data("id");
                var role = temp.data("role");
                //alert(orderId + role);
                var dialogList = $("#peepsUl");
                dialogList.empty();
                $("#peepsDialog").dialog("open");
                $("#peepsLoaderId").toggle();
                $.getJSON(options.PeepsUrl, { id: orderId, orderStatusCodeId: role }, function (result) {
                    $("#peepsLoaderId").toggle();
                    if (result == null || result.success == false) {
                        alert("There was a problem getting the list of users.");
                    } else {


                        $(result.peeps).each(function () {
                            dialogList.append("<li>" + this + "</li>");
                        });
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
            '</li>',
            sizeLimit: 4194304, //TODO: add configuration instead of hardcoding to 4MB
            onComplete: function (id, fileName, response) {
                if (response.success) {
                    var newFileContainer = $(uploader._getItemByFileId(id));
                    var fileDisplay = $("<a>").attr('href', '/Order/ViewFile?fileId=' + response.id).html(fileName);
                    newFileContainer.find(".qq-upload-file").empty().append(fileDisplay);
                    $(".attachments-not-found").empty();
                } else {
                    alert("File upload failed");
                }
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

} (window.purchasing = window.purchasing || {}, jQuery));