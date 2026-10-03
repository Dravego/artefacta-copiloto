# ARTEFACTA Copiloto Comercial Inteligente

Proyecto integrador de maestría. Aplicación web de apoyo a la decisión comercial para una empresa de retail, acompañada de un notebook de analítica y Machine Learning.

## Integrantes

- Gerardo Sol
- Leonardo Venuti
- José Vicente Solís

## Caso organizacional

El caso aborda la gestión de inventario de una empresa de retail con un volumen amplio de productos y transacciones. El problema de negocio es anticipar el comportamiento de la demanda para reducir el riesgo de faltantes y excedentes y mejorar las decisiones de reposición.

El proyecto utiliza analítica y ML **como apoyo a la decisión, no como sustituto del responsable comercial**.

## Pregunta de negocio

> **¿Cuándo y cuánto reponer?**

Para apoyar esa decisión, el proyecto responde dos preguntas analíticas:

1. **Predicción:** ¿Qué clientes tienen mayor propensión a recomprar en 90 días?
2. **Segmentación:** ¿Qué clientes presentan comportamientos de compra similares?

### Alcance del ML (aclaración importante)

El planteamiento inicial del caso era la **predicción de demanda por unidades**. El notebook ejecutado reformula el componente de ML a nivel **cliente**, en dos capacidades:

| Capacidad | Tipo de problema | Salida |
|---|---|---|
| Propensión de recompra a 90 días | Clasificación supervisada | Probabilidad de que el cliente vuelva a comprar |
| Segmentación de clientes | Agrupamiento (no supervisado) | Segmentos por recencia, frecuencia, valor monetario y comportamiento (RFM) |

Estas salidas son una señal **indirecta** de demanda: indican quién es probable que compre y con qué valor, pero no estiman unidades por producto. La predicción de demanda por unidades queda como línea de trabajo futura.

## Estructura del repositorio

```
artefacta-copiloto/
├── README.md                 ← este archivo
├── .gitignore
├── app/                      ← aplicación web (ASP.NET Core MVC, .NET 10)
│   ├── Artefacta.Copiloto.sln
│   ├── Artefacta.Copiloto.csproj
│   ├── Program.cs            ← configuración e inicio de la aplicación
│   ├── appsettings.json      ← cadena de conexión a Oracle (editar)
│   ├── Controllers/          ← rutas y lógica de cada pantalla
│   ├── Services/             ← reglas de negocio (oportunidades, recomendaciones, copiloto)
│   ├── Data/                 ← contexto de Entity Framework Core (mapeo de tablas)
│   ├── Models/               ← entidades del esquema Oracle
│   ├── DTOs/ ViewModels/     ← objetos de transferencia y de vista
│   ├── Security/             ← usuario actual y roles
│   ├── Views/                ← pantallas (Razor)
│   └── wwwroot/              ← estilos
├── database/                 ← scripts SQL para Oracle 19c
│   ├── sistema_ventas_oracle19c-1.sql   ← creación del esquema (tablas, secuencias, roles)
│   ├── prueba_consultas.sql             ← validación del dataset
│   └── prueba_entrega2.sql              ← validación de recomendaciones
├── notebooks/
│   └── ARTEFACTA_Oracle_ETL_EDA_ML.ipynb  ← ETL, EDA y ML (propensión + segmentación)
├── data/
│   └── DATASET_ARTEFACTA.csv   ← dataset exportado de Oracle (usado por el notebook)
├── requirements-ml.txt       ← librerías de Python para el notebook
└── docs/
    └── entregas/             ← notas técnicas de cada entrega parcial
```

## Resultados principales del notebook

Dataset: 132,766 líneas de venta (50,000 ventas, 1,000 clientes, 500 productos) del 27/08/2024 al 27/08/2026. Los controles de calidad no encontraron nulos, duplicados ni inconsistencias aritméticas.

**Propensión de recompra a 90 días** (evaluación en el bloque temporal de prueba, feb–may 2026):

| Modelo | Accuracy | Precision | Recall | F1 | ROC-AUC |
|---|---|---|---|---|---|
| **Random Forest** | **0.948** | **0.953** | **0.987** | **0.970** | **0.974** |
| Regresión logística | 0.924 | 0.928 | 0.986 | 0.956 | 0.965 |
| Línea base (clase mayoritaria) | 0.837 | 0.837 | 1.000 | 0.911 | 0.500 |

La tasa de recompra a 90 días es alta (92% en el conjunto de modelado), por lo que el modelo se compara contra una línea base y se evalúa principalmente con ROC-AUC.

**Segmentación (K-Means, K = 2 por Silhouette 0.452):**

| Segmento | Clientes | Recencia media (días) | Frecuencia media | Lectura |
|---|---|---|---|---|
| 0 | 595 | 75.7 | 22.3 | Recencia alta, frecuencia y valor bajos |
| 1 | 405 | 7.5 | 90.6 | Recencia baja, frecuencia y valor altos |

**Limitación:** el dataset no incluye costos, margen, campañas ni la aceptación de recomendaciones, por lo que el modelo predice recompra observada, pero no demuestra que una recomendación del Copiloto cause una venta.

## Tablero en Power BI

Dashboard interactivo con los indicadores, la evolución de ventas, las categorías, los productos y el comportamiento de clientes, usado en la demostración del proyecto:

**[Abrir tablero de Power BI](https://app.powerbi.com/view?r=eyJrIjoiYzM3ZDAxMWQtNDMxNC00ZWYyLTg3Y2ItYjY0ZWVlZGZlYWZkIiwidCI6IjBmYjMyNzNkLWVjMjQtNDE5ZC1hMTllLWRlNDRjOWQ0OTBjNSJ9)**

No requiere instalar nada; se abre en el navegador.

## Funcionalidades de la aplicación

- **Dashboard comercial** con indicadores principales.
- **Clientes, Productos y Ventas:** consulta y navegación.
- **Copiloto por cliente:** total histórico, última compra, productos frecuentes y recomendaciones pendientes.
- **Oportunidades de recompra:** motor determinístico basado en el ciclo promedio de compra de cada cliente.
- **Recomendaciones:** generación y gestión de estados (contactar, aceptar, rechazar) con historial.
- **Asistente conversacional:** preguntas en lenguaje natural sobre ventas, clientes, productos y oportunidades.
- **Comprar productos:** sugerencias por recompra, venta cruzada, popularidad y promociones.
- **Seguridad:** inicio de sesión, roles (ADMIN, GERENTE, SUPERVISOR, VENDEDOR, CONSULTA) y administración de usuarios.

## Cómo ejecutar el proyecto

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (o Visual Studio con soporte para ASP.NET Core y .NET 10).
- Oracle Database 19c con un usuario/esquema para la aplicación (por ejemplo `ARTEFACTA`).
- Para el notebook: Python 3 y Jupyter (o Google Colab).

### 1. Preparar la base de datos

1. En Oracle, crear el usuario/esquema de la aplicación.
2. Ejecutar `database/sistema_ventas_oracle19c-1.sql` con ese usuario para crear tablas, secuencias y roles.
3. Cargar el dataset de ventas del proyecto.
4. Verificar la carga con `database/prueba_consultas.sql`.

### 2. Configurar la conexión

Editar `app/appsettings.json` y reemplazar los valores por los de tu instalación:

```json
"OracleConnection": "User Id=ARTEFACTA;Password=TU_PASSWORD;Data Source=localhost:1521/ORCL"
```

Para no dejar la contraseña en el archivo, se recomienda usar User Secrets:

```bash
cd app
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=ARTEFACTA;Password=TU_PASSWORD;Data Source=localhost:1521/ORCL"
```

### 3. Ejecutar la aplicación

**Opción A – Visual Studio:** abrir `app/Artefacta.Copiloto.sln`, restaurar paquetes NuGet y presionar **F5**.

**Opción B – Terminal:**

```bash
cd app
dotnet restore
dotnet run
```

Abrir en el navegador `http://localhost:5187`.

### 4. Primer acceso

1. La aplicación redirige a `/Cuenta/Login`.
2. Si no existe un administrador, usar **Crear primer administrador** (`/Cuenta/InicializarAdministrador`).
3. Los usuarios de demostración del dataset (`admin`, `gerente`, `supervisor`, `vendedor`, `consulta`) deben definir su contraseña en el primer inicio de sesión.
4. Para comprobar la conexión con Oracle: `/Copiloto/PruebaConexion`.

> **Importante:** la aplicación trabaja sobre un esquema existente. No ejecutar `Database.Migrate()` ni `EnsureCreated()`.

### 5. Ejecutar el notebook

**Opción A – Google Colab (sin instalar nada):**

1. Abrir el notebook en Colab: [ARTEFACTA_Oracle_ETL_EDA_ML](https://colab.research.google.com/drive/1mO5iOx4gJ0405YvF6M8EUFhv-L2Zxio3?usp=sharing).
2. Subir `DATASET_ARTEFACTA.csv` al panel **Files** (ícono de carpeta a la izquierda).
3. Menú **Entorno de ejecución → Ejecutar todas**.

**Opción B – Jupyter local:**

```bash
pip install -r requirements-ml.txt
jupyter notebook notebooks/ARTEFACTA_Oracle_ETL_EDA_ML.ipynb
```

El notebook encuentra automáticamente el archivo en `data/DATASET_ARTEFACTA.csv`. Ver [`data/README.md`](data/README.md).

## Tecnologías

ASP.NET Core MVC (.NET 10) · Entity Framework Core · Oracle 19c · Python · pandas · scikit-learn · matplotlib · Jupyter / Google Colab · Power BI
