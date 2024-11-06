using DTO.Payload;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;

namespace Service.Interface.Classificacao
{
    public interface IClassificacaoContabilService
    {
        Task<PayloadDTO> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<PayloadDTO> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<PayloadDTO> ConsultarClassificacaoContabil();

        Task<PayloadDTO> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);
        Task<PayloadDTO> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);

        Task<PayloadDTO> ConsultarProjetoClassificacaoContabil();
        Task<PayloadDTO> ConsultarProjetoClassificacaoContabil(FiltroClassificacaoContabil filtro);
        Task<bool> VerificarRegraExcessaoContabil(FiltroClassificacaoContabil filtro);
        Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP();
    }
}
