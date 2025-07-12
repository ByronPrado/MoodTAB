# Proyecto WebConTablas

## Descripción general

Este proyecto es una aplicación web ASP.NET Core con arquitectura modular para facilitar su mantenimiento y crecimiento.  
Incluye controladores, servicios, modelos, acceso a datos con Entity Framework Core, vistas Razor y recursos estáticos.

---

## Estructura del proyecto

- `/Controllers`: Controladores MVC y API, que gestionan rutas y solicitudes HTTP.
- `/Models`: Clases que representan las entidades y modelos de datos.
- `/Helpers`: Clases que representan funciones auxiliares de utilidad.
- `/Data`: Contiene `ApplicationDbContext`, migraciones y clases de acceso a datos.
- `/Services`: Implementación de la lógica de negocio, dividida en áreas funcionales.
- `/Filters`: Filtros para validación, autorización y otros middleware específicos.
- `/Views`: Vistas Razor para la UI (interfaz de usuario).
- `/wwwroot`: Archivos estáticos (CSS, JS, imágenes).
- `/Properties`: Metadata y configuración del ensamblado.
- `/bin` y `/obj`: Carpetas generadas automáticamente con compilados y archivos temporales (no versionar).

---

## Archivos principales

- `Program.cs`: Punto de entrada de la aplicación, configuración del servidor y middleware.
- `appsettings.json` y `appsettings.Development.json`: Configuración general y por ambiente.
- `WebConTablas.csproj` y `WebConTablas.sln`: Archivos de proyecto y solución .NET.

---

## Documentación y convenciones

- Cada carpeta contiene un archivo `README.md` explicando su función y reglas de uso.
- En el código se utilizan comentarios XML para facilitar la comprensión desde el IDE.
- Se recomienda seguir la convención de nombrado con interfaces `I*` y servicios con nombres claros.
- Para API REST se sugiere usar Swagger para documentación automática.

---

# MoodTAB (Aplicación móvil)

## Descripción

MoodTAB es una aplicación móvil construida con .NET MAUI que utiliza una base de datos local SQLite para persistencia.

---

## Requerimientos y recursos

- Uso de base de datos local SQLite para almacenamiento de datos.
- Frameworks y paquetes NuGet recomendados:  
  - [sqlite-net-pcl](https://www.nuget.org/packages/sqlite-net-pcl/)  
  - [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm)

---

## Recursos útiles para desarrollo

- Tutorial en YouTube: [Base de datos local en .NET MAUI](https://www.youtube.com/watch?v=EitcH1aSivc&ab_channel=HumbertoJaimes)
- Documentación oficial Microsoft:  
  https://learn.microsoft.com/es-es/dotnet/maui/data-cloud/database-sqlite?view=net-maui-8.0
- PDF con prerrequisitos para MAUI: [PreReqMAUI.pdf](https://github.com/user-attachments/files/20543161/PreReqMAUI.pdf)
- Instalación y configuración del emulador Android (puede tardar):  
  [Google Drive con recursos necesarios](https://drive.google.com/drive/folders/10803r0ZeoeCFGvOI1vqsTHBPts5nK1Wb)

---

## Ejecución

Para ejecutar la aplicación MoodTAB en el emulador Android:

1. Asegurarse de tener configurado el emulador correctamente.
2. Abrir el proyecto en Visual Studio.
3. Ejecutar la aplicación iniciando desde `MainPage.xaml`.

---

## Estado actual

El proyecto está en una etapa inicial y cuenta con la estructura base para la gestión de datos locales mediante SQLite, junto con la configuración del patrón MVVM usando CommunityToolkit.

---

## Contacto y soporte

Para dudas o contribuciones, por favor contactar a los responsables del proyecto o abrir un issue en el repositorio.

---

*Este README será actualizado conforme el proyecto avance.*

