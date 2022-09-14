using Microsoft.AspNetCore.Identity;
using Plataforma.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Plataforma.Models.Identity;

public class User : IdentityUser<int>, IActivable {
    public override int Id { get; set; }


    private string _passwordHash;

    public override string PasswordHash {
        get => _passwordHash;
        set {
            if (string.Equals(value, _passwordHash)) return;

            _passwordHash = value;
        }
    }

    public bool Active { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;

    public string Name { get; set; }

    public bool Master { get; set; }

    [NotNull]
    [DefaultValue("TopCenter")]
    public string MessagesPosition { get; set; } = "TopCenter";

    public virtual List<IdentityUserClaim> Claims { get; set; } = new();

    public DateTime? LastLogin { get; set; }

    public string RecoverPasswordToken { get; set; }
    public DateTime? RecoverPasswordTime { get; set; }
}