using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using ReferenceModule.Data;
using ReferenceModule.Models;
using Utils;
using Validation;

namespace ReferenceModule
{
    public class ReferenceModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            this.RegisterModuleValidators(builder).RegisterHandlers(builder);
            builder.RegisterType<ModuleContext>().As<IModuleContext>().InstancePerLifetimeScope();
            builder.RegisterBuildCallback(OnLoaded);
            base.Load(builder);
        }

        private void OnLoaded(IContainer c)
        {
            using (var scope = c.BeginLifetimeScope("AutofacWebRequest"))
            {
                var context = (ModuleContext) scope.Resolve<IModuleContext>();
                // seed data in ENV = Developer
                context.Models.Add(new Model {Id = 1, Name = "foo"});
                context.Models.Add(new Model {Id = 2, Name = "bar"});
                context.SaveChanges();
            }
        }
    }
}
