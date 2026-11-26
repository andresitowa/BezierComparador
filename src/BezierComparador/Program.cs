using System;
using System.Windows.Forms;

namespace BezierComparador
{
    static class Program
    {
        // Punto de entrada principal de la aplicacion compatible con cualquier version de .NET
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmPrincipal());
        }
    }
}
