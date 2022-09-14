//#Active buttons
$(document).ready(function () {
    CreateActiveAction();
});

function CreateActiveAction(element) {
    var elements = [];
    if (element === undefined || element === null || element.length <= 0) {
        $("body").find("a[data-model].activable").each(function() {
            elements.push($(this));
        });
    } else {
        elements.push(element);
    }
    for (let i = 0; i < elements.length; i++) {
        let $a = elements[i];
        
        $a.unbind("click");
        $a.on("click", function (e) {
            e.preventDefault();
            let $this = $(this);
            let modelName = $this.attr("data-model");
            let key = $this.attr("data-key");
            if (modelName === undefined || key === undefined) return;

            $.post("/api/activable", { modelName, key }, function (data) {
                if (data === true) {
                    showMessage("Registo ativado com sucesso", 1);
                    $this
                        .removeClass("deactivated")
                        .addClass("activated")
                        .find(".fa-toggle-off").removeClass("fa-toggle-off").addClass("fa-toggle-on");

                } else if (data === false) {
                    showMessage("Registo inativado com sucesso", 1);
                    $this
                        .removeClass("activated")
                        .addClass("deactivated")
                        .find(".fa-toggle-on").removeClass("fa-toggle-on").addClass("fa-toggle-off");
                } else {
                    location.href = location.href;
                }
            }).fail(function () {
                    location.href = location.href;
            });
        });
    }
}

//#Active buttons