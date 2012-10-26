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

        //loadActiveIssues(); //TODO: removing active issue loading, at least until we use it for issues
        initUservoice();
        initBrowserDetect();
        konami(function () {
            $("#carty").show();
        });
    };

    function loadActiveIssues() {
        $.get(window.Configuration.ActiveIssuesCountUrl, null, function (result) {
            if (result.HasIssues) {
                $("#help-link").html("Help (" + result.IssuesCount + ")").css("color", "red");
            }
        });
    }

    function initUservoice() {
        var uv = document.createElement('script'); uv.type = 'text/javascript'; uv.async = true;
        uv.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'widget.uservoice.com/39iNhpJPwlSkFDpX5Ajxw.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(uv, s);
    }
    
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