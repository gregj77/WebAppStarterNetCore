using System;
using Autofac;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Validation
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>().SingleInstance();
            builder.RegisterInstance(AutofacValidatorFactory.Instance);
            builder.RegisterBuildCallback(OnContainerConfigured);

            base.Load(builder);
        }

        private void OnContainerConfigured(IContainer container)
        {
            var serviceProviderValidatorFactory = (AutofacValidatorFactory)container.Resolve<IValidatorFactory>();
            serviceProviderValidatorFactory.Container = container;
        }
    }
}
