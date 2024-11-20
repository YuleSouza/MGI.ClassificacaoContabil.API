﻿using CsvHelper;
using CsvHelper.Configuration;
using DTO.Payload;
using Infra.Interface;
using Service.DTO.Cenario;
using Service.DTO.Classificacao;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Projeto;
using Service.Enum;
using Service.Interface.Cenario;
using Service.Interface.Classificacao;
using Service.Interface.PainelClassificacao;
using Service.Interface.Parametrizacao;
using Service.Repository.PainelClassificacao;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;
        private Dictionary<int, string> tiposLancamento = new Dictionary<int, string>();
        private IClassificacaoEsgService _classificacaoEsgService;
        private IClassificacaoContabilService _classificacaoContabilService;
        private ICenarioService _cenarioService;
        private readonly IParametrizacaoService _parametrizacaoService;
        private IEnumerable<ParametrizacaoCenarioDTO> _parametrizacaoCenarioDTOs;
        private IEnumerable<ParametrizacaoClassificacaoGeralDTO> _parametrizacaoGrupoDTOs;
        public IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO> _parametrizacaoExecoes;
        public readonly IParametrizacaoCenarioService _parametrizacaoCenarioService;
        public readonly IParametrizacaoEsgGeralService _parametrizacaoEsgGeralService;
        public const int VALOR_ACUMULADO = 0;
        public const int VALOR_ANUAL = 1;
        private Dictionary<char, string> _tiposValores;

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(
            IPainelClassificacaoRepository PainelClassificacaoRepository, 
            IUnitOfWork unitOfWork,
            IClassificacaoEsgService classificacaoEsgService,
            IParametrizacaoService parametrizacaoService,
            ICenarioService cenarioService,
            IParametrizacaoCenarioService parametrizacaoCenarioService,
            IParametrizacaoEsgGeralService parametrizacaoEsgGeralService,
            IClassificacaoContabilService classificacaoContabilService)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
            _classificacaoEsgService = classificacaoEsgService;
            _parametrizacaoService = parametrizacaoService;
            _cenarioService = cenarioService;
            _parametrizacaoCenarioService = parametrizacaoCenarioService;
            _classificacaoContabilService = classificacaoContabilService;
            tiposLancamento.Add(1, "Provisão de Manutenção");
            tiposLancamento.Add(2, "Intangível");
            tiposLancamento.Add(3, "Imobilizado");
            _tiposValores = new Dictionary<char, string>();
            _tiposValores.Add('O', "O");
            _tiposValores.Add('R', "R");
            _tiposValores.Add('T', "J");
            _tiposValores.Add('C', "2");
            _tiposValores.Add('P', "P");
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
            Func<ClassificacaoContabilItemDTO, bool> predicateTendencia = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateRealizado = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateCiclo = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateOrcado = _ => true;

            Func<LancamentoFaseContabilDTO, bool> predicateFaseTendencia = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseRealizado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseCiclo = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto_Realizado = _ => true;

            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoRealizado = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateBaseOrcamentoPrevisto = _ => true;

            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_realizado = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_tendencia = _ => true;
            Func<ClassificacaoContabilItemDTO, bool> predicateFormatoAcomp_ciclo = _ => true;

            DateTime mesAnterior = DateTime.Now.AddMonths(-1);
            DateTime finalAno = new DateTime(DateTime.Now.Year, 12, 31);
            DateTime anoPosterior_inicio = new DateTime(DateTime.Now.Year + 1, 1, 1);
            DateTime anoPosterior_fim = new DateTime(DateTime.Now.Year + 1, 12, 31);
            DateTime mesAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            
            predicateOrcado = p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim;

            if (filtro.FormatAcompanhamento == 'C')
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto <= mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto <= finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.TipoLancamento == ETipoOrcamento.Ciclo;

                if (filtro.DataFim > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio  && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (filtro.DataFim < mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
                else if (filtro.DataInicio > mesAnterior)
                {
                    predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto < finalAno && p.TipoLancamento == ETipoOrcamento.Tendencia;
                    predicateFormatoAcomp_ciclo = p => p.DtLancamentoProjeto >= anoPosterior_inicio && p.DtLancamentoProjeto <= anoPosterior_fim && p.TipoLancamento == ETipoOrcamento.Ciclo;
                }
            }
            else if (filtro.FormatAcompanhamento == 'T')
            {
                predicateFormatoAcomp_realizado = p => p.DtLancamentoProjeto < mesAnterior && p.TipoLancamento == ETipoOrcamento.Realizado;
                predicateFormatoAcomp_tendencia = p => p.DtLancamentoProjeto >= mesAtual && p.TipoLancamento == ETipoOrcamento.Tendencia;

                if (filtro.DataFim > mesAnterior)
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
                else if (filtro.DataFim < mesAnterior)
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
                else if (filtro.DataInicio > mesAnterior)
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

            predicateTendencia = p => p.DtLancamentoProjeto >= filtro.DataTendenciaInicio && p.DtLancamentoProjeto <= filtro.DataTendenciaFim;
            predicateRealizado = p => p.DtLancamentoProjeto >= filtro.DataRealizadoInicio && p.DtLancamentoProjeto <= filtro.DataRealizadoFim;
            predicateCiclo = p => p.DtLancamentoProjeto >= filtro.DataCicloInicio && p.DtLancamentoProjeto <= filtro.DataCicloFim;

            predicateBaseOrcamentoRealizado = p => p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado;
            predicateBaseOrcamentoPrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            
            predicateFasePrevisto = p => p.DtLancamentoProjeto.Year >= DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Previsto;
            predicateFasePrevisto_Realizado = p => (p.DtLancamentoProjeto.Year < DateTime.Now.Year && p.TipoLancamento == ETipoOrcamento.Realizado);
            

            #endregion
            IEnumerable<ClassificacaoContabilItemDTO> lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoContabil(filtro);
            IEnumerable<LancamentoFaseContabilDTO> lancamentosFase = Enumerable.Empty<LancamentoFaseContabilDTO>();
            lancamentosFase = await _PainelClassificacaoRepository.ConsultarLancamentosDaFase(new FiltroLancamentoFase()
            {
                IdEmpresa = filtro.IdEmpresa,
                IdGestor = filtro.IdGestor,
                IdGrupoPrograma = filtro.IdGrupoPrograma,
                IdPrograma = filtro.IdPrograma,
                IdProjeto = filtro.IdProjeto
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
                                          RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                          ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                          ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                          ValorBaseOrcamento = CalcularValorBaseOrcamento(grp, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                          IdClassifContabil = grp.Key.IdClassifContabil,
                                          ValorFormatoAcompanhamento = 0m,
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
                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpGru, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),                                                                
                                                                IdClassifContabil = grp.Key.IdClassifContabil,
                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                ValorFormatoAcompanhamento = 0m,
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
                                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpGruPro, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                                ValorFormatoAcompanhamento = 0m,
                                                                         },                                                                         
                                                                         Projetos = from prj in lancamentos
                                                                                    where prj.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                       && prj.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                       && prj.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                       && prj.IdClassifContabil == grpGruPro.Key.IdClassifContabil
                                                                                    group prj by new { prj.IdEmpresa, prj.IdGrupoPrograma, prj.IdPrograma, prj.IdProjeto, prj.NomeProjeto, prj.IdClassifContabil, prj.SeqFase } into grpPrj
                                                                                    select new ProjetoDTO()
                                                                                    {
                                                                                        CodProjeto = grpPrj.Key.IdProjeto,
                                                                                        Nome = grpPrj.Key.NomeProjeto,
                                                                                        IdClassifContabil = grp.Key.IdClassifContabil,
                                                                                        Lancamentos = new LancamentoContabilDTO()
                                                                                        {
                                                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                                ValorBaseOrcamento = CalcularValorBaseOrcamento(grpPrj, filtro.BaseOrcamento,predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                                                ValorFormatoAcompanhamento = 0m,
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
                                                                                                                    OrcadoAcumulado = grpFse.AsQueryable().Where(predicateFaseOrcado).Sum(p => p.ValorOrcado),
                                                                                                                    RealizadoAcumulado = grpFse.AsQueryable().Where(predicateFaseRealizado).Sum(p => p.ValorRealizado),
                                                                                                                    ValorCiclo = grpFse.AsQueryable().Where(predicateFaseCiclo).Sum(p => p.ValorCiclo),
                                                                                                                    ValorTendencia = grpFse.Where(predicateFaseTendencia).Sum(p => p.ValorTendencia),
                                                                                                                    ValorReplan = grpFse.AsQueryable().Where(predicateFaseOrcado).Sum(p => p.ValorReplan),
                                                                                                                    ValorPrevisto = grpFse.AsQueryable().Where(predicateFasePrevisto).Sum(p => p.ValorPrevisto)
                                                                                                                                    + grpFse.AsQueryable().Where(predicateFasePrevisto_Realizado).Sum(p => p.ValorPrevisto),
                                                                                                                    NomeTipoClassificacao = grp.Key.NomeClassifContabil,
                                                                                                                    ValorBaseOrcamento = CalcularValorBaseOrcamento(grpFse, filtro.BaseOrcamento,predicateFasePrevisto, predicateFasePrevisto_Realizado),
                                                                                                                    ValorFormatoAcompanhamento = 0m,
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
                         group a by new { a.IdEmpresa} into grp
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
                                                    TotalBaseOrcamento = CalcularValorBaseOrcamento(grpLan, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                    TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorOrcado),
                                                    TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorReplan),
                                                    TotalTendencia = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorTendencia),
                                                    TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo),
                                                    TotalPrevisto = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorPrevisto),
                                                },
                                                TotalGrupoPrograma = from g in lancamentos
                                                                     where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                     group g by new { g.IdEmpresa, g.IdGrupoPrograma } into grpTotGru
                                                                     select new LancamentoContabilTotalDTO()
                                                                     {
                                                                         IdGrupoPrograma = grpTotGru.Key.IdGrupoPrograma,
                                                                         TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotGru, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                         TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorOrcado),
                                                                         TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorReplan),
                                                                         TotalTendencia = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorTendencia),
                                                                         TotalPrevisto = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorPrevisto),
                                                                         TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                                     },
                                                TotalPrograma = from g in lancamentos
                                                                where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma } into grpTotPrg
                                                                select new LancamentoContabilTotalDTO()
                                                                {
                                                                    IdGrupoPrograma = grpTotPrg.Key.IdGrupoPrograma,
                                                                    IdPrograma = grpTotPrg.Key.IdPrograma,
                                                                    TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotPrg, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                    TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                    ).Sum(p => p.ValorOrcado),
                                                                    TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorReplan),
                                                                    TotalPrevisto = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorPrevisto),
                                                                    TotalTendencia = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorTendencia),
                                                                    TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                                },
                                                TotalProjeto = from g in lancamentos
                                                               where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                               group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma, g.IdProjeto, g.SeqFase } into grpTotPrj
                                                               select new LancamentoContabilTotalDTO()
                                                               {
                                                                   IdGrupoPrograma = grpTotPrj.Key.IdGrupoPrograma,
                                                                   IdPrograma = grpTotPrj.Key.IdPrograma,
                                                                   IdProjeto = grpTotPrj.Key.IdProjeto,
                                                                   TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotPrj, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                   TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrj.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrj.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrj.Key.IdProjeto
                                                                                                   ).Sum(p => p.ValorOrcado),
                                                                   TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrj.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrj.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrj.Key.IdProjeto).Sum(p => p.ValorReplan),
                                                                   TotalPrevisto = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrj.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrj.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrj.Key.IdProjeto).Sum(p => p.ValorPrevisto),
                                                                   TotalTendencia = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrj.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrj.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrj.Key.IdProjeto).Sum(p => p.ValorTendencia),
                                                                   TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrj.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrj.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrj.Key.IdProjeto).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
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
                                                                TotalBaseOrcamento = CalcularValorBaseOrcamento(grpTotFse, filtro.BaseOrcamento, predicateBaseOrcamentoPrevisto, predicateBaseOrcamentoRealizado),
                                                                TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotFse.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotFse.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotFse.Key.IdProjeto
                                                                                                && p.SeqFase == grpTotFse.Key.SeqFase
                                                                                                ).Sum(p => p.ValorOrcado),
                                                                TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotFse.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotFse.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotFse.Key.IdProjeto
                                                                                                && p.SeqFase == grpTotFse.Key.SeqFase).Sum(p => p.ValorReplan),
                                                                TotalPrevisto = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotFse.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotFse.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotFse.Key.IdProjeto
                                                                                                && p.SeqFase == grpTotFse.Key.SeqFase).Sum(p => p.ValorPrevisto),
                                                                TotalTendencia = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotFse.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotFse.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotFse.Key.IdProjeto
                                                                                                && p.SeqFase == grpTotFse.Key.SeqFase).Sum(p => p.ValorTendencia),
                                                                TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotFse.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotFse.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotFse.Key.IdProjeto
                                                                                                && p.SeqFase == grpTotFse.Key.SeqFase).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
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

        private static decimal CalcularValorBaseOrcamento(IGrouping<object, ClassificacaoContabilItemDTO> lancamentos, string tipoOrcamento, Func<ClassificacaoContabilItemDTO, bool> predicatePrevisto
            , Func<ClassificacaoContabilItemDTO, bool> predicateRealizado)
        {
            switch (tipoOrcamento)
            {
                case ETipoOrcamento.Previsto:
                    return lancamentos.Where(predicatePrevisto).Sum(p => p.ValorPrevisto) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
                case ETipoOrcamento.Ciclo:
                    return lancamentos.Where(predicatePrevisto).Sum(p => p.ValorReplan) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
                default:
                    return lancamentos.Where(predicatePrevisto).Sum(p => p.ValorOrcado) +
                           lancamentos.Where(predicateRealizado).Sum(p => p.ValorRealizado);
            }
        }

        private static decimal CalcularValorBaseOrcamento(IGrouping<object, LancamentoFaseContabilDTO> lancamentos, string tipoOrcamento, Func<LancamentoFaseContabilDTO, bool> predicateFasePrevisto
            , Func<LancamentoFaseContabilDTO, bool> predicateFaseRealizado)
        {
            switch (tipoOrcamento)
            {
                case ETipoOrcamento.Previsto:
                    return lancamentos.Where(predicateFasePrevisto).Sum(p => p.ValorPrevisto) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
                case ETipoOrcamento.Ciclo:
                    return lancamentos.Where(predicateFasePrevisto).Sum(p => p.ValorReplan) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
                default:
                    return lancamentos.Where(predicateFasePrevisto).Sum(p => p.ValorOrcado) +
                           lancamentos.Where(predicateFaseRealizado).Sum(p => p.ValorRealizado);
            }
        }

        public async Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro)
        {
            return await _PainelClassificacaoRepository.ConsultarLancamentoSap(filtro);
        }
        public async Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            Func<LancamentoClassificacaoEsgDTO, bool> predicateTendencia = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateRealizado = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateCiclo = _ => true;
            Func<LancamentoClassificacaoEsgDTO, bool> predicateOrcado = _ => true;

            Func<LancamentoFaseContabilDTO, bool> predicateFaseTendencia = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseRealizado = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseCiclo = _ => true;
            Func<LancamentoFaseContabilDTO, bool> predicateFaseOrcado = _ => true;

            if (filtro.FormatAcompanhamento == 'C')
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
            else if (filtro.FormatAcompanhamento == 'T')
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
            predicateTendencia = p => p.DtLancamentoProjeto >= filtro.DataTendenciaInicio && p.DtLancamentoProjeto <= filtro.DataTendenciaFim;
            predicateRealizado = p => p.DtLancamentoProjeto >= filtro.DataRealizadoInicio && p.DtLancamentoProjeto <= filtro.DataRealizadoFim;
            predicateCiclo = p => p.DtLancamentoProjeto >= filtro.DataCicloInicio && p.DtLancamentoProjeto <= filtro.DataCicloFim;
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoEsg(filtro);
            predicateOrcado = p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim;
            var lancamentosFase = await _PainelClassificacaoRepository.ConsultarLancamentosDaFase(new FiltroLancamentoFase()
            {
                IdEmpresa = filtro.IdEmpresa,
                IdGestor = filtro.IdGestor,
                IdGrupoPrograma = filtro.IdGrupoPrograma,
                IdPrograma = filtro.IdPrograma,
                IdProjeto = filtro.IdProjeto
            });
            await PopularParametrizacoes();
            (int,string) esgClassif = RetornarClassificacaoEsg(filtro);
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
                                 Empresas = from a in lancamentos.AsQueryable().Where(predicateOrcado)
                                            group a by new { a.IdEmpresa, a.IdClassificacaoEsg, a.NomeEmpresa } into grp
                                            select new EmpresaEsgDTO()
                                            {
                                                IdEmpresa = grp.Key.IdEmpresa,
                                                Nome = grp.Key.NomeEmpresa,
                                                LancamentoESG = new LancamentoESG()
                                                {
                                                    IdClassificacaoEsg = grp.Key.IdClassificacaoEsg,
                                                    OrcadoAcumulado = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                    ValorReplan = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                    ValorTendencia = grp.AsQueryable().Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                    RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizadoSap)
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
                                                                                        OrcadoAcumulado = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                                        ValorReplan = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                                        ValorTendencia = lancamentos.AsQueryable().Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                        RealizadoAcumulado = lancamentos.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizadoSap)
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
                                                                                                        OrcadoAcumulado = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                                                        ValorReplan = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                                                        ValorTendencia = lancamentos.AsQueryable().Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                                        RealizadoAcumulado = lancamentos.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizadoSap)
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
                                                                                                       OrcadoAcumulado = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                                                       ValorReplan = lancamentos.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                                                       ValorTendencia = lancamentos.AsQueryable().Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                                       RealizadoAcumulado = lancamentos.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizadoSap)
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
                                                                                                                  OrcadoAcumulado = grpFse.Sum(p => p.ValorOrcado),
                                                                                                                  RealizadoAcumulado = grpFse.Sum(p => p.ValorRealizado),
                                                                                                                  ValorCiclo = grpFse.Sum(p => p.ValorCiclo),
                                                                                                                  ValorTendencia = grpFse.Sum(p => p.ValorTendencia),
                                                                                                                  ValorReplan = grpFse.Sum(p => p.ValorReplan)
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
                var paramCenario = _parametrizacaoCenarioDTOs.Where(p => p.IdParametrizacaoCenario == filtro.IdCenario && p.IdClassificacaoContabil == filtro.IdClassificacaoContabil);
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
                        if (paramExcecao.Any()) {
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
        public async Task<int> ConsultarClassifEsgPorCenario(FiltroPainelClassificacaoEsg filtro)
        {
            await PopularParametrizacoes();
            var retorno = await _PainelClassificacaoRepository.ConsultarClassifEsgPorProjeto(filtro.IdProjeto.Value, filtro.SeqFase.Value, filtro.IdEmpresa);
            filtro.IdPrograma = retorno.IdPrograma;
            filtro.IdGrupoPrograma = retorno.IdGrupoPrograma;
            filtro.IdCenario = filtro.IdCenario;
            (int id, string descricao) = RetornarClassificacaoEsg(filtro);
            return id;
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
            return GerarExcel(dadosExcel);
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

            if (filtro.FormatAcompanhamento == 'C')
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
            else if (filtro.FormatAcompanhamento == 'T')
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


            IEnumerable<RelatorioEsgDTO> dados = await _PainelClassificacaoRepository.ConsultarDadosRelatorio(filtro);
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
            return GerarExcel(dadosExcel);
        }
        public byte[] GerarExcel<T>(IEnumerable<T> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(false));
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
            using var csv = new CsvWriter(writer, config);

            if (data != null && data.Any())
            {
                csv.WriteRecords(data);
            }
            else
            {
                data = data.Append(Activator.CreateInstance<T>());
                csv.WriteRecords(data);
            }

            writer.Flush();

            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }       

        #endregion

    }
}
