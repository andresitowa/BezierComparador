using System;
using System.Collections.Generic;

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

        public int EntGrado
        {
            get { return entGrado; }
            set { entGrado = value; }
        }

        public int EntMuestras
        {
            get { return entMuestras; }
            set { entMuestras = value; }
        }

        public double DblTiempoBernsteinMs
        {
            get { return dblTiempoBernsteinMs; }
            set { dblTiempoBernsteinMs = value; }
        }

        public double DblDesviacionBernsteinMs
        {
            get { return dblDesviacionBernsteinMs; }
            set { dblDesviacionBernsteinMs = value; }
        }

        public double DblTiempoDeCasteljauMs
        {
            get { return dblTiempoDeCasteljauMs; }
            set { dblTiempoDeCasteljauMs = value; }
        }

        public double DblDesviacionDeCasteljauMs
        {
            get { return dblDesviacionDeCasteljauMs; }
            set { dblDesviacionDeCasteljauMs = value; }
        }

        public double DblErrorMaximo
        {
            get { return dblErrorMaximo; }
            set { dblErrorMaximo = value; }
        }

        public double DblErrorPromedio
        {
            get { return dblErrorPromedio; }
            set { dblErrorPromedio = value; }
        }

        public double DblErrorCuadraticoMedio
        {
            get { return dblErrorCuadraticoMedio; }
            set { dblErrorCuadraticoMedio = value; }
        }

        public string CadMetodoMasRapido
        {
            get { return cadMetodoMasRapido; }
            set { cadMetodoMasRapido = value; }
        }

        public clsFilaPrueba()
        {
            cadMetodoMasRapido = "";
        }
    }

    // Coordina los experimentos de rendimiento y precision entre Bernstein y De Casteljau
    public class clsControladorPruebas
    {
        private const int entSemillaFija = 12345;
        private int[] arrGradosPrueba;
        private int[] arrMuestrasPrueba;
        private clsMedidor objMedidor;
        private clsComparador objComparador;
        private IEvaluadorCurva objEvaluadorBernstein;
        private IEvaluadorCurva objEvaluadorDeCasteljau;
        private List<clsFilaPrueba> lstResultados;

        public List<clsFilaPrueba> LstResultados
        {
            get { return lstResultados; }
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
            objMedidor = new clsMedidor(10);
            objComparador = new clsComparador();
            objEvaluadorBernstein = new clsEvaluadorBernstein();
            objEvaluadorDeCasteljau = new clsEvaluadorDeCasteljau();
            lstResultados = new List<clsFilaPrueba>();
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
    }
}
