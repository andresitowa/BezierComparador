using System.Collections.Generic;

namespace BezierComparador
{
    // Contrato para los algoritmos que evaluan curvas de Bezier
    public interface IEvaluadorCurva
    {
        string CadNombre { get; }
        clsResultadoEvaluacion Evaluar(List<clsPuntoControl> lstPuntos, int entMuestras);
    }
}
