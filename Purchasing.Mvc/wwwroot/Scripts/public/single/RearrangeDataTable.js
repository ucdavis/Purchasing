function RearranngeDataTable($wrapper) {
    var $dtt = $wrapper.find(".DTTT_container");
    var $length = $wrapper.find(".dataTables_length");

    // dtt is present, move those accordingly
    if ($dtt.length > 0) {
        $("<div>").addClass("dttt_buttons").append($dtt.find("button")).insertAfter($length.find("label"));
        $wrapper.find(".DTTT_container").remove();
        $wrapper.find(".clear").remove();
    }
}