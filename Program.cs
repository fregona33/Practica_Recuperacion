using System.Diagnostics;

namespace EjRecuperacionBUENO;

class Program
{
    static void MostrarMenu()
    {
        Console.WriteLine("============ MENÚ ============");
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
    
    static void Main()
    {
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
                    Console.WriteLine("Has elegido Comprar. ");
                    break;

                case 2:
                    Console.WriteLine("Has elegido Reponer. ");
                    break;

                case 3:
                    Console.WriteLine("Has elegido Informe. ");
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