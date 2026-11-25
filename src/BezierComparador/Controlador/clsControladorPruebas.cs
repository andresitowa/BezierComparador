using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BezierComparador
{
    // Representa una fila de resultados de un experimento (grado y muestras fijos)
    public class clsFilaPrueba
    {
        public int entGrado;
        public int entMuestras;
        public double dblTiempoBernsteinMs;
        public double dblDesviacionBernsteinMs;
        public double dblTiempoDeCasteljauMs;
        public double dblDesviacionDeCasteljauMs;
        public double dblErrorMaximo;
        public double dblErrorPromedio;
        public double dblErrorCuadraticoMedio;
        public string cadMetodoMasRapido;

        public clsFilaPrueba()
        {
            cadMetodoMasRapido = "";
        }
    }

    // Representa el resultado de estabilidad numerica en grados altos (50 y 100) vs referencia decimal
    public class clsFilaEstabilidad
    {
        public int entGrado;
        public int entMuestras;
        public double dblErrorMaxBernsteinVsRef;
        public double dblErrorPromBernsteinVsRef;
        public double dblErrorMaxCasteljauVsRef;
        public double dblErrorPromCasteljauVsRef;
        public double dblDiferenciaEntreAmbosMax;
        public string cadMetodoMasEstable;

        public clsFilaEstabilidad()
        {
            cadMetodoMasEstable = "";
        }
    }

    // Coordina los experimentos de rendimiento, estabilidad numerica y exportacion CSV
    public class clsControladorPruebas
    {
        private const int entSemillaFija = 12345;
        private int[] arrGradosPrueba;
        private int[] arrMuestrasPrueba;
        private int[] arrGradosEstabilidad;
        private clsMedidor objMedidor;
        private clsComparador objComparador;
        private IEvaluadorCurva objEvaluadorBernstein;
        private IEvaluadorCurva objEvaluadorDeCasteljau;
        private List<clsFilaPrueba> lstResultados;
        private List<clsFilaEstabilidad> lstEstabilidad;

        public List<clsFilaPrueba> LstResultados
        {
            get { return lstResultados; }
        }

        public List<clsFilaEstabilidad> LstEstabilidad
        {
            get { return lstEstabilidad; }
        }

        public int[] ArrGradosPrueba
        {
            get { return arrGradosPrueba; }
        }

        public int[] ArrMuestrasPrueba
        {
            get { return arrMuestrasPrueba; }
        }

        public clsControladorPruebas()
        {
            arrGradosPrueba = new int[] { 2, 3, 5, 10, 20, 50 };
            arrMuestrasPrueba = new int[] { 100, 1000, 10000, 100000 };
            arrGradosEstabilidad = new int[] { 50, 100 };
            objMedidor = new clsMedidor(10);
            objComparador = new clsComparador();
            objEvaluadorBernstein = new clsEvaluadorBernstein();
            objEvaluadorDeCasteljau = new clsEvaluadorDeCasteljau();
            lstResultados = new List<clsFilaPrueba>();
            lstEstabilidad = new List<clsFilaEstabilidad>();
        }

        // Genera n + 1 puntos de control aleatorios reproducibles usando semilla fija
        public List<clsPuntoControl> GenerarPuntosAleatorios(int entGrado)
        {
            Random objAleatorio = new Random(entSemillaFija + entGrado);
            List<clsPuntoControl> lstPuntos = new List<clsPuntoControl>();
            int entCantidadPuntos = entGrado + 1;

            for (int entI = 0; entI < entCantidadPuntos; entI++)
            {
                double dblX = objAleatorio.NextDouble() * 800.0;
                double dblY = objAleatorio.NextDouble() * 600.0;
                lstPuntos.Add(new clsPuntoControl(dblX, dblY));
            }

            return lstPuntos;
        }

        // Ejecuta un caso individual de prueba para un grado y cantidad de muestras
        public clsFilaPrueba EjecutarCaso(int entGrado, int entMuestras, int entRepeticiones)
        {
            List<clsPuntoControl> lstPuntos = GenerarPuntosAleatorios(entGrado);

            clsResultadoEvaluacion objResBernstein = objMedidor.Medir(
                objEvaluadorBernstein,
                lstPuntos,
                entMuestras,
                entRepeticiones);

            clsResultadoEvaluacion objResCasteljau = objMedidor.Medir(
                objEvaluadorDeCasteljau,
                lstPuntos,
                entMuestras,
                entRepeticiones);

            clsResultadoComparacion objComparacion = objComparador.Comparar(
                objResBernstein.arrPuntosCurva,
                objResCasteljau.arrPuntosCurva);

            clsFilaPrueba objFila = new clsFilaPrueba();
            objFila.entGrado = entGrado;
            objFila.entMuestras = entMuestras;
            objFila.dblTiempoBernsteinMs = objResBernstein.dblTiempoPromedioMs;
            objFila.dblDesviacionBernsteinMs = objResBernstein.dblDesviacionEstandarMs;
            objFila.dblTiempoDeCasteljauMs = objResCasteljau.dblTiempoPromedioMs;
            objFila.dblDesviacionDeCasteljauMs = objResCasteljau.dblDesviacionEstandarMs;
            objFila.dblErrorMaximo = objComparacion.dblDiferenciaMaxima;
            objFila.dblErrorPromedio = objComparacion.dblDiferenciaPromedio;
            objFila.dblErrorCuadraticoMedio = objComparacion.dblErrorCuadraticoMedio;

            if (objFila.dblTiempoBernsteinMs <= objFila.dblTiempoDeCasteljauMs)
            {
                objFila.cadMetodoMasRapido = "Bernstein";
            }
            else
            {
                objFila.cadMetodoMasRapido = "De Casteljau";
            }

            return objFila;
        }

        // Ejecuta toda la bateria de pruebas (grados 2..50 y muestras 100..100000)
        public List<clsFilaPrueba> EjecutarTodasLasPruebas(int entRepeticiones, Action<int, string>? objProgreso)
        {
            if (entRepeticiones < 10)
            {
                entRepeticiones = 10;
            }

            lstResultados.Clear();
            int entTotalCasos = arrGradosPrueba.Length * arrMuestrasPrueba.Length;
            int entCasoActual = 0;

            for (int entIndiceGrado = 0; entIndiceGrado < arrGradosPrueba.Length; entIndiceGrado++)
            {
                int entGrado = arrGradosPrueba[entIndiceGrado];

                for (int entIndiceMuestra = 0; entIndiceMuestra < arrMuestrasPrueba.Length; entIndiceMuestra++)
                {
                    int entMuestras = arrMuestrasPrueba[entIndiceMuestra];
                    entCasoActual++;

                    int entPorcentaje = (entCasoActual * 100) / entTotalCasos;
                    string cadMensaje = "Evaluando grado " + entGrado + " con " + entMuestras + " muestras...";

                    if (objProgreso != null)
                    {
                        objProgreso(entPorcentaje, cadMensaje);
                    }

                    clsFilaPrueba objFila = EjecutarCaso(entGrado, entMuestras, entRepeticiones);
                    lstResultados.Add(objFila);
                }
            }

            return lstResultados;
        }

        // Evalua la curva de referencia usando precision decimal de 128 bits para medir estabilidad
        // Formula: P_i^(r) = (1 - t) * P_i^(r-1) + t * P_(i+1)^(r-1) en aritmetica decimal
        private clsPuntoControl[] EvaluarReferenciaDecimal(List<clsPuntoControl> lstPuntos, int entMuestras)
        {
            int entCantidad = lstPuntos.Count;
            int entGrado = entCantidad - 1;
            clsPuntoControl[] arrReferencia = new clsPuntoControl[entMuestras];
            decimal[] arrDecX = new decimal[entCantidad];
            decimal[] arrDecY = new decimal[entCantidad];

            for (int entK = 0; entK < entMuestras; entK++)
            {
                decimal decT = (decimal)entK / (decimal)(entMuestras - 1);
                decimal decUnoMenosT = 1.0m - decT;

                for (int entI = 0; entI < entCantidad; entI++)
                {
                    arrDecX[entI] = (decimal)lstPuntos[entI].dblX;
                    arrDecY[entI] = (decimal)lstPuntos[entI].dblY;
                }

                for (int entR = 1; entR <= entGrado; entR++)
                {
                    int entLimite = entGrado - entR;
                    for (int entI = 0; entI <= entLimite; entI++)
                    {
                        arrDecX[entI] = decUnoMenosT * arrDecX[entI] + decT * arrDecX[entI + 1];
                        arrDecY[entI] = decUnoMenosT * arrDecY[entI] + decT * arrDecY[entI + 1];
                    }
                }

                arrReferencia[entK] = new clsPuntoControl((double)arrDecX[0], (double)arrDecY[0]);
            }

            return arrReferencia;
        }

        // Prueba de estabilidad numerica en grados altos (50 y 100) contra referencia decimal
        public List<clsFilaEstabilidad> EjecutarPruebaEstabilidad(int entMuestras)
        {
            if (entMuestras < 100)
            {
                entMuestras = 1000;
            }

            lstEstabilidad.Clear();

            for (int entI = 0; entI < arrGradosEstabilidad.Length; entI++)
            {
                int entGrado = arrGradosEstabilidad[entI];
                List<clsPuntoControl> lstPuntos = GenerarPuntosAleatorios(entGrado);

                clsResultadoEvaluacion objBernstein = objEvaluadorBernstein.Evaluar(lstPuntos, entMuestras);
                clsResultadoEvaluacion objCasteljau = objEvaluadorDeCasteljau.Evaluar(lstPuntos, entMuestras);
                clsPuntoControl[] arrReferencia = EvaluarReferenciaDecimal(lstPuntos, entMuestras);

                clsResultadoComparacion objCompBernsteinRef = objComparador.Comparar(objBernstein.arrPuntosCurva, arrReferencia);
                clsResultadoComparacion objCompCasteljauRef = objComparador.Comparar(objCasteljau.arrPuntosCurva, arrReferencia);
                clsResultadoComparacion objCompAmbos = objComparador.Comparar(objBernstein.arrPuntosCurva, objCasteljau.arrPuntosCurva);

                clsFilaEstabilidad objFilaEst = new clsFilaEstabilidad();
                objFilaEst.entGrado = entGrado;
                objFilaEst.entMuestras = entMuestras;
                objFilaEst.dblErrorMaxBernsteinVsRef = objCompBernsteinRef.dblDiferenciaMaxima;
                objFilaEst.dblErrorPromBernsteinVsRef = objCompBernsteinRef.dblDiferenciaPromedio;
                objFilaEst.dblErrorMaxCasteljauVsRef = objCompCasteljauRef.dblDiferenciaMaxima;
                objFilaEst.dblErrorPromCasteljauVsRef = objCompCasteljauRef.dblDiferenciaPromedio;
                objFilaEst.dblDiferenciaEntreAmbosMax = objCompAmbos.dblDiferenciaMaxima;

                if (objFilaEst.dblErrorMaxCasteljauVsRef <= objFilaEst.dblErrorMaxBernsteinVsRef)
                {
                    objFilaEst.cadMetodoMasEstable = "De Casteljau";
                }
                else
                {
                    objFilaEst.cadMetodoMasEstable = "Bernstein";
                }

                lstEstabilidad.Add(objFilaEst);
            }

            return lstEstabilidad;
        }

        // Exporta las filas de resultados y de estabilidad numerica a un archivo CSV
        public void ExportarResultadosCsv(string cadRutaArchivo)
        {
            StringBuilder objTexto = new StringBuilder();
            CultureInfo objCultura = CultureInfo.InvariantCulture;

            objTexto.AppendLine("Grado,Muestras,TiempoBernsteinMs,DesvBernsteinMs,TiempoDeCasteljauMs,DesvDeCasteljauMs,ErrorMaximo,ErrorPromedio,ECM,MasRapido");

            for (int entI = 0; entI < lstResultados.Count; entI++)
            {
                clsFilaPrueba objFila = lstResultados[entI];
                objTexto.Append(objFila.entGrado.ToString(objCultura)).Append(",");
                objTexto.Append(objFila.entMuestras.ToString(objCultura)).Append(",");
                objTexto.Append(objFila.dblTiempoBernsteinMs.ToString("F6", objCultura)).Append(",");
                objTexto.Append(objFila.dblDesviacionBernsteinMs.ToString("F6", objCultura)).Append(",");
                objTexto.Append(objFila.dblTiempoDeCasteljauMs.ToString("F6", objCultura)).Append(",");
                objTexto.Append(objFila.dblDesviacionDeCasteljauMs.ToString("F6", objCultura)).Append(",");
                objTexto.Append(objFila.dblErrorMaximo.ToString("E6", objCultura)).Append(",");
                objTexto.Append(objFila.dblErrorPromedio.ToString("E6", objCultura)).Append(",");
                objTexto.Append(objFila.dblErrorCuadraticoMedio.ToString("E6", objCultura)).Append(",");
                objTexto.AppendLine(objFila.cadMetodoMasRapido);
            }

            if (lstEstabilidad.Count > 0)
            {
                objTexto.AppendLine();
                objTexto.AppendLine("PRUEBA DE ESTABILIDAD NUMERICA (CONTRA REFERENCIA DECIMAL 128 BITS)");
                objTexto.AppendLine("Grado,Muestras,ErrorMaxBernsteinVsDecimal,ErrorPromBernsteinVsDecimal,ErrorMaxCasteljauVsDecimal,ErrorPromCasteljauVsDecimal,DifMaxAmbos,MasEstable");

                for (int entI = 0; entI < lstEstabilidad.Count; entI++)
                {
                    clsFilaEstabilidad objEst = lstEstabilidad[entI];
                    objTexto.Append(objEst.entGrado.ToString(objCultura)).Append(",");
                    objTexto.Append(objEst.entMuestras.ToString(objCultura)).Append(",");
                    objTexto.Append(objEst.dblErrorMaxBernsteinVsRef.ToString("E6", objCultura)).Append(",");
                    objTexto.Append(objEst.dblErrorPromBernsteinVsRef.ToString("E6", objCultura)).Append(",");
                    objTexto.Append(objEst.dblErrorMaxCasteljauVsRef.ToString("E6", objCultura)).Append(",");
                    objTexto.Append(objEst.dblErrorPromCasteljauVsRef.ToString("E6", objCultura)).Append(",");
                    objTexto.Append(objEst.dblDiferenciaEntreAmbosMax.ToString("E6", objCultura)).Append(",");
                    objTexto.AppendLine(objEst.cadMetodoMasEstable);
                }
            }

            File.WriteAllText(cadRutaArchivo, objTexto.ToString(), Encoding.UTF8);
        }
    }
}
