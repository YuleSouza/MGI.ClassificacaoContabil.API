﻿using DTO.Payload;
using Service.DTO.Parametrizacao;
using Service.Interface.Parametrizacao;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametrizacaoController : ControllerBase
    {
        private readonly IParametrizacaoService _service;
        private readonly IParametrizacaoCenarioService _parametrizacaoCenarioService;
        private readonly IParametrizacaoEsgGeralService _parametrizacaoEsgService;
        private readonly ILogger<ParametrizacaoController> _logger;

        public ParametrizacaoController(
            IParametrizacaoService service, 
            IParametrizacaoCenarioService parametrizacaoCenarioService,
            IParametrizacaoEsgGeralService parametrizacaoEsgService,
            ILogger<ParametrizacaoController> logger)
        {
            _service = service;
            _logger = logger;
            _parametrizacaoCenarioService = parametrizacaoCenarioService;
            _parametrizacaoEsgService = parametrizacaoEsgService;
        }

        #region Parametrização Cenário 

        [HttpPost]
        [Route("v1/cenario/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosCenario([FromBody] ParametrizacaoCenarioDTO parametrizacao)
        {
            var retorno = await _parametrizacaoCenarioService.InserirParametrizacaoCenario(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
            
        }

        [HttpPost]
        [Route("v1/cenario/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosCenario([FromBody] ParametrizacaoCenarioDTO parametrizacao)
        {
            var retorno = await _parametrizacaoCenarioService.AlterarParametrizacaoCenario(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
            
        }

        [HttpGet]
        [Route("v1/cenario/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosParametrizacaoCenario()
        {
            var retorno = await _parametrizacaoCenarioService.ConsultarParametrizacaoCenario();
            return Ok(retorno);
        }

        #endregion

        #region Parametrização Classificacação ESG Geral

        [HttpPost]
        [Route("v1/geral/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosParametrizacaoClassificacaoGeral([FromBody] ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            var retorno = await _parametrizacaoEsgService.InserirParametrizacaoClassificacaoGeral(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/geral/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosParametrizacaoClassificacaoGeral([FromBody] ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            var retorno = await _parametrizacaoEsgService.AlterarParametrizacaoClassificacaoGeral(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);            
            return Ok(retorno);
        }

        [HttpGet]
        [Route("v1/geral/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosParametrizacaoClassificacaoGeral()
        {
            var retorno = await _parametrizacaoEsgService.ConsultarParametrizacaoClassificacaoGeral();
            return Ok(retorno);
        }

        #endregion

        #region Parametrização Classificacação ESG Exceção

        [HttpPost]
        [Route("v1/excecao/inserir")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> InserirDadosParametrizacaoClassificacaoExcecao([FromBody] ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            var retorno = await _service.InserirParametrizacaoClassificacaoExcecao(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
        }

        [HttpPost]
        [Route("v1/excecao/alterar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AlterarDadosParametrizacaoClassificacaoExcecao([FromBody] ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            var retorno = await _service.AlterarParametrizacaoClassificacaoExcecao(parametrizacao);
            if (!retorno.Sucesso) return BadRequest(retorno);
            return Ok(retorno);
            
        }

        [HttpGet]
        [Route("v1/excecao/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarDadosParametrizacaoClassificacaoExcecao()
        {
            var retorno = await _service.ConsultarParametrizacaoClassificacaoExcecao();
            return Ok(retorno);
        }
        #endregion
    }
}
