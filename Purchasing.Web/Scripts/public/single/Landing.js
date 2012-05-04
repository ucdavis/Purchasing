//Self-Executing Anonymous Function
(function (purchasing, $, undefined) {
    //Private Property
    var options = {};

    //Public Method
    purchasing.options = function (o) {
        $.extend(options, o);
    };

    purchasing.init = function () {
        setupInitialDisplay();
        attachTabEvents();
        attachToolTipEvents();
        loadRecentHistory();
        loadCompleteHistory();
        loadDeniedHistory();
        loadCommentHistory();
    };

    function setupInitialDisplay() {
        //Let's determine which tab to show based on which table contains >0 entries
        var currentlySelectedTab = $("#tab-container").find("li.selected");

        var urgentOrdersExist = $("#tab-orders-urgent").find(".bignum").html() > 0;

        if (!urgentOrdersExist) {
            //If urgent orders don't exist, show pending or your orders, depending on if there are any pending orders
            var pendingOrdersExist = $("#tab-orders-pending").find(".bignum").html() > 0;

            swapTables(currentlySelectedTab, pendingOrdersExist ? $("#tab-orders-pending") : $("#tab-orders-open"));
        }
    }

    function attachToolTipEvents() {
        $("section.dashboard-main").on('mouseenter focus', 'a[title], a[oldtitle]', function () {
            $(this).qtip({
                overwrite: false,
                content: {
                    text: function (api) {
                        return $(this).attr("oldtitle");
                    }
                },
                show: {
                    event: 'mouseenter focus',
                    ready: true
                },
                hide: {
                    event: 'mouseleave blur'
                },
                position: {
                    my: 'bottom center',
                    at: 'top center'
                }
            });
        });
    }

    function loadRecentHistory() {
        $("#recent-activity-container").load(options.RecentActivityUrl);
    }

    function loadCompleteHistory() {
        $("#completed-container").load(options.RecentlyCompletedUrl);
    }

    function loadDeniedHistory() {
        $("#denied-container").load(options.RecentlyDeniedUrl);
    }

    function loadCommentHistory() {
        $("#recent-comments-container").load(options.RecentComments);
    }

    function attachTabEvents() {
        $("#tab-container").on('click', "li", function (e) {
            var el = $(this);
            if (el.is(".action")) {
                return;
            }

            e.preventDefault();

            var currentlySelectedTab = $("#tab-container").find("li.selected");

            swapTables(currentlySelectedTab, el);
        });
    }

    function swapTables(selected, toSelect) {
        if (selected[0] === toSelect[0]) {
            return;
        }

        //We clicked on a new tab, so first remove the existing info and unselect the selected tab
        $("#" + selected.data("type")).html($("#main-orders-body").html());
        $("#main-orders-body").empty();
        selected.removeClass("selected");

        //Now use the new tab and move its data up into the order body
        var storedOrderData = $("#" + toSelect.data("type"));
        $("#main-orders-body").html(storedOrderData.html());
        storedOrderData.empty();
        toSelect.addClass("selected");

        //Update the 'view more orders like this' link
        $("#orders-view-history").attr("href", options.OrdersBaseUrl + toSelect.data("filter"));
    }

} (window.purchasing = window.purchasing || {}, jQuery));