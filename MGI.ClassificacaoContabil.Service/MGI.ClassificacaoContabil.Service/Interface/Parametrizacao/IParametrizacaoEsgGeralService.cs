using DTO.Payload;
using Service.DTO.Parametrizacao;

namespace Service.Interface.Parametrizacao
{
    public interface IParametrizacaoEsgGeralService
    {
        Task<PayloadDTO> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoGeralDTO>>> ConsultarParametrizacaoClassificacaoGeral();
    }
}
