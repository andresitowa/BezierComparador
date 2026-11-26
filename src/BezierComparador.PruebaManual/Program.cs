using System;
using System.IO;

namespace BezierComparador
{
    static class Program
    {
        // Ejecuta la validacion manual, la muestra por consola y la guarda en un txt
        static void Main()
        {
            clsPruebaManual objPrueba = new clsPruebaManual();
            string cadReporte = objPrueba.GenerarReporte();

            Console.WriteLine(cadReporte);

            string cadRuta = Path.Combine(Environment.CurrentDirectory, "validacion_manual.txt");
            File.WriteAllText(cadRuta, cadReporte);
            Console.WriteLine("Reporte guardado en: " + cadRuta);
        }
    }
}
