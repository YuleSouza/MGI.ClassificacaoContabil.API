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
        public FiltroTelaController(IFiltroTelaService telaFiltrosService)
        {
            _telaFiltrosService = telaFiltrosService;
        }

        /// <summary>
        /// Retorna dados para combo Empresa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/comboempresa")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboEmpresa([FromBody] FiltroEmpresa filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarEmpresa(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/combogrupoprograma")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboGrupoPrograma([FromBody] FiltroGrupoPrograma filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarGrupoPrograma(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Grupo Programa
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/comboprograma")]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> ComboPrograma([FromBody] FiltroPrograma filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarPrograma(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Diretoria
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/combodiretoria")]
        public async Task<IActionResult> ComboDiretoria([FromBody] FiltroDiretoria filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarDiretoria(filtro);
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
            var retorno = await _telaFiltrosService.ConsultarGerencia(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Gestor
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/combogestor")]
        public async Task<IActionResult> ComboGestor([FromBody] FiltroGestor filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarGestor(filtro);
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Projeto
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/comboprojeto")]
        public async Task<IActionResult> ComboProjeto([FromBody] FiltroProjeto filtro)
        {
            var retorno = await _telaFiltrosService.ConsultarProjeto(filtro);
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
        [HttpPost("v1/comboclassificacaocontabil")]
        public async Task<IActionResult> ComboClassificacaoContabil()
        {
            var retorno = await _telaFiltrosService.ConsultarClassificacaoContabil();
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Classificacao ESG
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/comboclassificacaoesg")]
        public async Task<IActionResult> ComboClassificacaoEsg()
        {
            var retorno = await _telaFiltrosService.ConsultarClassificacaoEsg();
            return Ok(retorno);
        }

        /// <summary>
        /// Retorna dados para combo Classificacao ESG
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost("v1/combocenario")]
        public async Task<IActionResult> ComboCenario()
        {
            var retorno = await _telaFiltrosService.ConsultarCenario();
            return Ok(retorno);
        }
    }
}
