# ARTEFACTA Copiloto — Entrega 6
## Comprar productos + recomendaciones personalizadas

Se añadió una nueva opción en el menú:

`Comprar`

Ruta:

`/Comprar`

## Flujo

1. El vendedor busca y selecciona un cliente.
2. El sistema analiza su historial.
3. Muestra tres grupos de productos:
   - recompra;
   - venta cruzada;
   - productos populares.
4. Si un producto tiene una promoción activa asociada mediante
   `PROMOCION_PRODUCTO`, la promoción se muestra en la tarjeta.

## 1. Recompra

Utiliza `CLIENTE_PRODUCTO`:

- NUM_COMPRAS
- ULTIMA_COMPRA
- PROMEDIO_DIAS_COMPRA
- IMPORTE_TOTAL

El score combina:
- frecuencia de compra;
- atraso respecto al ciclo esperado;
- valor histórico.

## 2. Venta cruzada

Busca categorías que el cliente ya compra y selecciona productos activos de
esas categorías que el cliente todavía no tiene en `CLIENTE_PRODUCTO`.

Se priorizan por ventas e importe de los últimos 12 meses.

## 3. Productos populares

Si hacen falta alternativas, se muestran productos de alta venta durante los
últimos 6 meses, evitando repetir los productos ya mostrados.

## 4. Promociones

Se consultan:
- PROMOCIONES
- PROMOCION_PRODUCTO

Sólo se muestran promociones:
- activas;
- cuya fecha actual esté entre FECHA_INICIO y FECHA_FIN.

## 5. Seguridad por vendedor

Un usuario con rol VENDEDOR y VENDEDOR_ID asociado sólo puede seleccionar
clientes con los que tenga ventas previas.

ADMIN, GERENTE, SUPERVISOR y CONSULTA conservan visión global.

## Importante

Esta entrega todavía NO crea una venta ni descuenta inventario.

La pantalla funciona como una experiencia de "compra asistida": ayuda al
vendedor a decidir qué ofrecerle al cliente.

## Siguiente paso recomendado

Entrega 7:
- carrito;
- cantidades;
- cálculo de subtotal/descuento/impuesto/total;
- confirmación;
- creación de VENTAS + DETALLE_VENTA;
- folio;
- validación de promociones;
- posteriormente inventario/almacén.
