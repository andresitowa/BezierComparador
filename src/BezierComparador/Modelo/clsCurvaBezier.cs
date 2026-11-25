using System.Collections.Generic;

namespace BezierComparador
{
    // Almacena y gestiona los puntos de control de una curva de Bezier
    public class clsCurvaBezier
    {
        public List<clsPuntoControl> lstPuntosControl;

        public List<clsPuntoControl> LstPuntosControl
        {
            get { return lstPuntosControl; }
            set { lstPuntosControl = value; }
        }

        public clsCurvaBezier()
        {
            lstPuntosControl = new List<clsPuntoControl>();
        }

        // Formula: Grado n = cantidad de puntos - 1
        public int ObtenerGrado()
        {
            return lstPuntosControl.Count - 1;
        }

        // Agrega un punto de control al final de la lista
        public void AgregarPunto(clsPuntoControl ptoNuevo)
        {
            if (ptoNuevo != null)
            {
                lstPuntosControl.Add(ptoNuevo);
            }
        }

        // Sobrecarga para agregar un punto con coordenadas X y Y
        public void AgregarPunto(double dblX, double dblY)
        {
            lstPuntosControl.Add(new clsPuntoControl(dblX, dblY));
        }

        // Elimina un punto segun su indice en la lista
        public void EliminarPunto(int entIndice)
        {
            if (entIndice >= 0 && entIndice < lstPuntosControl.Count)
            {
                lstPuntosControl.RemoveAt(entIndice);
            }
        }

        // Modifica las coordenadas de un punto existente
        public void ModificarPunto(int entIndice, double dblX, double dblY)
        {
            if (entIndice >= 0 && entIndice < lstPuntosControl.Count)
            {
                lstPuntosControl[entIndice].DblX = dblX;
                lstPuntosControl[entIndice].DblY = dblY;
            }
        }

        // Elimina todos los puntos de control de la curva
        public void Limpiar()
        {
            lstPuntosControl.Clear();
        }
    }
}
