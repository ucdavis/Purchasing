(function (purchasing, $, undefined) {
    purchasing.loadTableTools = false;

    purchasing.startup = function () {
        $('input[type=checkbox]').tzCheckbox({ labels: ['Yes', 'No'] });

        if (window.Configuration.LoadTableTools) {
            var table = $(".dt-table, .datatable").dataTable({ "bJQueryUI": false,
                "sPaginationType": "full_numbers",
                "oLanguage": { "sSearch": "Filter Within These Results: " },
                "iDisplayLength": window.Configuration.DataTablesPageSize,
                "sDom": 'T<"clear">lfrtip',
                "oTableTools": {
                    "sSwfPath": window.Configuration.TableToolsSwf,
                    "aButtons": ["copy", "xls"]
                }
            });

            $.each($(".dataTables_wrapper"), function (index, item) { RearranngeDataTable($(item)); });

            if (window.Configuration.LoadFixedHeaders) {
                new FixedHeader(table);
            }
        } else {
            var table = $(".dt-table, .datatable").dataTable({ "bJQueryUI": false, "sPaginationType": "full_numbers", "oLanguage": { "sSearch": "Filter Within These Results: " }, "iDisplayLength": window.Configuration.DataTablesPageSize });

            if (window.Configuration.LoadFixedHeaders) {
                new FixedHeader(table);
            }
        }



        $(".button, .text_btn, button").button();
        $('input[type="datetime"]').datepicker();
        $('input.datepicker').datepicker();

        initBrowserDetect();
        konami(function () {
            $("#carty").show();
        });
    };


    
    function initBrowserDetect() {
        if (BrowserDetect.unsupported) {
            $("#browser-warning").show();
        }
    }

    function konami(callback) {
        var code = "38,38,40,40,37,39,37,39,66,65";
        var kkeys = [];

        $(window).on('keydown', function (e) {
            kkeys.push(e.keyCode);
            while (kkeys.length > code.split(',').length) {
                kkeys.shift();
            }
            if (kkeys.toString().indexOf(code) >= 0) {
                $(this).unbind('keydown', arguments.callee);
                callback(e);
            }
        });
    }
} (window.purchasing = window.purchasing || {}, jQuery));

if (typeof console == "undefined") { //Define the console as a backup for browsers without console
    window.console = {
        log: function () { }
    };
}

$(function () {//run the page load startup code
    purchasing.startup();
});