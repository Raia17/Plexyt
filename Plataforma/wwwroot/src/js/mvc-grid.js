//#MVC Grid
$(document).ready(function () {
    CreateMVCGrid();
});

function CreateMVCGrid(element) {
    var elements = [];
    if (element === undefined || element === null || element.length <= 0) {
        $("body").find(".mvc-grid").each(function () {
            elements.push(this);
        });
    } else {
        elements.push(element);
    }
    for (let i = 0; i < elements.length; i++) {
        new MvcGrid(elements[i]);
    }
}
//#MVC Grid