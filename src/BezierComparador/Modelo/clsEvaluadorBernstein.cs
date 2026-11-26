using System;
using System.Collections.Generic;

namespace BezierComparador
{
    // Evalua la curva de Bezier en forma directa con polinomios de Bernstein
    // Complejidad: O(n) por muestra, O(N * n) para N muestras
    public class clsEvaluadorBernstein : IEvaluadorCurva
    {
        public string CadNombre
        {
            get { return "Bernstein"; }
        }

        // Formula: B(t) = suma para i=0..n de C(n,i) * (1-t)^(n-i) * t^i * P_i
        // Parametro: dblT = k / (entMuestras - 1), con k de 0 a entMuestras - 1
        // Los coeficientes C(n,i) se calculan una sola vez, fuera del ciclo de muestras
        public clsResultadoEvaluacion Evaluar(List<clsPuntoControl> lstPuntos, int entMuestras)
        {
            clsResultadoEvaluacion objResultado = new clsResultadoEvaluacion(CadNombre);

            // Valida minimo 2 puntos de control y 2 muestras
            if (lstPuntos == null || lstPuntos.Count < clsCurvaBezier.entMinimoPuntos || entMuestras < 2)
            {
                return objResultado;
            }

            int entN = lstPuntos.Count - 1;
            double[] arrCoeficientes = CalcularCoeficientesBinomiales(entN);
            clsPuntoControl[] arrCurva = new clsPuntoControl[entMuestras];

            // Recorre cada muestra del parametro t entre 0 y 1
            for (int entK = 0; entK < entMuestras; entK++)
            {
                double dblT = (double)entK / (double)(entMuestras - 1);
                double dblSumaX = 0.0;
                double dblSumaY = 0.0;

                // Suma ponderada de los puntos de control: n + 1 terminos, O(n)
                for (int entI = 0; entI <= entN; entI++)
                {
                    double dblBase = CalcularBase(arrCoeficientes[entI], entN, entI, dblT);
                    dblSumaX += dblBase * lstPuntos[entI].dblX;
                    dblSumaY += dblBase * lstPuntos[entI].dblY;
                }

                arrCurva[entK] = new clsPuntoControl(dblSumaX, dblSumaY);
            }

            objResultado.arrPuntosCurva = arrCurva;
            return objResultado;
        }

        // Formula: C(n,0) = 1 y C(n,i) = C(n,i-1) * (n - i + 1) / i
        // Iterativo sin factorial para evitar desbordamiento, O(n)
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

        // Formula: B_{i,n}(t) = C(n,i) * (1-t)^(n-i) * t^i
        public double CalcularBase(double dblCoeficiente, int entN, int entI, double dblT)
        {
            double dblPotenciaUnoMenosT = ElevarPotencia(1.0 - dblT, entN - entI);
            double dblPotenciaT = ElevarPotencia(dblT, entI);
            return dblCoeficiente * dblPotenciaUnoMenosT * dblPotenciaT;
        }

        // Calcula base^exponente con Math.Pow, tomando 0^0 = 1
        // Asi en t = 0 y t = 1 la curva pasa exacto por P_0 y P_n
        private double ElevarPotencia(double dblBase, int entExponente)
        {
            if (entExponente == 0)
            {
                return 1.0;
            }
            return Math.Pow(dblBase, entExponente);
        }
    }
}
