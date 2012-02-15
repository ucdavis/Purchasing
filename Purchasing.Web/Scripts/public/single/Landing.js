
//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = {};

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing.init = function () {
        attachTabEvents();
    };

    function attachTabEvents() {
        $("#tab-container").on('click', "li", function (e) {
            e.preventDefault();

            var el = $(this);
            var currentlySelectedTab = $("#tab-container").find("li.selected");

            if (currentlySelectedTab[0] === el[0]) {
                return;
            }

            //We clicked on a new tab, so first remove the existing info and unselect the selected tab
            $("#" + currentlySelectedTab.data("type")).html($("#main-orders-body").html());
            $("#main-orders-body").empty();
            currentlySelectedTab.removeClass("selected");

            //Now use the new tab and move its data up into the order body
            var storedOrderData = $("#" + el.data("type"));
            $("#main-orders-body").html(storedOrderData.html());
            storedOrderData.empty();
            el.addClass("selected");

            //Update the 'view more orders like this' link
            $("#orders-view-history").attr("href", options.OrdersBaseUrl + el.data("filter"));
        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));