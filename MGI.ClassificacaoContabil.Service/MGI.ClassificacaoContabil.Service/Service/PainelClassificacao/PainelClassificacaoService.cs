using DTO.Payload;
using Service.DTO.Classificacao;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Projeto;
using Service.Enum;
using Service.Helper;
using Service.Interface.Classificacao;
using Service.Interface.PainelClassificacao;
using Service.Interface.Parametrizacao;
using Service.Repository.PainelClassificacao;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;
        private Dictionary<int, string> tiposLancamento = new Dictionary<int, string>();
        private IClassificacaoContabilService _classificacaoContabilService;
        public readonly IParametrizacaoCenarioService _parametrizacaoCenarioService;
        public readonly IParametrizacaoEsgGeralService _parametrizacaoEsgGeralService;        
        private Dictionary<char, string> _tiposValores;
        private DateTime mesAnterior;
        private DateTime finalAno;
        private DateTime anoPosterior_inicio;
        private DateTime anoPosterior_fim;
        private DateTime mesAtual;       

        public PainelClassificacaoService(
            IPainelClassificacaoRepository PainelClassificacaoRepository,
            IParametrizacaoCenarioService parametrizacaoCenarioService,
            IParametrizacaoEsgGeralService parametrizacaoEsgGeralService,
            IClassificacaoContabilService classificacaoContabilService)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _parametrizacaoCenarioService = parametrizacaoCenarioService;
            _classificacaoContabilService = classificacaoContabilService;
            _parametrizacaoEsgGeralService = parametrizacaoEsgGeralService;
            tiposLancamento.Add(1, "Provisão de Manutenção");
            tiposLancamento.Add(2, "Intangível");
            tiposLancamento.Add(3, "Imobilizado");
            _tiposValores = new Dictionary<char, string>();
            _tiposValores.Add('O', "O");
            _tiposValores.Add('R', "R");
            _tiposValores.Add('T', "J");
            _tiposValores.Add('C', "1");
            _tiposValores.Add('P', "P");
            _tiposValores.Add('2', "2");
            SetDatas();
        }

        #region [Filtros]
        public async Task<PayloadDTO> FiltroPainelEmpresa(FiltroPainelEmpresa filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelEmpresa(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelProjeto(FiltroPainelProjeto filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelProjeto(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelDiretoria(FiltroPainelDiretoria filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelDiretoria(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGerencia(FiltroPainelGerencia filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGerencia(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGestor(FiltroPainelGestor filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGestor(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelGrupoPrograma(FiltroPainelGrupoPrograma filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelGrupoPrograma(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelPrograma(FiltroPainelPrograma filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelPrograma(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelCenario(FiltroPainelCenario filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelCenario(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelClassificacaoContabil(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> FiltroPainelClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            var resultado = await _PainelClassificacaoRepository.FiltroPainelClassificacaoESG(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }

        #endregion

        #region [Consuta da Tela]        

        private void SetDatas()
        {
            mesAnterior = DateTime.Now.AddMonths(-1);
            finalAno = new DateTime(DateTime.Now.Year, 12, 31);
            anoPosterior_inicio = new DateTime(DateTime.Now.Year + 1, 1, 1);
            anoPosterior_fim = new DateTime(DateTime.Now.Year + 1, 12, 31);
            mesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        public async Task<PainelClassificacaoContabilDTO> ConsultarClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            /*
                Formato acompanhamento (Ciclo Orçamentário ou Tendência) – Se selecionado o Ciclo Orçamentário será 
                considerado: Realizado até o mês anterior, Tendencia do mês atual até o último mês do ano vigente e ciclo a partir 
                do ano seguinte. Se selecionado Tendência será considerado: Realizado até o mês anterior e a tendência a partir do 
                mês atual.
             */
            /*
                se a data final do range for maior que o mês atual - 1, buscar os valores de tendencia até o final do ano atual e ciclo do ano posterior
                se a data final do range for menor que o mês atual - 1, buscar somente os valores realizado
                se a data inicial for maior que o mês atual -1, buscar somente valores de tendencia e ciclo
            */
            #region [ Predicados ]            
            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoRealizado = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoPrevisto = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoReplan = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoOrcado = _ => true;
            
            Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto_Realizado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseReplan = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;


            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_realizado = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_tendencia = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_ciclo = _ => true;

            Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_realizado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_tendencia = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcompFase_ciclo = _ => true;

            if (filtro.FormatAcompanhamento == 'C')
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (filtro.DataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio  && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (filtro.DataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;

                    predicateFormatoAcompFase_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcompFase_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (filtro.DataFim < mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                    predicateFormatoAcompFase_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }
            else if (filtro.FormatAcompanhamento == 'T')
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (filtro.DataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (filtro.DataFim < mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                }
                else if (filtro.DataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                }
            }

            predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateBaseOrcamentoReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateBaseOrcamentoOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);

            predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateFasePrevisto_Realizado = p => (p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado);
            predicateFaseReplan = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Replan);
            predicateFaseOrcado = p => (p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Orcado);

            #endregion
            IEnumerable<ClassificacaoContabilItemDTO> lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoContabil(filtro);
            IEnumerable<LancamentoFaseContabilDTO> lancamentosFase = await _PainelClassificacaoRepository.ConsultarLancamentosDaFase(new FiltroLancamentoFase()
            {
                IdEmpresa = filtro.IdEmpresa,
                IdGestor = filtro.IdGestor,
                IdGrupoPrograma = filtro.IdGrupoPrograma,
                IdPrograma = filtro.IdPrograma,
                IdProjeto = filtro.IdProjeto,
                DataInicio = filtro.DataInicio,
                DataFim = filtro.DataFim
            });
            var classificacoesMgp = await _classificacaoContabilService.ConsultarClassificacaoContabilMGP();

            PainelClassificacaoContabilDTO classificacaoContabil = new PainelClassificacaoContabilDTO();
            classificacaoContabil.Empresas = new List<EmpresaDTO>();
            var retorno = from a in lancamentos
                          group a by new { a.IdEmpresa, a.NomeEmpresa, a.IdClassifContabil, a.NomeClassifContabil } into grp
                          select new PainelClassificacaoContabilDTO()
                          {
                              Empresas = new List<EmpresaDTO>()
                              {
                                  new EmpresaDTO()
                                  {
                                      IdEmpresa = grp.Key.IdEmpresa,
                                      Nome = grp.Key.NomeEmpresa,
                                      IdClassifContabil = grp.Key.IdClassifContabil,
                                      #region [ Lancamentos ]
                                      Lancamentos = new LancamentoContabilDTO()
                                      {
                                          ValorBaseOrcamento = CalcularValorBaseOrcamento(grp, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                          ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grp, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),
                                          IdClassifContabil = grp.Key.IdClassifContabil,
                                          NomeTipoClassificacao = grp.Key.NomeClassifContabil
                                      },
                                      #endregion  
                                      GrupoPrograma = from gru in lancamentos
                                                      where gru.IdEmpresa == grp.Key.IdEmpresa
                                                      group gru by new { gru.IdEmpresa, gru.IdGrupoPrograma, gru.GrupoDePrograma, gru.IdClassifContabil } into grpGru
                                                      select new GrupoProgramaDTO()
                                                      {
                                                          IdGrupoPrograma = grpGru.Key.IdGrupoPrograma,
                                                          Nome = grpGru.Key.GrupoDePrograma,
                                                          IdClassifContabil = grpGru.Key.IdClassifContabil,
                                                          Lancamentos = new LancamentoContabilDTO()
                                                          {
                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpGru, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpGru, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),
                                                                IdClassifContabil = grp.Key.IdClassifContabil,
                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,                                                                
                                                          },
                                                          Programas = from gruPro in lancamentos
                                                                     where gruPro.IdEmpresa == grp.Key.IdEmpresa
                                                                        && gruPro.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                        && gruPro.IdClassifContabil == grpGru.Key.IdClassifContabil
                                                                     group gruPro by new { gruPro.IdEmpresa, gruPro.IdGrupoPrograma, gruPro.IdPrograma, gruPro.Programa, gruPro.IdClassifContabil } into grpGruPro
                                                                     select new ProgramaDTO()
                                                                     {
                                                                         CodPrograma = grpGruPro.Key.IdPrograma,
                                                                         Nome = grpGruPro.Key.Programa,
                                                                         IdClassifContabil = grp.Key.IdClassifContabil,
                                                                         Lancamentos = new LancamentoContabilDTO()
                                                                         {
                                                                                ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpGruPro, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),
                                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpGruPro, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,                                                                                
                                                                         },                                                                         
                                                                         Projetos = from prj in lancamentos
                                                                                    where prj.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                       && prj.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                       && prj.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                       //&& prj.IdClassifContabil == grpGruPro.Key.IdClassifContabil
                                                                                    group prj by new { prj.IdEmpresa, prj.IdGrupoPrograma, prj.IdPrograma, prj.IdProjeto, prj.NomeProjeto, prj.IdClassifContabil, prj.SeqFase } into grpPrj
                                                                                    select new ProjetoDTO()
                                                                                    {
                                                                                        CodProjeto = grpPrj.Key.IdProjeto,
                                                                                        Nome = grpPrj.Key.NomeProjeto,
                                                                                        IdClassifContabil = grp.Key.IdClassifContabil,
                                                                                        Lancamentos = new LancamentoContabilDTO()
                                                                                        {
                                                                                                ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpPrj, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),
                                                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpPrj, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                                        },
                                                                                        Fase = from fse in lancamentosFase
                                                                                                         where fse.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                             && fse.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                             && fse.SeqFase == grpPrj.Key.SeqFase
                                                                                                         group  fse by new { fse.IdEmpresa, fse.IdProjeto, fse.SeqFase, fse.NomeFase, fse.Pep } into grpFse
                                                                                                         select new FaseContabilDTO
                                                                                                         {
                                                                                                             IdEmpresa = grpFse.Key.IdEmpresa,
                                                                                                             FseSeq = grpFse.Key.SeqFase,
                                                                                                             Nome = grpFse.Key.NomeFase,
                                                                                                             Pep = grpFse.Key.Pep,
                                                                                                             IdClassifContabil = grp.Key.IdClassifContabil,
                                                                                                             Lancamentos = new LancamentoContabilDTO()
                                                                                                             {
                                                                                                                    NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                                                                    ValorBaseOrcamento = CalcularValorBaseOrcamento(grpFse, filtro.BaseOrcamento,predicateFasePrevisto, predicateFasePrevisto_Realizado, predicateFaseReplan, predicateFaseOrcado),
                                                                                                                    ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamentoFase(grpFse, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcompFase_realizado, predicateFormatoAcompFase_tendencia, predicateFormatoAcompFase_ciclo, filtro.DataFim, filtro.DataInicio),
                                                                                                            },
                                                                                                         }
                                                                                    }
                                                                     }
                                                      }
                                  }
                              },
                              Totalizador = new List<TotalizadorContabil>(),
                              Cabecalho = new List<ClassificacaoContabilMgpDTO>()
                          };

            var totais = from a in lancamentos
                         group a by new { a.IdEmpresa } into grp
                         select new PainelClassificacaoContabilDTO()
                         {
                             Totalizador = (from l in lancamentos
                                            where l.IdEmpresa == grp.Key.IdEmpresa
                                            group l by new { l.IdEmpresa } into grpLan
                                            select new TotalizadorContabil
                                            {
                                                IdEmpresa = grpLan.Key.IdEmpresa,
                                                TotalEmpresa = new LancamentoContabilTotalDTO()
                                                {
                                                    TotalBaseOrcamento = CalcularValorBaseOrcamento(grpLan, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                    TotalFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpLan, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),                                                    
                                                },
                                                TotalGrupoPrograma = from g in lancamentos
                                                                     where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                     group g by new { g.IdEmpresa, g.IdGrupoPrograma } into grpTotGru
                                                                     select new LancamentoContabilTotalDTO()
                                                                     {
                                                                         IdGrupoPrograma = grpTotGru.Key.IdGrupoPrograma,
                                                                         TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotGru, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                         TotalFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpTotGru, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio),
                                                                     },
                                                TotalPrograma = from g in lancamentos
                                                                where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma } into grpTotPrg
                                                                select new LancamentoContabilTotalDTO()
                                                                {
                                                                    IdGrupoPrograma = grpTotPrg.Key.IdGrupoPrograma,
                                                                    IdPrograma = grpTotPrg.Key.IdPrograma,
                                                                    TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotPrg, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                    TotalFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpTotPrg, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                                                },
                                                TotalProjeto = from g in lancamentos
                                                               where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                               group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma, g.IdProjeto, g.SeqFase } into grpTotPrj
                                                               select new LancamentoContabilTotalDTO()
                                                               {
                                                                   IdGrupoPrograma = grpTotPrj.Key.IdGrupoPrograma,
                                                                   IdPrograma = grpTotPrj.Key.IdPrograma,
                                                                   IdProjeto = grpTotPrj.Key.IdProjeto,
                                                                   TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotPrj, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                   TotalFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpTotPrj, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)                                                                   
                                                               },
                                                TotalFase = from g in lancamentos
                                                            where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                            group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma, g.IdProjeto, g.SeqFase } into grpTotFse
                                                            orderby grpTotFse.Key.SeqFase
                                                            select new LancamentoContabilTotalDTO()
                                                            {
                                                                IdGrupoPrograma = grpTotFse.Key.IdGrupoPrograma,
                                                                IdPrograma = grpTotFse.Key.IdPrograma,
                                                                IdProjeto = grpTotFse.Key.IdProjeto,
                                                                IdSeqFase = grpTotFse.Key.SeqFase,
                                                                TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotFse, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado, predicateBaseOrcamentoReplan, predicateBaseOrcamentoOrcado),
                                                                TotalFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpTotFse, filtro.FormatAcompanhamento.ToString(), predicateFormatoAcomp_realizado, predicateFormatoAcomp_tendencia, predicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                                            },

                                            }).ToList()
                         };

            var totalizador = retorno.SelectMany(p => p.Totalizador).ToList();
            totalizador.AddRange(totais.SelectMany(p => p.Totalizador));
            var empresa = retorno.SelectMany(p => p.Empresas).AsQueryable();
            var classifContabilEmpresa = empresa.Select(p => p.IdClassifContabil).Distinct().ToList();
            return new PainelClassificacaoContabilDTO()
            {
                Empresas = empresa.ToList(),
                Totalizador = totalizador,
                Cabecalho = classificacoesMgp.Where(p => classifContabilEmpresa.Contains(p.Id)).ToList()
            };
        }

        private decimal CalcularValorBaseOrcamento(IGrouping<object, ClassificacaoContabilItemDTO> lancamentos
            , string tipoOrcamento
            , Func<ClassificacaoContabilItemDTO, bool> predicatePrevisto
            , Func<ClassificacaoContabilItemDTO, bool> predicateRealizado
            , Func<ClassificacaoContabilItemDTO, bool> predicateReplan
            , Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoOrcado)
        {
            switch (tipoOrcamento)
            {
                case ETipoOrcamento.Previsto:
                    return lancamentos.Where(predicatePrevisto).Sum(p => p.ValorPrevisto) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
                case ETipoOrcamento.Replan:
                    return lancamentos.Where(predicateReplan).Sum(p => p.ValorReplan) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
                default:
                    return lancamentos.Where(predicateBaseOrcamentoOrcado).Sum(p => p.ValorOrcado) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
            }
        }

        private decimal CalcularValorBaseOrcamento(IGrouping<object, LancamentoFaseContabilDTO> lancamentos
            , string tipoOrcamento
            , Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto
            , Func<LancamentoFaseContabilDTO, bool> predicateFaseRealizado
            , Func<LancamentoFaseContabilDTO, bool> predicateFaseReplan
            , Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado)
        {
            decimal valor = 0;
            switch (tipoOrcamento)
            {
                case ETipoOrcamento.Previsto:
                    valor = lancamentos.Where(predicateFasePrevisto).Sum(p => p.ValorPrevisto) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
                    break;
                case ETipoOrcamento.Replan:
                    valor = lancamentos.Where(predicateFaseReplan).Sum(p => p.ValorReplan) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
                    break;
                default:
                    valor = lancamentos.Where(predicateFaseOrcado).Sum(p => p.ValorOrcado) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
                    break;
            }
            return valor;
        }
        
        private decimal CalcularValorFormaAcompanhamento(IGrouping<object, ClassificacaoContabilItemDTO> lancamentos
            , string formatoAcompanhamento
            , Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_realizado
            , Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_tendencia
            , Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_ciclo
            , DateTime dataFim
            , DateTime dataInicio)
        {
            switch (formatoAcompanhamento)
            {
                case "T":
                    {
                        if (dataInicio > mesAnterior)
                        {
                            return lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                        }
                        return lancamentos.Where(predicateFormatoAcomp_tendencia).Sum(p => p.ValorTendencia) +
                                lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                    }
                default:
                    {
                        if (dataInicio > mesAnterior)
                        {
                            return lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                        }                        
                        return lancamentos.Where(predicateFormatoAcomp_tendencia).Sum(p => p.ValorTendencia) +
                                lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado) +
                                lancamentos.Where(predicateFormatoAcomp_ciclo).Sum(p => p.ValorCiclo);
                    }
            }
        }

        private decimal CalcularValorFormaAcompanhamentoFase(IGrouping<object, LancamentoFaseContabilDTO> lancamentos
            , string formatoAcompanhamento
            , Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcomp_realizado
            , Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcomp_tendencia
            , Func<LancamentoFaseContabilDTO, bool> predicateFormatoAcomp_ciclo
            , DateTime dataFim
            , DateTime dataInicio)
        {
            switch (formatoAcompanhamento)
            {
                case "T":
                    {
                        if (dataInicio > mesAnterior)
                        {
                            return lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                        }
                        return lancamentos.Where(predicateFormatoAcomp_tendencia).Sum(p => p.ValorTendencia) +
                                lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                    }
                default:
                    {
                        if (dataInicio > mesAnterior)
                        {
                            return lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado);
                        }
                        return lancamentos.Where(predicateFormatoAcomp_tendencia).Sum(p => p.ValorTendencia) +
                                lancamentos.Where(predicateFormatoAcomp_realizado).Sum(p => p.ValorRealizado) +
                                lancamentos.Where(predicateFormatoAcomp_ciclo).Sum(p => p.ValorCiclo);
                    }
            }
        }


        public async Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro)
        {
            return await _PainelClassificacaoRepository.ConsultarLancamentoSap(filtro);
        }        
        public async Task<byte[]> GerarRelatorioContabil(FiltroPainelClassificacaoContabil filtro)
        {
            var tipoValor = _tiposValores.GetValueOrDefault(filtro.ValorInvestimento.Value);
            IEnumerable<RelatorioContabilDTO> dados = await _PainelClassificacaoRepository.ConsultarDadosRelatorio(filtro);
            var dadosExcel = from a in dados
                             where a.DtLancamentoProjeto >= filtro.DataInicio && a.DtLancamentoProjeto <= filtro.DataFim
                             && a.TipoValorProjeto == tipoValor
                             group a by new { a.CodExterno, a.TxDepreciacao, a.QtdProdutcaoTotal, a.SaldoInicialAndamento, a.TxImobilizado, a.Data, a.TxProducao, a.TxTransfDespesa } into grp
                             select new RelatorioContabilExcelDTO()
                             {
                                 CodExterno = grp.Key.CodExterno,
                                 TxDepreciacao = grp.Key.TxDepreciacao,
                                 QtdProdutcaoTotal = grp.Key.QtdProdutcaoTotal,
                                 SaldoInicialAndamento = grp.Key.SaldoInicialAndamento,
                                 TxImobilizado = grp.Key.TxImobilizado,
                                 Data = grp.Key.Data,
                                 TxProducao = grp.Key.TxProducao,
                                 TxTransfDespesa = grp.Key.TxTransfDespesa,
                                 ValorInvestimento = grp.Sum(p => p.ValorProjeto)
                             };
            return ReportService.GerarExcel(dadosExcel);
        }        

        #endregion

    }
}
