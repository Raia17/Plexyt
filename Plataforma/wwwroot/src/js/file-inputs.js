//#Image inputs
$(document).ready(function () {
    $(".file-input-delete").on("click", function () {
        $(this).siblings("input").val(true);
        $(this).siblings(".open-file").remove();
        $(this).parent().siblings(".custom-file").removeClass("d-none");
        $(this).remove();
    });
});
//#Image inputs