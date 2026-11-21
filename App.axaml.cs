using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InventarioVideojuegos.Data;
using InventarioVideojuegos.Services;
using InventarioVideojuegos.Views;

namespace InventarioVideojuegos;

/// <summary>
/// Clase de aplicación. Aquí se "ensamblan" las capas del sistema (composición):
/// Conexión -> DAO -> Servicio -> Ventana.
/// </summary>
public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var conexion = new ConexionDb();
            IVideojuegoDAO dao = new VideojuegoDAO(conexion);
            var servicio = new VideojuegoServicio(dao);

            desktop.MainWindow = new MainWindow(servicio);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
