-- Esquema de la base de datos SQLite del proyecto.
-- NOTA: la aplicación ejecuta esta misma sentencia automáticamente al iniciar
-- (ver Data/ConexionDb.cs), por lo que NO es necesario correr este script a mano.
-- Se incluye como documentación y para poder inspeccionar la BD con DB Browser for SQLite.

CREATE TABLE IF NOT EXISTS Videojuegos (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Titulo      TEXT    NOT NULL,
    Plataforma  TEXT    NOT NULL,
    Genero      TEXT    NOT NULL,
    Anio        INTEGER NOT NULL,
    Precio      REAL    NOT NULL CHECK (Precio >= 0),
    Cantidad    INTEGER NOT NULL CHECK (Cantidad >= 0)
);

-- Datos de ejemplo opcionales (descomentar para probar manualmente):
-- INSERT INTO Videojuegos (Titulo, Plataforma, Genero, Anio, Precio, Cantidad) VALUES
--   ('The Legend of Zelda: Breath of the Wild', 'Nintendo Switch', 'Aventura', 2017, 59.99, 3),
--   ('God of War', 'PlayStation 4', 'Acción', 2018, 39.99, 2);
