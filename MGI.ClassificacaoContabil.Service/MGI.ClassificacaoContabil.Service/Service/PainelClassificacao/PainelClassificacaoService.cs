using CsvHelper;
using CsvHelper.Configuration;
using DTO.Payload;
using Infra.Interface;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Service.DTO.Classificacao;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Projeto;
using Service.Interface.Classificacao;
using Service.Interface.PainelClassificacao;
using Service.Interface.Parametrizacao;
using Service.Repository.PainelClassificacao;
using System.Globalization;
using System.Text;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;
        private Dictionary<int, string> tiposLancamento = new Dictionary<int, string>();
        private IClassificacaoService _classificacaoEsgService;
        private readonly IParametrizacaoService _parametrizacaoService;
        private IEnumerable<ParametrizacaoCenarioDTO> _parametrizacaoCenarioDTOs;
        private IEnumerable<ParametrizacaoClassificacaoGeralDTO> _parametrizacaoGrupoDTOs;
        public IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO> _parametrizacaoExecoes;
        public const int VALOR_ACUMULADO = 0;
        public const int VALOR_ANUAL = 1;

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(
            IPainelClassificacaoRepository PainelClassificacaoRepository, 
            IUnitOfWork unitOfWork,
            IClassificacaoService classificacaoEsgService,
            IParametrizacaoService parametrizacaoService)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
            _classificacaoEsgService = classificacaoEsgService;
            _parametrizacaoService = parametrizacaoService;
            tiposLancamento.Add(1, "Provisão de Manutenção");
            tiposLancamento.Add(2, "Intangível");
            tiposLancamento.Add(3, "Imobilizado");
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


            predicateOrcado = p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim;

            if (filtro.FormatAcompanhamento == 'C')
            {
                if (filtro.DataFim > DateTime.Now.AddMonths(-1))
                {
                    filtro.DataTendenciaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0,0,0);
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
            #endregion
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoContabil(filtro);
            var lancamentosFase = await _PainelClassificacaoRepository.ConsultarLancamentosDaFase(filtro);
            var lancamentosSap = await _PainelClassificacaoRepository.ConsultarLancamentoSap(filtro);
            var classificacoesMgp = await _classificacaoEsgService.ConsultarClassificacaoContabilMGP();

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
                                          OrcadoAcumulado = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                          RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                          ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                          ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                          ValorReplan = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
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
                                                                OrcadoAcumulado = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                ValorReplan = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                IdClassifContabil = grp.Key.IdClassifContabil,
                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil
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
                                                                                OrcadoAcumulado = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                ValorReplan = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil
                                                                         },                                                                         
                                                                         Projetos = from prj in lancamentos
                                                                                    where prj.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                       && prj.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                       && prj.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                       && prj.IdClassifContabil == grpGruPro.Key.IdClassifContabil
                                                                                    group prj by new { prj.IdEmpresa, prj.IdGrupoPrograma, prj.IdPrograma, prj.IdProjeto, prj.NomeProjeto, prj.IdClassifContabil } into grpPrj
                                                                                    select new ProjetoDTO()
                                                                                    {
                                                                                        CodProjeto = grpPrj.Key.IdProjeto,
                                                                                        NomeProjeto = grpPrj.Key.NomeProjeto,
                                                                                        IdClassifContabil = grp.Key.IdClassifContabil,
                                                                                        Lancamentos = new LancamentoContabilDTO()
                                                                                        {
                                                                                                OrcadoAcumulado = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorOrcado),
                                                                                                RealizadoAcumulado = grp.AsQueryable().Where(predicateRealizado).Sum(p => p.ValorRealizado),
                                                                                                ValorCiclo = grp.AsQueryable().Where(predicateCiclo).Sum(p => p.ValorCiclo),
                                                                                                ValorTendencia = grp.Where(predicateTendencia).Sum(p => p.ValorTendencia),
                                                                                                ValorReplan = grp.AsQueryable().Where(predicateOrcado).Sum(p => p.ValorReplan),
                                                                                                NomeTipoClassificacao = grp.Key.NomeClassifContabil
                                                                                        },
                                                                                        Fase = from fse in lancamentosFase
                                                                                                         join p in grpPrj on new { fse.IdProjeto, fse.FseSeq } equals new { p.IdProjeto, p.FseSeq } into qPrj
                                                                                                         group  fse by new { fse.IdEmpresa, fse.IdProjeto, fse.FseSeq, fse.NomeFase, fse.Pep } into grpFse
                                                                                                         select new FaseContabilDTO
                                                                                                         {
                                                                                                             IdEmpresa = grpFse.Key.IdEmpresa,
                                                                                                             FseSeq = grpFse.Key.FseSeq,
                                                                                                             NomeFase = grpFse.Key.NomeFase,
                                                                                                             Pep = grpFse.Key.Pep,
                                                                                                             IdClassifContabil = grp.Key.IdClassifContabil,
                                                                                                             Lancamentos = new LancamentoContabilDTO()
                                                                                                             {
                                                                                                                    OrcadoAcumulado = grpFse.AsQueryable().Where(predicateFaseOrcado).Sum(p => p.ValorOrcado),
                                                                                                                    RealizadoAcumulado = grpFse.AsQueryable().Where(predicateFaseRealizado).Sum(p => p.ValorRealizado),
                                                                                                                    ValorCiclo = grpFse.AsQueryable().Where(predicateFaseCiclo).Sum(p => p.ValorCiclo),
                                                                                                                    ValorTendencia = grpFse.Where(predicateFaseTendencia).Sum(p => p.ValorTendencia),
                                                                                                                    ValorReplan = grpFse.AsQueryable().Where(predicateFaseOrcado).Sum(p => p.ValorReplan),                                                                                                                    
                                                                                                                    NomeTipoClassificacao = grp.Key.NomeClassifContabil
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
                                                    TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorOrcado),
                                                    TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorReplan),
                                                    TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                },
                                                TotalGrupoPrograma = from g in lancamentos
                                                                     where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                     group g by new { g.IdEmpresa, g.IdGrupoPrograma } into grpTotGru
                                                                     select new LancamentoContabilTotalDTO()
                                                                     {
                                                                         IdGrupoPrograma = grpTotGru.Key.IdGrupoPrograma,
                                                                         TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorOrcado),
                                                                         TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorReplan),
                                                                         TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa && p.IdGrupoPrograma == grpTotGru.Key.IdGrupoPrograma).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                                     },
                                                TotalPrograma = from g in lancamentos
                                                                where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                                group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma } into grpTotPrg
                                                                select new LancamentoContabilTotalDTO()
                                                                {
                                                                    IdGrupoPrograma = grpTotPrg.Key.IdGrupoPrograma,
                                                                    IdPrograma = grpTotPrg.Key.IdPrograma,
                                                                    TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                    ).Sum(p => p.ValorOrcado),
                                                                    TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorReplan),
                                                                    TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa 
                                                                                                    && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                    && p.IdPrograma == grpTotPrg.Key.IdPrograma).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                                },
                                                TotalProjeto = from g in lancamentos
                                                               where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                               group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma, g.IdProjeto } into grpTotPrg
                                                               select new LancamentoContabilTotalDTO()
                                                               {
                                                                   IdGrupoPrograma = grpTotPrg.Key.IdGrupoPrograma,
                                                                   IdPrograma = grpTotPrg.Key.IdPrograma,
                                                                   IdProjeto = grpTotPrg.Key.IdProjeto,
                                                                   TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrg.Key.IdProjeto
                                                                                                   ).Sum(p => p.ValorOrcado),
                                                                   TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrg.Key.IdProjeto).Sum(p => p.ValorReplan),
                                                                   TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                   && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                   && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                   && p.IdProjeto == grpTotPrg.Key.IdProjeto).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                               },
                                                TotalFase = from g in lancamentos
                                                            where g.IdEmpresa == grpLan.Key.IdEmpresa
                                                            group g by new { g.IdEmpresa, g.IdGrupoPrograma, g.IdPrograma, g.IdProjeto, g.FseSeq } into grpTotPrg
                                                            orderby grpTotPrg.Key.FseSeq
                                                            select new LancamentoContabilTotalDTO()
                                                            {
                                                                IdGrupoPrograma = grpTotPrg.Key.IdGrupoPrograma,
                                                                IdPrograma = grpTotPrg.Key.IdPrograma,
                                                                IdProjeto = grpTotPrg.Key.IdProjeto,
                                                                IdSeqFase = grpTotPrg.Key.FseSeq,
                                                                TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotPrg.Key.IdProjeto
                                                                                                && p.FseSeq == grpTotPrg.Key.FseSeq
                                                                                                ).Sum(p => p.ValorOrcado),
                                                                TotalReplan = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotPrg.Key.IdProjeto
                                                                                                && p.FseSeq == grpTotPrg.Key.FseSeq).Sum(p => p.ValorReplan),
                                                                TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpLan.Key.IdEmpresa
                                                                                                && p.IdGrupoPrograma == grpTotPrg.Key.IdGrupoPrograma
                                                                                                && p.IdPrograma == grpTotPrg.Key.IdPrograma
                                                                                                && p.IdProjeto == grpTotPrg.Key.IdProjeto
                                                                                                && p.FseSeq == grpTotPrg.Key.FseSeq).Sum(p => p.ValorRealizado + p.ValorTendencia + p.ValorCiclo)
                                                            },

                                            }).ToList()
                         };

            var totalizador = retorno.SelectMany(p => p.Totalizador).ToList();
            totalizador.AddRange(totais.SelectMany(p => p.Totalizador));
            var empresa = retorno.SelectMany(p => p.Empresas).AsQueryable(); 

            return new PainelClassificacaoContabilDTO()
            {
                Empresas = empresa.ToList(),
                Totalizador = totalizador,
                Cabecalho = classificacoesMgp.ToList()
            };
        }
        public async Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            int idCenario = filtro.IdCenario;
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoEsg(filtro);
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
                                                    IdClassificacaoEsg = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa).FirstOrDefault()!.IdClassificacaoEsg,
                                                    OrcadoAcumulado = filtro.TipoAcumuladoOuAnual == VALOR_ACUMULADO ? lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdClassificacaoEsg == grp.Key.IdClassificacaoEsg).Sum(p => p.ValorOrcado)
                                                                      : lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdClassificacaoEsg == grp.Key.IdClassificacaoEsg).Sum(p => p.ValorTendencia),
                                                    RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdClassificacaoEsg == grp.Key.IdClassificacaoEsg).Sum(p => p.ValorRealizadoSap)
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
                                                                                        IdClassificacaoEsg = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa 
                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).FirstOrDefault()!.IdClassificacaoEsg,
                                                                                        OrcadoAcumulado = filtro.TipoAcumuladoOuAnual == VALOR_ACUMULADO ? lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa 
                                                                                                                                                                                && p.IdClassificacaoEsg == grpGru.Key.IdClassificacaoEsg 
                                                                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorOrcado)
                                                                                                                                                            : lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa 
                                                                                                                                                                                && p.IdClassificacaoEsg == grpGru.Key.IdClassificacaoEsg 
                                                                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorTendencia),
                                                                                        RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa 
                                                                                                                                && p.IdClassificacaoEsg == grpGru.Key.IdClassificacaoEsg
                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorRealizadoSap)
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
                                                                                                        IdClassificacaoEsg = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa 
                                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma).FirstOrDefault()!.IdClassificacaoEsg,
                                                                                                        OrcadoAcumulado = filtro.TipoAcumuladoOuAnual == VALOR_ACUMULADO ? lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma).Sum(p => p.ValorOrcado)
                                                                                                                                                            : lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma).Sum(p => p.ValorTendencia),
                                                                                                        RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma).Sum(p => p.ValorRealizadoSap)
                                                                                                    },
                                                                                    Projetos = from p in lancamentos
                                                                                               where p.IdEmpresa == grp.Key.IdEmpresa
                                                                                               && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                               && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                               && p.IdClassificacaoEsg == grp.Key.IdClassificacaoEsg
                                                                                               group p by new { p.IdEmpresa, p.IdGrupoPrograma, p.IdPrograma, p.IdClassificacaoEsg, p.IdProjeto, p.NomeProjeto } into grpProj
                                                                                               select new ProjetoEsgDTO()
                                                                                               {
                                                                                                   IdProjeto = grpProj.Key.IdProjeto,
                                                                                                   NomeProjeto = grpProj.Key.NomeProjeto,                                                                                                  
                                                                                                   LancamentoESG = new LancamentoESG()
                                                                                                   {
                                                                                                       IdClassificacaoEsg = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                                                                                && p.IdProjeto == grpProj.Key.IdProjeto).FirstOrDefault()!.IdClassificacaoEsg,
                                                                                                       OrcadoAcumulado = filtro.TipoAcumuladoOuAnual == VALOR_ACUMULADO ? lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                                                                                                                && p.IdProjeto == grpProj.Key.IdProjeto).Sum(p => p.ValorOrcado)
                                                                                                                                                            : lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                                                                                                                && p.IdProjeto == grpProj.Key.IdProjeto).Sum(p => p.ValorTendencia),
                                                                                                       RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                                                                && p.IdClassificacaoEsg == grpPro.Key.IdClassificacaoEsg
                                                                                                                                                && p.IdGrupoPrograma == grpPro.Key.IdGrupoPrograma
                                                                                                                                                && p.IdPrograma == grpPro.Key.IdPrograma
                                                                                                                                                && p.IdProjeto == grpProj.Key.IdProjeto).Sum(p => p.ValorRealizadoSap)
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
            var parametrizacoes = await _parametrizacaoService.ConsultarParametrizacaoCenario();
            if (parametrizacoes.ObjetoRetorno != null)
            {
                _parametrizacaoCenarioDTOs = parametrizacoes.ObjetoRetorno;
            }
            var parametrizacoesGeral = await _parametrizacaoService.ConsultarParametrizacaoClassificacaoGeral();
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
                var paramCenario = _parametrizacaoCenarioDTOs.Where(p => p.IdParametrizacaoCenario == filtro.IdCenario);
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
                var paramExcecao = _parametrizacaoExecoes.Where(p => p.IdEmpresa == filtro.IdEmpresa
                                                                    && p.IdCenario == filtro.IdCenario);
                if (paramExcecao.Any())
                {
                    if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
                    {
                        paramExcecao = paramExcecao.Where(p => p.IdGrupoPrograma == filtro.IdGrupoPrograma);
                    }
                    if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
                    {
                        paramExcecao = paramExcecao.Where(p => p.IdPrograma == filtro.IdPrograma);
                    }
                    if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
                    {
                        paramExcecao = paramExcecao.Where(p => p.IdProjeto == filtro.IdProjeto);
                    }
                    if (paramExcecao.Any())
                    {
                        idEsg = paramExcecao.FirstOrDefault().IdParametrizacaoEsgExc;
                        nome = paramExcecao.FirstOrDefault().NomeClassificacaoEsg;
                    }
                }
                return (idEsg,nome);
            }
            return (idEsg, nome);
        }
        public Task<byte[]> GerarRelatorioContabilg(FiltroPainelClassificacaoContabil filtro)
        {
            throw new NotImplementedException();
        }
        public async Task<byte[]> GerarRelatorioContabilEsg(FiltroPainelClassificacaoEsg filtro)
        {
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoEsg(filtro);
            return GerarExcel(lancamentos);
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
