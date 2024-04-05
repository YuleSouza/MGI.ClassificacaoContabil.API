using Service.DTO.Classificacao;
using Service.DTO.Filtros;

namespace Service.Repository.Classificacao
{
    public interface IClassificacaoRepository
    {
        #region Contabil
        Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil();
        Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro);

        Task<bool> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);
        Task<bool> InserirProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos);

        Task<bool> AlterarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos);
        Task<bool> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);


        Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil();
        Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil(ClassificacaoContabilFiltro filtro);
        #endregion

        #region ESG
        Task<bool> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<bool> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao); 
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);
        #endregion
    }
}
