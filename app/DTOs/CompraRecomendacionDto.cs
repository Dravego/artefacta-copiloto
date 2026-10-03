namespace Artefacta.Copiloto.DTOs;

public class CompraClienteDto
{
    public long ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string Cliente { get; set; } = "";
    public DateTime? UltimaCompra { get; set; }
    public decimal TotalHistorico { get; set; }
    public int NumeroVentas { get; set; }
    public List<ProductoCompraDto> Recompra { get; set; } = [];
    public List<ProductoCompraDto> VentaCruzada { get; set; } = [];
    public List<ProductoCompraDto> Populares { get; set; } = [];
}

public class ProductoCompraDto
{
    public long ProductoId { get; set; }
    public string CodigoProducto { get; set; } = "";
    public string Producto { get; set; } = "";
    public string? Categoria { get; set; }
    public decimal Precio { get; set; }
    public string TipoSugerencia { get; set; } = "";
    public decimal Score { get; set; }
    public string Motivo { get; set; } = "";
    public string? Promocion { get; set; }
    public decimal? ValorPromocion { get; set; }
}
