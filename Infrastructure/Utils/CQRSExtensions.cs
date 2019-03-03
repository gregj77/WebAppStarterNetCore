using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Quarks.CQRS;
using Utils.CQRS;

namespace Utils
{
    public static class CQRSExtensions
    {
        public static Module RegisterHandlers(this Module module, ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(module.GetType().Assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(module.GetType().Assembly)
                .AsClosedTypesOf(typeof(IQueryHandler<,>))
                .AsImplementedInterfaces();

            var types = module.GetType().Assembly
                .GetTypes()
                .Where(p => p.BaseType != null &&
                            p.BaseType.IsGenericType &&
                            p.BaseType.GetGenericTypeDefinition() == typeof(GenericCUDQueryHandler<,>))
                .ToList();

            foreach (var type in types)
            {
                var genericArguments = type.BaseType.GetGenericArguments();
                var makeGenericType = typeof(IQueryHandler<,>).MakeGenericType(genericArguments);
                builder.RegisterType(type).As(makeGenericType);
            }

            return module;
        }
    }
}
