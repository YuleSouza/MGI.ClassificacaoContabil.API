using DTO.Payload;
using Service.DTO.Filtros;
using Service.Interface.FiltroTela;
using Service.Repository.FiltroTela;

namespace Service.FiltroTela
{
    public class FiltroTelaService : IFiltroTelaService
    {
        private readonly IFiltroTelaRepository _FiltroTelaRepository;
        public FiltroTelaService(IFiltroTelaRepository FiltroTelaRepository)
        {
            _FiltroTelaRepository = FiltroTelaRepository;
        }

        public async Task<PayloadDTO> ConsultarEmpresa(FiltroEmpresa filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarEmpresaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarProjeto(FiltroProjeto filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarProjetoClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarDiretoria(FiltroDiretoria filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarDiretoriaClassifiContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarGerencia(FiltroGerencia filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGerenciaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarGestor(FiltroGestor filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGestorClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarGrupoPrograma(FiltroGrupoPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGrupoProgramaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarPrograma(FiltroPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarProgramaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarClassificacaoContabil()
        {
            var resultado = await _FiltroTelaRepository.ConsultarClassificacaoContabil();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarClassificacaoEsg()
        {
            var resultado = await _FiltroTelaRepository.ConsultarClassificacaoEsg();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarCenario()
        {
            var resultado = await _FiltroTelaRepository.ConsultarCenario();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }

        public async Task<PayloadDTO> ConsultarCoordenadoria(FiltroCoordenadoria filtro)
        {
            var coordenadoria = await _FiltroTelaRepository.ConsultarCoordenadoria(filtro);

            var retorno = coordenadoria.Select(item => new
            {
                IdCoordenadoria = item.Id.ToString(),
                Nome = item.Descricao
            }).ToList();
            return new PayloadDTO(string.Empty, true, string.Empty, retorno);
        }
    }
}
