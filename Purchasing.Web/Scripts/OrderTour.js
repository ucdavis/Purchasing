//Self-Executing Anonymous Function
(function (tour, $, undefined) {
    tour.start = function () {
        guiders.createGuider({
            buttons: [{ name: "Next", another: "Next"}],
            attachTo: ".orders-nav",
            description: "Lorem ipsum",
            id: "first",
            next: "second",
            position: 2,
            highlight: ".orders-nav",
            title: "Sample title"
        }).show();

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "#split-by-line",
            description: "Lorem ipsum",
            id: "second",
            next: "third",
            position: 1,
            title: "Sample title"
        });

        guiders.createGuider({
            buttons: [{ name: "Close" }],
            attachTo: ".sub-line-item-split:first",
            description: "Lorem ipsum",
            id: "third",
            next: "fourth",
            position: 1,
            title: "Sample title",
            onShow: function() {
                $("#split-by-line").trigger('click', { automate: true });
            }
        });
    };
} (window.tour = window.tour || {}, jQuery));