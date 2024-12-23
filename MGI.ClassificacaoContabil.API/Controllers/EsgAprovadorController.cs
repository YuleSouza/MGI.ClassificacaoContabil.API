using MGI.ClassificacaoContabil.API.ControllerAtributes;
using MGI.ClassificacaoContabil.API.Model;
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

        [HttpGet("v1/consultar/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Consultar usuário aprovador")]
        public async Task<IActionResult> Consultar(string usuario, string email)
        {
            var resultado = await _service.ConsultarUsuarioAprovador(usuario, email);
            return Ok(resultado);
        }

        [HttpPut("v1/alterar/{id}/{usuario}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Alterar usuário aprovador")]
        public async Task<IActionResult> Alterar([FromBody] AprovadorModel aprovador)
        {
            await _service.AlterarUsuarioAprovador(aprovador.Email, aprovador.Id);
            return Ok();
        }

        [HttpPut("v1/excluir/{id:int}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Excluir usuário aprovador")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _service.ExcluirUsuarioAprovador(id);
            return Ok();
        }

        [HttpPost("v1/inserir")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ActionDescription("Inserir usuário aprovador")]
        public async Task<IActionResult> Inserir([FromBody] AprovadorModel aprovador)
        {
            var resultado = await _service.InserirUsuarioAprovador(aprovador.Usuario, aprovador.Email);
            if (!resultado.Sucesso) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
