# Plantilla de Monolito Modular con CQRS, Wolverine y Versionado de API

## Descripción General

Este proyecto es una plantilla para construir aplicaciones .NET 8 bajo el enfoque de **monolito modular**. Cada módulo es autónomo, con su propia infraestructura, lógica de aplicación y presentación, pero todos conviven en el mismo proceso y base de código. La comunicación entre módulos se realiza mediante CQRS y eventos usando Wolverine, lo que permite desacoplar la lógica y facilitar la escalabilidad futura.

---

## Arquitectura

- **Monolito Modular:** Cada módulo (por ejemplo, `Module.User`, `Module.Auth`) es un ensamblado independiente, pero todos se despliegan juntos en la misma aplicación.
- **CQRS + Event Sourcing:** Los módulos se comunican mediante comandos, consultas y eventos usando Wolverine, desacoplando la lógica de negocio y la persistencia.
- **Versionado de API:** Gestionado con `Asp.Versioning`, permitiendo múltiples versiones y evolución controlada de los endpoints.
- **Logging Centralizado:** Uso de `LoggerManager` para centralizar logs de debug, info, warning y error.
- **Persistencia Resiliente:** Integración con PostgreSQL y políticas de reintento para conexiones robustas.

---

## Estructura de un Módulo

Cada módulo debe seguir la siguiente estructura:

---

## Reglas para Crear un Nuevo Módulo

1. **Crear la carpeta `Module.[Nombre]`** bajo `Modules`.
2. **Agregar un archivo `[Nombre]ModuleStartup.cs`** con un método `Add[Nombre]Module` para registrar servicios, contexto y mapeos.
3. **Definir carpetas para Application, Domain, Infrastructure y Presentation** siguiendo el ejemplo de los módulos existentes.
4. **Registrar el módulo** en `API/Extensions/ConfigurationModules.cs` llamando a `services.Add[Nombre]Module(config);`.

---

## Reglas para Controladores

- Los controladores deben estar en `Presentation/Controllers` y terminar con `Controller`.
- Se detectan automáticamente si cumplen con el sufijo o tienen el atributo `[Controller]`.
- Deben devolver respuestas usando el estándar de `ApiResponse` (ver sección de Respuestas).

---

## Comunicación entre Módulos (CQRS + Wolverine)

- **Queries y Commands:** 
  - Las clases deben terminar en `Queries` o `Commands`.
  - Deben tener el atributo `[WModuleHandler]`.
  - Deben implementar un método público `Handle`.
- **Eventos:** 
  - Los eventos se publican y consumen usando Wolverine, permitiendo integración asíncrona entre módulos.
- **Descubrimiento Automático:** 
  - El sistema escanea todos los ensamblados y registra automáticamente los handlers válidos (ver `WolverineDiscoveryExtensions.cs`).

---

## Configuración de Handlers, Queries y Commands

- Para que un handler sea registrado:
  - Debe ser una clase concreta (no abstracta).
  - Nombre debe terminar en `Query` o `Command`.
  - Decorado con `[WModuleHandler]`.
  - Debe tener un método público `Handle`.
- Ejemplo:

---

## Estándar de Respuesta

- Todas las respuestas deben usar el objeto `ApiResponse`:
  - Incluye: `Message`, `StatusCode`, `Details` y opcionalmente `Pagination`.
  - Utilizar la extensión `CustomResponse` para construir respuestas uniformes.
- Ejemplo:

---

## Versionado de API

- Configurado en `VersioningConfigure.cs`.
- Soporta:
  - Header: `X-version`
  - QueryString: `ApiVersion`
  - Segmento de URL
- La versión por defecto se define en la configuración (`API_Versioning:DEFAULT_VERSION`).
- Las versiones se reportan automáticamente en las respuestas.

---

## Logging

- Usar `ILoggerManager` para registrar logs.
- Métodos disponibles: `LogDebug`, `LogInfo`, `LogWarn`, `LogError`.
- Los mensajes se truncan a 500 caracteres para evitar saturación de logs.

---

## Persistencia y Conexión

- Cada módulo puede tener su propio `DbContext`.
- Uso de PostgreSQL con políticas de reintento para conexiones resilientes.
- La cadena de conexión se define en la configuración (`StringConnection`).

---

## Cómo Usar la Plantilla

1. Clona el repositorio.
2. Crea un nuevo módulo siguiendo la estructura y reglas descritas.
3. Registra el módulo en `ConfigurationModules.cs`.
4. Implementa tus comandos, queries y eventos usando Wolverine.
5. Usa el estándar de respuesta y logging.
6. Configura el versionado de API según tus necesidades.

---

## Recomendaciones

- Mantén cada módulo lo más independiente posible.
- Usa eventos para integración entre módulos.
- Versiona tus endpoints para evitar breaking changes.
- Centraliza la gestión de logs y respuestas.

---

> Para dudas técnicas, revisa los archivos de ejemplo y la documentación inline en el código.

