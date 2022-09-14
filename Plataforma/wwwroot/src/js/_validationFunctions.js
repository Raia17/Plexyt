//# VAT
function validatePtVat(value) {
    if (value.length === 0) return true;
    if (typeof value !== "string" || value.length !== 9) return false;
    let sumAux = 0;
    for (let i = 9; i >= 2; i--) {
        sumAux += i * (parseInt(value[value.length - i]) || 0);
    }
    let module = sumAux % 11;

    // Get the eight first numbers
    let nifWithoutLastDigit = value.slice(0, value.length - 1);

    if (module === 0 || module === 1) {
        return `${nifWithoutLastDigit}0` === value;
    } else {
        return `${nifWithoutLastDigit}${11 - module}` === value;
    }
}
//# VAT


//# Email
function validateEmail($email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}
//# Email

//# Select
function validateSelect($element) {
    return $element.val() != null && $element.val() != undefined && $element.val() != "";
}
//# Select