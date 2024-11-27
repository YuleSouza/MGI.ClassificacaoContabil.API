using Service.DTO.Esg;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Esg
{
    public class PainelEsgService : IPainelEsgService
    {
        private readonly IPainelEsgRepository _painelEsgRepository;
        public PainelEsgService(IPainelEsgRepository painelEsgRepository)
        {
            _painelEsgRepository = painelEsgRepository;
        }

        public async Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento()
        {
            return await _painelEsgRepository.ConsultarCalssifInvestimento();
        }

        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro)
        {
            return await _painelEsgRepository.ConsultarProjetos(filtro);
        }

        public async Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto()
        {
            return await _painelEsgRepository.ConsultarStatusProjeto();
        }
        public async Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro)
        {
            return await _painelEsgRepository.ConsultarProjetosEsg(filtro);
        }
    }
}
