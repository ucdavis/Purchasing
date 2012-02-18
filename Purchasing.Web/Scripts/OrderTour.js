//Self-Executing Anonymous Function
(function (tour, $, undefined) {
    var closeButton = { name: "Quit Tour", onclick: function () { tour.complete(); } };

    tour.isOriginalRequest = function () {
        return window.location.toString().indexOf("/Request/") !== -1;
    };

    tour.startTour = function (startId) {
        purchasing.preTour(tour.isOriginalRequest());

        guiders.show(startId);
    };

    tour.complete = function () {
        guiders.hideAll();
        purchasing.postTour(tour.isOriginalRequest());
    };

    function loadTourInfo() {
        var intro = tour.isOriginalRequest() === true ? "Don't panic!  Your current form will be saved and restored whenever you choose to quit the tour" : "Your existing data will be lost in order for the tour to continue. If this is not ok, please click close now";

        guiders.createGuider({
            buttons: [{ name: "Close" }, { name: "Let's get started", onclick: function () {
                //We have chosen to enter the tour, so reset the page
                $("#item-modification-button").trigger('click', { automate: true }); //allow modification if available
                purchasing.setSplitType("None");
                scrollTo(0, 0); //reset at the top of the page
                guiders.next();
            }
            }],
            description: intro + "<br/><br/>Prepare to be taken to a dreamworld of magic",
            id: "intro",
            //next: "justification",
            next: "lineitemsoverview",
            position: 0,
            overlay: true,
            title: "Guided Tour"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
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
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#vendor",
            description: "Select a vendor or leave the vendor 'unspecified'. If you do not see your desired vendor in the drop down list, you can add a new vendor or lookup an existing vendor from the campus financial data source",
            id: "vendor",
            next: "addvendorbutton",
            position: 1,
            title: "Vendor Selection Options"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#add-vendor",
            description: "Let's look at adding a new vendor first <br/><br/>Clicking on the 'Add New Vendor' button will bring up the Add Vendor dialog",
            id: "addvendorbutton",
            next: "addvendor",
            overlay: true,
            highlight: "#add-vendor",
            position: 1,
            title: "Add A New Vendor"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#vendor-name",
            onShow: function (guider) {
                $("#add-vendor").click(); //TODO: either turn off animation or wait until complete
                $("#vendor-name").val("New Vendor Name");
                $("#vendor-address").val("One Infinite Loop");
                $("#vendor-city").val("Coopertino");
                $("#vendor-state").val("CA");
                $("#vendor-zip").val("90210");
                $("#vendor-phone").val("555-5555");
                $("#vendor-email").val("contact@buymore.com");
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
            },
            description: "Fill out the required<span class='required'>*</span> fields and then click the 'Create Vendor' button at the bottom of the dialog. "
            + "This will create a new vendor and automatically select it for you in the vendor list"
            + "<br/><br/>Click cancel if you want to close the dialog without saving your changes",
            id: "addvendor",
            next: "searchvendorbutton",
            position: 3,
            title: "Add A New Vendor"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#search-vendor",
            description: "Now let's look at searching for an existing vendor (from KFS)<br/><br/>Clicking on the 'Search Vendor' button will bring up the Search Vendor dialog",
            id: "searchvendorbutton",
            next: "searchvendor",
            overlay: true,
            highlight: "#search-vendor",
            position: 1,
            title: "Vendor Search"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#search-vendor",
            description: "Maybe use an image to show vendor search... animated gif ok...",
            id: "searchvendor",
            next: "address",
            position: 0,
            title: "Vendor Search"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#shipAddress",
            description: "Select an address where the order should be shipped. If you do not see your desired address in the drop down list, you can add a new address at any time by clicking the 'Add New Shipping Address' button",
            id: "address",
            next: "addaddressbutton",
            position: 1,
            title: "Address Selection Options"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#add-address",
            description: "Let's quickly look at adding a new address <br/><br/>Clicking on the 'Add New Shipping Address' button will bring up the Add Shipping Address dialog",
            id: "addaddressbutton",
            next: "addaddress",
            overlay: true,
            highlight: "#add-address",
            position: 1,
            title: "Add A New Address"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#address-name",
            onShow: function (guider) {
                $("#add-address").click(); //TODO: either turn off animation or wait until complete
                $("#address-name").val("New Address Name");
                $("#address-building").val("Mrak Hall");
                $("#address-room").val("38");
                $("#address-address").val("One Shields Ave");
                $("#address-city").val("Davis");
                $("#address-state").val("CA");
                $("#address-zip").val("95616");
                $("#address-phone").val("754-5555");
            },
            onHide: function () {
                $(".ui-dialog-titlebar-close").click();
            },
            description: "Fill out the required<span class='required'>*</span> fields and then click the 'Create Address' button at the bottom of the dialog. "
            + "This will create a new address and automatically select it for you in the address list"
            + "<br/><br/>Click cancel if you want to close the dialog without saving your changes",
            id: "addaddress",
            next: "lineitemsoverview",
            position: 3,
            title: "Add A New Address"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "body",
            description: "Now let's look at a quick overview of adding line items <br/><br/>"
            + "For in depth information on different line item features, look for the "
            + "<span class=\"ui-icon ui-icon-help\"></span>"
            + "icons which will take you on an in depty guided tour of relevant features at any time",
            id: "lineitemsoverview",
            next: "lineitems",
            overlay: true,
            position: 0,
            title: "Line Items Overview"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#line-items",
            description: "Fill in at least the quantity, description, and unit price of each line item in your order.  All price calculations will update automatically as you type"
            + "<br/><br/>You can add new line item rows as needed by clicking 'Add New Item'",
            onShow: function () {
                $("input[name='items[0].quantity']").val(12);
                $("input[name='items[0].description']").val("lawn chairs");
                $("input[name='items[0].price']").val(20.25);
                $("input[name='items[1].quantity']").val(3);
                $("select[name='items[1].units']").val("DZ");
                $("input[name='items[1].description']").val("apples");
                $("input[name='items[1].price']").val(5).change();
            },
            id: "lineitems",
            next: "orderdetailsoverview",
            overlay: true,
            highlight: "#line-items-section",
            position: 1,
            title: "Line Items Overview"
        });

        var hasApprover = $("#approver").length !== 0;

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#line-items",
            description: "When you are finished adding line items, you need to select how to account for the costs. "
            + "We'll start by looking at <strong>Account Selection</strong>"
            + (hasApprover ? " and <strong>Approver/Manager Selection</strong>, the simplest choices" : " ")
            + "<br/><br/>For advanced features like splitting orders across multiple accounts, or even splitting each line item across multiple accounts, please click on the relevant"
            + "<span class=\"ui-icon ui-icon-help\"></span>"
            + "icons to get a targeted tour at any time",
            id: "orderdetailsoverview",
            next: "orderdetails-accountselection",
            overlay: true,
            position: 0,
            title: "Order Details Overview"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#Account",
            description: "If you know the account this order should be charged against, select it from the account drop down.  If there are any subaccounts related to the selected account, they will appear in the '--Sub Account-' drop down."
            + "<br/><br/>If you do not see the desired account in the list, you can use the <img src=\"/Images/details.png\"> icon to lookup any account",
            onShow: function () {
                var firstChoice = $("#Account :nth-child(2)").val();
                $("#Account").val(firstChoice);
            },
            id: "orderdetails-accountselection",
            next: hasApprover ? "orderdetails-approverselection" : "orderformcompletion",
            overlay: false,
            highlight: '#account-form',
            position: 1,
            title: "Account Selection"
        });

        if (hasApprover) {
            guiders.createGuider({
                buttons: [closeButton, { name: "Next" }],
                attachTo: "#approvers",
                description: "Alternately, you can select a Purchasing Agent to route this order to. An approver can also be specified."
                + "<br/><br/>This option would be useful if you do not know the account this order should be charged to",
                onShow: function() {
                    var firstChoice = $("#Account :nth-child(2)").val();
                    $("#Account").val(firstChoice);
                },
                id: "orderdetails-approverselection",
                next: "orderformcompletion",
                highlight: '#account-form',
                position: 1,
                title: "Purchasing Agent Selection"
            });
        }

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "#order-preferences-section",
            description: "Fill out the rest of the form as desired, including uploading associated files",
            id: "orderformcompletion",
            next: "submit",
            position: 6,
            title: "Complete The Order Form"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: "input.order-submit",
            description: "When you are ready, click 'Create' to create a new order!",
            id: "submit",
            next: "coda",
            position: 12,
            title: "Submit"
        });

        guiders.createGuider({
            buttons: [{ name: "Thanks for the tour, I'll take it from here!", onclick: function () { tour.complete(); } }],
            description: "Blah Blah",
            id: "coda",
            overlay: true,
            position: 0,
            title: "Coda"
        });

        guiders.createGuider({
            buttons: [closeButton, { name: "Next"}],
            attachTo: ".orders-nav",
            description: "Lorem ipsum",
            id: "navbar",
            next: "justification",
            position: 2,
            highlight: ".orders-nav",
            title: "Navigation Sidebar"
        });
    }

    loadTourInfo(); //load tour info when this object is loaded, and wait for start to be called

} (window.tour = window.tour || {}, jQuery));