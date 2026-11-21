using System;
using System.Collections.Generic;
using InventarioVideojuegos.Models;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Data;

/// <summary>
/// Acceso a datos (DAO) de videojuegos usando ADO.NET puro con Microsoft.Data.Sqlite.
/// Aquí viven las cuatro operaciones CRUD. Todas las consultas con datos del usuario
/// usan parámetros (@nombre) para evitar inyección SQL, y cada conexión se libera con "using".
/// </summary>
public class VideojuegoDAO : IVideojuegoDAO
{
    private const string Columnas = "Id, Titulo, Plataforma, Genero, Anio, Precio, Cantidad";

    private readonly ConexionDb _conexionDb;

    public VideojuegoDAO(ConexionDb conexionDb)
    {
        _conexionDb = conexionDb;
    }

    public void Inicializar() => _conexionDb.InicializarBaseDeDatos();

    // ---------------------------------------------------------------- CREATE
    public int Insertar(Videojuego juego)
    {
        const string sql = @"
            INSERT INTO Videojuegos (Titulo, Plataforma, Genero, Anio, Precio, Cantidad)
            VALUES (@titulo, @plataforma, @genero, @anio, @precio, @cantidad);
            SELECT last_insert_rowid();";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        AgregarParametros(comando, juego);

        int id = Convert.ToInt32(comando.ExecuteScalar());
        juego.AsignarId(id);
        return id;
    }

    // ------------------------------------------------------------------ READ
    public List<Videojuego> ObtenerTodos()
    {
        string sql = $"SELECT {Columnas} FROM Videojuegos ORDER BY Titulo COLLATE NOCASE;";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;

        return LeerLista(comando);
    }

    public List<Videojuego> Buscar(string texto)
    {
        string sql = $@"
            SELECT {Columnas} FROM Videojuegos
            WHERE Titulo LIKE @filtro OR Plataforma LIKE @filtro OR Genero LIKE @filtro
            ORDER BY Titulo COLLATE NOCASE;";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        comando.Parameters.AddWithValue("@filtro", $"%{texto}%");

        return LeerLista(comando);
    }

    // ---------------------------------------------------------------- UPDATE
    public bool Actualizar(Videojuego juego)
    {
        const string sql = @"
            UPDATE Videojuegos
            SET Titulo = @titulo, Plataforma = @plataforma, Genero = @genero,
                Anio = @anio, Precio = @precio, Cantidad = @cantidad
            WHERE Id = @id;";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        AgregarParametros(comando, juego);
        comando.Parameters.AddWithValue("@id", juego.Id);

        return comando.ExecuteNonQuery() > 0;
    }

    // ---------------------------------------------------------------- DELETE
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM Videojuegos WHERE Id = @id;";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        comando.Parameters.AddWithValue("@id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    // ------------------------------------------------------------ AUXILIARES
    public bool ExisteDuplicado(string titulo, string plataforma, int idExcluido)
    {
        const string sql = @"
            SELECT COUNT(*) FROM Videojuegos
            WHERE LOWER(Titulo) = LOWER(@titulo)
              AND LOWER(Plataforma) = LOWER(@plataforma)
              AND Id <> @id;";

        using var conexion = _conexionDb.CrearConexion();
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        comando.Parameters.AddWithValue("@titulo", titulo.Trim());
        comando.Parameters.AddWithValue("@plataforma", plataforma.Trim());
        comando.Parameters.AddWithValue("@id", idExcluido);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    private static void AgregarParametros(SqliteCommand comando, Videojuego juego)
    {
        comando.Parameters.AddWithValue("@titulo", juego.Titulo);
        comando.Parameters.AddWithValue("@plataforma", juego.Plataforma);
        comando.Parameters.AddWithValue("@genero", juego.Genero);
        comando.Parameters.AddWithValue("@anio", juego.Anio);
        comando.Parameters.AddWithValue("@precio", (double)juego.Precio);
        comando.Parameters.AddWithValue("@cantidad", juego.Cantidad);
    }

    private static List<Videojuego> LeerLista(SqliteCommand comando)
    {
        var lista = new List<Videojuego>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Videojuego(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                lector.GetString(3),
                lector.GetInt32(4),
                Convert.ToDecimal(lector.GetDouble(5)),
                lector.GetInt32(6)));
        }

        return lista;
    }
}
