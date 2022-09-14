using Microsoft.AspNetCore.Identity;
using Plataforma.Models.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagementPlatform.Helpers;

public static class PasswordValidation {
    public static async Task<List<string>> GetList(UserManager<User> _userManager, User user) {
        var list = new List<string>();
        foreach (var userManagerPasswordValidator in _userManager.PasswordValidators) {
            var errorList = await userManagerPasswordValidator.ValidateAsync(_userManager, user, "");
            foreach (var identityError in errorList.Errors) list.Add(identityError.Description);
        }

        return list;
    }
}
