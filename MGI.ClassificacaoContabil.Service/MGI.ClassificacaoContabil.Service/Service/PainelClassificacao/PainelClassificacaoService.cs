using DTO.Payload;
using Infra.Interface;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using Service.DTO.Cenario;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Projeto;
using Service.Interface.Cenario;
using Service.Interface.PainelClassificacao;
using Service.Interface.Parametrizacao;
using Service.Repository.PainelClassificacao;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;
        private Dictionary<int, string> tiposLancamento = new Dictionary<int, string>();
        private readonly ICenarioService _cenarioService;
        private readonly IParametrizacaoService _parametrizacaoService;
        private IEnumerable<ParametrizacaoCenarioDTO> _parametrizacaoCenarioDTOs;
        private IEnumerable<ParametrizacaoClassificacaoGeralDTO> _parametrizacaoGrupoDTOs;
        public IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO> _parametrizacaoExecoes;

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(
            IPainelClassificacaoRepository PainelClassificacaoRepository, 
            IUnitOfWork unitOfWork, 
            ICenarioService cenarioService,
            IParametrizacaoService parametrizacaoService)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
            _cenarioService = cenarioService;
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

        public async Task<IList<EmpresaDTO>> ConsultarClassificacaoContabil()
        {
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoContabil();
            ClassificacaoContabilDTO classificacaoContabil = new ClassificacaoContabilDTO();
            classificacaoContabil.Empresas = new List<EmpresaDTO>();
            var retorno = from a in lancamentos
                          orderby a.IdTipoClassificacao
                          group a by new { a.IdEmpresa, a.NomeEmpresa } into grp
                          select new ClassificacaoContabilDTO()
                          {
                              Empresas = new List<EmpresaDTO>()
                              {
                                  new EmpresaDTO()
                                  {
                                      IdEmpresa = grp.Key.IdEmpresa,
                                      Nome = grp.Key.NomeEmpresa,
                                      #region [ Lancamentos ]
                                      LancamentoProvisao = new LancamentoContabilDTO()
                                      {
                                          OrcadoAcumulado = grp.Where(p => p.IdTipoClassificacao == 1).Sum(p => p.ValorProjeto),
                                          RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdTipoClassificacao == 1).Sum(p => p.ValorRealizadoSap),
                                          IdTipoClassificacao = 1,
                                          NomeTipoClassificacao = "Provisão de Manutenção"
                                      },
                                      LancamentoIntangivel = new LancamentoContabilDTO() {
                                          OrcadoAcumulado = grp.Where(p => p.IdTipoClassificacao == 2).Sum(p => p.ValorProjeto),
                                          RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdTipoClassificacao == 2).Sum(p => p.ValorRealizadoSap),
                                          IdTipoClassificacao = 2,
                                          NomeTipoClassificacao = "Intangível"
                                      },
                                      LancamentoImobilizado = new LancamentoContabilDTO() {
                                          OrcadoAcumulado = grp.Where(p => p.IdTipoClassificacao == 3).Sum(p => p.ValorProjeto),
                                          RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdTipoClassificacao == 3).Sum(p => p.ValorRealizadoSap),
                                          IdTipoClassificacao = 3,
                                          NomeTipoClassificacao = "Imobilizado"
                                      },
                                      TotalLancamento = new LancamentoContabilTotalDTO()
                                      {
                                          TotalOrcado = grp.Sum(p => p.ValorProjeto),
                                          TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa).Sum(p => p.ValorRealizadoSap)
                                      },
                                      #endregion  
                                      GrupoPrograma = from gru in lancamentos
                                                      where gru.IdEmpresa == grp.Key.IdEmpresa
                                                      group gru by new { gru.IdEmpresa, gru.IdGrupoPrograma, gru.GrupoDePrograma, gru.IdTipoClassificacao } into grpGru
                                                      select new GrupoProgramaDTO()
                                                      {
                                                          CodGrupoPrograma = grpGru.Key.IdGrupoPrograma,
                                                          Nome = grpGru.Key.GrupoDePrograma,
                                                          LancamentoProvisao = new LancamentoContabilDTO()
                                                          {
                                                              IdTipoClassificacao = 1,
                                                              OrcadoAcumulado = grpGru.Where(p => p.IdTipoClassificacao == 1 && p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorProjeto),
                                                              RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma && p.IdTipoClassificacao == 1).Sum(p => p.ValorRealizadoSap)
                                                          },
                                                          LancamentoIntangivel = new LancamentoContabilDTO()
                                                          {
                                                              IdTipoClassificacao = 2,
                                                              OrcadoAcumulado = grpGru.Where(p => p.IdTipoClassificacao == 2 && p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorProjeto),
                                                              RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma && p.IdTipoClassificacao == 2).Sum(p => p.ValorRealizadoSap)
                                                          },
                                                          LancamentoImobilizado = new LancamentoContabilDTO()
                                                          {
                                                              IdTipoClassificacao = 3,
                                                              OrcadoAcumulado = grpGru.Where(p => p.IdTipoClassificacao == 3 && p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorProjeto),
                                                              RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma && p.IdTipoClassificacao == 3).Sum(p => p.ValorRealizadoSap)
                                                          },
                                                          TotalLancamento = new LancamentoContabilTotalDTO()
                                                          {
                                                              TotalOrcado = grp.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma) .Sum(p => p.ValorProjeto),
                                                              TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma).Sum(p => p.ValorRealizadoSap)
                                                          },
                                                          Programas = from gruPro in lancamentos
                                                                     where gruPro.IdEmpresa == grp.Key.IdEmpresa
                                                                        && gruPro.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                        && gruPro.IdTipoClassificacao == grpGru.Key.IdTipoClassificacao
                                                                     group gruPro by new { gruPro.IdEmpresa, gruPro.IdGrupoPrograma, gruPro.IdPrograma, gruPro.Programa, gruPro.IdTipoClassificacao } into grpGruPro
                                                                     select new ProgramaDTO()
                                                                     {
                                                                         CodPrograma = grpGruPro.Key.IdPrograma,
                                                                         Nome = grpGruPro.Key.Programa,
                                                                         LancamentoProvisao = new LancamentoContabilDTO()
                                                                         {
                                                                             IdTipoClassificacao = 1,
                                                                             OrcadoAcumulado = grpGruPro.Where(p => p.IdTipoClassificacao == 1
                                                                                                                && p.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                                                && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                && p.IdPrograma == grpGruPro.Key.IdPrograma).Sum(p => p.ValorProjeto),
                                                                             RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa
                                                                                                                    && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                    && p.IdTipoClassificacao == 1).Sum(p => p.ValorRealizadoSap)
                                                                         },
                                                                         LancamentoIntangivel = new LancamentoContabilDTO()
                                                                         {
                                                                             IdTipoClassificacao = 2,
                                                                             OrcadoAcumulado = grpGruPro.Where(p => p.IdTipoClassificacao == 2
                                                                                                                && p.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                                                && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                && p.IdPrograma == grpGruPro.Key.IdPrograma).Sum(p => p.ValorProjeto),
                                                                             RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa
                                                                                                                    && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                    && p.IdTipoClassificacao == 2).Sum(p => p.ValorRealizadoSap)
                                                                         },
                                                                         LancamentoImobilizado = new LancamentoContabilDTO()
                                                                         {
                                                                             IdTipoClassificacao = 3,
                                                                             OrcadoAcumulado = grpGruPro.Where(p => p.IdTipoClassificacao == 3
                                                                                                                && p.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                                                && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                && p.IdPrograma == grpGruPro.Key.IdPrograma).Sum(p => p.ValorProjeto),
                                                                             RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa
                                                                                                                    && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                    && p.IdTipoClassificacao == 3).Sum(p => p.ValorRealizadoSap)
                                                                         },
                                                                         TotalLancamento = new LancamentoContabilTotalDTO()
                                                                          {
                                                                              TotalOrcado = grp.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                        && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                        && p.IdPrograma == grpGruPro.Key.IdPrograma) .Sum(p => p.ValorProjeto),
                                                                              TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa
                                                                                                        && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma
                                                                                                        && p.IdPrograma == grpGruPro.Key.IdPrograma).Sum(p => p.ValorRealizadoSap)
                                                                          },
                                                                         Projetos = from prj in lancamentos
                                                                                    where prj.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                       && prj.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                       && prj.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                       && prj.IdTipoClassificacao == grpGruPro.Key.IdTipoClassificacao
                                                                                    group prj by new { prj.IdEmpresa, prj.IdGrupoPrograma, prj.IdPrograma, prj.IdProjeto, prj.NomeProjeto, prj.IdTipoClassificacao } into grpPrj
                                                                                    select new ProjetoDTO()
                                                                                    {
                                                                                        CodProjeto = grpPrj.Key.IdProjeto,
                                                                                        NomeProjeto = grpPrj.Key.NomeProjeto,
                                                                                        LancamentoSAPProvisao = new LancamentoSAP()
                                                                                        {
                                                                                                IdTipoClassificacao = 1,
                                                                                                OrcadoAcumulado = lancamentos.Where(p => p.IdTipoClassificacao == 1
                                                                                                                                    && p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto).Sum(p => p.ValorProjeto),
                                                                                                RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                    && p.IdTipoClassificacao == 1).Sum(p => p.ValorRealizadoSap)
                                                                                        },
                                                                                        LancamentoSAPIntangivel = new LancamentoSAP()
                                                                                        {
                                                                                                IdTipoClassificacao = 2,
                                                                                                OrcadoAcumulado = lancamentos.Where(p => p.IdTipoClassificacao == 2
                                                                                                                                    && p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto).Sum(p => p.ValorProjeto),
                                                                                                RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                    && p.IdTipoClassificacao == 2).Sum(p => p.ValorRealizadoSap)
                                                                                        },
                                                                                        LancamentoSAPImobilizado = new LancamentoSAP() {
                                                                                                IdTipoClassificacao = 3,
                                                                                                OrcadoAcumulado = lancamentos.Where(p => p.IdTipoClassificacao == 3
                                                                                                                                    && p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto).Sum(p => p.ValorProjeto),
                                                                                                RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                    && p.IdTipoClassificacao == 3).Sum(p => p.ValorRealizadoSap)
                                                                                        }                                                                                                                ,
                                                                                        TotalLancamentoSAP = new LancamentoContabilTotalDTO() { 
                                                                                                TotalOrcado = lancamentos.Where(p => p.IdEmpresa == grpGruPro.Key.IdEmpresa
                                                                                                                                    && p.IdGrupoPrograma == grpGruPro.Key.IdGrupoPrograma
                                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                                    && p.IdProjeto == grpPrj.Key.IdProjeto).Sum(p => p.ValorProjeto),
                                                                                                TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                && p.IdProjeto == grpPrj.Key.IdProjeto)
                                                                                                                                .Sum(p => p.ValorRealizadoSap)
                                                                                        }
                                                                                    }
                                                                     }
                                                      }
                                  }
                              }
                          };
            return retorno.SelectMany(p => p.Empresas).ToList();
        }
        public async Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg()
        {
            int idCenario = 1;
            int idGrupoPrograma = 1;
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoEsg();
            await PopularParametrizacoes();
            foreach (var item in lancamentos)
            {
                item.IdClassificacaoESG = RetornarClassificacaoEsg(item.IdTipoClassificacao, idCenario, item.IdGrupoPrograma, item.IdEmpresa);
            }
            var cenarios = await _cenarioService.ConsultarCenario();
            if (cenarios.ObjetoRetorno != null)
            {
                IEnumerable<CenarioDTO> cenarioDTO = (IEnumerable<CenarioDTO>)cenarios.ObjetoRetorno;
                CabecalhoEsg cabecalhoEsg = new CabecalhoEsg();
                cabecalhoEsg.Cenarios = new List<CenarioDTO>();
                foreach (var item in cenarioDTO)
                {
                    cabecalhoEsg.Cenarios.Add(item);
                }
                
                var retorno = new PainelClassificacaoEsg()
                              {
                                 Cabecalho = cabecalhoEsg,
                                 Empresas = from a in lancamentos
                                            group a by new { a.IdEmpresa, a.IdClassificacaoESG, a.NomeEmpresa } into grp
                                            select new EmpresaDTO()
                                            {
                                                IdEmpresa = grp.Key.IdEmpresa,
                                                Nome = grp.Key.NomeEmpresa,
                                                LancamentoESG = from b in lancamentos
                                                                where b.IdEmpresa == grp.Key.IdEmpresa
                                                                group b by new { b.IdEmpresa, b.NomeEmpresa, b.IdClassificacaoESG } into grpLanc
                                                                select new LancamentoESG()
                                                                {
                                                                    Id = grpLanc.Key.IdClassificacaoESG,
                                                                    OrcadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa && p.IdClassificacaoESG == grpLanc.Key.IdClassificacaoESG).Sum(p => p.ValorProjeto),
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
        private int RetornarClassificacaoEsg(int idTipoClassificacaoContabil, int idCenario, int idGrupoPrograma, int idEmpresa)
        {
            int idEsg = 22;
            if (_parametrizacaoCenarioDTOs.Any())
            {
                var paramCenario = _parametrizacaoCenarioDTOs.Where(p => p.IdParametrizacaoCenario == idCenario);
                if (paramCenario.Any())
                {
                    idEsg = paramCenario.FirstOrDefault().IdClassificacaoEsg;
                }
                var paramGeral = _parametrizacaoGrupoDTOs.Where(p => p.IdGrupoPrograma == idGrupoPrograma);
                if (paramGeral.Any())
                {
                    idEsg = paramGeral.FirstOrDefault().IdClassificacaoEsg;
                }
                //var paramExcecao = _parametrizacaoExecoes.Where(p => p.IdEmpresa == idEmpresa 
                //                                                    && p.IdCenarioClassificacaoContabil == idTipoClassificacaoContabil
                //                                                    && p.IdGrupoPrograma == idGrupoPrograma);
                //if (paramExcecao.Any())
                //{

                //}
                return idEsg;
            }
            return idEsg;
        }

        #endregion

    }
}
