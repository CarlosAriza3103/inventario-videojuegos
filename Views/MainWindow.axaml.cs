using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using InventarioVideojuegos.Models;
using InventarioVideojuegos.Services;

namespace InventarioVideojuegos.Views;

/// <summary>
/// Ventana principal. Solo se ocupa de la interfaz: lee los controles, llama al servicio
/// y muestra el resultado. No contiene SQL ni reglas de negocio.
/// </summary>
public partial class MainWindow : Window
{
    private readonly VideojuegoServicio _servicio = null!;
    private Videojuego? _seleccionado;
    private bool _cargandoSeleccion;

    private static readonly IBrush ColorExito = new SolidColorBrush(Color.Parse("#2E7D32"));
    private static readonly IBrush ColorError = new SolidColorBrush(Color.Parse("#C62828"));
    private static readonly IBrush ColorNormal = new SolidColorBrush(Color.Parse("#374151"));

    /// <summary>Constructor sin parámetros, requerido por el diseñador de Avalonia.</summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(VideojuegoServicio servicio) : this()
    {
        _servicio = servicio;

        CmbPlataforma.ItemsSource = VideojuegoServicio.Plataformas;
        CmbGenero.ItemsSource = VideojuegoServicio.Generos;

        Opened += MainWindow_Opened;
    }

    // ------------------------------------------------------------ Arranque
    private async void MainWindow_Opened(object? sender, EventArgs e)
    {
        try
        {
            _servicio.Inicializar();   // crea la base de datos y la tabla si no existen
            CargarJuegos();
            MostrarEstado("Base de datos lista.", ColorNormal);
        }
        catch (Exception ex)
        {
            MostrarEstado("No se pudo abrir la base de datos.", ColorError);
            await Dialogo.MostrarAsync(this, "Error de base de datos",
                $"No fue posible abrir o crear la base de datos.\n\nDetalle: {ex.Message}");
        }
    }

    // -------------------------------------------------------------- CREATE
    private async void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Videojuego nuevo = _servicio.Registrar(
                TxtTitulo.Text, CmbPlataforma.SelectedItem as string, CmbGenero.SelectedItem as string,
                TxtAnio.Text, TxtPrecio.Text, TxtCantidad.Text);

            CargarJuegos();
            LimpiarFormulario();
            MostrarEstado($"Se registró \"{nuevo.Titulo}\" correctamente.", ColorExito);
        }
        catch (ValidacionException ex)
        {
            await ReportarValidacion(ex);
        }
        catch (Exception ex)
        {
            await ReportarErrorInesperado(ex);
        }
    }

    // -------------------------------------------------------------- UPDATE
    private async void BtnActualizar_Click(object? sender, RoutedEventArgs e)
    {
        if (_seleccionado is null)
        {
            MostrarEstado("Selecciona un videojuego de la tabla para modificarlo.", ColorError);
            return;
        }

        try
        {
            Videojuego editado = _servicio.Actualizar(
                _seleccionado.Id,
                TxtTitulo.Text, CmbPlataforma.SelectedItem as string, CmbGenero.SelectedItem as string,
                TxtAnio.Text, TxtPrecio.Text, TxtCantidad.Text);

            CargarJuegos();
            LimpiarFormulario();
            MostrarEstado($"Se actualizó \"{editado.Titulo}\" correctamente.", ColorExito);
        }
        catch (ValidacionException ex)
        {
            await ReportarValidacion(ex);
        }
        catch (Exception ex)
        {
            await ReportarErrorInesperado(ex);
        }
    }

    // -------------------------------------------------------------- DELETE
    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_seleccionado is null)
        {
            MostrarEstado("Selecciona un videojuego de la tabla para eliminarlo.", ColorError);
            return;
        }

        bool confirmado = await Dialogo.ConfirmarAsync(this, "Confirmar eliminación",
            $"¿Seguro que deseas eliminar \"{_seleccionado.Titulo}\"?\nEsta acción no se puede deshacer.");

        if (!confirmado)
        {
            MostrarEstado("Eliminación cancelada.", ColorNormal);
            return;
        }

        try
        {
            string titulo = _seleccionado.Titulo;
            _servicio.Eliminar(_seleccionado.Id);

            CargarJuegos();
            LimpiarFormulario();
            MostrarEstado($"Se eliminó \"{titulo}\".", ColorExito);
        }
        catch (ValidacionException ex)
        {
            await ReportarValidacion(ex);
        }
        catch (Exception ex)
        {
            await ReportarErrorInesperado(ex);
        }
    }

    // ---------------------------------------------------------------- READ
    private void BtnBuscar_Click(object? sender, RoutedEventArgs e) => EjecutarBusqueda();

    private void BtnMostrarTodo_Click(object? sender, RoutedEventArgs e)
    {
        TxtBuscar.Text = string.Empty;
        EjecutarBusqueda();
    }

    private void TxtBuscar_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            EjecutarBusqueda();
    }

    private async void EjecutarBusqueda()
    {
        try
        {
            CargarJuegos();
            MostrarEstado(string.IsNullOrWhiteSpace(TxtBuscar.Text)
                ? "Mostrando todos los videojuegos."
                : $"Resultados para \"{TxtBuscar.Text!.Trim()}\".", ColorNormal);
        }
        catch (Exception ex)
        {
            await ReportarErrorInesperado(ex);
        }
    }

    /// <summary>Consulta los registros (aplicando el filtro actual) y refresca la tabla.</summary>
    private void CargarJuegos()
    {
        List<Videojuego> juegos = _servicio.Listar(TxtBuscar.Text);
        GridJuegos.ItemsSource = juegos;

        decimal valor = _servicio.CalcularValorInventario(juegos);
        TxtResumen.Text = $"{juegos.Count} título(s)  |  Valor del inventario: {valor.ToString("N2", CultureInfo.CurrentCulture)}";
    }

    // ----------------------------------------------------------- Selección
    private void GridJuegos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_cargandoSeleccion)
            return;

        _seleccionado = GridJuegos.SelectedItem as Videojuego;

        if (_seleccionado is null)
        {
            BtnGuardar.IsEnabled = true;
            BtnActualizar.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            return;
        }

        TxtTitulo.Text = _seleccionado.Titulo;
        CmbPlataforma.SelectedItem = _seleccionado.Plataforma;
        CmbGenero.SelectedItem = _seleccionado.Genero;
        TxtAnio.Text = _seleccionado.Anio.ToString(CultureInfo.InvariantCulture);
        TxtPrecio.Text = _seleccionado.Precio.ToString("0.00", CultureInfo.InvariantCulture);
        TxtCantidad.Text = _seleccionado.Cantidad.ToString(CultureInfo.InvariantCulture);

        BtnGuardar.IsEnabled = false;
        BtnActualizar.IsEnabled = true;
        BtnEliminar.IsEnabled = true;
        MostrarEstado($"Editando \"{_seleccionado.Titulo}\".", ColorNormal);
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        MostrarEstado("Formulario limpio.", ColorNormal);
    }

    private void LimpiarFormulario()
    {
        _cargandoSeleccion = true;
        try
        {
            GridJuegos.SelectedItem = null;
        }
        finally
        {
            _cargandoSeleccion = false;
        }

        _seleccionado = null;
        TxtTitulo.Text = string.Empty;
        CmbPlataforma.SelectedItem = null;
        CmbGenero.SelectedItem = null;
        TxtAnio.Text = string.Empty;
        TxtPrecio.Text = string.Empty;
        TxtCantidad.Text = string.Empty;

        BtnGuardar.IsEnabled = true;
        BtnActualizar.IsEnabled = false;
        BtnEliminar.IsEnabled = false;
    }

    // ------------------------------------------------------------ Mensajes
    private void MostrarEstado(string mensaje, IBrush color)
    {
        TxtEstado.Text = mensaje;
        TxtEstado.Foreground = color;
    }

    private async System.Threading.Tasks.Task ReportarValidacion(ValidacionException ex)
    {
        MostrarEstado(ex.Message, ColorError);
        await Dialogo.MostrarAsync(this, "Revisa los datos", ex.Message);
    }

    private async System.Threading.Tasks.Task ReportarErrorInesperado(Exception ex)
    {
        MostrarEstado("Ocurrió un error inesperado.", ColorError);
        await Dialogo.MostrarAsync(this, "Error",
            $"Ocurrió un error al procesar la operación.\n\nDetalle: {ex.Message}");
    }
}
