using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;


namespace ToDoList.Infrastructure
{
    public static class ModelStateExtensions
    {
        public static void AddValidationErrors(this ModelStateDictionary modelState, ValidationResult errors)
        {
            if (errors?.Errors?.Count == 0) return;

            foreach (var e in errors.Errors)
                modelState.AddModelError(e.PropertyName, e.ErrorMessage);
        }

        public static void AddErrors(this ModelStateDictionary modelState, IEnumerable<string>? errors)
        {
            if (errors is null) return;
            foreach (var e in errors)
                modelState.AddModelError(string.Empty, e); 
        }
    }
}
