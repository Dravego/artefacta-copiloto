namespace Artefacta.Copiloto.DTOs;

public class OportunidadDto
{
    public long ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string Cliente { get; set; } = "";
    public long ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public DateTime UltimaCompra { get; set; }
    public decimal PromedioDiasCompra { get; set; }
    public int DiasDesdeUltimaCompra { get; set; }
    public int DiasAtraso { get; set; }
    public long NumCompras { get; set; }
    public decimal ImporteTotal { get; set; }
    public decimal Score { get; set; }
    public string Prioridad { get; set; } = "";
    public string Motivo { get; set; } = "";
}
