using Shared.Core.Attributes;
using System.Reflection;
using Wolverine.Configuration;

namespace API.Extensions
{
    public static class WolverineDiscoveryExtensions
    {
        public static HandlerDiscovery IncludeAllModuleHandlers(this HandlerDiscovery discovery)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                .Distinct()
                .ToArray();

            var handlerAssemblies = assemblies
                .Where(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes().Any(t =>
                            t.GetCustomAttribute<ModuleHandlerAttribute>() != null);
                    }
                    catch
                    {
                        return false;
                    }
                });

            foreach (var assembly in handlerAssemblies)
            {
                discovery.IncludeAssembly(assembly);
            }

            return discovery;
        }
    }
}
