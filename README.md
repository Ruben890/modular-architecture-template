# Plantilla de Monolito Modular con CQRS, Wolverine y Versionado de API

## Descripci�n General

Este proyecto es una plantilla para construir aplicaciones .NET 8 bajo el enfoque de **monolito modular**. Cada m�dulo es aut�nomo, con su propia infraestructura, l�gica de aplicaci�n y presentaci�n, pero todos conviven en el mismo proceso y base de c�digo. La comunicaci�n entre m�dulos se realiza mediante CQRS y eventos usando Wolverine, lo que permite desacoplar la l�gica y facilitar la escalabilidad futura.

---

## Arquitectura

- **Monolito Modular:** Cada m�dulo (por ejemplo, `Module.User`, `Module.Auth`) es un ensamblado independiente, pero todos se despliegan juntos en la misma aplicaci�n.
- **CQRS + Event Sourcing:** Los m�dulos se comunican mediante comandos, consultas y eventos usando Wolverine, desacoplando la l�gica de negocio y la persistencia.
- **Versionado de API:** Gestionado con `Asp.Versioning`, permitiendo m�ltiples versiones y evoluci�n controlada de los endpoints.
- **Logging Centralizado:** Uso de `LoggerManager` para centralizar logs de debug, info, warning y error.
- **Persistencia Resiliente:** Integraci�n con PostgreSQL y pol�ticas de reintento para conexiones robustas.

---

## Estructura de un M�dulo

Cada m�dulo debe seguir la siguiente estructura:

---

## Reglas para Crear un Nuevo M�dulo

1. **Crear la carpeta `Module.[Nombre]`** bajo `Modules`.
2. **Agregar un archivo `[Nombre]ModuleStartup.cs`** con un m�todo `Add[Nombre]Module` para registrar servicios, contexto y mapeos.
3. **Definir carpetas para Application, Domain, Infrastructure y Presentation** siguiendo el ejemplo de los m�dulos existentes.
4. **Registrar el m�dulo** en `API/Extensions/ConfigurationModules.cs` llamando a `services.Add[Nombre]Module(config);`.

---

## Reglas para Controladores

- Los controladores deben estar en `Presentation/Controllers` y terminar con `Controller`.
- Se detectan autom�ticamente si cumplen con el sufijo o tienen el atributo `[Controller]`.
- Deben devolver respuestas usando el est�ndar de `ApiResponse` (ver secci�n de Respuestas).

---

## Comunicaci�n entre M�dulos (CQRS + Wolverine)

- **Queries y Commands:** 
  - Las clases deben terminar en `Queries` o `Commands`.
  - Deben tener el atributo `[WModuleHandler]`.
  - Deben implementar un m�todo p�blico `Handle`.
- **Eventos:** 
  - Los eventos se publican y consumen usando Wolverine, permitiendo integraci�n as�ncrona entre m�dulos.
- **Descubrimiento Autom�tico:** 
  - El sistema escanea todos los ensamblados y registra autom�ticamente los handlers v�lidos (ver `WolverineDiscoveryExtensions.cs`).

---

## Configuraci�n de Handlers, Queries y Commands

- Para que un handler sea registrado:
  - Debe ser una clase concreta (no abstracta).
  - Nombre debe terminar en `Queries` o `Commands`.
  - Decorado con `[WModuleHandler]`.
  - Debe tener un m�todo p�blico `Handle`.
- Ejemplo:

---

## Est�ndar de Respuesta

- Todas las respuestas deben usar el objeto `ApiResponse`:
  - Incluye: `Message`, `StatusCode`, `Details` y opcionalmente `Pagination`.
  - Utilizar la extensi�n `CustomResponse` para construir respuestas uniformes.
- Ejemplo:

---

## Versionado de API

- Configurado en `VersioningConfigure.cs`.
- Soporta:
  - Header: `X-version`
  - QueryString: `ApiVersion`
  - Segmento de URL
- La versi�n por defecto se define en la configuraci�n (`API_Versioning:DEFAULT_VERSION`).
- Las versiones se reportan autom�ticamente en las respuestas.

---

## Logging

- Usar `ILoggerManager` para registrar logs.
- M�todos disponibles: `LogDebug`, `LogInfo`, `LogWarn`, `LogError`.
- Los mensajes se truncan a 500 caracteres para evitar saturaci�n de logs.

---

## Persistencia y Conexi�n

- Cada m�dulo puede tener su propio `DbContext`.
- Uso de PostgreSQL con pol�ticas de reintento para conexiones resilientes.
- La cadena de conexi�n se define en la configuraci�n (`StringConnection`).

---

## C�mo Usar la Plantilla

1. Clona el repositorio.
2. Crea un nuevo m�dulo siguiendo la estructura y reglas descritas.
3. Registra el m�dulo en `ConfigurationModules.cs`.
4. Implementa tus comandos, queries y eventos usando Wolverine.
5. Usa el est�ndar de respuesta y logging.
6. Configura el versionado de API seg�n tus necesidades.

---

## Recomendaciones

- Mant�n cada m�dulo lo m�s independiente posible.
- Usa eventos para integraci�n entre m�dulos.
- Versiona tus endpoints para evitar breaking changes.
- Centraliza la gesti�n de logs y respuestas.

---

> Para dudas t�cnicas, revisa los archivos de ejemplo y la documentaci�n inline en el c�digo.

