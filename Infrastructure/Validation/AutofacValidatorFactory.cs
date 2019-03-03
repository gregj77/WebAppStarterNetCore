using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Validation
{
    public sealed class AutofacValidatorFactory : ValidatorFactoryBase
    {
        internal static readonly AutofacValidatorFactory SingletonInstance = new AutofacValidatorFactory();

        internal IContainer Container { get; set; }
        private ILogger<AutofacValidatorFactory> _logger => Container.Resolve<ILogger<AutofacValidatorFactory>>();

        public static IValidatorFactory Instance => SingletonInstance;

        private AutofacValidatorFactory()
        {
            ValidationExtensions.ValidatorFactory = this;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            if (Container.IsRegistered(validatorType))
            {
                try
                {
                    var result = (IValidator)Container.Resolve(validatorType);

                    _logger.LogTrace($"Found validator for type: {validatorType.GetGenericArguments()[0].FullName} -> type: {result.GetType().FullName}");

                    return result;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to resolve validator {validatorType.FullName} due to <{e.GetType().FullName}>{e.Message}\n{e.StackTrace}");
                }
            }

            _logger.LogTrace($"No validator found for type: {validatorType.GetGenericArguments()[0].FullName}, assuming the model is correct.");

            return null;
        }    
    }
}
