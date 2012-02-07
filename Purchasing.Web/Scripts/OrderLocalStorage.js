///<reference path="jquery-1.6.2.js"/>
///<reference path="Order.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Public Method
    purchasing.initLocalStorage = function () {
        if (window.Modernizr.localstorage) {
            attachUserTrackingEvents();
        }
    };

    function attachUserTrackingEvents() {
        checkFirstTime();

        $("#clear-user-history").live('click', function (e) {
            e.preventDefault();
            localStorage.clear(); //TODO: clears all local storage, remove when done testing
            alert("cleared!  Refresh the page to appear like a first timer");
        });
    }

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

} (window.purchasing = window.purchasing || {}, jQuery));