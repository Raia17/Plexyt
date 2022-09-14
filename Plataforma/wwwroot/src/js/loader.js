$(document).ready(function () {
    initLoader();
});
function initLoader() {
    if (!$("body").find(".loader").length) {
        $("body").append("<div class=\"loader\"><div class=\"spinner-border text-white\"></div><div class=\"message\"></div></div></div>");
    }
}
function showLoader(content) {
    initLoader();

    if (content === undefined || content === "") {
        content = "Por favor aguarde...";
    }

    $("body").find(".loader .message").html(content);
    $("body").find(".loader").show();
}
function hideLoader() {
    $("body").find(".loader .message").html("");
    $("body").find(".loader").hide();
}