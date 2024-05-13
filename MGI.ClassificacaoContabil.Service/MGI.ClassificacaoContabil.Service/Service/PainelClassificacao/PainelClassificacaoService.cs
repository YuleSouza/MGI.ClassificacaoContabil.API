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

        private IUnitOfWork _unitOfWork;
        public PainelClassificacaoService(IPainelClassificacaoRepository PainelClassificacaoRepository, IUnitOfWork unitOfWork)
        {
            _PainelClassificacaoRepository = PainelClassificacaoRepository;
            _unitOfWork = unitOfWork;
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

        public async Task<IEnumerable<ClassificacaoContabilDTO>> ConsultarClassificacaoContabil()
        {
            var lancamentos = await _PainelClassificacaoRepository.ConsultarClassificacaoContabil();
            ClassificacaoContabilDTO classificacaoContabil = new ClassificacaoContabilDTO();
            classificacaoContabil.Empresas = new List<EmpresaDTO>();
            var retorno = from a in lancamentos
                          group a by new { a.IdEmpresa, a.NomeEmpresa, a.IdTipoClassificacao } into grp
                          select new ClassificacaoContabilDTO()
                          {
                              Empresas = new List<EmpresaDTO>()
                              {
                                  new EmpresaDTO()
                                  {
                                      IdEmpresa = grp.Key.IdEmpresa,
                                      Nome = grp.Key.NomeEmpresa,                                      
                                      Lancamento = new LancamentoContabilDTO()
                                      {
                                          OrcadoAcumulado = grp.Sum(p => p.ValorProjeto),
                                          RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grp.Key.IdEmpresa).Sum(p => p.ValorRealizadoSap),
                                          IdTipoClassificacao = grp.Key.IdTipoClassificacao                                          
                                      },
                                      GrupoPrograma = from gru in lancamentos
                                                      where gru.IdEmpresa == grp.Key.IdEmpresa
                                                      group gru by new { gru.IdEmpresa, gru.IdGrupoPrograma, gru.GrupoDePrograma, gru.IdTipoClassificacao } into grpGru
                                                      select new GrupoProgramaDTO()
                                                      {
                                                          CodGrupoPrograma = grpGru.Key.IdGrupoPrograma,
                                                          Nome = grpGru.Key.GrupoDePrograma,
                                                          Lancamento = new LancamentoContabilDTO()
                                                          {
                                                              IdTipoClassificacao = grpGru.Key.IdTipoClassificacao,
                                                              OrcadoAcumulado = grpGru.Sum(p => p.ValorProjeto),
                                                              RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma && p.IdTipoClassificacao == grpGru.Key.IdTipoClassificacao).Sum(p => p.ValorRealizadoSap)
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
                                                                         Lancamentos = new LancamentoContabilDTO()
                                                                         {
                                                                             IdTipoClassificacao = grpGruPro.Key.IdTipoClassificacao,
                                                                             OrcadoAcumulado = grpGruPro.Sum(p => p.ValorProjeto),
                                                                             RealizadoAcumulado = lancamentos.Where(p => p.IdEmpresa == grpGru.Key.IdEmpresa 
                                                                                                                    && p.IdGrupoPrograma == grpGru.Key.IdGrupoPrograma 
                                                                                                                    && p.IdPrograma == grpGruPro.Key.IdPrograma
                                                                                                                    && p.IdTipoClassificacao == grpGruPro.Key.IdTipoClassificacao).Sum(p => p.ValorRealizadoSap)
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
                                                                                        LancamentosSAP = from lancSap in lancamentos
                                                                                                         where lancSap.IdProjeto == grpPrj.Key.IdProjeto
                                                                                                           && lancSap.IdEmpresa == grpPrj.Key.IdEmpresa
                                                                                                           && lancSap.IdPrograma == grpPrj.Key.IdPrograma
                                                                                                           && lancSap.IdGrupoPrograma == grpPrj.Key.IdGrupoPrograma
                                                                                                           && lancSap.IdTipoClassificacao == grpPrj.Key.IdTipoClassificacao
                                                                                                        select new LancamentoSAP()
                                                                                                        {
                                                                                                            IdTipoClassificacao = grpPrj.Key.IdTipoClassificacao,
                                                                                                            DescricaoLancamento = lancSap.DescricaoLancSap,
                                                                                                            OrcadoAcumulado = lancSap.ValorProjeto,
                                                                                                            RealizadoAcumulado = lancSap.ValorRealizadoSap                                                                                                            
                                                                                                        }
                                                                                    }
                                                                     }
                                                      }
                                  }
                              }
                          };
            return retorno;
        }


        #endregion

    }
}
