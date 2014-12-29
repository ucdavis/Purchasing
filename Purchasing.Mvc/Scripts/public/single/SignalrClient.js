/// <reference path="../../jquery-1.7.1-vsdoc.js" />
/// <reference path="../../jquery.signalR-2.1.1.js" />
/// <reference path="~/Scripts/toastr.js" />
$(function () {

    window.purchaseInfo = $.connection.purchaseInfo;

    toastr.options.timeOut = 60 * 1000; //60 seconds
    toastr.options.closeButton = true;

    // Add client-side hub methods that the server will call
    $.extend(window.purchaseInfo.client, {
        orderUpdate: function (info) {
            toastr.info("<h3><a href='http://google.com' target='_blank'>Click here to review order #ABSC-123SFG2</a><h3>", "Order Status Changed");
        },
    });

    // Start the connection
    $.connection.hub.start();
});