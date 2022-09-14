using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plataforma.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OneOfTheTwoAttribute : ValidationAttribute, IClientModelValidator {
    public string OtherProperty { get; private set; }
    public override bool RequiresValidationContext => true;

    public OneOfTheTwoAttribute(string otherProperty, string errorMessage) {
        OtherProperty = otherProperty ?? throw new ArgumentNullException(nameof(otherProperty));
        base.ErrorMessage = errorMessage;
    }

    public string GetErrorMessage() {
        return base.ErrorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
        if (otherPropertyInfo == null)
            return new ValidationResult(GetErrorMessage());

        if (value is not string propValue)
            return new ValidationResult(GetErrorMessage());

        var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

        if (otherPropertyValue is not string otherPropValue)
            return new ValidationResult(GetErrorMessage());

        if (string.IsNullOrWhiteSpace(propValue) && string.IsNullOrWhiteSpace(otherPropValue)) {
            return new ValidationResult(GetErrorMessage());
        }

        return null;
    }

    public void AddValidation(ClientModelValidationContext context) {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-oneofthetwo", GetErrorMessage());
        MergeAttribute(context.Attributes, "data-val-oneofthetwoprop", OtherProperty);
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value) {
        if (attributes.ContainsKey(key)) return false;
        attributes.Add(key, value);
        return true;
    }
}