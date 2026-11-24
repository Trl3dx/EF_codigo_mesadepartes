// =====================================================================
// MESA DE PARTES DIGITAL - COLEGIO
// Curso: Fundamentos de Programación (CIIN1205P)
// CLASE Program: menú e interacción con el usuario (entrada/salida).
// La lógica y los datos están en la clase GestorExpedientes (Class1.cs).
// =====================================================================
using System;
using System.Text;

namespace MesaDePartesDigital
{
    class Program
    {
        static GestorExpedientes gestor = new GestorExpedientes();

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;   // tildes y ñ
            gestor.CargarArchivo();

            int opcion;
            do
            {
                MostrarMenu();
                opcion = LeerEntero("Elija una opción: ");

                switch (opcion)
                {
                    case 1: RegistrarExpediente(); break;
                    case 2: BuscarExpediente(); break;
                    case 3: OrdenarExpedientes(); break;
                    case 4: CambiarEstado(); break;
                    case 5: MostrarResumen(); break;
                    case 6: Console.WriteLine("Cerrando sistema..."); break;
                    default: Console.WriteLine("Opción inválida."); break;
                }
            } while (opcion != 6);
        }

        static void MostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== MESA DE PARTES DIGITAL - COLEGIO =====");
            Console.WriteLine("1. Registrar expediente");
            Console.WriteLine("2. Buscar expediente por código");
            Console.WriteLine("3. Ordenar y listar expedientes");
            Console.WriteLine("4. Cambiar estado de un expediente");
            Console.WriteLine("5. Ver resumen de expedientes");
            Console.WriteLine("6. Salir");
        }

        // ---------------- REGISTRAR ----------------
        static void RegistrarExpediente()
        {
            Console.WriteLine("\n--- REGISTRO DE EXPEDIENTE ---");

            if (gestor.EstaLleno())
            {
                Console.WriteLine("Capacidad llena. No se pueden registrar más expedientes.");
                return;
            }

            string dni = LeerDni();
            string nombre = LeerNombre();
            string tramite = ElegirTramite();
            string asunto = LeerAsunto();

            string codigo = gestor.Registrar(dni, nombre, tramite, asunto);
            Console.WriteLine("Expediente registrado. Su código es: " + codigo);
        }

        static string LeerDni()
        {
            string dni;
            do
            {
                dni = LeerTexto("DNI del solicitante (8 dígitos): ");
                if (!GestorExpedientes.DniValido(dni))
                    Console.WriteLine("DNI inválido. Debe tener 8 dígitos numéricos.");
            } while (!GestorExpedientes.DniValido(dni));
            return dni;
        }

        static string LeerNombre()
        {
            string nombre;
            do
            {
                nombre = LeerTexto("Apellidos y nombres: ").ToUpper();
                if (!GestorExpedientes.NombreValido(nombre))
                    Console.WriteLine("Nombre inválido. Use solo letras y espacios (mínimo 3).");
            } while (!GestorExpedientes.NombreValido(nombre));
            return nombre;
        }

        static string ElegirTramite()
        {
            Console.WriteLine("Tipo de trámite:");
            for (int i = 0; i < GestorExpedientes.TIPOS.Length; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". " + GestorExpedientes.TIPOS[i]);
            }

            int total = GestorExpedientes.TIPOS.Length;
            int opcion;
            do
            {
                opcion = LeerEntero("Elija el tipo (1-" + total + "): ");
                if (opcion < 1 || opcion > total)
                    Console.WriteLine("Opción fuera de rango.");
            } while (opcion < 1 || opcion > total);

            return GestorExpedientes.TIPOS[opcion - 1];
        }

        static string LeerAsunto()
        {
            string asunto;
            do
            {
                asunto = LeerTexto("Asunto (breve): ");
                if (asunto.Length == 0)
                    Console.WriteLine("El asunto no puede estar vacío.");
            } while (asunto.Length == 0);

            return GestorExpedientes.LimpiarAsunto(asunto);
        }

        // ---------------- BUSCAR ----------------
        static void BuscarExpediente()
        {
            Console.WriteLine("\n--- BÚSQUEDA DE EXPEDIENTE ---");
            string codigo = LeerTexto("Ingrese el código (ej. EXP-0001): ").ToUpper();

            int pos = gestor.BuscarPosicion(codigo);
            if (pos == -1)
                Console.WriteLine("No se encontró ningún expediente con el código " + codigo + ".");
            else
                gestor.Mostrar(pos);
        }

        // ---------------- ORDENAR ----------------
        static void OrdenarExpedientes()
        {
            Console.WriteLine("\n--- ORDENAR EXPEDIENTES ---");
            if (gestor.Cantidad == 0)
            {
                Console.WriteLine("No hay expedientes registrados.");
                return;
            }

            Console.WriteLine("1. Por código");
            Console.WriteLine("2. Por apellidos y nombres (A-Z)");
            int criterio = LeerEntero("Elija el criterio: ");
            if (criterio != 1 && criterio != 2)
            {
                Console.WriteLine("Criterio inválido.");
                return;
            }

            gestor.Ordenar(criterio);
            gestor.Listar();
        }

        // ---------------- CAMBIAR ESTADO ----------------
        static void CambiarEstado()
        {
            Console.WriteLine("\n--- CAMBIAR ESTADO ---");
            string codigo = LeerTexto("Código del expediente: ").ToUpper();

            int pos = gestor.BuscarPosicion(codigo);
            if (pos == -1)
            {
                Console.WriteLine("No se encontró ningún expediente con el código " + codigo + ".");
                return;
            }

            Console.WriteLine("Estado actual: " + gestor.ObtenerEstado(pos));
            for (int i = 0; i < GestorExpedientes.ESTADOS.Length; i++)
            {
                Console.WriteLine("  " + (i + 1) + ". " + GestorExpedientes.ESTADOS[i]);
            }

            int opcion = LeerEntero("Nuevo estado: ");
            if (opcion < 1 || opcion > GestorExpedientes.ESTADOS.Length)
            {
                Console.WriteLine("Opción inválida. No se hicieron cambios.");
                return;
            }

            gestor.CambiarEstado(pos, GestorExpedientes.ESTADOS[opcion - 1]);
            Console.WriteLine("Estado actualizado correctamente.");
        }

        // ---------------- RESUMEN ----------------
        static void MostrarResumen()
        {
            Console.WriteLine("\n--- RESUMEN DE EXPEDIENTES ---");
            if (gestor.Cantidad == 0)
            {
                Console.WriteLine("No hay expedientes registrados.");
                return;
            }
            gestor.MostrarResumen();
        }

        // ---------------- ENTRADA ----------------
        static string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine();
            if (texto == null) texto = "";
            return texto.Trim();
        }

        static int LeerEntero(string mensaje)
        {
            int numero;
            Console.Write(mensaje);
            while (!int.TryParse(Console.ReadLine(), out numero))
            {
                Console.Write("Entrada inválida. " + mensaje);
            }
            return numero;
        }
    }
}