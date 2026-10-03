using System.Security.Claims;

namespace Artefacta.Copiloto.Security;

public class UsuarioActual(IHttpContextAccessor accessor)
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public bool Autenticado => User?.Identity?.IsAuthenticated == true;
    public long? UsuarioId => ParseLong(User?.FindFirstValue(ClaimTypes.NameIdentifier));
    public long? VendedorId => ParseLong(User?.FindFirstValue("vendedor_id"));
    public string Username => User?.FindFirstValue(ClaimTypes.Name) ?? "";
    public string Nombre => User?.FindFirstValue("nombre") ?? Username;

    public bool EsAdmin => User?.IsInRole("ADMIN") == true;
    public bool EsGerente => User?.IsInRole("GERENTE") == true;
    public bool EsSupervisor => User?.IsInRole("SUPERVISOR") == true;
    public bool EsVendedor => User?.IsInRole("VENDEDOR") == true;
    public bool EsConsulta => User?.IsInRole("CONSULTA") == true;

    public bool PuedeEscribir =>
        EsAdmin || EsGerente || EsSupervisor || EsVendedor;

    public bool VisionGlobal =>
        EsAdmin || EsGerente || EsSupervisor || EsConsulta;

    private static long? ParseLong(string? value)
        => long.TryParse(value, out var result) ? result : null;
}
