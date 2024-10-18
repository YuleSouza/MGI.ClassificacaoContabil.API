using Dapper;
using Infra.Data;
using Infra.Interface;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;

using System.Data;
using Service.Repository.Classificacao;
using Dapper.Oracle.BulkSql;


namespace Repository.Classificacao
{
    public class ClassificacaoRepository : IClassificacaoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSession _session;

        public ClassificacaoRepository(IUnitOfWork unitOfWork, DbSession session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
        }
        #region Contabil
        public async Task<bool> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classificacao_contabil (id_empresa, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao, dat_termino_concessao) 
                                                                  values (:idempresa, :status, :dataInicial, :dataFinal, :uscriacao, sysdate, :dat_termino_concessao)",
            new
            {
                idempresa = classificacao.IdEmpresa,
                status = 1,
                dataInicial = classificacao.MesAnoInicio!.Value.ToString("01/01/yyyy"),
                dataFinal = classificacao.MesAnoFim!.Value.ToString("01/01/yyyy"),
                dat_termino_concessao = classificacao.DataTerminoConcessao!.Value.ToString("01/MM/yyyy"),
                uscriacao = classificacao.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classificacao_contabil 
                                                                     set id_empresa      = :idempresa,
                                                                         status          = nvl(:status,status), 
                                                                         mesano_inicio   = :dataInicial,
                                                                         mesano_fim      = :dataFinal,
                                                                         usalteracao     = :usalteracao, 
                                                                         dtalteracao     = :dtalteracao,
                                                                         dat_termino_concessao     = :dat_termino_concessao
                                                                   where id_classificacao_contabil = :idclassificacao",
            new
            {
                idclassificacao = classificacao.IdClassificacaoContabil,
                idempresa = classificacao.IdEmpresa,
                status = classificacao.Status,
                dataInicial = classificacao.MesAnoInicio!.Value.ToString("01/01/yyyy"),
                dataFinal = classificacao.MesAnoFim!.Value.ToString("01/01/yyyy"),
                usalteracao = classificacao.Usuario?.UsuarioModificacao,
                dtalteracao = classificacao.Usuario?.DataModificacao,
                dat_termino_concessao = classificacao.DataTerminoConcessao!.Value.ToString("01/MM/yyyy"),
            });
            return result == 1;
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil()
        {

            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilDTO>($@"
                                                    select nvl(cc.id_classificacao_contabil,0)  as IdClassificacaoContabil, 
                                                           a.empcod                      as idEmpresa,         
                                                           ltrim(rtrim(a.empnomfan))     as NomeEmpresa,
                                                           cc.status                     as Status, 
                                                           nvl(cc.mesano_inicio,null)              as MesAnoInicio,
                                                           nvl(cc.mesano_fim,null)                 as MesAnoFim,
                                                           cc.dtcriacao                  as DataCriacao,
                                                           cc.uscriacao                  as UsuarioCriacao,
                                                           cc.dtalteracao                as DataModificacao,
                                                           cc.usalteracao                as UsuarioModificacao,
                                                           nvl(cc.dat_termino_concessao,null) as DataTerminoConcessao
                                                      from corpora.empres a
                                                            left join classificacao_contabil cc  on (cc.id_empresa = a.empcod)
                                                     where 1 = 1
                                                       and a.empsit = 'A'
                                                     order by mesano_fim");
            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil(FiltroClassificacaoContabil filtro)
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
            if (filtro.IdProjeto > 0)
            {
                parametros += " and id_projeto = :idprojeto";
            }
            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilDTO>($@"
                                                    select nvl(cc.id_classificacao_contabil,0)   as IdClassificacaoContabil, 
                                                           a.empcod                              as IdEmpresa,
                                                           ltrim(rtrim(a.empnomfan))             as NomeEmpresa,
                                                           cc.status                             as Status,
                                                           nvl(cc.mesano_inicio,sysdate)         as MesAnoInicio,
                                                           nvl(cc.mesano_fim,sysdate)            as MesAnoFim,
                                                           cc.dtcriacao                          as DataCriacao,
                                                           cc.uscriacao                          as UsuarioCriacao,
                                                           cc.dtalteracao                        as DataModificacao,
                                                           cc.usalteracao                        as UsuarioModificacao,
                                                           nvl(cc.dat_termino_concessao,sysdate) as DataTerminoConcessao
                                                     from classificacao_contabil cc 
                                                            right join corpora.empres a on (cc.id_empresa = a.empcod and a.empsit = 'A')
                                                     where 1 = 1
                                                     {parametros}
                                                     order by mesano_fim
                                        ", new
            {
                idclassificacao = filtro.IdClassificacaoContabil,
                idempresa = filtro.IdEmpresa
            });
            return resultado;
        }

        public async Task<bool> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into classif_contabil_prj (id_classificacao_contabil, id_projeto, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaocontabil, :idprojeto, :status, :dataInicial, :dataFinal, :uscriacao, sysdate)",
            new
            {
                idclassificacaocontabil = projeto.IdClassificacaoContabil,
                idprojeto = projeto.IdProjeto,
                status = projeto.Status,
                dataInicial = projeto.MesAnoInicio,
                dataFinal = projeto.MesAnoFim,
                uscriacao = projeto.Usuario?.UsuarioCriacao
            });
            return result == 1;
        }
        public async Task<bool> InserirProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos)
        {
            if (projetos.Count > 0)
            {
                IList<BulkMapping<ClassificacaoProjetoDTO>> bulkMappings = new List<BulkMapping<ClassificacaoProjetoDTO>>();
                var prop = new BulkMapping<ClassificacaoProjetoDTO>("id_classificacao_contabil", p => p.IdClassificacaoContabil, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("id_projeto", p => p.IdProjeto, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("status", p => p.Status, Dapper.Oracle.OracleMappingType.Char);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("mesano_inicio", p => p.MesAnoInicio, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("mesano_fim", p => p.MesAnoFim, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("uscriacao", p => p.Usuario?.UsuarioCriacao, Dapper.Oracle.OracleMappingType.Varchar2);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("dtcriacao", p => p.Usuario?.DataCriacao, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                var bulk = await BulkOperation.SqlBulkAsync(_session.Connection, @"insert into classif_contabil_prj (id_classificacao_contabil, id_projeto, status, mesano_inicio, mesano_fim, uscriacao, dtcriacao)  
                                                                                 values (:id_classificacao_contabil, :id_projeto, :status, :mesano_inicio, :mesano_fim, :uscriacao, sysdate)", projetos.AsEnumerable(), bulkMappings.AsEnumerable());
            }
            return true;
        }


        public async Task<bool> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto)
        {
            int result = await _session.Connection.ExecuteAsync(@"update classif_contabil_prj 
                                                                     set id_classificacao_contabil      = :idclassificacaocontabil,
                                                                         id_projeto                     = :idprojeto,
                                                                         status                         = nvl(:status,status), 
                                                                         mesano_inicio                  = :dataInicial,
                                                                         mesano_fim                     = :dataFinal,
                                                                         usalteracao                    = :usalteracao, 
                                                                         dtalteracao                    = :dtalteracao
                                                                   where id_classif_contabil_prj        = :idclassificacaoprojeto",
            new
            {
                idclassificacaoprojeto = projeto.IdClassificacaoContabilProjeto,
                idclassificacaocontabil = projeto.IdClassificacaoContabil,
                idprojeto = projeto.IdProjeto,
                status = projeto.Status,
                dataInicial = projeto.MesAnoInicio,
                dataFinal = projeto.MesAnoFim,
                usalteracao = projeto.Usuario?.UsuarioModificacao,
                dtalteracao = projeto.Usuario?.DataModificacao
            });
            return result == 1;
        }
        public async Task<bool> AlterarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos)
        {
            if (projetos.Count > 0)
            {
                IList<BulkMapping<ClassificacaoProjetoDTO>> bulkMappings = new List<BulkMapping<ClassificacaoProjetoDTO>>();
                var prop = new BulkMapping<ClassificacaoProjetoDTO>("id_classificacao_contabil", p => p.IdClassificacaoContabil, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("id_classif_contabil_prj", p => p.IdClassificacaoContabilProjeto, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("id_projeto", p => p.IdProjeto, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("status", p => p.Status, Dapper.Oracle.OracleMappingType.Char);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("mesano_inicio", p => p.MesAnoInicio, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("mesano_fim", p => p.MesAnoFim, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("usalteracao", p => p.Usuario?.UsuarioModificacao, Dapper.Oracle.OracleMappingType.Varchar2);
                bulkMappings.Add(prop);
                prop = new BulkMapping<ClassificacaoProjetoDTO>("dtalteracao", p => p.Usuario?.DataModificacao, Dapper.Oracle.OracleMappingType.Date);
                bulkMappings.Add(prop);
                var bulk = await BulkOperation.SqlBulkAsync(_session.Connection, @"update classif_contabil_prj 
                                                                                     set id_classificacao_contabil      = nvl(:id_classificacao_contabil, id_classificacao_contabil),
                                                                                         id_projeto                     = nvl(:id_projeto, id_projeto),
                                                                                         status                         = nvl(:status, status), 
                                                                                         mesano_inicio                  = nvl(:mesano_inicio, mesano_inicio),
                                                                                         mesano_fim                     = nvl(:mesano_fim, mesano_fim),
                                                                                         usalteracao                    = :usalteracao, 
                                                                                         dtalteracao                    = :dtalteracao
                                                                                   where id_classif_contabil_prj        = :id_classif_contabil_prj
                                                                                 ", projetos.AsEnumerable(), bulkMappings.AsEnumerable(), CommandType.Text);
            }
            return true;
        }

        public async Task<bool> DeletarProjetosClassificacaoContabil(IList<ClassificacaoProjetoDTO> projetos)
        {
            if (projetos.Count > 0)
            {
                IList<BulkMapping<ClassificacaoProjetoDTO>> bulkMappings = new List<BulkMapping<ClassificacaoProjetoDTO>>();
                var prop = new BulkMapping<ClassificacaoProjetoDTO>("id_classif_contabil_prj", p => p.IdClassificacaoContabilProjeto, Dapper.Oracle.OracleMappingType.Int32);
                bulkMappings.Add(prop);
                var bulk = await BulkOperation.SqlBulkAsync(_session.Connection, @"delete classif_contabil_prj 
                                                                                   where id_classif_contabil_prj = :id_classif_contabil_prj
                                                                                 ", projetos.AsEnumerable(), bulkMappings.AsEnumerable(), CommandType.Text);
            }
            return true;
        }

        public async Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil()
        {

            var resultado = await _session.Connection.QueryAsync<ClassificacaoProjetoDTO>($@"
                                                    select cp.id_classif_contabil_prj                                    as IdClassificacaoContabilProjeto, 
                                                           cp.id_classificacao_contabil                                  as IdClassificacaoContabil,
                                                           cp.id_projeto                                                 as IdProjeto,  
                                                           to_char(p.prjcod, '00000') || ' - ' || ltrim(rtrim(p.prjnom)) as Nomeprojeto,
                                                           cp.status                                                     as Status, 
                                                           cp.mesano_inicio                                              as MesAnoInicio,
                                                           cp.mesano_fim                                                 as MesAnoFim,
                                                           cp.dtcriacao                                                  as DataCriacao,
                                                           cp.uscriacao                                                  as UsuarioCriacao,
                                                           cp.dtalteracao                                                as DataModificacao,
                                                           cp.usalteracao                                                as UsuarioModificacao
                                                     from classif_contabil_prj cp 
                                                     inner join servdesk.projeto p on p.prjcod = cp.id_projeto
                                                     where 1 = 1 
                                                     and p.prjsit = 'A'
                                                     order by mesano_fim");
            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoProjetoDTO>> ConsultarProjetoClassificacaoContabil(FiltroClassificacaoContabil filtro)
        {
            string parametros = string.Empty;
            if (filtro.IdClassificacaoContabil >= 0)
            {
                parametros += " and cp.id_classificacao_contabil = :idclassificacao";
            }
            if (filtro.IdProjeto >= 0)
            {
                parametros += " and cp.id_projeto = :idprojeto";
            }
            if (filtro.Ano.HasValue && filtro.Ano >= 0)
            {
                parametros += " and :ano between extract(year from cp.mesano_inicio)  and extract(year from cp.mesano_fim)";
            }
            var resultado = await _session.Connection.QueryAsync<ClassificacaoProjetoDTO>($@"
                                                    select cp.id_classif_contabil_prj                                    as IdClassificacaoContabilProjeto, 
                                                           cp.id_classificacao_contabil                                  as IdClassificacaoContabil,
                                                           cp.id_projeto                                                 as IdProjeto,  
                                                           to_char(p.prjcod, '00000') || ' - ' || ltrim(rtrim(p.prjnom)) as Nomeprojeto,
                                                           cp.status                                                     as Status, 
                                                           cp.mesano_inicio                                              as MesAnoInicio,
                                                           cp.mesano_fim                                                 as MesAnoFim,
                                                           cp.dtcriacao                                                  as DataCriacao,
                                                           cp.uscriacao                                                  as UsuarioCriacao,
                                                           cp.dtalteracao                                                as DataModificacao,
                                                           cp.usalteracao                                                as UsuarioModificacao
                                                     from classif_contabil_prj cp 
                                                     inner join servdesk.projeto p on p.prjcod = cp.id_projeto
                                                     where 1 = 1 
                                                     and p.prjsit = 'A'
                                                     {parametros}
                                                     order by mesano_fim
                                        ", new
            {
                idclassificacao = filtro.IdClassificacaoContabil,
                idempresa = filtro.IdEmpresa,
                idprojeto = filtro.IdProjeto,
                ano = filtro.Ano,
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
        public async Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg()
        {
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
                                              and status = 'A'");
            return resultado;
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

        public async Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP()
        {
            return await _session.Connection.QueryAsync<ClassificacaoContabilMgpDTO>("select ccocod as Id, clacon.cconom as Nome from clacon where ccosit = 'A'");
        }
        #endregion
    } 
}
