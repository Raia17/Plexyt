//# Can validate hidden fields (select2 for example)
$.validator.setDefaults({ ignore: '' });

//# Overrides jquery validator number validation
$.validator.methods.number = function (value, element) {
    return this.optional(element)
        || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)?(?:,\d+)?$/.test(value)
        || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value);
};
//# Overrides jquery validator number validation


//# Minimum elements in select
jQuery.validator.addMethod("minimumelements",
    function (value, element) {
        if (value != null && value != undefined) {
            if ($.isArray(value)) {
                return value.length > 0;
            } else {
                return value != "";
            }
        }
        return false;
    });
jQuery.validator.unobtrusive.adapters.addBool("minimumelements");
//# Minimum elements in select



//# One of the two filled
jQuery.validator.addMethod("oneofthetwo",
    function (value, element) {
        let otherProperty = $("#" + $(element).data("val-oneofthetwoprop"));
        let otherPropertyValue = otherProperty.val();
        return !((value == null || value == undefined || value == "") && (otherPropertyValue == null || otherPropertyValue == undefined || otherPropertyValue == ""));
    });
jQuery.validator.unobtrusive.adapters.addBool("oneofthetwo");
//# One of the two filled


//# Change messages
jQuery.extend(jQuery.validator.messages, {
    required: "Campo obrigatório",
    remote: "Corrija este campo",
    email: "Introduza um endereço de e-mail válido",
    url: "Introduza um URL válido",
    date: "Introduza uma data válida",
    dateISO: "Introduza uma data válida",
    number: "Introduza um número válido",
    digits: "Introduza apenas dígitos",
    creditcard: "Introduza um número de cartão de crédito válido",
    equalTo: "Introduza o mesmo valor",
    maxlength: jQuery.validator.format("Não introduza mais do que {0} caracteres"),
    minlength: jQuery.validator.format("Introduza pelo menos {0} caracteres"),
    rangelength: jQuery.validator.format("Introduza um valor entre {0} e {1} caracteres"),
    range: jQuery.validator.format("Introduza um valor entre {0} e {1}"),
    max: jQuery.validator.format("Introduza um valor inferior ou igual a {0}"),
    min: jQuery.validator.format("Introduza um valor superior ou igual a {0}")
});
//# Change messages