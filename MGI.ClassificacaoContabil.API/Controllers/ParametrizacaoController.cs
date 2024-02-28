using DTO.Payload;
using Service.Interface.Parametrizacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Filtros;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametrizacaoController : ControllerBase
    {
        private readonly IParametrizacaoService _service;
        private readonly ILogger<ParametrizacaoController> _logger;

        public ParametrizacaoController(IParametrizacaoService service, ILogger<ParametrizacaoController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpPost]
        [Route("v1/cenario/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosCenario([FromBody] ParametrizacaoCenarioDTO parametrizacao)
        {
            var retorno = await _service.InserirParametrizacaoCenario(parametrizacao);
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
        [Route("v1/cenario/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosCenario([FromBody] ParametrizacaoCenarioDTO parametrizacao)
        {
            var retorno = await _service.AlterarParametrizacaoCenario(parametrizacao);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro alteração dados da parametrização do cenário.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpGet]
        [Route("v1/cenario/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosParametrizacaoCenario()
        {
            var retorno = await _service.ConsultarParametrizacaoCenario();
            return Ok(retorno);
        }
    }
}
