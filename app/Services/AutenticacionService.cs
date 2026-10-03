using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class AutenticacionService(
    ArtefactaDbContext db,
    IPasswordHasher<Usuario> passwordHasher)
{
    // Valor especial ya existente en el dataset.
    // Mientras un usuario activo tenga exactamente este valor,
    // el sistema NO intenta autenticarlo: obliga a definir una contraseña nueva.
    public const string PasswordPendiente =
        "$2a$10$DEMO_HASH_NO_UTILIZABLE_REEMPLAZAR";

    public async Task<Usuario?> ValidarAsync(string username, string password)
    {
        var normalizado = username.Trim().ToUpper();

        var usuario = await db.Usuarios
            .Include(x => x.Roles)
                .ThenInclude(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.Activo &&
                x.Username.ToUpper() == normalizado);

        if (usuario is null)
            return null;

        // El valor centinela nunca se trata como una contraseña real.
        if (usuario.PasswordHash == PasswordPendiente)
            return null;

        PasswordVerificationResult resultado;
        try
        {
            resultado = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                password);
        }
        catch
        {
            return null;
        }

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, password);
        }

        usuario.UltimoAcceso = DateTime.Now;
        await db.SaveChangesAsync();

        return usuario;
    }

    public async Task<bool> RequiereCambioPasswordAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var normalizado = username.Trim().ToUpper();

        // Traemos el hash y hacemos la comparación en C#.
        // Esto evita cualquier particularidad del proveedor Oracle y además
        // tolera espacios accidentales al inicio/final del valor almacenado.
        var passwordHash = await db.Usuarios
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.Username.ToUpper() == normalizado)
            .Select(x => x.PasswordHash)
            .FirstOrDefaultAsync();

        return passwordHash is not null &&
               passwordHash.Trim() == PasswordPendiente;
    }

    public async Task<bool> EstablecerPasswordPendienteAsync(
        string username,
        string nuevaPassword)
    {
        var normalizado = username.Trim().ToUpper();

        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(x =>
                x.Activo &&
                x.Username.ToUpper() == normalizado);

        if (usuario is null)
            return false;

        // Protección principal: sólo puede usarse este flujo si el DBA
        // dejó explícitamente el valor centinela en PASSWORD_HASH.
        if (usuario.PasswordHash?.Trim() != PasswordPendiente)
            return false;

        usuario.PasswordHash = passwordHasher.HashPassword(
            usuario,
            nuevaPassword);

        // No marcamos ULTIMO_ACCESO aquí; eso ocurrirá en el login real.
        await db.SaveChangesAsync();
        return true;
    }


    public async Task<bool> CambiarPasswordAutenticadoAsync(
        long usuarioId,
        string passwordActual,
        string nuevaPassword)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Activo);

        if (usuario is null)
            return false;

        if (usuario.PasswordHash?.Trim() == PasswordPendiente)
            return false;

        PasswordVerificationResult resultado;
        try
        {
            resultado = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                passwordActual);
        }
        catch
        {
            return false;
        }

        if (resultado == PasswordVerificationResult.Failed)
            return false;

        usuario.PasswordHash = passwordHasher.HashPassword(
            usuario,
            nuevaPassword);

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ForzarCambioPasswordAsync(long usuarioId)
    {
        var usuario = await db.Usuarios
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

        if (usuario is null)
            return false;

        usuario.PasswordHash = PasswordPendiente;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteAdminActivoAsync()
    {
        // Oracle 19c: COUNT(*) evita traducciones escalares TRUE/FALSE.
        var cantidad = await db.UsuarioRoles
            .CountAsync(x =>
                x.Usuario.Activo &&
                x.Rol.Nombre == "ADMIN");

        return cantidad > 0;
    }

    public async Task<Usuario> CrearPrimerAdminAsync(
        string username,
        string nombre,
        string? email,
        string password)
    {
        if (await ExisteAdminActivoAsync())
            throw new InvalidOperationException("Ya existe un administrador activo.");

        var normalizado = username.Trim();

        var usuariosConMismoNombre = await db.Usuarios
            .CountAsync(x => x.Username.ToUpper() == normalizado.ToUpper());

        if (usuariosConMismoNombre > 0)
            throw new InvalidOperationException("Ese nombre de usuario ya existe.");

        var rolAdmin = await db.Roles.FirstOrDefaultAsync(x => x.Nombre == "ADMIN")
            ?? throw new InvalidOperationException("No existe el rol ADMIN en Oracle.");

        var usuario = new Usuario
        {
            Username = normalizado,
            Nombre = nombre.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            Activo = true,
            FechaAlta = DateTime.Now,
            PasswordHash = "PENDIENTE"
        };

        usuario.PasswordHash = passwordHasher.HashPassword(usuario, password);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        db.UsuarioRoles.Add(new UsuarioRol
        {
            UsuarioId = usuario.UsuarioId,
            RolId = rolAdmin.RolId
        });

        await db.SaveChangesAsync();
        return usuario;
    }
}
