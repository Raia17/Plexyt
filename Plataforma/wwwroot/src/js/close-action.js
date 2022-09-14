//#Active buttons
$(document).ready(function () {
    CreateCloseAction();
});

function CreateCloseAction(element) {
    var elements = [];
    if (element === undefined || element === null || element.length <= 0) {
        $("body").find("span[data-model].closable").each(function () {
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
            let open = $this.data("open");

            $.post("/api/closable", { modelName, key, open }, function (data) {
                console.log($this.find("i"));

                if (data === true) {
                    showMessage("Registo fechado com sucesso", 1);
                    $this.data("open", "1")
                        .find("i")
                        .removeClass("fa-check")
                        .addClass("fa-rotate-left");

                } else if (data === false) {
                    showMessage("Registo aberto com sucesso", 1);
                    $this.data("open", "0")
                        .find("i")
                        .removeClass("fa-rotate-left")
                        .addClass("fa-check");

                } else {
                    location.href = location.href;
                }
            }).fail(function (jqXHR, textStatus) {
                console.log(jqXHR);
            });
        });
    }
}

//#Active buttons