///<reference path="jquery-1.6.2-vsdoc.js"/>

//Self-Executing Anonymous Function
//Adding New Functionality to Purchasing for Edit
(function (purchasing, $, undefined) {
    //Private Property
    var routingAdjusted = false;

    //Public Method
    purchasing.initEdit = function () {
        loadLineItems();
    };

    //Loads up the line litem info from ajax for this order
    function loadLineItems() {
        //Place a 'loading line items' ui block
        $.getJSON(purchasing._getOption("LoadLineItemsUrl"), null, function (result) {
            console.log(result);
        });
    }

} (window.purchasing = window.purchasing || {}, jQuery));
