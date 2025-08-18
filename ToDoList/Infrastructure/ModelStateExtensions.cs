using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

public static class ModelStateExtensions
{
    public static void AddValidationErrors(this ModelStateDictionary modelState, ValidationResult vr)
    {
        if (vr == null) return;
        foreach (var f in vr.Errors)
            modelState.AddModelError(f.PropertyName ?? string.Empty, f.ErrorMessage);
    }

    public static void AddValidationErrors(this ModelStateDictionary modelState, IEnumerable<ValidationFailure> failures)
    {
        if (failures == null) return;
        foreach (var f in failures)
            modelState.AddModelError(f.PropertyName ?? string.Empty, f.ErrorMessage);
    }

    public static void AddErrors(this ModelStateDictionary modelState, IEnumerable<string> errors)
    {
        if (errors == null) return;
        foreach (var e in errors)
            modelState.AddModelError(string.Empty, e);
    }
}
