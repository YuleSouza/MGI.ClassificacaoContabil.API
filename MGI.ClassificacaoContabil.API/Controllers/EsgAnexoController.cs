﻿using DTO.Payload;
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

        [HttpGet("v1/download/{idAnexo}")]
        public async Task<IActionResult> DownloadAnexo([FromRoute] int idAnexo)
        {
            try
            {
                var anexo = await _service.ObterAnexo(idAnexo);
                var tipoArquivo = await _service.GetContentType(idAnexo);
                return File(anexo, tipoArquivo.extensao, tipoArquivo.nomeArquico);
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
        public async Task<IActionResult> UploadAnexos([FromForm] IFormFile arquivo, [FromForm] string anexos)
        {
            var listaAnexos = System.Text.Json.JsonSerializer.Deserialize<List<AnexoJustificaitvaClassifEsgDTO>>(anexos);
            int idProjeto = listaAnexos.Select(p => p.IdProjeto).FirstOrDefault();
            await _service.InserirAnexos(listaAnexos);
            var arquivoGravado = await _service.SalvarAnexo(arquivo, idProjeto);
            if (!arquivoGravado.Sucesso)
            {
                return BadRequest(arquivoGravado);
            }
            return Ok();
        }

        [HttpDelete("v1/delete/{idAnexo}")]
        public async Task<IActionResult> DeleteAnexo([FromRoute] int idAnexo)
        {
            var resultado = await _service.ApagarAnexo(idAnexo);
            return Ok(resultado);
        }

        [HttpGet("v1/consultar/{idClassifEsg}")]
        public async Task<IActionResult> ConsultarAnexos([FromRoute] int idClassifEsg)
        {
            var resultado = await _service.ConsultarAnexos(idClassifEsg);
            return  Ok(resultado);
        }
    }
}
