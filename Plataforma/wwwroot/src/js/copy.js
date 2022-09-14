$(document).ready(function () {
    $(".copy").on("click", function () {
        var textBox = $(this).siblings("input");
        var text = textBox.val();
        if (!textBox.length) {
            textBox = $(this).siblings(".form-control");
            text = textBox.val();
            if (textBox.prop("tagName") == "DIV")
                text = textBox.text();
        }
        copyText(text);
    });
})
function copyText(text) {
    var inputCopy = $("body").find("textarea#copy");
    if (!inputCopy.length) {
        $("body").append("<textarea id=\"copy\" style=\"width:1; height:1; position:fixed; top:-1; left:-1;\"></textarea>");
        inputCopy = $("body").find("textarea#copy");
    }
    inputCopy.val(text);
    inputCopy.select();
    document.execCommand("copy");
    showMessage("Texto copiado com sucesso", 1)
}