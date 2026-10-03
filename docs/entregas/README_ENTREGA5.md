# ARTEFACTA Copiloto — Entrega 5
## Administración de cuentas y asociación Usuario ↔ Vendedor

Esta entrega completa la administración básica de usuarios sobre las tablas
existentes `USUARIOS`, `ROLES`, `USUARIO_ROL` y `VENDEDORES`.

## Funciones nuevas

### Nuevo usuario
Ruta:

`/Administracion/Nuevo`

El ADMIN puede definir:
- username;
- nombre;
- correo;
- vendedor asociado;
- estado activo/inactivo;
- uno o varios roles.

El usuario nuevo se crea con:

`$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR`

por lo que en su primer acceso deberá definir su contraseña.

### Editar usuario
Desde:

`/Administracion/Usuarios`

se puede abrir **Editar** y modificar:
- username;
- nombre;
- correo;
- vendedor asociado;
- roles;
- estado.

Se mantiene la protección del último ADMIN activo.

### Reset de contraseña
El botón **Reset password** vuelve a colocar el valor centinela en
`PASSWORD_HASH`.

En el siguiente acceso, el usuario deberá definir una nueva contraseña.

### Cambio de contraseña del usuario autenticado
Al pulsar el nombre del usuario en la barra superior se abre:

`/Cuenta/CambiarMiPassword`

Solicita:
- contraseña actual;
- nueva contraseña;
- confirmación.

## Contexto del vendedor

Para cuentas con rol `VENDEDOR`, se recomienda asociar siempre
`USUARIOS.VENDEDOR_ID`.

El motor de oportunidades ya utiliza esta asociación para limitar la visión
comercial del vendedor.

Ejemplo:

Usuario `vendedor`
→ VENDEDOR_ID = 14
→ oportunidades relacionadas con clientes a los que el vendedor 14 ha vendido.

## Prueba sugerida

1. Inicia sesión como ADMIN.
2. Abre Administración → Usuarios.
3. Crea un nuevo usuario de prueba.
4. Asígnale rol VENDEDOR.
5. Selecciona un vendedor real.
6. Cierra sesión.
7. Entra con el nuevo username.
8. Debe pedir definir contraseña.
9. Inicia sesión con la contraseña recién creada.
10. Confirma que no aparece Administración.
11. Revisa Oportunidades para verificar el contexto del vendedor.
12. Pulsa el nombre del usuario y prueba Cambiar contraseña.

## Próxima etapa sugerida

Entrega 6:
- auditoría de sesiones y acciones;
- permisos más finos por módulo;
- promociones dentro de las recomendaciones;
- preparación de la capa de IA mediante herramientas controladas.
