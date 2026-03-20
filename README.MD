# 📘 Práctica 2 - CitizenRegistryApi

## 🚀 Descripción

**CitizenRegistryApi** es una API Web desarrollada en **ASP.NET Core** para gestionar ciudadanos dentro de un sistema de registro ciudadano.

La aplicación permite realizar operaciones **CRUD** sobre ciudadanos, es decir:

- 📄 Listar ciudadanos
- 🔍 Buscar un ciudadano por su CI
- ➕ Crear ciudadanos
- ✏️ Actualizar ciudadanos
- ❌ Eliminar ciudadanos

Cada ciudadano contiene los siguientes datos:

- `FirstName`
- `LastName`
- `CI`
- `BloodGroup`
- `PersonalAsset`

Al momento de crear un ciudadano, el sistema:

- 🎲 Asigna automáticamente un grupo sanguíneo aleatorio
- 🌐 Obtiene un objeto aleatorio desde una API externa para usarlo como `PersonalAsset`

## ✅ Funcionalidades principales

Este proyecto permite:

- Consultar todos los ciudadanos registrados
- Consultar un ciudadano específico por su CI
- Registrar nuevos ciudadanos
- Actualizar nombre y apellido de ciudadanos existentes
- Eliminar ciudadanos
- Guardar los datos en un archivo CSV
- Probar todos los endpoints desde Swagger
- Registrar eventos y errores usando Serilog

## 🛠️ Tecnologías utilizadas

- **ASP.NET Core Web API**
- **C#**
- **Swagger / Swashbuckle**
- **Newtonsoft.Json**
- **Serilog**
- **Archivo CSV** como persistencia local
- **HttpClient** para consumir API externa

## ▶️ Cómo ejecutar el proyecto

### 1. Clonar el repositorio
git clone <URL-del-repositorio>
    1.1 Entrar a la carpeta del proyecto 
    1.2 Restaurar dependencias
            dotnet restore
    1.3 Compilar el proyecto
            dotnet build
    1.4 Ejecutar la aplicación
            dotnet run

### 📚 Documentación de la API

Una vez ejecutado el proyecto, se puede acceder a la documentación interactiva desde Swagger.
### 📂 Persistencia de datos

La información de los ciudadanos se guarda en un archivo CSV.
Esto sirve para:
- Mantener los datos aunque la aplicación se cierre
- Evitar depender únicamente de memoria
- Simular una persistencia sencilla sin usar base de datos
### 🌐 Consumo de API externa

Cuando se registra un ciudadano, la aplicación consulta una API externa para obtener un objeto aleatorio que se asigna al campo PersonalAsset.

### 📝 Logging

Se utilizó Serilog para registrar eventos y errores del sistema.
Los logs se guardan en:
- consola
- archivos rotativos

### 🔀 Uso de ramas
Para el desarrollo del proyecto trabajé con ramas en Git para mantener una mejor organización del trabajo.
Durante el proceso tuve un inconveniente inicial con la creación de ramas principales como main y develop, pero lo resolví creando ambas ramas y apuntándolas correctamente al inicio del proyecto. Como el desarrollo se estaba realizando en una rama de trabajo, este problema no afectó el avance de la práctica.
Esto me permitió mantener el control de versiones y continuar el proyecto de forma ordenada.

## Aplicacion de los 12 factores 
1. Codebase
En este proyecto trabajé con una sola base de código versionada con Git.
Todo el desarrollo estuvo centralizado en un único repositorio, organizado mediante ramas. 

2. Dependencies
Las dependencias del proyecto están declaradas de forma explícita mediante paquetes NuGet.
Entre las principales están:
- `Swashbuckle.AspNetCore`
- `Newtonsoft.Json`
- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`

Esto facilita restaurar el proyecto en otra máquina sin depender de instalaciones manuales.

3. Config
La configuración del proyecto no está escrita directamente en la lógica del código.
Se maneja mediante appsettings.json, por ejemplo para:

- la ruta del archivo CSV
- la URL base de la API externa

Esto permite cambiar valores de configuración sin modificar el código fuente.

4. Processes

La aplicación funciona como un proceso stateless, asyns/await.
Cada solicitud HTTP se procesa de forma independiente.
Aunque temporalmente se cargan datos en memoria al trabajar con la lista de ciudadanos, la persistencia real se mantiene en el archivo CSV, por lo que la aplicación no depende únicamente del estado en memoria para conservar datos.
Como posible mejora, podrían existir otros procesos como:

- procesos administrativos para limpiar o cargar datos
- procesos en segundo plano para tareas más largas

5. Build, Release, Run

Mantuve separadas las etapas de construcción y ejecución con comandos como:
- dotnet build
- dotnet run


6. Backing Services

La API externa de objetos funciona como un servicio adjunto al proyecto.
La aplicación la consume como un recurso externo y no como algo embebido dentro del código.

7. Port Binding

La API se expone directamente mediante un puerto, por ejemplo:
http://localhost:5082
Eso permite que el servicio responda solicitudes HTTP de forma directa.

8. Concurrency

En este proyecto no se aplico concurrencia.
Sí utilicé async/await en la llamada a la API externa.
Si el sistema necesitara escalar más adelante, sería mejor reemplazar el CSV por una solución de persistencia centralizada.

9. Disposability

La aplicación inicia rápido y también puede detenerse sin complicaciones.
Además, el uso de Serilog con Log.CloseAndFlush() ayuda a cerrar correctamente los logs al finalizar.

10. Dev / Prod Parity

Mantuve una estructura bastante similar entre desarrollo y ejecución, usando el mismo código, las mismas dependencias y la misma forma de correr el proyecto con .NET.

11. Logs

Los logs se tratan como flujo de eventos.
Los configuré para mostrarse en consola y además guardarse en archivos de log rotativos.

12. Admin Processes

No implementé procesos administrativos separados como comandos independientes, pero podrían existir tareas aisladas como:
- limpiar el CSV
- cargar datos de prueba
- importar ciudadanos

Esas tareas podrían ejecutarse como procesos puntuales fuera del flujo normal de la API.

📮 Endpoints principales

`GET /Citizen`
Obtiene la lista de todos los ciudadanos registrados.

`GET /Citizen/{ci}`
Obtiene un ciudadano específico según su CI.

`POST /Citizen`
Crea un nuevo ciudadano.

`PUT /Citizen/{ci}`
Actualiza el nombre y apellido de un ciudadano existente.

`DELETE /Citizen/{ci}`
Elimina un ciudadano por su CI.
