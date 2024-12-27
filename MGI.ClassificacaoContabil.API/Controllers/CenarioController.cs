using DTO.Payload;
using Service.DTO.Cenario;
using Service.DTO.Filtros;
using Service.Interface.Cenario;

using Microsoft.AspNetCore.Mvc;
using MGI.ClassificacaoContabil.API.ControllerAtributes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenarioController : ControllerBase
    {
        private readonly ICenarioService _service;

        public CenarioController(ICenarioService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("v1/inserir")]
        [ActionDescription("Inserir cenário")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosCenario([FromBody] CenarioDTO cenario)
        {
            var retorno = await _service.InserirCenario(cenario);
            if (!retorno.Sucesso) return BadRequest(retorno);            
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/alterar")]
        [ActionDescription("Alterar cenário")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosCenario([FromBody] CenarioDTO cenario)
        {
            var retorno = await _service.AlterarCenario(cenario);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);            
        }

        [HttpPost]
        [Route("v1/consultar")]
        [ActionDescription("Consultar cenário")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosCenario([FromBody] CenarioFiltro filtro)
        {
            var retorno = await _service.ConsultarCenario(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/consultar")]
        [ActionDescription("Consultar cenário")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosCenario()
        {
            var retorno = await _service.ConsultarCenario();
            return Ok(retorno);
        }
    }
}
