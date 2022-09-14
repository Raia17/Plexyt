$(document).ready(function() {
    $(".help-button").on("click", function() {
        dynamicModal($(this).next(".help-text").html(), function () { }, function () { }, "Fechar", "", "", false);
    });
})