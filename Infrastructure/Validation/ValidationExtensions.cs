using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Validation
{
    public static class ValidationExtensions
    {
        internal static IValidatorFactory ValidatorFactory;

        private static readonly ConditionalWeakTable<object, ValidationResult> ValidationCache = new ConditionalWeakTable<object, ValidationResult>();

        public static Module RegisterModuleValidators(this Module module, ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(module.GetType().Assembly)
                .Where(p => typeof(IValidator).IsAssignableFrom(p))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return module;
        }

        public static ValidationResult StoreResult<TModel>(this IValidator<TModel> validator, TModel model, ValidationResult result)
        {
            if (!ValidationCache.TryGetValue(model, out var tmp))
            {
                ValidationCache.Add(model, result);
            }

            return result;
        }        

        public static async Task<TModel> Validate<TModel>(this TModel model, ILogger logger = null)
        {
            ValidationResult result;
            if (model != null)
            {
                result = await model.EnsureModelIsValidAsync();
            }
            else
            {
                result = new ValidationResult(new[] {new ValidationFailure(string.Empty, "Model is not set to a valid reference")});
            }

            if (!result.IsValid)
            {
                throw new ValidationException($"The model {model?.GetType().FullName ?? "<null>"} is invalid", result.Errors);
            }

            return model;
        }

        public static async Task<ValidationResult> EnsureModelIsValidAsync<TModel>(this TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (ValidationCache.TryGetValue(model, out var result))
                return result;

            return await InternalValidateAsync(model);
        }

        private static async Task<ValidationResult> InternalValidateAsync(object model)
        {
            var validator = ValidatorFactory.GetValidator(model.GetType());
            if (validator == null)
                throw new InvalidOperationException($"Validator for model {model.GetType().FullName} not found!");

            var result = await validator.ValidateAsync(model);

            if (!ValidationCache.TryGetValue(model, out var tmp))
                ValidationCache.Add(model, result);

            return result;
        }
    }

}
