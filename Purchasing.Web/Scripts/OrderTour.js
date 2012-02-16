//Self-Executing Anonymous Function
(function (tour, $, undefined) {
    tour.start = function () {
        guiders.createGuider({
            buttons: [{ name: "Close" }, { name: "Let's get started", onclick: guiders.next}],
            description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam interdum dapibus velit id fermentum. Suspendisse potenti. Sed pellentesque lacus id ipsum iaculis dapibus. Nullam semper facilisis velit eget accumsan.",
            id: "intro",
            next: "navbar",
            position: 0,
            overlay: true,
            title: "Guided Tour"
        }).show();

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: ".orders-nav",
            description: "Lorem ipsum",
            id: "navbar",
            next: "justification",
            position: 2,
            highlight: ".orders-nav",
            title: "Navigation Sidebar"
        });

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "textarea[name=justification]",
            description: "Note the required <span class='required'>*</span> for required fields.",
            id: "navbar",
            //id: "justification",
            next: "second",
            onShow: function (guider) {
                $(guider.attachTo).val("Enter some text");
            },
            position: 11,
            highlight: ".orders-nav",
            title: "Fill Out Fields"
        });

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
            buttons: [{ name: "Close"}],
            attachTo: ".sub-line-item-split:first",
            description: "Lorem ipsum",
            id: "third",
            next: "fourth",
            position: 1,
            title: "Sample title",
            onShow: function () {
                $("#split-by-line").trigger('click', { automate: true });
            }
        });
    };
} (window.tour = window.tour || {}, jQuery));