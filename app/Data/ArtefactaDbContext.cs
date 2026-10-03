using Artefacta.Copiloto.Models;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Data;

public class ArtefactaDbContext(DbContextOptions<ArtefactaDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
    public DbSet<Promocion> Promociones => Set<Promocion>();
    public DbSet<PromocionProducto> PromocionProductos => Set<PromocionProducto>();
    public DbSet<PromocionCliente> PromocionClientes => Set<PromocionCliente>();
    public DbSet<ClienteProducto> ClienteProductos => Set<ClienteProducto>();
    public DbSet<Recomendacion> Recomendaciones => Set<Recomendacion>();
    public DbSet<HistorialRecomendacion> HistorialRecomendaciones => Set<HistorialRecomendacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarCliente(modelBuilder);
        ConfigurarCategoria(modelBuilder);
        ConfigurarProducto(modelBuilder);
        ConfigurarVendedor(modelBuilder);
        ConfigurarRol(modelBuilder);
        ConfigurarUsuario(modelBuilder);
        ConfigurarUsuarioRol(modelBuilder);
        ConfigurarVenta(modelBuilder);
        ConfigurarDetalleVenta(modelBuilder);
        ConfigurarPromocion(modelBuilder);
        ConfigurarPromocionProducto(modelBuilder);
        ConfigurarPromocionCliente(modelBuilder);
        ConfigurarClienteProducto(modelBuilder);
        ConfigurarRecomendacion(modelBuilder);
        ConfigurarHistorialRecomendacion(modelBuilder);
    }

    private static void ConfigurarCliente(ModelBuilder b)
    {
        var e = b.Entity<Cliente>();
        e.ToTable("CLIENTES");
        e.HasKey(x => x.ClienteId).HasName("PK_CLIENTES");
        e.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_CLIENTES.NEXTVAL");
        e.Property(x => x.CodigoCliente).HasColumnName("CODIGO_CLIENTE").HasMaxLength(30).IsRequired();
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(150).IsRequired();
        e.Property(x => x.RazonSocial).HasColumnName("RAZON_SOCIAL").HasMaxLength(200);
        e.Property(x => x.Rfc).HasColumnName("RFC").HasMaxLength(13);
        e.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(150);
        e.Property(x => x.Telefono).HasColumnName("TELEFONO").HasMaxLength(30);
        e.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
        e.HasIndex(x => x.CodigoCliente).IsUnique().HasDatabaseName("UK_CLIENTES_CODIGO");
        e.HasIndex(x => x.Rfc).IsUnique().HasDatabaseName("UK_CLIENTES_RFC");
    }

    private static void ConfigurarCategoria(ModelBuilder b)
    {
        var e = b.Entity<Categoria>();
        e.ToTable("CATEGORIAS");
        e.HasKey(x => x.CategoriaId).HasName("PK_CATEGORIAS");
        e.Property(x => x.CategoriaId).HasColumnName("CATEGORIA_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_CATEGORIAS.NEXTVAL");
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(100).IsRequired();
        e.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(500);
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
        e.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UK_CATEGORIAS_NOMBRE");
    }

    private static void ConfigurarProducto(ModelBuilder b)
    {
        var e = b.Entity<Producto>();
        e.ToTable("PRODUCTOS");
        e.HasKey(x => x.ProductoId).HasName("PK_PRODUCTOS");
        e.Property(x => x.ProductoId).HasColumnName("PRODUCTO_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_PRODUCTOS.NEXTVAL");
        e.Property(x => x.CodigoProducto).HasColumnName("CODIGO_PRODUCTO").HasMaxLength(50).IsRequired();
        e.Property(x => x.Sku).HasColumnName("SKU").HasMaxLength(80);
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(200).IsRequired();
        e.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(1000);
        e.Property(x => x.CategoriaId).HasColumnName("CATEGORIA_ID").HasPrecision(12);
        e.Property(x => x.Precio).HasColumnName("PRECIO").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Costo).HasColumnName("COSTO").HasPrecision(15, 2);
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
        e.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.HasIndex(x => x.CodigoProducto).IsUnique().HasDatabaseName("UK_PRODUCTOS_CODIGO");
        e.HasIndex(x => x.Sku).IsUnique().HasDatabaseName("UK_PRODUCTOS_SKU");
        e.HasOne(x => x.Categoria).WithMany(x => x.Productos).HasForeignKey(x => x.CategoriaId).HasConstraintName("FK_PRODUCTOS_CATEGORIA");
    }

    private static void ConfigurarVendedor(ModelBuilder b)
    {
        var e = b.Entity<Vendedor>();
        e.ToTable("VENDEDORES");
        e.HasKey(x => x.VendedorId).HasName("PK_VENDEDORES");
        e.Property(x => x.VendedorId).HasColumnName("VENDEDOR_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_VENDEDORES.NEXTVAL");
        e.Property(x => x.CodigoVendedor).HasColumnName("CODIGO_VENDEDOR").HasMaxLength(30).IsRequired();
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(150).IsRequired();
        e.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(150);
        e.Property(x => x.Telefono).HasColumnName("TELEFONO").HasMaxLength(30);
        e.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
        e.HasIndex(x => x.CodigoVendedor).IsUnique().HasDatabaseName("UK_VENDEDORES_CODIGO");
    }

    private static void ConfigurarRol(ModelBuilder b)
    {
        var e = b.Entity<Rol>();
        e.ToTable("ROLES");
        e.HasKey(x => x.RolId).HasName("PK_ROLES");
        e.Property(x => x.RolId).HasColumnName("ROL_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_ROLES.NEXTVAL");
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(50).IsRequired();
        e.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(250);
        e.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UK_ROLES_NOMBRE");
    }

    private static void ConfigurarUsuario(ModelBuilder b)
    {
        var e = b.Entity<Usuario>();
        e.ToTable("USUARIOS");
        e.HasKey(x => x.UsuarioId).HasName("PK_USUARIOS");
        e.Property(x => x.UsuarioId).HasColumnName("USUARIO_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_USUARIOS.NEXTVAL");
        e.Property(x => x.Username).HasColumnName("USERNAME").HasMaxLength(100).IsRequired();
        e.Property(x => x.PasswordHash).HasColumnName("PASSWORD_HASH").HasMaxLength(255).IsRequired();
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(150).IsRequired();
        e.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(150);
        e.Property(x => x.VendedorId).HasColumnName("VENDEDOR_ID").HasPrecision(12);
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
        e.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.Property(x => x.UltimoAcceso).HasColumnName("ULTIMO_ACCESO").HasColumnType("TIMESTAMP");
        e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("UK_USUARIOS_USERNAME");
        e.HasOne(x => x.Vendedor).WithMany(x => x.Usuarios).HasForeignKey(x => x.VendedorId).HasConstraintName("FK_USUARIOS_VENDEDOR");
    }

    private static void ConfigurarUsuarioRol(ModelBuilder b)
    {
        var e = b.Entity<UsuarioRol>();
        e.ToTable("USUARIO_ROL");
        e.HasKey(x => new { x.UsuarioId, x.RolId }).HasName("PK_USUARIO_ROL");
        e.Property(x => x.UsuarioId).HasColumnName("USUARIO_ID").HasPrecision(12);
        e.Property(x => x.RolId).HasColumnName("ROL_ID").HasPrecision(12);
        e.HasOne(x => x.Usuario).WithMany(x => x.Roles).HasForeignKey(x => x.UsuarioId).HasConstraintName("FK_USUARIO_ROL_USUARIO");
        e.HasOne(x => x.Rol).WithMany(x => x.Usuarios).HasForeignKey(x => x.RolId).HasConstraintName("FK_USUARIO_ROL_ROL");
    }

    private static void ConfigurarVenta(ModelBuilder b)
    {
        var e = b.Entity<Venta>();
        e.ToTable("VENTAS");
        e.HasKey(x => x.VentaId).HasName("PK_VENTAS");
        e.Property(x => x.VentaId).HasColumnName("VENTA_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_VENTAS.NEXTVAL");
        e.Property(x => x.Folio).HasColumnName("FOLIO").HasMaxLength(40).IsRequired();
        e.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").HasPrecision(12);
        e.Property(x => x.VendedorId).HasColumnName("VENDEDOR_ID").HasPrecision(12);
        e.Property(x => x.FechaVenta).HasColumnName("FECHA_VENTA").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.Property(x => x.Subtotal).HasColumnName("SUBTOTAL").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Descuento).HasColumnName("DESCUENTO").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Impuesto).HasColumnName("IMPUESTO").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Total).HasColumnName("TOTAL").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Estatus).HasColumnName("ESTATUS").HasMaxLength(20).HasDefaultValue("COMPLETADA").IsRequired();
        e.HasIndex(x => x.Folio).IsUnique().HasDatabaseName("UK_VENTAS_FOLIO");
        e.HasOne(x => x.Cliente).WithMany(x => x.Ventas).HasForeignKey(x => x.ClienteId).HasConstraintName("FK_VENTAS_CLIENTE");
        e.HasOne(x => x.Vendedor).WithMany(x => x.Ventas).HasForeignKey(x => x.VendedorId).HasConstraintName("FK_VENTAS_VENDEDOR");
    }

    private static void ConfigurarDetalleVenta(ModelBuilder b)
    {
        var e = b.Entity<DetalleVenta>();
        e.ToTable("DETALLE_VENTA");
        e.HasKey(x => x.DetalleVentaId).HasName("PK_DETALLE_VENTA");
        e.Property(x => x.DetalleVentaId).HasColumnName("DETALLE_VENTA_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_DETALLE_VENTA.NEXTVAL");
        e.Property(x => x.VentaId).HasColumnName("VENTA_ID").HasPrecision(12);
        e.Property(x => x.ProductoId).HasColumnName("PRODUCTO_ID").HasPrecision(12);
        e.Property(x => x.Cantidad).HasColumnName("CANTIDAD").HasPrecision(15, 4);
        e.Property(x => x.PrecioUnitario).HasColumnName("PRECIO_UNITARIO").HasPrecision(15, 2);
        e.Property(x => x.Descuento).HasColumnName("DESCUENTO").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Importe).HasColumnName("IMPORTE").HasPrecision(15, 2);
        e.HasOne(x => x.Venta).WithMany(x => x.Detalles).HasForeignKey(x => x.VentaId).HasConstraintName("FK_DETALLE_VENTA_VENTA");
        e.HasOne(x => x.Producto).WithMany(x => x.DetallesVenta).HasForeignKey(x => x.ProductoId).HasConstraintName("FK_DETALLE_VENTA_PRODUCTO");
    }

    private static void ConfigurarPromocion(ModelBuilder b)
    {
        var e = b.Entity<Promocion>();
        e.ToTable("PROMOCIONES");
        e.HasKey(x => x.PromocionId).HasName("PK_PROMOCIONES");
        e.Property(x => x.PromocionId).HasColumnName("PROMOCION_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_PROMOCIONES.NEXTVAL");
        e.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(150).IsRequired();
        e.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(1000);
        e.Property(x => x.Tipo).HasColumnName("TIPO").HasMaxLength(30).IsRequired();
        e.Property(x => x.Valor).HasColumnName("VALOR").HasPrecision(15, 2);
        e.Property(x => x.FechaInicio).HasColumnName("FECHA_INICIO").HasColumnType("DATE");
        e.Property(x => x.FechaFin).HasColumnName("FECHA_FIN").HasColumnType("DATE");
        e.Property(x => x.MinimoCompra).HasColumnName("MINIMO_COMPRA").HasPrecision(15, 2).HasDefaultValue(0m);
        e.Property(x => x.Activo).HasColumnName("ACTIVO").HasConversion<int>().HasDefaultValue(true);
    }

    private static void ConfigurarPromocionProducto(ModelBuilder b)
    {
        var e = b.Entity<PromocionProducto>();
        e.ToTable("PROMOCION_PRODUCTO");
        e.HasKey(x => new { x.PromocionId, x.ProductoId }).HasName("PK_PROMOCION_PRODUCTO");
        e.Property(x => x.PromocionId).HasColumnName("PROMOCION_ID").HasPrecision(12);
        e.Property(x => x.ProductoId).HasColumnName("PRODUCTO_ID").HasPrecision(12);
        e.HasOne(x => x.Promocion).WithMany(x => x.Productos).HasForeignKey(x => x.PromocionId).HasConstraintName("FK_PP_PROMOCION");
        e.HasOne(x => x.Producto).WithMany(x => x.Promociones).HasForeignKey(x => x.ProductoId).HasConstraintName("FK_PP_PRODUCTO");
    }

    private static void ConfigurarPromocionCliente(ModelBuilder b)
    {
        var e = b.Entity<PromocionCliente>();
        e.ToTable("PROMOCION_CLIENTE");
        e.HasKey(x => new { x.PromocionId, x.ClienteId }).HasName("PK_PROMOCION_CLIENTE");
        e.Property(x => x.PromocionId).HasColumnName("PROMOCION_ID").HasPrecision(12);
        e.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").HasPrecision(12);
        e.Property(x => x.FechaAsignacion).HasColumnName("FECHA_ASIGNACION").HasColumnType("DATE").HasDefaultValueSql("SYSDATE");
        e.Property(x => x.FechaExpiracion).HasColumnName("FECHA_EXPIRACION").HasColumnType("DATE");
        e.Property(x => x.Estatus).HasColumnName("ESTATUS").HasMaxLength(20).HasDefaultValue("PENDIENTE").IsRequired();
        e.HasOne(x => x.Promocion).WithMany(x => x.Clientes).HasForeignKey(x => x.PromocionId).HasConstraintName("FK_PC_PROMOCION");
        e.HasOne(x => x.Cliente).WithMany(x => x.Promociones).HasForeignKey(x => x.ClienteId).HasConstraintName("FK_PC_CLIENTE");
    }

    private static void ConfigurarClienteProducto(ModelBuilder b)
    {
        var e = b.Entity<ClienteProducto>();
        e.ToTable("CLIENTE_PRODUCTO");
        e.HasKey(x => new { x.ClienteId, x.ProductoId }).HasName("PK_CLIENTE_PRODUCTO");
        e.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").HasPrecision(12);
        e.Property(x => x.ProductoId).HasColumnName("PRODUCTO_ID").HasPrecision(12);
        e.Property(x => x.PrimeraCompra).HasColumnName("PRIMERA_COMPRA").HasColumnType("DATE");
        e.Property(x => x.UltimaCompra).HasColumnName("ULTIMA_COMPRA").HasColumnType("DATE");
        e.Property(x => x.NumCompras).HasColumnName("NUM_COMPRAS").HasPrecision(12).HasDefaultValue(0L);
        e.Property(x => x.CantidadTotal).HasColumnName("CANTIDAD_TOTAL").HasPrecision(18, 4).HasDefaultValue(0m);
        e.Property(x => x.ImporteTotal).HasColumnName("IMPORTE_TOTAL").HasPrecision(18, 2).HasDefaultValue(0m);
        e.Property(x => x.PromedioDiasCompra).HasColumnName("PROMEDIO_DIAS_COMPRA").HasPrecision(12, 2);
        e.Property(x => x.TicketPromedio).HasColumnName("TICKET_PROMEDIO").HasPrecision(18, 2);
        e.Property(x => x.FechaActualizacion).HasColumnName("FECHA_ACTUALIZACION").HasColumnType("TIMESTAMP").HasDefaultValueSql("SYSTIMESTAMP");
        e.HasOne(x => x.Cliente).WithMany(x => x.Productos).HasForeignKey(x => x.ClienteId).HasConstraintName("FK_CP_CLIENTE");
        e.HasOne(x => x.Producto).WithMany(x => x.Clientes).HasForeignKey(x => x.ProductoId).HasConstraintName("FK_CP_PRODUCTO");
    }

    private static void ConfigurarRecomendacion(ModelBuilder b)
    {
        var e = b.Entity<Recomendacion>();
        e.ToTable("RECOMENDACIONES");
        e.HasKey(x => x.RecomendacionId).HasName("PK_RECOMENDACIONES");
        e.Property(x => x.RecomendacionId).HasColumnName("RECOMENDACION_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_RECOMENDACIONES.NEXTVAL");
        e.Property(x => x.ClienteId).HasColumnName("CLIENTE_ID").HasPrecision(12);
        e.Property(x => x.ProductoId).HasColumnName("PRODUCTO_ID").HasPrecision(12);
        e.Property(x => x.Tipo).HasColumnName("TIPO").HasMaxLength(40).IsRequired();
        e.Property(x => x.Score).HasColumnName("SCORE").HasPrecision(6, 2);
        e.Property(x => x.Prioridad).HasColumnName("PRIORIDAD").HasMaxLength(20).IsRequired();
        e.Property(x => x.Motivo).HasColumnName("MOTIVO").HasMaxLength(1000).IsRequired();
        e.Property(x => x.FechaGeneracion).HasColumnName("FECHA_GENERACION").HasColumnType("TIMESTAMP").HasDefaultValueSql("SYSTIMESTAMP");
        e.Property(x => x.FechaExpiracion).HasColumnName("FECHA_EXPIRACION").HasColumnType("TIMESTAMP");
        e.Property(x => x.Estatus).HasColumnName("ESTATUS").HasMaxLength(20).HasDefaultValue("PENDIENTE").IsRequired();
        e.HasOne(x => x.Cliente).WithMany(x => x.Recomendaciones).HasForeignKey(x => x.ClienteId).HasConstraintName("FK_REC_CLIENTE");
        e.HasOne(x => x.Producto).WithMany(x => x.Recomendaciones).HasForeignKey(x => x.ProductoId).HasConstraintName("FK_REC_PRODUCTO");
    }

    private static void ConfigurarHistorialRecomendacion(ModelBuilder b)
    {
        var e = b.Entity<HistorialRecomendacion>();
        e.ToTable("HISTORIAL_RECOMENDACION");
        e.HasKey(x => x.HistorialId).HasName("PK_HIST_RECOMENDACION");
        e.Property(x => x.HistorialId).HasColumnName("HISTORIAL_ID").HasPrecision(12).ValueGeneratedOnAdd().HasDefaultValueSql("SEQ_HIST_RECOMENDACION.NEXTVAL");
        e.Property(x => x.RecomendacionId).HasColumnName("RECOMENDACION_ID").HasPrecision(12);
        e.Property(x => x.Fecha).HasColumnName("FECHA").HasColumnType("TIMESTAMP").HasDefaultValueSql("SYSTIMESTAMP");
        e.Property(x => x.Accion).HasColumnName("ACCION").HasMaxLength(40).IsRequired();
        e.Property(x => x.UsuarioId).HasColumnName("USUARIO_ID").HasPrecision(12);
        e.Property(x => x.Resultado).HasColumnName("RESULTADO").HasMaxLength(1000);
        e.HasOne(x => x.Recomendacion).WithMany(x => x.Historial).HasForeignKey(x => x.RecomendacionId).HasConstraintName("FK_HIST_RECOMENDACION");
        e.HasOne(x => x.Usuario).WithMany(x => x.HistorialRecomendaciones).HasForeignKey(x => x.UsuarioId).HasConstraintName("FK_HIST_USUARIO");
    }
}
