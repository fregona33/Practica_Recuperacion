using System.Diagnostics;
using System.Text;
using System.Globalization;

namespace EjRecuperacionBUENO;

public class Program
{
    private static readonly CultureInfo ESPANOL = new("es-ES");

    private const int PRECIO_CAFE = 90;
    private const int PRECIO_CHOCOLATE = 110;
    private const int PRECIO_CRUASAN = 160;
    private const int CAPACIDAD = 8;
    
    private const int MONEDA_10 = 10;
    private const int MONEDA_20 = 20;
    private const int MONEDA_50 = 50;
    private const int MONEDA_100 = 100;
    private const int MONEDA_200 = 200;

    // TODO: Guardar el stock y las ventas en un archivo para conservarlos al iniciar.
    
    private static int stockCafe = CAPACIDAD;
    private static int stockChocolate = CAPACIDAD;
    private static int stockCruasan = CAPACIDAD;
    
    private static int ventasChocolate = 0;
    private static int ventasCafe = 0;
    private static int ventasCruasan = 0;

    private static int recaudacion = 0;
    
    
    static void MostrarMenu()
    {
        Console.WriteLine("============ MÁQUINA EXPENDEDORA ============");
        Console.WriteLine("1. Comprar");
        Console.WriteLine("2. Reponer");
        Console.WriteLine("3. Informe");
        Console.WriteLine("0. Apagar");
    }
    
    
    /// <summary>Muestra una pregunta y lee la respuesta del usuario.</summary>
    /// <param name="mensaje">Pregunta que se muestra en la consola.</param>
    /// <returns>La respuesta del usuario o null si se acaba la entrada.</returns>
    static string? LeerEntrada(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine();
    }
    /// <summary>
    /// Obtiene el precio en centimos de un producto
    /// </summary>
    /// <param name="producto"></param>
    /// <returns>Precio del producto en centimos</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int Precio(Producto producto)
    {
        return producto switch
        {
            Producto.Cafe => PRECIO_CAFE,
            Producto.Cruasan => PRECIO_CRUASAN,
            Producto.Chocolate => PRECIO_CHOCOLATE,
            _ => throw new ArgumentOutOfRangeException(
                nameof(producto), producto, "Producto no válido")
        };
    }

    /// <summary>
    /// Comprueba si la máquina acepta la moneda
    /// </summary>
    /// <param name="centimos"></param>
    /// <returns></returns>
    public static bool EsMonedaValida(int centimos)
    {
        return centimos == MONEDA_10 ||
               centimos == MONEDA_20 ||
               centimos == MONEDA_50 ||
               centimos == MONEDA_100 ||
               centimos == MONEDA_200;
    }

    /// <summary>
    /// Pide monedas para cubrir el precio o cancelar.
    /// </summary>
    /// <param name="precio"></param>
    /// <param name="pagado"></param>
    /// <returns>False si se acaba la entrada, true en otro caso</returns>
    static bool PedirMonedas(int precio, out int pagado)
    {
        pagado = 0;

        while (pagado < precio)
        {
            Console.WriteLine($"Faltan {Formatear(precio - pagado)}");

            string? entrada = LeerEntrada(
                "Moneda en céntimos (0 cancela): ");

            if (entrada == null)
            {
                Console.WriteLine($"No hay más entrada. Se devuelven {Formatear(pagado)}");
                Console.WriteLine("Se apaga la máquina. ");
                return false;
            }

            if (!int.TryParse(entrada, out int moneda))
            {
                Console.WriteLine("Debes escribir un número. ");
                continue;
            }

            if (moneda == 0)
            {
                Console.WriteLine($"Compra cancelada. Se devuelven {Formatear(pagado)}");
                pagado = 0;
                return true;
            }

            if (!EsMonedaValida(moneda))
            {
                Console.WriteLine("Moneda no admitida. Usa 10, 20, 50, 100 o 200");
                continue;
            }

            pagado += moneda;
        }
        return true;
    }

    /// <summary>
    /// Convierte céntimos en un texto con formato de la moneda española.
    ///ç</summary>
    /// <param name="centimos"></param>
    /// <returns>Importe en euros.</returns>
    public static string Formatear(int centimos)
    {
        return (centimos / 100m).ToString("C", ESPANOL);
    }
    /// <summary>
    /// Muestra los productos y permite seleccionar uno.
    /// </summary>
    /// <returns>False si se acaba la entrada de teclado, true en todos los demas casos.</returns>
    static bool Comprar()
    {
        Console.WriteLine("========= PRODUCTOS =========");
        Console.WriteLine($"1. Café     {Formatear(Precio(Producto.Cafe))}");
        Console.WriteLine($"2. Chocolate    {Formatear(Precio(Producto.Chocolate))}");
        Console.WriteLine($"3. Cruasán    {Formatear(Precio(Producto.Cruasan))}");
        Console.WriteLine("0. Volver");

        string? entrada = LeerEntrada("Producto: ");

        if (entrada == null)
        {
            Console.WriteLine("No hay ninguna entrada. Se apaga la máquina. ");
            return false;
        }

        if (!int.TryParse(entrada, out int opcion))
        {
            Console.WriteLine("Debes escribir un número. ");
            return true;
        }

        if (opcion == 0)
            return true;

        if (opcion < (int)Producto.Cafe || opcion > (int)Producto.Cruasan)
        {
            Console.WriteLine("Producto no válido. ");
            return true;
        }

        Producto producto = (Producto)opcion;
        int precio = Precio(producto);

        Console.WriteLine($"Precio del producto elegido: {Formatear(precio)}");

        if (!PedirMonedas(precio, out int pagado))
            return false;
        
        if (pagado == 0)
            return true;

        try
        {
            int cambio = Cambio(pagado, precio);
            string desglose = Desglose(cambio);

            RegistrarVenta(producto);

            Console.WriteLine($"Retira tu producto: {producto}.");
            if (cambio > 0)
            {
                Console.WriteLine(desglose);
            }

            Console.WriteLine($"Unidades restantes: {ObtenerStock(producto)}");
            Console.WriteLine("Gracias por su compra. ");
        }

        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"No puede pagarse menos del precio del producto. {ex.Message}");
            Console.WriteLine($"Se devuelven {Formatear(pagado)}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"Se devuelven {Formatear(pagado)}. ");
        }
        return true;
    }
    /// <summary>
    /// Calcula el cambio que debe devolverse.
    /// </summary>
    /// <param name="pagado"></param>
    /// <param name="precio"></param>
    /// <returns>Diferencia entre el importe pagado y el precio</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// El importe pagado es inferior al precio
    /// </exception>
    public static int Cambio(int pagado, int precio)
    {
        if (pagado < precio)
        {
            throw new ArgumentOutOfRangeException(nameof(pagado),
                "El importe pagado no puede ser inferior al precio. ");
        }

        return pagado - precio;
    }

    /// <summary>
    /// Desglosa una parte del importe usando el menor número de monedas.
    /// </summary>
    /// <param name="centimos"></param>
    /// <returns>Desglose de monedas o "Sin cambio" si el importe es cero</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// El importe es negativo o no es múltiplo de diez.
    /// </exception>
    public static string Desglose(int centimos)
    {
        if (centimos < 0 || centimos % MONEDA_10 != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(centimos),
                "El importe debe ser positivo o cero o múltiplo de diez.");
        }

        if (centimos == 0)
        {
            return "Sin cambio";
        }

        int[] monedas =
        {
            MONEDA_200,
            MONEDA_100,
            MONEDA_50,
            MONEDA_20,
            MONEDA_10
        };

        string resultado = "";

        for (int i = 0; i < monedas.Length; i++)
        {
            int cantidad = centimos / monedas[i];
            centimos %= monedas[i];

            if (cantidad > 0)
            {
                if (resultado != "")
                {
                    resultado += " · ";
                }

                resultado += $"{cantidad} x {Formatear(monedas[i])} ";
            }
        }

        return resultado;
    }

    /// <summary>
    /// Consulta las unidades disponibles del producto.
    /// </summary>
    /// <param name="producto"></param>
    /// <returns>Número de unidades disponibles.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// El producto indicado no existe
    /// </exception>
    static int ObtenerStock(Producto producto)
    {
        return producto switch
        {
            Producto.Cafe => stockCafe,
            Producto.Cruasan => stockCruasan,
            Producto.Chocolate => stockChocolate,
            _ => throw new ArgumentOutOfRangeException(nameof(producto))
        };
    }

    static void RegistrarVenta(Producto producto)
    {
        if (ObtenerStock(producto) <= 0)
        {
            throw new InvalidOperationException("El producto esta agotado. ");
        }

        switch (producto)
        {
            case Producto.Cafe:
                stockCafe--;
                ventasCafe++;
                break;
            
            case Producto.Chocolate:
                stockChocolate--;
                ventasChocolate++;
                break;
            
            case Producto.Cruasan:
                stockCruasan--;
                ventasCruasan++;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(producto));
        }

        recaudacion += Precio(producto);
    }

    /// <summary>
    /// Calcula el stock resultante de una reposición
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="cantidad"></param>
    /// <param name="maximo"></param>
    /// <returns>Stock después de la reposición</returns>
    /// <exception cref="ArgumentOutOfRangeException">La cantidad es negativa o el stock resultante supera la capacidad</exception>
    public static int NuevoStock(int actual, int cantidad, int maximo)
    {
        if (cantidad < 0 || (long)actual + cantidad > maximo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cantidad),
                "La cantidad no puede ser negativa ni superar la capacidad.");
        }

        return actual + cantidad;
    }

    /// <summary>
    /// Permite reponer unidades de un producto.
    /// </summary>
    /// <returns>True si se completa de forma correcta, false si se acaba la entrada.</returns>
    static bool Reponer()
    {
        Console.WriteLine("============ REPONER ============");
        Console.WriteLine($"1. Café     Stock: {stockCafe}");
        Console.WriteLine($"2. Chocolate     Stock: {stockChocolate}");
        Console.WriteLine($"3. Cruasán     Stock: {stockCruasan}");

        string? entrada = LeerEntrada("Producto: ");

        if (entrada == null)
        {
            Console.WriteLine("No hay más entrada. La máquina se apaga");
            return false;
        }

        if (!int.TryParse(entrada, out int opcion))
        {
            Console.WriteLine("Debes escribir un número. ");
            return true;
        }

        if (opcion < (int)Producto.Cafe || opcion > (int)Producto.Cruasan)
        {
            Console.WriteLine("Producto no válido. ");
            return true;
        }

        if (opcion == 0)
            return true;

        Producto producto = (Producto)opcion;
        int capacidad = ObtenerStock(producto);
        
        Console.WriteLine($"Al producto le quedan {capacidad} unidades. ");
        string? repuesto = LeerEntrada("Unidades a añadir: ");

        if (repuesto == null)
        {
            Console.WriteLine("No hay entrada. Se apaga la máquina. ");
            return false;
        }

        if (!int.TryParse(repuesto, out int cantidad))
        {
            Console.WriteLine("Se debe poner un número. ");
            return true;
        }

        try
        {
            int stockNuevo = NuevoStock(
                capacidad, cantidad, CAPACIDAD);

            switch (producto)
            {
                case Producto.Cafe:
                    stockCafe = stockNuevo;
                    break;

                case Producto.Chocolate:
                    stockChocolate = stockNuevo;
                    break;

                case Producto.Cruasan:
                    stockCruasan = stockNuevo;
                    break;
            }
            Console.WriteLine($"Operacion realizada. Stock de {producto}: {stockNuevo}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"La cantidad debe ser cero o positiva y el stock no debe superar {CAPACIDAD}. ");
        }

        return true;
    }

    /// <summary>
    /// Muestra el inventario, la recaudación y las estadísticas de las ventas.
    /// </summary>
    static void MostrarInforme()
    {
        Console.WriteLine("============ INFORME ============");
        Console.WriteLine($"{"Producto",-14} {"Precio",10} {"Ventas",8} {"Stock",8}");
        Console.WriteLine($"{"Café",-14} {Formatear(Precio(Producto.Cafe)),10}" +
                          $"{ventasCafe,8} {stockCafe,8}");
        
        Console.WriteLine($"{"Chocolate",-14} {Formatear(Precio(Producto.Chocolate)),10}" +
                          $"{ventasChocolate,8} {stockChocolate,8}");

        Console.WriteLine($"{"Cruasán",-14} {Formatear(Precio(Producto.Cruasan)),10}" +
                          $"{ventasCruasan,8} {stockCruasan,8}");
        
        Console.WriteLine();
        Console.WriteLine($"Recaudación: {Formatear(recaudacion)}");

        string masVendido = "Café";
        int mayorVentas = ventasCafe;

        if (ventasChocolate > mayorVentas)
        {
            masVendido = "Chocolate";
            mayorVentas = ventasChocolate;
        }

        if (ventasCruasan > mayorVentas)
        {
            masVendido = "Cruasán";
        }
        
        Console.WriteLine($"Producto más vendido: {masVendido}");

        int totalVentas = ventasChocolate + ventasCafe + ventasCruasan;
        decimal mediaEuros = 0;

        if (totalVentas > 0)
        {
            mediaEuros = recaudacion / (100m * totalVentas);
        }
        Console.WriteLine($"Importe medio por venta: {mediaEuros.ToString("C", ESPANOL)}");

    }

    
    static void Main()
    {
        CultureInfo.CurrentCulture = ESPANOL;
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
        bool apagada = false;

        do
        {
            MostrarMenu();
            string? entrada = LeerEntrada("Opción: ");

            if (entrada is null)
            {
                Console.WriteLine("No hay mas entrada. Se apaga la máquina.");
                return;
            }

            if (!int.TryParse(entrada, out int opcion))
            {
                Console.WriteLine("Escribe un número del menú: ");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    Comprar();
                    break;

                case 2:
                    Reponer();
                    break;

                case 3:
                    MostrarInforme();
                    break;

                case 0:
                    string? confirmacion = LeerEntrada("¿Quieres apagar la máquina? (S / N)");

                    if (confirmacion is null)
                    {
                        Console.WriteLine("No hay más entrada, se apaga la máquina. ");
                    }

                    apagada = confirmacion == "S" || confirmacion == "s";
                    break;

                default:
                    Console.WriteLine("Opción desconocida: ");
                    break;
            }
        } while (!apagada);
        {
            Console.WriteLine("Máquina apagada. ");
        }
    }
}

///<summary> Productos disponibles de la máquina </summary>
public enum Producto
{
    Cafe = 1,
    Chocolate = 2,
    Cruasan = 3
}