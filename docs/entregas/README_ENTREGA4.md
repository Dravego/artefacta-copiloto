# ARTEFACTA Copiloto — Entrega 4
## Login, roles, permisos y contexto de vendedor

Esta entrega incorpora autenticación por cookie usando las tablas existentes:

- `USUARIOS`
- `ROLES`
- `USUARIO_ROL`
- `VENDEDORES`

No se agregó una segunda estructura de seguridad: la aplicación usa directamente
el modelo Oracle del proyecto.

## 1. Primer acceso

Al iniciar la aplicación, cualquier ruta protegida redirige a:

`/Cuenta/Login`

Si no existe un usuario activo con rol `ADMIN`, aparece el enlace:

**Crear primer administrador**

Ruta:

`/Cuenta/InicializarAdministrador`

La contraseña se guarda con `PasswordHasher<Usuario>` de ASP.NET Core.
No se almacena texto plano.

Una vez creado un ADMIN activo, la pantalla de inicialización deja de estar disponible.

## 2. Roles

Se utilizan exactamente los roles que ya existen en Oracle:

- ADMIN
- GERENTE
- SUPERVISOR
- VENDEDOR
- CONSULTA

### ADMIN
- acceso completo;
- puede abrir Administración;
- puede cambiar roles y activar/desactivar usuarios.

### GERENTE
- acceso comercial global;
- puede generar y operar recomendaciones.

### SUPERVISOR
- acceso comercial global;
- puede generar y operar recomendaciones.

### VENDEDOR
- acceso comercial;
- puede operar recomendaciones;
- en oportunidades, si `USUARIOS.VENDEDOR_ID` está asociado, se filtran clientes
  con ventas realizadas por ese vendedor.

### CONSULTA
- acceso de lectura a información comercial;
- no puede ejecutar operaciones POST que cambien recomendaciones.

## 3. Administración de roles

Sólo ADMIN:

`/Administracion/Usuarios`

Desde allí:
- consultar usuarios;
- ver vendedor asociado;
- ver roles;
- activar/inactivar;
- asignar o retirar roles.

Hay una protección para evitar retirar/desactivar al último ADMIN activo.

## 4. Inicio y cierre de sesión

La aplicación usa cookie HttpOnly:

`Artefacta.Copiloto.Auth`

El usuario puede salir desde el menú superior.

## 5. Importante sobre usuarios preexistentes

El login de esta entrega valida contraseñas creadas con el hasher de ASP.NET Core.

Si tu dataset ya contiene usuarios con `PASSWORD_HASH` generado con otro algoritmo,
esos usuarios no podrán autenticarse hasta que definamos la estrategia de migración
de hashes. El primer ADMIN creado desde la aplicación sí funcionará.

## 6. Prueba recomendada

1. Ejecuta la aplicación.
2. Debe enviarte a `/Cuenta/Login`.
3. Si aún no existe ADMIN, pulsa **Crear primer administrador**.
4. Crea una contraseña de al menos 10 caracteres.
5. Inicia sesión.
6. Confirma que aparece **Administración** en el menú.
7. Abre `/Administracion/Usuarios`.
8. Comprueba los roles existentes.
9. Abre `/Asistente` y `/Oportunidades`.
10. Cierra sesión y confirma que una ruta protegida regresa al Login.

## 7. Siguiente entrega sugerida

- alta/edición completa de usuarios;
- asociación visual usuario ↔ vendedor;
- cambio de contraseña;
- recuperación/reset administrado;
- auditoría de sesiones y acciones;
- IA real mediante herramientas controladas.
