using Service.DTO.Filtros;
using Service.Interface.FiltroTela;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiltroTelaController : ControllerBase
    {
        private readonly IFiltroTelaService _telaFiltrosService;
        private readonly ILogger<FiltroTelaController> _logger;
        public FiltroTelaController(IFiltroTelaService telaFiltrosService, ILogger<FiltroTelaController> logger)
        {
            _telaFiltrosService = telaFiltrosService;
            _logger = logger;
        }

        /// <summary>
        /// Retorna dados para combo Empresa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboempresa")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboEmpresa([FromBody] FiltroEmpresa filtro)
        {
            var retorno = await _telaFiltrosService.EmpresaClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogrupoprograma")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboGrupoPrograma([FromBody] FiltroGrupoPrograma filtro)
        {
            var retorno = await _telaFiltrosService.GrupoProgramaClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboprograma")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboPrograma([FromBody] FiltroPrograma filtro)
        {
            var retorno = await _telaFiltrosService.ProgramaClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Diretoria
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combodiretoria")]
        public async Task<IActionResult> ComboDiretoria([FromBody] FiltroDiretoria filtro)
        {
            var retorno = await _telaFiltrosService.DiretoriaClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Gerencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogerencia")]
        public async Task<IActionResult> ComboGerencia([FromBody] FiltroGerencia filtro)
        {
            var retorno = await _telaFiltrosService.GerenciaClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Gestor
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combogestor")]
        public async Task<IActionResult> ComboGestor([FromBody] FiltroGestor filtro)
        {
            var retorno = await _telaFiltrosService.GestorClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboprojeto")]
        public async Task<IActionResult> ComboProjeto([FromBody] FiltroProjeto filtro)
        {
            var retorno = await _telaFiltrosService.ProjetoClassificacaoContabil(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Coordenadoria
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/combocoordenadoriajustificavariacao")]
        public async Task<IActionResult> ComboCoordenadoria([FromBody] FiltroCoordenadoria filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarCoordenadoria(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Classificacao Contabil
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboclassificacaocontabil")]
        public async Task<IActionResult> ComboClassificacaoContabil()
        {
            var retorno = await _telaFiltrosService.ClassificacaoContabil();
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Classificacao ESG
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/comboclassificacaoesg")]
        public async Task<IActionResult> ComboClassificacaoEsg()
        {
            var retorno = await _telaFiltrosService.ClassificacaoEsg();
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Classificacao ESG
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/combocenario")]
        public async Task<IActionResult> ComboCenario()
        {
            var retorno = await _telaFiltrosService.Cenario();
            return Ok(retorno);
        }
    }
}
