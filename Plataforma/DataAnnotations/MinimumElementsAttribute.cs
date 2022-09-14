using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Enum | AttributeTargets.All)]
public class MinimumElementsAttribute : ValidationAttribute, IClientModelValidator {
    public override bool IsValid(object value) {
        if (value is IList list)
            return list.Count >= 1;
        return false;
    }

    public void AddValidation(ClientModelValidationContext context) {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-minimumelements", ErrorMessageString);
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value) {
        if (attributes.ContainsKey(key)) return false;
        attributes.Add(key, value);
        return true;
    }

}