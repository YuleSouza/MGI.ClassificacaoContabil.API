using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;

namespace Service.Interface.Classificacao
{
    public interface IClassificacaoEsgService
    {

        Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> ConsultarClassificacaoEsg();
        Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);

    }
}
