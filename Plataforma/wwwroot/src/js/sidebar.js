//#Scrollbar width
$(document).ready(function () {
    //let scrollbarWith = getScrollBarWidth();
    //$(".aside-container").css("padding-right", '' + scrollbarWith + "px");
    //$("aside").css("right", '-' + scrollbarWith + "px");

    //#Sidebar Toggle
    $(".sidebar-toggle-container").on("click", function () {
        if ($("aside").css("position") === "relative") {
            $("body").removeClass("sidebar-hide");
            $("#menus").collapse('toggle');
        } else {
            $("#menuAccordion").toggleClass("close")
            $("body").toggleClass("sidebar-hide");
            $(".sidebar-toggle").toggleClass('active');
            $(".logo-container .div-text").toggleClass('active');
            $(".sidebar-bottom").toggleClass("close");
            $(".menu-item").each(function (index, value) {
                $(this).toggleClass("close")
            })
        }
    });
    //#Sidebar Toggle

});


//#Scrollbar width
function getScrollBarWidth() {
    var inner = document.createElement('p');
    inner.style.width = "100%";
    inner.style.height = "200px";

    var outer = document.createElement('div');
    outer.style.position = "absolute";
    outer.style.top = "0px";
    outer.style.left = "0px";
    outer.style.visibility = "hidden";
    outer.style.width = "200px";
    outer.style.height = "150px";
    outer.style.overflow = "hidden";
    outer.appendChild(inner);

    document.body.appendChild(outer);
    var w1 = inner.offsetWidth;
    outer.style.overflow = 'scroll';
    var w2 = inner.offsetWidth;
    if (w1 == w2) w2 = outer.clientWidth;

    document.body.removeChild(outer);

    return (w1 - w2);
};
//#Scrollbar width