using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BezierComparador
{
    // Validacion matematica manual con una curva cubica fija
    // P0=(0,0) P1=(1,3) P2=(4,3) P3=(5,0), t = 0, 0.25, 0.5, 0.75, 1
    // Muestra paso a paso Bernstein y De Casteljau usando las clases del Modelo
    public class clsPruebaManual
    {
        private List<clsPuntoControl> lstPuntos;
        private double[] arrValoresT;
        private clsEvaluadorBernstein objBernstein;
        private clsEvaluadorDeCasteljau objDeCasteljau;
        private CultureInfo objCultura;

        // Prepara la curva cubica y los valores de t de la prueba
        public clsPruebaManual()
        {
            lstPuntos = new List<clsPuntoControl>();
            lstPuntos.Add(new clsPuntoControl(0, 0));
            lstPuntos.Add(new clsPuntoControl(1, 3));
            lstPuntos.Add(new clsPuntoControl(4, 3));
            lstPuntos.Add(new clsPuntoControl(5, 0));

            arrValoresT = new double[] { 0.0, 0.25, 0.5, 0.75, 1.0 };
            objBernstein = new clsEvaluadorBernstein();
            objDeCasteljau = new clsEvaluadorDeCasteljau();
            objCultura = CultureInfo.InvariantCulture;
        }

        // Arma el reporte completo de la validacion manual
        public string GenerarReporte()
        {
            StringBuilder objTexto = new StringBuilder();
            int entN = lstPuntos.Count - 1;

            objTexto.AppendLine("VALIDACION MATEMATICA MANUAL - CURVA CUBICA (n = " + entN + ")");
            objTexto.AppendLine("Puntos de control:");
            for (int entI = 0; entI <= entN; entI++)
            {
                objTexto.AppendLine("  P" + entI + " = " + Punto(lstPuntos[entI]));
            }
            objTexto.AppendLine();

            // Coeficientes binomiales C(3,i), calculados una sola vez
            double[] arrCoeficientes = objBernstein.CalcularCoeficientesBinomiales(entN);
            objTexto.Append("Coeficientes binomiales C(" + entN + ",i): ");
            for (int entI = 0; entI <= entN; entI++)
            {
                objTexto.Append(Numero(arrCoeficientes[entI], 0) + " ");
            }
            objTexto.AppendLine();
            objTexto.AppendLine();

            // Resultado de cada metodo para la tabla resumen
            clsPuntoControl[] arrResumenBernstein = new clsPuntoControl[arrValoresT.Length];
            clsPuntoControl[] arrResumenCasteljau = new clsPuntoControl[arrValoresT.Length];

            for (int entJ = 0; entJ < arrValoresT.Length; entJ++)
            {
                double dblT = arrValoresT[entJ];
                objTexto.AppendLine("==================================================");
                objTexto.AppendLine("t = " + Numero(dblT, 2) + "   (1 - t = " + Numero(1.0 - dblT, 2) + ")");
                objTexto.AppendLine("==================================================");

                arrResumenBernstein[entJ] = EscribirBernstein(objTexto, arrCoeficientes, dblT);
                arrResumenCasteljau[entJ] = EscribirDeCasteljau(objTexto, dblT);

                // Diferencia entre ambos metodos en este t
                double dblDifX = arrResumenBernstein[entJ].dblX - arrResumenCasteljau[entJ].dblX;
                double dblDifY = arrResumenBernstein[entJ].dblY - arrResumenCasteljau[entJ].dblY;
                double dblDistancia = Math.Sqrt(dblDifX * dblDifX + dblDifY * dblDifY);
                objTexto.AppendLine("Diferencia |Bernstein - De Casteljau| = " + dblDistancia.ToString("E3", objCultura));
                objTexto.AppendLine();
            }

            EscribirResumen(objTexto, arrResumenBernstein, arrResumenCasteljau);
            return objTexto.ToString();
        }

        // Formula: B(t) = suma C(n,i) * (1-t)^(n-i) * t^i * P_i
        private clsPuntoControl EscribirBernstein(StringBuilder objTexto, double[] arrCoeficientes, double dblT)
        {
            int entN = lstPuntos.Count - 1;
            double dblSumaX = 0.0;
            double dblSumaY = 0.0;

            objTexto.AppendLine("[Bernstein]  B(t) = suma C(n,i) * (1-t)^(n-i) * t^i * P_i");
            for (int entI = 0; entI <= entN; entI++)
            {
                double dblBase = objBernstein.CalcularBase(arrCoeficientes[entI], entN, entI, dblT);
                double dblAporteX = dblBase * lstPuntos[entI].dblX;
                double dblAporteY = dblBase * lstPuntos[entI].dblY;
                dblSumaX += dblAporteX;
                dblSumaY += dblAporteY;

                objTexto.AppendLine("  i=" + entI + ": B" + entI + "," + entN + " = " + Numero(arrCoeficientes[entI], 0)
                    + " * " + Numero(1.0 - dblT, 2) + "^" + (entN - entI)
                    + " * " + Numero(dblT, 2) + "^" + entI
                    + " = " + Numero(dblBase, 6)
                    + "  ->  B * P" + entI + " = (" + Numero(dblAporteX, 6) + ", " + Numero(dblAporteY, 6) + ")");
            }

            clsPuntoControl ptoResultado = new clsPuntoControl(dblSumaX, dblSumaY);
            objTexto.AppendLine("  Suma: B(" + Numero(dblT, 2) + ") = " + Punto(ptoResultado));
            return ptoResultado;
        }

        // Formula: P_i^(r) = (1-t) * P_i^(r-1) + t * P_(i+1)^(r-1)
        private clsPuntoControl EscribirDeCasteljau(StringBuilder objTexto, double dblT)
        {
            List<List<clsPuntoControl>> lstNiveles = objDeCasteljau.ObtenerNivelesIntermedios(lstPuntos, dblT);

            objTexto.AppendLine("[De Casteljau]  P_i^(r) = (1-t) * P_i^(r-1) + t * P_(i+1)^(r-1)");
            for (int entR = 0; entR < lstNiveles.Count; entR++)
            {
                objTexto.Append("  r=" + entR + ": ");
                for (int entI = 0; entI < lstNiveles[entR].Count; entI++)
                {
                    objTexto.Append("P" + entI + "^(" + entR + ")=" + Punto(lstNiveles[entR][entI]) + "  ");
                }
                objTexto.AppendLine();
            }

            // El ultimo nivel tiene un solo punto: el punto de la curva
            clsPuntoControl ptoResultado = lstNiveles[lstNiveles.Count - 1][0];
            objTexto.AppendLine("  Resultado: C(" + Numero(dblT, 2) + ") = " + Punto(ptoResultado));
            return ptoResultado;
        }

        // Tabla final y verificacion contra el metodo Evaluar de cada clase
        private void EscribirResumen(StringBuilder objTexto, clsPuntoControl[] arrBernstein, clsPuntoControl[] arrCasteljau)
        {
            // Con 5 muestras, Evaluar usa t = k/4 = 0, 0.25, 0.5, 0.75, 1
            clsResultadoEvaluacion objEvalBernstein = objBernstein.Evaluar(lstPuntos, arrValoresT.Length);
            clsResultadoEvaluacion objEvalCasteljau = objDeCasteljau.Evaluar(lstPuntos, arrValoresT.Length);
            clsResultadoComparacion objComparacion = new clsComparador().Comparar(objEvalBernstein, objEvalCasteljau);

            objTexto.AppendLine("RESUMEN");
            objTexto.AppendLine("  t     | Bernstein                | De Casteljau");
            for (int entJ = 0; entJ < arrValoresT.Length; entJ++)
            {
                objTexto.AppendLine("  " + Numero(arrValoresT[entJ], 2) + "  | " + Punto(arrBernstein[entJ]).PadRight(24)
                    + " | " + Punto(arrCasteljau[entJ]));
            }
            objTexto.AppendLine();
            objTexto.AppendLine("Verificacion con Evaluar(lstPuntos, 5) de cada clase:");
            objTexto.AppendLine("  Diferencia maxima       = " + objComparacion.dblDiferenciaMaxima.ToString("E3", objCultura));
            objTexto.AppendLine("  Diferencia promedio     = " + objComparacion.dblDiferenciaPromedio.ToString("E3", objCultura));
            objTexto.AppendLine("  Error cuadratico medio  = " + objComparacion.dblErrorCuadraticoMedio.ToString("E3", objCultura));
        }

        // Formatea un numero con punto decimal
        private string Numero(double dblValor, int entDecimales)
        {
            return dblValor.ToString("F" + entDecimales, objCultura);
        }

        // Formatea un punto como (x, y) con 6 decimales
        private string Punto(clsPuntoControl ptoValor)
        {
            return "(" + Numero(ptoValor.dblX, 6) + ", " + Numero(ptoValor.dblY, 6) + ")";
        }
    }
}
