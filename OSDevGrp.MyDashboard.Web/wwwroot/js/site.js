function openLink(link) {
    var win = open(link, '_blank');
    if (win != null) {
        win.focus();
    }
}

$(document).ready(function() {
    var checkboxUseReddit = $("#UseReddit");
    var checkboxIncludeNsfwContent = $("#NotNullableIncludeNsfwContent");
    var checkboxOnlyNsfwContent = $("#NotNullableOnlyNsfwContent");
    if (checkboxUseReddit.length == 0 || checkboxIncludeNsfwContent.length == 0 || checkboxOnlyNsfwContent.length == 0) {
        return;
    }

    if (checkboxUseReddit.prop('checked') == false) {
        checkboxIncludeNsfwContent.prop('checked', false);
        checkboxIncludeNsfwContent.prop('disabled', true);
        checkboxOnlyNsfwContent.prop('checked', false);
        checkboxOnlyNsfwContent.prop('disabled', true);
    }
    
    if (checkboxIncludeNsfwContent.prop('checked') == false) {
        checkboxOnlyNsfwContent.prop('checked', false);
        checkboxOnlyNsfwContent.prop('disabled', true);
    }

    checkboxUseReddit.change(function() {
        if ($(this).prop('checked')) {
            checkboxIncludeNsfwContent.prop('disabled', false);
            checkboxOnlyNsfwContent.prop('disabled', true);
            return;
        }
        checkboxIncludeNsfwContent.prop('checked', false);
        checkboxIncludeNsfwContent.prop('disabled', true);
        checkboxOnlyNsfwContent.prop('checked', false);
        checkboxOnlyNsfwContent.prop('disabled', true);
    });

    checkboxIncludeNsfwContent.change(function() {
        if ($(this).prop('checked')) {
            checkboxOnlyNsfwContent.prop('disabled', false);
            return;
        }
        checkboxOnlyNsfwContent.prop('checked', false);
        checkboxOnlyNsfwContent.prop('disabled', true);
    });
});