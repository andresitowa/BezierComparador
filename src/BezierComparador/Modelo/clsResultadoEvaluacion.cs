namespace BezierComparador
{
    // Guarda los puntos calculados y las metricas de tiempo de una evaluacion
    // Los tiempos los llena clsMedidor; el evaluador solo llena los puntos
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

        // Crea un resultado vacio
        public clsResultadoEvaluacion()
        {
            cadNombreMetodo = "";
            arrPuntosCurva = new clsPuntoControl[0];
            dblTiempoPromedioMs = 0.0;
            dblTiempoMinimoMs = 0.0;
            dblTiempoMaximoMs = 0.0;
            dblDesviacionEstandarMs = 0.0;
        }

        // Crea un resultado vacio con el nombre del metodo
        public clsResultadoEvaluacion(string cadNombre) : this()
        {
            cadNombreMetodo = cadNombre;
        }

        // Indica si la evaluacion produjo puntos
        public bool TienePuntos()
        {
            return arrPuntosCurva != null && arrPuntosCurva.Length > 0;
        }
    }
}
