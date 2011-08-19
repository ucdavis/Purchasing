(function ($) {

    // attach this new method to jQuery
    $.fn.extend({

        autoCompleteSelect: function (options) {

            var settings = $.extend({

                dataUrl: "",
                minLength: 2

            }, options); // end of the extend

            return this.each(function (index, item) {

                // the multiselect with the selected values
                var $multiselect = $(item);
                $multiselect.hide();

                // the container that displays to the user
                var $container = $("<div>").addClass("ac-container").addClass("ac-gradientbackground");

                // setup the search box and associated controls
                setupSearch($container, $multiselect);

                // setup the container that houses the selected items

                // insert the container after the original multiselect
                $container.insertAfter($multiselect);

            });

            // initializes all the controls
            function setupSearch($container, $multiselect) {

                // create the controls
                var $searchContainer = $("<div>").addClass("ac-searchContainer");
                var $searchBox = $("<input>").addClass("ac-searchBox");
                var $loadingIcon = $("<div>").html("&nbsp;").addClass("ac-loading").hide();
                var $selectedContainer = $("<div>").addClass("ac-selectedContainer");

                // append the controls
                $searchContainer.append($searchBox);
                $searchContainer.append($loadingIcon);
                $container.append($searchContainer);
                $container.append($selectedContainer);

                $container.append($("<div>").css("clear", "both"));
                
                setupSelected($multiselect, $selectedContainer);
                attachAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
            }

            // attach the autocomplete into the textbox
            function attachAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect) {

                $searchBox.autocomplete({

                    minLength: settings.minLength,
                    source: function (request, response) {

                        // show the loading icon
                        $loadingIcon.show();
                        $loadingIcon.css("display", "inline-block");

                        $.getJSON(settings.dataUrl, { searchTerm: request.term }, function (data) {

                            // set the values to display
                            response($.map(data, function (item, index) { return { label: item.Label, value: item.Id }; }));

                            // hide the loading icon
                            $loadingIcon.hide();

                        });

                    },
                    select: function (event, ui) {

                        addSelected($selectedContainer, $multiselect, ui.item.value, ui.item.label);

                    },
                    close: function (event, ui) {

                        $searchBox.val("");

                    }

                });

            }

            // setup the already selected on population
            function setupSelected($multiselect, $container) {

                $.each($multiselect.find("option:checked"), function (index, item) {

                    addSelected($container, $multiselect, $(item).val(), $(item).html());

                });


            }

            // Add selected values into the display list
            function addSelected($container, $multiselect, id, label) {

                // create the objects
                var $selected = $("<span>").addClass("ac-selected").data("id", id).addClass("ac-gradientbackground");
                $selected.append($("<span>").html(label).css("display", "inline-block"));
                $selected.append($("<span>").addClass("ui-icon ui-icon-circle-close").css("margin", "-3px 4px").css("display", "inline-block"));

                // add the value into the multiselect control
                $multiselect.append($("<option>").val(id).html(label).attr("selected", ""));

                // add to the selected container
                $container.append($selected);

                // attach a click event
                $selected.click(function () {

                    // remove the object from the multiselect
                    $multiselect.find('option[value="' + id + '"]').remove();

                    // remove from the container
                    $(this).remove();

                });
            }

        } // end of the auto complete select

    }); // end of $.fn.extend


})(jQuery);