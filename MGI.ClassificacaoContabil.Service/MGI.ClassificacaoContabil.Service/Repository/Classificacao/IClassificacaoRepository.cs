﻿using Service.DTO.Classificacao;
using Service.DTO.Filtros;

namespace Service.Repository.Classificacao
{
    public interface IClassificacaoRepository
    {
        #region Contabil
        Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro);
        #endregion

        #region ESG
        Task<bool> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<bool> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);
        #endregion
    }
}
