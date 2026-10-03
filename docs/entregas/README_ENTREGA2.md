# ARTEFACTA Copiloto — Entrega 2

Esta entrega amplía la versión que ya confirmó conexión correcta con Oracle
(`CLIENTES=1,000`, `PRODUCTOS=500`, `VENTAS=50,000`).

## Nuevas funciones

### 1. Dashboard comercial
- Clientes activos.
- Productos activos.
- Ventas del mes.
- Total vendido del mes.
- Ticket promedio.
- Recomendaciones pendientes.
- Top 5 vendedores del mes.
- Top 5 productos del mes.

### 2. Ficha inteligente del cliente
- Número de ventas.
- Total histórico.
- Ticket promedio.
- Última compra.
- Días sin comprar.
- Productos frecuentes.
- Últimas 10 ventas.
- Recomendaciones activas.

### 3. Motor de oportunidades
Ruta:

`/Oportunidades`

La primera regla busca combinaciones `CLIENTE_PRODUCTO` con:
- al menos 2 compras;
- `ULTIMA_COMPRA` informada;
- `PROMEDIO_DIAS_COMPRA` informado;
- cliente y producto activos.

Se considera oportunidad cuando:

`días desde última compra > promedio histórico de días de compra`

El score se calcula de forma determinística de 0 a 100:
- 60% atraso relativo;
- 25% recurrencia;
- 15% valor histórico.

Prioridades:
- 80 a 100: CRITICA
- 65 a 79.99: ALTA
- 45 a 64.99: MEDIA
- menor de 45: BAJA

### 4. Generación de recomendaciones
Desde `/Oportunidades`, el botón **Generar recomendaciones** crea registros
en `RECOMENDACIONES` con `TIPO = RECOMPRA`.

No duplica una recomendación si ya existe para el mismo cliente + producto
con tipo `RECOMPRA` y estatus `PENDIENTE` o `CONTACTADO`.

### 5. Gestión de recomendaciones
Ruta:

`/Recomendaciones`

Se puede cambiar el estado a:
- CONTACTADO
- ACEPTADA
- RECHAZADA

Cada cambio agrega un registro en `HISTORIAL_RECOMENDACION`.

## Cómo probar esta entrega

1. Conserva la misma cadena Oracle que ya te funcionó.
2. Abre `Artefacta.Copiloto.sln`.
3. Restaura NuGet.
4. Ejecuta con F5.
5. Primero abre `/` para ver el dashboard.
6. Abre `/Oportunidades`.
7. Revisa si detecta oportunidades.
8. Presiona **Generar recomendaciones**.
9. Abre `/Recomendaciones`.
10. Cambia una recomendación a CONTACTADO y confirma que se actualiza.
11. Entra a `/Copiloto`, selecciona un cliente y revisa su ficha.

## Importante

Esta entrega todavía NO utiliza un modelo de IA externo.

El objetivo es validar primero la lógica comercial con datos reales y reglas
determinísticas. Después añadiremos una capa conversacional para que el vendedor
pregunte en lenguaje natural, sin perder trazabilidad sobre los datos.

## Posible incidencia

Si `/Oportunidades` aparece vacío, es probable que `CLIENTE_PRODUCTO` no esté
poblada o que `PROMEDIO_DIAS_COMPRA` esté nulo. En ese caso, ejecuta:

```sql
SELECT COUNT(*) FROM CLIENTE_PRODUCTO;

SELECT COUNT(*)
FROM CLIENTE_PRODUCTO
WHERE NUM_COMPRAS >= 2
  AND ULTIMA_COMPRA IS NOT NULL
  AND PROMEDIO_DIAS_COMPRA IS NOT NULL;
```

Si el segundo conteo es 0, la siguiente tarea será construir el proceso de
actualización de `CLIENTE_PRODUCTO` a partir de `VENTAS` y `DETALLE_VENTA`.
