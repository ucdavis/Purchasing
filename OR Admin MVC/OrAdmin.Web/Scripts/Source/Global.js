
/* Helpspot widget
-------------------------------------------------------------------*/
try {
    HelpSpotWidget.Tab.show({
        host: 'https://iss-help.ucdavis.edu'
    });
}
catch (ex) { }

function ShowHelpSpotWidget() {
    try {
        HelpSpotWidget.Tab.open();
    }
    catch (ex) {
        window.location = "mailto:iss-help@ucdavis.edu";
    }
}

/* Control buttons
-------------------------------------------------------------------*/
$(function () {
    $('.sprite a, .sprite img').live('click', function () {
        if ($(this).parent().find('input').size() > 0)
            $(this).parent().find('input').click();
        else
            document.location = $(this).parent().find('a').attr('href');
    });
});

/* Progressive enhancements
-------------------------------------------------------------------*/
$(function () {
    $('table.simple').find('tr:last').addClass('last');
});

// Esc key
$(document).keyup(function (e) {
    if (e.keyCode == 27) {
        $('.t-window').each(function () {
            $('#' + $(this).attr('id')).data('tWindow').close();
        });
    }   
});

/* Equal Height
-------------------------------------------------------------------*/
$(function () {
    (function ($) {
        $.fn.eqHeight = function (options) {
            var config = { 'height': 'height' };
            var o = $.extend(config, options);
            var tallest = 0;
            this.each(function () {
                if ($(this).height() > tallest) {
                    if (o.height === 'height')
                        tallest = $(this).height();
                    else if (o.height === 'inner')
                        tallest = $(this).innerHeight();
                    else if (o.height === 'outer')
                        tallest = $(this).outerHeight();
                }
            });
            this.each(function () {
                $(this).css('height', tallest + 'px');
            });
            return this;
        };
    })(jQuery);
});

/* Session defibrillator
-------------------------------------------------------------------*/
var defibrillator;
window.onload = Defibrillator;
function Defibrillator() {
    defibrillator = window.setInterval(XMLHttpPost, 60000);
}

function XMLHttpPost() {
    var oReq = getXMLHttpRequest();
    if (oReq != null) {
        try {
            var dt = new Date();
            var d = dt.getDate();
            var m = dt.getMonth();
            var y = dt.getFullYear();
            var h = dt.getHours();
            var m = dt.getMinutes();

            oReq.open("POST", "/home/defibrillator?" + m + d + y + h + m, false);
            oReq.send();
        }
        catch (ex) {
            window.alert('Could not keep your session alive. Any data entered into the current page may be lost.');
            return null;
        }
    }
    else {
        window.alert("AJAX (XmlHTTP) not supported! You must use a modern web browser.");
    }
}

function getXMLHttpRequest() {
    if (window.XMLHttpRequest) {
        return new window.XMLHttpRequest;
    }
    else {
        try {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        }
        catch (ex) {
            return null;
        }
    }
}