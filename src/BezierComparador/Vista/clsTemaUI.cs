using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BezierComparador
{
    // Panel estilizado tipo tarjeta con borde sutil y doble buffer para una jerarquia visual limpia
    public class pnlTarjetaUI : Panel
    {
        private Color colBorde;
        private Color colAcentoSuperior;
        private bool bolMostrarAcentoSuperior;

        public Color ColBorde
        {
            get { return colBorde; }
            set { colBorde = value; Invalidate(); }
        }

        public Color ColAcentoSuperior
        {
            get { return colAcentoSuperior; }
            set { colAcentoSuperior = value; Invalidate(); }
        }

        public bool BolMostrarAcentoSuperior
        {
            get { return bolMostrarAcentoSuperior; }
            set { bolMostrarAcentoSuperior = value; Invalidate(); }
        }

        public pnlTarjetaUI()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.BackColor = clsTemaUI.ColSuperficieTarjeta;
            this.Padding = new Padding(clsTemaUI.EntEspacioMd);
            colBorde = clsTemaUI.ColBordeTarjeta;
            colAcentoSuperior = clsTemaUI.ColPrimario;
            bolMostrarAcentoSuperior = false;
        }

        protected override void OnPaint(PaintEventArgs objArgs)
        {
            base.OnPaint(objArgs);
            Graphics objG = objArgs.Graphics;

            using (Pen penBorde = new Pen(colBorde, 1F))
            {
                objG.DrawRectangle(penBorde, 0, 0, this.Width - 1, this.Height - 1);
            }

            if (bolMostrarAcentoSuperior && this.Width > 2)
            {
                using (Brush brsAcento = new SolidBrush(colAcentoSuperior))
                {
                    objG.FillRectangle(brsAcento, 1, 1, this.Width - 2, 3);
                }
            }
        }
    }

    // Sistema de diseno centralizado (tokens de color WCAG AA/AAA, tipografia, espaciado y estilos base)
    public static class clsTemaUI
    {
        // --- TOKENS DE COLOR: SUPERFICIES Y BORDES (Slate Light CAD) ---
        public static readonly Color ColFondoApp = Color.FromArgb(248, 250, 252);          // #F8FAFC
        public static readonly Color ColSuperficieTarjeta = Color.FromArgb(255, 255, 255); // #FFFFFF
        public static readonly Color ColSuperficieSutil = Color.FromArgb(241, 245, 249);   // #F1F5F9
        public static readonly Color ColCabeceraApp = Color.FromArgb(255, 255, 255);       // #FFFFFF
        public static readonly Color ColBordeTarjeta = Color.FromArgb(203, 213, 225);      // #CBD5E1
        public static readonly Color ColBordeSutil = Color.FromArgb(226, 232, 240);        // #E2E8F0

        // --- TOKENS DE COLOR: TEXTO (Contraste WCAG AA/AAA sobre blanco) ---
        public static readonly Color ColTextoPrincipal = Color.FromArgb(15, 23, 42);       // #0F172A (17.9:1)
        public static readonly Color ColTextoSecundario = Color.FromArgb(71, 85, 105);     // #475569 (7.58:1)
        public static readonly Color ColTextoTenue = Color.FromArgb(100, 116, 139);        // #64748B (4.76:1)

        // --- TOKENS DE COLOR: SEMANTICOS (Bernstein, De Casteljau, Interpolaciones, Control) ---
        public static readonly Color ColPrimario = Color.FromArgb(30, 64, 175);            // #1E40AF (Bernstein - 8.59:1)
        public static readonly Color ColPrimarioHover = Color.FromArgb(29, 78, 216);       // #1D4ED8
        public static readonly Color ColPrimarioActivo = Color.FromArgb(30, 58, 138);      // #1E3A8A
        public static readonly Color ColPrimarioSuave = Color.FromArgb(239, 246, 255);     // #EFF6FF
        public static readonly Color ColPrimarioBorde = Color.FromArgb(191, 219, 254);     // #BFDBFE

        public static readonly Color ColCasteljau = Color.FromArgb(194, 65, 12);           // #C2410C (Texto AA 5.18:1)
        public static readonly Color ColCasteljauTrazo = Color.FromArgb(234, 88, 12);      // #EA580C (Trazo curva/grafico)
        public static readonly Color ColCasteljauSuave = Color.FromArgb(255, 247, 237);    // #FFF7ED

        public static readonly Color ColExito = Color.FromArgb(4, 120, 87);                // #047857 (Emerald 700 - 5.48:1)
        public static readonly Color ColExitoHover = Color.FromArgb(6, 95, 70);            // #065F46
        public static readonly Color ColExitoActivo = Color.FromArgb(6, 78, 59);           // #064E3B
        public static readonly Color ColExitoSuave = Color.FromArgb(220, 252, 231);        // #DCFCE7 (Celda ganadora)
        public static readonly Color ColExitoTextoOscuro = Color.FromArgb(20, 83, 45);     // #14532D (8.77:1 sobre suave)
        public static readonly Color ColExitoBorde = Color.FromArgb(167, 243, 208);        // #A7F3D0

        public static readonly Color ColPeligro = Color.FromArgb(220, 38, 38);             // #DC2626 (Control points - 4.83:1)
        public static readonly Color ColPeligroHover = Color.FromArgb(185, 28, 28);        // #B91C1C
        public static readonly Color ColPeligroSuave = Color.FromArgb(254, 242, 242);      // #FEF2F2
        public static readonly Color ColPeligroBorde = Color.FromArgb(254, 202, 202);      // #FECACA

        public static readonly Color ColFoco = Color.FromArgb(37, 99, 235);                // #2563EB (Anillo de foco)
        public static readonly Color ColSeleccionPunto = Color.FromArgb(245, 158, 11);     // #F59E0B (Resalte arrastre)
        public static readonly Color ColHoverPunto = Color.FromArgb(59, 130, 246);         // #3B82F6 (Resalte hover)
        public static readonly Color ColPoligonoControl = Color.FromArgb(148, 163, 184);   // #94A3B8
        public static readonly Color ColCuadriculaMenor = Color.FromArgb(241, 245, 249);   // #F1F5F9
        public static readonly Color ColCuadriculaMayor = Color.FromArgb(226, 232, 240);   // #E2E8F0

        // --- TOKENS DE ESPACIADO (Escala 4px) ---
        public const int EntEspacioXs = 4;
        public const int EntEspacioSm = 8;
        public const int EntEspacioMd = 12;
        public const int EntEspacioLg = 16;
        public const int EntEspacioXl = 24;
        public const int EntAlturaBoton = 34;

        // --- TOKENS DE TIPOGRAFIA ---
        public static readonly Font FntTituloVentana = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font FntSubtituloVentana = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        public static readonly Font FntTituloTarjeta = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font FntCuerpo = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FntCuerpoNegrita = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font FntEtiquetaPeque = new Font("Segoe UI", 8.25F, FontStyle.Regular);
        public static readonly Font FntEtiquetaPequeNegrita = new Font("Segoe UI", 8.25F, FontStyle.Bold);
        public static readonly Font FntMonoespacio = new Font("Consolas", 9F, FontStyle.Regular);
        public static readonly Font FntMonoespacioNegrita = new Font("Consolas", 9.25F, FontStyle.Bold);
        public static readonly Font FntMonoespacioPeque = new Font("Consolas", 8.25F, FontStyle.Regular);

        // Aplica configuracion visual base y escalado DPI a cualquier formulario
        public static void AplicarEstiloFormulario(Form frmVentana)
        {
            frmVentana.BackColor = ColFondoApp;
            frmVentana.ForeColor = ColTextoPrincipal;
            frmVentana.Font = FntCuerpo;
            frmVentana.AutoScaleMode = AutoScaleMode.Dpi;
        }

        // Configura estilo de boton primario (azul cobalto con alto contraste)
        public static void AplicarEstiloBotonPrimario(Button btnControl)
        {
            AplicarBaseBoton(btnControl, ColPrimario, Color.White, ColPrimario, ColPrimarioHover, ColPrimarioActivo);
            btnControl.Font = FntCuerpoNegrita;
        }

        // Configura estilo de boton de exito / accion analitica (verde esmeralda)
        public static void AplicarEstiloBotonExito(Button btnControl)
        {
            AplicarBaseBoton(btnControl, ColExito, Color.White, ColExito, ColExitoHover, ColExitoActivo);
            btnControl.Font = FntCuerpoNegrita;
        }

        // Configura estilo de boton secundario con borde limpio
        public static void AplicarEstiloBotonSecundario(Button btnControl)
        {
            AplicarBaseBoton(btnControl, ColSuperficieTarjeta, ColTextoPrincipal, ColBordeTarjeta, ColSuperficieSutil, ColBordeSutil);
            btnControl.Font = FntCuerpoNegrita;
        }

        // Configura estilo de boton de accion destructiva controlada (limpiar)
        public static void AplicarEstiloBotonPeligroSuave(Button btnControl)
        {
            AplicarBaseBoton(btnControl, ColPeligroSuave, ColPeligroHover, ColPeligroBorde, Color.FromArgb(254, 226, 226), ColPeligroBorde);
            btnControl.Font = FntCuerpoNegrita;
        }

        // Aplica estilo plano consistente, estados hover/down e indicador de foco visible por teclado
        private static void AplicarBaseBoton(
            Button btnControl,
            Color colFondo,
            Color colTexto,
            Color colBorde,
            Color colHover,
            Color colPresionado)
        {
            btnControl.FlatStyle = FlatStyle.Flat;
            btnControl.BackColor = colFondo;
            btnControl.ForeColor = colTexto;
            btnControl.Cursor = Cursors.Hand;
            btnControl.UseVisualStyleBackColor = false;
            btnControl.FlatAppearance.BorderSize = 1;
            btnControl.FlatAppearance.BorderColor = colBorde;
            btnControl.FlatAppearance.MouseOverBackColor = colHover;
            btnControl.FlatAppearance.MouseDownBackColor = colPresionado;
            btnControl.Height = Math.Max(btnControl.Height, EntAlturaBoton);

            Color colBordeOriginal = colBorde;
            Color colFondoOriginal = colFondo;
            Color colTextoOriginal = colTexto;

            btnControl.Enter += (objRem, objArgs) =>
            {
                btnControl.FlatAppearance.BorderSize = 2;
                btnControl.FlatAppearance.BorderColor = ColFoco;
            };
            btnControl.Leave += (objRem, objArgs) =>
            {
                btnControl.FlatAppearance.BorderSize = 1;
                btnControl.FlatAppearance.BorderColor = colBordeOriginal;
            };
            btnControl.EnabledChanged += (objRem, objArgs) =>
            {
                if (btnControl.Enabled)
                {
                    btnControl.BackColor = colFondoOriginal;
                    btnControl.ForeColor = colTextoOriginal;
                    btnControl.FlatAppearance.BorderColor = colBordeOriginal;
                    btnControl.Cursor = Cursors.Hand;
                }
                else
                {
                    btnControl.BackColor = ColBordeSutil;
                    btnControl.ForeColor = ColTextoSecundario;
                    btnControl.FlatAppearance.BorderColor = ColBordeTarjeta;
                    btnControl.Cursor = Cursors.Default;
                }
            };
        }

        // Aplica diseno moderno, legible y con numeros tabulares a un DataGridView
        public static void AplicarEstiloDataGridView(DataGridView dgvTabla)
        {
            dgvTabla.BackgroundColor = ColSuperficieTarjeta;
            dgvTabla.BorderStyle = BorderStyle.None;
            dgvTabla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTabla.GridColor = ColBordeSutil;
            dgvTabla.EnableHeadersVisualStyles = false;
            dgvTabla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTabla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTabla.ColumnHeadersHeight = 36;
            dgvTabla.RowTemplate.Height = 31;
            dgvTabla.AlternatingRowsDefaultCellStyle.BackColor = ColFondoApp;

            dgvTabla.ColumnHeadersDefaultCellStyle.BackColor = ColSuperficieSutil;
            dgvTabla.ColumnHeadersDefaultCellStyle.ForeColor = ColTextoPrincipal;
            dgvTabla.ColumnHeadersDefaultCellStyle.Font = FntEtiquetaPequeNegrita;
            dgvTabla.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColSuperficieSutil;
            dgvTabla.ColumnHeadersDefaultCellStyle.SelectionForeColor = ColTextoPrincipal;
            dgvTabla.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 6, 4);

            dgvTabla.DefaultCellStyle.BackColor = ColSuperficieTarjeta;
            dgvTabla.DefaultCellStyle.ForeColor = ColTextoPrincipal;
            dgvTabla.DefaultCellStyle.Font = FntMonoespacio;
            dgvTabla.DefaultCellStyle.SelectionBackColor = ColPrimarioSuave;
            dgvTabla.DefaultCellStyle.SelectionForeColor = ColTextoPrincipal;
            dgvTabla.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);
        }

        // Crea una etiqueta de insignia (pill) de alto contraste
        public static Label CrearInsignia(string cadTexto, Color colFondo, Color colTexto)
        {
            Label lblInsignia = new Label();
            lblInsignia.Text = cadTexto;
            lblInsignia.AutoSize = true;
            lblInsignia.BackColor = colFondo;
            lblInsignia.ForeColor = colTexto;
            lblInsignia.Font = FntEtiquetaPequeNegrita;
            lblInsignia.Padding = new Padding(8, 3, 8, 3);
            lblInsignia.Margin = new Padding(0);
            return lblInsignia;
        }

        // Crea una linea divisoria horizontal sutil de 1px
        public static Panel CrearSeparadorHorizontal()
        {
            Panel pnlLinea = new Panel();
            pnlLinea.Height = 1;
            pnlLinea.Dock = DockStyle.Top;
            pnlLinea.BackColor = ColBordeSutil;
            return pnlLinea;
        }

        // Configura un ToolTip con tiempos de respuesta suaves y consistentes
        public static void ConfigurarToolTip(ToolTip tipAyuda)
        {
            tipAyuda.AutoPopDelay = 6000;
            tipAyuda.InitialDelay = 350;
            tipAyuda.ReshowDelay = 150;
            tipAyuda.ShowAlways = true;
        }
    }
}
