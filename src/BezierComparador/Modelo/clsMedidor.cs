using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BezierComparador
{
    // Mide el rendimiento de un evaluador de curvas usando Stopwatch
    public class clsMedidor
    {
        private int entRepeticiones;

        public int EntRepeticiones
        {
            get { return entRepeticiones; }
            set
            {
                if (value < 10)
                {
                    entRepeticiones = 10;
                }
                else
                {
                    entRepeticiones = value;
                }
            }
        }

        public clsMedidor()
        {
            entRepeticiones = 10;
        }

        public clsMedidor(int entCantidadRepeticiones)
        {
            EntRepeticiones = entCantidadRepeticiones;
        }

        // Formula: Promedio = (1/N) * suma(t_i)
        // Formula: Desviacion = raiz( (1/N) * suma((t_i - Promedio)^2) )
        public clsResultadoEvaluacion Medir(IEvaluadorCurva objEvaluador, List<clsPuntoControl> lstPuntos, int entMuestras)
        {
            return Medir(objEvaluador, lstPuntos, entMuestras, entRepeticiones);
        }

        // Ejecuta calentamiento descartado, mide N repeticiones sin dibujar y calcula estadisticas
        public clsResultadoEvaluacion Medir(IEvaluadorCurva objEvaluador, List<clsPuntoControl> lstPuntos, int entMuestras, int entCantidadRepeticiones)
        {
            if (entCantidadRepeticiones < 10)
            {
                entCantidadRepeticiones = 10;
            }

            clsResultadoEvaluacion objResultadoFinal = new clsResultadoEvaluacion();

            if (objEvaluador == null || lstPuntos == null || lstPuntos.Count < 2 || entMuestras < 2)
            {
                return objResultadoFinal;
            }

            objResultadoFinal.cadNombreMetodo = objEvaluador.CadNombre;

            // Ejecucion de calentamiento descartada para estabilizar cache y JIT
            clsResultadoEvaluacion objCalentamiento = objEvaluador.Evaluar(lstPuntos, entMuestras);
            objResultadoFinal.arrPuntosCurva = objCalentamiento.arrPuntosCurva;

            double[] arrTiemposMs = new double[entCantidadRepeticiones];
            double dblSumaTiempos = 0.0;
            double dblMinimo = double.MaxValue;
            double dblMaximo = 0.0;
            Stopwatch objReloj = new Stopwatch();

            // Ciclo principal de medicion pura del calculo matematico
            for (int entI = 0; entI < entCantidadRepeticiones; entI++)
            {
                objReloj.Restart();
                clsResultadoEvaluacion objIteracion = objEvaluador.Evaluar(lstPuntos, entMuestras);
                objReloj.Stop();

                double dblTiempoActual = objReloj.Elapsed.TotalMilliseconds;
                arrTiemposMs[entI] = dblTiempoActual;
                dblSumaTiempos += dblTiempoActual;

                if (dblTiempoActual < dblMinimo)
                {
                    dblMinimo = dblTiempoActual;
                }
                if (dblTiempoActual > dblMaximo)
                {
                    dblMaximo = dblTiempoActual;
                }

                objResultadoFinal.arrPuntosCurva = objIteracion.arrPuntosCurva;
            }

            double dblPromedio = dblSumaTiempos / (double)entCantidadRepeticiones;

            // Calculo de la desviacion estandar en milisegundos
            double dblSumaDiferenciasCuadradas = 0.0;
            for (int entI = 0; entI < entCantidadRepeticiones; entI++)
            {
                double dblDiferencia = arrTiemposMs[entI] - dblPromedio;
                dblSumaDiferenciasCuadradas += dblDiferencia * dblDiferencia;
            }

            double dblVarianza = dblSumaDiferenciasCuadradas / (double)entCantidadRepeticiones;
            double dblDesviacionEstandar = Math.Sqrt(dblVarianza);

            objResultadoFinal.dblTiempoPromedioMs = dblPromedio;
            objResultadoFinal.dblTiempoMinimoMs = dblMinimo;
            objResultadoFinal.dblTiempoMaximoMs = dblMaximo;
            objResultadoFinal.dblDesviacionEstandarMs = dblDesviacionEstandar;

            return objResultadoFinal;
        }
    }
}
