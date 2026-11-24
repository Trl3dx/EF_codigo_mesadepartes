
// =====================================================================
using System;
using System.IO;

namespace MesaDePartesDigital
{
    class GestorExpedientes
    {
        // ---------------- DATOS ----------------
        public const int MAXIMO = 100;                 // capacidad del prototipo
        const string ARCHIVO = "expedientes.txt";

        // Arreglos paralelos: la posición i de cada arreglo
        // pertenece al MISMO expediente.
        string[] codigos = new string[MAXIMO];
        string[] dnis = new string[MAXIMO];
        string[] nombres = new string[MAXIMO];
        string[] tramites = new string[MAXIMO];
        string[] asuntos = new string[MAXIMO];
        string[] fechas = new string[MAXIMO];
        string[] estados = new string[MAXIMO];
        int cantidad = 0;

        // Listas fijas de opciones
        public static string[] TIPOS = {
            "Constancia de estudios",
            "Justificación de inasistencia",
            "Traslado de matrícula",
            "Certificado de notas"
        };
        public static string[] ESTADOS = { "Pendiente", "En revisión", "Atendido" };

        public int Cantidad
        {
            get { return cantidad; }
        }

        public bool EstaLleno()
        {
            return cantidad >= MAXIMO;
        }

        // ---------------- VALIDACIONES ----------------
        public static bool DniValido(string dni)
        {
            if (dni.Length != 8) return false;
            for (int i = 0; i < dni.Length; i++)
            {
                if (!char.IsDigit(dni[i])) return false;
            }
            return true;
        }

        public static bool NombreValido(string nombre)
        {
            if (nombre.Length < 3) return false;
            for (int i = 0; i < nombre.Length; i++)
            {
                if (!char.IsLetter(nombre[i]) && nombre[i] != ' ') return false;
            }
            return true;
        }

        // ';' separa campos en el archivo, y el asunto se limita a 60 caracteres
        public static string LimpiarAsunto(string asunto)
        {
            asunto = asunto.Replace(";", ",");
            if (asunto.Length > 60)
                asunto = asunto.Substring(0, 60);
            return asunto;
        }

        // ---------------- REGISTRAR ----------------
        // Devuelve el código generado (EXP-0001, EXP-0002, ...)
        public string Registrar(string dni, string nombre, string tramite, string asunto)
        {
            codigos[cantidad] = "EXP-" + (cantidad + 1).ToString("0000");
            dnis[cantidad] = dni;
            nombres[cantidad] = nombre;
            tramites[cantidad] = tramite;
            asuntos[cantidad] = asunto;
            fechas[cantidad] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            estados[cantidad] = ESTADOS[0];            // inicia "Pendiente"
            cantidad++;

            GuardarArchivo();
            return codigos[cantidad - 1];
        }

        // ---------------- BUSCAR (secuencial) ----------------
        public int BuscarPosicion(string codigo)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (codigos[i] == codigo) return i;
            }
            return -1;                                  // no encontrado
        }

        public void Mostrar(int i)
        {
            Console.WriteLine("Código : " + codigos[i]);
            Console.WriteLine("DNI    : " + dnis[i]);
            Console.WriteLine("Nombre : " + nombres[i]);
            Console.WriteLine("Trámite: " + tramites[i]);
            Console.WriteLine("Asunto : " + asuntos[i]);
            Console.WriteLine("Fecha  : " + fechas[i]);
            Console.WriteLine("Estado : " + estados[i]);
        }

        public string ObtenerEstado(int i)
        {
            return estados[i];
        }

        // ---------------- CAMBIAR ESTADO ----------------
        public void CambiarEstado(int pos, string nuevoEstado)
        {
            estados[pos] = nuevoEstado;
            GuardarArchivo();
        }

        // ---------------- ORDENAR (burbuja) ----------------
        // criterio 1 = por código, 2 = por apellidos y nombres
        public void Ordenar(int criterio)
        {
            for (int i = 0; i < cantidad - 1; i++)
            {
                for (int j = 0; j < cantidad - 1 - i; j++)
                {
                    string a, b;
                    if (criterio == 1)
                    {
                        a = codigos[j];
                        b = codigos[j + 1];
                    }
                    else
                    {
                        a = nombres[j];
                        b = nombres[j + 1];
                    }

                    if (string.Compare(a, b) > 0)
                        Intercambiar(j, j + 1);
                }
            }
        }

        // Intercambia TODOS los datos de dos posiciones
        void Intercambiar(int x, int y)
        {
            string aux;
            aux = codigos[x]; codigos[x] = codigos[y]; codigos[y] = aux;
            aux = dnis[x]; dnis[x] = dnis[y]; dnis[y] = aux;
            aux = nombres[x]; nombres[x] = nombres[y]; nombres[y] = aux;
            aux = tramites[x]; tramites[x] = tramites[y]; tramites[y] = aux;
            aux = asuntos[x]; asuntos[x] = asuntos[y]; asuntos[y] = aux;
            aux = fechas[x]; fechas[x] = fechas[y]; fechas[y] = aux;
            aux = estados[x]; estados[x] = estados[y]; estados[y] = aux;
        }

        // ---------------- LISTAR ----------------
        public void Listar()
        {
            Console.WriteLine("\nCÓDIGO   | DNI      | APELLIDOS Y NOMBRES | TRÁMITE | ESTADO");
            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine(codigos[i] + " | " + dnis[i] + " | " + nombres[i]
                                  + " | " + tramites[i] + " | " + estados[i]);
            }
            Console.WriteLine("Total: " + cantidad + " expediente(s).");
        }

        // ---------------- RESUMEN (contabilizar) ----------------
        // Cuenta cuántos expedientes hay por tipo de trámite y por estado.
        // Usa arreglos contadores: conteoTipos[k] cuenta los expedientes de TIPOS[k].
        public void MostrarResumen()
        {
            int[] conteoTipos = new int[TIPOS.Length];
            int[] conteoEstados = new int[ESTADOS.Length];

            for (int i = 0; i < cantidad; i++)
            {
                for (int k = 0; k < TIPOS.Length; k++)
                {
                    if (tramites[i] == TIPOS[k]) conteoTipos[k]++;
                }
                for (int k = 0; k < ESTADOS.Length; k++)
                {
                    if (estados[i] == ESTADOS[k]) conteoEstados[k]++;
                }
            }

            Console.WriteLine("\nExpedientes por tipo de trámite:");
            for (int k = 0; k < TIPOS.Length; k++)
            {
                Console.WriteLine("  " + conteoTipos[k] + " en " + TIPOS[k]);
            }

            Console.WriteLine("\nExpedientes por estado:");
            for (int k = 0; k < ESTADOS.Length; k++)
            {
                Console.WriteLine("  " + conteoEstados[k] + " " + ESTADOS[k]);
            }

            Console.WriteLine("\nTotal de expedientes registrados: " + cantidad);
        }

        // ---------------- ARCHIVOS DE TEXTO ----------------
        // Cada línea = un expediente, campos separados por ';'
        public void GuardarArchivo()
        {
            try
            {
                StreamWriter escritor = new StreamWriter(ARCHIVO);   // abrir
                for (int i = 0; i < cantidad; i++)
                {
                    string linea = codigos[i] + ";" + dnis[i] + ";" + nombres[i] + ";"
                                 + tramites[i] + ";" + asuntos[i] + ";" + fechas[i] + ";"
                                 + estados[i];
                    escritor.WriteLine(linea);                       // escribir
                }
                escritor.Close();                                    // cerrar
            }
            catch (IOException)
            {
                Console.WriteLine("Error: no se pudo guardar el archivo " + ARCHIVO);
            }
        }

        public void CargarArchivo()
        {
            if (!File.Exists(ARCHIVO))
            {
                Console.WriteLine("No hay archivo previo. Se inicia con el sistema vacío.");
                return;
            }

            try
            {
                StreamReader lector = new StreamReader(ARCHIVO);     // abrir
                string linea = lector.ReadLine();                    // leer
                while (linea != null && cantidad < MAXIMO)
                {
                    string[] campos = linea.Split(';');
                    if (campos.Length == 7)
                    {
                        codigos[cantidad] = campos[0];
                        dnis[cantidad] = campos[1];
                        nombres[cantidad] = campos[2];
                        tramites[cantidad] = campos[3];
                        asuntos[cantidad] = campos[4];
                        fechas[cantidad] = campos[5];
                        estados[cantidad] = campos[6];
                        cantidad++;
                    }
                    linea = lector.ReadLine();
                }
                lector.Close();                                      // cerrar

                Console.WriteLine(cantidad + " expediente(s) cargado(s) desde el archivo.");
            }
            catch (IOException)
            {
                Console.WriteLine("Error: no se pudo leer el archivo " + ARCHIVO);
            }
        }
    }
}