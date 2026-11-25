namespace BezierComparador
{
    // Representa un punto 2D (X, Y) en el plano cartesiano
    public class clsPuntoControl
    {
        public double dblX;
        public double dblY;

        public double DblX
        {
            get { return dblX; }
            set { dblX = value; }
        }

        public double DblY
        {
            get { return dblY; }
            set { dblY = value; }
        }

        // Crea el punto en el origen (0, 0)
        public clsPuntoControl()
        {
            dblX = 0.0;
            dblY = 0.0;
        }

        // Crea el punto con coordenadas dadas
        public clsPuntoControl(double dblValorX, double dblValorY)
        {
            dblX = dblValorX;
            dblY = dblValorY;
        }

        // Devuelve una copia independiente del punto
        public clsPuntoControl Clonar()
        {
            return new clsPuntoControl(dblX, dblY);
        }

        // Texto del punto, ejemplo: (1.0000, 3.0000)
        public override string ToString()
        {
            return "(" + dblX.ToString("F4") + ", " + dblY.ToString("F4") + ")";
        }
    }
}
