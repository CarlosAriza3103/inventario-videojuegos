# Inventario de Videojuegos

Parcial final - Electiva de Profundización I
Profesor: Eduardo Pimienta León
Estudiante: Carlos David Ariza Ospino

## Información general

Este proyecto es una aplicación de escritorio para llevar el inventario de una colección o tienda de videojuegos. Cada juego se guarda con su título, plataforma, género, año de lanzamiento, precio y cantidad disponible.

La idea nació de un problema muy común: llevar este control en papel o en hojas sueltas es desordenado y es fácil cometer errores o perder datos. Con esta aplicación toda la información queda en un solo lugar, se valida lo que se escribe y los datos se conservan aunque se cierre el programa.

El objetivo principal fue construir una aplicación sencilla pero completa, que permita registrar, consultar, modificar y eliminar videojuegos, aplicando Programación Orientada a Objetos, una interfaz gráfica con Avalonia UI y una base de datos SQLite manejada con ADO.NET.

## Funcionalidades

Con la aplicación se puede:

- Registrar un videojuego nuevo llenando el formulario.
- Ver todos los juegos guardados en una tabla, ordenados por título.
- Buscar juegos por título, plataforma o género.
- Modificar un juego: se selecciona en la tabla, sus datos pasan al formulario, se cambian y se guardan.
- Eliminar un juego, con una ventana que pide confirmación antes de borrarlo.
- Ver en la parte de abajo cuántos títulos hay y el valor total del inventario.

La aplicación también valida los datos. No deja guardar si el título está vacío, si el año no es válido, si el precio o la cantidad no son números o son negativos, ni si ya existe el mismo juego en la misma plataforma. Cuando hay un error muestra un mensaje claro y el programa no se cierra. Cuando una operación sale bien, lo indica en la barra inferior.

## Tecnologías utilizadas

- C# con .NET 8
- Avalonia UI 11.3.3 para la interfaz gráfica (archivos .axaml)
- ADO.NET con el paquete Microsoft.Data.Sqlite 8.0.0
- SQLite como base de datos
- Paquetes de Avalonia: Avalonia, Avalonia.Desktop, Avalonia.Themes.Fluent y Avalonia.Controls.DataGrid (este último se usa para la tabla)

No se usó Entity Framework ni ningún otro ORM, todo el acceso a datos se hace con ADO.NET.

## Estructura del proyecto

```
InventarioVideojuegos/
    InventarioVideojuegos.csproj
    Program.cs
    App.axaml
    App.axaml.cs
    Models/
        Videojuego.cs
    Data/
        ConexionDb.cs
        IVideojuegoDAO.cs
        VideojuegoDAO.cs
    Services/
        VideojuegoServicio.cs
        ValidacionException.cs
    Views/
        MainWindow.axaml
        MainWindow.axaml.cs
        Dialogo.cs
    Database/
        schema.sql
    evidencias/
    README.md
    .gitignore
```

Organicé el código en capas para que cada parte tenga una sola responsabilidad:

- **Videojuego (Models):** representa un juego con sus datos. Además valida sus propios valores, por ejemplo que el título no esté vacío o que el precio no sea negativo.
- **ConexionDb (Data):** se encarga de la conexión con SQLite y de crear la base de datos y la tabla la primera vez.
- **IVideojuegoDAO y VideojuegoDAO (Data):** la interfaz define qué operaciones existen y la clase VideojuegoDAO las implementa con ADO.NET. Aquí está el CRUD.
- **VideojuegoServicio (Services):** recibe lo que escribió el usuario, lo convierte y lo valida, aplica las reglas (como no repetir juegos) y le pide al DAO que guarde o consulte.
- **ValidacionException (Services):** una excepción propia para diferenciar un dato mal escrito por el usuario de un error técnico.
- **MainWindow (Views):** la ventana principal. Maneja los eventos de los botones y muestra los resultados, pero no tiene código SQL.
- **Dialogo (Views):** las ventanas emergentes de mensajes y de confirmación.

El flujo es: la ventana llama al servicio, el servicio llama al DAO y el DAO habla con SQLite. Así, por ejemplo, cuando se pulsa "Agregar videojuego", la ventana toma los datos del formulario, el servicio los valida, el DAO los guarda y la ventana informa el resultado.

### Dónde se aplica la POO

- **Clases y objetos:** Videojuego, VideojuegoDAO, VideojuegoServicio, ConexionDb y MainWindow.
- **Atributos y propiedades:** la clase Videojuego tiene campos privados (por ejemplo _titulo y _precio) que se usan a través de propiedades.
- **Constructores:** Videojuego tiene un constructor para juegos nuevos y otro para juegos que ya vienen de la base de datos con su Id. El DAO y el servicio reciben lo que necesitan por constructor.
- **Encapsulamiento:** las propiedades validan el valor antes de aceptarlo, y el Id solo se puede asignar desde dentro de la clase.
- **Interfaces y abstracción:** el servicio trabaja con IVideojuegoDAO y no directamente con SQLite, así la lógica no depende de cómo se guarden los datos.
- **Separación de responsabilidades:** interfaz, lógica y datos están en clases y carpetas distintas.

## Instalación de dependencias

1. Instalar el SDK de .NET 8 desde https://dotnet.microsoft.com/download. Se puede comprobar con el comando `dotnet --version`, que debe mostrar una versión 8.x.x.
2. Instalar Git desde https://git-scm.com/downloads.
3. Clonar el repositorio:

```
git clone https://github.com/TU_USUARIO/TU_REPOSITORIO.git
cd TU_REPOSITORIO
```

4. Restaurar las dependencias (descarga los paquetes que están declarados en el archivo .csproj):

```
dotnet restore
```

No hace falta instalar SQLite por aparte, porque el paquete Microsoft.Data.Sqlite ya lo incluye. Solo se necesita internet la primera vez para descargar los paquetes. No se requiere ninguna configuración adicional.

## Ejecución del programa

Desde la carpeta donde está el archivo InventarioVideojuegos.csproj, ejecutar:

```
dotnet restore
dotnet run
```

Se abre la ventana de la aplicación. Si al clonar el repositorio el proyecto quedó dentro de otra carpeta, primero hay que entrar a ella con `cd`.

## Base de datos

La aplicación guarda los videojuegos en una tabla llamada Videojuegos, con estos campos:

- Id: número único que se genera automáticamente (clave primaria).
- Titulo: nombre del juego.
- Plataforma: consola o sistema en el que se juega.
- Genero: género del juego.
- Anio: año de lanzamiento.
- Precio: precio de cada unidad (no puede ser negativo).
- Cantidad: unidades disponibles en el inventario (no puede ser negativa).

La conexión se hace en la clase ConexionDb, que arma la cadena de conexión y entrega objetos SqliteConnection. Cada operación abre su conexión dentro de un bloque `using`, para que se cierre sola al terminar.

Las operaciones CRUD están en la clase VideojuegoDAO:

- Create: método Insertar (INSERT)
- Read: métodos ObtenerTodos y Buscar (SELECT)
- Update: método Actualizar (UPDATE)
- Delete: método Eliminar (DELETE)

Todos los datos que escribe el usuario se envían como parámetros (por ejemplo @titulo o @precio) y no pegados dentro del texto de la consulta SQL, para evitar problemas de seguridad.

La base de datos se crea automáticamente al abrir el programa por primera vez, así que no hay que ejecutar ningún procedimiento inicial. El archivo Database/schema.sql tiene el mismo esquema, y está solo como documentación.

El archivo de la base de datos se llama videojuegos.db y se guarda en la carpeta de datos del usuario, dentro de una carpeta InventarioVideojuegos. En Windows queda en `C:\Users\<usuario>\AppData\Local\InventarioVideojuegos\`. Gracias a esto, los datos siguen ahí cuando se cierra y se vuelve a abrir la aplicación.

## Problemas encontrados y soluciones

**1. La tabla no se mostraba.** Al principio el control DataGrid no aparecía como esperaba. Descubrí que en Avalonia 11 el DataGrid no viene en el paquete base. Lo solucioné instalando el paquete Avalonia.Controls.DataGrid y agregando su estilo en el archivo App.axaml.

**2. Errores al recargar la tabla.** Después de guardar o eliminar un registro, la tabla se volvía a cargar y eso disparaba el evento de selección, lo que cambiaba el estado de los botones sin querer. Lo resolví usando una variable de control y creando un método LimpiarFormulario que deja los botones y los campos en el estado correcto.

**3. Precios con coma o con punto.** Dependiendo de cómo escribiera el usuario el precio (59,99 o 59.99), el programa podía fallar al convertirlo a número. Lo solucioné cambiando la coma por punto en el servicio y convirtiendo el texto con CultureInfo.InvariantCulture.
