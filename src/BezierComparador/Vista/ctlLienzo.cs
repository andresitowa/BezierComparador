using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
        private int entIndiceHover;
        private double dblParametroTActual;

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

        public int EntIndiceHover
        {
            get { return entIndiceHover; }
            set
            {
                if (entIndiceHover != value)
                {
                    entIndiceHover = value;
                    Invalidate();
                }
            }
        }

        public double DblParametroTActual
        {
            get { return dblParametroTActual; }
            set
            {
                dblParametroTActual = value;
                Invalidate();
            }
        }

        public ctlLienzo()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.BackColor = clsTemaUI.ColSuperficieTarjeta;
            this.BorderStyle = BorderStyle.None;
            this.Cursor = Cursors.Cross;

            lstPuntosControl = new List<clsPuntoControl>();
            arrPuntosBernstein = new clsPuntoControl[0];
            arrPuntosCasteljau = new clsPuntoControl[0];
            lstNivelesIntermedios = new List<List<clsPuntoControl>>();

            bolMostrarBernstein = true;
            bolMostrarCasteljau = true;
            bolMostrarPoligonoControl = true;
            bolMostrarInterpolaciones = true;
            entIndiceSeleccionado = -1;
            entIndiceHover = -1;
            dblParametroTActual = 0.50;
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
            objG.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DibujarCuadriculaCAD(objG);

            if (lstPuntosControl.Count == 0)
            {
                DibujarEstadoVacio(objG);
                return;
            }

            // 1. Poligono de control en gris pizarra punteado con DrawLine
            if (bolMostrarPoligonoControl && lstPuntosControl.Count >= 2)
            {
                using (Pen penPoligono = new Pen(clsTemaUI.ColPoligonoControl, 1.5F))
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

            // 2. Niveles intermedios de De Casteljau en esmeralda con DrawLine
            if (bolMostrarInterpolaciones && lstNivelesIntermedios.Count > 1)
            {
                using (Pen penVerde = new Pen(clsTemaUI.ColExito, 1.6F))
                using (Brush brsVerde = new SolidBrush(clsTemaUI.ColExito))
                using (Brush brsHaloVerde = new SolidBrush(Color.FromArgb(45, clsTemaUI.ColExito)))
                using (Pen penBlanco = new Pen(Color.White, 1.5F))
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
                            objG.DrawEllipse(penBlanco, fltX - 3.5F, fltY - 3.5F, 7F, 7F);
                        }
                    }

                    // Resalta el punto final C(t) en el ultimo nivel con halo y etiqueta
                    List<clsPuntoControl> lstUltimo = lstNivelesIntermedios[lstNivelesIntermedios.Count - 1];
                    if (lstUltimo.Count == 1)
                    {
                        float fltX = (float)lstUltimo[0].DblX;
                        float fltY = (float)lstUltimo[0].DblY;
                        objG.FillEllipse(brsHaloVerde, fltX - 11F, fltY - 11F, 22F, 22F);
                        objG.FillEllipse(brsVerde, fltX - 5.5F, fltY - 5.5F, 11F, 11F);
                        objG.DrawEllipse(penBlanco, fltX - 5.5F, fltY - 5.5F, 11F, 11F);

                        string cadEtiquetaC = "C(" + dblParametroTActual.ToString("F2") + ")";
                        DibujarEtiquetaPastilla(objG, cadEtiquetaC, fltX + 9F, fltY - 18F, clsTemaUI.ColExitoSuave, clsTemaUI.ColExitoBorde, clsTemaUI.ColExitoTextoOscuro);
                    }
                }
            }

            // 3. Curva de Bernstein en azul cobalto uniendo muestras con DrawLine
            if (bolMostrarBernstein && arrPuntosBernstein.Length >= 2)
            {
                using (Pen penBernstein = new Pen(clsTemaUI.ColPrimario, 3.0F))
                {
                    penBernstein.StartCap = LineCap.Round;
                    penBernstein.EndCap = LineCap.Round;
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

            // 4. Curva de De Casteljau en naranja quemado uniendo muestras con DrawLine
            if (bolMostrarCasteljau && arrPuntosCasteljau.Length >= 2)
            {
                using (Pen penCasteljau = new Pen(clsTemaUI.ColCasteljauTrazo, 2.2F))
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

            // 5. Puntos de control en carmesi con halo de seleccion/hover y etiqueta pastilla P_i
            using (Brush brsRojo = new SolidBrush(clsTemaUI.ColPeligro))
            using (Brush brsHaloSeleccion = new SolidBrush(Color.FromArgb(70, clsTemaUI.ColSeleccionPunto)))
            using (Pen penSeleccion = new Pen(clsTemaUI.ColSeleccionPunto, 2F))
            using (Brush brsHaloHover = new SolidBrush(Color.FromArgb(50, clsTemaUI.ColHoverPunto)))
            using (Pen penBlanco = new Pen(Color.White, 2F))
            {
                for (int entI = 0; entI < lstPuntosControl.Count; entI++)
                {
                    float fltX = (float)lstPuntosControl[entI].DblX;
                    float fltY = (float)lstPuntosControl[entI].DblY;

                    if (entI == entIndiceSeleccionado)
                    {
                        objG.FillEllipse(brsHaloSeleccion, fltX - 13F, fltY - 13F, 26F, 26F);
                        objG.DrawEllipse(penSeleccion, fltX - 9F, fltY - 9F, 18F, 18F);
                    }
                    else if (entI == entIndiceHover)
                    {
                        objG.FillEllipse(brsHaloHover, fltX - 12F, fltY - 12F, 24F, 24F);
                    }

                    objG.FillEllipse(brsRojo, fltX - 6F, fltY - 6F, 12F, 12F);
                    objG.DrawEllipse(penBlanco, fltX - 6F, fltY - 6F, 12F, 12F);

                    string cadTag = (entI == entIndiceSeleccionado)
                        ? "P" + entI + " (" + fltX.ToString("F0") + ", " + fltY.ToString("F0") + ")"
                        : "P" + entI;

                    DibujarEtiquetaPastilla(
                        objG,
                        cadTag,
                        fltX + 9F,
                        fltY - 18F,
                        Color.FromArgb(240, 255, 255, 255),
                        entI == entIndiceSeleccionado ? clsTemaUI.ColSeleccionPunto : clsTemaUI.ColBordeTarjeta,
                        clsTemaUI.ColTextoPrincipal);
                }
            }

            // 6. Leyenda flotante compacta en la esquina superior derecha
            DibujarLeyendaFlotante(objG);
        }

        // Dibuja cuadricula CAD con lineas menores (25px), mayores (100px) y marcas de ejes
        private void DibujarCuadriculaCAD(Graphics objG)
        {
            int entPasoMenor = 25;
            int entPasoMayor = 100;

            using (Pen penMenor = new Pen(clsTemaUI.ColCuadriculaMenor, 1F))
            using (Pen penMayor = new Pen(clsTemaUI.ColCuadriculaMayor, 1F))
            using (Brush brsRegla = new SolidBrush(clsTemaUI.ColTextoTenue))
            {
                for (int entX = entPasoMenor; entX < this.Width; entX += entPasoMenor)
                {
                    bool bolMayor = (entX % entPasoMayor == 0);
                    objG.DrawLine(bolMayor ? penMayor : penMenor, entX, 0, entX, this.Height);
                    if (bolMayor)
                    {
                        objG.DrawString(entX.ToString(), clsTemaUI.FntMonoespacioPeque, brsRegla, entX + 3, 4);
                    }
                }

                for (int entY = entPasoMenor; entY < this.Height; entY += entPasoMenor)
                {
                    bool bolMayor = (entY % entPasoMayor == 0);
                    objG.DrawLine(bolMayor ? penMayor : penMenor, 0, entY, this.Width, entY);
                    if (bolMayor)
                    {
                        objG.DrawString(entY.ToString(), clsTemaUI.FntMonoespacioPeque, brsRegla, 4, entY + 2);
                    }
                }
            }
        }

        // Dibuja una tarjeta de estado vacio centrada cuando no hay puntos de control
        private void DibujarEstadoVacio(Graphics objG)
        {
            int entAnchoCaja = 430;
            int entAltoCaja = 92;
            int entX = (this.Width - entAnchoCaja) / 2;
            int entY = (this.Height - entAltoCaja) / 2;

            if (entX < 16) entX = 16;
            if (entY < 16) entY = 16;

            Rectangle objCaja = new Rectangle(entX, entY, entAnchoCaja, entAltoCaja);

            using (Brush brsFondo = new SolidBrush(Color.FromArgb(248, 255, 255, 255)))
            using (Pen penBorde = new Pen(clsTemaUI.ColBordeTarjeta, 1F))
            using (Brush brsTit = new SolidBrush(clsTemaUI.ColTextoPrincipal))
            using (Brush brsSub = new SolidBrush(clsTemaUI.ColTextoSecundario))
            {
                objG.FillRectangle(brsFondo, objCaja);
                objG.DrawRectangle(penBorde, objCaja);

                objG.DrawString("Lienzo vacio — Agrega puntos de control", clsTemaUI.FntTituloTarjeta, brsTit, entX + 20, entY + 16);
                objG.DrawString(
                    "Haz clic izquierdo para crear puntos (minimo 2), arrastra para\ndeformar la curva o haz clic derecho para eliminar un punto.",
                    clsTemaUI.FntEtiquetaPeque,
                    brsSub,
                    entX + 20,
                    entY + 42);
            }
        }

        // Dibuja una etiqueta tipo pastilla legible sobre el lienzo
        private void DibujarEtiquetaPastilla(
            Graphics objG,
            string cadTexto,
            float fltX,
            float fltY,
            Color colFondo,
            Color colBorde,
            Color colTexto)
        {
            SizeF objMedida = objG.MeasureString(cadTexto, clsTemaUI.FntMonoespacioPeque);
            RectangleF objRect = new RectangleF(fltX, fltY, objMedida.Width + 6F, objMedida.Height + 2F);

            if (objRect.Right > this.Width - 6)
            {
                objRect.X = fltX - objRect.Width - 18F;
            }
            if (objRect.Top < 6)
            {
                objRect.Y = fltY + 24F;
            }

            using (Brush brsFondo = new SolidBrush(colFondo))
            using (Pen penBorde = new Pen(colBorde, 1F))
            using (Brush brsTxt = new SolidBrush(colTexto))
            {
                objG.FillRectangle(brsFondo, objRect);
                objG.DrawRectangle(penBorde, objRect.X, objRect.Y, objRect.Width, objRect.Height);
                objG.DrawString(cadTexto, clsTemaUI.FntMonoespacioPeque, brsTxt, objRect.X + 3F, objRect.Y + 1F);
            }
        }

        // Dibuja la leyenda compacta de capas activas en la esquina superior derecha del lienzo
        private void DibujarLeyendaFlotante(Graphics objG)
        {
            if (this.Width < 360 || this.Height < 220)
            {
                return;
            }

            int entAncho = 228;
            int entAlto = 98;
            int entX = this.Width - entAncho - 14;
            int entY = 14;
            Rectangle objCaja = new Rectangle(entX, entY, entAncho, entAlto);

            using (Brush brsFondo = new SolidBrush(Color.FromArgb(242, 255, 255, 255)))
            using (Pen penBorde = new Pen(clsTemaUI.ColBordeTarjeta, 1F))
            using (Brush brsTit = new SolidBrush(clsTemaUI.ColTextoSecundario))
            using (Brush brsTxt = new SolidBrush(clsTemaUI.ColTextoPrincipal))
            using (Pen penBern = new Pen(clsTemaUI.ColPrimario, 2.5F))
            using (Pen penCast = new Pen(clsTemaUI.ColCasteljauTrazo, 2.2F))
            using (Pen penInterp = new Pen(clsTemaUI.ColExito, 1.8F))
            using (Brush brsCtrl = new SolidBrush(clsTemaUI.ColPeligro))
            {
                penCast.DashStyle = DashStyle.Dash;

                objG.FillRectangle(brsFondo, objCaja);
                objG.DrawRectangle(penBorde, objCaja);

                objG.DrawString("CAPAS DEL TRAZADO (DrawLine)", clsTemaUI.FntEtiquetaPequeNegrita, brsTit, entX + 10, entY + 7);

                int entFilaY = entY + 28;
                objG.DrawLine(penBern, entX + 10, entFilaY + 6, entX + 28, entFilaY + 6);
                objG.DrawString("Bernstein (linea continua)", clsTemaUI.FntEtiquetaPeque, brsTxt, entX + 34, entFilaY);

                entFilaY += 16;
                objG.DrawLine(penCast, entX + 10, entFilaY + 6, entX + 28, entFilaY + 6);
                objG.DrawString("De Casteljau (discontinua)", clsTemaUI.FntEtiquetaPeque, brsTxt, entX + 34, entFilaY);

                entFilaY += 16;
                objG.DrawLine(penInterp, entX + 10, entFilaY + 6, entX + 28, entFilaY + 6);
                objG.DrawString("Subdivision t = " + dblParametroTActual.ToString("F2"), clsTemaUI.FntEtiquetaPeque, brsTxt, entX + 34, entFilaY);

                entFilaY += 16;
                objG.FillEllipse(brsCtrl, entX + 15, entFilaY + 2, 8, 8);
                objG.DrawString("Puntos de control P_i", clsTemaUI.FntEtiquetaPeque, brsTxt, entX + 34, entFilaY);
            }
        }
    }
}
