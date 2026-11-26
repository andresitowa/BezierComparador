using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
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

    // Ventana de experimentacion con pestanas (TabControl) para tabla y graficos comparativos
    public class frmPruebas : Form
    {
        private clsControladorPruebas objControladorPruebas;
        private Panel pnlCabecera = null!;
        private Panel pnlSuperior = null!;
        private pnlTarjetaUI pnlTarjetaControles = null!;
        private Label lblRepeticiones = null!;
        private NumericUpDown numRepeticiones = null!;
        private Button btnEjecutar = null!;
        private Button btnEstabilidad = null!;
        private Button btnExportarCsv = null!;
        private Button btnGuardarPng = null!;
        private ProgressBar prgProgreso = null!;
        private Label lblEstado = null!;
        private Panel pnlCuerpo = null!;
        private TabControl tabContenedor = null!;
        private TabPage tabTablaResultados = null!;
        private TabPage tabGraficosComparativos = null!;
        private pnlTarjetaUI pnlTarjetaTabla = null!;
        private DataGridView dgvResultados = null!;
        private Panel pnlBannerEstabilidad = null!;
        private Label lblResumenEstabilidad = null!;
        private pnlTarjetaUI pnlTarjetaFiltros = null!;
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

        // Configura los controles visuales usando tarjetas responsivas y TabControl moderno
        private void InicializarComponentes()
        {
            this.Text = "Laboratorio de Pruebas: Bernstein vs De Casteljau";
            this.Size = new Size(1300, 780);
            this.MinimumSize = new Size(1020, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            clsTemaUI.AplicarEstiloFormulario(this);
            this.DoubleBuffered = true;

            tipAyuda = new ToolTip();
            clsTemaUI.ConfigurarToolTip(tipAyuda);

            // 1. Cabecera superior limpia
            pnlCabecera = new Panel();
            pnlCabecera.Dock = DockStyle.Top;
            pnlCabecera.Height = 56;
            pnlCabecera.BackColor = clsTemaUI.ColCabeceraApp;
            pnlCabecera.Padding = new Padding(clsTemaUI.EntEspacioLg, 8, clsTemaUI.EntEspacioLg, 8);

            TableLayoutPanel tblCab = new TableLayoutPanel();
            tblCab.Dock = DockStyle.Fill;
            tblCab.ColumnCount = 2;
            tblCab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            tblCab.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            FlowLayoutPanel flwCabTitulos = new FlowLayoutPanel();
            flwCabTitulos.Dock = DockStyle.Fill;
            flwCabTitulos.FlowDirection = FlowDirection.TopDown;
            flwCabTitulos.WrapContents = false;
            flwCabTitulos.Margin = new Padding(0);

            Label lblTitVentana = new Label();
            lblTitVentana.Text = "Laboratorio de Pruebas y Benchmarking";
            lblTitVentana.Font = clsTemaUI.FntTituloVentana;
            lblTitVentana.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTitVentana.AutoSize = true;
            lblTitVentana.Margin = new Padding(0);

            Label lblSubVentana = new Label();
            lblSubVentana.Text = "Rendimiento temporal (Stopwatch) y estabilidad numerica frente a referencia decimal de 128 bits";
            lblSubVentana.Font = clsTemaUI.FntSubtituloVentana;
            lblSubVentana.ForeColor = clsTemaUI.ColTextoSecundario;
            lblSubVentana.AutoSize = true;
            lblSubVentana.Margin = new Padding(0, 1, 0, 0);

            flwCabTitulos.Controls.Add(lblTitVentana);
            flwCabTitulos.Controls.Add(lblSubVentana);

            FlowLayoutPanel flwCabInsignia = new FlowLayoutPanel();
            flwCabInsignia.Dock = DockStyle.Fill;
            flwCabInsignia.FlowDirection = FlowDirection.RightToLeft;
            flwCabInsignia.WrapContents = false;
            flwCabInsignia.Padding = new Padding(0, 8, 0, 0);

            Label lblInsigniaSuite = clsTemaUI.CrearInsignia("24 combinaciones · Warm-up JIT", clsTemaUI.ColPrimarioSuave, clsTemaUI.ColPrimario);
            lblInsigniaSuite.Font = clsTemaUI.FntMonoespacioPeque;
            flwCabInsignia.Controls.Add(lblInsigniaSuite);

            tblCab.Controls.Add(flwCabTitulos, 0, 0);
            tblCab.Controls.Add(flwCabInsignia, 1, 0);

            Panel pnlBordeCab = new Panel();
            pnlBordeCab.Dock = DockStyle.Bottom;
            pnlBordeCab.Height = 1;
            pnlBordeCab.BackColor = clsTemaUI.ColBordeSutil;

            pnlCabecera.Controls.Add(tblCab);
            pnlCabecera.Controls.Add(pnlBordeCab);

            // 2. Barra de comandos superior compacta dentro de tarjeta responsiva (sin exceso de espacio blanco)
            pnlSuperior = new Panel();
            pnlSuperior.Dock = DockStyle.Top;
            pnlSuperior.Height = 104;
            pnlSuperior.Padding = new Padding(clsTemaUI.EntEspacioMd, 10, clsTemaUI.EntEspacioMd, 4);
            pnlSuperior.BackColor = clsTemaUI.ColFondoApp;

            pnlTarjetaControles = new pnlTarjetaUI();
            pnlTarjetaControles.Dock = DockStyle.Fill;
            pnlTarjetaControles.Padding = new Padding(12, 8, 12, 8);

            TableLayoutPanel tblBarraComandos = new TableLayoutPanel();
            tblBarraComandos.Dock = DockStyle.Fill;
            tblBarraComandos.ColumnCount = 1;
            tblBarraComandos.RowCount = 2;
            tblBarraComandos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblBarraComandos.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tblBarraComandos.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel flwAcciones = new FlowLayoutPanel();
            flwAcciones.Dock = DockStyle.Fill;
            flwAcciones.WrapContents = false;
            flwAcciones.Margin = new Padding(0);

            lblRepeticiones = new Label();
            lblRepeticiones.Text = "Repeticiones (mín. 10):";
            lblRepeticiones.Font = clsTemaUI.FntCuerpoNegrita;
            lblRepeticiones.ForeColor = clsTemaUI.ColTextoSecundario;
            lblRepeticiones.AutoSize = true;
            lblRepeticiones.Margin = new Padding(0, 8, 6, 0);

            numRepeticiones = new NumericUpDown();
            numRepeticiones.Minimum = 10;
            numRepeticiones.Maximum = 100;
            numRepeticiones.Value = 10;
            numRepeticiones.Width = 68;
            numRepeticiones.Font = clsTemaUI.FntMonoespacio;
            numRepeticiones.TextAlign = HorizontalAlignment.Right;
            numRepeticiones.TabIndex = 0;
            numRepeticiones.AccessibleName = "Repeticiones por prueba";
            numRepeticiones.Margin = new Padding(0, 5, 14, 0);
            tipAyuda.SetToolTip(numRepeticiones, "Cantidad de repeticiones por prueba (con calentamiento previo)");

            btnEjecutar = new Button();
            btnEjecutar.Text = "Ejecutar pruebas";
            btnEjecutar.Size = new Size(148, clsTemaUI.EntAlturaBoton);
            btnEjecutar.TabIndex = 1;
            btnEjecutar.AccessibleName = "Ejecutar batería completa de pruebas";
            btnEjecutar.Margin = new Padding(0, 0, 8, 0);
            clsTemaUI.AplicarEstiloBotonPrimario(btnEjecutar);
            btnEjecutar.Click += BtnEjecutar_Click;
            tipAyuda.SetToolTip(btnEjecutar, "Ejecuta los experimentos de rendimiento y actualiza tabla y gráficos");

            btnEstabilidad = new Button();
            btnEstabilidad.Text = "Estabilidad (n=50, 100)";
            btnEstabilidad.Size = new Size(176, clsTemaUI.EntAlturaBoton);
            btnEstabilidad.TabIndex = 2;
            btnEstabilidad.AccessibleName = "Ejecutar prueba de estabilidad numérica en grados 50 y 100";
            btnEstabilidad.Margin = new Padding(0, 0, 14, 0);
            clsTemaUI.AplicarEstiloBotonExito(btnEstabilidad);
            btnEstabilidad.Click += BtnEstabilidad_Click;
            tipAyuda.SetToolTip(btnEstabilidad, "Compara grados 50 y 100 contra referencia decimal de 128 bits");

            btnExportarCsv = new Button();
            btnExportarCsv.Text = "Exportar CSV";
            btnExportarCsv.Size = new Size(124, clsTemaUI.EntAlturaBoton);
            btnExportarCsv.TabIndex = 3;
            btnExportarCsv.AccessibleName = "Exportar resultados a archivo CSV";
            btnExportarCsv.Margin = new Padding(0, 0, 8, 0);
            clsTemaUI.AplicarEstiloBotonSecundario(btnExportarCsv);
            btnExportarCsv.Click += BtnExportarCsv_Click;
            tipAyuda.SetToolTip(btnExportarCsv, "Exporta la tabla de resultados y estabilidad a un archivo CSV");

            btnGuardarPng = new Button();
            btnGuardarPng.Text = "Guardar PNG";
            btnGuardarPng.Size = new Size(124, clsTemaUI.EntAlturaBoton);
            btnGuardarPng.TabIndex = 4;
            btnGuardarPng.AccessibleName = "Guardar gráficos comparativos como imágenes PNG";
            btnGuardarPng.Margin = new Padding(0);
            clsTemaUI.AplicarEstiloBotonSecundario(btnGuardarPng);
            btnGuardarPng.Click += BtnGuardarPng_Click;
            tipAyuda.SetToolTip(btnGuardarPng, "Guarda los dos gráficos comparativos en formato PNG para el informe LaTeX");

            flwAcciones.Controls.Add(lblRepeticiones);
            flwAcciones.Controls.Add(numRepeticiones);
            flwAcciones.Controls.Add(btnEjecutar);
            flwAcciones.Controls.Add(btnEstabilidad);
            flwAcciones.Controls.Add(btnExportarCsv);
            flwAcciones.Controls.Add(btnGuardarPng);

            TableLayoutPanel tblEstado = new TableLayoutPanel();
            tblEstado.Dock = DockStyle.Fill;
            tblEstado.ColumnCount = 2;
            tblEstado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74F));
            tblEstado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26F));
            tblEstado.Margin = new Padding(0);

            lblEstado = new Label();
            lblEstado.Text = "Estado: Listo. Presione «Ejecutar pruebas» o «Estabilidad» para iniciar.";
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.AutoEllipsis = true;
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            lblEstado.Font = clsTemaUI.FntEtiquetaPequeNegrita;
            lblEstado.ForeColor = clsTemaUI.ColTextoSecundario;

            prgProgreso = new ProgressBar();
            prgProgreso.Dock = DockStyle.Fill;
            prgProgreso.Minimum = 0;
            prgProgreso.Maximum = 100;
            prgProgreso.Value = 0;
            prgProgreso.AccessibleName = "Progreso de ejecución de pruebas";
            prgProgreso.Margin = new Padding(12, 4, 0, 4);

            tblEstado.Controls.Add(lblEstado, 0, 0);
            tblEstado.Controls.Add(prgProgreso, 1, 0);

            tblBarraComandos.Controls.Add(flwAcciones, 0, 0);
            tblBarraComandos.Controls.Add(tblEstado, 0, 1);
            pnlTarjetaControles.Controls.Add(tblBarraComandos);
            pnlSuperior.Controls.Add(pnlTarjetaControles);

            // 3. Cuerpo principal con TabControl estilizado
            pnlCuerpo = new Panel();
            pnlCuerpo.Dock = DockStyle.Fill;
            pnlCuerpo.Padding = new Padding(clsTemaUI.EntEspacioMd, 6, clsTemaUI.EntEspacioMd, clsTemaUI.EntEspacioMd);
            pnlCuerpo.BackColor = clsTemaUI.ColFondoApp;

            tabContenedor = new TabControl();
            tabContenedor.Dock = DockStyle.Fill;
            tabContenedor.TabIndex = 5;
            tabContenedor.AccessibleName = "Vistas del laboratorio de pruebas";
            tabContenedor.Font = clsTemaUI.FntCuerpoNegrita;
            tabContenedor.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabContenedor.SizeMode = TabSizeMode.Fixed;
            tabContenedor.ItemSize = new Size(310, 36);
            tabContenedor.DrawItem += TabContenedor_DrawItem;

            tabTablaResultados = new TabPage("1. Tabla de Resultados");
            tabTablaResultados.BackColor = clsTemaUI.ColFondoApp;
            tabTablaResultados.Padding = new Padding(0, 8, 0, 0);

            tabGraficosComparativos = new TabPage("2. Gráficos Comparativos (Muestras y Grado)");
            tabGraficosComparativos.BackColor = clsTemaUI.ColFondoApp;
            tabGraficosComparativos.Padding = new Padding(0, 8, 0, 0);

            // Pestana 1: Tarjeta de tabla de resultados + banner de estabilidad
            pnlTarjetaTabla = new pnlTarjetaUI();
            pnlTarjetaTabla.Dock = DockStyle.Fill;
            pnlTarjetaTabla.Padding = new Padding(12);

            Panel pnlCabTabla = new Panel();
            pnlCabTabla.Dock = DockStyle.Top;
            pnlCabTabla.Height = 32;
            pnlCabTabla.Padding = new Padding(0, 0, 0, 6);

            Label lblTitTabla = new Label();
            lblTitTabla.Text = "Tabla comparativa de tiempos y errores numericos";
            lblTitTabla.Font = clsTemaUI.FntTituloTarjeta;
            lblTitTabla.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTitTabla.Dock = DockStyle.Left;
            lblTitTabla.AutoSize = true;

            Label lblInsigniaVerde = clsTemaUI.CrearInsignia("Verde = metodo mas rapido por fila", clsTemaUI.ColExitoSuave, clsTemaUI.ColExitoTextoOscuro);
            lblInsigniaVerde.Dock = DockStyle.Right;

            pnlCabTabla.Controls.Add(lblTitTabla);
            pnlCabTabla.Controls.Add(lblInsigniaVerde);

            pnlBannerEstabilidad = new Panel();
            pnlBannerEstabilidad.Dock = DockStyle.Bottom;
            pnlBannerEstabilidad.Height = 38;
            pnlBannerEstabilidad.BackColor = clsTemaUI.ColExitoSuave;
            pnlBannerEstabilidad.Padding = new Padding(12, 0, 12, 0);
            pnlBannerEstabilidad.Paint += (objRem, objArgs) =>
            {
                using (Pen penBorde = new Pen(clsTemaUI.ColExitoBorde, 1F))
                {
                    objArgs.Graphics.DrawRectangle(penBorde, 0, 0, pnlBannerEstabilidad.Width - 1, pnlBannerEstabilidad.Height - 1);
                }
            };

            lblResumenEstabilidad = new Label();
            lblResumenEstabilidad.Dock = DockStyle.Fill;
            lblResumenEstabilidad.Text = "Estabilidad numerica (grados 50 y 100 vs referencia decimal 128 bits): pendiente de ejecucion.";
            lblResumenEstabilidad.Font = clsTemaUI.FntMonoespacioPeque;
            lblResumenEstabilidad.ForeColor = clsTemaUI.ColExitoTextoOscuro;
            lblResumenEstabilidad.TextAlign = ContentAlignment.MiddleLeft;
            pnlBannerEstabilidad.Controls.Add(lblResumenEstabilidad);

            Panel pnlEspacioTabla = new Panel();
            pnlEspacioTabla.Dock = DockStyle.Bottom;
            pnlEspacioTabla.Height = 8;
            pnlEspacioTabla.BackColor = clsTemaUI.ColSuperficieTarjeta;

            dgvResultados = new DataGridView();
            dgvResultados.Dock = DockStyle.Fill;
            dgvResultados.TabIndex = 6;
            dgvResultados.AccessibleName = "Tabla comparativa de resultados experimentales";
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.ReadOnly = true;
            dgvResultados.RowHeadersVisible = false;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            clsTemaUI.AplicarEstiloDataGridView(dgvResultados);
            ConfigurarColumnasTabla();

            pnlTarjetaTabla.Controls.Add(dgvResultados);
            pnlTarjetaTabla.Controls.Add(pnlEspacioTabla);
            pnlTarjetaTabla.Controls.Add(pnlBannerEstabilidad);
            pnlTarjetaTabla.Controls.Add(pnlCabTabla);
            tabTablaResultados.Controls.Add(pnlTarjetaTabla);

            // Pestana 2: Filtros en tarjeta compacta + 2 tarjetas de graficos lado a lado
            pnlTarjetaFiltros = new pnlTarjetaUI();
            pnlTarjetaFiltros.Dock = DockStyle.Top;
            pnlTarjetaFiltros.Height = 46;
            pnlTarjetaFiltros.Padding = new Padding(12, 8, 12, 8);

            FlowLayoutPanel flwFiltros = new FlowLayoutPanel();
            flwFiltros.Dock = DockStyle.Fill;
            flwFiltros.WrapContents = false;
            flwFiltros.Margin = new Padding(0);

            lblGradoFijo = new Label();
            lblGradoFijo.Text = "Grado para Tiempo vs Muestras:";
            lblGradoFijo.Font = clsTemaUI.FntCuerpoNegrita;
            lblGradoFijo.ForeColor = clsTemaUI.ColTextoSecundario;
            lblGradoFijo.AutoSize = true;
            lblGradoFijo.Margin = new Padding(0, 6, 6, 4);

            cmbGradoFijo = new ComboBox();
            cmbGradoFijo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGradoFijo.Width = 76;
            cmbGradoFijo.Font = clsTemaUI.FntMonoespacio;
            cmbGradoFijo.Margin = new Padding(0, 2, 18, 2);
            int[] arrGrados = objControladorPruebas.ArrGradosPrueba;
            for (int entI = 0; entI < arrGrados.Length; entI++)
            {
                cmbGradoFijo.Items.Add(arrGrados[entI]);
            }
            cmbGradoFijo.SelectedIndex = arrGrados.Length - 1;
            cmbGradoFijo.SelectedIndexChanged += FiltroGraficos_Changed;

            lblMuestrasFijas = new Label();
            lblMuestrasFijas.Text = "Muestras para Tiempo vs Grado:";
            lblMuestrasFijas.Font = clsTemaUI.FntCuerpoNegrita;
            lblMuestrasFijas.ForeColor = clsTemaUI.ColTextoSecundario;
            lblMuestrasFijas.AutoSize = true;
            lblMuestrasFijas.Margin = new Padding(0, 6, 6, 4);

            cmbMuestrasFijas = new ComboBox();
            cmbMuestrasFijas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMuestrasFijas.Width = 96;
            cmbMuestrasFijas.Font = clsTemaUI.FntMonoespacio;
            cmbMuestrasFijas.Margin = new Padding(0, 2, 20, 2);
            int[] arrMuestras = objControladorPruebas.ArrMuestrasPrueba;
            for (int entI = 0; entI < arrMuestras.Length; entI++)
            {
                cmbMuestrasFijas.Items.Add(arrMuestras[entI]);
            }
            cmbMuestrasFijas.SelectedIndex = arrMuestras.Length - 1;
            cmbMuestrasFijas.SelectedIndexChanged += FiltroGraficos_Changed;

            lblLeyendaAzul = clsTemaUI.CrearInsignia("● Polinomios de Bernstein", clsTemaUI.ColPrimarioSuave, clsTemaUI.ColPrimario);
            lblLeyendaAzul.Margin = new Padding(0, 2, 8, 2);

            lblLeyendaNaranja = clsTemaUI.CrearInsignia("● Algoritmo de De Casteljau", clsTemaUI.ColCasteljauSuave, clsTemaUI.ColCasteljau);
            lblLeyendaNaranja.Margin = new Padding(0, 2, 0, 2);

            flwFiltros.Controls.Add(lblGradoFijo);
            flwFiltros.Controls.Add(cmbGradoFijo);
            flwFiltros.Controls.Add(lblMuestrasFijas);
            flwFiltros.Controls.Add(cmbMuestrasFijas);
            flwFiltros.Controls.Add(lblLeyendaAzul);
            flwFiltros.Controls.Add(lblLeyendaNaranja);
            pnlTarjetaFiltros.Controls.Add(flwFiltros);

            Panel pnlSepGraficos = new Panel();
            pnlSepGraficos.Dock = DockStyle.Top;
            pnlSepGraficos.Height = 10;
            pnlSepGraficos.BackColor = clsTemaUI.ColFondoApp;

            tblGraficos = new TableLayoutPanel();
            tblGraficos.Dock = DockStyle.Fill;
            tblGraficos.ColumnCount = 2;
            tblGraficos.RowCount = 1;
            tblGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblGraficos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblGraficos.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            pnlTarjetaUI pnlTarjetaGrafMuestras = new pnlTarjetaUI();
            pnlTarjetaGrafMuestras.Dock = DockStyle.Fill;
            pnlTarjetaGrafMuestras.Padding = new Padding(4);
            pnlTarjetaGrafMuestras.Margin = new Padding(0, 0, 5, 0);

            pnlGraficoMuestras = new pnlGraficoDobleBuffer();
            pnlGraficoMuestras.Dock = DockStyle.Fill;
            pnlGraficoMuestras.BackColor = clsTemaUI.ColSuperficieTarjeta;
            pnlGraficoMuestras.Paint += PnlGraficoMuestras_Paint;
            pnlTarjetaGrafMuestras.Controls.Add(pnlGraficoMuestras);

            pnlTarjetaUI pnlTarjetaGrafGrados = new pnlTarjetaUI();
            pnlTarjetaGrafGrados.Dock = DockStyle.Fill;
            pnlTarjetaGrafGrados.Padding = new Padding(4);
            pnlTarjetaGrafGrados.Margin = new Padding(5, 0, 0, 0);

            pnlGraficoGrados = new pnlGraficoDobleBuffer();
            pnlGraficoGrados.Dock = DockStyle.Fill;
            pnlGraficoGrados.BackColor = clsTemaUI.ColSuperficieTarjeta;
            pnlGraficoGrados.Paint += PnlGraficoGrados_Paint;
            pnlTarjetaGrafGrados.Controls.Add(pnlGraficoGrados);

            tblGraficos.Controls.Add(pnlTarjetaGrafMuestras, 0, 0);
            tblGraficos.Controls.Add(pnlTarjetaGrafGrados, 1, 0);

            tabGraficosComparativos.Controls.Add(tblGraficos);
            tabGraficosComparativos.Controls.Add(pnlSepGraficos);
            tabGraficosComparativos.Controls.Add(pnlTarjetaFiltros);

            tabContenedor.TabPages.Add(tabTablaResultados);
            tabContenedor.TabPages.Add(tabGraficosComparativos);
            pnlCuerpo.Controls.Add(tabContenedor);

            this.Controls.Add(pnlCuerpo);
            this.Controls.Add(pnlSuperior);
            this.Controls.Add(pnlCabecera);
        }

        // Dibuja las pestanas del TabControl con estilo plano moderno e indicador inferior activo
        private void TabContenedor_DrawItem(object? objRemitente, DrawItemEventArgs objArgs)
        {
            TabPage objPagina = tabContenedor.TabPages[objArgs.Index];
            Rectangle objRect = objArgs.Bounds;
            bool bolSeleccionada = (objArgs.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color colFondo = bolSeleccionada ? clsTemaUI.ColSuperficieTarjeta : clsTemaUI.ColSuperficieSutil;
            Color colTexto = bolSeleccionada ? clsTemaUI.ColPrimario : clsTemaUI.ColTextoSecundario;

            using (Brush brsFondo = new SolidBrush(colFondo))
            {
                objArgs.Graphics.FillRectangle(brsFondo, objRect);
            }

            if (bolSeleccionada)
            {
                using (Brush brsIndicador = new SolidBrush(clsTemaUI.ColPrimario))
                {
                    objArgs.Graphics.FillRectangle(brsIndicador, objRect.Left, objRect.Bottom - 3, objRect.Width, 3);
                }
            }
            else
            {
                using (Pen penDiv = new Pen(clsTemaUI.ColBordeSutil, 1F))
                {
                    objArgs.Graphics.DrawLine(penDiv, objRect.Left, objRect.Bottom - 1, objRect.Right, objRect.Bottom - 1);
                }
            }

            if ((objArgs.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                Rectangle objRectFoco = new Rectangle(objRect.X + 2, objRect.Y + 2, objRect.Width - 5, objRect.Height - 6);
                using (Pen penFoco = new Pen(clsTemaUI.ColFoco, 1.5F))
                {
                    penFoco.DashStyle = DashStyle.Dot;
                    objArgs.Graphics.DrawRectangle(penFoco, objRectFoco);
                }
            }

            TextRenderer.DrawText(
                objArgs.Graphics,
                objPagina.Text,
                bolSeleccionada ? clsTemaUI.FntCuerpoNegrita : clsTemaUI.FntCuerpo,
                objRect,
                colTexto,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        // Define las columnas de la tabla de resultados y alineacion tabular
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

            for (int entI = 0; entI <= 7; entI++)
            {
                dgvResultados.Columns[entI].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvResultados.Columns[entI].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvResultados.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultados.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResultados.Columns[8].DefaultCellStyle.Font = clsTemaUI.FntCuerpoNegrita;
        }

        // Redibuja ambos paneles cuando cambia el filtro de grado o muestras
        private void FiltroGraficos_Changed(object? objRemitente, EventArgs objArgumentos)
        {
            pnlGraficoMuestras.Invalidate();
            pnlGraficoGrados.Invalidate();
        }

        // Ejecuta los experimentos de forma asincrona y actualiza tabla, estabilidad y graficos
        private async void BtnEjecutar_Click(object? objRemitente, EventArgs objArgumentos)
        {
            if (bolEjecutando)
            {
                return;
            }

            bolEjecutando = true;
            btnEjecutar.Enabled = false;
            btnEjecutar.Text = "Ejecutando…";
            btnEstabilidad.Enabled = false;
            numRepeticiones.Enabled = false;
            dgvResultados.Rows.Clear();
            prgProgreso.Value = 0;
            lblEstado.Text = "Estado: Iniciando experimentos…";

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

            List<clsFilaEstabilidad> lstEst = await Task.Run(() =>
                objControladorPruebas.EjecutarPruebaEstabilidad(1000));

            lstDatosActuales = new List<clsFilaPrueba>(lstFilas);
            PoblarTablaResultados(lstDatosActuales);
            ActualizarEtiquetaEstabilidad(lstEst);
            pnlGraficoMuestras.Invalidate();
            pnlGraficoGrados.Invalidate();

            prgProgreso.Value = 100;
            lblEstado.Text = "Estado: Pruebas completadas. Puede cambiar a la pestana '2. Graficos Comparativos'.";
            btnEjecutar.Text = "Ejecutar pruebas";
            btnEjecutar.Enabled = true;
            btnEstabilidad.Enabled = true;
            numRepeticiones.Enabled = true;
            bolEjecutando = false;
        }

        // Ejecuta y muestra el reporte detallado de estabilidad numerica en grados 50 y 100
        private async void BtnEstabilidad_Click(object? objRemitente, EventArgs objArgumentos)
        {
            if (bolEjecutando)
            {
                return;
            }

            bolEjecutando = true;
            btnEstabilidad.Enabled = false;
            btnEstabilidad.Text = "Evaluando…";
            lblEstado.Text = "Estado: Evaluando estabilidad en grados 50 y 100 contra referencia decimal…";

            List<clsFilaEstabilidad> lstEst = await Task.Run(() =>
                objControladorPruebas.EjecutarPruebaEstabilidad(1000));

            ActualizarEtiquetaEstabilidad(lstEst);
            lblEstado.Text = "Estado: Prueba de estabilidad numerica completada.";
            btnEstabilidad.Text = "Estabilidad (n=50, 100)";
            btnEstabilidad.Enabled = true;
            bolEjecutando = false;

            string cadMensaje = "Prueba de estabilidad numerica (1000 muestras vs referencia decimal 128 bits):\n\n";
            for (int entI = 0; entI < lstEst.Count; entI++)
            {
                clsFilaEstabilidad objEst = lstEst[entI];
                cadMensaje += "Grado n = " + objEst.entGrado + ":\n";
                cadMensaje += "  - Error Max Bernstein vs Ref: " + objEst.dblErrorMaxBernsteinVsRef.ToString("E4") + "\n";
                cadMensaje += "  - Error Max De Casteljau vs Ref: " + objEst.dblErrorMaxCasteljauVsRef.ToString("E4") + "\n";
                cadMensaje += "  - Diferencia Max entre ambos: " + objEst.dblDiferenciaEntreAmbosMax.ToString("E4") + "\n";
                cadMensaje += "  - Metodo mas estable: " + objEst.cadMetodoMasEstable + "\n\n";
            }

            MessageBox.Show(cadMensaje, "Estabilidad Numerica en Grados Altos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Actualiza el resumen inferior con los resultados de estabilidad en grados 50 y 100
        private void ActualizarEtiquetaEstabilidad(List<clsFilaEstabilidad> lstEst)
        {
            if (lstEst == null || lstEst.Count < 2)
            {
                return;
            }

            clsFilaEstabilidad objG50 = lstEst[0];
            clsFilaEstabilidad objG100 = lstEst[1];
            lblResumenEstabilidad.Text =
                "Estabilidad vs Decimal -> Grado 50: ErrBernstein=" + objG50.dblErrorMaxBernsteinVsRef.ToString("E2") +
                ", ErrCasteljau=" + objG50.dblErrorMaxCasteljauVsRef.ToString("E2") +
                " (" + objG50.cadMetodoMasEstable + ") | Grado 100: ErrBernstein=" + objG100.dblErrorMaxBernsteinVsRef.ToString("E2") +
                ", ErrCasteljau=" + objG100.dblErrorMaxCasteljauVsRef.ToString("E2") +
                " (" + objG100.cadMetodoMasEstable + ")";
        }

        // Exporta la tabla de resultados y estabilidad a un archivo CSV
        private void BtnExportarCsv_Click(object? objRemitente, EventArgs objArgumentos)
        {
            if (lstDatosActuales.Count == 0)
            {
                MessageBox.Show("Ejecute primero las pruebas antes de exportar a CSV.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlgGuardar = new SaveFileDialog())
            {
                dlgGuardar.Title = "Exportar resultados a CSV";
                dlgGuardar.Filter = "Archivo CSV (*.csv)|*.csv";
                dlgGuardar.FileName = "resultados_bezier.csv";

                if (dlgGuardar.ShowDialog() == DialogResult.OK)
                {
                    objControladorPruebas.ExportarResultadosCsv(dlgGuardar.FileName);
                    lblEstado.Text = "Estado: CSV exportado en " + dlgGuardar.FileName;
                    MessageBox.Show("Resultados exportados correctamente a CSV.", "Exportacion exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Guarda los dos graficos comparativos como archivos PNG para el informe en LaTeX
        private void BtnGuardarPng_Click(object? objRemitente, EventArgs objArgumentos)
        {
            if (lstDatosActuales.Count == 0)
            {
                MessageBox.Show("Ejecute primero las pruebas antes de guardar los graficos PNG.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FolderBrowserDialog dlgCarpeta = new FolderBrowserDialog())
            {
                dlgCarpeta.Description = "Seleccione la carpeta donde se guardaran los graficos PNG (ej. informe/figuras)";

                if (dlgCarpeta.ShowDialog() == DialogResult.OK)
                {
                    string cadRutaMuestras = Path.Combine(dlgCarpeta.SelectedPath, "exp_tiempo_vs_muestras.png");
                    string cadRutaGrados = Path.Combine(dlgCarpeta.SelectedPath, "exp_tiempo_vs_grado.png");

                    GuardarGraficoComoPng(cadRutaMuestras, true);
                    GuardarGraficoComoPng(cadRutaGrados, false);

                    lblEstado.Text = "Estado: Graficos PNG guardados en " + dlgCarpeta.SelectedPath;
                    MessageBox.Show(
                        "Graficos guardados como PNG:\n- " + cadRutaMuestras + "\n- " + cadRutaGrados,
                        "Imagenes guardadas",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        // Renderiza el grafico directamente en un Bitmap de alta resolucion y lo guarda en PNG
        private void GuardarGraficoComoPng(string cadRutaArchivo, bool bolEsGraficoMuestras)
        {
            int entAncho = 800;
            int entAlto = 480;

            using (Bitmap bmpImagen = new Bitmap(entAncho, entAlto))
            using (Graphics objLienzo = Graphics.FromImage(bmpImagen))
            {
                Rectangle objArea = new Rectangle(0, 0, entAncho, entAlto);
                if (bolEsGraficoMuestras)
                {
                    DibujarGraficoMuestras(objLienzo, objArea);
                }
                else
                {
                    DibujarGraficoGrados(objLienzo, objArea);
                }

                bmpImagen.Save(cadRutaArchivo, ImageFormat.Png);
            }
        }

        // Llena el DataGridView y resalta en verde el metodo mas rapido de cada fila
        private void PoblarTablaResultados(List<clsFilaPrueba> lstFilas)
        {
            dgvResultados.Rows.Clear();
            Color colVerdeGanador = clsTemaUI.ColExitoSuave;
            Color colTextoGanador = clsTemaUI.ColExitoTextoOscuro;

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
                objFilaVisual.Cells[8].Style.ForeColor = colTextoGanador;

                if (objFila.dblTiempoBernsteinMs <= objFila.dblTiempoDeCasteljauMs)
                {
                    objFilaVisual.Cells[2].Style.BackColor = colVerdeGanador;
                    objFilaVisual.Cells[2].Style.ForeColor = colTextoGanador;
                    objFilaVisual.Cells[2].Style.Font = clsTemaUI.FntMonoespacioNegrita;
                }
                else
                {
                    objFilaVisual.Cells[4].Style.BackColor = colVerdeGanador;
                    objFilaVisual.Cells[4].Style.ForeColor = colTextoGanador;
                    objFilaVisual.Cells[4].Style.Font = clsTemaUI.FntMonoespacioNegrita;
                }
            }

            dgvResultados.ClearSelection();
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

        // Dibuja ejes, leyenda y series usando unicamente DrawLine, FillEllipse, FillPolygon y DrawString
        private void DibujarGraficoComparativo(
            Graphics objLienzo,
            Rectangle objArea,
            string cadTitulo,
            string cadEjeX,
            List<string> lstEtiquetasX,
            List<double> lstSerieBernstein,
            List<double> lstSerieCasteljau)
        {
            objLienzo.SmoothingMode = SmoothingMode.AntiAlias;
            objLienzo.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            objLienzo.Clear(clsTemaUI.ColSuperficieTarjeta);

            using (Font fntTitulo = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Font fntEjes = new Font("Consolas", 8F, FontStyle.Regular))
            using (Font fntEtiquetaEje = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (Brush brsTexto = new SolidBrush(clsTemaUI.ColTextoPrincipal))
            using (Brush brsSecundario = new SolidBrush(clsTemaUI.ColTextoSecundario))
            using (Brush brsFondoPlot = new SolidBrush(clsTemaUI.ColFondoApp))
            using (Pen penEje = new Pen(clsTemaUI.ColTextoSecundario, 1.5F))
            using (Pen penGuia = new Pen(clsTemaUI.ColBordeSutil, 1F))
            using (Pen penBernstein = new Pen(clsTemaUI.ColPrimario, 2.6F))
            using (Pen penCasteljau = new Pen(clsTemaUI.ColCasteljauTrazo, 2.6F))
            using (Pen penHaloBlanco = new Pen(Color.White, 1.6F))
            using (Brush brsBernstein = new SolidBrush(clsTemaUI.ColPrimario))
            using (Brush brsCasteljau = new SolidBrush(clsTemaUI.ColCasteljauTrazo))
            {
                penGuia.DashStyle = DashStyle.Dot;
                penBernstein.StartCap = LineCap.Round;
                penBernstein.EndCap = LineCap.Round;
                penCasteljau.StartCap = LineCap.Round;
                penCasteljau.EndCap = LineCap.Round;

                objLienzo.DrawString(cadTitulo, fntTitulo, brsTexto, 16, 10);

                if (lstEtiquetasX.Count == 0)
                {
                    int entAnchoVacio = Math.Min(360, objArea.Width - 40);
                    int entAltoVacio = 74;
                    int entX0 = (objArea.Width - entAnchoVacio) / 2;
                    int entY0 = (objArea.Height - entAltoVacio) / 2;
                    Rectangle objCajaVacia = new Rectangle(Math.Max(12, entX0), Math.Max(40, entY0), Math.Max(120, entAnchoVacio), entAltoVacio);

                    using (Pen penMarcoVacio = new Pen(clsTemaUI.ColBordeTarjeta, 1F))
                    {
                        objLienzo.FillRectangle(brsFondoPlot, objCajaVacia);
                        objLienzo.DrawRectangle(penMarcoVacio, objCajaVacia);
                    }
                    objLienzo.DrawString(
                        "Sin datos experimentales todavia",
                        fntEtiquetaEje,
                        brsTexto,
                        objCajaVacia.X + 16,
                        objCajaVacia.Y + 14);
                    objLienzo.DrawString(
                        "Presione 'Ejecutar pruebas' para calcular y trazar las series.",
                        clsTemaUI.FntEtiquetaPeque,
                        brsSecundario,
                        objCajaVacia.X + 16,
                        objCajaVacia.Y + 38);
                    return;
                }

                int entMargenIzq = 72;
                int entMargenDer = 28;
                int entMargenSup = 44;
                int entMargenInf = 54;

                int entAnchoGrafico = objArea.Width - entMargenIzq - entMargenDer;
                int entAltoGrafico = objArea.Height - entMargenSup - entMargenInf;

                if (entAnchoGrafico <= 40 || entAltoGrafico <= 40)
                {
                    return;
                }

                int entOrigenX = entMargenIzq;
                int entOrigenY = entMargenSup + entAltoGrafico;

                // Fondo sutil del area de trazado
                objLienzo.FillRectangle(brsFondoPlot, entOrigenX, entMargenSup, entAnchoGrafico, entAltoGrafico);

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
                dblMaximoY = dblMaximoY * 1.15;

                // Dibuja lineas guia horizontales punteadas y marcas del eje Y (Tiempo en ms)
                int entDivisionesY = 4;
                for (int entPaso = 0; entPaso <= entDivisionesY; entPaso++)
                {
                    double dblFraccion = (double)entPaso / (double)entDivisionesY;
                    int entY = entOrigenY - (int)(dblFraccion * entAltoGrafico);
                    double dblValorY = dblFraccion * dblMaximoY;

                    objLienzo.DrawLine(penGuia, entOrigenX, entY, entOrigenX + entAnchoGrafico, entY);
                    string cadMarcaY = dblValorY >= 10.0 ? dblValorY.ToString("F1") : dblValorY.ToString("F2");
                    SizeF objTamY = objLienzo.MeasureString(cadMarcaY, fntEjes);
                    objLienzo.DrawString(cadMarcaY, fntEjes, brsSecundario, entOrigenX - objTamY.Width - 6, entY - objTamY.Height / 2F);
                }

                objLienzo.DrawString("ms", clsTemaUI.FntMonoespacioPeque, brsSecundario, 16, entMargenSup - 14);

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

                    objLienzo.DrawLine(penEje, entX, entOrigenY, entX, entOrigenY + 5);
                    SizeF objTamX = objLienzo.MeasureString(lstEtiquetasX[entI], fntEjes);
                    objLienzo.DrawString(lstEtiquetasX[entI], fntEjes, brsSecundario, entX - objTamX.Width / 2F, entOrigenY + 8);
                }

                // Une los puntos consecutivos con segmentos rectos (DrawLine)
                for (int entI = 0; entI < entCantidadPuntos - 1; entI++)
                {
                    objLienzo.DrawLine(penBernstein, arrPuntosBernstein[entI], arrPuntosBernstein[entI + 1]);
                    objLienzo.DrawLine(penCasteljau, arrPuntosCasteljau[entI], arrPuntosCasteljau[entI + 1]);
                }

                // Dibuja marcadores diferenciados por forma y color: circulo (Bernstein) y rombo (De Casteljau)
                for (int entI = 0; entI < entCantidadPuntos; entI++)
                {
                    Point ptoB = arrPuntosBernstein[entI];
                    objLienzo.FillEllipse(brsBernstein, ptoB.X - 4.5F, ptoB.Y - 4.5F, 9F, 9F);
                    objLienzo.DrawEllipse(penHaloBlanco, ptoB.X - 4.5F, ptoB.Y - 4.5F, 9F, 9F);

                    Point ptoC = arrPuntosCasteljau[entI];
                    Point[] arrRombo = new Point[]
                    {
                        new Point(ptoC.X, ptoC.Y - 5),
                        new Point(ptoC.X + 5, ptoC.Y),
                        new Point(ptoC.X, ptoC.Y + 5),
                        new Point(ptoC.X - 5, ptoC.Y)
                    };
                    objLienzo.FillPolygon(brsCasteljau, arrRombo);
                    objLienzo.DrawPolygon(penHaloBlanco, arrRombo);
                }

                // Anotacion directa del ultimo valor de cada serie para lectura inmediata
                if (entCantidadPuntos > 0)
                {
                    int entUltimo = entCantidadPuntos - 1;
                    string cadUltB = lstSerieBernstein[entUltimo].ToString("F2") + " ms";
                    string cadUltC = lstSerieCasteljau[entUltimo].ToString("F2") + " ms";
                    SizeF objTamB = objLienzo.MeasureString(cadUltB, fntEjes);
                    SizeF objTamC = objLienzo.MeasureString(cadUltC, fntEjes);

                    int entPosYB = arrPuntosBernstein[entUltimo].Y - 18;
                    int entPosYC = arrPuntosCasteljau[entUltimo].Y - 18;
                    if (Math.Abs(entPosYB - entPosYC) < 14)
                    {
                        entPosYB = Math.Min(entOrigenY - 16, arrPuntosBernstein[entUltimo].Y + 6);
                    }

                    objLienzo.DrawString(cadUltB, fntEjes, brsBernstein, arrPuntosBernstein[entUltimo].X - objTamB.Width - 6, Math.Max(entMargenSup + 2, entPosYB));
                    objLienzo.DrawString(cadUltC, fntEjes, brsCasteljau, arrPuntosCasteljau[entUltimo].X - objTamC.Width - 6, Math.Max(entMargenSup + 2, entPosYC));
                }

                // Etiqueta centrada del eje X
                SizeF objTamEjeX = objLienzo.MeasureString(cadEjeX, fntEtiquetaEje);
                objLienzo.DrawString(cadEjeX, fntEtiquetaEje, brsSecundario, entOrigenX + (entAnchoGrafico - objTamEjeX.Width) / 2F, objArea.Height - 22);

                // Caja de leyenda interna diferenciada por forma y color
                int entLeyendaX = entOrigenX + 10;
                int entLeyendaY = entMargenSup + 8;
                Rectangle objCajaLeyenda = new Rectangle(entLeyendaX, entLeyendaY, 214, 24);
                using (Brush brsCajaLey = new SolidBrush(Color.FromArgb(242, 255, 255, 255)))
                using (Pen penCajaLey = new Pen(clsTemaUI.ColBordeSutil, 1F))
                {
                    objLienzo.FillRectangle(brsCajaLey, objCajaLeyenda);
                    objLienzo.DrawRectangle(penCajaLey, objCajaLeyenda);
                }

                objLienzo.DrawLine(penBernstein, entLeyendaX + 8, entLeyendaY + 12, entLeyendaX + 24, entLeyendaY + 12);
                objLienzo.FillEllipse(brsBernstein, entLeyendaX + 13, entLeyendaY + 9, 6, 6);
                objLienzo.DrawString("Bernstein", clsTemaUI.FntEtiquetaPequeNegrita, brsBernstein, entLeyendaX + 29, entLeyendaY + 4);

                int entLeyCX = entLeyendaX + 104;
                objLienzo.DrawLine(penCasteljau, entLeyCX, entLeyendaY + 12, entLeyCX + 16, entLeyendaY + 12);
                Point[] arrMiniRombo = new Point[]
                {
                    new Point(entLeyCX + 8, entLeyendaY + 8),
                    new Point(entLeyCX + 12, entLeyendaY + 12),
                    new Point(entLeyCX + 8, entLeyendaY + 16),
                    new Point(entLeyCX + 4, entLeyendaY + 12)
                };
                objLienzo.FillPolygon(brsCasteljau, arrMiniRombo);
                objLienzo.DrawString("De Casteljau", clsTemaUI.FntEtiquetaPequeNegrita, brsCasteljau, entLeyCX + 21, entLeyendaY + 4);
            }
        }
    }
}
