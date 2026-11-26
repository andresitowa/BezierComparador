using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BezierComparador
{
    // Lienzo interactivo con doble buffer que dibuja las curvas usando unicamente DrawLine
    public class ctlLienzo : Panel
    {
        private List<clsPuntoControl> lstPuntosControl;
        private clsPuntoControl[] arrPuntosBernstein;
        private clsPuntoControl[] arrPuntosCasteljau;
        private List<List<clsPuntoControl>> lstNivelesIntermedios;
        private bool bolMostrarBernstein;
        private bool bolMostrarCasteljau;
        private bool bolMostrarPoligonoControl;
        private bool bolMostrarInterpolaciones;
        private int entIndiceSeleccionado;

        public bool BolMostrarBernstein
        {
            get { return bolMostrarBernstein; }
            set { bolMostrarBernstein = value; Invalidate(); }
        }

        public bool BolMostrarCasteljau
        {
            get { return bolMostrarCasteljau; }
            set { bolMostrarCasteljau = value; Invalidate(); }
        }

        public bool BolMostrarPoligonoControl
        {
            get { return bolMostrarPoligonoControl; }
            set { bolMostrarPoligonoControl = value; Invalidate(); }
        }

        public bool BolMostrarInterpolaciones
        {
            get { return bolMostrarInterpolaciones; }
            set { bolMostrarInterpolaciones = value; Invalidate(); }
        }

        public int EntIndiceSeleccionado
        {
            get { return entIndiceSeleccionado; }
            set { entIndiceSeleccionado = value; Invalidate(); }
        }

        public ctlLienzo()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            lstPuntosControl = new List<clsPuntoControl>();
            arrPuntosBernstein = new clsPuntoControl[0];
            arrPuntosCasteljau = new clsPuntoControl[0];
            lstNivelesIntermedios = new List<List<clsPuntoControl>>();

            bolMostrarBernstein = true;
            bolMostrarCasteljau = true;
            bolMostrarPoligonoControl = true;
            bolMostrarInterpolaciones = true;
            entIndiceSeleccionado = -1;
        }

        // Recibe los puntos ya calculados por el Controlador/Modelo y solicita redibujar
        public void ActualizarGeometria(
            List<clsPuntoControl> lstPuntos,
            clsPuntoControl[] arrBernstein,
            clsPuntoControl[] arrCasteljau,
            List<List<clsPuntoControl>> lstNiveles)
        {
            lstPuntosControl = lstPuntos ?? new List<clsPuntoControl>();
            arrPuntosBernstein = arrBernstein ?? new clsPuntoControl[0];
            arrPuntosCasteljau = arrCasteljau ?? new clsPuntoControl[0];
            lstNivelesIntermedios = lstNiveles ?? new List<List<clsPuntoControl>>();
            this.Invalidate();
        }

        // Dibuja cuadricula, poligono, interpolaciones y curvas usando solo DrawLine, puntos y texto
        protected override void OnPaint(PaintEventArgs objArgs)
        {
            base.OnPaint(objArgs);
            Graphics objG = objArgs.Graphics;
            objG.SmoothingMode = SmoothingMode.AntiAlias;

            DibujarCuadricula(objG);

            if (lstPuntosControl.Count == 0)
            {
                using (Font fntAviso = new Font("Segoe UI", 10.5F, FontStyle.Regular))
                using (Brush brsAviso = new SolidBrush(Color.FromArgb(110, 110, 110)))
                {
                    string cadTexto = "Haga clic izquierdo para agregar puntos de control (minimo 2 puntos).\n" +
                                      "Arrastre un punto para moverlo o haga clic derecho para eliminarlo.";
                    objG.DrawString(cadTexto, fntAviso, brsAviso, 24, 24);
                }
                return;
            }

            // 1. Poligono de control en gris punteado con DrawLine
            if (bolMostrarPoligonoControl && lstPuntosControl.Count >= 2)
            {
                using (Pen penPoligono = new Pen(Color.FromArgb(160, 160, 160), 1.5F))
                {
                    penPoligono.DashStyle = DashStyle.Dash;
                    for (int entI = 0; entI < lstPuntosControl.Count - 1; entI++)
                    {
                        objG.DrawLine(
                            penPoligono,
                            (float)lstPuntosControl[entI].DblX,
                            (float)lstPuntosControl[entI].DblY,
                            (float)lstPuntosControl[entI + 1].DblX,
                            (float)lstPuntosControl[entI + 1].DblY);
                    }
                }
            }

            // 2. Niveles intermedios de De Casteljau en verde con DrawLine
            if (bolMostrarInterpolaciones && lstNivelesIntermedios.Count > 1)
            {
                using (Pen penVerde = new Pen(Color.FromArgb(30, 165, 75), 1.6F))
                using (Brush brsVerde = new SolidBrush(Color.FromArgb(30, 165, 75)))
                {
                    for (int entNivel = 1; entNivel < lstNivelesIntermedios.Count; entNivel++)
                    {
                        List<clsPuntoControl> lstNivel = lstNivelesIntermedios[entNivel];

                        for (int entI = 0; entI < lstNivel.Count - 1; entI++)
                        {
                            objG.DrawLine(
                                penVerde,
                                (float)lstNivel[entI].DblX,
                                (float)lstNivel[entI].DblY,
                                (float)lstNivel[entI + 1].DblX,
                                (float)lstNivel[entI + 1].DblY);
                        }

                        for (int entI = 0; entI < lstNivel.Count; entI++)
                        {
                            float fltX = (float)lstNivel[entI].DblX;
                            float fltY = (float)lstNivel[entI].DblY;
                            objG.FillEllipse(brsVerde, fltX - 3.5F, fltY - 3.5F, 7F, 7F);
                        }
                    }

                    // Resalta el punto final C(t) en el ultimo nivel
                    List<clsPuntoControl> lstUltimo = lstNivelesIntermedios[lstNivelesIntermedios.Count - 1];
                    if (lstUltimo.Count == 1)
                    {
                        float fltX = (float)lstUltimo[0].DblX;
                        float fltY = (float)lstUltimo[0].DblY;
                        objG.FillEllipse(brsVerde, fltX - 6F, fltY - 6F, 12F, 12F);
                        using (Pen penBorde = new Pen(Color.White, 1.5F))
                        {
                            objG.DrawEllipse(penBorde, fltX - 6F, fltY - 6F, 12F, 12F);
                        }
                    }
                }
            }

            // 3. Curva de Bernstein en azul uniendo muestras con DrawLine
            if (bolMostrarBernstein && arrPuntosBernstein.Length >= 2)
            {
                using (Pen penBernstein = new Pen(Color.FromArgb(0, 102, 204), 3.0F))
                {
                    for (int entK = 0; entK < arrPuntosBernstein.Length - 1; entK++)
                    {
                        objG.DrawLine(
                            penBernstein,
                            (float)arrPuntosBernstein[entK].DblX,
                            (float)arrPuntosBernstein[entK].DblY,
                            (float)arrPuntosBernstein[entK + 1].DblX,
                            (float)arrPuntosBernstein[entK + 1].DblY);
                    }
                }
            }

            // 4. Curva de De Casteljau en naranja uniendo muestras con DrawLine
            if (bolMostrarCasteljau && arrPuntosCasteljau.Length >= 2)
            {
                using (Pen penCasteljau = new Pen(Color.FromArgb(235, 115, 0), 2.0F))
                {
                    if (bolMostrarBernstein)
                    {
                        penCasteljau.DashStyle = DashStyle.Dash;
                    }

                    for (int entK = 0; entK < arrPuntosCasteljau.Length - 1; entK++)
                    {
                        objG.DrawLine(
                            penCasteljau,
                            (float)arrPuntosCasteljau[entK].DblX,
                            (float)arrPuntosCasteljau[entK].DblY,
                            (float)arrPuntosCasteljau[entK + 1].DblX,
                            (float)arrPuntosCasteljau[entK + 1].DblY);
                    }
                }
            }

            // 5. Puntos de control en rojo con su etiqueta P_i
            using (Brush brsRojo = new SolidBrush(Color.FromArgb(215, 35, 35)))
            using (Brush brsSeleccion = new SolidBrush(Color.FromArgb(255, 190, 0)))
            using (Pen penBlanco = new Pen(Color.White, 1.5F))
            using (Font fntPunto = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (Brush brsTexto = new SolidBrush(Color.FromArgb(30, 30, 30)))
            {
                for (int entI = 0; entI < lstPuntosControl.Count; entI++)
                {
                    float fltX = (float)lstPuntosControl[entI].DblX;
                    float fltY = (float)lstPuntosControl[entI].DblY;

                    if (entI == entIndiceSeleccionado)
                    {
                        objG.FillEllipse(brsSeleccion, fltX - 8F, fltY - 8F, 16F, 16F);
                    }

                    objG.FillEllipse(brsRojo, fltX - 5.5F, fltY - 5.5F, 11F, 11F);
                    objG.DrawEllipse(penBlanco, fltX - 5.5F, fltY - 5.5F, 11F, 11F);
                    objG.DrawString("P" + entI, fntPunto, brsTexto, fltX + 7F, fltY - 14F);
                }
            }
        }

        // Dibuja una cuadricula suave de fondo con DrawLine
        private void DibujarCuadricula(Graphics objG)
        {
            int entPaso = 40;
            using (Pen penCuadricula = new Pen(Color.FromArgb(242, 242, 242), 1F))
            {
                for (int entX = entPaso; entX < this.Width; entX += entPaso)
                {
                    objG.DrawLine(penCuadricula, entX, 0, entX, this.Height);
                }
                for (int entY = entPaso; entY < this.Height; entY += entPaso)
                {
                    objG.DrawLine(penCuadricula, 0, entY, this.Width, entY);
                }
            }
        }
    }
}
