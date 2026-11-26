namespace BezierComparador
{
    // Guarda las metricas de diferencia numerica entre dos curvas evaluadas
    public class clsResultadoComparacion
    {
        public double dblDiferenciaMaxima;
        public double dblDiferenciaPromedio;
        public double dblErrorCuadraticoMedio;
        public int entMuestrasComparadas;

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

        public int EntMuestrasComparadas
        {
            get { return entMuestrasComparadas; }
            set { entMuestrasComparadas = value; }
        }

        // Crea un resultado en cero
        public clsResultadoComparacion()
        {
            dblDiferenciaMaxima = 0.0;
            dblDiferenciaPromedio = 0.0;
            dblErrorCuadraticoMedio = 0.0;
            entMuestrasComparadas = 0;
        }
    }
}
