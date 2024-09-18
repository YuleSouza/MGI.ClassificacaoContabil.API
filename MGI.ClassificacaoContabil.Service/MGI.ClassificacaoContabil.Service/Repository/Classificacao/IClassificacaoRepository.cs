using Service.DTO.Filtros;
using Service.DTO.Classificacao;

namespace Service.Repository.Classificacao
{
    public interface IClassificacaoRepository
    {
        #region Contabil
        Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil();
        Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(FiltroClassificacaoContabil filtro);

        Task<bool> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);
        Task<bool> InserirProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos);

        Task<bool> AlterarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos);
        Task<bool> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);

        Task<bool> DeletarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos);

        Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil();
        Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil(FiltroClassificacaoContabil filtro);
        Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP();
        #endregion

        #region ESG
        Task<bool> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<bool> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao); 
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);
        #endregion
    }
}
