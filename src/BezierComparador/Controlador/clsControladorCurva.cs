using System.Collections.Generic;

namespace BezierComparador
{
    // Une la Vista interactiva con el Modelo para editar puntos y evaluar ambas curvas
    public class clsControladorCurva
    {
        private clsCurvaBezier objCurva;
        private clsEvaluadorBernstein objEvaluadorBernstein;
        private clsEvaluadorDeCasteljau objEvaluadorDeCasteljau;
        private clsMedidor objMedidor;
        private clsComparador objComparador;

        public clsCurvaBezier ObjCurva
        {
            get { return objCurva; }
        }

        public clsControladorCurva()
        {
            objCurva = new clsCurvaBezier();
            objEvaluadorBernstein = new clsEvaluadorBernstein();
            objEvaluadorDeCasteljau = new clsEvaluadorDeCasteljau();
            objMedidor = new clsMedidor(10);
            objComparador = new clsComparador();
        }

        // Agrega un nuevo punto de control al modelo
        public void AgregarPunto(double dblX, double dblY)
        {
            objCurva.AgregarPunto(dblX, dblY);
        }

        // Elimina un punto de control segun su indice
        public void EliminarPunto(int entIndice)
        {
            objCurva.EliminarPunto(entIndice);
        }

        // Actualiza la posicion de un punto de control al arrastrarlo
        public void ModificarPunto(int entIndice, double dblX, double dblY)
        {
            objCurva.ModificarPunto(entIndice, dblX, dblY);
        }

        // Elimina todos los puntos de control actuales
        public void Limpiar()
        {
            objCurva.Limpiar();
        }

        // Busca si el clic del raton cae sobre algun punto de control dentro de un radio
        public int BuscarIndicePuntoCercano(double dblX, double dblY, double dblRadioPixeles)
        {
            List<clsPuntoControl> lstPuntos = objCurva.LstPuntosControl;
            double dblRadioCuadrado = dblRadioPixeles * dblRadioPixeles;

            for (int entI = lstPuntos.Count - 1; entI >= 0; entI--)
            {
                double dblDx = lstPuntos[entI].DblX - dblX;
                double dblDy = lstPuntos[entI].DblY - dblY;
                if (dblDx * dblDx + dblDy * dblDy <= dblRadioCuadrado)
                {
                    return entI;
                }
            }

            return -1;
        }

        // Evalua rapidamente la curva con Bernstein para dibujo interactivo fluido
        public clsResultadoEvaluacion EvaluarBernsteinRapido(int entMuestras)
        {
            return objEvaluadorBernstein.Evaluar(objCurva.LstPuntosControl, entMuestras);
        }

        // Evalua rapidamente la curva con De Casteljau para dibujo interactivo fluido
        public clsResultadoEvaluacion EvaluarDeCasteljauRapido(int entMuestras)
        {
            return objEvaluadorDeCasteljau.Evaluar(objCurva.LstPuntosControl, entMuestras);
        }

        // Mide formalmente ambos metodos con Stopwatch (10 repeticiones + calentamiento)
        public void MedirYComparar(
            int entMuestras,
            out clsResultadoEvaluacion objResBernstein,
            out clsResultadoEvaluacion objResCasteljau,
            out clsResultadoComparacion objResComparacion)
        {
            List<clsPuntoControl> lstPuntos = objCurva.LstPuntosControl;
            objResBernstein = objMedidor.Medir(objEvaluadorBernstein, lstPuntos, entMuestras, 10);
            objResCasteljau = objMedidor.Medir(objEvaluadorDeCasteljau, lstPuntos, entMuestras, 10);
            objResComparacion = objComparador.Comparar(objResBernstein.ArrPuntosCurva, objResCasteljau.ArrPuntosCurva);
        }

        // Obtiene los poligonos intermedios de De Casteljau para un parametro t fijo
        public List<List<clsPuntoControl>> ObtenerInterpolaciones(double dblT)
        {
            return objEvaluadorDeCasteljau.ObtenerNivelesIntermedios(objCurva.LstPuntosControl, dblT);
        }
    }
}
