﻿using MGI.ClassificacaoContabil.API.ControllerAtributes;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Esg;
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
        [ActionDescription("Consultar Projetos Esg")]
        public async Task<IActionResult> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro)
        {
            var resultado = await _service.ConsultarProjetosPainelEsg(filtro);
            return Ok(resultado);
        }

        [HttpGet("v1/consultar/classif-investimento")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Consultar Classificação de Investimentos")]
        public async Task<IActionResult> ConsultarClassifInvestimento()
        {
            var resultado = await _service.ConsultarCalssifInvestimento();
            return Ok(resultado);
        }

        [HttpGet("v1/projeto/status")]
        [ActionDescription("Consultar Status do Projeto")]
        public async Task<IActionResult> ConsultarStatusProjeto()
        {
            var resultado = await _service.ConsultarStatusProjeto();
            return Ok(resultado);
        }

        [HttpPost("v1/projeto")]
        [ActionDescription("Consultar Projetos Painel Esg")]
        public async Task<IActionResult> ConsultarComboProjetosEsg(FiltroProjeto filtro)
        {
            var resultado = await _service.ConsultarComboProjetosEsg(filtro);
            return Ok(resultado);
        }

        [HttpGet("v1/classificacao")]
        [ActionDescription("Consultar todas Classificação Esg")]
        public async Task<IActionResult> ConsultarClassificacaoEsg()
        {
            var resultado = await _service.ConsultarClassificacaoEsg();
            return Ok(resultado);
        }

        [HttpGet("v1/subclassificacao/{idClassificacao}")]
        [ActionDescription("Consultar Sub Classificação Esg por id")]
        public async Task<IActionResult> ConsultarSubClassificacaoEsg([FromRoute] int idClassificacao)
        {
            var resultado = await _service.ConsultarSubClassificacaoEsg(idClassificacao);
            return Ok(resultado);
        }

        [HttpPost("v1/classificacao/inserir")]
        [ActionDescription("Inserir Classificação e Justificativa Painel Esg")]
        public async Task<IActionResult> InserirJustificativa([FromBody] JustificativaClassifEsg justificativaClassifEsg)
        {
            var resultado = await _service.InserirJustificativaEsg(justificativaClassifEsg);
            if (resultado != null) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("v1/classificacao/alterar")]
        [ActionDescription("Alterar Classificação e Justificativa Painel Esg")]
        public async Task<IActionResult> AlterarJustificativa([FromBody] AlteracaoJustificativaClassifEsg justificativaClassifEsg)
        {
            var resultado = await _service.AlterarJustificativaEsg(justificativaClassifEsg);
            if (resultado != null) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("v1/classificacao/consultar")]
        [ActionDescription("Consultar Classificação e Justificativa Painel Esg")]
        public async Task<IActionResult> ConsultarJustificativa([FromBody] FiltroJustificativaClassifEsg justificativaClassifEsg)
        {
            var resultado = await _service.ConsultarJustificativaEsg(justificativaClassifEsg);
            return Ok(resultado);
        }

        [HttpGet("v1/classificacao/log/{idJustificativa}")]
        [ActionDescription("Consultar Logs de aprovação")]
        public async Task<IActionResult> ConsultarLogAprovacao([FromRoute] int idJustificativa)
        {
            var resultado = await _service.ConsultarLogAprovacoesPorId(idJustificativa);
            return Ok(resultado);
        }

        [HttpPost("v1/classificacao/aprovar/{idClassifEsg}/{aprovacao}/{usuarioAprovacao}")]
        [ActionDescription("Aprovação Classificação e Justificativa Painel Esg")]
        public async Task<IActionResult> Aprovar([FromRoute] int idClassifEsg, string statusAprovacao, string usuarioAprovacao)
        {
            var resultado = await _service.InserirAprovacao(idClassifEsg, statusAprovacao, usuarioAprovacao);
            if (resultado != null) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("v1/classificacao/excluir/{idClassifEsg}/{usuarioExclusao}")]
        [ActionDescription("Exclusão da classificação Esg")]
        public async Task<IActionResult> Excluir([FromRoute] int idClassifEsg, string usuarioExclusao)
        {
            var resultado = await _service.ExcluirClassificacao(idClassifEsg, usuarioExclusao);
            if (!resultado.Sucesso) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
