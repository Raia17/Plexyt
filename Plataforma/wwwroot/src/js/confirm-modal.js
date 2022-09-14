function Confirm(text, functionYes, functionNo, textYes, textNo) {
    if (text === undefined || text === null || text === "")
        text = "Deseja continuar?";
    if (textYes === undefined || textYes === null || textYes === "")
        textYes = "Sim";
    if (textNo === undefined || textNo === null || textNo === "")
        textNo = "Não";
    $("body").find("#modalConfirm").find(".modal-text").html(text);

    var modal = new bootstrap.Modal(document.getElementById('modalConfirm'), { backdrop: "static" });
    modal.show();

    $("body")
        .find("#modalConfirm")
        .find(".btn-primary")
        .text(textYes)
        .unbind("click")
        .on("click", functionYes)
        .on("click", function () { $("body").find("#modalConfirm").modal("hide") })
        .trigger("focus");

    $("body")
        .find("#modalConfirm")
        .find(".btn-outline-primary")
        .text(textNo)
        .unbind("click")
        .on("click", functionNo)
        .on("click", function () { $("body").find("#modalConfirm").modal("hide") })
        .trigger("focus");
}

$(document).ready(function () {
    $(".form-confirm").on("submit", function (event) {
        if ($(this).hasClass("form-confirm")) {
            event.preventDefault();
            event.stopPropagation();
            let $element = $(this);

            Confirm($element.data("message"), function () {
                $element.removeClass("form-confirm");
                $element.submit();
            });
            return false;
        }
    });

    $(".btn-confirm").on("submit click", function (event) {
        if ($(this).hasClass("btn-confirm")) {
            event.preventDefault();
            event.stopPropagation();
            let $element = $(this);

            Confirm($element.data("message"), function () {
                $element.removeClass("btn-confirm");
                if ($element.find("i").length)
                    $element.find("i").trigger("click");
                else
                    $element.trigger("click");
            });
            return false;
        }
    });
});