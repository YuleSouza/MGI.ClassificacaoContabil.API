using MGI.ClassificacaoContabil.API.ControllerAtributes;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.PainelEsg;

namespace MGI.ClassificacaoContabil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsgAprovadorController : ControllerBase
    {
        private readonly IEsgAprovadorService _service;
        public EsgAprovadorController(IEsgAprovadorService service)
        {
            _service = service;
        }

        [HttpPost("v1/consultar/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Consultar usuário aprovador")]
        public async Task<IActionResult> ConsultarAprovador([FromQuery] string usuario)
        {
            await _service.ConsultarAprovadorPorUsuario(usuario);
            return Ok();
        }

        [HttpPut("v1/alterar/{id}/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Alterar usuário aprovador")]
        public async Task<IActionResult> AlterarAprovador([FromQuery] int id, [FromQuery] string usuario)
        {
            await _service.ConsultarAprovadorPorUsuario(usuario);
            return Ok();
        }

        [HttpPut("v1/excluir/{id}/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Excluir usuário aprovador")]
        public async Task<IActionResult> ExcluirAprovador([FromQuery] int id)
        {
            await _service.ExcluirUsuarioAprovador(id);
            return Ok();
        }

        [HttpPost("v1/inserir/{email}/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Excluir usuário aprovador")]
        public async Task<IActionResult> ExcluirAprovador([FromQuery] string usuario, string email)
        {
            await _service.InserirUsuarioAprovador(usuario, email);
            return Ok();
        }
    }
}
