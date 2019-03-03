using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quarks.CQRS.Impl;

namespace Utils
{
    internal class HandlerFactory : ICommandHandlerFactory, IQueryHandlerFactory
    {
        private readonly IComponentContext _container;
        private readonly ILogger<HandlerFactory> _logger;

        public HandlerFactory(IComponentContext container)
        {
            _container = container;
            _logger = container.Resolve<ILogger<HandlerFactory>>();
        }

        public object CreateHandler(Type handlerType)
        {
            if (_container.IsRegistered(handlerType))
            {
                try
                {
                    var result = _container.Resolve(handlerType);

                    _logger.LogTrace($"Found handler for type: {handlerType.GetGenericArguments()[0].FullName} -> type: {result.GetType().FullName}");

                    return result;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to resolve handler {handlerType.FullName} due to <{e.GetType().FullName}>{e.Message}\n{e.StackTrace}");
                }
            }

            _logger.LogTrace($"No handler found for type: {handlerType.GetGenericArguments()[0].FullName}, assuming the model is correct.");

            throw new NotSupportedException($"QueryHandler {handlerType.FullName} is not registered!");
        }
    }
}
