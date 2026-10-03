# Notebook de analítica y Machine Learning

**Archivo:** `ARTEFACTA_Oracle_ETL_EDA_ML.ipynb` (versión ejecutada, con resultados visibles).

## Contenido

1. **Carga y reconocimiento** del dataset exportado de Oracle.
2. **ETL y controles de calidad:** tipos, fechas, nulos, duplicados y reglas aritméticas (`IMPORTE` y `TOTAL`).
3. **EDA:** evolución mensual de ingresos, categorías, productos, vendedores y distribución del ticket.
4. **Modelo supervisado:** propensión de recompra a 90 días (línea base, regresión logística y Random Forest), con separación temporal entrenamiento/prueba para evitar fuga de información.
5. **Modelo no supervisado:** segmentación de clientes con variables RFM y de comportamiento mediante K-Means, con selección de K por Silhouette y Davies-Bouldin.
6. **Conclusiones**, criterio de puesta en producción, gobernanza y limitaciones.

## Ejecución

Requiere `DATASET_ARTEFACTA.csv`, incluido en la carpeta `data/` (ver [`data/README.md`](../data/README.md)) y las librerías de [`requirements-ml.txt`](../requirements-ml.txt).
