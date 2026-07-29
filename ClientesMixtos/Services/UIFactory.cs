using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Services
{
    public class UIFactory(IServiceProvider serviceProvider, ILogger<UIFactory> logger)
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILogger<UIFactory> logger = logger;

        public T Create<T>(params object[] args) where T : class {

            logger.LogDebug("Resolviendo {UiType} con {ArgumentCount} argumentos en tiempo ejecucion",
                typeof(T).FullName, args.Length);

            return ActivatorUtilities.CreateInstance<T>(serviceProvider, args);
        }
    }
}
