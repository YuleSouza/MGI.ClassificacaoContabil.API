using Service.DTO.Parametrizacao;

namespace Service.Repository.Parametrizacao
{
    public interface IParametrizacaoRepository
    {
        #region Parametrização Cenario
        Task<bool> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<bool> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<IEnumerable<ParametrizacaoCenarioDTO>> ConsultarParametrizacaoCenario();
        #endregion

        #region Parametrização Classificacação ESG Geral
        Task<bool> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<bool> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao);
        Task<IEnumerable<ParametrizacaoClassificacaoGeralDTO>> ConsultarParametrizacaoClassificacaoGeral();
        #endregion

        #region Parametrização Classificacação ESG Exceção
        Task<bool> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<bool> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>> ConsultarParametrizacaoClassificacaoExcecao();
        #endregion
    }
}
