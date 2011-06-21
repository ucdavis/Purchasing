if(!HelpSpotWidget) {
	var HelpSpotWidget = {}
}

HelpSpotWidget.Util = {
	render: function(template, params) {
		return template.replace(/\#{([^{}]*)}/g,
			function (a, b) {
				var r = params[b]
				return typeof r === 'string' || typeof r === 'number' ? r : a
			}
		)
	},
	toQueryString: function(values) {
		var pairs = []
		for (key in values) { 
			if (values[key] != null && values[key] != '' && key.substring(0,2) != 'on') { //search for on prevents callbacks from being passed through
				pairs.push([key, escape(values[key])].join('='));
			}
		}
		return pairs.join('&')
	}	
}

HelpSpotWidget.Tab = {
	id: "helpspot-widget-tab",

	css_template: "body a##{id}, body a##{id}:link { #{alignment}: 0; top: #{top} !important; margin-#{alignment}: -5px !important; background-position: top left !important; background-color: #{background_color}; background-image: url(#{tabimage}); }" +
				  " body a##{id}:hover { background-color: #{hover_color};margin-#{alignment}: -3px !important; }" +
				  " * html a##{id}, * html a##{id}:link { filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='#{tabimage}'); }" +
				  " #helpspot_widget_tab_close img { filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='#{closeimage}'); cursor:pointer;}" +
				  " #helpspot_widget_tab_overlay {top:0; left:0; width: 100%; height: 100%; position: fixed; _position: absolute; z-index: 100002;} " + 
				  " #helpspot_widget_tab_overlay * { margin: 0; padding: 0; font-family: 'Lucida Grande', 'Lucida Sans Unicode', sans-serif; font-style:normal; font-variant:normal;} " +
				  " #helpspot_widget_mask { top:0; left:0; z-index:1; width: 100%; position: absolute; background-color: #{overlay_color}; opacity: 0.45; filter:alpha(opacity=45); -moz-opacity: 0.45;}" + 
				  " #helpspot_widget_tab_loading {background-color: #{popup_background_color}; border-style: solid; border-color: #{popup_border_color}; border-width: #{popup_border_size}; }",
	
	setupOptions: function(options) {
	 	this.options = {
			alignment: 'left',
			tabtype: 'questions',
			tabtype_custom_img: '',
			top: '30%',
			width: 600,
			host: '/', 
			color: 'white',
			background_color: '#222222',
			hover_color: '#222',
			popup_background_color: '#fff',
			popup_border_color: '#ccc',
			popup_border_size: '10px',
			overlay_color: '#000',
			default_note: '',
			default_name: '',
			default_email: '',
			use_field_name: false,
			text_header: 'How can we help?',
			text_intro: 'Submit your question/comment for a member of our team.',
			text_note: 'Question',
			text_note_er: 'Please provide some information on your request',
			text_email: 'Your Email',
			text_email_er: 'Please provide your email address',
			text_name: 'Your Name',
			text_name_er: 'Please provide your name',
			text_submit: 'Submit',
			text_msg_submit: 'Message sent, thank you.',
			text_msg_submit_error: 'Sorry, there was an error.',
			text_msg_submit_error_link: 'Please try this form',
			text_msg_submit_error_url: '',
			text_loading: 'Loading...',
			text_special: '',
			onLoad: false,
			onClose: false		
	 	}
    	for(attr in options){ this.options[attr] = options[attr] }
    	this.options.id = this.id;
    	this.options.closeimage = this.options.host + '/widgets/images/close.png';
    	this.options.width = (this.options.width ? this.options.width : 600);
    	this.options.text_msg_submit_error_url = (this.options.text_msg_submit_error_url ? this.options.text_msg_submit_error_url : this.options.host + '/index.php?pg=request');
    	
    	if(this.options.tabtype_custom_img){
    		this.options.tabimage = this.options.tabtype_custom_img;
    	}else{
    		this.options.tabimage = this.options.host + '/widgets/images/tab/' + this.options.tabtype + '-' + this.options.color + '.png';
    	}
    },
	show: function(options){
		this.setupOptions(options || {});
		
		document.write('<a id="' + this.id + '" onclick="HelpSpotWidget.Tab.open(); return false;" href=""></a>');	
		document.write('<style type="text/css">' + HelpSpotWidget.Util.render(this.css_template, this.options) + '</style>');
	},
	open: function(){
		if(!document.getElementById('helpspot_widget_tab_overlay')){
			var overlay = document.createElement('div');
			overlay.id = 'helpspot_widget_tab_overlay';
			overlay.style.display = 'none';
			overlay.innerHTML = '<div id="helpspot_widget_tab_wrapper">' + 
					'<div id="helpspot_widget_tab_close"><img src="' + this.options.closeimage + '" onclick="HelpSpotWidget.Tab.close();return false" /></div>' + 
					'<div id="helpspot_widget_tab_loading">' + this.options.text_loading + '</div>' + 
					'<iframe src="' + this.options.host + '/widgets/tab.php?' + HelpSpotWidget.Util.toQueryString(this.options) + '&rand=' + Math.floor(Math.random()*1000) + '" name="helpspot_widget_tab_iframe" id="helpspot_widget_tab_iframe" allowTransparency="true" scrolling="no" frameborder="0" class=""></iframe>' +
				'</div>' + 
				'<div id="helpspot_widget_mask" onclick="HelpSpotWidget.Tab.close();return false"></div>';
			
			//Append overlay
			document.body.appendChild(overlay);
			
			//Add event for iframe load
			if(document.getElementById('helpspot_widget_tab_iframe').attachEvent){ //IE
				document.getElementById('helpspot_widget_tab_iframe').attachEvent("onload", HelpSpotWidget.Tab.ready);
			}else if (document.getElementById('helpspot_widget_tab_iframe').addEventListener){
				document.getElementById('helpspot_widget_tab_iframe').addEventListener("load", HelpSpotWidget.Tab.ready, false);
			}			
	
			//Adjust locations
			document.getElementById('helpspot_widget_mask').style.height = HelpSpotWidget.page.theight() + "px";
			document.getElementById('helpspot_widget_mask').style.width = HelpSpotWidget.page.twidth() + "px";
			document.getElementById('helpspot_widget_tab_wrapper').style.top =  (0.10 * HelpSpotWidget.page.height()) + "px";
		}
		
		//Show mask
		document.getElementById('helpspot_widget_tab_overlay').style.display = 'block';
	},
	ready: function(){
		document.getElementById('helpspot_widget_tab_loading').style.display= "none";
		document.getElementById('helpspot_widget_tab_iframe').style.display= "block";
		
		//Do callback
		if(HelpSpotWidget.Tab.options.onLoad) HelpSpotWidget.Tab.options.onLoad();		
	},
	close: function(){
		//document.getElementById('helpspot_widget_tab_overlay').style.display = 'none';
		HelpSpotWidget.Tab.reset();
		
		//Do callback
		if(HelpSpotWidget.Tab.options.onClose) HelpSpotWidget.Tab.options.onClose();		
	},
	reset: function(){
		document.body.removeChild(document.getElementById('helpspot_widget_tab_overlay'));
	}
}

HelpSpotWidget.page=function(){
	return{
		top:function(){return document.body.scrollTop||document.documentElement.scrollTop},
		width:function(){return self.innerWidth||document.documentElement.clientWidth},
		height:function(){return self.innerHeight||document.documentElement.clientHeight},
		theight:function(){
			var d=document, b=d.body, e=d.documentElement;
			return Math.max(Math.max(b.scrollHeight,e.scrollHeight),Math.max(b.clientHeight,e.clientHeight))
		},
		twidth:function(){
			var d=document, b=d.body, e=d.documentElement;
			return Math.max(Math.max(b.scrollWidth,e.scrollWidth),Math.max(b.clientWidth,e.clientWidth))
		}
	}
}();