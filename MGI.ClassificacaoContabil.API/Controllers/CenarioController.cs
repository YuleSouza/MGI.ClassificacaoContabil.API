using DTO.Payload;
using Service.Interface.Cenario;
using Service.DTO.Cenario;
using Service.DTO.Filtros;

using Microsoft.AspNetCore.Mvc;
using Service.DTO.Classificacao;


namespace MGI.ClassificacaoContabil.API.Controllers
{
    public class CenarioController : ControllerBase
    {
        private readonly ICenarioService _service;
        private readonly ILogger<CenarioController> _logger;

        public CenarioController(ICenarioService service, ILogger<CenarioController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpPost]
        [Route("v1/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosCenario([FromBody] CenarioDTO cenario)
        {
            var retorno = await _service.InserirCenario(cenario);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro inserir dados do cenário.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosCenario([FromBody] CenarioDTO cenario)
        {
            var retorno = await _service.AlterarCenario(cenario);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro alteracao dados do cenário.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosCenario([FromBody] CenarioFiltro filtro)
        {
            var retorno = await _service.ConsultarCenario(filtro);
            return Ok(retorno);
        }
    }
}
