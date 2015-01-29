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
        //if (options.CanCancel) {
        //    attachCancelEvents();
        //}

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
            var url = '@Url.Action("ReceiveItemsNotes", new {id = Model.OrderId})';

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

}(window.purchasing = window.purchasing || {}, jQuery));

