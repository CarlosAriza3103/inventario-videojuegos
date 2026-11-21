using System.Collections.Generic;
using InventarioVideojuegos.Models;

namespace InventarioVideojuegos.Data;

/// <summary>
/// Contrato de acceso a datos para videojuegos. El servicio depende de esta
/// interfaz y no de SQLite, de modo que la lógica queda independiente del almacenamiento.
/// </summary>
public interface IVideojuegoDAO
{
    void Inicializar();
    int Insertar(Videojuego juego);
    List<Videojuego> ObtenerTodos();
    List<Videojuego> Buscar(string texto);
    bool Actualizar(Videojuego juego);
    bool Eliminar(int id);
    bool ExisteDuplicado(string titulo, string plataforma, int idExcluido);
}
