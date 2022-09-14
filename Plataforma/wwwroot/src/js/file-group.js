//#FileGroup
$(document).ready(function () {
    CreateFileGroup();
});
function CreateFileGroup(element) {
    var elements = [];
    if (element === undefined || element === null || element.length <= 0) {
        $("body").find(".fileGroup").each(function () {
            elements.push($(this));
        });
    } else {
        elements.push(element);
    }

    for (let i = 0; i < elements.length; i++) {
        let $fileGroup = elements[i];

        if ($fileGroup.find(".fileGroupFiles").length == 1) {
            sortable($fileGroup.find(".fileGroupFiles"),
                {
                    handle: ".sort-handle",
                    forcePlaceholderSize: true,
                    placeholder: "<div class=\"col-12 mb-3\"><div class=\"w-100 h-100 bg-grey\"></div></div>",
                    itemSerializer: (serializedItem) => {
                        return $(serializedItem.node).data("id");
                    }
                }
            )[0].addEventListener('sortupdate', function (e) {
                let items = sortable($(e.target), "serialize")[0].items;
                let model = $(e.target).find(".sort-handle").data("model");
                $.ajax({
                    url: "/api/sortable",
                    method: "POST",
                    data: { modelName: model, items: items }
                }).fail(function (jqXHR, textStatus) {
                    console.log(jqXHR);
                });
            });
        }
        $fileGroup.find("button.fileGroupDeleteButton").each(function (el) { $(el).unbind("click") });
        $fileGroup.find("button.fileGroupEditButton").each(function (el) { $(el).unbind("click") });

        $fileGroup.find("input.fileGroupUploadInput").on("change", function (e) {
            if (this.files !== undefined && this.files !== null) {
                let uploadFiles = [];
                let files = this.files;
                let arrayLength = this.files.length;
                for (let j = 0; j < arrayLength; j++) {
                    uploadFiles.push(files[j]);
                }
                
                FileGroupUploadFiles($(e.target).parent(), uploadFiles);
                $(e.target).val("");
            }
        });

        $fileGroup.find("button.fileGroupUploadButton").on("click", function (e) {
            e.preventDefault();
            $(e.target).siblings("input.fileGroupUploadInput").trigger("click");
            return false;
        });

        $fileGroup.on("click", "button.fileGroupDeleteButton", function (e) {
            e.stopPropagation();
            e.preventDefault();
            let $file = $(e.target).parentsUntil(".fileGroupFileContainer").parent();
            let id = $file.data("id");
            Confirm("Deseja apagar o ficheiro?",
                function () {
                    $.ajax({
                        method: "POST",
                        url: "/api/files/filedelete",
                        data: { fileid: id }
                    }).done(function (data) {
                        $file.remove();
                        showMessage("Ficheiro eliminado com sucesso.", 1);
                    }).fail(function (data) {
                        showMessage("Não foi possível apagar o ficheiro.", -1);
                    });
                }
            );

            return false;
        });

    }
}

function FileGroupUploadFiles($fileGroup, files) {
    let fileGroupId = $fileGroup.data("filegroup");
    let progressBarTemplate = $fileGroup.find(".progress-template").clone().html();

    for (let i = 0; i < files.length; i++) {
        let file = files[i];

        if (file.size > maxFileSizeBytes) {
            showMessage("O ficheiro introduzido é demasiado grande", "Danger");
            continue;
        }

        let formdata = new FormData();
        formdata.append("fileGroupId", fileGroupId);
        formdata.append("file", file);

        

        let ajax = new XMLHttpRequest();
        ajax.upload.index = i;

        let $progressBar = $(progressBarTemplate);
        $($progressBar).insertAfter($fileGroup.find(".fileGroupUploadInput"));

        ajax.upload.progressBar = $progressBar;
        ajax.upload.fileGroup = $fileGroup;
        ajax.upload.addEventListener("progress", progressHandler, false);
        ajax.addEventListener("load", completeHandler, false);
        ajax.addEventListener("error", errorHandler);
        ajax.open("POST", "/api/files/fileupload");
        ajax.send(formdata);
    }
    return false;
}

function progressHandler(event) {
    let $progressBar = $(event.currentTarget.progressBar).find(".progress-bar");
    var percent = event.loaded / event.total * 100;
    if (percent >= 100) {
        percent = 99;
    }
    let percentText = Math.round(percent) + "%";
    $progressBar.text(percentText);
    $progressBar.css("width", percent * 100);
}

function completeHandler(event) {
    let $progressBar = event.currentTarget.upload.progressBar;
    let $fileGroup = event.currentTarget.upload.fileGroup;
    let status = event.currentTarget.status;
    if (status >= 400) {
        errorHandler(event);
    } else {
        let resposta = event.currentTarget.response;
        $fileGroup.find(".fileGroupFiles").prepend(resposta);

        if ($fileGroup.find(".fileGroupFiles").length == 1) {
            sortable($fileGroup.find(".fileGroupFiles"),
                {
                    handle: ".sort-handle",
                    forcePlaceholderSize: true,
                    placeholder: "<div class=\"col-12 mb-3\"><div class=\"w-100 h-100 bg-grey\"></div></div>",
                    itemSerializer: (serializedItem) => {
                        return $(serializedItem.node).data("id");
                    }
                }
            )[0].addEventListener('sortupdate', function (e) {
                let items = sortable($(e.target), "serialize")[0].items;
                let model = $(e.target).find(".sort-handle").data("model");
                $.ajax({
                    url: "/api/sortable",
                    method: "POST",
                    data: { modelName: model, items: items }
                }).fail(function (jqXHR, textStatus) {
                    console.log(jqXHR);
                });
            });
        }

        $progressBar.remove();
    }
}
function errorHandler(event) {
    var message = "Não foi possível inserir o ficheiro.";
    if (event.currentTarget.status == 400) {
        message = event.currentTarget.response;
    }
    showMessage(message, -1);
    $(event.currentTarget.upload.progressBar).remove();
}
//#FileGroup