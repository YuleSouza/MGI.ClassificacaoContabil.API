using DTO.Payload;
using Infra.Interface;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Projeto;
using Service.Interface.PainelClassificacao;
using Service.Repository.PainelClassificacao;
using System.Linq;

namespace Service.PainelClassificacao
{
    public class PainelClassificacaoService : IPainelClassificacaoService
    {
        private readonly IPainelClassificacaoRepository _PainelClassificacaoRepository;
        private Dictionary<int, string> tiposLancamento = new Dictionary<int, string>();

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(IPainelClassificacaoRepository PainelClassificacaoRepository, IUnitOfWork unitOfWork)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
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
                                                                                        LancamentoSAPProvisao = (from lancSap in lancamentos
                                                                                                                 where lancSap.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                   && lancSap.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                   && lancSap.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                   && lancSap.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                   && lancSap.IdTipoClassificacao == 1
                                                                                                                select new LancamentoSAP()
                                                                                                                {
                                                                                                                    IdTipoClassificacao = 1,                                                                                                                    
                                                                                                                    OrcadoAcumulado = lancSap.ValorProjeto,
                                                                                                                    RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                                        && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                                        && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                                        && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                                        && p.IdTipoClassificacao == 1).Sum(p => p.ValorRealizadoSap)
                                                                                                                }),
                                                                                        LancamentoSAPIntangivel = (from lancSap in lancamentos
                                                                                                                 where lancSap.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                   && lancSap.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                   && lancSap.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                   && lancSap.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                   && lancSap.IdTipoClassificacao == 2
                                                                                                                select new LancamentoSAP()
                                                                                                                {
                                                                                                                    IdTipoClassificacao = 2,                                                                                                                    
                                                                                                                    OrcadoAcumulado = lancSap.ValorProjeto,
                                                                                                                    RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                                        && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                                        && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                                        && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                                        && p.IdTipoClassificacao == 2).Sum(p => p.ValorRealizadoSap)
                                                                                                                }),
                                                                                        LancamentoSAPImobilizado = (from lancSap in lancamentos
                                                                                                                 where lancSap.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                   && lancSap.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                   && lancSap.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                   && lancSap.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                   && lancSap.IdTipoClassificacao == 3
                                                                                                                select new LancamentoSAP()
                                                                                                                {
                                                                                                                    IdTipoClassificacao = 3,                                                                                                                    
                                                                                                                    OrcadoAcumulado = lancSap.ValorProjeto,
                                                                                                                    RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                                                        && p.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                                                        && p.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                                                        && p.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                                                        && p.IdTipoClassificacao == 3).Sum(p => p.ValorRealizadoSap)
                                                                                                                }),
                                                                                        TotalLancamentoSAP = (from lancSap in lancamentos
                                                                                                                group lancSap by new { lancSap.IdEmpresa, lancSap.IdGrupoPrograma, lancSap.IdPrograma, lancSap.IdProjeto } into grpTotal
                                                                                                                 where grpTotal.Key.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                                   && grpTotal.Key.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                                   && grpTotal.Key.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                                   &&  grpTotal.Key.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                                select new LancamentoContabilTotalDTO()
                                                                                                                {
                                                                                                                    TotalOrcado = grpTotal.Sum(p => p.ValorProjeto),
                                                                                                                    TotalRealizado = lancamentos.Where(p => p.IdEmpresa == grpTotal.Key.IdEmpresa
                                                                                                                                                    && p.IdGrupoPrograma == grpTotal.Key.IdGrupoPrograma
                                                                                                                                                    && p.IdPrograma == grpTotal.Key.IdPrograma
                                                                                                                                                    && p.IdProjeto == grpTotal.Key.IdProjeto)
                                                                                                                                                    .Sum(p => p.ValorRealizadoSap)
                                                                                                                }),
                                                                                    }
                                                                     }
                                                      }
                                  }
                              }
                          };
            return retorno.SelectMany(p => p.Empresas).ToList();
        }


        #endregion

    }
}
