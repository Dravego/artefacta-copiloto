# Datos

El notebook utiliza el archivo **`DATASET_ARTEFACTA.csv`**, exportado de una vista Oracle del proyecto ARTEFACTA.

- **Granularidad:** una fila por línea de venta (una misma `VENTA_ID` puede tener varias filas).
- **Tamaño:** 132,766 filas y 20 columnas.
- **Periodo:** 27/08/2024 a 27/08/2026.
- **Cobertura:** 50,000 ventas, 1,000 clientes, 500 productos, 50 vendedores y 50 categorías.

**Columnas:** `VENTA_ID`, `FECHA_VENTA` (dd/mm/aaaa), `AÑO`, `MES`, `CLIENTE_ID`, `CLIENTE`, `VENDEDOR_ID`, `VENDEDOR`, `ITEM`, `PRODUCTO_ID`, `PRODUCTO`, `CATEGORIA_ID`, `SKU`, `CATEGORIA`, `CANTIDAD`, `PRECIO_UNITARIO`, `DESCUENTO`, `IMPORTE`, `IMPUESTOS`, `TOTAL`.

## Cómo usarlo

- **Jupyter local:** no hay que mover nada; el notebook busca el archivo en esta carpeta (`data/`).
- **Google Colab:** descarga `DATASET_ARTEFACTA.csv` desde esta carpeta y súbelo al panel **Files** (ícono de carpeta a la izquierda) antes de ejecutar las celdas.
