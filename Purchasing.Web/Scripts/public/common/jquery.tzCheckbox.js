(function ($) {
    $.fn.tzCheckbox = function (options) {

        // Default On / Off labels:

        options = $.extend({
            labels: ['ON', 'OFF']
        }, options);

        return this.each(function () {
            var originalCheckBox = $(this),
				labels = [];

            // Checking for the data-on / data-off HTML5 data attributes:
            if (originalCheckBox.data('on')) {
                labels[0] = originalCheckBox.data('on');
                labels[1] = originalCheckBox.data('off');
            }
            else labels = options.labels;

            // Creating the new checkbox markup:
            var checkBox = $('<span>', {
                "class": 'tzCheckBox ' + (this.checked ? 'checked' : ''),
                html: '<span class="tzCBContent">' + labels[this.checked ? 0 : 1] +
						'</span><span class="tzCBPart"></span>'
            });

            // -- Original Code, just hides the checkbox -- //
            // Inserting the new checkbox, and hiding the original:
            //checkBox.insertAfter(originalCheckBox.hide());

            // -- Replacement code to try to get keyboard changes working -- //
            var hidden = originalCheckBox.siblings("input[type='hidden'][name='" + originalCheckBox.attr("id") + "']");

            // put the checkbox and everything into a container
            var container = $("<span>", { "class": 'tzCheckBox-Container' });
            container.append(checkBox);
            container.insertAfter(originalCheckBox);

            // move the original checkbox controls
            container.append(originalCheckBox);
            container.append(hidden);


            checkBox.click(function () {
                checkBox.toggleClass('checked');

                var isChecked = checkBox.hasClass('checked');

                // Synchronizing the original checkbox:
                originalCheckBox.attr('checked', isChecked);
                checkBox.find('.tzCBContent').html(labels[isChecked ? 0 : 1]);

                originalCheckBox.trigger('click');

                originalCheckBox.attr('checked', isChecked);
                checkBox.find('.tzCBContent').html(labels[isChecked ? 0 : 1]);
            });

            // Trigger 'checked' if the original changes & the new one will be updated
            originalCheckBox.bind('checked', function () {
                checkBox.click();
            });

            originalCheckBox.change(function () { checkBox.click(); });

            originalCheckBox.focusin(function () { checkBox.addClass("focus"); container.find(".tzCBPart").addClass("focus"); }).focusout(function () { checkBox.removeClass("focus"); container.find(".tzCBPart").removeClass("focus"); });

        });
    };
})(jQuery);