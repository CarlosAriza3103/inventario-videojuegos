using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Data;

/// <summary>
/// Se encarga exclusivamente de la conexión con SQLite y de crear la base de datos
/// la primera vez que se ejecuta el programa.
/// </summary>
public class ConexionDb
{
    private const string SqlCrearTabla = @"
        CREATE TABLE IF NOT EXISTS Videojuegos (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Titulo      TEXT    NOT NULL,
            Plataforma  TEXT    NOT NULL,
            Genero      TEXT    NOT NULL,
            Anio        INTEGER NOT NULL,
            Precio      REAL    NOT NULL CHECK (Precio >= 0),
            Cantidad    INTEGER NOT NULL CHECK (Cantidad >= 0)
        );";

    /// <summary>Ruta completa del archivo .db.</summary>
    public string RutaArchivo { get; }

    private readonly string _cadenaConexion;

    /// <param name="rutaArchivo">
    /// Ruta del archivo de base de datos. Si es null se usa la carpeta de datos
    /// del usuario, para que funcione igual en Windows, Linux y macOS.
    /// </param>
    public ConexionDb(string? rutaArchivo = null)
    {
        RutaArchivo = rutaArchivo ?? ObtenerRutaPorDefecto();

        _cadenaConexion = new SqliteConnectionStringBuilder
        {
            DataSource = RutaArchivo,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    /// <summary>Crea una conexión nueva. Quien la use debe cerrarla (using).</summary>
    public SqliteConnection CrearConexion() => new SqliteConnection(_cadenaConexion);

    /// <summary>
    /// Crea la carpeta, el archivo de base de datos y la tabla si no existen.
    /// Es seguro llamarlo en cada arranque.
    /// </summary>
    public void InicializarBaseDeDatos()
    {
        string? carpeta = Path.GetDirectoryName(RutaArchivo);
        if (!string.IsNullOrEmpty(carpeta))
            Directory.CreateDirectory(carpeta);

        using var conexion = CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlCrearTabla;
        comando.ExecuteNonQuery();
    }

    private static string ObtenerRutaPorDefecto()
    {
        string baseDatos = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(baseDatos))
            baseDatos = AppContext.BaseDirectory;

        return Path.Combine(baseDatos, "InventarioVideojuegos", "videojuegos.db");
    }
}
