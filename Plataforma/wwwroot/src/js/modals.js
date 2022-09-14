$(document).ready(function () {
    $("body").on("click", ".btn-modal",  function () {
        showLoader();
        $.get($(this).data("url"))
            .done(function (data, textStatus, jqXHR) {
                xhrModal("", data, true);
                hideLoader();
            })
            .fail(function (jqXhr, textStatus, errorThrown) {
                console.log(jqXhr.responseText);
                showMessage("Não foi possível obter o pedido", -1);
                hideLoader();
            });
    });
});


function dynamicModal(html, functionYes, functionNo, textYes, textNo, extraclass, showNo) {
    if (textYes === undefined || textYes === null || textYes === "")
        textYes = "Sim";

    if (textNo === undefined || textNo === null || textNo === "")
        textNo = "Não";
    if (extraclass === undefined || extraclass === null)
        extraclass = "";

    $("body").find("#modalDynamic").find(".modal-html").html(html);

    $("body").find("#modalDynamic").find(".modal-dialog").removeClass().addClass("modal-dialog " + extraclass);

    var modal = new bootstrap.Modal(document.getElementById('modalDynamic'), { backdrop: "static" });
    modal.show();

    $("body")
        .find("#modalDynamic")
        .find(".btn-primary")
        .text(textYes)
        .unbind("click")
        .on("click", functionYes)
        .on("click", function () { $("body").find("#modalDynamic").modal("hide") });

    $("body")
        .find("#modalDynamic")
        .find(".btn-outline-primary")
        .text(textNo)
        .removeClass("d-none")
        .unbind("click")
        .on("click", functionNo)
        .on("click", function () { $("body").find("#modalDynamic").modal("hide") });

    if (showNo == false)
        $("body")
            .find("#modalDynamic")
            .find(".btn-outline-primary")
            .addClass("d-none");

}

function xhrModal(title, html, getTitle = false, full = false) {

    $("body").find("#modalXhr").find(".modal-html").html(html);
    $("body").find("#modalXhr").find(".modal-title").html(title);
    if (getTitle) {
        title = $("body").find("#modalXhr").find(".modal-html").find("input.title").val()
        $("body").find("#modalXhr").find(".modal-title").html(title);
    }
    $("body").find("#modalXhr").find(".modal-content").removeClass("full");
    if (full) {
        $("body").find("#modalXhr").find(".modal-content").addClass("full");
    }
    var modal = new bootstrap.Modal(document.getElementById('modalXhr'), { backdrop: "static" });
    modal.show();


    $("#modalXhr form").removeData("validator");
    $("#modalXhr form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("#modalXhr form");
}

function clearXhrModal() {
    $("body").find("#modalXhr").modal("hide");
    $("body").find("#modalXhr").find(".modal-html").html("");
    $("body").find("#modalXhr").find(".modal-title").html("");
}