(function ($) {

    // attach this new method to jQuery
    $.fn.extend({

        autoCompleteSelect: function (options) {

            var settings = $.extend({

                useRemote: true,
                dataUrl: "",
                minLength: 2,
                placeholder: ""

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
                var $searchBox = $("<input>").addClass("ac-searchBox").attr("placeholder", settings.placeholder);
                var $loadingIcon = $("<div>").html("&nbsp;").addClass("ac-loading").hide();
                var $selectedContainer = $("<div>").addClass("ac-selectedContainer");

                // append the controls
                $searchContainer.append($searchBox);
                $searchContainer.append($loadingIcon);
                $container.append($searchContainer);
                $container.append($selectedContainer);

                $container.append($("<div>").css("clear", "both"));

                setupSelected($multiselect, $selectedContainer);

                // rely on remote call to populate
                if (settings.useRemote) {
                    attachJsonAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
                }
                // rely on data inside select list
                else {
                    attachLocalAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
                }

            }

            // attach the autocomplete into the textbox
            function attachJsonAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect) {

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

            // attach the auto complete without the need for a remote data source
            function attachLocalAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect) {

                // build the list of available
                var options = $multiselect.find("option(:not(:selected))");
                var available = $.map(options, function (item, index) { return { label: $(item).html(), value: $(item).val()} });

                $searchBox.autocomplete({

                    source: available,
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

                $.each($multiselect.find("option:selected"), function (index, item) {

                    addSelected($container, $multiselect, $(item).val(), $(item).html());

                });


            }

            // Add selected values into the display list
            function addSelected($container, $multiselect, id, label) {

                // create the objects
                var $selected = $("<span>").addClass("ac-selected").data("id", id).addClass("ac-gradientbackground");
                $selected.append($("<span>").html(label).css("display", "inline-block"));
                $selected.append($("<span>").addClass("ui-icon ui-icon-circle-close").css("margin", "-3px 4px").css("display", "inline-block"));
                $selected.data("id", id);

                // check if the option exists yet
                var $selectedOption = $multiselect.find('option[value="' + id + '"]');

                if ($selectedOption.length < 1) {
                    // add the value into the multiselect control
                    $multiselect.append($("<option>").val(id).html(label).attr("selected", ""));
                }
                else {
                    $selectedOption.attr("selected", "");
                }

                var $selectedDisplayObj = $container.children().filter(function (index) { return $(this).data("id") == id; });

                // do not add the object if it already exists
                if ($selectedDisplayObj.length < 1) {
                    // add to the selected container
                    $container.append($selected);

                    // attach a click event
                    $selected.click(function () {

                        // remove the object from the multiselect
                        $multiselect.find('option[value="' + id + '"]').removeAttr("selected");

                        // remove from the container
                        $(this).remove();

                    });
                }
            }

        } // end of the auto complete select

    }); // end of $.fn.extend


})(jQuery);