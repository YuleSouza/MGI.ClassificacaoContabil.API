using Service.DTO.Esg;
using Service.DTO.Filtros;

namespace Service.Interface.PainelEsg
{
    public interface IPainelEsgService
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
    }
}