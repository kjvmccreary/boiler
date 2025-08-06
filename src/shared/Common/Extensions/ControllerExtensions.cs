using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using DTOs.Common;

namespace Common.Extensions;

public static class ControllerExtensions
{
    public static async Task<T> ValidateAsync<T>(this ControllerBase controller, T model, IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(model);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => new ErrorDto(
                "VALIDATION_ERROR",
                error.ErrorMessage,
                error.PropertyName,
                error.AttemptedValue
            )).ToList();
            
            throw new Common.Exceptions.ValidationException(errors);
        }
        
        return model;
    }
}
