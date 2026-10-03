namespace Artefacta.Copiloto.DTOs;

public class CopilotoChatDto
{
    public string Pregunta { get; set; } = "";
    public string Respuesta { get; set; } = "";
    public string Intencion { get; set; } = "";
    public List<CopilotoDatoDto> Datos { get; set; } = [];
    public List<string> Sugerencias { get; set; } = [];
}

public class CopilotoDatoDto
{
    public string Titulo { get; set; } = "";
    public string Subtitulo { get; set; } = "";
    public string Detalle { get; set; } = "";
    public string? Url { get; set; }
}
