/// <reference path="../../jquery-1.7.1-vsdoc.js" />
/// <reference path="../../jquery.signalR-2.1.1.js" />
/// <reference path="~/Scripts/toastr.js" />
$(function () {

    var orderNotification = $.connection.orderNotification;

    toastr.options.timeOut = 60 * 1000; //60 seconds
    toastr.options.closeButton = true;

    // Add client-side hub methods that the server will call
    $.extend(orderNotification.client, {
        orderUpdate: function (order) {
            var url = window.Configuration.ReviewUrl + '/' + order.Id;
            toastr.info("<h3><a href='" + url + "' target='_blank'>Click here to review order #" + order.RequestNumber + "</a><h3>", "New Order Requires Your Approval");
        },
    });

    // Start the connection
    $.connection.hub.start();
});