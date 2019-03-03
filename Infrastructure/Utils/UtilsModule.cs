using System;
using Autofac;
using Quarks.CQRS;
using Quarks.CQRS.Impl;

namespace Utils
{
    public sealed class UtilsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QueryDispatcher>()
                .As<IQueryDispatcher>();

            builder.RegisterType<CommandDispatcher>()
                .As<ICommandDispatcher>();

            builder.RegisterType<HandlerFactory>()
                .As<ICommandHandlerFactory>()
                .As<IQueryHandlerFactory>();

            base.Load(builder);
        }
    }
}
