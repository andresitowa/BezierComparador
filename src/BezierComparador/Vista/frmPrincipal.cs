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
        private Panel pnlCabecera = null!;
        private Label lblTituloApp = null!;
        private Label lblSubtituloApp = null!;
        private Label lblInsigniaGradoCabecera = null!;
        private Label lblCoordenadasCursor = null!;
        private Panel pnlContenedorLienzo = null!;
        private pnlTarjetaUI pnlMarcoLienzo = null!;
        private Panel pnlBarraEstadoLienzo = null!;
        private Label lblGuiaInteraccion = null!;
        private ctlLienzo picLienzo = null!;
        private Panel pnlLateral = null!;
        private TableLayoutPanel tblInspector = null!;
        private pnlTarjetaUI pnlTarjetaPuntos = null!;
        private ListBox lstListaPuntos = null!;
        private Label lblGradoActual = null!;
        private Button btnCurvaEjemplo = null!;
        private Button btnLimpiar = null!;
        private pnlTarjetaUI pnlTarjetaConfiguracion = null!;
        private Label lblMuestras = null!;
        private NumericUpDown numMuestras = null!;
        private CheckBox chkBernstein = null!;
        private CheckBox chkCasteljau = null!;
        private CheckBox chkPoligono = null!;
        private CheckBox chkInterpolaciones = null!;
        private Label lblParametroT = null!;
        private TrackBar trkParametroT = null!;
        private pnlTarjetaUI pnlTarjetaResultados = null!;
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

        // Construye la interfaz siguiendo el sistema de diseno y estructura responsiva
        private void InicializarComponentes()
        {
            this.Text = "Comparador de Curvas de Bezier: Polinomios de Bernstein vs De Casteljau";
            this.Size = new Size(1300, 780);
            this.MinimumSize = new Size(980, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            clsTemaUI.AplicarEstiloFormulario(this);
            this.DoubleBuffered = true;

            tipAyuda = new ToolTip();
            clsTemaUI.ConfigurarToolTip(tipAyuda);

            // 1. Cabecera superior limpia con titulo, grado actual y coordenadas del cursor
            pnlCabecera = new Panel();
            pnlCabecera.Dock = DockStyle.Top;
            pnlCabecera.Height = 58;
            pnlCabecera.BackColor = clsTemaUI.ColCabeceraApp;
            pnlCabecera.Padding = new Padding(clsTemaUI.EntEspacioLg, 8, clsTemaUI.EntEspacioLg, 8);

            TableLayoutPanel tblCabecera = new TableLayoutPanel();
            tblCabecera.Dock = DockStyle.Fill;
            tblCabecera.ColumnCount = 2;
            tblCabecera.RowCount = 1;
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));

            FlowLayoutPanel flwTitulos = new FlowLayoutPanel();
            flwTitulos.Dock = DockStyle.Fill;
            flwTitulos.FlowDirection = FlowDirection.TopDown;
            flwTitulos.WrapContents = false;
            flwTitulos.Margin = new Padding(0);

            lblTituloApp = new Label();
            lblTituloApp.Text = "BezierComparador";
            lblTituloApp.Font = clsTemaUI.FntTituloVentana;
            lblTituloApp.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTituloApp.AutoSize = true;
            lblTituloApp.Margin = new Padding(0);

            lblSubtituloApp = new Label();
            lblSubtituloApp.Text = "Polinomios de Bernstein vs. Algoritmo de De Casteljau";
            lblSubtituloApp.Font = clsTemaUI.FntSubtituloVentana;
            lblSubtituloApp.ForeColor = clsTemaUI.ColTextoSecundario;
            lblSubtituloApp.AutoSize = true;
            lblSubtituloApp.Margin = new Padding(0, 1, 0, 0);

            flwTitulos.Controls.Add(lblTituloApp);
            flwTitulos.Controls.Add(lblSubtituloApp);

            FlowLayoutPanel flwInsignias = new FlowLayoutPanel();
            flwInsignias.Dock = DockStyle.Fill;
            flwInsignias.FlowDirection = FlowDirection.RightToLeft;
            flwInsignias.WrapContents = false;
            flwInsignias.Padding = new Padding(0, 8, 0, 0);
            flwInsignias.Margin = new Padding(0);

            lblInsigniaGradoCabecera = clsTemaUI.CrearInsignia("Grado n = 0 (0 pts)", clsTemaUI.ColPrimarioSuave, clsTemaUI.ColPrimario);
            lblInsigniaGradoCabecera.Font = clsTemaUI.FntMonoespacioNegrita;
            lblInsigniaGradoCabecera.Margin = new Padding(8, 0, 0, 0);

            lblCoordenadasCursor = clsTemaUI.CrearInsignia("Cursor: X —  Y —", clsTemaUI.ColSuperficieSutil, clsTemaUI.ColTextoSecundario);
            lblCoordenadasCursor.Font = clsTemaUI.FntMonoespacioPeque;

            flwInsignias.Controls.Add(lblInsigniaGradoCabecera);
            flwInsignias.Controls.Add(lblCoordenadasCursor);

            tblCabecera.Controls.Add(flwTitulos, 0, 0);
            tblCabecera.Controls.Add(flwInsignias, 1, 0);

            Panel pnlBordeCabecera = new Panel();
            pnlBordeCabecera.Dock = DockStyle.Bottom;
            pnlBordeCabecera.Height = 1;
            pnlBordeCabecera.BackColor = clsTemaUI.ColBordeSutil;

            pnlCabecera.Controls.Add(tblCabecera);
            pnlCabecera.Controls.Add(pnlBordeCabecera);

            // 2. Panel lateral derecho con scroll automatico y 3 tarjetas modernas
            pnlLateral = new Panel();
            pnlLateral.Dock = DockStyle.Right;
            pnlLateral.Width = 396;
            pnlLateral.BackColor = clsTemaUI.ColFondoApp;
            pnlLateral.AutoScroll = true;
            pnlLateral.Padding = new Padding(0, clsTemaUI.EntEspacioMd, clsTemaUI.EntEspacioMd, clsTemaUI.EntEspacioMd);

            tblInspector = new TableLayoutPanel();
            tblInspector.Dock = DockStyle.Top;
            tblInspector.AutoSize = true;
            tblInspector.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tblInspector.ColumnCount = 1;
            tblInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblInspector.RowCount = 3;
            tblInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblInspector.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // --- TARJETA 1: Puntos de control ---
            pnlTarjetaPuntos = new pnlTarjetaUI();
            pnlTarjetaPuntos.Dock = DockStyle.Top;
            pnlTarjetaPuntos.AutoSize = true;
            pnlTarjetaPuntos.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTarjetaPuntos.BolMostrarAcentoSuperior = true;
            pnlTarjetaPuntos.ColAcentoSuperior = clsTemaUI.ColPeligro;
            pnlTarjetaPuntos.Padding = new Padding(12, 12, 12, 10);
            pnlTarjetaPuntos.Margin = new Padding(0, 0, 0, 10);

            TableLayoutPanel tblPuntos = new TableLayoutPanel();
            tblPuntos.Dock = DockStyle.Top;
            tblPuntos.AutoSize = true;
            tblPuntos.ColumnCount = 1;
            tblPuntos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            TableLayoutPanel tblCabPuntos = new TableLayoutPanel();
            tblCabPuntos.Dock = DockStyle.Top;
            tblCabPuntos.AutoSize = true;
            tblCabPuntos.ColumnCount = 2;
            tblCabPuntos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            tblCabPuntos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tblCabPuntos.Margin = new Padding(0, 0, 0, 6);

            Label lblTitPuntos = new Label();
            lblTitPuntos.Text = "Puntos de control";
            lblTitPuntos.Font = clsTemaUI.FntTituloTarjeta;
            lblTitPuntos.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTitPuntos.AutoSize = true;
            lblTitPuntos.Anchor = AnchorStyles.Left;

            lblGradoActual = clsTemaUI.CrearInsignia("n = 0 (0 puntos)", clsTemaUI.ColPeligroSuave, clsTemaUI.ColPeligroHover);
            lblGradoActual.Anchor = AnchorStyles.Right;

            tblCabPuntos.Controls.Add(lblTitPuntos, 0, 0);
            tblCabPuntos.Controls.Add(lblGradoActual, 1, 0);

            Panel pnlMarcoLista = new Panel();
            pnlMarcoLista.Dock = DockStyle.Top;
            pnlMarcoLista.Height = 96;
            pnlMarcoLista.BackColor = clsTemaUI.ColFondoApp;
            pnlMarcoLista.Padding = new Padding(1);
            pnlMarcoLista.Margin = new Padding(0, 0, 0, 8);
            pnlMarcoLista.Paint += (objRem, objArgs) =>
            {
                using (Pen penBorde = new Pen(clsTemaUI.ColBordeSutil, 1F))
                {
                    objArgs.Graphics.DrawRectangle(penBorde, 0, 0, pnlMarcoLista.Width - 1, pnlMarcoLista.Height - 1);
                }
            };

            lstListaPuntos = new ListBox();
            lstListaPuntos.Dock = DockStyle.Fill;
            lstListaPuntos.BorderStyle = BorderStyle.None;
            lstListaPuntos.BackColor = clsTemaUI.ColFondoApp;
            lstListaPuntos.ForeColor = clsTemaUI.ColTextoPrincipal;
            lstListaPuntos.Font = clsTemaUI.FntMonoespacio;
            lstListaPuntos.IntegralHeight = false;
            pnlMarcoLista.Controls.Add(lstListaPuntos);

            TableLayoutPanel tblBotonesPuntos = new TableLayoutPanel();
            tblBotonesPuntos.Dock = DockStyle.Top;
            tblBotonesPuntos.Height = clsTemaUI.EntAlturaBoton;
            tblBotonesPuntos.ColumnCount = 2;
            tblBotonesPuntos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            tblBotonesPuntos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            tblBotonesPuntos.Margin = new Padding(0);

            btnCurvaEjemplo = new Button();
            btnCurvaEjemplo.Text = "Cargar curva grado 3";
            btnCurvaEjemplo.Dock = DockStyle.Fill;
            btnCurvaEjemplo.Margin = new Padding(0, 0, 4, 0);
            clsTemaUI.AplicarEstiloBotonSecundario(btnCurvaEjemplo);
            btnCurvaEjemplo.Click += BtnCurvaEjemplo_Click;
            tipAyuda.SetToolTip(btnCurvaEjemplo, "Carga 4 puntos de control de ejemplo para una curva cubica");

            btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar lienzo";
            btnLimpiar.Dock = DockStyle.Fill;
            btnLimpiar.Margin = new Padding(4, 0, 0, 0);
            clsTemaUI.AplicarEstiloBotonPeligroSuave(btnLimpiar);
            btnLimpiar.Click += BtnLimpiar_Click;
            tipAyuda.SetToolTip(btnLimpiar, "Elimina todos los puntos de control del lienzo");

            tblBotonesPuntos.Controls.Add(btnCurvaEjemplo, 0, 0);
            tblBotonesPuntos.Controls.Add(btnLimpiar, 1, 0);

            tblPuntos.Controls.Add(tblCabPuntos, 0, 0);
            tblPuntos.Controls.Add(pnlMarcoLista, 0, 1);
            tblPuntos.Controls.Add(tblBotonesPuntos, 0, 2);
            pnlTarjetaPuntos.Controls.Add(tblPuntos);

            // --- TARJETA 2: Configuracion y Capas ---
            pnlTarjetaConfiguracion = new pnlTarjetaUI();
            pnlTarjetaConfiguracion.Dock = DockStyle.Top;
            pnlTarjetaConfiguracion.AutoSize = true;
            pnlTarjetaConfiguracion.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTarjetaConfiguracion.BolMostrarAcentoSuperior = true;
            pnlTarjetaConfiguracion.ColAcentoSuperior = clsTemaUI.ColPrimario;
            pnlTarjetaConfiguracion.Padding = new Padding(12, 12, 12, 8);
            pnlTarjetaConfiguracion.Margin = new Padding(0, 0, 0, 10);

            TableLayoutPanel tblConfig = new TableLayoutPanel();
            tblConfig.Dock = DockStyle.Top;
            tblConfig.AutoSize = true;
            tblConfig.ColumnCount = 1;
            tblConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Label lblTitConfig = new Label();
            lblTitConfig.Text = "Configuracion y capas";
            lblTitConfig.Font = clsTemaUI.FntTituloTarjeta;
            lblTitConfig.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTitConfig.AutoSize = true;
            lblTitConfig.Margin = new Padding(0, 0, 0, 8);

            TableLayoutPanel tblFilaMuestras = new TableLayoutPanel();
            tblFilaMuestras.Dock = DockStyle.Top;
            tblFilaMuestras.Height = 30;
            tblFilaMuestras.ColumnCount = 2;
            tblFilaMuestras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tblFilaMuestras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tblFilaMuestras.Margin = new Padding(0, 0, 0, 6);

            lblMuestras = new Label();
            lblMuestras.Text = "Cantidad de muestras (N):";
            lblMuestras.ForeColor = clsTemaUI.ColTextoSecundario;
            lblMuestras.AutoSize = true;
            lblMuestras.Anchor = AnchorStyles.Left;

            numMuestras = new NumericUpDown();
            numMuestras.Minimum = 10;
            numMuestras.Maximum = 100000;
            numMuestras.Increment = 100;
            numMuestras.Value = 200;
            numMuestras.Font = clsTemaUI.FntMonoespacio;
            numMuestras.Dock = DockStyle.Fill;
            numMuestras.TextAlign = HorizontalAlignment.Right;
            numMuestras.ValueChanged += Configuracion_Changed;
            tipAyuda.SetToolTip(numMuestras, "Numero de puntos evaluados a lo largo de la curva");

            tblFilaMuestras.Controls.Add(lblMuestras, 0, 0);
            tblFilaMuestras.Controls.Add(numMuestras, 1, 0);

            chkBernstein = new CheckBox();
            chkBernstein.Text = "[Azul] Curva con Polinomios de Bernstein";
            chkBernstein.ForeColor = clsTemaUI.ColPrimario;
            chkBernstein.Font = clsTemaUI.FntCuerpoNegrita;
            chkBernstein.Checked = true;
            chkBernstein.AutoSize = true;
            chkBernstein.Cursor = Cursors.Hand;
            chkBernstein.Margin = new Padding(2, 2, 0, 2);
            chkBernstein.CheckedChanged += Configuracion_Changed;

            chkCasteljau = new CheckBox();
            chkCasteljau.Text = "[Naranja] Curva con Algoritmo De Casteljau";
            chkCasteljau.ForeColor = clsTemaUI.ColCasteljau;
            chkCasteljau.Font = clsTemaUI.FntCuerpoNegrita;
            chkCasteljau.Checked = true;
            chkCasteljau.AutoSize = true;
            chkCasteljau.Cursor = Cursors.Hand;
            chkCasteljau.Margin = new Padding(2, 2, 0, 2);
            chkCasteljau.CheckedChanged += Configuracion_Changed;

            chkInterpolaciones = new CheckBox();
            chkInterpolaciones.Text = "[Verde] Interpolaciones de De Casteljau";
            chkInterpolaciones.ForeColor = clsTemaUI.ColExito;
            chkInterpolaciones.Font = clsTemaUI.FntCuerpoNegrita;
            chkInterpolaciones.Checked = true;
            chkInterpolaciones.AutoSize = true;
            chkInterpolaciones.Cursor = Cursors.Hand;
            chkInterpolaciones.Margin = new Padding(2, 2, 0, 2);
            chkInterpolaciones.CheckedChanged += Configuracion_Changed;

            chkPoligono = new CheckBox();
            chkPoligono.Text = "[Rojo/Gris] Puntos y poligono de control";
            chkPoligono.ForeColor = clsTemaUI.ColPeligroHover;
            chkPoligono.Font = clsTemaUI.FntCuerpo;
            chkPoligono.Checked = true;
            chkPoligono.AutoSize = true;
            chkPoligono.Cursor = Cursors.Hand;
            chkPoligono.Margin = new Padding(2, 2, 0, 6);
            chkPoligono.CheckedChanged += Configuracion_Changed;

            Panel pnlSepT = clsTemaUI.CrearSeparadorHorizontal();
            pnlSepT.Margin = new Padding(0, 2, 0, 6);

            lblParametroT = new Label();
            lblParametroT.Text = "Parametro de interpolacion t = 0.50";
            lblParametroT.Font = clsTemaUI.FntCuerpoNegrita;
            lblParametroT.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblParametroT.AutoSize = true;
            lblParametroT.Margin = new Padding(0, 2, 0, 0);

            trkParametroT = new TrackBar();
            trkParametroT.Minimum = 0;
            trkParametroT.Maximum = 100;
            trkParametroT.Value = 50;
            trkParametroT.TickFrequency = 10;
            trkParametroT.Dock = DockStyle.Top;
            trkParametroT.Height = 36;
            trkParametroT.Margin = new Padding(0, 2, 0, 0);
            trkParametroT.Scroll += TrkParametroT_Scroll;
            tipAyuda.SetToolTip(trkParametroT, "Deslice para ver las subdivisiones de De Casteljau en t");

            tblConfig.Controls.Add(lblTitConfig, 0, 0);
            tblConfig.Controls.Add(tblFilaMuestras, 0, 1);
            tblConfig.Controls.Add(chkBernstein, 0, 2);
            tblConfig.Controls.Add(chkCasteljau, 0, 3);
            tblConfig.Controls.Add(chkInterpolaciones, 0, 4);
            tblConfig.Controls.Add(chkPoligono, 0, 5);
            tblConfig.Controls.Add(pnlSepT, 0, 6);
            tblConfig.Controls.Add(lblParametroT, 0, 7);
            tblConfig.Controls.Add(trkParametroT, 0, 8);
            pnlTarjetaConfiguracion.Controls.Add(tblConfig);

            // --- TARJETA 3: Resultados y acceso al Laboratorio de Pruebas ---
            pnlTarjetaResultados = new pnlTarjetaUI();
            pnlTarjetaResultados.Dock = DockStyle.Top;
            pnlTarjetaResultados.AutoSize = true;
            pnlTarjetaResultados.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlTarjetaResultados.BolMostrarAcentoSuperior = true;
            pnlTarjetaResultados.ColAcentoSuperior = clsTemaUI.ColExito;
            pnlTarjetaResultados.Padding = new Padding(12, 12, 12, 12);
            pnlTarjetaResultados.Margin = new Padding(0);

            TableLayoutPanel tblRes = new TableLayoutPanel();
            tblRes.Dock = DockStyle.Top;
            tblRes.AutoSize = true;
            tblRes.ColumnCount = 1;
            tblRes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            TableLayoutPanel tblCabRes = new TableLayoutPanel();
            tblCabRes.Dock = DockStyle.Top;
            tblCabRes.AutoSize = true;
            tblCabRes.ColumnCount = 2;
            tblCabRes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tblCabRes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tblCabRes.Margin = new Padding(0, 0, 0, 8);

            Label lblTitRes = new Label();
            lblTitRes.Text = "Metricas de la curva actual";
            lblTitRes.Font = clsTemaUI.FntTituloTarjeta;
            lblTitRes.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblTitRes.AutoSize = true;
            lblTitRes.Anchor = AnchorStyles.Left;

            Label lblBadgePrec = clsTemaUI.CrearInsignia("IEEE-754 64b", clsTemaUI.ColExitoSuave, clsTemaUI.ColExitoTextoOscuro);
            lblBadgePrec.Font = clsTemaUI.FntMonoespacioPeque;
            lblBadgePrec.Anchor = AnchorStyles.Right;

            tblCabRes.Controls.Add(lblTitRes, 0, 0);
            tblCabRes.Controls.Add(lblBadgePrec, 1, 0);

            // Caja de metricas tabulares
            Panel pnlCajaMetricas = new Panel();
            pnlCajaMetricas.Dock = DockStyle.Top;
            pnlCajaMetricas.AutoSize = true;
            pnlCajaMetricas.BackColor = clsTemaUI.ColFondoApp;
            pnlCajaMetricas.Padding = new Padding(10, 8, 10, 8);
            pnlCajaMetricas.Margin = new Padding(0, 0, 0, 10);
            pnlCajaMetricas.Paint += (objRem, objArgs) =>
            {
                using (Pen penBorde = new Pen(clsTemaUI.ColBordeSutil, 1F))
                {
                    objArgs.Graphics.DrawRectangle(penBorde, 0, 0, pnlCajaMetricas.Width - 1, pnlCajaMetricas.Height - 1);
                }
            };

            TableLayoutPanel tblMetricas = new TableLayoutPanel();
            tblMetricas.Dock = DockStyle.Top;
            tblMetricas.AutoSize = true;
            tblMetricas.ColumnCount = 1;
            tblMetricas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblTiempoBernstein = new Label();
            lblTiempoBernstein.Text = "Tiempo Bernstein (10 rep):    0.0000 ms";
            lblTiempoBernstein.Font = clsTemaUI.FntMonoespacioNegrita;
            lblTiempoBernstein.ForeColor = clsTemaUI.ColPrimario;
            lblTiempoBernstein.AutoSize = true;
            lblTiempoBernstein.Margin = new Padding(0, 1, 0, 3);

            lblTiempoCasteljau = new Label();
            lblTiempoCasteljau.Text = "Tiempo De Casteljau (10 rep): 0.0000 ms";
            lblTiempoCasteljau.Font = clsTemaUI.FntMonoespacioNegrita;
            lblTiempoCasteljau.ForeColor = clsTemaUI.ColCasteljau;
            lblTiempoCasteljau.AutoSize = true;
            lblTiempoCasteljau.Margin = new Padding(0, 1, 0, 6);

            Panel pnlSepMet = clsTemaUI.CrearSeparadorHorizontal();
            pnlSepMet.Margin = new Padding(0, 0, 0, 6);

            lblErrorMaximo = new Label();
            lblErrorMaximo.Text = "Diferencia maxima:   0.000E+000";
            lblErrorMaximo.Font = clsTemaUI.FntMonoespacioPeque;
            lblErrorMaximo.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblErrorMaximo.AutoSize = true;
            lblErrorMaximo.Margin = new Padding(0, 1, 0, 2);

            lblErrorPromedio = new Label();
            lblErrorPromedio.Text = "Diferencia promedio: 0.000E+000";
            lblErrorPromedio.Font = clsTemaUI.FntMonoespacioPeque;
            lblErrorPromedio.ForeColor = clsTemaUI.ColTextoSecundario;
            lblErrorPromedio.AutoSize = true;
            lblErrorPromedio.Margin = new Padding(0, 1, 0, 2);

            lblErrorEcm = new Label();
            lblErrorEcm.Text = "ECM:                 0.000E+000";
            lblErrorEcm.Font = clsTemaUI.FntMonoespacioPeque;
            lblErrorEcm.ForeColor = clsTemaUI.ColTextoSecundario;
            lblErrorEcm.AutoSize = true;
            lblErrorEcm.Margin = new Padding(0, 1, 0, 1);

            tblMetricas.Controls.Add(lblTiempoBernstein, 0, 0);
            tblMetricas.Controls.Add(lblTiempoCasteljau, 0, 1);
            tblMetricas.Controls.Add(pnlSepMet, 0, 2);
            tblMetricas.Controls.Add(lblErrorMaximo, 0, 3);
            tblMetricas.Controls.Add(lblErrorPromedio, 0, 4);
            tblMetricas.Controls.Add(lblErrorEcm, 0, 5);
            pnlCajaMetricas.Controls.Add(tblMetricas);

            btnMedir = new Button();
            btnMedir.Text = "Medir curva actual (Stopwatch 10 rep)";
            btnMedir.Dock = DockStyle.Top;
            btnMedir.Height = 36;
            btnMedir.Margin = new Padding(0, 0, 0, 8);
            clsTemaUI.AplicarEstiloBotonPrimario(btnMedir);
            btnMedir.Click += BtnMedir_Click;
            tipAyuda.SetToolTip(btnMedir, "Mide 10 repeticiones con calentamiento de la curva dibujada");

            btnAbrirLaboratorio = new Button();
            btnAbrirLaboratorio.Text = "Abrir Laboratorio y Graficos Comparativos";
            btnAbrirLaboratorio.Dock = DockStyle.Top;
            btnAbrirLaboratorio.Height = 38;
            btnAbrirLaboratorio.Margin = new Padding(0);
            clsTemaUI.AplicarEstiloBotonExito(btnAbrirLaboratorio);
            btnAbrirLaboratorio.Click += BtnAbrirLaboratorio_Click;
            tipAyuda.SetToolTip(btnAbrirLaboratorio, "Abre la ventana frmPruebas con la tabla de experimentos y los graficos Tiempo vs Muestras/Grado");

            tblRes.Controls.Add(tblCabRes, 0, 0);
            tblRes.Controls.Add(pnlCajaMetricas, 0, 1);
            tblRes.Controls.Add(btnMedir, 0, 2);
            tblRes.Controls.Add(btnAbrirLaboratorio, 0, 3);
            pnlTarjetaResultados.Controls.Add(tblRes);

            tblInspector.Controls.Add(pnlTarjetaPuntos, 0, 0);
            tblInspector.Controls.Add(pnlTarjetaConfiguracion, 0, 1);
            tblInspector.Controls.Add(pnlTarjetaResultados, 0, 2);
            pnlLateral.Controls.Add(tblInspector);

            // 3. Area izquierda: lienzo enmarcado en tarjeta con barra de estado inferior
            pnlContenedorLienzo = new Panel();
            pnlContenedorLienzo.Dock = DockStyle.Fill;
            pnlContenedorLienzo.Padding = new Padding(clsTemaUI.EntEspacioMd);
            pnlContenedorLienzo.BackColor = clsTemaUI.ColFondoApp;

            pnlMarcoLienzo = new pnlTarjetaUI();
            pnlMarcoLienzo.Dock = DockStyle.Fill;
            pnlMarcoLienzo.Padding = new Padding(1);

            pnlBarraEstadoLienzo = new Panel();
            pnlBarraEstadoLienzo.Dock = DockStyle.Bottom;
            pnlBarraEstadoLienzo.Height = 32;
            pnlBarraEstadoLienzo.BackColor = clsTemaUI.ColSuperficieSutil;
            pnlBarraEstadoLienzo.Padding = new Padding(12, 0, 12, 0);

            Panel pnlLineaEstado = new Panel();
            pnlLineaEstado.Dock = DockStyle.Top;
            pnlLineaEstado.Height = 1;
            pnlLineaEstado.BackColor = clsTemaUI.ColBordeSutil;

            lblMensajeEstado = new Label();
            lblMensajeEstado.Text = "Listo para interactuar con el lienzo.";
            lblMensajeEstado.Dock = DockStyle.Left;
            lblMensajeEstado.AutoSize = true;
            lblMensajeEstado.TextAlign = ContentAlignment.MiddleLeft;
            lblMensajeEstado.Font = clsTemaUI.FntEtiquetaPequeNegrita;
            lblMensajeEstado.ForeColor = clsTemaUI.ColTextoPrincipal;
            lblMensajeEstado.Padding = new Padding(0, 8, 0, 0);

            lblGuiaInteraccion = new Label();
            lblGuiaInteraccion.Text = "Clic izq.: agregar/mover  |  Arrastrar: deformar  |  Clic der.: eliminar";
            lblGuiaInteraccion.Dock = DockStyle.Right;
            lblGuiaInteraccion.AutoSize = true;
            lblGuiaInteraccion.TextAlign = ContentAlignment.MiddleRight;
            lblGuiaInteraccion.Font = clsTemaUI.FntEtiquetaPeque;
            lblGuiaInteraccion.ForeColor = clsTemaUI.ColTextoSecundario;
            lblGuiaInteraccion.Padding = new Padding(0, 8, 0, 0);

            pnlBarraEstadoLienzo.Controls.Add(lblMensajeEstado);
            pnlBarraEstadoLienzo.Controls.Add(lblGuiaInteraccion);
            pnlBarraEstadoLienzo.Controls.Add(pnlLineaEstado);

            picLienzo = new ctlLienzo();
            picLienzo.Dock = DockStyle.Fill;
            picLienzo.BorderStyle = BorderStyle.None;
            picLienzo.MouseDown += PicLienzo_MouseDown;
            picLienzo.MouseMove += PicLienzo_MouseMove;
            picLienzo.MouseUp += PicLienzo_MouseUp;
            picLienzo.MouseLeave += PicLienzo_MouseLeave;

            pnlMarcoLienzo.Controls.Add(picLienzo);
            pnlMarcoLienzo.Controls.Add(pnlBarraEstadoLienzo);
            pnlContenedorLienzo.Controls.Add(pnlMarcoLienzo);

            this.Controls.Add(pnlContenedorLienzo);
            this.Controls.Add(pnlLateral);
            this.Controls.Add(pnlCabecera);
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

        // Arrastra el punto de control seleccionado y actualiza coordenadas visuales del cursor
        private void PicLienzo_MouseMove(object? objRemitente, MouseEventArgs objArgs)
        {
            lblCoordenadasCursor.Text = "Cursor: X " + objArgs.X.ToString("000") + "  Y " + objArgs.Y.ToString("000");

            if (entIndiceArrastrando >= 0 && objArgs.Button == MouseButtons.Left)
            {
                double dblX = Math.Max(10, Math.Min(picLienzo.Width - 10, objArgs.X));
                double dblY = Math.Max(10, Math.Min(picLienzo.Height - 10, objArgs.Y));
                objControlador.ModificarPunto(entIndiceArrastrando, dblX, dblY);
                RecalcularYRedibujar(false);
            }
        }

        // Restablece el indicador visual del cursor al salir del lienzo
        private void PicLienzo_MouseLeave(object? objRemitente, EventArgs objArgs)
        {
            lblCoordenadasCursor.Text = "Cursor: X —  Y —";
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
            lblParametroT.Text = "Parametro de interpolacion t = " + dblT.ToString("F2");
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
                    "P" + entI.ToString().PadRight(2) + " = (" +
                    lstPuntos[entI].DblX.ToString("F1").PadLeft(6) + ", " +
                    lstPuntos[entI].DblY.ToString("F1").PadLeft(6) + ")");
            }

            if (entCantidad < 2)
            {
                lblGradoActual.Text = "Sin curva (" + entCantidad + " pts)";
                lblInsigniaGradoCabecera.Text = "Sin curva (" + entCantidad + " pts)";
                lblMensajeEstado.Text = "Agregue al menos 2 puntos en el lienzo.";
                picLienzo.ActualizarGeometria(lstPuntos, new clsPuntoControl[0], new clsPuntoControl[0], new List<List<clsPuntoControl>>());
                return;
            }

            int entGrado = objControlador.ObjCurva.ObtenerGrado();
            int entMuestras = (int)numMuestras.Value;
            double dblT = trkParametroT.Value / 100.0;

            lblGradoActual.Text = "n = " + entGrado + " (" + entCantidad + " pts)";
            lblInsigniaGradoCabecera.Text = "Grado n = " + entGrado + " (" + entCantidad + " pts)";
            lblMensajeEstado.Text = "Curva evaluada con " + entMuestras + " muestras (t = " + dblT.ToString("F2") + ").";

            List<List<clsPuntoControl>> lstNiveles = objControlador.ObtenerInterpolaciones(dblT);

            if (bolMedirConStopwatch)
            {
                clsResultadoEvaluacion objResB;
                clsResultadoEvaluacion objResC;
                clsResultadoComparacion objComp;
                objControlador.MedirYComparar(entMuestras, out objResB, out objResC, out objComp);

                lblTiempoBernstein.Text = "Tiempo Bernstein (10 rep):    " + objResB.DblTiempoPromedioMs.ToString("F4") + " ms";
                lblTiempoCasteljau.Text = "Tiempo De Casteljau (10 rep): " + objResC.DblTiempoPromedioMs.ToString("F4") + " ms";
                lblErrorMaximo.Text = "Diferencia maxima:   " + objComp.DblDiferenciaMaxima.ToString("E3");
                lblErrorPromedio.Text = "Diferencia promedio: " + objComp.DblDiferenciaPromedio.ToString("E3");
                lblErrorEcm.Text = "ECM:                 " + objComp.DblErrorCuadraticoMedio.ToString("E3");

                picLienzo.ActualizarGeometria(lstPuntos, objResB.ArrPuntosCurva, objResC.ArrPuntosCurva, lstNiveles);
            }
            else
            {
                clsResultadoEvaluacion objResB = objControlador.EvaluarBernsteinRapido(entMuestras);
                clsResultadoEvaluacion objResC = objControlador.EvaluarDeCasteljauRapido(entMuestras);
                clsComparador objComp = new clsComparador();
                clsResultadoComparacion objDif = objComp.Comparar(objResB.ArrPuntosCurva, objResC.ArrPuntosCurva);

                lblErrorMaximo.Text = "Diferencia maxima:   " + objDif.DblDiferenciaMaxima.ToString("E3");
                lblErrorPromedio.Text = "Diferencia promedio: " + objDif.DblDiferenciaPromedio.ToString("E3");
                lblErrorEcm.Text = "ECM:                 " + objDif.DblErrorCuadraticoMedio.ToString("E3");

                picLienzo.ActualizarGeometria(lstPuntos, objResB.ArrPuntosCurva, objResC.ArrPuntosCurva, lstNiveles);
            }
        }
    }
}
