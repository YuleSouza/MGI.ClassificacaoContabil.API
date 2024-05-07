using DTO.Payload;
using Infra.Interface;
using Service.DTO.Filtros;
using Service.Interface.PainelClassificacao;
using Service.Repository.PainelClassificacao;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(IPainelClassificacaoRepository PainelClassificacaoRepository, IUnitOfWork unitOfWork)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
        }

        #region [Filtros]
        public async Task<PayloadDTO> FiltroPainelEmpresa(FiltroPainelEmpresa filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelEmpresa(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelProjeto(FiltroPainelProjeto filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelProjeto(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelDiretoria(FiltroPainelDiretoria filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelDiretoria(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGerencia(FiltroPainelGerencia filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGerencia(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGestor(FiltroPainelGestor filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGestor(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGrupoPrograma(FiltroPainelGrupoPrograma filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGrupoPrograma(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelPrograma(FiltroPainelPrograma filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelPrograma(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelCenario(FiltroPainelCenario filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelCenario(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelClassificacaoESG(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        #endregion

    }
}
