# BezierComparador

Aplicación de escritorio en **C# (Windows Forms)** que compara cuantitativa y visualmente la evaluación de curvas de Bézier mediante **Polinomios de Bernstein** frente al **Algoritmo de De Casteljau**, analizando rendimiento temporal (`Stopwatch`), diferencia numérica punto a punto y estabilidad en grados elevados ($n = 50$ y $n = 100$) contra referencia `decimal` de 128 bits.

> **Restricción académica cumplida:** Toda curva y gráfico comparativo se traza exclusivamente mediante segmentos de recta (`Graphics.DrawLine`), sin utilizar `DrawBezier`, `DrawCurve` ni librerías externas de gráficos.

## Estructura del Repositorio

```text
BezierComparador/
├── BezierComparador.sln
├── src/
│   ├── BezierComparador/                 # Aplicación Windows Forms (MVC)
│   │   ├── Modelo/                       # Evaluadores Bernstein, De Casteljau, Medidor, Comparador
│   │   ├── Controlador/                  # clsControladorCurva, clsControladorPruebas
│   │   └── Vista/                        # frmPrincipal, ctlLienzo, frmPruebas, clsTemaUI
│   └── BezierComparador.PruebaManual/    # Consola de validación matemática paso a paso
└── informe/
    ├── secciones/                        # Secciones LaTeX (01 a 08)
    └── figuras/                          # Capturas y gráficos PNG del informe
```

## Requisitos y Compilación

- **Visual Studio 2022 / 2026** o **.NET SDK 8.0 / 9.0 / 10.0** (detección automática en `.csproj`).
- Windows 10 / 11 (x64).

```bash
# Compilar toda la solución
dotnet build BezierComparador.sln

# Ejecutar la aplicación interactiva
dotnet run --project src/BezierComparador/BezierComparador.csproj

# Ejecutar la validación matemática por consola
dotnet run --project src/BezierComparador.PruebaManual/BezierComparador.PruebaManual.csproj
```

## Uso Rápido

### Ventana Principal (`frmPrincipal`)
- **Clic izquierdo en el lienzo:** agrega un punto de control o selecciona uno existente.
- **Arrastrar punto:** deforma la curva en tiempo real y recalcula las métricas al soltar.
- **Clic derecho sobre un punto:** elimina el punto de control.
- **Panel derecho:** ajusta la cantidad de muestras $N$, activa/desactiva capas (Bernstein, De Casteljau, interpolaciones, polígono), controla el parámetro $t \in [0, 1]$ y mide con `Stopwatch` (10 repeticiones + calentamiento JIT).

### Laboratorio de Pruebas (`frmPruebas`)
- Ejecuta automáticamente las **24 combinaciones** de grado $n \in \{2, 3, 5, 10, 20, 50\}$ y muestras $N \in \{100, 1000, 10000, 100000\}$.
- Ejecuta la prueba de **estabilidad numérica** en grados $n = 50$ y $n = 100$ frente a referencia `decimal` de 128 bits.
- Exporta resultados a **CSV** y gráficos comparativos a **PNG** (`800×480`) para el informe en LaTeX.
