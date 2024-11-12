using Service.DTO.Esg;
using Service.DTO.Filtros;

namespace Service.Repository.Esg
{
    public interface IPainelEsgRepository
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro);
    }
}
