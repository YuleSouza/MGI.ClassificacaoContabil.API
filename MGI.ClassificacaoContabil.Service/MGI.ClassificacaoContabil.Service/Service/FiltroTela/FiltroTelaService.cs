using DTO.Payload;
using Service.DTO.Filtros;
using Service.Repository.FiltroTela;
using Service.Interface.FiltroTela;
using Infra.Interface;

namespace Service.FiltroTela
{
    public class FiltroTelaService : IFiltroTelaService
    {
        private readonly IFiltroTelaRepository _FiltroTelaRepository;

        private IUnitOfWork _unitOfWork;
        public FiltroTelaService(IFiltroTelaRepository FiltroTelaRepository, IUnitOfWork unitOfWork)
        {
            _FiltroTelaRepository = FiltroTelaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PayloadDTO>EmpresaClassificacaoContabil(FiltroEmpresa filtro)
        {
            var resultado = await _FiltroTelaRepository.EmpresaClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ProjetoClassificacaoContabil(FiltroProjeto filtro)
        {
            var resultado = await _FiltroTelaRepository.ProjetoClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> DiretoriaClassificacaoContabil(FiltroDiretoria filtro)
        {
            var resultado = await _FiltroTelaRepository.DiretoriaClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GerenciaClassificacaoContabil(FiltroGerencia filtro)
        {
            var resultado = await _FiltroTelaRepository.GerenciaClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GestorClassificacaoContabil(FiltroGestor filtro)
        {
            var resultado = await _FiltroTelaRepository.GestorClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.GrupoProgramaClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ProgramaClassificacaoContabil(FiltroPrograma filtro)
        {
            var resultado = await _FiltroTelaRepository.ProgramaClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
    }
}
