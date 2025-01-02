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

        public async Task<PayloadDTO>EmpresaClassificacaoContabil(FiltroEmpresa filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarEmpresaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ProjetoClassificacaoContabil(FiltroProjeto filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarProjetoClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> DiretoriaClassificacaoContabil(FiltroDiretoria filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarDiretoriaClassifiContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GerenciaClassificacaoContabil(FiltroGerencia filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGerenciaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GestorClassificacaoContabil(FiltroGestor filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGestorClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarGrupoProgramaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ProgramaClassificacaoContabil(FiltroPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.ConsultarProgramaClassifContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ClassificacaoContabil()
        {
            var resultado = await _FiltroTelaRepository.ConsultarClassificacaoContabil();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ClassificacaoEsg()
        {
            var resultado = await _FiltroTelaRepository.ConsultarClassificacaoEsg();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> Cenario()
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
