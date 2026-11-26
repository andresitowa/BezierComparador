using System.Collections.Generic;

namespace BezierComparador
{
    // Contrato comun para los algoritmos que evaluan curvas de Bezier
    // Lo implementan clsEvaluadorBernstein y clsEvaluadorDeCasteljau
    public interface IEvaluadorCurva
    {
        // Nombre del metodo, ejemplo: "Bernstein"
        string CadNombre { get; }

        // Calcula entMuestras puntos de la curva con t = k / (entMuestras - 1)
        // Requiere al menos 2 puntos de control y 2 muestras
        clsResultadoEvaluacion Evaluar(List<clsPuntoControl> lstPuntos, int entMuestras);
    }
}
