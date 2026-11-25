namespace BezierComparador
{
    // Guarda los puntos calculados y las metricas de tiempo de una evaluacion
    public class clsResultadoEvaluacion
    {
        public string cadNombreMetodo;
        public clsPuntoControl[] arrPuntosCurva;
        public double dblTiempoPromedioMs;
        public double dblTiempoMinimoMs;
        public double dblTiempoMaximoMs;
        public double dblDesviacionEstandarMs;

        public string CadNombreMetodo
        {
            get { return cadNombreMetodo; }
            set { cadNombreMetodo = value; }
        }

        public clsPuntoControl[] ArrPuntosCurva
        {
            get { return arrPuntosCurva; }
            set { arrPuntosCurva = value; }
        }

        public double DblTiempoPromedioMs
        {
            get { return dblTiempoPromedioMs; }
            set { dblTiempoPromedioMs = value; }
        }

        public double DblTiempoMinimoMs
        {
            get { return dblTiempoMinimoMs; }
            set { dblTiempoMinimoMs = value; }
        }

        public double DblTiempoMaximoMs
        {
            get { return dblTiempoMaximoMs; }
            set { dblTiempoMaximoMs = value; }
        }

        public double DblDesviacionEstandarMs
        {
            get { return dblDesviacionEstandarMs; }
            set { dblDesviacionEstandarMs = value; }
        }

        public clsResultadoEvaluacion()
        {
            cadNombreMetodo = "";
            arrPuntosCurva = new clsPuntoControl[0];
            dblTiempoPromedioMs = 0.0;
            dblTiempoMinimoMs = 0.0;
            dblTiempoMaximoMs = 0.0;
            dblDesviacionEstandarMs = 0.0;
        }
    }
}
