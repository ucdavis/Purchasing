var BrowserDetect = {
    init: function () {
        this.unsupported = false;
        this.browser = this.searchString(this.dataBrowser) || "unknown";
        if (this.browser === "Explorer") {
            this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "unknown";
            if (this.version < 9) {
                this.unsupported = true;
            }
        }

        if (this.browser === "Firefox") {
            this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "unknown";
            if (this.version < 6) {
                this.unsupported = true;
            }
        }
        if (this.browser === "Explorer11") {
            this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "unknown";
            if (this.version === "unknown" || this.version < 11) {
                this.unsupported = true;
            }
        }

        if (this.browser === "unknown") {
            this.unsupported = true;
        }
    },
    searchString: function (data) {
        for (var i = 0; i < data.length; i++) {
            var dataString = data[i].string;
            var dataProp = data[i].prop;
            this.versionSearchString = data[i].versionSearch || data[i].identity;
            if (dataString) {
                if (dataString.indexOf(data[i].subString) != -1)
                    return data[i].identity;
            }
            else if (dataProp)
                return data[i].identity;
        }
    },
    searchVersion: function (dataString) {

        var index = dataString.indexOf(this.versionSearchString);
        if (index == -1) return;
        return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
    },
    dataBrowser: [
		{
		    string: navigator.userAgent,
		    subString: "Chrome",
		    identity: "Chrome"
		},
		{
		    string: navigator.vendor,
		    subString: "Apple",
		    identity: "Safari",
		    versionSearch: "Version"
		},
		{
		    prop: window.opera,
		    identity: "Opera",
		    versionSearch: "Version"
		},
		{
		    string: navigator.userAgent,
		    subString: "Firefox",
		    identity: "Firefox"
		},
		{
		    string: navigator.userAgent,
		    subString: "MSIE",
		    identity: "Explorer",
		    versionSearch: "MSIE"
		},
        {
            //"Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; InfoPath.3; .NET4.0E; MS-RTC LM 8; rv:11.0) like Gecko"
            string: navigator.userAgent,
            subString: "Trident/7.0",
            identity: "Explorer11",
            versionSearch: "rv"
        }
	]
};
BrowserDetect.init();