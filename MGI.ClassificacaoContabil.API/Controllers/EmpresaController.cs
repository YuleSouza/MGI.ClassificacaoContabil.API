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
        public EmpresaController(IEmpresaService service, ILogger<EmpresaController> logger)
        {
            _service = service;
        }

        [HttpGet]
        [Route("v1/consultar")]
        [ProducesResponseType(typeof(PayloadDTO), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 120)]
        public async Task<IActionResult> Consultar()
        {
            var retorno = await _service.ConsultarEmpresa();
            return Ok(retorno);
        }
    }
}
