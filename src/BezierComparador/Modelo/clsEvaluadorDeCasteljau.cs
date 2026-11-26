using System.Collections.Generic;

namespace BezierComparador
{
    // Evalua la curva de Bezier con interpolaciones lineales sucesivas (De Casteljau)
    // Complejidad: O(n^2) por muestra, O(N * n^2) para N muestras
    public class clsEvaluadorDeCasteljau : IEvaluadorCurva
    {
        public string CadNombre
        {
            get { return "De Casteljau"; }
        }

        // Formula: P_i^(r) = (1-t) * P_i^(r-1) + t * P_(i+1)^(r-1)
        // r de 1..n, i de 0..n-r. El punto de la curva es P_0^(n)
        // Parametro: dblT = k / (entMuestras - 1), con k de 0 a entMuestras - 1
        public clsResultadoEvaluacion Evaluar(List<clsPuntoControl> lstPuntos, int entMuestras)
        {
            clsResultadoEvaluacion objResultado = new clsResultadoEvaluacion(CadNombre);

            // Valida minimo 2 puntos de control y 2 muestras
            if (lstPuntos == null || lstPuntos.Count < clsCurvaBezier.entMinimoPuntos || entMuestras < 2)
            {
                return objResultado;
            }

            int entN = lstPuntos.Count - 1;
            clsPuntoControl[] arrCurva = new clsPuntoControl[entMuestras];

            // Arreglos de trabajo para la copia de X y Y (se reservan una sola vez)
            double[] arrX = new double[entN + 1];
            double[] arrY = new double[entN + 1];

            // Recorre cada muestra del parametro t entre 0 y 1
            for (int entK = 0; entK < entMuestras; entK++)
            {
                double dblT = (double)entK / (double)(entMuestras - 1);
                double dblUnoMenosT = 1.0 - dblT;

                // Copia los puntos de control (nivel r = 0) para no alterar la lista original
                for (int entI = 0; entI <= entN; entI++)
                {
                    arrX[entI] = lstPuntos[entI].dblX;
                    arrY[entI] = lstPuntos[entI].dblY;
                }

                // Reduce por niveles: cada nivel tiene un punto menos, n(n+1)/2 operaciones
                for (int entR = 1; entR <= entN; entR++)
                {
                    for (int entI = 0; entI <= entN - entR; entI++)
                    {
                        arrX[entI] = dblUnoMenosT * arrX[entI] + dblT * arrX[entI + 1];
                        arrY[entI] = dblUnoMenosT * arrY[entI] + dblT * arrY[entI + 1];
                    }
                }

                // Al final del ultimo nivel queda P_0^(n), el punto de la curva
                arrCurva[entK] = new clsPuntoControl(arrX[0], arrY[0]);
            }

            objResultado.arrPuntosCurva = arrCurva;
            return objResultado;
        }

        // Formula: P_i^(r) = (1-t) * P_i^(r-1) + t * P_(i+1)^(r-1) para un t fijo
        // Devuelve todos los niveles: nivel 0 = puntos de control, nivel n = punto de la curva
        // Se usa para dibujar las interpolaciones (no para medir tiempos)
        public List<List<clsPuntoControl>> ObtenerNivelesIntermedios(List<clsPuntoControl> lstPuntos, double dblT)
        {
            List<List<clsPuntoControl>> lstNiveles = new List<List<clsPuntoControl>>();

            if (lstPuntos == null || lstPuntos.Count < clsCurvaBezier.entMinimoPuntos)
            {
                return lstNiveles;
            }

            // Mantiene t dentro del intervalo [0, 1]
            if (dblT < 0.0)
            {
                dblT = 0.0;
            }
            if (dblT > 1.0)
            {
                dblT = 1.0;
            }
            double dblUnoMenosT = 1.0 - dblT;

            // Nivel 0: copia de los puntos de control
            List<clsPuntoControl> lstNivelCero = new List<clsPuntoControl>();
            for (int entI = 0; entI < lstPuntos.Count; entI++)
            {
                lstNivelCero.Add(lstPuntos[entI].Clonar());
            }
            lstNiveles.Add(lstNivelCero);

            // Niveles 1..n: interpola cada par de puntos consecutivos del nivel anterior
            int entN = lstPuntos.Count - 1;
            for (int entR = 1; entR <= entN; entR++)
            {
                List<clsPuntoControl> lstAnterior = lstNiveles[entR - 1];
                List<clsPuntoControl> lstNuevo = new List<clsPuntoControl>();

                for (int entI = 0; entI <= entN - entR; entI++)
                {
                    double dblX = dblUnoMenosT * lstAnterior[entI].dblX + dblT * lstAnterior[entI + 1].dblX;
                    double dblY = dblUnoMenosT * lstAnterior[entI].dblY + dblT * lstAnterior[entI + 1].dblY;
                    lstNuevo.Add(new clsPuntoControl(dblX, dblY));
                }

                lstNiveles.Add(lstNuevo);
            }

            return lstNiveles;
        }
    }
}
