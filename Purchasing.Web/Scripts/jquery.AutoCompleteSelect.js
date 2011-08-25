(function ($) {

    // attach this new method to jQuery
    $.fn.extend({

        autoCompleteSelect: function (options) {

            var settings = $.extend({

                useRemote: true,
                dataUrl: "",
                minLength: 2,
                placeholder: "",
                showOptions: false  // can only be used if useRemote = false

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
                else if (!settings.showOptions) {
                    attachLocalAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
                }
                // show all options as a drop down and do not attach auto complete
                else {
                    attachLocal($searchBox, $selectedContainer, $multiselect);
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

                        // get the values already selected
                        var selected = $multiselect.find("option:selected").map(function () { return $(this).val(); }).get();

                        $.getJSON(settings.dataUrl, { searchTerm: request.term, selected: selected }, function (data) {

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
                var options = $multiselect.find("option:not(:selected)");
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

            // attach a drop down to the text box
            function attachLocal($searchBox, $selectedContainer, $multiselect) {

                // initialize the options box
                var $optionsBox = $("<div>").addClass("ac-optionsbox").hide();
                var $optionsList = $("<ul>").addClass("ac-optionslist").addClass("ui-menu ui-autocomplete ui-widget ui-widget-content");

                $optionsBox.append($optionsList);
                $optionsBox.insertAfter($searchBox);

                // assign click on the search box
                $searchBox.click(function () {

                    // clear the options in there now
                    $optionsList.empty();

                    // refresh the results
                    var options = $multiselect.find("option:not(:selected)");
                    var available = $.map(options, function (item, index) {
                        var li = $("<li>").addClass("ui-menu-item");
                        var link = $("<a>").data("id", $(item).val()).html($(item).text()).addClass("ac-option");

                        li.hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); });

                        return li.append(link);
                    });

                    // put it into the box
                    $.each(available, function (index, item) {
                        $optionsList.append(item);
                    });

                    // show the box
                    $optionsBox.show();

                });

                // add click event for selecting an option
                $(".ac-option").live('click', function () {

                    var id = $(this).data('id');
                    var txt = $(this).html();

                    addSelected($selectedContainer, $multiselect, id, txt);

                    $optionsBox.hide();

                });

                // detect click outside of the two controls
                $('body').click(function (event) {

                    var $target = $(event.target);

                    if ($target.parents(".ac-optionsbox").length == 0 && !$target.hasClass("ac-searchBox")) {
                        $optionsBox.hide();
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