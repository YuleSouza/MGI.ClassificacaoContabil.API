using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;
using Service.Interface.Classificacao;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> ConsultarDadosClassificacao([FromBody] FiltroClassificacaoContabil filtro)
        {
            var retorno = await _service.ConsultarClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/contabil/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacao()
        {
            var retorno = await _service.ConsultarClassificacaoContabil();
            return Ok(retorno);
        }


        [HttpPost]
        [Route("v1/contabil/projeto/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosProjetoClassificacao([FromBody] ClassificacaoProjetoDTO projeto)
        {
            var retorno = await _service.InserirProjetoClassificacaoContabil(projeto);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro inserir dados do projeto classificação contábil.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/contabil/projeto/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosProjetoClassificacao([FromBody] ClassificacaoProjetoDTO projeto)
        {
            var retorno = await _service.AlterarProjetoClassificacaoContabil(projeto);
            if (retorno.Sucesso)
            {
                return Ok(retorno);
            }
            else
            {
                _logger.LogError("Erro alteracao dados do projeto classificação contábil.", retorno);
                return BadRequest(new
                {
                    message = retorno.MensagemErro
                });
            }
        }

        [HttpPost]
        [Route("v1/contabil/projeto/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadoProjetosClassificacao([FromBody] FiltroClassificacaoContabil filtro)
        {
            var retorno = await _service.ConsultarProjetoClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/contabil/projeto/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosProjetoClassificacao()
        {
            var retorno = await _service.ConsultarProjetoClassificacaoContabil();
            return Ok(retorno);
        }

        [HttpGet("v1/contabil/projeto/regra-excessao/{idprojeto}/ano/{ano}")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosProjetoClassificacao([FromRoute] int idprojeto, int ano)
        {
            var retorno = await _service.VerificarRegraExcessaoContabil(new FiltroClassificacaoContabil()
            {
                Ano = ano,
                IdProjeto = idprojeto,
            });
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
        public async Task<IActionResult> ConsultarDadosClassificacaoEsg([FromBody] ClassificacaoEsgFiltro filtro)
        {
            var retorno = await _service.ConsultarClassificacaoEsg(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/esg/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacaoEsg()
        {
            var retorno = await _service.ConsultarClassificacaoEsg();
            return Ok(retorno);
        }
        #endregion
    }
}
