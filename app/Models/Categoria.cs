namespace Artefacta.Copiloto.Models;

public class Categoria
{
    public long CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
