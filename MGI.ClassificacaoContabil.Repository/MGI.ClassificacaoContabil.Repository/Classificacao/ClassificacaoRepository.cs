﻿using Dapper;

using Infra.Data;
using Infra.Interface;

using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.Repository.Classificacao;


namespace Repository.Classificacao
{
    public class ClassificacaoRepository : IClassificacaoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSession _session;

        #region Contabil
        public ClassificacaoRepository(IUnitOfWork unitOfWork, DbSession session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
        }
        public async Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classificacao_contabil (id_empresa, nome, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao) 
                                                                  values (:idempresa, :nome, :status, :dataInicial, :dataFinal, :uscriacao, sysdate)",
            new
            {
                idempresa = classificacao.IdEmpresa,
                nome = classificacao.Nome,
                status = classificacao.Status,
                dataInicial = classificacao.MesAnoInicio,
                dataFinal = classificacao.MesAnoFim,
                uscriacao = classificacao.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classificacao_contabil 
                                                                     set id_empresa      = :idempresa,
                                                                         nome            = nvl(:nome,nome),  
                                                                         status          = nvl(:status,status), 
                                                                         mesano_inicio   = :dataInicial,
                                                                         mesano_fim      = :dataFinal,
                                                                         usalteracao     = :usalteracao, 
                                                                         dtalteracao     = :dtalteracao
                                                                   where id_classificacao_contabil = :idclassificacao",
            new
            {
                idclassificacao = classificacao.IdClassificacaoContabil,
                idempresa = classificacao.IdEmpresa,
                nome = classificacao.Nome,
                status = classificacao.Status,
                dataInicial = classificacao.MesAnoInicio,
                dataFinal = classificacao.MesAnoFim,
                usalteracao = classificacao.Usuario?.UsuarioModificacao,
                dtalteracao = classificacao.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro)
        {
            string parametros = string.Empty;
            if (filtro.IdClassificacaoContabil > 0)
            {
                parametros += " and id_classificacao_contabil = :idclassificacao";
            }
            if (filtro.IdEmpresa > 0)
            {
                parametros += " and id_empresa = :idempresa";
            }
            if (!string.IsNullOrEmpty(filtro.DataInicial))
            {
                parametros += " and mesano_inicio >= to_date(:dataInicial,'DD/MM/RRRR')";
            }
            if (!string.IsNullOrEmpty(filtro.DataFinal))
            {
                parametros += " and mesano_fim <= to_date(:dataFinal,'DD/MM/RRRR')";
            }
            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilDTO>($@"
                                                    select id_classificacao_contabil  as IdClassificacaoContabil, 
                                                           id_empresa                 as IdEmpresa,
                                                           nome                       as Nome,
                                                           status                     as Status, 
                                                           mesano_inicio              as MesAnoInicio,
                                                           mesano_fim                 as MesAnoFim,
                                                           dtcriacao                  as DataCriacao,
                                                           uscriacao                  as UsuarioCriacao,
                                                           dtalteracao                as DataModificacao,
                                                           usalteracao                as UsuarioModificacao
                                                      from classificacao_contabil 
                                                     where 1 = 1
                                                     {parametros}
                                                     order by mesano_fim
                                        ", new
            {
                idclassificacao = filtro.IdClassificacaoContabil,
                idempresa = filtro.IdEmpresa,
                dataInicial = filtro.DataInicial,
                dataFinal = filtro.DataFinal
            });
            return resultado;
        }
        #endregion

        #region ESG
        public async Task<bool> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classificacao_esg (nome, status, uscriacao, dtcriacao) 
                                                                  values (:nome, :status, :uscriacao, sysdate)",
            new
            {
                nome = classificacao.Nome,
                status = classificacao.Status,
                uscriacao = classificacao.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classificacao_esg 
                                                                     set nome = nvl(:nome,nome),  
                                                                         status = nvl(:status,status), 
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = :dtalteracao
                                                                   where id_classificacao_esg = :idclassificacaoesg",
            new
            {
                idclassificacaoesg = classificacao.IdClassificacaoEsg,
                nome = classificacao.Nome,
                status = classificacao.Status,
                usalteracao = classificacao.Usuario?.UsuarioModificacao,
                dtalteracao = classificacao.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro)
        {
            var parametros = string.Empty;
            if (filtro.IdClassificacaoEsg > 0)
            {
                parametros += $" and id_classificacao_esg = :idclassificacaoesg";
            }
            if (!string.IsNullOrEmpty(filtro.Nome))
            {
                parametros += " and upper(nome) like upper(:nome)";
            }
            if (!string.IsNullOrEmpty(filtro.Status))
            {
                parametros += $" and status = :status";
            }

            var resultado = await _session.Connection.QueryAsync<ClassificacaoEsgDTO>($@"
                                           select 
                                                id_classificacao_esg  as IdClassificacaoEsg,
                                                nome                as Nome,
                                                status              as Status,
                                                dtcriacao           as DataCriacao,
                                                uscriacao           as UsuarioCriacao,
                                                dtalteracao         as DataModificacao,
                                                usalteracao         as UsuarioModificacao
                                            from classificacao_esg
                                            where 1 = 1
                                           {parametros}
                                        ", new
            {
                idclassificacaoesg = filtro.IdClassificacaoEsg,
                nome = $"%{filtro.Nome}%",
                status = filtro.Status
            });
            return resultado;
        }
        #endregion
    }
}
