using System.Collections.Generic;

namespace BezierComparador
{
    // Almacena y gestiona los puntos de control de una curva de Bezier
    public class clsCurvaBezier
    {
        // Minimo de puntos para poder evaluar (curva de grado 1)
        public const int entMinimoPuntos = 2;

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

        // Formula: grado n = cantidad de puntos - 1
        public int ObtenerGrado()
        {
            return lstPuntosControl.Count - 1;
        }

        // Devuelve cuantos puntos de control tiene la curva
        public int ObtenerCantidadPuntos()
        {
            return lstPuntosControl.Count;
        }

        // Verifica que el indice exista dentro de la lista
        public bool EsIndiceValido(int entIndice)
        {
            return entIndice >= 0 && entIndice < lstPuntosControl.Count;
        }

        // Verifica que haya al menos 2 puntos para evaluar
        public bool PuedeEvaluar()
        {
            return lstPuntosControl.Count >= entMinimoPuntos;
        }

        // Mensaje para el usuario si la curva no se puede evaluar
        public string ObtenerMensajeValidacion()
        {
            if (PuedeEvaluar())
            {
                return "";
            }
            return "Agregue al menos " + entMinimoPuntos + " puntos";
        }

        // Agrega un punto al final; devuelve false si es nulo
        public bool AgregarPunto(clsPuntoControl ptoNuevo)
        {
            if (ptoNuevo == null)
            {
                return false;
            }
            lstPuntosControl.Add(ptoNuevo);
            return true;
        }

        // Agrega un punto con coordenadas X y Y
        public bool AgregarPunto(double dblX, double dblY)
        {
            return AgregarPunto(new clsPuntoControl(dblX, dblY));
        }

        // Devuelve el punto del indice o null si el indice no existe
        public clsPuntoControl? ObtenerPunto(int entIndice)
        {
            if (!EsIndiceValido(entIndice))
            {
                return null;
            }
            return lstPuntosControl[entIndice];
        }

        // Elimina el punto del indice; devuelve false si el indice no existe
        public bool EliminarPunto(int entIndice)
        {
            if (!EsIndiceValido(entIndice))
            {
                return false;
            }
            lstPuntosControl.RemoveAt(entIndice);
            return true;
        }

        // Cambia las coordenadas del punto; devuelve false si el indice no existe
        public bool ModificarPunto(int entIndice, double dblX, double dblY)
        {
            if (!EsIndiceValido(entIndice))
            {
                return false;
            }
            lstPuntosControl[entIndice].dblX = dblX;
            lstPuntosControl[entIndice].dblY = dblY;
            return true;
        }

        // Elimina todos los puntos de control
        public void Limpiar()
        {
            lstPuntosControl.Clear();
        }
    }
}
