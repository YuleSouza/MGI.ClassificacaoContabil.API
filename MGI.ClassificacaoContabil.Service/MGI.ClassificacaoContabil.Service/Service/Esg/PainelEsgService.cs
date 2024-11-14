using Service.DTO.Esg;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;
using Service.DTO.Filtros;

namespace Service.Esg
{
    public class PainelEsgService : IPainelEsgService
    {
        private readonly IPainelEsgRepository _painelEsgRepository;
        public PainelEsgService(IPainelEsgRepository painelEsgRepository)
        {
            _painelEsgRepository = painelEsgRepository;
        }

        public Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {            
            return await _painelEsgRepository.ConsultarProjetosEsg(filtro);
        }
    }
}
