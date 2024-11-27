using Microsoft.AspNetCore.Mvc;
using Service.DTO.Filtros;
using Service.Interface.PainelEsg;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsgController : ControllerBase
    {
        private readonly IPainelEsgService _service;
        public EsgController(IPainelEsgService service)
        {
            _service = service;
        }

        [HttpPost("v1/consultar")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {
            var resultado = await _service.ConsultarProjetos(filtro);
            return Ok(resultado);
        }
        
        [HttpPost("v1/consultar/classif-investimento")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarClassifInvestimento()
        {
            var resultado = await _service.ConsultarCalssifInvestimento();
            return Ok(resultado);
        }
        
        [HttpGet("v1/projeto/status")]
        public async Task<IActionResult> ConsultarStatusProjeto()
        {
            var resultado = await _service.ConsultarStatusProjeto();
            return Ok(resultado);
        }

        [HttpPost("v1/projeto")]
        public async Task<IActionResult> ConsultarProjetosEsg(FiltroProjeto filtro)
        {
            filtro.ProjetoEsg = true;
            var resulto = await _service.ConsultarProjetosEsg(filtro);
            return Ok(resulto);
        }
    }
}
