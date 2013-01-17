/*
* stickyfloat - jQuery plugin for verticaly floating anything in a constrained area
* 
* Example: jQuery('#menu').stickyfloat({duration: 400});
* parameters:
* 		duration 		(number, 200) - the duration of the animation
*		startOffset 	(number) - the amount of scroll offset after the animations kicks in
*		offsetY			(number) - the offset from the top when the object is animated
*		lockBottom		(boolean, true)	 - set to false if you don't want your floating box to stop at parent's bottom
*		delay			(0) - delay in milliseconds  until the animnations starts
easing			(linear) - easing function (jQuery has by default only 'swing' & 'linear')
stickToBottom 	(boolean, false) - to make the element stick to the bottom instead to the top
* $Version: 12.02.2012 r4
* $Version: 11.28.2011 r3
* $Version: 08.10.2011 r2
* $Version: 05.16.2009 r1
* Copyright (c) 2012 Yair Even-Or
* vsync.design@gmail.com
*/
(function ($) {
	$.fn.stickyfloat = function (options) {
		var $obj = this,
			doc = $(document),
			parentPaddingTop = parseInt($obj.parent().css('padding-top')),
			startOffset = $obj.parent().offset().top,
			opts, bottomPos, pastStartOffset, objFartherThanTopPos, objBiggerThanWindow, newpos, checkTimer, lastDocPos = doc.scrollTop();

		$.extend($.fn.stickyfloat.opts, options, { startOffset: startOffset, offsetY: parentPaddingTop });
		opts = $.fn.stickyfloat.opts;
		$obj.css({ position: 'absolute' });

		function checkLockBottom() {
			if (opts.lockBottom) {
				bottomPos = $obj.parent().height() - $obj.outerHeight() + parentPaddingTop; //get the maximum scrollTop value
				if (bottomPos < 0)
					bottomPos = 0;
			}
		};

		function checkScroll() {
			checkLockBottom();
			if (opts.duration > 40) {
				clearTimeout(checkTimer);
				checkTimer = setTimeout(function () {
					//if( Math.abs(doc.scrollTop() - lastDocPos) > 0 ){
					lastDocPos = doc.scrollTop();
					initFloat();
					//}
				}, 40);
			}
			else initFloat();
		}

		function initFloat() {
			$obj.stop(); // stop all calculations on scroll event

			pastStartOffset = doc.scrollTop() > opts.startOffset; // check if the window was scrolled down more than the start offset declared.
			objFartherThanTopPos = $obj.offset().top > startOffset; // check if the object is at it's top position (starting point)
			objBiggerThanWindow = $obj.outerHeight() < $(window).height(); // if the window size is smaller than the Obj size, do not animate.

			// if window scrolled down more than startOffset OR obj position is greater than
			// the top position possible (+ offsetY) AND window size must be bigger than Obj size
			if ((pastStartOffset || objFartherThanTopPos) && objBiggerThanWindow) {
				newpos = opts.stickToBottom ? doc.scrollTop() + $(window.top).height() - $obj.outerHeight() - startOffset - opts.offsetY : (doc.scrollTop() - startOffset + opts.offsetY);

				if (newpos > bottomPos)
					newpos = bottomPos;
				// if window scrolled < starting offset, then reset Obj position (opts.offsetY);
				if (doc.scrollTop() < opts.startOffset && !opts.stickToBottom)
					newpos = parentPaddingTop;

				$obj.delay(opts.delay).animate({ top: newpos }, opts.duration, opts.easing);
			}
		}

		$(window).bind('scroll.sticky', checkScroll);
	};

	$.fn.stickyfloat.opts = { duration: 200, lockBottom: true, delay: 0, easing: 'linear', stickToBottom: false };
})(jQuery);