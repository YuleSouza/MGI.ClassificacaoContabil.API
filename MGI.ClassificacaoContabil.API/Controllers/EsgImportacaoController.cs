using MGI.ClassificacaoContabil.API.ControllerAtributes;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.PainelEsg;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsgImportacaoController : ControllerBase
    {
        private readonly IImportacaoProjetoMGPService _service;
        public EsgImportacaoController(IImportacaoProjetoMGPService service)
        {
            _service = service;
        }

        [HttpPost("v1/importar")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Importar Projetos Esg do MGP")]
        public async Task<IActionResult> ConsultarProjetosPainelEsg()
        {
            await _service.ImportarProjetosEsg();
            return Ok();
        }
    }
}
