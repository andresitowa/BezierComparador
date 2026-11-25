using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierComparador
{
    // Panel con doble buffer activado para evitar parpadeo al dibujar graficos
    public class pnlGraficoDobleBuffer : Panel
    {
        public pnlGraficoDobleBuffer()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }
    }

    // Ventana de experimentacion, tabla comparativa y graficos de rendimiento
    public class frmPruebas : Form
    {
        private clsControladorPruebas objControladorPruebas;
        private Panel pnlSuperior = null!;
        private GroupBox grpConfiguracion = null!;
        private Label lblRepeticiones = null!;
        private NumericUpDown numRepeticiones = null!;
        private Button btnEjecutar = null!;
        private ProgressBar prgProgreso = null!;
        private Label lblEstado = null!;
        private SplitContainer splContenedor = null!;
        private GroupBox grpResultados = null!;
        private DataGridView dgvResultados = null!;
        private GroupBox grpGraficos = null!;
        private Panel pnlFiltroGraficos = null!;
        private Label lblGradoFijo = null!;
        private ComboBox cmbGradoFijo = null!;
        private Label lblMuestrasFijas = null!;
        private ComboBox cmbMuestrasFijas = null!;
        private Label lblLeyendaAzul = null!;
        private Label lblLeyendaNaranja = null!;
        private TableLayoutPanel tblGraficos = null!;
        private pnlGraficoDobleBuffer pnlGraficoMuestras = null!;
        private pnlGraficoDobleBuffer pnlGraficoGrados = null!;
        private ToolTip tipAyuda = null!;
        private List<clsFilaPrueba> lstDatosActuales;
        private bool bolEjecutando;

        public frmPruebas()
        {
            objControladorPruebas = new clsControladorPruebas();
            lstDatosActuales = new List<clsFilaPrueba>();
            bolEjecutando = false;
            InicializarComponentes();
        }

        // Configura los controles visuales, tabla y paneles de graficos
        private void InicializarComponentes()
        {
            this.Text = "Laboratorio de Pruebas: Bernstein vs De Casteljau";
            this.Size = new Size(1180, 780);
            this.MinimumSize = new Size(960, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.DoubleBuffered = true;

            tipAyuda = new ToolTip();

            pnlSuperior = new Panel();
            pnlSuperior.Dock = DockStyle.Top;
            pnlSuperior.Height = 85;
            pnlSuperior.Padding = new Padding(12, 8, 12, 6);
            pnlSuperior.BackColor = Color.White;

            grpConfiguracion = new GroupBox();
            grpConfiguracion.Text = "Configuracion de experimentos (Grados: 2, 3, 5, 10, 20, 50 | Muestras: 100 a 100000)";
            grpConfiguracion.Dock = DockStyle.Fill;

            lblRepeticiones = new Label();
            lblRepeticiones.Text = "Repeticiones (min 10):";
            lblRepeticiones.Location = new Point(16, 30);
            lblRepeticiones.AutoSize = true;

            numRepeticiones = new NumericUpDown();
            numRepeticiones.Minimum = 10;
            numRepeticiones.Maximum = 100;
            numRepeticiones.Value = 10;
            numRepeticiones.Location = new Point(155, 27);
            numRepeticiones.Width = 65;
            tipAyuda.SetToolTip(numRepeticiones, "Cantidad de repeticiones por prueba (con calentamiento previo)");

            btnEjecutar = new Button();
            btnEjecutar.Text = "Ejecutar pruebas";
            btnEjecutar.Location = new Point(235, 24);
            btnEjecutar.Size = new Size(145, 32);
            btnEjecutar.BackColor = Color.FromArgb(0, 102, 204);
            btnEjecutar.ForeColor = Color.White;
            btnEjecutar.FlatStyle = FlatStyle.Flat;
            btnEjecutar.Cursor = Cursors.Hand;
            btnEjecutar.Click += BtnEjecutar_Click;
            tipAyuda.SetToolTip(btnEjecutar, "Ejecuta los experimentos y actualiza la tabla y los graficos");

            prgProgreso = new ProgressBar();
            prgProgreso.Location = new Point(395, 28);
            prgProgreso.Size = new Size(210, 24);
            prgProgreso.Minimum = 0;
            prgProgreso.Maximum = 100;
            prgProgreso.Value = 0;

            lblEstado = new Label();
            lblEstado.Text = "Estado: Listo. Presione Ejecutar pruebas para iniciar.";
            lblEstado.Location = new Point(620, 31);
            lblEstado.AutoSize = true;
            lblEstado.ForeColor = Color.FromArgb(50, 50, 50);

            grpConfiguracion.Controls.Add(lblRepeticiones);
            grpConfiguracion.Controls.Add(numRepeticiones);
            grpConfiguracion.Controls.Add(btnEjecutar);
            grpConfiguracion.Controls.Add(prgProgreso);
            grpConfiguracion.Controls.Add(lblEstado);
            pnlSuperior.Controls.Add(grpConfiguracion);

            splContenedor = new SplitContainer();
            splContenedor.Dock = DockStyle.Fill;
            splContenedor.Orientation = Orientation.Horizontal;
            splContenedor.SplitterDistance = 310;
            splContenedor.Padding = new Padding(12, 6, 12, 12);

            grpResultados = new GroupBox();
            grpResultados.Text = "Tabla de resultados (en verde el metodo mas rapido por fila)";
            grpResultados.Dock = DockStyle.Fill;
            grpResultados.Padding = new Padding(8);

            dgvResultados = new DataGridView();
            dgvResultados.Dock = DockStyle.Fill;
            dgvResultados.BackgroundColor = Color.White;
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.ReadOnly = true;
            dgvResultados.RowHeadersVisible = false;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConfigurarColumnasTabla();

            grpResultados.Controls.Add(dgvResultados);
            splContenedor.Panel1.Controls.Add(grpResultados);

            grpGraficos = new GroupBox();
            grpGraficos.Text = "Graficos comparativos propios (dibujados con DrawLine)";
            grpGraficos.Dock = DockStyle.Fill;
            grpGraficos.Padding = new Padding(8);

            pnlFiltroGraficos = new Panel();
            pnlFiltroGraficos.Dock = DockStyle.Top;
            pnlFiltroGraficos.Height = 36;
            pnlFiltroGraficos.BackColor = Color.White;

            lblGradoFijo = new Label();
            lblGradoFijo.Text = "Grado para Tiempo vs Muestras:";
            lblGradoFijo.Location = new Point(10, 9);
            lblGradoFijo.AutoSize = true;

            cmbGradoFijo = new ComboBox();
            cmbGradoFijo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGradoFijo.Location = new Point(205, 6);
            cmbGradoFijo.Width = 75;
            int[] arrGrados = objControladorPruebas.ArrGradosPrueba;
            for (int entI = 0; entI < arrGrados.Length; entI++)
            {
                cmbGradoFijo.Items.Add(arrGrados[entI]);
            }
            cmbGradoFijo.SelectedIndex = arrGrados.Length - 1;
            cmbGradoFijo.SelectedIndexChanged += FiltroGraficos_Changed;

            lblMuestrasFijas = new Label();
            lblMuestrasFijas.Text = "Muestras para Tiempo vs Grado:";
            lblMuestrasFijas.Location = new Point(305, 9);
            lblMuestrasFijas.AutoSize = true;

            cmbMuestrasFijas = new ComboBox();
            cmbMuestrasFijas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMuestrasFijas.Location = new Point(500, 6);
            cmbMuestrasFijas.Width = 95;
            int[] arrMuestras = objControladorPruebas.ArrMuestrasPrueba;
            for (int entI = 0; entI < arrMuestras.Length; entI++)
            {
                cmbMuestrasFijas.Items.Add(arrMuestras[entI]);
            }
            cmbMuestrasFijas.SelectedIndex = arrMuestras.Length - 1;
            cmbMuestrasFijas.SelectedIndexChanged += FiltroGraficos_Changed;

            lblLeyendaAzul = new Label();
            lblLeyendaAzul.Text = "[Azul] Polinomios de Bernstein";
            lblLeyendaAzul.ForeColor = Color.FromArgb(0, 102, 204);
            lblLeyendaAzul.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLeyendaAzul.Location = new Point(625, 9);
            lblLeyendaAzul.AutoSize = true;

            lblLeyendaNaranja = new Label();
            lblLeyendaNaranja.Text = "[Naranja] Algoritmo de De Casteljau";
            lblLeyendaNaranja.ForeColor = Color.FromArgb(230, 115, 0);
            lblLeyendaNaranja.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLeyendaNaranja.Location = new Point(830, 9);
            lblLeyendaNaranja.AutoSize = true;

            pnlFiltroGraficos.Controls.Add(lblGradoFijo);
            pnlFiltroGraficos.Controls.Add(cmbGradoFijo);
            pnlFiltroGraficos.Controls.Add(lblMuestrasFijas);
            pnlFiltroGraficos.Controls.Add(cmbMuestrasFijas);
            pnlFiltroGraficos.Controls.Add(lblLeyendaAzul);
            pnlFiltroGraficos.Controls.Add(lblLeyendaNaranja);

            tblGraficos = new TableLayoutPanel();
            tblGraficos.Dock = DockStyle.Fill;
            tblGraficos.ColumnCount = 2;
            tblGraficos.RowCount = 1;
            tblGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblGraficos.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            pnlGraficoMuestras = new pnlGraficoDobleBuffer();
            pnlGraficoMuestras.Dock = DockStyle.Fill;
            pnlGraficoMuestras.BackColor = Color.White;
            pnlGraficoMuestras.Margin = new Padding(4);
            pnlGraficoMuestras.BorderStyle = BorderStyle.FixedSingle;
            pnlGraficoMuestras.Paint += PnlGraficoMuestras_Paint;

            pnlGraficoGrados = new pnlGraficoDobleBuffer();
            pnlGraficoGrados.Dock = DockStyle.Fill;
            pnlGraficoGrados.BackColor = Color.White;
            pnlGraficoGrados.Margin = new Padding(4);
            pnlGraficoGrados.BorderStyle = BorderStyle.FixedSingle;
            pnlGraficoGrados.Paint += PnlGraficoGrados_Paint;

            tblGraficos.Controls.Add(pnlGraficoMuestras, 0, 0);
            tblGraficos.Controls.Add(pnlGraficoGrados, 1, 0);

            grpGraficos.Controls.Add(tblGraficos);
            grpGraficos.Controls.Add(pnlFiltroGraficos);
            splContenedor.Panel2.Controls.Add(grpGraficos);

            this.Controls.Add(splContenedor);
            this.Controls.Add(pnlSuperior);
        }

        // Define las columnas de la tabla de resultados
        private void ConfigurarColumnasTabla()
        {
            dgvResultados.Columns.Clear();
            dgvResultados.Columns.Add("colGrado", "Grado (n)");
            dgvResultados.Columns.Add("colMuestras", "Muestras");
            dgvResultados.Columns.Add("colBernstein", "Bernstein Prom (ms)");
            dgvResultados.Columns.Add("colDesvBernstein", "Bernstein Desv (ms)");
            dgvResultados.Columns.Add("colCasteljau", "De Casteljau Prom (ms)");
            dgvResultados.Columns.Add("colDesvCasteljau", "De Casteljau Desv (ms)");
            dgvResultados.Columns.Add("colErrorMax", "Error Maximo");
            dgvResultados.Columns.Add("colErrorProm", "Error Promedio");
            dgvResultados.Columns.Add("colMasRapido", "Mas Rapido");
        }

        // Redibuja ambos paneles cuando cambia el filtro de grado o muestras
        private void FiltroGraficos_Changed(object? objRemitente, EventArgs objArgumentos)
        {
            pnlGraficoMuestras.Invalidate();
            pnlGraficoGrados.Invalidate();
        }

        // Ejecuta los experimentos de forma asincrona y actualiza tabla y graficos
        private async void BtnEjecutar_Click(object? objRemitente, EventArgs objArgumentos)
        {
            if (bolEjecutando)
            {
                return;
            }

            bolEjecutando = true;
            btnEjecutar.Enabled = false;
            numRepeticiones.Enabled = false;
            dgvResultados.Rows.Clear();
            prgProgreso.Value = 0;
            lblEstado.Text = "Estado: Iniciando experimentos...";

            int entRepeticiones = (int)numRepeticiones.Value;

            Action<int, string> objReporteProgreso = new Action<int, string>((entPorcentaje, cadTexto) =>
            {
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (entPorcentaje >= 0 && entPorcentaje <= 100)
                        {
                            prgProgreso.Value = entPorcentaje;
                        }
                        lblEstado.Text = "Estado: " + cadTexto;
                    }));
                }
            });

            List<clsFilaPrueba> lstFilas = await Task.Run(() =>
                objControladorPruebas.EjecutarTodasLasPruebas(entRepeticiones, objReporteProgreso));

            lstDatosActuales = new List<clsFilaPrueba>(lstFilas);
            PoblarTablaResultados(lstDatosActuales);
            pnlGraficoMuestras.Invalidate();
            pnlGraficoGrados.Invalidate();

            prgProgreso.Value = 100;
            lblEstado.Text = "Estado: Pruebas completadas (" + lstDatosActuales.Count + " combinaciones evaluadas).";
            btnEjecutar.Enabled = true;
            numRepeticiones.Enabled = true;
            bolEjecutando = false;
        }

        // Llena el DataGridView y resalta en verde el metodo mas rapido de cada fila
        private void PoblarTablaResultados(List<clsFilaPrueba> lstFilas)
        {
            dgvResultados.Rows.Clear();
            Color colVerdeGanador = Color.FromArgb(200, 242, 205);

            for (int entI = 0; entI < lstFilas.Count; entI++)
            {
                clsFilaPrueba objFila = lstFilas[entI];
                int entIndiceFila = dgvResultados.Rows.Add(
                    objFila.entGrado,
                    objFila.entMuestras,
                    objFila.dblTiempoBernsteinMs.ToString("F4"),
                    objFila.dblDesviacionBernsteinMs.ToString("F4"),
                    objFila.dblTiempoDeCasteljauMs.ToString("F4"),
                    objFila.dblDesviacionDeCasteljauMs.ToString("F4"),
                    objFila.dblErrorMaximo.ToString("E3"),
                    objFila.dblErrorPromedio.ToString("E3"),
                    objFila.cadMetodoMasRapido
                );

                DataGridViewRow objFilaVisual = dgvResultados.Rows[entIndiceFila];
                objFilaVisual.Cells[8].Style.BackColor = colVerdeGanador;

                if (objFila.dblTiempoBernsteinMs <= objFila.dblTiempoDeCasteljauMs)
                {
                    objFilaVisual.Cells[2].Style.BackColor = colVerdeGanador;
                }
                else
                {
                    objFilaVisual.Cells[4].Style.BackColor = colVerdeGanador;
                }
            }
        }

        // Evento Paint para el grafico de Tiempo vs Muestras
        private void PnlGraficoMuestras_Paint(object? objRemitente, PaintEventArgs objArgumentos)
        {
            DibujarGraficoMuestras(objArgumentos.Graphics, pnlGraficoMuestras.ClientRectangle);
        }

        // Evento Paint para el grafico de Tiempo vs Grado
        private void PnlGraficoGrados_Paint(object? objRemitente, PaintEventArgs objArgumentos)
        {
            DibujarGraficoGrados(objArgumentos.Graphics, pnlGraficoGrados.ClientRectangle);
        }

        // Filtra los datos por grado fijo y llama al dibujador de lineas
        protected void DibujarGraficoMuestras(Graphics objLienzo, Rectangle objArea)
        {
            int entGradoSeleccionado = 50;
            if (cmbGradoFijo.SelectedItem != null)
            {
                entGradoSeleccionado = (int)cmbGradoFijo.SelectedItem;
            }

            List<string> lstEtiquetasX = new List<string>();
            List<double> lstTiemposBernstein = new List<double>();
            List<double> lstTiemposCasteljau = new List<double>();

            for (int entI = 0; entI < lstDatosActuales.Count; entI++)
            {
                clsFilaPrueba objFila = lstDatosActuales[entI];
                if (objFila.entGrado == entGradoSeleccionado)
                {
                    lstEtiquetasX.Add(objFila.entMuestras.ToString());
                    lstTiemposBernstein.Add(objFila.dblTiempoBernsteinMs);
                    lstTiemposCasteljau.Add(objFila.dblTiempoDeCasteljauMs);
                }
            }

            string cadTitulo = "Tiempo (ms) vs Muestras (Grado n = " + entGradoSeleccionado + ")";
            DibujarGraficoComparativo(
                objLienzo,
                objArea,
                cadTitulo,
                "Cantidad de muestras",
                lstEtiquetasX,
                lstTiemposBernstein,
                lstTiemposCasteljau);
        }

        // Filtra los datos por cantidad de muestras fija y llama al dibujador de lineas
        protected void DibujarGraficoGrados(Graphics objLienzo, Rectangle objArea)
        {
            int entMuestrasSeleccionadas = 100000;
            if (cmbMuestrasFijas.SelectedItem != null)
            {
                entMuestrasSeleccionadas = (int)cmbMuestrasFijas.SelectedItem;
            }

            List<string> lstEtiquetasX = new List<string>();
            List<double> lstTiemposBernstein = new List<double>();
            List<double> lstTiemposCasteljau = new List<double>();

            for (int entI = 0; entI < lstDatosActuales.Count; entI++)
            {
                clsFilaPrueba objFila = lstDatosActuales[entI];
                if (objFila.entMuestras == entMuestrasSeleccionadas)
                {
                    lstEtiquetasX.Add("n=" + objFila.entGrado);
                    lstTiemposBernstein.Add(objFila.dblTiempoBernsteinMs);
                    lstTiemposCasteljau.Add(objFila.dblTiempoDeCasteljauMs);
                }
            }

            string cadTitulo = "Tiempo (ms) vs Grado (Muestras = " + entMuestrasSeleccionadas + ")";
            DibujarGraficoComparativo(
                objLienzo,
                objArea,
                cadTitulo,
                "Grado de la curva (n)",
                lstEtiquetasX,
                lstTiemposBernstein,
                lstTiemposCasteljau);
        }

        // Dibuja ejes, leyenda y series usando unicamente DrawLine, FillEllipse y DrawString
        private void DibujarGraficoComparativo(
            Graphics objLienzo,
            Rectangle objArea,
            string cadTitulo,
            string cadEjeX,
            List<string> lstEtiquetasX,
            List<double> lstSerieBernstein,
            List<double> lstSerieCasteljau)
        {
            objLienzo.Clear(Color.White);

            using (Font fntTitulo = new Font("Segoe UI", 9.5F, FontStyle.Bold))
            using (Font fntEjes = new Font("Segoe UI", 8F, FontStyle.Regular))
            using (Brush brsTexto = new SolidBrush(Color.FromArgb(40, 40, 40)))
            using (Pen penEje = new Pen(Color.FromArgb(100, 100, 100), 1.5F))
            using (Pen penGuia = new Pen(Color.FromArgb(228, 228, 228), 1F))
            using (Pen penBernstein = new Pen(Color.FromArgb(0, 102, 204), 2.2F))
            using (Pen penCasteljau = new Pen(Color.FromArgb(230, 115, 0), 2.2F))
            using (Brush brsBernstein = new SolidBrush(Color.FromArgb(0, 102, 204)))
            using (Brush brsCasteljau = new SolidBrush(Color.FromArgb(230, 115, 0)))
            {
                objLienzo.DrawString(cadTitulo, fntTitulo, brsTexto, 14, 8);

                if (lstEtiquetasX.Count == 0)
                {
                    objLienzo.DrawString(
                        "Presione 'Ejecutar pruebas' para generar el grafico.",
                        fntEjes,
                        brsTexto,
                        20,
                        objArea.Height / 2 - 10);
                    return;
                }

                int entMargenIzq = 64;
                int entMargenDer = 24;
                int entMargenSup = 36;
                int entMargenInf = 48;

                int entAnchoGrafico = objArea.Width - entMargenIzq - entMargenDer;
                int entAltoGrafico = objArea.Height - entMargenSup - entMargenInf;

                if (entAnchoGrafico <= 40 || entAltoGrafico <= 40)
                {
                    return;
                }

                int entOrigenX = entMargenIzq;
                int entOrigenY = entMargenSup + entAltoGrafico;

                // Determina el valor maximo del eje Y para escalar los puntos
                double dblMaximoY = 0.0001;
                for (int entI = 0; entI < lstSerieBernstein.Count; entI++)
                {
                    if (lstSerieBernstein[entI] > dblMaximoY)
                    {
                        dblMaximoY = lstSerieBernstein[entI];
                    }
                    if (lstSerieCasteljau[entI] > dblMaximoY)
                    {
                        dblMaximoY = lstSerieCasteljau[entI];
                    }
                }
                dblMaximoY = dblMaximoY * 1.12;

                // Dibuja lineas guia horizontales y marcas del eje Y (Tiempo en ms)
                int entDivisionesY = 4;
                for (int entPaso = 0; entPaso <= entDivisionesY; entPaso++)
                {
                    double dblFraccion = (double)entPaso / (double)entDivisionesY;
                    int entY = entOrigenY - (int)(dblFraccion * entAltoGrafico);
                    double dblValorY = dblFraccion * dblMaximoY;

                    objLienzo.DrawLine(penGuia, entOrigenX, entY, entOrigenX + entAnchoGrafico, entY);
                    string cadMarcaY = dblValorY.ToString("F2");
                    objLienzo.DrawString(cadMarcaY, fntEjes, brsTexto, 6, entY - 7);
                }

                // Dibuja los ejes principales X y Y con DrawLine
                objLienzo.DrawLine(penEje, entOrigenX, entMargenSup, entOrigenX, entOrigenY);
                objLienzo.DrawLine(penEje, entOrigenX, entOrigenY, entOrigenX + entAnchoGrafico, entOrigenY);

                // Calcula las coordenadas en pantalla de cada punto y los une con DrawLine
                int entCantidadPuntos = lstEtiquetasX.Count;
                Point[] arrPuntosBernstein = new Point[entCantidadPuntos];
                Point[] arrPuntosCasteljau = new Point[entCantidadPuntos];

                for (int entI = 0; entI < entCantidadPuntos; entI++)
                {
                    double dblProporcionX = 0.0;
                    if (entCantidadPuntos > 1)
                    {
                        dblProporcionX = (double)entI / (double)(entCantidadPuntos - 1);
                    }

                    int entX = entOrigenX + (int)(dblProporcionX * entAnchoGrafico);
                    int entYBernstein = entOrigenY - (int)((lstSerieBernstein[entI] / dblMaximoY) * entAltoGrafico);
                    int entYCasteljau = entOrigenY - (int)((lstSerieCasteljau[entI] / dblMaximoY) * entAltoGrafico);

                    arrPuntosBernstein[entI] = new Point(entX, entYBernstein);
                    arrPuntosCasteljau[entI] = new Point(entX, entYCasteljau);

                    objLienzo.DrawLine(penEje, entX, entOrigenY, entX, entOrigenY + 4);
                    objLienzo.DrawString(lstEtiquetasX[entI], fntEjes, brsTexto, entX - 18, entOrigenY + 6);
                }

                // Une los puntos consecutivos con segmentos rectos (DrawLine)
                for (int entI = 0; entI < entCantidadPuntos - 1; entI++)
                {
                    objLienzo.DrawLine(penBernstein, arrPuntosBernstein[entI], arrPuntosBernstein[entI + 1]);
                    objLienzo.DrawLine(penCasteljau, arrPuntosCasteljau[entI], arrPuntosCasteljau[entI + 1]);
                }

                // Dibuja los marcadores circulares sobre cada muestra medida
                for (int entI = 0; entI < entCantidadPuntos; entI++)
                {
                    objLienzo.FillEllipse(brsBernstein, arrPuntosBernstein[entI].X - 4, arrPuntosBernstein[entI].Y - 4, 8, 8);
                    objLienzo.FillEllipse(brsCasteljau, arrPuntosCasteljau[entI].X - 4, arrPuntosCasteljau[entI].Y - 4, 8, 8);
                }

                // Etiqueta del eje X y leyenda interna
                objLienzo.DrawString(cadEjeX, fntEjes, brsTexto, entOrigenX + entAnchoGrafico / 2 - 45, objArea.Height - 20);

                int entLeyendaX = entOrigenX + 12;
                int entLeyendaY = entMargenSup + 6;
                objLienzo.DrawLine(penBernstein, entLeyendaX, entLeyendaY + 6, entLeyendaX + 18, entLeyendaY + 6);
                objLienzo.DrawString("Bernstein", fntEjes, brsBernstein, entLeyendaX + 22, entLeyendaY - 1);

                objLienzo.DrawLine(penCasteljau, entLeyendaX + 95, entLeyendaY + 6, entLeyendaX + 113, entLeyendaY + 6);
                objLienzo.DrawString("De Casteljau", fntEjes, brsCasteljau, entLeyendaX + 117, entLeyendaY - 1);
            }
        }
    }
}
