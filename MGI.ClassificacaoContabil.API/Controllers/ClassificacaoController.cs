using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;
using Service.Interface.Classificacao;

using Microsoft.AspNetCore.Mvc;
using MGI.ClassificacaoContabil.API.ControllerAtributes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassificacaoController : ControllerBase
    {
        private readonly IClassificacaoEsgService _service;
        private readonly IClassificacaoContabilService _contabilService;
        private readonly ILogger<ClassificacaoController> _logger;

        public ClassificacaoController(IClassificacaoEsgService service, ILogger<ClassificacaoController> logger, IClassificacaoContabilService contabilService)
        {
            _service = service;
            _logger = logger;
            _contabilService = contabilService;
        }

        #region Contábil
        [HttpPost]
        [Route("v1/contabil/inserir")]
        [ActionDescription("Inserir classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosClassificacao([FromBody] ClassificacaoContabilDTO classificacao)
        {
            var retorno = await _contabilService.InserirClassificacaoContabil(classificacao);
            if (!retorno.Sucesso) return BadRequest(retorno);            
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/contabil/alterar")]
        [ActionDescription("Alterar classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosClassificacao([FromBody] ClassificacaoContabilDTO classificacao)
        {
            var retorno = await _contabilService.AlterarClassificacaoContabil(classificacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/contabil/consultar")]
        [ActionDescription("Consultar classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacao([FromBody] FiltroClassificacaoContabil filtro)
        {
            var retorno = await _contabilService.ConsultarClassificacaoContabil();
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/contabil/consultar/mgp")]
        [ActionDescription("Consultar classificação contabil MGP")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacaoMgp()
        {
            var retorno = await _contabilService.ConsultarClassificacaoContabilMGP();
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/contabil/consultar")]
        [ActionDescription("Consultar classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacao()
        {
            var retorno = await _contabilService.ConsultarClassificacaoContabil();
            return Ok(retorno);
        }


        [HttpPost]
        [Route("v1/contabil/projeto/inserir")]
        [ActionDescription("Inserir projeto classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosProjetoClassificacao([FromBody] ClassificacaoProjetoDTO projeto)
        {
            var retorno = await _contabilService.InserirProjetoClassificacaoContabil(projeto);
            if (!retorno.Sucesso) BadRequest(retorno);            
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/contabil/projeto/alterar")]
        [ActionDescription("Alterar projeto classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosProjetoClassificacao([FromBody] ClassificacaoProjetoDTO projeto)
        {
            var retorno = await _contabilService.AlterarProjetoClassificacaoContabil(projeto);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/contabil/projeto/consultar")]
        [ActionDescription("Consultar projeto classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadoProjetosClassificacao([FromBody] FiltroClassificacaoContabil filtro)
        {
            var retorno = await _contabilService.ConsultarProjetoClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/contabil/projeto/consultar")]
        [ActionDescription("Consultar projeto classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosProjetoClassificacao()
        {
            var retorno = await _contabilService.ConsultarProjetoClassificacaoContabil();
            return Ok(retorno);
        }

        [HttpGet("v1/contabil/projeto/regra-excessao/{idprojeto}/ano/{ano}")]
        [ActionDescription("Consultar projeto classificação contábil")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosProjetoClassificacao([FromRoute] int idprojeto, int ano)
        {
            var retorno = await _contabilService.VerificarRegraExcessaoContabil(new FiltroClassificacaoContabil()
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
        [ActionDescription("Inserir classificação ESG")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosClassificacao([FromBody] ClassificacaoEsgDTO classificacao)
        {
            var retorno = await _service.InserirClassificacaoEsg(classificacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/esg/alterar")]
        [ActionDescription("Alterar classificação ESG")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosClassificacao([FromBody] ClassificacaoEsgDTO classificacao)
        {
            var retorno = await _service.AlterarClassificacaoEsg(classificacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/esg/consultar")]
        [ActionDescription("Cosnultar classificação ESG")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacaoEsg([FromBody] ClassificacaoEsgFiltro filtro)
        {
            var retorno = await _service.ConsultarClassificacaoEsg(filtro);
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/esg/consultar")]
        [ActionDescription("Cosnultar classificação ESG")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosClassificacaoEsg()
        {
            var retorno = await _service.ConsultarClassificacaoEsg();
            return Ok(retorno);
        }
        #endregion
    }
}
