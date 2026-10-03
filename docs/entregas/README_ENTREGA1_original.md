# ARTEFACTA Copiloto — Entrega 1

Primera versión funcional del código fuente para conectar **ASP.NET Core / .NET 10** con la base de datos **Oracle 19c** del proyecto ARTEFACTA.

## Incluye

- Mapeo de las 15 tablas del esquema mediante Entity Framework Core.
- Manejo de las secuencias Oracle definidas en el esquema.
- Navegación entre Clientes, Productos y Ventas.
- Pantalla inicial con indicadores.
- Primera pantalla de Copiloto por cliente:
  - total histórico,
  - última compra,
  - productos frecuentes desde `CLIENTE_PRODUCTO`,
  - recomendaciones pendientes desde `RECOMENDACIONES`.
- Endpoint `/Copiloto/PruebaConexion` para comprobar Oracle y contar CLIENTES, PRODUCTOS y VENTAS.

## Requisitos

1. Visual Studio con soporte para ASP.NET Core y .NET 10 SDK.
2. Acceso a la instancia Oracle 19c que contiene las tablas y el dataset.
3. Restaurar paquetes NuGet al abrir la solución.

## Configurar la conexión

Editar `appsettings.json`:

```json
"OracleConnection": "User Id=ARTEFACTA;Password=TU_PASSWORD;Data Source=localhost:1521/ORCLPDB1"
```

Ajusta `User Id`, `Password`, host, puerto y service name a tu instalación.

Para no guardar la contraseña en el archivo, durante desarrollo puedes usar User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=ARTEFACTA;Password=TU_PASSWORD;Data Source=localhost:1521/ORCLPDB1"
```

## Ejecutar

1. Abrir `Artefacta.Copiloto.sln`.
2. Restaurar NuGet.
3. Verificar `appsettings.json`.
4. Ejecutar con F5.
5. Abrir `/Copiloto/PruebaConexion`.

## Importante

Este proyecto usa la base existente. **No ejecutes `Database.Migrate()` ni `EnsureCreated()`**, porque el esquema Oracle ya fue creado y ya contiene datos.

## Próxima entrega sugerida

- Autenticación de usuarios y roles.
- Operación de recomendaciones (contactar, aceptar, rechazar).
- Motor de reglas para generar nuevas recomendaciones.
- Chat de Copiloto con consultas comerciales en lenguaje natural.
- Módulo Almacén cuando tengamos la estructura de existencias/disponibilidad, ya que no aparece en el esquema SQL actual.


## Diagnóstico de conexión Oracle

La prueba de conexión ahora abre la conexión explícitamente. Si falla, la URL
`/Copiloto/PruebaConexion` mostrará el mensaje y código ORA reales (por ejemplo
ORA-12514, ORA-01017, ORA-12541), lo que permite identificar si el problema es
Service Name, credenciales, listener o red.


## Entrega 2

La solución incluye ahora dashboard comercial, ficha inteligente de cliente,
motor determinístico de oportunidades de recompra, generación de recomendaciones
y gestión de estados con historial.

Consulta `README_ENTREGA2.md` para las pruebas.


## Entrega 3

Se añadió `/Asistente`, una interfaz conversacional determinística sobre Oracle.
Permite preguntar por oportunidades, ventas del mes, vendedores, productos,
recomendaciones y clientes. Ver `README_ENTREGA3.md`.


## Entrega 4

Se incorporó autenticación por cookie, primer administrador, roles Oracle,
administración de permisos y contexto básico por vendedor.
Ver `README_ENTREGA4.md`.


## Entrega 4 final

El valor `$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR` se utiliza como estado
controlado de contraseña pendiente. Al detectarlo en el Login, la aplicación
obliga a establecer una contraseña nueva y la guarda con PasswordHasher de
ASP.NET Core. Ver `README_ENTREGA4_FINAL.md`.


## Entrega 5

Administración completa básica de usuarios: alta, edición, roles, asociación
Usuario ↔ Vendedor, reset de contraseña y cambio de contraseña propio.
Ver `README_ENTREGA5.md`.


## Entrega 6

Se agregó Comprar productos, con recomendaciones personalizadas por recompra,
venta cruzada, popularidad y promociones activas.
Ver `README_ENTREGA6.md`.
