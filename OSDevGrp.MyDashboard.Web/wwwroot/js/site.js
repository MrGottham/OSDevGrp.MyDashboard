(function($) {
    $.fn.extend({
        openLink: function(link) {
            var win = open(link, "_blank");
            if (win != null) {
                win.focus();
            }
        },

        handleUrlInstantLoading: function(document) {
            var elementArray = $.makeArray(document.find("[data-url-instant-loading]"));
            $.each(elementArray, function() {
                var replaceElementArray = $(this).find("[data-url]");
                if (replaceElementArray.length === 0) {
                    return;
                }

                $.each(replaceElementArray, function() {
                    $().replaceWithPartialViewFromUrl(this);
                });
            });
        },

        replaceWithPartialViewFromUrl: function(replaceElement) {
            var url = $(replaceElement).data("url");
            if (url == null) {
                return;
            }

            $.get(url, null, null, "html")
                .done(function(data) {
                    $(replaceElement).replaceWith(data);
                })
                .fail(function(jqXhr, textStatus, errorThrown) {
                    $(replaceElement).replaceWith("<div class=\"alert alert-danger\" role=\"alert\">" + errorThrown + "</div>");
                });
        },

        startMainContentObserver: function(element) {
            var mainContentObserver = new MutationObserver($().mainContentObserverCallback);
            mainContentObserver.observe(element, { childList: true });
        },

        mainContentObserverCallback: function(mutationsList, observer) {
            if (mutationsList.length === 0) {
                return;
            }

            observer.disconnect();

            $().reloadSubContent();
        },

        reloadSubContent: function() {
            var observeElementArray = $.merge($.merge($("#topContent"), $("#subContent")), $("#dashboardSettings"));
            if (observeElementArray.length === 0)
            {
                return;
            }

            $.each(observeElementArray, function() {
                var replaceElementArray = $(this).find("[data-url]");
                if (replaceElementArray.length === 0)
                {
                    return;
                }

                $().setVisible("#" + this.id, true);
                $().startSubContentObserver(this);

                $.each(replaceElementArray, function() {
                    $().replaceWithPartialViewFromUrl(this);
                });
            });
        },

        startSubContentObserver: function(element) {
            var subContentObserver = new MutationObserver($().subContentObserverCallback);
            subContentObserver.observe(element, { childList: true });
        },

        subContentObserverCallback: function(mutationsList, observer) {
            if (mutationsList.length === 0) {
                return;
            }

            observer.disconnect();

            $.each(mutationsList, function() {
                $.each($(this.target).find(".carousel"), function() {
                    $(this).carousel();
                });

                $.each($(this.target).find("#commit"), function() {
                    $().setupDashboardSettings($(document));
                });
            });
        },

        setVisible: function(elementId, isVisible) {
            $.each($(elementId), function() {
                if (isVisible)
                {
                    $(this).show();
                    return;
                }

                $(this).hide();
            });
        },

        enableFormValidation: function(formId) {
            var formArray = $(document).find(formId);
            if (formArray.length === 0) {
                return;
            }

            $.each(formArray, function() {
                $(this).removeData("validator");
                $(this).removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse($(this));
            });
        },

        setupDashboardSettings: function(document) {
            $().enableFormValidation("#dashboardSettings");

            var checkboxUseReddit = document.find("#UseReddit");
            var checkboxIncludeNsfwContent = document.find("#NotNullableIncludeNsfwContent");
            var checkboxOnlyNsfwContent = document.find("#NotNullableOnlyNsfwContent");
            if (checkboxUseReddit.length == 0 || checkboxIncludeNsfwContent.length == 0 || checkboxOnlyNsfwContent.length == 0) {
                return;
            }

            if (checkboxUseReddit.prop("checked") == false) {
                checkboxIncludeNsfwContent.prop("checked", false);
                checkboxIncludeNsfwContent.prop("disabled", true);
                checkboxOnlyNsfwContent.prop("checked", false);
                checkboxOnlyNsfwContent.prop("disabled", true);
            }

            if (checkboxIncludeNsfwContent.prop("checked") == false) {
                checkboxOnlyNsfwContent.prop("checked", false);
                checkboxOnlyNsfwContent.prop("disabled", true);
            }

            checkboxUseReddit.change(function() {
                if ($(this).prop("checked")) {
                    checkboxIncludeNsfwContent.prop("disabled", false);
                    checkboxOnlyNsfwContent.prop("disabled", true);
                    return;
                }
                checkboxIncludeNsfwContent.prop("checked", false);
                checkboxIncludeNsfwContent.prop("disabled", true);
                checkboxOnlyNsfwContent.prop("checked", false);
                checkboxOnlyNsfwContent.prop("disabled", true);
            });

            checkboxIncludeNsfwContent.change(function() {
                if ($(this).prop("checked")) {
                    checkboxOnlyNsfwContent.prop("disabled", false);
                    return;
                }
                checkboxOnlyNsfwContent.prop("checked", false);
                checkboxOnlyNsfwContent.prop("disabled", true);
            });
        },
    }),

    $(document).ready(function() {
        $().setVisible("#topContent", false);
        $().setVisible("#subContent", false);

        $("#commit").prop("disabled", true);
        $().setupDashboardSettings($(document));

        $().startMainContentObserver(document.getElementById("mainContent"));

        $().handleUrlInstantLoading($(document));
    });
})(jQuery);