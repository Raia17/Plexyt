$(document).ready(function () {
    $(".checkbox-auto-submit").on("change", function () {
        let $parent = $(this).parent();
        var counter = 0;
        while ($parent.prop("tagName").toUpperCase() != "BODY" && $parent.prop("tagName").toUpperCase() != "FORM" && counter < 20) {
            $parent = $parent.parent();
            counter += 1;
        }
        if ($parent.prop("tagName").toUpperCase() == "FORM")
            $parent.submit();
    });
});

