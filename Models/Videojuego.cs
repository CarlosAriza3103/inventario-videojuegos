using System;

namespace InventarioVideojuegos.Models;

/// <summary>
/// Entidad principal del sistema. Representa un videojuego del inventario.
/// Aplica encapsulamiento: los campos son privados y cada propiedad valida
/// el valor antes de aceptarlo, así nunca existe un objeto en estado inválido.
/// </summary>
public class Videojuego
{
    private string _titulo = string.Empty;
    private string _plataforma = string.Empty;
    private string _genero = string.Empty;
    private int _anio;
    private decimal _precio;
    private int _cantidad;

    /// <summary>Identificador en la base de datos (0 si aún no se ha guardado).</summary>
    public int Id { get; private set; }

    public string Titulo
    {
        get => _titulo;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El título es obligatorio.");
            if (value.Trim().Length > 100)
                throw new ArgumentException("El título no puede superar los 100 caracteres.");
            _titulo = value.Trim();
        }
    }

    public string Plataforma
    {
        get => _plataforma;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Debes seleccionar una plataforma.");
            _plataforma = value.Trim();
        }
    }

    public string Genero
    {
        get => _genero;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Debes seleccionar un género.");
            _genero = value.Trim();
        }
    }

    public int Anio
    {
        get => _anio;
        set
        {
            int maximo = DateTime.Now.Year + 1;
            if (value < 1970 || value > maximo)
                throw new ArgumentException($"El año de lanzamiento debe estar entre 1970 y {maximo}.");
            _anio = value;
        }
    }

    public decimal Precio
    {
        get => _precio;
        set
        {
            if (value < 0)
                throw new ArgumentException("El precio no puede ser negativo.");
            _precio = Math.Round(value, 2);
        }
    }

    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (value < 0)
                throw new ArgumentException("La cantidad en inventario no puede ser negativa.");
            _cantidad = value;
        }
    }

    /// <summary>Valor total en inventario de este título (precio por cantidad).</summary>
    public decimal ValorTotal => Precio * Cantidad;

    /// <summary>Constructor para un videojuego nuevo (aún sin Id).</summary>
    public Videojuego(string titulo, string plataforma, string genero, int anio, decimal precio, int cantidad)
        : this(0, titulo, plataforma, genero, anio, precio, cantidad)
    {
    }

    /// <summary>Constructor para un videojuego que ya existe en la base de datos.</summary>
    public Videojuego(int id, string titulo, string plataforma, string genero, int anio, decimal precio, int cantidad)
    {
        Id = id;
        Titulo = titulo;
        Plataforma = plataforma;
        Genero = genero;
        Anio = anio;
        Precio = precio;
        Cantidad = cantidad;
    }

    /// <summary>Asigna el Id generado por la base de datos tras insertar.</summary>
    public void AsignarId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El Id debe ser mayor que cero.");
        Id = id;
    }

    public override string ToString() => $"{Titulo} ({Plataforma}, {Anio})";
}
