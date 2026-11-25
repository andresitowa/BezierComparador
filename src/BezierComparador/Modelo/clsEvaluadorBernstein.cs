using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BezierComparador
{
    // Evalua la curva de Bezier usando polinomios de Bernstein
    public class clsEvaluadorBernstein : IEvaluadorCurva
    {
        public string CadNombre
        {
            get { return "Bernstein"; }
        }

        // Formula: B(t) = suma para i=0..n de C(n,i) * (1-t)^(n-i) * t^i * P_i
        // Parametro: dblT = k / (entMuestras - 1), con k de 0 a entMuestras - 1
        public clsResultadoEvaluacion Evaluar(List<clsPuntoControl> lstPuntos, int entMuestras)
        {
            clsResultadoEvaluacion objResultado = new clsResultadoEvaluacion();
            objResultado.cadNombreMetodo = CadNombre;

            if (lstPuntos == null || lstPuntos.Count < 2 || entMuestras < 2)
            {
                objResultado.arrPuntosCurva = new clsPuntoControl[0];
                return objResultado;
            }

            Stopwatch objReloj = Stopwatch.StartNew();

            int entN = lstPuntos.Count - 1;
            double[] arrCoeficientes = CalcularCoeficientesBinomiales(entN);
            clsPuntoControl[] arrCurva = new clsPuntoControl[entMuestras];

            // Recorre todas las muestras del parametro t entre 0 y 1
            for (int entK = 0; entK < entMuestras; entK++)
            {
                double dblT = (double)entK / (double)(entMuestras - 1);
                double dblUnoMenosT = 1.0 - dblT;
                double dblSumaX = 0.0;
                double dblSumaY = 0.0;

                // Evalua la combinacion lineal con los polinomios base de Bernstein
                for (int entI = 0; entI <= entN; entI++)
                {
                    double dblPotenciaUnoMenosT = Math.Pow(dblUnoMenosT, entN - entI);
                    double dblPotenciaT = Math.Pow(dblT, entI);
                    double dblBaseBernstein = arrCoeficientes[entI] * dblPotenciaUnoMenosT * dblPotenciaT;

                    dblSumaX += dblBaseBernstein * lstPuntos[entI].dblX;
                    dblSumaY += dblBaseBernstein * lstPuntos[entI].dblY;
                }

                arrCurva[entK] = new clsPuntoControl(dblSumaX, dblSumaY);
            }

            objReloj.Stop();
            double dblMs = objReloj.Elapsed.TotalMilliseconds;

            objResultado.arrPuntosCurva = arrCurva;
            objResultado.dblTiempoPromedioMs = dblMs;
            objResultado.dblTiempoMinimoMs = dblMs;
            objResultado.dblTiempoMaximoMs = dblMs;
            return objResultado;
        }

        // Formula: C(n,i) = C(n, i-1) * (n - i + 1) / i (iterativo sin factorial)
        public double[] CalcularCoeficientesBinomiales(int entN)
        {
            double[] arrCoef = new double[entN + 1];
            arrCoef[0] = 1.0;

            for (int entI = 1; entI <= entN; entI++)
            {
                arrCoef[entI] = arrCoef[entI - 1] * (double)(entN - entI + 1) / (double)entI;
            }

            return arrCoef;
        }
    }
}
