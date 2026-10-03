# Corrección del flujo de cambio inicial de contraseña

## Problema detectado

El Login validaba `ModelState` antes de comprobar si el usuario tenía el valor:

`$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR`

en `PASSWORD_HASH`.

Como el campo Contraseña es obligatorio, si se dejaba vacío el flujo podía detenerse
antes de ejecutar la redirección al cambio obligatorio. Además, la comparación del
hash ahora se realiza en C# después de leerlo de Oracle y aplicar `Trim()`.

## Flujo corregido

1. El usuario escribe solamente su username, por ejemplo `admin`.
2. El POST del Login consulta primero si el hash es el valor centinela.
3. Si lo es, redirige inmediatamente a `CambiarPasswordInicial`.
4. No importa si el campo contraseña está vacío.
5. El usuario define una nueva contraseña.
6. Se genera y almacena el hash ASP.NET Core.
7. Luego inicia sesión normalmente.

## Diagnóstico temporal

Para comprobar que la aplicación detecta correctamente el centinela:

`/Cuenta/DiagnosticoPassword?username=admin`

Debe devolver:

`Usuario=admin | CambioPasswordPendiente=SI`

No muestra el hash ni información sensible.
