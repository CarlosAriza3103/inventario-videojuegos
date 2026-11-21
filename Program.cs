using Avalonia;
using System;

namespace InventarioVideojuegos;

/// <summary>
/// Punto de entrada de la aplicación. Configura Avalonia y arranca el ciclo de vida de escritorio.
/// </summary>
internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
