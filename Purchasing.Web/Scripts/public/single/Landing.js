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
        attachToolTipEvents();
        loadRecentHistory();
        loadCompleteHistory();
        loadCommentHistory();
    };
    
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
        $("#recently-completed-container").load(options.RecentlyCompletedUrl);
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