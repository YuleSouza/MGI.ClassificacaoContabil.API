using Service.DTO.Cenario;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.Enum;
using Service.Helper;
using Service.Interface.Cenario;
using Service.Interface.Classificacao;
using Service.Interface.PainelClassificacao;
using Service.Interface.Parametrizacao;
using Service.Repository.PainelClassificacao;
using System;

namespace MGI.ClassificacaoContabil.Service.Service.PainelClassificacao
{
    public class PainelClassificacaoEsgService : IPainelClassificacaoEsgService
    {
        private readonly IPainelClassificacaoRepository _painelClassificacaoRepository;
        private IClassificacaoEsgService _classificacaoEsgService;
        private ICenarioService _cenarioService;
        private readonly IParametrizacaoService _parametrizacaoService;
        public readonly IParametrizacaoCenarioService _parametrizacaoCenarioService;
        public readonly IParametrizacaoEsgGeralService _parametrizacaoEsgGeralService;
        private IEnumerable<ParametrizacaoCenarioDTO> _parametrizacaoCenarioDTOs;
        private IEnumerable<ParametrizacaoClassificacaoGeralDTO> _parametrizacaoGrupoDTOs;
        private IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO> _parametrizacaoExecoes;

        private DateTime mesAnterior;
        private DateTime finalAno;
        private DateTime anoPosterior_inicio;
        private DateTime anoPosterior_fim;
        private DateTime mesAtual;

        private PredicateFactory<LancamentoClassificacaoEsgDTO> predicate;
        private PredicateFactory<LancamentoFaseContabilDTO> _predicateFase;
        public PainelClassificacaoEsgService(
            IPainelClassificacaoRepository painelClassificacaoRepository,
            IClassificacaoEsgService classificacaoEsgService,
            ICenarioService cenarioService,
            IParametrizacaoService parametrizacaoService,
            IParametrizacaoCenarioService parametrizacaoCenarioService,
            IParametrizacaoEsgGeralService parametrizacaoEsgGeralService)
        {
            _painelClassificacaoRepository = painelClassificacaoRepository;
            _classificacaoEsgService = classificacaoEsgService;
            _cenarioService = cenarioService;
            _parametrizacaoService = parametrizacaoService;
            _parametrizacaoCenarioService = parametrizacaoCenarioService;
            _parametrizacaoEsgGeralService = parametrizacaoEsgGeralService;
            predicate = new PredicateFactory<LancamentoClassificacaoEsgDTO>(new LancamentoClassificacaoEsgDTO());
            _predicateFase = new PredicateFactory<LancamentoFaseContabilDTO>(new LancamentoFaseContabilDTO());
            SetDatas();
        }

        private void SetDatas()
        {
            mesAnterior = DateTime.Now.AddMonths(-1);
            finalAno = new DateTime(DateTime.Now.Year, 12, 31);
            anoPosterior_inicio = new DateTime(DateTime.Now.Year + 1, 1, 1);
            anoPosterior_fim = new DateTime(DateTime.Now.Year + 1, 12, 31);
            mesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        public async Task<int> ConsultarClassifEsgPorCenario(FiltroPainelClassificacaoEsg filtro)
        {
            await PopularParametrizacoes();
            var retorno = await _painelClassificacaoRepository.ConsultarClassifEsgPorProjeto(filtro.IdProjeto.Value, filtro.SeqFase.Value, filtro.IdEmpresa);
            filtro.IdPrograma = retorno.IdPrograma;
            filtro.IdGrupoPrograma = retorno.IdGrupoPrograma;
            filtro.IdCenario = filtro.IdCenario;
            (int id, string descricao) = RetornarClassificacaoEsg(filtro);
            return id;
        }

        private void SetPredicates()
        {
            
        }
        public async Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            var p = new PredicateHelper(filtro.FormatAcompanhamento, filtro.DataFim, filtro.DataInicio, "E");

            var lancamentos = await _painelClassificacaoRepository.ConsultarClassificacaoEsg(filtro);
            var lancamentosFase = await _painelClassificacaoRepository.ConsultarLancamentosDaFase(new FiltroLancamentoFase()
            {
                IdEmpresa = filtro.IdEmpresa,
                IdGestor = filtro.IdGestor,
                IdGrupoPrograma = filtro.IdGrupoPrograma,
                IdPrograma = filtro.IdPrograma,
                IdProjeto = filtro.IdProjeto,
                DataFim = filtro.DataFim,
                DataInicio = filtro.DataInicio
            });
            await PopularParametrizacoes();
            (int, string) esgClassif = RetornarClassificacaoEsg(filtro);
            foreach (var item in lancamentos)
            {
                filtro.IdProjeto = item.IdProjeto;
                filtro.IdPrograma = item.IdPrograma;
                filtro.IdGrupoPrograma = Convert.ToInt32(item.IdGrupoPrograma);
                esgClassif = RetornarClassificacaoEsg(filtro);
                if (esgClassif.Item1 == filtro.IdCenario)
                {
                    item.IdClassificacaoEsg = esgClassif.Item1;
                    item.NomeClassificacaoEsg = esgClassif.Item2;
                }
            }
            var classificacoesEsg = await _classificacaoEsgService.ConsultarClassificacaoEsg();
            if (classificacoesEsg.ObjetoRetorno != null)
            {
                IEnumerable<ClassificacaoEsgDTO> classificacaoEsgDTO = (IEnumerable<ClassificacaoEsgDTO>)classificacoesEsg.ObjetoRetorno;
                CabecalhoEsg cabecalhoEsg = new CabecalhoEsg()
                {
                    IdClassificacaoEsg = esgClassif.Item1,
                    Nome = esgClassif.Item2
                };

                var retorno = new PainelClassificacaoEsg()
                {
                    Cabecalho = cabecalhoEsg,
                    Empresas = from a in lancamentos
                               group a by new { a.IdEmpresa, a.IdClassificacaoEsg, a.NomeEmpresa } into grp
                               select new EmpresaEsgDTO()
                               {
                                   IdEmpresa = grp.Key.IdEmpresa,
                                   Nome = grp.Key.NomeEmpresa,
                                   LancamentoESG = new LancamentoESG()
                                   {
                                       IdClassificacaoEsg = grp.Key.IdClassificacaoEsg,
                                       ValorBaseOrcamento = CalcularValorBaseOrcamento(grp, filtro.BaseOrcamento, p.PredicateBaseOrcamentoPrevisto, p.PredicateBaseOrcamentoRealizado, p.PredicateBaseOrcamentoReplan, p.PredicateBaseOrcamentoOrcado),
                                       ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grp, filtro.FormatAcompanhamento, p.PredicateFormatoAcomp_realizado, p.PredicateFormatoAcomp_tendencia, p.PredicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                   },
                                   GrupoPrograma = from c in lancamentos
                                                   where c.IdEmpresa == grp.Key.IdEmpresa
                                                   group c by new { c.IdEmpresa, c.IdGrupoPrograma, c.GrupoDePrograma, c.IdClassificacaoEsg } into grpGru
                                                   select new GrupoProgramaEsgDTO()
                                                   {
                                                       IdGrupoPrograma = grpGru.Key.IdGrupoPrograma,
                                                       Nome = grpGru.Key.GrupoDePrograma,
                                                       LancamentoESG = new LancamentoESG()
                                                       {
                                                           IdClassificacaoEsg = grpGru.Key.IdClassificacaoEsg,
                                                           ValorBaseOrcamento = CalcularValorBaseOrcamento(grpGru, filtro.BaseOrcamento, p.PredicateBaseOrcamentoPrevisto, p.PredicateBaseOrcamentoRealizado, p.PredicateBaseOrcamentoReplan, p.PredicateBaseOrcamentoOrcado),
                                                           ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpGru, filtro.FormatAcompanhamento, p.PredicateFormatoAcomp_realizado, p.PredicateFormatoAcomp_tendencia, p.PredicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                                       },
                                                       Programas = from p in lancamentos
                                                                   where p.IdEmpresa == grp.Key.IdEmpresa
                                                                   && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                   && p.IdClassificacaoEsg == grpGru.Key.IdClassificacaoEsg
                                                                   group p by new { p.IdEmpresa, p.IdGrupoPrograma, p.IdPrograma, p.Programa, p.IdClassificacaoEsg } into grpPro
                                                                   select new ProgramaEsgDTO()
                                                                   {
                                                                       IdPrograma = grpPro.Key.IdPrograma,
                                                                       Nome = grpPro.Key.Programa,
                                                                       LancamentoESG = new LancamentoESG()
                                                                       {
                                                                           IdClassificacaoEsg = grpPro.Key.IdClassificacaoEsg,
                                                                           ValorBaseOrcamento = CalcularValorBaseOrcamento(grpPro, filtro.BaseOrcamento, p.PredicateBaseOrcamentoPrevisto, p.PredicateBaseOrcamentoRealizado, p.PredicateBaseOrcamentoReplan, p.PredicateBaseOrcamentoOrcado),
                                                                           ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpPro, filtro.FormatAcompanhamento, p.PredicateFormatoAcomp_realizado, p.PredicateFormatoAcomp_tendencia, p.PredicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                                                       },
                                                                       Projetos = from p in lancamentos
                                                                                  where p.IdEmpresa == grp.Key.IdEmpresa
                                                                                  && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                  && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                  && p.IdClassificacaoEsg == grp.Key.IdClassificacaoEsg
                                                                                  group p by new { p.IdEmpresa, p.IdGrupoPrograma, p.IdPrograma, p.IdClassificacaoEsg, p.IdProjeto, p.NomeProjeto, p.SeqFase } into grpPrj
                                                                                  select new ProjetoEsgDTO()
                                                                                  {
                                                                                      IdProjeto = grpPrj.Key.IdProjeto,
                                                                                      NomeProjeto = grpPrj.Key.NomeProjeto,
                                                                                      LancamentoESG = new LancamentoESG()
                                                                                      {
                                                                                          IdClassificacaoEsg = grpPrj.Key.IdClassificacaoEsg,
                                                                                          ValorBaseOrcamento = CalcularValorBaseOrcamento(grpPrj, filtro.BaseOrcamento, p.PredicateBaseOrcamentoPrevisto, p.PredicateBaseOrcamentoRealizado, p.PredicateBaseOrcamentoReplan, p.PredicateBaseOrcamentoOrcado),
                                                                                          ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamento(grpPrj, filtro.FormatAcompanhamento, p.PredicateFormatoAcomp_realizado, p.PredicateFormatoAcomp_tendencia, p.PredicateFormatoAcomp_ciclo, filtro.DataFim, filtro.DataInicio)
                                                                                      },
                                                                                      Fase = from fse in lancamentosFase
                                                                                             where fse.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                && fse.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                && fse.SeqFase == grpPrj.Key.SeqFase
                                                                                             group fse by new { fse.IdEmpresa, fse.IdProjeto, fse.SeqFase, fse.NomeFase, fse.Pep } into grpFse
                                                                                             select new FaseEsgDTO
                                                                                             {
                                                                                                 IdEmpresa = grpFse.Key.IdEmpresa,
                                                                                                 SeqFase = grpFse.Key.SeqFase,
                                                                                                 Nome = grpFse.Key.NomeFase,
                                                                                                 Pep = grpFse.Key.Pep,
                                                                                                 LancamentoESG = new LancamentoESG()
                                                                                                 {
                                                                                                     ValorBaseOrcamento = CalcularValorBaseOrcamento(grpFse, filtro.BaseOrcamento, p.PredicateFasePrevisto, p.PredicateFasePrevisto_Realizado, p.PredicateFaseReplan, p.PredicateFaseOrcado),
                                                                                                     ValorFormatoAcompanhamento = CalcularValorFormaAcompanhamentoFase(grpFse, filtro.FormatAcompanhamento, p.PredicateFormatoAcompFase_realizado, p.PredicateFormatoAcompFase_tendencia, p.PredicateFormatoAcompFase_ciclo, filtro.DataInicio, filtro.DataFim)
                                                                                                 },
                                                                                             }

                                                                                  }
                                                                   }

                                                   }
                               }
                };
                return retorno;
            }
            return null;
        }

        private decimal CalcularValorBaseOrcamento(IGrouping<object, LancamentoClassificacaoEsgDTO> lancamentos
            , string tipoOrcamento
            , Func<LancamentoClassificacaoEsgDTO, bool> predicatePrevisto
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateRealizado
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateReplan
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateBaseOrcamentoOrcado)
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

        private decimal CalcularValorFormaAcompanhamento(IGrouping<object, LancamentoClassificacaoEsgDTO> lancamentos
            , string formatoAcompanhamento
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateFormatoAcomp_realizado
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateFormatoAcomp_tendencia
            , Func<LancamentoClassificacaoEsgDTO, bool> predicateFormatoAcomp_ciclo
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
        public async Task<byte[]> GerarRelatorioEsg(FiltroPainelClassificacaoEsg filtro)
        {
            Func<LancamentoClassificacaoEsgDTO, bool> predicateTendencia = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateRealizado = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateCiclo = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateOrcado = _ => true;

            Func<LancamentoFaseContabilDTO, bool> predicateFaseTendencia = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseRealizado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseCiclo = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;

            if (filtro.FormatAcompanhamento == "C")
            {
                if (filtro.DataFim > DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    DateTime dataAux = DateTime.Now.AddMonths(12 - Convert.ToInt32(DateTime.Now.ToString("MM")));
                    filtro.DataTendenciaFim = new DateTime(dataAux.Year, dataAux.Month, 1, 0, 0, 0);
                    dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(dataAux.Year, dataAux.Month, 1, 0, 0, 0);
                    dataAux = DateTime.Now.AddMonths(-1);
                    filtro.DataRealizadoFim = new DateTime(dataAux.Year, dataAux.Month, 1, 0, 0, 0);
                    filtro.DataCicloInicio = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, 1, 0, 0, 0);
                    filtro.DataCicloFim = new DateTime(DateTime.Now.Year + 1, 12, 1, 0, 0, 0);
                }
                else if (filtro.DataFim < DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(1999, 1, 1, 0, 0, 0);
                    filtro.DataTendenciaFim = new DateTime(1999, 1, 1, 0, 0, 0);

                    DateTime dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(dataAux.Year, dataAux.Month, 1, 0, 0, 0);
                    dataAux = DateTime.Now.AddMonths(-1);
                    filtro.DataRealizadoFim = new DateTime(dataAux.Year, dataAux.Month, 1, 0, 0, 0);

                    filtro.DataCicloInicio = new DateTime(1999, 1, 1);
                    filtro.DataCicloFim = new DateTime(1999, 1, 1);
                }
                else if (filtro.DataInicio > DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime dataAux = DateTime.Now.AddMonths(12 - Convert.ToInt32(DateTime.Now.ToString("MM")));
                    filtro.DataTendenciaFim = new DateTime(dataAux.Year, dataAux.Month, 1);
                    dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(1999, 1, 1);
                    filtro.DataRealizadoFim = new DateTime(1999, 1, 1);
                    filtro.DataCicloInicio = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, 1);
                    filtro.DataCicloFim = new DateTime(DateTime.Now.Year + 1, 12, 1);
                }
            }
            else if (filtro.FormatAcompanhamento == "T")
            {
                if (filtro.DataFim > DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime dataAux = DateTime.Now.AddMonths(12 - Convert.ToInt32(DateTime.Now.ToString("MM")));
                    filtro.DataTendenciaFim = new DateTime(dataAux.Year, dataAux.Month, 1);
                    dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(dataAux.Year, dataAux.Month, 1);
                    dataAux = DateTime.Now.AddMonths(-1);
                    filtro.DataRealizadoFim = new DateTime(dataAux.Year, dataAux.Month, 1);
                    filtro.DataCicloInicio = new DateTime(1999, 1, 1);
                    filtro.DataCicloFim = new DateTime(1999, 1, 1);
                }
                else if (filtro.DataFim < DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(1999, 1, 1);
                    filtro.DataTendenciaFim = new DateTime(1999, 1, 1);

                    DateTime dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(dataAux.Year, dataAux.Month, 1);
                    dataAux = DateTime.Now.AddMonths(-1);
                    filtro.DataRealizadoFim = new DateTime(dataAux.Year, dataAux.Month, 1);

                    filtro.DataCicloInicio = new DateTime(1999, 1, 1);
                    filtro.DataCicloFim = new DateTime(1999, 1, 1);
                }
                else if (filtro.DataInicio > DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime dataAux = DateTime.Now.AddMonths(12 - Convert.ToInt32(DateTime.Now.ToString("MM")));
                    filtro.DataTendenciaFim = new DateTime(dataAux.Year, dataAux.Month, 1);
                    dataAux = DateTime.Now.AddMonths(Convert.ToInt32(DateTime.Now.ToString("MM")) - 1);
                    filtro.DataRealizadoInicio = new DateTime(1999, 1, 1);
                    filtro.DataRealizadoFim = new DateTime(1999, 1, 1);
                    filtro.DataCicloInicio = new DateTime(1999, 1, 1);
                    filtro.DataCicloFim = new DateTime(1999, 1, 1);
                }
            }

            IEnumerable<RelatorioEsgDTO> dados = await _painelClassificacaoRepository.ConsultarDadosRelatorio(filtro);
            var cenarios = await _cenarioService.ConsultarCenario(new CenarioFiltro() { IdCenario = filtro.IdCenario });
            string nomCenario = ((IEnumerable<CenarioDTO>)cenarios.ObjetoRetorno).FirstOrDefault().Nome;
            await PopularParametrizacoes();
            (int, string) esgClassif = RetornarClassificacaoEsg(filtro);
            foreach (var item in dados)
            {
                filtro.IdProjeto = item.IdProjeto;
                filtro.IdPrograma = item.IdPrograma;
                filtro.IdGrupoPrograma = Convert.ToInt32(item.IdGrupoPrograma);
                esgClassif = RetornarClassificacaoEsg(filtro);
                if (esgClassif.Item1 == filtro.IdCenario)
                {
                    item.IdClassificacaoEsg = esgClassif.Item1;
                    item.NomeClassificacaoEsg = esgClassif.Item2;
                }
            }
            var dadosExcel = from a in dados

                             select new RelatorioEsgExcelDTO()
                             {
                                 DiretoriaExecutora = a.DiretoriaExecutora,
                                 GerenciaExecutora = a.GerenciaExecutora,
                                 DiretoriaSolicitante = a.GerenciaSolicitante,
                                 GerenciaSolicitante = a.GerenciaSolicitante,
                                 Gestor = a.Gestor,
                                 IdProjeto = a.IdProjeto,
                                 NomeFase = a.NomeFase,
                                 NomeEmpresa = a.NomeEmpresa,
                                 ValoBaseOrcamento = a.ValoBaseOrcamento,
                                 ValorFormatoAcompanhamento = a.ValorFormatoAcompanhamento,
                                 ClassifEsg = a.NomeClassificacaoEsg,
                                 Cenario = nomCenario

                             };
            return ReportService.GerarExcel(dadosExcel);
        }

        private async Task PopularParametrizacoes()
        {
            var parametrizacoes = await _parametrizacaoCenarioService.ConsultarParametrizacaoCenario();
            if (parametrizacoes.ObjetoRetorno != null)
            {
                _parametrizacaoCenarioDTOs = parametrizacoes.ObjetoRetorno;
            }
            var parametrizacoesGeral = await _parametrizacaoEsgGeralService.ConsultarParametrizacaoClassificacaoGeral();
            if (parametrizacoesGeral.ObjetoRetorno != null)
            {
                _parametrizacaoGrupoDTOs = parametrizacoesGeral.ObjetoRetorno;
            }
            var parametrizacoesExcecao = await _parametrizacaoService.ConsultarParametrizacaoClassificacaoExcecao();
            if (parametrizacoesExcecao.ObjetoRetorno != null)
            {
                _parametrizacaoExecoes = parametrizacoesExcecao.ObjetoRetorno;
            }
        }
        private (int id, string nome) RetornarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            int idEsg = 0;
            string nome = string.Empty;
            if (_parametrizacaoCenarioDTOs.Any())
            {
                var paramCenario = _parametrizacaoCenarioDTOs.Where(p => p.IdParametrizacaoCenario == filtro.IdCenario && p.IdClassificacaoEsg == filtro.IdClassificacaoEsg);
                if (paramCenario.Any())
                {
                    idEsg = paramCenario.FirstOrDefault().IdClassificacaoEsg;
                    nome = paramCenario.FirstOrDefault().NomeClassifEsg;
                }
                var paramGeral = _parametrizacaoGrupoDTOs.Where(p => p.IdGrupoPrograma == filtro.IdGrupoPrograma);
                if (paramGeral.Any())
                {
                    idEsg = paramGeral.FirstOrDefault().IdClassificacaoEsg;
                    nome = paramGeral.FirstOrDefault().NomeClassificacaoEsg;
                }
                var paramExcecao = _parametrizacaoExecoes.Where(p => p.IdCenario == filtro.IdCenario);
                if (paramExcecao.Any())
                {
                    if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
                    {
                        paramExcecao = paramExcecao.Where(p => p.IdProjeto == filtro.IdProjeto);
                        if (paramExcecao.Any())
                        {
                            idEsg = paramExcecao.FirstOrDefault().IdParametrizacaoEsgExc;
                            nome = paramExcecao.FirstOrDefault().NomeClassificacaoEsg;
                            return (idEsg, nome);
                        }
                    }
                    paramExcecao = paramExcecao.Where(p => p.IdEmpresa == filtro.IdEmpresa
                                                    || p.IdGrupoPrograma == (filtro.IdGrupoPrograma.HasValue ? filtro.IdGrupoPrograma.Value : p.IdGrupoPrograma)
                                                    || p.IdPrograma == (filtro.IdPrograma.HasValue ? filtro.IdPrograma : p.IdPrograma));

                    if (paramExcecao.Any())
                    {
                        idEsg = paramExcecao.FirstOrDefault().IdParametrizacaoEsgExc;
                        nome = paramExcecao.FirstOrDefault().NomeClassificacaoEsg;
                    }
                }
            }
            return (idEsg, nome);
        }
    }
}
