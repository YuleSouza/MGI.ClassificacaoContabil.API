using Service.Interface.Classificacao;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;

using Microsoft.AspNetCore.Mvc;
using DTO.Payload;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    public class ClassificacaoController : ControllerBase
    {
        private readonly IClassificacaoService _service;
        private readonly ILogger<ClassificacaoController> _logger;

        public ClassificacaoController(IClassificacaoService service, ILogger<ClassificacaoController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Contábil
        [HttpPost]
        [Route("v1/contabil/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosClassificacao([FromBody] ClassificacaoContabilDTO classificacao)
        {
            var retorno = await _service.InserirClassificacaoContabil(classificacao);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro inserir dados da classificação contábil.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/contabil/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosClassificacao([FromBody] ClassificacaoContabilDTO classificacao)
        {
            var retorno = await _service.AlterarClassificacaoContabil(classificacao);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro alteracao dados da classificação contábil.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/contabil/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacao([FromBody] ClassificacaoContabilFiltro filtro)
        {
            var retorno = await _service.ConsultarClassificacaoContabil(filtro);
            return Ok(retorno);
        }
        #endregion

        #region ESG
        [HttpPost]
        [Route("v1/esg/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosClassificacao([FromBody] ClassificacaoEsgDTO classificacao)
        {
            var retorno = await _service.InserirClassificacaoEsg(classificacao);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro inserir dados da classificação ESG.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/esg/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosClassificacao([FromBody] ClassificacaoEsgDTO classificacao)
        {
            var retorno = await _service.AlterarClassificacaoEsg(classificacao);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro alteracao dados da classificação ESG.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/esg/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacao([FromBody] ClassificacaoEsgFiltro filtro)
        {
            var retorno = await _service.ConsultarClassificacaoEsg(filtro);
            return Ok(retorno);
        }

        #endregion

    }
}
