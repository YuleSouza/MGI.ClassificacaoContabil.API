using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Interface.PainelEsg
{
    public interface IPainelEsgService
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro);
    }
}