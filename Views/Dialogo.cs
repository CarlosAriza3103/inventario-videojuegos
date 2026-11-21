using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace InventarioVideojuegos.Views;

/// <summary>
/// Ventanas emergentes sencillas (mensaje y confirmación) construidas por código.
/// Se usan para avisar errores de validación y confirmar eliminaciones.
/// </summary>
public static class Dialogo
{
    public static Task MostrarAsync(Window propietario, string titulo, string mensaje) =>
        Mostrar(propietario, titulo, mensaje, pedirConfirmacion: false);

    public static Task<bool> ConfirmarAsync(Window propietario, string titulo, string mensaje) =>
        Mostrar(propietario, titulo, mensaje, pedirConfirmacion: true);

    private static async Task<bool> Mostrar(Window propietario, string titulo, string mensaje, bool pedirConfirmacion)
    {
        bool aceptado = false;

        var ventana = new Window
        {
            Title = titulo,
            Width = 420,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var texto = new TextBlock
        {
            Text = mensaje,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };

        var botonAceptar = new Button
        {
            Content = pedirConfirmacion ? "Sí, continuar" : "Aceptar",
            MinWidth = 100
        };
        botonAceptar.Click += (_, _) =>
        {
            aceptado = true;
            ventana.Close();
        };

        var botones = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8
        };
        botones.Children.Add(botonAceptar);

        if (pedirConfirmacion)
        {
            var botonCancelar = new Button { Content = "Cancelar", MinWidth = 100 };
            botonCancelar.Click += (_, _) => ventana.Close();
            botones.Children.Add(botonCancelar);
        }

        var contenido = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 18
        };
        contenido.Children.Add(texto);
        contenido.Children.Add(botones);

        ventana.Content = contenido;

        await ventana.ShowDialog(propietario);
        return aceptado;
    }
}
