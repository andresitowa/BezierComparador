namespace BezierComparador
{
    // Representa un punto 2D en el plano cartesiano
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

        public clsPuntoControl()
        {
            dblX = 0.0;
            dblY = 0.0;
        }

        public clsPuntoControl(double dblValorX, double dblValorY)
        {
            dblX = dblValorX;
            dblY = dblValorY;
        }
    }
}
