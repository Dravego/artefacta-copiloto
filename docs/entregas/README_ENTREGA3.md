# ARTEFACTA Copiloto — Entrega 3

## Objetivo

Añadir la primera interfaz conversacional del Copiloto sin depender todavía de
un modelo de IA externo.

La intención de esta fase es validar que las respuestas comerciales son
correctas, trazables y derivadas de Oracle antes de permitir lenguaje natural
más abierto.

## Nueva ruta

`/Asistente`

También aparece **Asistente ✦** en el menú principal.

## Preguntas soportadas

Ejemplos:

- ¿Qué clientes debo contactar hoy?
- ¿Cuánto hemos vendido este mes?
- ¿Quiénes son los mejores vendedores?
- ¿Cuáles son los productos más vendidos?
- Muéstrame las recomendaciones pendientes.
- ¿Qué le puedo ofrecer a Sofía?
- Analiza cliente CLI000078.
- Historial de Luis González.

## Cómo funciona

`CopilotoConversacionalService`:

1. normaliza la pregunta;
2. identifica una intención comercial;
3. ejecuta solamente consultas EF Core conocidas;
4. devuelve texto + tarjetas con datos;
5. nunca genera SQL libre;
6. nunca inventa resultados fuera de Oracle.

## Intenciones de esta entrega

- OPORTUNIDADES
- VENTAS_MES
- TOP_VENDEDORES
- TOP_PRODUCTOS
- RECOMENDACIONES
- CLIENTE
- CLIENTE_AMBIGUO
- NO_RECONOCIDA

## Búsqueda de clientes

El motor intenta detectar:
- código de cliente;
- RFC;
- palabras significativas del nombre.

Si varias coincidencias son parecidas, el sistema NO adivina: muestra las
opciones para que el vendedor seleccione el cliente correcto.

## Prueba sugerida

1. Ejecuta el proyecto con la misma cadena Oracle validada.
2. Abre `/Asistente`.
3. Pulsa "¿Qué clientes debo contactar hoy?".
4. Después pregunta "¿Cuánto hemos vendido este mes?".
5. Prueba con un nombre que aparezca en `/Oportunidades`, por ejemplo:
   `¿Qué le puedo ofrecer a <nombre del cliente>?`
6. Haz clic en una tarjeta para abrir la ficha inteligente correspondiente.

## Próxima fase

La siguiente entrega puede incorporar un LLM como capa de comprensión de
lenguaje natural, pero manteniendo herramientas/servicios controlados para
consultar Oracle. El modelo no debería recibir permiso para generar SQL
arbitrario contra producción.
