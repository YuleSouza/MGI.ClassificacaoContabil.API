using DTO.Payload;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Esg;
using Service.Interface.PainelEsg;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsgAnexoController : ControllerBase
    {
        private readonly IPainelEsgService _service;

        public EsgAnexoController(IPainelEsgService service)
        {
            _service = service;
        }

        [HttpGet("v1/download")]
        public async Task<IActionResult> DownloadAnexo([FromQuery] string nomeArquivo)
        {
            try
            {
                var arquivo = await _service.ObterArquivo(nomeArquivo);
                return File(arquivo, GetContentType(nomeArquivo), nomeArquivo);
            }
            catch (FileNotFoundException)
            {

                return NotFound(new PayloadDTO(string.Empty, false, "Arquivo não encontrado."));
            }
            catch (Exception ex)
            {
                return BadRequest(new PayloadDTO(string.Empty, false, "Erro ao fazer download do arquivo"));

            }
        }

        [HttpPost("v1/upload")]
        public async Task<IActionResult> UploadAnexo(List<IFormFile> arquivos, List<AnexoJustificaitvaClassifEsgDTO> anexos)
        {
            return Ok();
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string> {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
    }
}
