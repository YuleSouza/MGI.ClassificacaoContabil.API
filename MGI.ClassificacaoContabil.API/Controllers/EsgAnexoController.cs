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
        private readonly IEsgAnexoService _service;

        public EsgAnexoController(IEsgAnexoService service)
        {
            _service = service;
        }

        [HttpGet("v1/download")]
        public async Task<IActionResult> DownloadAnexo([FromQuery] string nomeArquivo)
        {
            try
            {
                var arquivo = await _service.ObterArquivo(nomeArquivo);
                return File(arquivo, _service.GetContentType(nomeArquivo), nomeArquivo);
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
        public async Task<IActionResult> UploadAnexo(List<IFormFile> arquivos)
        {
            await _service.SalvarAnexos(arquivos);
            return Ok();
        }

        [HttpPost("v1/adicionar")]
        public async Task<IActionResult> AdicionarAnexos(List<IFormFile> arquivos, string anexos)
        {            
            var listaAnexos = System.Text.Json.JsonSerializer.Deserialize<List<AnexoJustificaitvaClassifEsgDTO>>(anexos);
            await _service.InserirAnexos(listaAnexos);
            var arquivoGravado = await _service.SalvarAnexos(arquivos);
            if (!arquivoGravado.Sucesso)
            {
                return BadRequest(arquivoGravado);
            }
            return Ok();
        }

        [HttpDelete("v1/delete")]
        public async Task<IActionResult> DeleteAnexo([FromQuery] int idAnexo)
        {
            var resultado = await _service.ApagarAnexo(idAnexo);
            return Ok(resultado);
        }

        [HttpGet("v1/consultar")]
        public async Task<IActionResult> ConsultarAnexos([FromQuery] int idClassifEsg)
        {
            var resultado = await _service.ConsultarAnexos(idClassifEsg);
            return  Ok(resultado);
        }
    }
}
