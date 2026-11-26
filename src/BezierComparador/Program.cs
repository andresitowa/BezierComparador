using System;
using System.Windows.Forms;

namespace BezierComparador
{
    static class Program
    {
        // Punto de entrada principal de la aplicacion
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new frmPrincipal());
        }
    }
}
