using System.Collections.Generic;
using System.Diagnostics;

namespace BezierComparador
{
    // Evalua la curva de Bezier mediante interpolaciones sucesivas de De Casteljau
    public class clsEvaluadorDeCasteljau : IEvaluadorCurva
    {
        public string CadNombre
        {
            get { return "De Casteljau"; }
        }

        // Formula: P_i^(r) = (1 - t) * P_i^(r-1) + t * P_(i+1)^(r-1)
        // r de 1..n, i de 0..n-r. Parametro: dblT = k / (entMuestras - 1)
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

            int entCantidad = lstPuntos.Count;
            int entGrado = entCantidad - 1;
            clsPuntoControl[] arrCurva = new clsPuntoControl[entMuestras];
            double[] arrTrabajoX = new double[entCantidad];
            double[] arrTrabajoY = new double[entCantidad];

            // Evalua cada muestra del parametro t
            for (int entK = 0; entK < entMuestras; entK++)
            {
                double dblT = (double)entK / (double)(entMuestras - 1);
                double dblUnoMenosT = 1.0 - dblT;

                // Copia los puntos de control iniciales para el nivel r = 0
                for (int entI = 0; entI < entCantidad; entI++)
                {
                    arrTrabajoX[entI] = lstPuntos[entI].dblX;
                    arrTrabajoY[entI] = lstPuntos[entI].dblY;
                }

                // Interpolacion lineal recursiva por niveles r = 1..n
                for (int entR = 1; entR <= entGrado; entR++)
                {
                    int entLimite = entGrado - entR;
                    for (int entI = 0; entI <= entLimite; entI++)
                    {
                        arrTrabajoX[entI] = dblUnoMenosT * arrTrabajoX[entI] + dblT * arrTrabajoX[entI + 1];
                        arrTrabajoY[entI] = dblUnoMenosT * arrTrabajoY[entI] + dblT * arrTrabajoY[entI + 1];
                    }
                }

                arrCurva[entK] = new clsPuntoControl(arrTrabajoX[0], arrTrabajoY[0]);
            }

            objReloj.Stop();
            double dblMs = objReloj.Elapsed.TotalMilliseconds;

            objResultado.arrPuntosCurva = arrCurva;
            objResultado.dblTiempoPromedioMs = dblMs;
            objResultado.dblTiempoMinimoMs = dblMs;
            objResultado.dblTiempoMaximoMs = dblMs;
            return objResultado;
        }

        // Formula: P_i^(r) = (1 - t) * P_i^(r-1) + t * P_(i+1)^(r-1) para un t fijo
        public List<List<clsPuntoControl>> ObtenerNivelesIntermedios(List<clsPuntoControl> lstPuntos, double dblT)
        {
            List<List<clsPuntoControl>> lstNiveles = new List<List<clsPuntoControl>>();

            if (lstPuntos == null || lstPuntos.Count < 2)
            {
                return lstNiveles;
            }

            double dblUnoMenosT = 1.0 - dblT;
            List<clsPuntoControl> lstNivelActual = new List<clsPuntoControl>();

            for (int entI = 0; entI < lstPuntos.Count; entI++)
            {
                lstNivelActual.Add(new clsPuntoControl(lstPuntos[entI].dblX, lstPuntos[entI].dblY));
            }
            lstNiveles.Add(lstNivelActual);

            int entGrado = lstPuntos.Count - 1;
            for (int entR = 1; entR <= entGrado; entR++)
            {
                List<clsPuntoControl> lstAnterior = lstNiveles[entR - 1];
                List<clsPuntoControl> lstNuevoNivel = new List<clsPuntoControl>();

                for (int entI = 0; entI < lstAnterior.Count - 1; entI++)
                {
                    double dblX = dblUnoMenosT * lstAnterior[entI].dblX + dblT * lstAnterior[entI + 1].dblX;
                    double dblY = dblUnoMenosT * lstAnterior[entI].dblY + dblT * lstAnterior[entI + 1].dblY;
                    lstNuevoNivel.Add(new clsPuntoControl(dblX, dblY));
                }

                lstNiveles.Add(lstNuevoNivel);
            }

            return lstNiveles;
        }
    }
}
