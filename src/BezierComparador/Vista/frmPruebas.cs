using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BezierComparador
{
    // Ventana de experimentacion y tabla comparativa de tiempos y errores
    public class frmPruebas : Form
    {
        private clsControladorPruebas objControladorPruebas;
        private Panel pnlSuperior;
        private GroupBox grpConfiguracion;
        private Label lblRepeticiones;
        private NumericUpDown numRepeticiones;
        private Button btnEjecutar;
        private ProgressBar prgProgreso;
        private Label lblEstado;
        private GroupBox grpResultados;
        private DataGridView dgvResultados;
        private ToolTip tipAyuda;
        private bool bolEjecutando;

        public frmPruebas()
        {
            objControladorPruebas = new clsControladorPruebas();
            bolEjecutando = false;
            InicializarComponentes();
        }

        // Configura los controles visuales siguiendo el diseno UI/UX del proyecto
        private void InicializarComponentes()
        {
            this.Text = "Laboratorio de Pruebas: Bernstein vs De Casteljau";
            this.Size = new Size(1050, 680);
            this.MinimumSize = new Size(880, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.DoubleBuffered = true;

            tipAyuda = new ToolTip();

            pnlSuperior = new Panel();
            pnlSuperior.Dock = DockStyle.Top;
            pnlSuperior.Height = 95;
            pnlSuperior.Padding = new Padding(12, 10, 12, 6);
            pnlSuperior.BackColor = Color.White;

            grpConfiguracion = new GroupBox();
            grpConfiguracion.Text = "Configuracion de experimentos (Grados: 2, 3, 5, 10, 20, 50 | Muestras: 100 a 100000)";
            grpConfiguracion.Dock = DockStyle.Fill;

            lblRepeticiones = new Label();
            lblRepeticiones.Text = "Repeticiones (min 10):";
            lblRepeticiones.Location = new Point(16, 32);
            lblRepeticiones.AutoSize = true;

            numRepeticiones = new NumericUpDown();
            numRepeticiones.Minimum = 10;
            numRepeticiones.Maximum = 100;
            numRepeticiones.Value = 10;
            numRepeticiones.Location = new Point(155, 29);
            numRepeticiones.Width = 65;
            tipAyuda.SetToolTip(numRepeticiones, "Cantidad de repeticiones por prueba (con calentamiento previo)");

            btnEjecutar = new Button();
            btnEjecutar.Text = "Ejecutar pruebas";
            btnEjecutar.Location = new Point(238, 26);
            btnEjecutar.Size = new Size(145, 32);
            btnEjecutar.BackColor = Color.FromArgb(0, 102, 204);
            btnEjecutar.ForeColor = Color.White;
            btnEjecutar.FlatStyle = FlatStyle.Flat;
            btnEjecutar.Cursor = Cursors.Hand;
            btnEjecutar.Click += BtnEjecutar_Click;
            tipAyuda.SetToolTip(btnEjecutar, "Ejecuta la bateria de experimentos sin congelar la ventana");

            prgProgreso = new ProgressBar();
            prgProgreso.Location = new Point(400, 30);
            prgProgreso.Size = new Size(230, 24);
            prgProgreso.Minimum = 0;
            prgProgreso.Maximum = 100;
            prgProgreso.Value = 0;

            lblEstado = new Label();
            lblEstado.Text = "Estado: Listo. Presione Ejecutar pruebas para iniciar.";
            lblEstado.Location = new Point(645, 33);
            lblEstado.AutoSize = true;
            lblEstado.ForeColor = Color.FromArgb(50, 50, 50);

            grpConfiguracion.Controls.Add(lblRepeticiones);
            grpConfiguracion.Controls.Add(numRepeticiones);
            grpConfiguracion.Controls.Add(btnEjecutar);
            grpConfiguracion.Controls.Add(prgProgreso);
            grpConfiguracion.Controls.Add(lblEstado);
            pnlSuperior.Controls.Add(grpConfiguracion);

            grpResultados = new GroupBox();
            grpResultados.Text = "Resultados comparativos (en verde el metodo mas rapido por fila)";
            grpResultados.Dock = DockStyle.Fill;
            grpResultados.Padding = new Padding(12);

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

            this.Controls.Add(grpResultados);
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

        // Ejecuta los experimentos en segundo plano con async/await para no bloquear la UI
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

            PoblarTablaResultados(lstFilas);

            prgProgreso.Value = 100;
            lblEstado.Text = "Estado: Pruebas completadas (" + lstFilas.Count + " combinaciones evaluadas).";
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
    }
}
