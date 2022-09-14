
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Plataforma.Helpers;

public static class TranslationsHelper {

    #region Identity
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = "Ocorreu um erro" }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Ocorreu um erro" }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Password inválida" }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Token inválido" }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Já existe um utilizador com este email" }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"O utilizador '{userName}' é inválido" }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"O email '{email}' é inválido" }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Já existe um utilizador com este username '{userName}'" }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Já existe um utilizador com este email '{email}'" }; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"A permissão '{role}' é inválida" }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Já existe a permissão '{role}'" }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "O utilizador já tem uma password definida" }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "O bloqueio não é permitido para este utilizador" }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"O utilizador já tem a permissão '{role}'" }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"O utilizador não tem a permissão '{role}'" }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"A password deve conter pelo menos {length} caracter{(length > 0 ? "es" : "")}" }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "A password deve conter pelo menos um caracter não alfanumérico" }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "A password deve conter pelo menos um digito ('0'-'9')" }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "A password deve conter pelo menos um caracter minúsculo ('a'-'z')" }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "A password deve conter pelo menos um caracter maiúsculo ('A'-'Z')" }; }
        public override IdentityError PasswordRequiresUniqueChars(int number) { return new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"A password deve conter pelo menos {number} caracter{(number > 0 ? "es" : "")} únicos" }; }
    }
    #endregion

    #region Validations
    public static void TranslateValidations(ref MvcOptions options) {
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(_ => "Campo obrigatório");
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((_, _) => "O valor inserido é inválido");
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(_ => "O valor inserido é inválido");
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "Campo obrigatório");
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Campo obrigatório");
        options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor inserido é inválido");
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(_ => "O valor inserido é inválido");
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(_ => "O valor inserido é inválido");
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "O valor inserido tem de ser numérico");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(_ => "O valor inserido deve ser numérico");
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "O valor não pode ser vazio");
    }
    #endregion

}