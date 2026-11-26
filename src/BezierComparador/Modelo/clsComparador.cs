using System;

namespace BezierComparador
{
    // Calcula la diferencia numerica punto a punto entre dos curvas evaluadas
    public class clsComparador
    {
        // Formula por muestra: d_k = raiz((x1 - x2)^2 + (y1 - y2)^2)
        // Maxima: max(d_k). Promedio: (1/M) * suma(d_k)
        // Error cuadratico medio: ECM = (1/M) * suma(d_k^2)
        // M = cantidad de muestras comparadas. Complejidad O(M)
        public clsResultadoComparacion Comparar(clsPuntoControl[] arrCurvaA, clsPuntoControl[] arrCurvaB)
        {
            clsResultadoComparacion objResultado = new clsResultadoComparacion();

            if (arrCurvaA == null || arrCurvaB == null)
            {
                return objResultado;
            }

            // Si los tamanos difieren solo se comparan las muestras comunes
            int entCantidad = Math.Min(arrCurvaA.Length, arrCurvaB.Length);
            if (entCantidad == 0)
            {
                return objResultado;
            }

            double dblMaximo = 0.0;
            double dblSumaDistancias = 0.0;
            double dblSumaCuadrados = 0.0;

            // Recorre cada muestra k y acumula la distancia entre ambos puntos
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
            objResultado.dblDiferenciaPromedio = dblSumaDistancias / entCantidad;
            objResultado.dblErrorCuadraticoMedio = dblSumaCuadrados / entCantidad;
            objResultado.entMuestrasComparadas = entCantidad;
            return objResultado;
        }

        // Compara directamente dos resultados de evaluacion
        public clsResultadoComparacion Comparar(clsResultadoEvaluacion objResultadoA, clsResultadoEvaluacion objResultadoB)
        {
            if (objResultadoA == null || objResultadoB == null)
            {
                return new clsResultadoComparacion();
            }
            return Comparar(objResultadoA.arrPuntosCurva, objResultadoB.arrPuntosCurva);
        }
    }
}
