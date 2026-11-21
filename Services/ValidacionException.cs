using System;

namespace InventarioVideojuegos.Services;

/// <summary>
/// Excepción propia para errores de validación de datos ingresados por el usuario.
/// Permite a la interfaz distinguir "dato incorrecto" (se explica al usuario)
/// de un fallo técnico inesperado.
/// </summary>
public class ValidacionException : Exception
{
    public ValidacionException(string mensaje) : base(mensaje)
    {
    }
}
