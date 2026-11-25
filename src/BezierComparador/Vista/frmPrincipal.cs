using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierComparador
{
    // Ventana principal interactiva para dibujar y comparar curvas de Bezier
    public class frmPrincipal : Form
    {
        private clsControladorCurva objControlador;
        private Panel pnlContenedorLienzo = null!;
        private ctlLienzo picLienzo = null!;
        private Panel pnlLateral = null!;
        private GroupBox grpPuntosControl = null!;
        private ListBox lstListaPuntos = null!;
        private Label lblGradoActual = null!;
        private Button btnCurvaEjemplo = null!;
        private Button btnLimpiar = null!;
        private GroupBox grpConfiguracion = null!;
        private Label lblMuestras = null!;
        private NumericUpDown numMuestras = null!;
        private CheckBox chkBernstein = null!;
        private CheckBox chkCasteljau = null!;
        private CheckBox chkPoligono = null!;
        private CheckBox chkInterpolaciones = null!;
        private Label lblParametroT = null!;
        private TrackBar trkParametroT = null!;
        private GroupBox grpResultados = null!;
        private Label lblTiempoBernstein = null!;
        private Label lblTiempoCasteljau = null!;
        private Label lblErrorMaximo = null!;
        private Label lblErrorPromedio = null!;
        private Label lblErrorEcm = null!;
        private Button btnMedir = null!;
        private Button btnAbrirLaboratorio = null!;
        private Label lblMensajeEstado = null!;
        private ToolTip tipAyuda = null!;
        private int entIndiceArrastrando;

        public frmPrincipal()
        {
            objControlador = new clsControladorCurva();
            entIndiceArrastrando = -1;
            InicializarComponentes();
            CargarCurvaCubicaInicial();
        }

        // Construye la interfaz siguiendo el diseno UI/UX del proyecto
        private void InicializarComponentes()
        {
            this.Text = "Comparador de Curvas de Bezier: Polinomios de Bernstein vs De Casteljau";
            this.Size = new Size(1240, 760);
            this.MinimumSize = new Size(980, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            clsTemaUI.AplicarEstiloFormulario(this);
            this.DoubleBuffered = true;

            tipAyuda = new ToolTip();
            clsTemaUI.ConfigurarToolTip(tipAyuda);

            // Panel lateral derecho blanco con los 3 GroupBox obligatorios
            pnlLateral = new Panel();
            pnlLateral.Dock = DockStyle.Right;
            pnlLateral.Width = 360;
            pnlLateral.BackColor = clsTemaUI.ColSuperficieTarjeta;
            pnlLateral.Padding = new Padding(clsTemaUI.EntEspacioMd);

            // 1. GroupBox: Puntos de control
            grpPuntosControl = new GroupBox();
            grpPuntosControl.Text = "Puntos de control (Rojo)";
            grpPuntosControl.Location = new Point(12, 10);
            grpPuntosControl.Size = new Size(336, 195);

            lblGradoActual = new Label();
            lblGradoActual.Text = "Grado actual: n = 0 (0 puntos)";
            lblGradoActual.Location = new Point(12, 24);
            lblGradoActual.AutoSize = true;
            lblGradoActual.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            lstListaPuntos = new ListBox();
            lstListaPuntos.Location = new Point(12, 46);
            lstListaPuntos.Size = new Size(312, 100);

            btnCurvaEjemplo = new Button();
            btnCurvaEjemplo.Text = "Cargar curva grado 3";
            btnCurvaEjemplo.Location = new Point(12, 154);
            btnCurvaEjemplo.Size = new Size(150, 30);
            clsTemaUI.AplicarEstiloBotonSecundario(btnCurvaEjemplo);
            btnCurvaEjemplo.Click += BtnCurvaEjemplo_Click;
            tipAyuda.SetToolTip(btnCurvaEjemplo, "Carga 4 puntos de control de ejemplo para una curva cubica");

            btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar lienzo";
            btnLimpiar.Location = new Point(174, 154);
            btnLimpiar.Size = new Size(150, 30);
            clsTemaUI.AplicarEstiloBotonPeligroSuave(btnLimpiar);
            btnLimpiar.Click += BtnLimpiar_Click;
            tipAyuda.SetToolTip(btnLimpiar, "Elimina todos los puntos de control del lienzo");

            grpPuntosControl.Controls.Add(lblGradoActual);
            grpPuntosControl.Controls.Add(lstListaPuntos);
            grpPuntosControl.Controls.Add(btnCurvaEjemplo);
            grpPuntosControl.Controls.Add(btnLimpiar);

            // 2. GroupBox: Configuracion y leyenda de colores
            grpConfiguracion = new GroupBox();
            grpConfiguracion.Text = "Configuracion y Leyenda";
            grpConfiguracion.Location = new Point(12, 212);
            grpConfiguracion.Size = new Size(336, 225);

            lblMuestras = new Label();
            lblMuestras.Text = "Cantidad de muestras (N):";
            lblMuestras.Location = new Point(12, 27);
            lblMuestras.AutoSize = true;

            numMuestras = new NumericUpDown();
            numMuestras.Minimum = 10;
            numMuestras.Maximum = 100000;
            numMuestras.Increment = 100;
            numMuestras.Value = 200;
            numMuestras.Location = new Point(190, 24);
            numMuestras.Width = 130;
            numMuestras.ValueChanged += Configuracion_Changed;
            tipAyuda.SetToolTip(numMuestras, "Numero de puntos evaluados a lo largo de la curva");

            chkBernstein = new CheckBox();
            chkBernstein.Text = "[Azul] Curva con Polinomios de Bernstein";
            chkBernstein.ForeColor = Color.FromArgb(0, 102, 204);
            chkBernstein.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkBernstein.Checked = true;
            chkBernstein.Location = new Point(14, 58);
            chkBernstein.AutoSize = true;
            chkBernstein.CheckedChanged += Configuracion_Changed;

            chkCasteljau = new CheckBox();
            chkCasteljau.Text = "[Naranja] Curva con Algoritmo De Casteljau";
            chkCasteljau.ForeColor = Color.FromArgb(230, 115, 0);
            chkCasteljau.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkCasteljau.Checked = true;
            chkCasteljau.Location = new Point(14, 84);
            chkCasteljau.AutoSize = true;
            chkCasteljau.CheckedChanged += Configuracion_Changed;

            chkInterpolaciones = new CheckBox();
            chkInterpolaciones.Text = "[Verde] Interpolaciones de De Casteljau";
            chkInterpolaciones.ForeColor = Color.FromArgb(25, 145, 65);
            chkInterpolaciones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkInterpolaciones.Checked = true;
            chkInterpolaciones.Location = new Point(14, 110);
            chkInterpolaciones.AutoSize = true;
            chkInterpolaciones.CheckedChanged += Configuracion_Changed;

            chkPoligono = new CheckBox();
            chkPoligono.Text = "[Rojo/Gris] Puntos y poligono de control";
            chkPoligono.ForeColor = Color.FromArgb(180, 35, 35);
            chkPoligono.Checked = true;
            chkPoligono.Location = new Point(14, 136);
            chkPoligono.AutoSize = true;
            chkPoligono.CheckedChanged += Configuracion_Changed;

            lblParametroT = new Label();
            lblParametroT.Text = "Parametro de interpolacion t = 0.50:";
            lblParametroT.Location = new Point(12, 165);
            lblParametroT.AutoSize = true;

            trkParametroT = new TrackBar();
            trkParametroT.Minimum = 0;
            trkParametroT.Maximum = 100;
            trkParametroT.Value = 50;
            trkParametroT.TickFrequency = 10;
            trkParametroT.Location = new Point(12, 184);
            trkParametroT.Size = new Size(312, 35);
            trkParametroT.Scroll += TrkParametroT_Scroll;
            tipAyuda.SetToolTip(trkParametroT, "Deslice para ver las subdivisiones de De Casteljau en t");

            grpConfiguracion.Controls.Add(lblMuestras);
            grpConfiguracion.Controls.Add(numMuestras);
            grpConfiguracion.Controls.Add(chkBernstein);
            grpConfiguracion.Controls.Add(chkCasteljau);
            grpConfiguracion.Controls.Add(chkInterpolaciones);
            grpConfiguracion.Controls.Add(chkPoligono);
            grpConfiguracion.Controls.Add(lblParametroT);
            grpConfiguracion.Controls.Add(trkParametroT);

            // 3. GroupBox: Resultados y acceso al Laboratorio de Pruebas
            grpResultados = new GroupBox();
            grpResultados.Text = "Resultados de la curva actual";
            grpResultados.Location = new Point(12, 444);
            grpResultados.Size = new Size(336, 255);

            lblTiempoBernstein = new Label();
            lblTiempoBernstein.Text = "Tiempo Bernstein (10 rep): 0.0000 ms";
            lblTiempoBernstein.Location = new Point(14, 26);
            lblTiempoBernstein.AutoSize = true;
            lblTiempoBernstein.ForeColor = Color.FromArgb(0, 102, 204);

            lblTiempoCasteljau = new Label();
            lblTiempoCasteljau.Text = "Tiempo De Casteljau (10 rep): 0.0000 ms";
            lblTiempoCasteljau.Location = new Point(14, 48);
            lblTiempoCasteljau.AutoSize = true;
            lblTiempoCasteljau.ForeColor = Color.FromArgb(210, 100, 0);

            lblErrorMaximo = new Label();
            lblErrorMaximo.Text = "Diferencia maxima: 0.000E+000";
            lblErrorMaximo.Location = new Point(14, 74);
            lblErrorMaximo.AutoSize = true;

            lblErrorPromedio = new Label();
            lblErrorPromedio.Text = "Diferencia promedio: 0.000E+000";
            lblErrorPromedio.Location = new Point(14, 96);
            lblErrorPromedio.AutoSize = true;

            lblErrorEcm = new Label();
            lblErrorEcm.Text = "Error cuadratico medio (ECM): 0.000E+000";
            lblErrorEcm.Location = new Point(14, 118);
            lblErrorEcm.AutoSize = true;

            btnMedir = new Button();
            btnMedir.Text = "Medir curva actual (Stopwatch 10 rep)";
            btnMedir.Location = new Point(12, 146);
            btnMedir.Size = new Size(312, 32);
            clsTemaUI.AplicarEstiloBotonPrimario(btnMedir);
            btnMedir.Click += BtnMedir_Click;
            tipAyuda.SetToolTip(btnMedir, "Mide 10 repeticiones con calentamiento de la curva dibujada");

            btnAbrirLaboratorio = new Button();
            btnAbrirLaboratorio.Text = "Abrir Laboratorio y Graficos Comparativos";
            btnAbrirLaboratorio.Location = new Point(12, 186);
            btnAbrirLaboratorio.Size = new Size(312, 36);
            clsTemaUI.AplicarEstiloBotonExito(btnAbrirLaboratorio);
            btnAbrirLaboratorio.Click += BtnAbrirLaboratorio_Click;
            tipAyuda.SetToolTip(btnAbrirLaboratorio, "Abre la ventana frmPruebas con la tabla de experimentos y los graficos Tiempo vs Muestras/Grado");

            lblMensajeEstado = new Label();
            lblMensajeEstado.Text = "Clic izq: agrega | Arrastrar: mueve | Clic der: elimina";
            lblMensajeEstado.Location = new Point(12, 228);
            lblMensajeEstado.AutoSize = true;
            lblMensajeEstado.ForeColor = Color.FromArgb(80, 80, 80);

            grpResultados.Controls.Add(lblTiempoBernstein);
            grpResultados.Controls.Add(lblTiempoCasteljau);
            grpResultados.Controls.Add(lblErrorMaximo);
            grpResultados.Controls.Add(lblErrorPromedio);
            grpResultados.Controls.Add(lblErrorEcm);
            grpResultados.Controls.Add(btnMedir);
            grpResultados.Controls.Add(btnAbrirLaboratorio);
            grpResultados.Controls.Add(lblMensajeEstado);

            pnlLateral.Controls.Add(grpPuntosControl);
            pnlLateral.Controls.Add(grpConfiguracion);
            pnlLateral.Controls.Add(grpResultados);

            // Lienzo grande a la izquierda
            pnlContenedorLienzo = new Panel();
            pnlContenedorLienzo.Dock = DockStyle.Fill;
            pnlContenedorLienzo.Padding = new Padding(12);

            picLienzo = new ctlLienzo();
            picLienzo.Dock = DockStyle.Fill;
            picLienzo.MouseDown += PicLienzo_MouseDown;
            picLienzo.MouseMove += PicLienzo_MouseMove;
            picLienzo.MouseUp += PicLienzo_MouseUp;

            pnlContenedorLienzo.Controls.Add(picLienzo);

            this.Controls.Add(pnlContenedorLienzo);
            this.Controls.Add(pnlLateral);
        }

        // Carga 4 puntos de control iniciales para que el usuario vea una curva de grado 3 al abrir
        private void CargarCurvaCubicaInicial()
        {
            objControlador.Limpiar();
            objControlador.AgregarPunto(110, 500);
            objControlador.AgregarPunto(240, 120);
            objControlador.AgregarPunto(560, 120);
            objControlador.AgregarPunto(700, 500);
            RecalcularYRedibujar(true);
        }

        // Clic izquierdo agrega o selecciona para arrastrar; clic derecho elimina un punto
        private void PicLienzo_MouseDown(object? objRemitente, MouseEventArgs objArgs)
        {
            int entIndice = objControlador.BuscarIndicePuntoCercano(objArgs.X, objArgs.Y, 12.0);

            if (objArgs.Button == MouseButtons.Left)
            {
                if (entIndice >= 0)
                {
                    entIndiceArrastrando = entIndice;
                    picLienzo.EntIndiceSeleccionado = entIndice;
                }
                else
                {
                    objControlador.AgregarPunto(objArgs.X, objArgs.Y);
                    entIndiceArrastrando = objControlador.ObjCurva.LstPuntosControl.Count - 1;
                    picLienzo.EntIndiceSeleccionado = entIndiceArrastrando;
                    RecalcularYRedibujar(false);
                }
            }
            else if (objArgs.Button == MouseButtons.Right)
            {
                if (entIndice >= 0)
                {
                    objControlador.EliminarPunto(entIndice);
                    picLienzo.EntIndiceSeleccionado = -1;
                    RecalcularYRedibujar(false);
                }
            }
        }

        // Arrastra el punto de control seleccionado y actualiza la curva en tiempo real
        private void PicLienzo_MouseMove(object? objRemitente, MouseEventArgs objArgs)
        {
            if (entIndiceArrastrando >= 0 && objArgs.Button == MouseButtons.Left)
            {
                double dblX = Math.Max(10, Math.Min(picLienzo.Width - 10, objArgs.X));
                double dblY = Math.Max(10, Math.Min(picLienzo.Height - 10, objArgs.Y));
                objControlador.ModificarPunto(entIndiceArrastrando, dblX, dblY);
                RecalcularYRedibujar(false);
            }
        }

        // Suelta el punto arrastrado y actualiza las metricas
        private void PicLienzo_MouseUp(object? objRemitente, MouseEventArgs objArgs)
        {
            if (entIndiceArrastrando >= 0)
            {
                entIndiceArrastrando = -1;
                picLienzo.EntIndiceSeleccionado = -1;
                RecalcularYRedibujar(true);
            }
        }

        // Actualiza la vista cuando cambian las casillas o el numero de muestras
        private void Configuracion_Changed(object? objRemitente, EventArgs objArgs)
        {
            picLienzo.BolMostrarBernstein = chkBernstein.Checked;
            picLienzo.BolMostrarCasteljau = chkCasteljau.Checked;
            picLienzo.BolMostrarInterpolaciones = chkInterpolaciones.Checked;
            picLienzo.BolMostrarPoligonoControl = chkPoligono.Checked;
            RecalcularYRedibujar(false);
        }

        // Actualiza los poligonos de interpolacion de De Casteljau al mover la barra de t
        private void TrkParametroT_Scroll(object? objRemitente, EventArgs objArgs)
        {
            double dblT = trkParametroT.Value / 100.0;
            lblParametroT.Text = "Parametro de interpolacion t = " + dblT.ToString("F2") + ":";
            RecalcularYRedibujar(false);
        }

        // Restaura la curva cubica de ejemplo
        private void BtnCurvaEjemplo_Click(object? objRemitente, EventArgs objArgs)
        {
            CargarCurvaCubicaInicial();
        }

        // Limpia todos los puntos del lienzo
        private void BtnLimpiar_Click(object? objRemitente, EventArgs objArgs)
        {
            objControlador.Limpiar();
            RecalcularYRedibujar(false);
        }

        // Ejecuta la medicion con Stopwatch (10 repeticiones + calentamiento) sobre la curva actual
        private void BtnMedir_Click(object? objRemitente, EventArgs objArgs)
        {
            RecalcularYRedibujar(true);
        }

        // Abre la ventana frmPruebas con las tablas y los graficos comparativos de rendimiento
        private void BtnAbrirLaboratorio_Click(object? objRemitente, EventArgs objArgs)
        {
            frmPruebas objVentanaPruebas = new frmPruebas();
            objVentanaPruebas.Show(this);
        }

        // Evalua ambos metodos y envia los puntos calculados al lienzo para dibujarlos con DrawLine
        private void RecalcularYRedibujar(bool bolMedirConStopwatch)
        {
            List<clsPuntoControl> lstPuntos = objControlador.ObjCurva.LstPuntosControl;
            int entCantidad = lstPuntos.Count;

            lstListaPuntos.Items.Clear();
            for (int entI = 0; entI < entCantidad; entI++)
            {
                lstListaPuntos.Items.Add(
                    "P" + entI + " = (" + lstPuntos[entI].DblX.ToString("F1") + ", " + lstPuntos[entI].DblY.ToString("F1") + ")");
            }

            if (entCantidad < 2)
            {
                lblGradoActual.Text = "Grado actual: sin curva (" + entCantidad + " puntos)";
                lblMensajeEstado.Text = "Agregue al menos 2 puntos en el lienzo.";
                picLienzo.ActualizarGeometria(lstPuntos, new clsPuntoControl[0], new clsPuntoControl[0], new List<List<clsPuntoControl>>());
                return;
            }

            int entGrado = objControlador.ObjCurva.ObtenerGrado();
            int entMuestras = (int)numMuestras.Value;
            double dblT = trkParametroT.Value / 100.0;

            lblGradoActual.Text = "Grado actual: n = " + entGrado + " (" + entCantidad + " puntos)";
            lblMensajeEstado.Text = "Curva evaluada con " + entMuestras + " muestras.";

            List<List<clsPuntoControl>> lstNiveles = objControlador.ObtenerInterpolaciones(dblT);

            if (bolMedirConStopwatch)
            {
                clsResultadoEvaluacion objResB;
                clsResultadoEvaluacion objResC;
                clsResultadoComparacion objComp;
                objControlador.MedirYComparar(entMuestras, out objResB, out objResC, out objComp);

                lblTiempoBernstein.Text = "Tiempo Bernstein (10 rep): " + objResB.DblTiempoPromedioMs.ToString("F4") + " ms";
                lblTiempoCasteljau.Text = "Tiempo De Casteljau (10 rep): " + objResC.DblTiempoPromedioMs.ToString("F4") + " ms";
                lblErrorMaximo.Text = "Diferencia maxima: " + objComp.DblDiferenciaMaxima.ToString("E3");
                lblErrorPromedio.Text = "Diferencia promedio: " + objComp.DblDiferenciaPromedio.ToString("E3");
                lblErrorEcm.Text = "Error cuadratico medio (ECM): " + objComp.DblErrorCuadraticoMedio.ToString("E3");

                picLienzo.ActualizarGeometria(lstPuntos, objResB.ArrPuntosCurva, objResC.ArrPuntosCurva, lstNiveles);
            }
            else
            {
                clsResultadoEvaluacion objResB = objControlador.EvaluarBernsteinRapido(entMuestras);
                clsResultadoEvaluacion objResC = objControlador.EvaluarDeCasteljauRapido(entMuestras);
                clsComparador objComp = new clsComparador();
                clsResultadoComparacion objDif = objComp.Comparar(objResB.ArrPuntosCurva, objResC.ArrPuntosCurva);

                lblErrorMaximo.Text = "Diferencia maxima: " + objDif.DblDiferenciaMaxima.ToString("E3");
                lblErrorPromedio.Text = "Diferencia promedio: " + objDif.DblDiferenciaPromedio.ToString("E3");
                lblErrorEcm.Text = "Error cuadratico medio (ECM): " + objDif.DblErrorCuadraticoMedio.ToString("E3");

                picLienzo.ActualizarGeometria(lstPuntos, objResB.ArrPuntosCurva, objResC.ArrPuntosCurva, lstNiveles);
            }
        }
    }
}
