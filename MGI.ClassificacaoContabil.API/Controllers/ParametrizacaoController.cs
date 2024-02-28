using DTO.Payload;
using Service.Interface.Parametrizacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Filtros;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametrizacaoController : ControllerBase
    {
        private readonly IParametrizacaoService _service;
        private readonly ILogger<ParametrizacaoController> _logger;

        public ParametrizacaoController(IParametrizacaoService service, ILogger<ParametrizacaoController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
