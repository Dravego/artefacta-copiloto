using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Models;
using Artefacta.Copiloto.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class AdministracionUsuariosService(ArtefactaDbContext db)
{
    public async Task<UsuarioCrearViewModel> PrepararNuevoAsync()
    {
        return new UsuarioCrearViewModel
        {
            Activo = true,
            Roles = await ObtenerRolesAsync(),
            Vendedores = await ObtenerVendedoresAsync()
        };
    }

    public async Task<UsuarioEditarViewModel?> PrepararEdicionAsync(long id)
    {
        var usuario = await db.Usuarios
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.UsuarioId == id);

        if (usuario is null)
            return null;

        return new UsuarioEditarViewModel
        {
            UsuarioId = usuario.UsuarioId,
            Username = usuario.Username,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            VendedorId = usuario.VendedorId,
            Activo = usuario.Activo,
            RolesSeleccionados = usuario.Roles.Select(x => x.RolId).ToList(),
            Roles = await ObtenerRolesAsync(),
            Vendedores = await ObtenerVendedoresAsync()
        };
    }

    public async Task<(bool ok, string mensaje)> CrearAsync(UsuarioCrearViewModel model)
    {
        var username = model.Username.Trim();

        var existentes = await db.Usuarios.CountAsync(
            x => x.Username.ToUpper() == username.ToUpper());

        if (existentes > 0)
            return (false, "Ya existe un usuario con ese nombre.");

        var usuario = new Usuario
        {
            Username = username,
            PasswordHash = AutenticacionService.PasswordPendiente,
            Nombre = model.Nombre.Trim(),
            Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
            VendedorId = model.VendedorId,
            Activo = model.Activo,
            FechaAlta = DateTime.Now
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        foreach (var rolId in model.RolesSeleccionados.Distinct())
        {
            db.UsuarioRoles.Add(new UsuarioRol
            {
                UsuarioId = usuario.UsuarioId,
                RolId = rolId
            });
        }

        await db.SaveChangesAsync();

        return (true,
            "Usuario creado. En su primer acceso deberá definir una contraseña.");
    }

    public async Task<(bool ok, string mensaje)> ActualizarAsync(UsuarioEditarViewModel model)
    {
        var usuario = await db.Usuarios
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.UsuarioId == model.UsuarioId);

        if (usuario is null)
            return (false, "Usuario no encontrado.");

        var username = model.Username.Trim();

        var duplicados = await db.Usuarios.CountAsync(x =>
            x.UsuarioId != model.UsuarioId &&
            x.Username.ToUpper() == username.ToUpper());

        if (duplicados > 0)
            return (false, "Ya existe otro usuario con ese nombre.");

        var rolAdmin = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Nombre == "ADMIN");

        if (rolAdmin is not null)
        {
            var eraAdmin = usuario.Roles.Any(x => x.RolId == rolAdmin.RolId);
            var seguiraAdmin = model.RolesSeleccionados.Contains(rolAdmin.RolId);

            if (eraAdmin && (!model.Activo || !seguiraAdmin))
            {
                var otrosAdmins = await db.UsuarioRoles.CountAsync(x =>
                    x.RolId == rolAdmin.RolId &&
                    x.UsuarioId != model.UsuarioId &&
                    x.Usuario.Activo);

                if (otrosAdmins == 0)
                    return (false,
                        "No puedes desactivar o retirar el rol al único ADMIN activo.");
            }
        }

        usuario.Username = username;
        usuario.Nombre = model.Nombre.Trim();
        usuario.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        usuario.VendedorId = model.VendedorId;
        usuario.Activo = model.Activo;

        db.UsuarioRoles.RemoveRange(usuario.Roles);

        foreach (var rolId in model.RolesSeleccionados.Distinct())
        {
            db.UsuarioRoles.Add(new UsuarioRol
            {
                UsuarioId = usuario.UsuarioId,
                RolId = rolId
            });
        }

        await db.SaveChangesAsync();

        return (true, "Usuario actualizado.");
    }

    public async Task CargarCatalogosAsync(UsuarioCrearViewModel model)
    {
        model.Roles = await ObtenerRolesAsync();
        model.Vendedores = await ObtenerVendedoresAsync();
    }

    public async Task CargarCatalogosAsync(UsuarioEditarViewModel model)
    {
        model.Roles = await ObtenerRolesAsync();
        model.Vendedores = await ObtenerVendedoresAsync();
    }

    private async Task<List<RolOpcionViewModel>> ObtenerRolesAsync()
        => await db.Roles.AsNoTracking()
            .OrderBy(x => x.Nombre)
            .Select(x => new RolOpcionViewModel
            {
                RolId = x.RolId,
                Nombre = x.Nombre
            })
            .ToListAsync();

    private async Task<List<VendedorOpcionViewModel>> ObtenerVendedoresAsync()
        => await db.Vendedores.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new VendedorOpcionViewModel
            {
                VendedorId = x.VendedorId,
                CodigoVendedor = x.CodigoVendedor,
                Nombre = x.Nombre
            })
            .ToListAsync();
}
