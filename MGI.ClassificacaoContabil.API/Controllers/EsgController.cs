using MGI.ClassificacaoContabil.API.ControllerAtributes;
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
        public async Task<IActionResult> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {
            var resultado = await _service.ConsultarProjetos(filtro);
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
        public async Task<IActionResult> ConsultarProjetosEsg(FiltroProjeto filtro)
        {
            filtro.ProjetoEsg = true;
            var resultado = await _service.ConsultarProjetosEsg(filtro);
            return Ok(resultado);
        }

        [HttpGet("v1/categoria")]
        [ActionDescription("Consultar todas Categorias Esg")]
        public async Task<IActionResult> ConsultarCategoriaEsg()
        {
            var resultado = await _service.ConsultarCategoriaEsg();
            return Ok(resultado);
        }

        [HttpGet("v1/subcategoria/{idCategoria}")]
        [ActionDescription("Consultar Sub Categorias Esg por id de Categoria")]
        public async Task<IActionResult> ConsultarSubCategoriaEsg([FromRoute] int idCategoria)
        {
            var resultado = await _service.ConsultarSubCategoriaEsg(idCategoria);
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

        [HttpPost("v1/classificacao/aprovar/{idClassifEsg}/{aprovacao}/{usuarioAprovacao}")]
        [ActionDescription("Aprovação Classificação e Justificativa Painel Esg")]
        public async Task<IActionResult> Aprovar([FromRoute] int idClassifEsg, char aprovacao, string usuarioAprovacao)
        {
            var resultado = await _service.InserirAprovacao(idClassifEsg, aprovacao, usuarioAprovacao);
            if (resultado != null) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("v1/classificacao/excluir/{idClassifEsg}/{usuarioExclusao}")]
        [ActionDescription("Exclusão da classificação Esg")]
        public async Task<IActionResult> Excluir([FromRoute] int idClassifEsg, string usuarioExclusao) 
        {
            var resultado = await _service. ExcluirClassificacao(idClassifEsg, usuarioExclusao);
            if (resultado != null) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
