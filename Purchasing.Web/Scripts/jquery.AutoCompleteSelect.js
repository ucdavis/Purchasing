/* 

Jquery Ajax enabled Multiselect control

Written By: Alan Lai
Date:       8/25/2011

Parameters:

dataUrl:    url to a Json feed that returns objects {id = [id value], label = [label value]}
minLength:  minimum length before a search is executed
placeholder:placeholder text for the search box

Callbacks:

onSelected:     event raised when a new item is selected
onRemoved:     event raised when a selected item is removed

*/

(function ($) {

    $.fn.extend({

        multiSelector: function (options) {

            var settings = $.extend({

                dataUrl: '',
                minLength: 2,
                placeholder: '',
                debug: false,
                showOptions: false,

                onSelected: undefined,
                onRemove: undefined

            }, options); // end of settings

            return this.each(function (index, item) {

                // get the main controls
                var $multiselect = $(this);
                $multiselect.hide();
                var $container = $("<div>").addClass("ac-container").addClass("ac-gradientbackground");
                $container.insertAfter($multiselect);

                // initialize all the necessary controls and containers
                init($multiselect, $container);
                bindEvents($multiselect, $container);

                preloadItems($multiselect, $container);

            });

            // initialize the controls
            function init($multiselect, $container) {

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

                // initialize the search results box
                var $optionsBox = $("<div>").addClass("ac-optionsbox").hide();
                var $optionsList = $("<ul>").addClass("ac-optionslist").addClass("ui-menu ui-autocomplete ui-widget ui-widget-content");
                $optionsBox.append($optionsList);
                $optionsBox.insertAfter($searchBox);

                $container.append($("<div>").css("clear", "both"));

                if (settings.debug) initDebug($container);

                configureSearch($multiselect, $container);
            }

            function preloadItems($multiselect, $container) {

                var selected = $multiselect.find("option:selected");
                $.each(selected, function (index, item) {

                    var id = $(item).val();
                    var label = $(item).text();
                    onSelected($container, $multiselect, id, label);

                });

            }

            function bindEvents($multiselect, $container) {

                // detect click outside of our controls
                $('body').click(function (event) {

                    var $target = $(event.target);

                    if ($target.parents(".ac-optionsbox").length == 0 && !$target.hasClass("ac-searchBox")) {
                        $container.find(".ac-optionsbox").hide();
                    }

                });

                // capture down arrow from the textbox
                $container.find(".ac-searchBox").keydown(function (event) {

                    // get the options box next to the search box
                    var $optionBox = $(this).siblings(".ac-optionsbox");

                    // up/down and at least one item is eligible to be selected
                    if ((event.keyCode == 40 || event.keyCode == 38) && $optionBox.find("a").length > 0) {

                        var $selected = $optionBox.find(".ui-state-hover");
                        // any of them have a ui-state-hover
                        if ($selected.length > 0) {

                            // scroll up one
                            if (event.keyCode == 38) {

                                $selected.prev().addClass("ui-state-hover");
                                $selected.removeClass("ui-state-hover");

                            }

                            // scroll down one
                            if (event.keyCode == 40) {

                                $selected.next().addClass("ui-state-hover");
                                $selected.removeClass("ui-state-hover");

                            }

                        }
                        // nothing selected in the options box
                        else {
                            $optionBox.find("li").removeClass("ui-state-hover");

                            // set the first one to be highlighted
                            $optionBox.find("li:first").addClass("ui-state-hover");
                        }

                    }

                    // enter key
                    if (event.keyCode == 13) {

                        var $selected = $optionBox.find(".ui-state-hover a");
                        onSelected($container, $multiselect, $selected.data("id"), $selected.html());

                        $optionBox.hide();
                    }

                });

            }

            function initDebug($container) {

                var $debugList = $("<ul>").addClass("ac-debug-list");

                $debugList.append($("<li>").addClass("ac-debug-item").html("Term Length:").append($("<span>").addClass("ac-debug-text")));

                var $debugContainer = $("<div>").addClass("ac-debug-container");
                $debugContainer.append($debugList);
                $container.append($debugContainer);

            }

            // =================================================
            // search events on the text box
            // =================================================
            function configureSearch($multiselect, $container) {

                var $searchBox = $container.find(".ac-searchBox");

                $searchBox.keyup(function (event) {

                    if (event.keyCode != 38 && event.keyCode != 40 && event.keyCode != 39 && event.keyCode != 41) {

                        var searchTerm = $searchBox.val();

                        if (settings.debug) { $container.find(".ac-debug-termLength").html('[' + searchTerm + ']' + searchTerm.length); }

                        if (searchTerm.length >= settings.minLength) {

                            onSearch(searchTerm, $multiselect, $container);

                        }
                    }

                });

                if (settings.showOptions) {
                    // show all
                    $searchBox.click(function () {

                        var results = $multiselect.find("option:not(:selected)");
                        var available = $.map(results, function (item, index) {

                            return { id: $(item).val(), label: $(item).text() };

                        });

                        displaySearchResults(available, $container, $multiselect);

                    });
                }


            }
            // =================================================

            // event when a search is being executed
            function onSearch(searchTerm, $multiselect, $container) {

                // execute remote search
                if (settings.dataUrl != '') {
                    remoteSearch(searchTerm, $multiselect, $container);
                }
                // execute a local search
                else {
                    localSearch(searchTerm, $multiselect, $container);
                }

            }

            // execute a search against the remote datasource
            function remoteSearch(searchTerm, $multiselect, $container) {

                $.getJSON(settings.dataUrl, { searchTerm: searchTerm }, function (results) {

                    displaySearchResults(results, $container, $multiselect);

                });

            }

            // execute a search against the multiselect values
            function localSearch(searchTerm, $multiselect, $container) {

                // find the items in the multiselect
                var results = $multiselect.find("option").filter(function () {

                    var id = $(this).val().toLowerCase();
                    var txt = $(this).text().toLowerCase();

                    return (id.indexOf(searchTerm) >= 0 || txt.indexOf(searchTerm) >= 0);

                });

                var available = $.map(results, function (item, index) {

                    return { Id: $(item).val(), Label: $(item).text() };

                });

                displaySearchResults(available, $container, $multiselect);
            }

            function displaySearchResults(results, $container, $multiselect) {

                // check if the result box is visible (show if not)
                var $optionsBox = $container.find(".ac-optionsbox");
                if (!$optionsBox.is(":visible")) { $optionsBox.show(); }

                var $optionsList = $optionsBox.find(".ac-optionslist");
                $optionsList.empty();

                $.each(results, function (index, item) {

                    var link;
                    if (item.id == undefined) {
                        link = $("<a>").data("id", item.Id).html(item.Label).addClass("ac-option");
                    }
                    if (item.Id == undefined) {
                        link = $("<a>").data("id", item.id).html(item.label).addClass("ac-option");
                    }

                    link.click(function () {

                        var id = $(this).data("id");
                        var label = $(this).html();

                        onSelected($container, $multiselect, id, label);
                        $container.find(".ac-optionsbox").hide();

                    });

                    var li = $("<li>").addClass("ui-menu-item");
                    li.hover(
                    // mouse over
                        function () {
                            $(this).siblings("li.ui-menu-item").removeClass("ui-state-hover");
                            $(this).addClass("ui-state-hover");
                        },
                    // mouse out
                        function () {
                            $(this).removeClass("ui-state-hover");
                        });
                    li.append(link);

                    $optionsList.append(li);

                });

                if ($optionsList.children().length == 0) {
                    $optionsList.append($("<li>").html("No results found."));
                }


            }

            // event when an object is selected to the list
            function onSelected($container, $multiselect, id, label) {

                $container = $container.find(".ac-selectedContainer");

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

                if (settings.onSelected != undefined) {
                    settings.onSelected(id, label);
                }

            }

            // event when an object is removed from the list
            function onRemoved() { }

        } // end of multiSelector

    }); // end of fn.extend


})(jQuery);



//(function ($) {

//    // attach this new method to jQuery
//    $.fn.extend({

//        autoCompleteSelect: function (options) {

//            var settings = $.extend({

//                useRemote: true,
//                dataUrl: "",
//                minLength: 2,
//                placeholder: "",
//                showOptions: false  // can only be used if useRemote = false

//            }, options); // end of the extend

//            return this.each(function (index, item) {

//                // the multiselect with the selected values
//                var $multiselect = $(item);
//                $multiselect.hide();

//                // the container that displays to the user
//                var $container = $("<div>").addClass("ac-container").addClass("ac-gradientbackground");

//                // setup the search box and associated controls
//                setupSearch($container, $multiselect);

//                // setup the container that houses the selected items

//                // insert the container after the original multiselect
//                $container.insertAfter($multiselect);

//            });

//            // initializes all the controls
//            function setupSearch($container, $multiselect) {

//                // create the controls
//                var $searchContainer = $("<div>").addClass("ac-searchContainer");
//                var $searchBox = $("<input>").addClass("ac-searchBox").attr("placeholder", settings.placeholder);
//                var $loadingIcon = $("<div>").html("&nbsp;").addClass("ac-loading").hide();
//                var $selectedContainer = $("<div>").addClass("ac-selectedContainer");

//                // append the controls
//                $searchContainer.append($searchBox);
//                $searchContainer.append($loadingIcon);
//                $container.append($searchContainer);
//                $container.append($selectedContainer);

//                $container.append($("<div>").css("clear", "both"));

//                setupSelected($multiselect, $selectedContainer);

//                // rely on remote call to populate
//                if (settings.useRemote) {
//                    attachJsonAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
//                }
//                // rely on data inside select list
//                else if (!settings.showOptions) {
//                    attachLocalAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect);
//                }
//                // show all options as a drop down and do not attach auto complete
//                else {
//                    attachLocal($searchBox, $selectedContainer, $multiselect);
//                }

//            }

//            // attach the autocomplete into the textbox
//            function attachJsonAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect) {

//                $searchBox.autocomplete({

//                    minLength: settings.minLength,
//                    source: function (request, response) {

//                        // show the loading icon
//                        $loadingIcon.show();
//                        $loadingIcon.css("display", "inline-block");

//                        // get the values already selected
//                        var selected = $multiselect.find("option:selected").map(function () { return $(this).val(); }).get();

//                        $.getJSON(settings.dataUrl, { searchTerm: request.term, selected: selected }, function (data) {

//                            // set the values to display
//                            response($.map(data, function (item, index) { return { label: item.Label, value: item.Id }; }));

//                            // hide the loading icon
//                            $loadingIcon.hide();

//                        });

//                    },
//                    select: function (event, ui) {

//                        addSelected($selectedContainer, $multiselect, ui.item.value, ui.item.label);

//                    },
//                    close: function (event, ui) {

//                        $searchBox.val("");

//                    }

//                });

//            }

//            // attach the auto complete without the need for a remote data source
//            function attachLocalAutoComplete($searchBox, $loadingIcon, $selectedContainer, $multiselect) {

//                // build the list of available
//                var options = $multiselect.find("option:not(:selected)");
//                var available = $.map(options, function (item, index) { return { label: $(item).html(), value: $(item).val()} });

//                $searchBox.autocomplete({

//                    source: available,
//                    select: function (event, ui) {

//                        addSelected($selectedContainer, $multiselect, ui.item.value, ui.item.label);

//                    },
//                    close: function (event, ui) {

//                        $searchBox.val("");

//                    }
//                });
//            }

//            // attach a drop down to the text box
//            function attachLocal($searchBox, $selectedContainer, $multiselect) {

//                // initialize the options box
//                var $optionsBox = $("<div>").addClass("ac-optionsbox").hide();
//                var $optionsList = $("<ul>").addClass("ac-optionslist").addClass("ui-menu ui-autocomplete ui-widget ui-widget-content");

//                $optionsBox.append($optionsList);
//                $optionsBox.insertAfter($searchBox);

//                // assign click on the search box
//                $searchBox.click(function () {

//                    // clear the options in there now
//                    $optionsList.empty();

//                    // refresh the results
//                    var options = $multiselect.find("option:not(:selected)");
//                    var available = $.map(options, function (item, index) {
//                        var li = $("<li>").addClass("ui-menu-item");
//                        var link = $("<a>").data("id", $(item).val()).html($(item).text()).addClass("ac-option");

//                        li.hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); });

//                        return li.append(link);
//                    });

//                    // put it into the box
//                    $.each(available, function (index, item) {
//                        $optionsList.append(item);
//                    });

//                    // show the box
//                    $optionsBox.show();

//                });

//                // simulate the autocomplete
//                $searchBox.keypress(function () {

//                    var searchTerm = $(this).val();

//                    // if the person typed more than 2 characters, execute a "filter"
//                    if (searchTerm.length > 1) {

//                        $optionsList.empty();

//                        var options = searchForTerm($multiselect, searchTerm);

//                        var available = $.map(options, function (item, index) {
//                            var li = $("<li>").addClass("ui-menu-item");
//                            var link = $("<a>").data("id", $(item).val()).html($(item).text()).addClass("ac-option");

//                            li.hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); });

//                            return li.append(link);
//                        });

//                        // put it into the box
//                        $.each(available, function (index, item) {
//                            $optionsList.append(item);
//                        });
//                    }

//                });

//                // add click event for selecting an option
//                $(".ac-option").live('click', function () {

//                    var id = $(this).data('id');
//                    var txt = $(this).html();

//                    addSelected($selectedContainer, $multiselect, id, txt);

//                    $optionsBox.hide();

//                });

//                // detect click outside of the two controls
//                $('body').click(function (event) {

//                    var $target = $(event.target);

//                    if ($target.parents(".ac-optionsbox").length == 0 && !$target.hasClass("ac-searchBox")) {
//                        $optionsBox.hide();
//                    }

//                });

//            }

//            // search for the term in the txt and id
//            function searchForTerm($multiselect, searchTerm) {

//                var options = $multiselect.find("option:not(:selected)");

//                var filtered = options.filter(function () {

//                    var id = $(this).val();
//                    var txt = $(this).html();

//                    return id.indexOf(searchTerm) > 0 || txt.indexOf(searchTerm) > 0;

//                });

//                return filtered;
//            }

//            // setup the already selected on population
//            function setupSelected($multiselect, $container) {

//                $.each($multiselect.find("option:selected"), function (index, item) {

//                    addSelected($container, $multiselect, $(item).val(), $(item).html());

//                });


//            }

//            // Add selected values into the display list
//            function addSelected($container, $multiselect, id, label) {

//                // create the objects
//                var $selected = $("<span>").addClass("ac-selected").data("id", id).addClass("ac-gradientbackground");
//                $selected.append($("<span>").html(label).css("display", "inline-block"));
//                $selected.append($("<span>").addClass("ui-icon ui-icon-circle-close").css("margin", "-3px 4px").css("display", "inline-block"));
//                $selected.data("id", id);

//                // check if the option exists yet
//                var $selectedOption = $multiselect.find('option[value="' + id + '"]');

//                if ($selectedOption.length < 1) {
//                    // add the value into the multiselect control
//                    $multiselect.append($("<option>").val(id).html(label).attr("selected", ""));
//                }
//                else {
//                    $selectedOption.attr("selected", "");
//                }

//                var $selectedDisplayObj = $container.children().filter(function (index) { return $(this).data("id") == id; });

//                // do not add the object if it already exists
//                if ($selectedDisplayObj.length < 1) {
//                    // add to the selected container
//                    $container.append($selected);

//                    // attach a click event
//                    $selected.click(function () {

//                        // remove the object from the multiselect
//                        $multiselect.find('option[value="' + id + '"]').removeAttr("selected");

//                        // remove from the container
//                        $(this).remove();

//                    });
//                }
//            }

//        } // end of the auto complete select

//    }); // end of $.fn.extend


//})(jQuery);