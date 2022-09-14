$(document).ready(function () {
    $("body").on("click", ".clear-input", function () {
        $(this).siblings("input").val("");
    });
    $("body").on("click", ".clear-select", function () {
        clearSelect($(this));
    });
});

function clearSelect($select) {
    if ($select.parent().find("select").hasClass("select2-hidden-accessible")) {
        $select.parent().find("select")[0].value = null;
        $select.parent().find("select").trigger("change").trigger("select2:select");
    }
    else
        $select.parent().find("select").val("").trigger("change").trigger("select2:select");
}