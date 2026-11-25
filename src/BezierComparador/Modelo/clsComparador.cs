using System;

namespace BezierComparador
{
    // Almacena las metricas de diferencia numerica entre dos curvas evaluadas
    public class clsResultadoComparacion
    {
        public double dblDiferenciaMaxima;
        public double dblDiferenciaPromedio;
        public double dblErrorCuadraticoMedio;

        public double DblDiferenciaMaxima
        {
            get { return dblDiferenciaMaxima; }
            set { dblDiferenciaMaxima = value; }
        }

        public double DblDiferenciaPromedio
        {
            get { return dblDiferenciaPromedio; }
            set { dblDiferenciaPromedio = value; }
        }

        public double DblErrorCuadraticoMedio
        {
            get { return dblErrorCuadraticoMedio; }
            set { dblErrorCuadraticoMedio = value; }
        }
    }

    // Calcula la distancia euclidiana punto a punto entre las dos evaluaciones
    public class clsComparador
    {
        // Formula por muestra: d_k = raiz((x1 - x2)^2 + (y1 - y2)^2)
        // Formula ECM: (1/M) * suma(d_k^2)
        public clsResultadoComparacion Comparar(clsPuntoControl[] arrCurvaA, clsPuntoControl[] arrCurvaB)
        {
            clsResultadoComparacion objResultado = new clsResultadoComparacion();

            if (arrCurvaA == null || arrCurvaB == null)
            {
                return objResultado;
            }

            int entCantidad = arrCurvaA.Length;
            if (arrCurvaB.Length < entCantidad)
            {
                entCantidad = arrCurvaB.Length;
            }

            if (entCantidad == 0)
            {
                return objResultado;
            }

            double dblMaximo = 0.0;
            double dblSumaDistancias = 0.0;
            double dblSumaCuadrados = 0.0;

            // Recorre cada muestra k y calcula la diferencia entre ambos metodos
            for (int entK = 0; entK < entCantidad; entK++)
            {
                double dblDifX = arrCurvaA[entK].dblX - arrCurvaB[entK].dblX;
                double dblDifY = arrCurvaA[entK].dblY - arrCurvaB[entK].dblY;
                double dblDistanciaCuadrada = dblDifX * dblDifX + dblDifY * dblDifY;
                double dblDistancia = Math.Sqrt(dblDistanciaCuadrada);

                if (dblDistancia > dblMaximo)
                {
                    dblMaximo = dblDistancia;
                }

                dblSumaDistancias += dblDistancia;
                dblSumaCuadrados += dblDistanciaCuadrada;
            }

            objResultado.dblDiferenciaMaxima = dblMaximo;
            objResultado.dblDiferenciaPromedio = dblSumaDistancias / (double)entCantidad;
            objResultado.dblErrorCuadraticoMedio = dblSumaCuadrados / (double)entCantidad;

            return objResultado;
        }
    }
}
