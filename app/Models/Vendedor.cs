namespace Artefacta.Copiloto.Models;

public class Vendedor
{
    public long VendedorId { get; set; }
    public string CodigoVendedor { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
