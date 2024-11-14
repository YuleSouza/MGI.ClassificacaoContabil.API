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
            var resultado = await _service.ConsultarProjetosEsg(filtro);
            return Ok(resultado);
        }

        //api/esg/consultar/classif-investimento
        [HttpPost("v1/consultar/classif-investimento")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarClassifInvestimento(FiltroProjetoEsg filtro)
        {
            var resultado = await _service.ConsultarProjetosEsg(filtro);
            return Ok(resultado);
        }
    }
}
