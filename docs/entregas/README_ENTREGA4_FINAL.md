# ARTEFACTA Copiloto — Entrega 4 final

## Cambio obligatorio / recuperación controlada de contraseña

El dataset contiene usuarios como:

- admin
- gerente
- supervisor
- vendedor
- consulta

con el valor:

`$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR`

en `USUARIOS.PASSWORD_HASH`.

La aplicación interpreta ese valor como un **estado especial de contraseña
pendiente**, no como un hash válido.

## Flujo

1. El usuario escribe su `USERNAME` en `/Cuenta/Login`.
2. Si está activo y `PASSWORD_HASH` es exactamente el valor centinela,
   la contraseña escrita en el login se ignora.
3. El sistema redirige a `/Cuenta/CambiarPasswordInicial`.
4. El usuario define una contraseña nueva de mínimo 10 caracteres.
5. ASP.NET Core genera el hash mediante `PasswordHasher<Usuario>`.
6. El hash reemplaza el valor centinela en Oracle.
7. El usuario vuelve al login y entra normalmente.

## Recuperación manual futura

Si un administrador de base de datos necesita forzar nuevamente un cambio
de contraseña:

```sql
UPDATE USUARIOS
SET PASSWORD_HASH = '$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR'
WHERE USERNAME = 'admin';

COMMIT;
```

En el siguiente intento de acceso, ARTEFACTA Copiloto obligará al usuario a
definir una nueva contraseña.

## Consideración de seguridad

El valor centinela funciona como un mecanismo administrativo de reset.
Mientras un usuario tenga ese valor, cualquier persona que conozca su USERNAME
podría intentar definir la nueva contraseña desde la aplicación.

Por ello:
- úsalo únicamente como procedimiento administrativo controlado;
- no dejes una cuenta con el centinela más tiempo del necesario;
- en producción conviene sustituirlo por tokens de recuperación de un solo uso,
  caducidad y auditoría.

## Prueba

Para `admin`, deja:

```sql
UPDATE USUARIOS
SET PASSWORD_HASH = '$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR'
WHERE USERNAME = 'admin';

COMMIT;
```

Después:

1. abre `/Cuenta/Login`;
2. escribe `admin`;
3. en contraseña puedes escribir cualquier texto temporal;
4. pulsa Entrar;
5. debe aparecer **Define tu contraseña**;
6. introduce dos veces la contraseña definitiva;
7. regresa al Login;
8. inicia sesión con la nueva contraseña;
9. confirma que aparece el menú **Administración**.
