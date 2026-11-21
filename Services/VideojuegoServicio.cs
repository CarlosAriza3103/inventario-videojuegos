using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InventarioVideojuegos.Data;
using InventarioVideojuegos.Models;

namespace InventarioVideojuegos.Services;

/// <summary>
/// Capa de lógica del programa. Recibe los textos de la interfaz, los convierte y valida,
/// aplica las reglas (por ejemplo, no repetir título en la misma plataforma) y delega
/// el almacenamiento al DAO. La ventana nunca habla directamente con la base de datos.
/// </summary>
public class VideojuegoServicio
{
    public static readonly IReadOnlyList<string> Plataformas = new[]
    {
        "PC", "PlayStation 5", "PlayStation 4", "Xbox Series X/S", "Xbox One",
        "Nintendo Switch", "Nintendo 3DS", "Móvil", "Otra"
    };

    public static readonly IReadOnlyList<string> Generos = new[]
    {
        "Acción", "Aventura", "RPG", "Estrategia", "Deportes", "Carreras",
        "Shooter", "Plataformas", "Simulación", "Terror", "Puzzle", "Otro"
    };

    private readonly IVideojuegoDAO _dao;

    public VideojuegoServicio(IVideojuegoDAO dao)
    {
        _dao = dao;
    }

    public void Inicializar() => _dao.Inicializar();

    public List<Videojuego> Listar(string? filtro = null)
    {
        return string.IsNullOrWhiteSpace(filtro)
            ? _dao.ObtenerTodos()
            : _dao.Buscar(filtro.Trim());
    }

    public Videojuego Registrar(string? titulo, string? plataforma, string? genero,
                                string? anio, string? precio, string? cantidad)
    {
        Videojuego juego = Construir(0, titulo, plataforma, genero, anio, precio, cantidad);

        if (_dao.ExisteDuplicado(juego.Titulo, juego.Plataforma, 0))
            throw new ValidacionException($"Ya existe \"{juego.Titulo}\" para {juego.Plataforma}.");

        _dao.Insertar(juego);
        return juego;
    }

    public Videojuego Actualizar(int id, string? titulo, string? plataforma, string? genero,
                                 string? anio, string? precio, string? cantidad)
    {
        Videojuego juego = Construir(id, titulo, plataforma, genero, anio, precio, cantidad);

        if (_dao.ExisteDuplicado(juego.Titulo, juego.Plataforma, id))
            throw new ValidacionException($"Ya existe otro registro \"{juego.Titulo}\" para {juego.Plataforma}.");

        if (!_dao.Actualizar(juego))
            throw new ValidacionException("El registro ya no existe en la base de datos.");

        return juego;
    }

    public void Eliminar(int id)
    {
        if (!_dao.Eliminar(id))
            throw new ValidacionException("El registro ya no existe en la base de datos.");
    }

    public decimal CalcularValorInventario(IEnumerable<Videojuego> juegos) =>
        juegos.Sum(j => j.ValorTotal);

    /// <summary>
    /// Convierte los textos del formulario en un objeto Videojuego válido.
    /// Cualquier problema se reporta como ValidacionException con un mensaje claro.
    /// </summary>
    private static Videojuego Construir(int id, string? titulo, string? plataforma, string? genero,
                                        string? anioTexto, string? precioTexto, string? cantidadTexto)
    {
        if (!int.TryParse(anioTexto?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int anio))
            throw new ValidacionException("El año debe ser un número entero (ej. 2021).");

        string precioNormalizado = (precioTexto ?? string.Empty).Trim().Replace(',', '.');
        if (!decimal.TryParse(precioNormalizado, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
            throw new ValidacionException("El precio debe ser un número (ej. 59.99).");

        if (!int.TryParse(cantidadTexto?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cantidad))
            throw new ValidacionException("La cantidad debe ser un número entero (ej. 3).");

        try
        {
            return new Videojuego(id, titulo ?? string.Empty, plataforma ?? string.Empty,
                                  genero ?? string.Empty, anio, precio, cantidad);
        }
        catch (ArgumentException ex)
        {
            // El modelo valida cada propiedad; aquí traducimos su error a uno de negocio.
            throw new ValidacionException(ex.Message);
        }
    }
}
