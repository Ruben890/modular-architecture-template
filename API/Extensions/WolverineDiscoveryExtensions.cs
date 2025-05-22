using Shared.Core.Attributes;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Wolverine.Configuration;

namespace API.Extensions
{
    public static class WolverineModuleHandlerDiscoveryExtensions
    {
        // Cache concurrente para almacenar resultados de si un assembly contiene handlers modulares válidos
        private static readonly ConcurrentDictionary<Assembly, Lazy<bool>> HandlerCache = new();

        /// <summary>
        /// Incluye en Wolverine solo los assemblies que contienen handlers modulares válidos:
        /// - Clases que terminan en "Queries" o "Commands"
        /// - Que tengan el atributo [WModuleHandler]
        /// - Que tengan un método público "Handle"
        /// 
        /// Utiliza paralelismo controlado para aprovechar varios núcleos sin saturar la memoria.
        /// Usa cache con Lazy para evitar cálculos repetidos.
        /// </summary>
        /// <param name="discovery">El objeto HandlerDiscovery de Wolverine.</param>
        /// <returns>El mismo objeto HandlerDiscovery para permitir encadenamiento.</returns>
        public static HandlerDiscovery IncludeAllWolverineModuleHandlers(this HandlerDiscovery discovery)
        {
            // Obtiene todos los assemblies cargados que no sean dinámicos y tengan nombre válido
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                .Distinct()
                .ToArray();

            // Limita el grado de paralelismo a la mitad de los núcleos disponibles para evitar saturación
            int maxDegree = Math.Max(1, Environment.ProcessorCount / 2);

            // Particiona los assemblies para procesarlos en paralelo sin sobrecargar memoria
            var partitioner = Partitioner.Create(assemblies, true);

            Parallel.ForEach(partitioner, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, assembly =>
            {
                try
                {
                    // Usa cache para evitar repetir el trabajo de reflección sobre el mismo assembly
                    var hasValidHandler = HandlerCache.GetOrAdd(assembly, asm => new Lazy<bool>(() =>
                    {
                        try
                        {
                            // Verifica que existan tipos que cumplan con los requisitos de handler modular
                            return asm.GetTypes().Any(type =>
                                type.IsClass &&
                                !type.IsAbstract &&
                                (type.Name.EndsWith("Queries", StringComparison.OrdinalIgnoreCase) ||
                                 type.Name.EndsWith("Commands", StringComparison.OrdinalIgnoreCase)) &&
                                type.GetCustomAttribute<WModuleHandlerAttribute>() != null &&
                                type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                    .Any(m => m.Name == "Handle"));
                        }
                        catch
                        {
                            // Si falla al obtener tipos, no considerar assembly
                            return false;
                        }
                    }));

                    if (hasValidHandler.Value)
                    {
                        discovery.IncludeAssembly(assembly);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.WriteLine($"[WolverineDiscovery] Could not load types from assembly {assembly.FullName}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[WolverineDiscovery] Error scanning assembly {assembly.FullName}: {ex.Message}");
                }
            });

            return discovery;
        }
    }
}
