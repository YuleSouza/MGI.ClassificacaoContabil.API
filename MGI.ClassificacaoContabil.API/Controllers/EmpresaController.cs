using DTO.Payload;
using Service.Interface.Empresa;

using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaService _service;
        private readonly ILogger<EmpresaController> _logger;

        public EmpresaController(IEmpresaService service, ILogger<EmpresaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Route("v1/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Consultar()
        {
            var retorno = await _service.ConsultarEmpresa();
            return Ok(retorno);
        }
    }
}
