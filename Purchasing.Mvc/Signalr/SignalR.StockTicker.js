/// <reference path="../Scripts/jquery-1.10.2.js" />
/// <reference path="../Scripts/jquery.signalR-2.1.1.js" />
$(function () {

    window.purchaseInfo = $.connection.purchaseInfo;
    
    // Add client-side hub methods that the server will call
    $.extend(window.purchaseInfo.client, {
        orderUpdate: function (info) {
            alert('testing call from server');
        },
    });

    // Start the connection
    $.connection.hub.start();
});