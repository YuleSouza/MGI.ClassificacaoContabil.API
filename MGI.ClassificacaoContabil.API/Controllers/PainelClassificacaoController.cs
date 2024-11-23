using Service.DTO.Filtros;
using Service.Interface.PainelClassificacao;

using Microsoft.AspNetCore.Mvc;
using MGI.ClassificacaoContabil.API.Model;
using Service.Enum;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PainelClassificacaoController : ControllerBase
    {
        private readonly IPainelClassificacaoService _service;
        private readonly IPainelClassificacaoEsgService _serviceEsg;
        private readonly ILogger<PainelClassificacaoController> _logger;

        public PainelClassificacaoController(IPainelClassificacaoService service, 
            ILogger<PainelClassificacaoController> logger,
            IPainelClassificacaoEsgService serviceEsg)
        {
            _service = service;
            _serviceEsg = serviceEsg;
            _logger = logger;
        }

        #region [Filtros]

        /// <summary>
        /// Retorna dados para combo Empresa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboempresa")]
        public async Task<IActionResult> ComboEmpresa([FromBody] FiltroPainelEmpresa filtro)
        {
            var retorno = await _service.FiltroPainelEmpresa(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogrupoprograma")]
        public async Task<IActionResult> ComboGrupoPrograma([FromBody] FiltroPainelGrupoPrograma filtro)
        {
            var retorno = await _service.FiltroPainelGrupoPrograma(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboprograma")]
        public async Task<IActionResult> ComboPrograma([FromBody] FiltroPainelPrograma filtro)
        {
            var retorno = await _service.FiltroPainelPrograma(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Diretoria
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combodiretoria")]
        public async Task<IActionResult> ComboDiretoria([FromBody] FiltroPainelDiretoria filtro)
        {
            var retorno = await _service.FiltroPainelDiretoria(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Gerencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogerencia")]
        public async Task<IActionResult> ComboGerencia([FromBody] FiltroPainelGerencia filtro)
        {
            var retorno = await _service.FiltroPainelGerencia(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Gestor
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogestor")]
        public async Task<IActionResult> ComboGestor([FromBody] FiltroPainelGestor filtro)
        {
            var retorno = await _service.FiltroPainelGestor(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboprojeto")]
        public async Task<IActionResult> ComboProjeto([FromBody] FiltroPainelProjeto filtro)
        {
            var retorno = await _service.FiltroPainelProjeto(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combocenario")]
        public async Task<IActionResult> ComboCenario([FromBody] FiltroPainelCenario filtro)
        {
            var retorno = await _service.FiltroPainelCenario(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combocontabil")]
        public async Task<IActionResult> ComboClassificacaoContabil([FromBody] FiltroPainelClassificacaoContabil filtro)
        {
            var retorno = await _service.FiltroPainelClassificacaoContabil(filtro);
            return Ok(retorno);
        }
        [HttpPost("v1/consultar/lancamentosap")]
        public async Task<IActionResult> ConsultarLancamentoSap([FromBody] FiltroLancamentoSap filtro)
        {
            return Ok(await _service.ConsultarLancamentoSap(filtro));
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboesg")]
        public async Task<IActionResult> ComboClassificacaoEsg([FromBody] FiltroPainelClassificacaoEsg filtro)
        {
            var retorno = await _service.FiltroPainelClassificacaoEsg(filtro);
            return Ok(retorno);
        }
        #endregion

        [HttpPost("v1/consultar")]
        public async Task<IActionResult> Consultar(PainelClassificacaoContabilModel filtro)
        {
            var retorno = await _service.ConsultarClassificacaoContabil(new FiltroPainelClassificacaoContabil()
            {
                IdClassificacaoContabil = filtro.IdClassificacaoContabil,
                ClassificacaoContabil = filtro.ClassificacaoContabil,
                FormatAcompanhamento = filtro.FormatAcompanhamento,
                TipoAcumuladoOuAnual = filtro.TipoAcumuladoOuAnual,
                IdGrupoPrograma = filtro.IdGrupoPrograma,
                BaseOrcamento = filtro.BaseOrcamento,
                IdPrograma = filtro.IdPrograma,
                DataInicio = filtro.DataInicio,
                IdEmpresa = filtro.IdEmpresa,
                IdProjeto = filtro.IdProjeto,
                IdGestor = filtro.IdGestor,
                DataFim = filtro.DataFim,
            });
            return Ok(retorno);
        }

        [HttpPost("v1/consultar/esg")]
        public async Task<IActionResult> ConsultarEsg(FiltroPainelClassificacaoEsg filtro) 
        {
            if (filtro.BaseOrcamento == "R")
            {
                filtro.BaseOrcamento = "2";
            }
            var retorno = await _serviceEsg.ConsultarClassificacaoEsg(filtro);
            return Ok(retorno);
        }
        

        [HttpGet("v1/consultar/classifesg/{idprojeto}/{idcenario}/{seqfase}")]
        public async Task<IActionResult> ConsultarClassifEsg([FromRoute]int idprojeto, int idcenario, int seqfase)
        {
            var retorno = await _serviceEsg.ConsultarClassifEsgPorCenario(new FiltroPainelClassificacaoEsg() 
            { 
                IdCenario = idcenario,
                IdProjeto = idprojeto,
                SeqFase = seqfase
            });
            return Ok(retorno);
        }

        [HttpPost("v1/relatorio/contabil")]
        public async Task<IActionResult> GerarRelatorioContabil(FiltroPainelClassificacaoContabil filtro)
        {
            var retorno = await _service.GerarRelatorioContabil(filtro);
            if (retorno == null) return BadRequest("Erro ao gerar relatório");
            return File(retorno, "text/csv", $"relatorio_contabil_{DateTime.Now.Millisecond}.csv");
        }

        [HttpPost("v1/relatorio/esg")]
        public async Task<IActionResult> GerarRelatorioContabilEsg(FiltroPainelClassificacaoEsg filtro)
        {
            var retorno = await _serviceEsg.GerarRelatorioEsg(filtro);
            if (retorno == null)
            {
                return BadRequest("Erro ao gerar relatório");
            }
            string filename = "relatorio.csv";
            return File(retorno, "text/csv", $"relatorio_esg_{DateTime.Now.Millisecond}.csv");
        }

    }
}