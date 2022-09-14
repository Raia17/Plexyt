/*Alert messages*/
$(document).ready(function () {
    initMessages();
});
function initMessages() {
    if (!$("body").find(".alert-messages").length) {
        $("body").append("<div class=\"alert-messages\"></div>");
    }
}
function showMessage(text, type) {
    initMessages();
    var alertClass = "info";
    var iconClass = "info";
    if (type == 1 || type == "Success") {
        alertClass = "success";
        iconClass = "check";
    } else if (type == -1 || type == "Danger") {
        alertClass = "danger";
        iconClass = "exclamation";
    } else if (type == -2 || type == "Warning") {
        alertClass = "warning";
        iconClass = "exclamation";
    }

    let textSpan = document.createElement("span");
    let $textSpan = $(textSpan);

    $textSpan.html(text);
    $textSpan.prepend(`<i class="text-${alertClass} fa-solid fa-${iconClass} me-2"></i>`);

    let alertRow = document.createElement("DIV");
    let $alertRow = $(alertRow);
    $alertRow
        .addClass("row m-0")
        .on("click", function () {
            hideMessage($(this), true);
        });
    let alertDiv = document.createElement("DIV");
    let $alert = $(alertDiv);
    $alert
        .addClass(`bg-white cursor-pointer border-${alertClass} border rounded p-2 mb-2 w-auto`)
        .attr("role", "alert")
        .append($textSpan);
    $alertRow.append($alert);
    $(".alert-messages").append($alertRow);

    setTimeout(hideMessage, 5000, $alertRow);
}
function hideMessage($element, removeTimeout) {
    if (removeTimeout) {
        removeMessage($element);
    }
    else {
        $element.addClass("hidding");
        $element.on("animationend", removeMessage);
        $element.on("webkitAnimationEnd", removeMessage);
        $element.on("mozAnimationEnd", removeMessage);
        setTimeout(removeMessage, 300, $element);
    }
}
function removeMessage(element) {
    if ($(element).length)
        $(element).remove();
}
/*Alert messages*/