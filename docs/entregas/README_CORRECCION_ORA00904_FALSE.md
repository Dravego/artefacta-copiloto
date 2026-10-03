# Corrección ORA-00904: "FALSE": identificador no válido

## Causa

Oracle 19c no admite los literales SQL booleanos `TRUE` y `FALSE`.

En determinadas operaciones, especialmente `AnyAsync()`, el proveedor de
Entity Framework Core para Oracle puede generar internamente una expresión
escalar semejante a:

```sql
CASE
    WHEN EXISTS (...) THEN TRUE
    ELSE FALSE
END
```

En Oracle 19c, `FALSE` se interpreta como si fuera el nombre de una columna,
por lo que produce:

`ORA-00904: "FALSE": identificador no válido`

## Corrección

Se sustituyeron las comprobaciones escalares `AnyAsync()` por:

```csharp
var cantidad = await consulta.CountAsync(...);
return cantidad > 0;
```

El SQL resultante usa `COUNT(*)`, que es plenamente compatible con Oracle 19c.

Se corrigieron:
- `AutenticacionService.ExisteAdminActivoAsync()`
- comprobación de nombre de usuario existente
- comprobación de recomendaciones duplicadas

No es necesario modificar la base de datos.
