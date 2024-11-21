using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;

namespace Service.Interface.PainelClassificacao
{
    public interface IPainelClassificacaoEsgService
    {
        Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro);        
        Task<int> ConsultarClassifEsgPorCenario(FiltroPainelClassificacaoEsg filtro);
        Task<byte[]> GerarRelatorioEsg(FiltroPainelClassificacaoEsg filtro);
    }
}
