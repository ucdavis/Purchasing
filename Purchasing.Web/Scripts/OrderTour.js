//Self-Executing Anonymous Function
(function (tour, $, undefined) {
    tour.startOverview = function () {
        guiders.createGuider({
            buttons: [{ name: "Close" }, { name: "Let's get started", onclick: guiders.next}],
            description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam interdum dapibus velit id fermentum. Suspendisse potenti. Sed pellentesque lacus id ipsum iaculis dapibus. Nullam semper facilisis velit eget accumsan.",
            id: "intro",
            next: "justification",
            position: 0,
            overlay: true,
            title: "Guided Tour"
        }).show();

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "textarea[name=justification]",
            description: "Note the required <span class='required'>*</span> for required fields.",
            id: "justification",
            next: "vendor",
            onShow: function (guider) {
                $(guider.attachTo).val("Enter some text");
            },
            position: 11,
            highlight: ".orders-nav",
            title: "Fill Out Fields"
        });

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "#vendor",
            description: "Select a vendor or leave the vendor 'unspecified'. If you do not see your desired vendor in the drop down list, you can add a new vendor or lookup an existing vendor from the campus financial data source",
            id: "vendor",
            next: "addvendorbutton",
            position: 1,
            title: "Vendor Selection Options"
        });

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "#add-vendor",
            description: "Let's look at adding a new vendor first <br/><br/>Clicking on the 'Add New Vendor' button will bring up the Add Vendor dialog",
            id: "addvendorbutton",
            next: "addvendor",
            overlay: true,
            highlight: "#add-vendor",
            position: 1,
            title: "Vendor Selection Options"
        });

        guiders.createGuider({
            buttons: [{ name: "Next"}],
            attachTo: "#vendor-name",
            onShow: function (guider) {
                $("#add-vendor").click(); //TODO: either turn off animation or wait until complete
                $("#vendor-name").val("New Vendor Name");
                $("#vendor-address").val("1 shields ave, 38 mrak hall");
                $("#vendor-city").val("Davis");
                $("#vendor-state").val("CA");
                $("#vendor-zip").val("95616");
                $("#vendor-phone").val("555-5555");
                $("#vendor-email").val("vendorcontact@buymore.com");
            },
            description: "Fill out the required<span class='required'>*</span> fields and then click the 'Create Vendor' button at the bottom of the dialog. "
                + "This will create a new vendor and automatically select it for you in the vendor list"
                + "<br/><br/>Click cancel if you want to close the dialog without saving your changes",
            id: "addvendor",
            next: "searchvendor",
            position: 3,
            title: "Add A New Vendor"
        });

        /*
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
        */

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
    };
} (window.tour = window.tour || {}, jQuery));