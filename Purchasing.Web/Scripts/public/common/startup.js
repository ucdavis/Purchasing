var uvOptions = {};

if (typeof console == "undefined") { //Define the console as a backup for browsers without console
    window.console = {
        log: function () { }
    };
}

$(function() { //run the page load startup code
    $('input[type=checkbox]').tzCheckbox({ labels: ['Yes', 'No'] });
    $(".dt-table, .datatable").dataTable({ "bJQueryUI": true, "sPaginationType": "full_numbers", "iDisplayLength": window.Configuration.DataTablesPageSize });
    $(".button, .text_btn, button").button();
    $('input[type="datetime"]').datepicker();
    $('input.datepicker').datepicker();

    initUservoice();
});

function initUservoice() {
    var uv = document.createElement('script'); uv.type = 'text/javascript'; uv.async = true;
    uv.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'widget.uservoice.com/39iNhpJPwlSkFDpX5Ajxw.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(uv, s);
}