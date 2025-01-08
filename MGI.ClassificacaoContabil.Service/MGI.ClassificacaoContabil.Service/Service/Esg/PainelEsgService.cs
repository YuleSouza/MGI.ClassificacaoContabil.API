using DTO.Payload;
using Service.Base;
using Service.DTO.Combos;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.Enum;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;
using System.Collections;
using System.Linq.Expressions;

namespace Service.Esg
{
    public class PainelEsgService : ServiceBase, IPainelEsgService
    {
        private readonly IPainelEsgRepository _repository;
        private readonly IEsgAnexoRepository _anexoRepository;        
        private List<string> _aprovacoes = new List<string> { EStatusAprovacao.Pendente, EStatusAprovacao.Aprovado, EStatusAprovacao.Reprovado };
        public PainelEsgService(IPainelEsgRepository painelEsgRepository
            , ITransactionHelper transactionHelper
            , IEsgAnexoRepository esgAnexoRepository
            ) : base(transactionHelper)
        {
            _repository = painelEsgRepository;
            _anexoRepository = esgAnexoRepository;
        }

        #region [ Classificação Esg]
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg()
        {
            return await _repository.ConsultarClassificacaoEsg();
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarSubClassificacaoEsg(int idClassificacao)
        {            
            return await _repository.ConsultarSubClassificacaoEsg(idClassificacao);
        }

        #endregion
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarComboProjetosEsg(FiltroProjeto filtro)
        {
            return await _repository.ConsultarComboProjetosEsg(filtro);
        }        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarCalssifInvestimento()
        {
            return await _repository.ConsultarCalssifInvestimento();
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarStatusProjeto()
        {
            return await _repository.ConsultarStatusProjeto();
        }
        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {
            var projetosEsg = await _repository.ConsultarProjetosEsg(filtro);
            if (filtro.TipoValor == ETipoOrcamento.Orcado || filtro.TipoValor == ETipoOrcamento.Previsto || filtro.TipoValor == ETipoOrcamento.Replan)
            {
                IEnumerable<ProjetoEsgDTO> realizados = Enumerable.Empty<ProjetoEsgDTO>();
                IEnumerable<ProjetoEsgDTO> orcado = Enumerable.Empty<ProjetoEsgDTO>();
                IEnumerable<ProjetoEsgDTO> previsto = Enumerable.Empty<ProjetoEsgDTO>();
                IEnumerable<ProjetoEsgDTO> replan = Enumerable.Empty<ProjetoEsgDTO>();
                if (filtro.DataInicio < DateTime.Now && filtro.DataFim > DateTime.Now)
                {
                    DateTime anoAnterior = new DateTime(DateTime.Now.Year - 1, 12, 1);
                    realizados = projetosEsg.Where(p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= anoAnterior && p.TipoValor == ETipoOrcamento.Realizado).ToList();
                    orcado = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Orcado).ToList();
                    previsto = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Previsto).ToList();
                    replan = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Replan).ToList();
                    
                }
                else if ((filtro.DataInicio < DateTime.Now && filtro.DataInicio.Year == DateTime.Now.Year) && filtro.DataFim > DateTime.Now)
                {
                    DateTime inicioAno = new DateTime(DateTime.Now.Year, 1, 1);
                    DateTime proximoMes = new DateTime(DateTime.Now.Year, filtro.DataInicio.Month + 1, 1);
                    DateTime mesAnterior = DateTime.Now;
                    if (filtro.DataInicio.Month > 1)
                    {
                        mesAnterior = new DateTime(DateTime.Now.Year, filtro.DataInicio.Month - 1, 1);
                    }
                    realizados = projetosEsg.Where(p => p.DtLancamentoProjeto >= inicioAno && p.DtLancamentoProjeto <= mesAnterior && p.TipoValor == ETipoOrcamento.Realizado).ToList();
                    orcado = projetosEsg.Where(p => p.DtLancamentoProjeto > proximoMes && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Orcado).ToList();
                    previsto = projetosEsg.Where(p => p.DtLancamentoProjeto > proximoMes && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Previsto).ToList();
                    replan = projetosEsg.Where(p => p.DtLancamentoProjeto > proximoMes && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Replan).ToList();
                }
                else if (filtro.DataInicio > DateTime.Now && (filtro.DataFim > DateTime.Now && filtro.DataFim.Year == DateTime.Now.Year))
                {
                    realizados = projetosEsg.Where(p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Realizado).ToList();
                    orcado = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Orcado).ToList();
                    previsto = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Previsto).ToList();
                    replan = projetosEsg.Where(p => p.DtLancamentoProjeto > filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Replan).ToList();
                }
                else if (filtro.DataInicio < DateTime.Now && filtro.DataFim < DateTime.Now)
                {
                    realizados = projetosEsg.Where(p => p.DtLancamentoProjeto >= filtro.DataInicio && p.DtLancamentoProjeto <= filtro.DataFim && p.TipoValor == ETipoOrcamento.Realizado).ToList();
                }
                var result = from a in projetosEsg
                             select new ProjetoEsgDTO()
                             {
                                 ValorProjeto = SomarValores(realizados, orcado, previsto,replan,filtro.TipoValor),
                                 IdEmpresa = a.IdEmpresa,
                                 IdProjeto = a.IdProjeto,
                                 DtLancamentoProjeto = a.DtLancamentoProjeto,
                                 IdStatusProjeto = a.IdStatusProjeto,
                                 NomeEmpresa = a.NomeEmpresa,
                                 NomePatrocinador = a.NomePatrocinador,
                                 DescricaoStatusProjeto = a.DescricaoStatusProjeto,
                                 IdGestor = a.IdGestor,
                                 TipoValor = a.TipoValor,
                             };
                return result;
            }
            return Enumerable.Empty<ProjetoEsgDTO>();
        }

        private decimal SomarValores(IEnumerable<ProjetoEsgDTO> realizados, IEnumerable<ProjetoEsgDTO> orcado, IEnumerable<ProjetoEsgDTO> previsto, IEnumerable<ProjetoEsgDTO> replan, string tipoValor)
        {
            decimal soma = 0;
            switch (tipoValor)
            {
                case ETipoOrcamento.Orcado:
                    {
                        soma = realizados.Sum(p => p.ValorOrcamento) + orcado.Sum(p => p.ValorOrcamento);
                        break;
                    }
                case ETipoOrcamento.Previsto:
                    {
                        soma = realizados.Sum(p => p.ValorOrcamento) + previsto.Sum(p => p.ValorOrcamento);
                        break;
                    }
                case ETipoOrcamento.Replan:
                    {
                        soma = realizados.Sum(p => p.ValorOrcamento) + replan.Sum(p => p.ValorOrcamento);
                        break;
                    }
                default:
                    break;
            }

            return soma;
        }
        
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            var retorno = await _repository.ConsultarJustificativaEsg(filtro);
            foreach (var item in retorno)
            {
                item.ValorKpi = (item.PercentualKpi / 100) * filtro.ValorProjeto;
            }
            foreach (var item in retorno)
            {
                var aprovacoes = await _repository.ConsultarLogAprovacoesPorId(item.IdJustifClassifEsg);
                item.Logs = new List<AprovacaoClassifEsg>();
                item.Logs.AddRange(aprovacoes);
            }
            return retorno;
        }

        public async Task<IEnumerable<AprovacaoClassifEsg>> ConsultarLogAprovacoesPorId(int id)
        {
            return await _repository.ConsultarLogAprovacoesPorId(id);
        }

        #region [Crud]
        public async Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa)
        {
            var validacao = new ValidacaoJustificativaClassif()
            {
                IdEmpresa = justificativa.IdEmpresa,                
                IdProjeto = justificativa.IdProjeto,
                Percentual = justificativa.PercentualKpi,
                IdSubClassif = justificativa.IdSubClassif,
                IdClassif = justificativa.IdClassif,
            };
            PayloadDTO classificacaoValida = await ValidarClassificacaoEsg(validacao);
            if (!classificacaoValida.Sucesso) return classificacaoValida;
            PayloadDTO percentualValido = await ValidarPercentualKpi(validacao);
            if (!percentualValido.Sucesso) return percentualValido;
            return await ExecutarTransacao(
                async () =>
                {
                    justificativa.StatusAprovacao = EStatusAprovacao.Pendente;
                    justificativa.DataClassif = new DateTime(justificativa.DataClassif.Year, justificativa.DataClassif.Month, 1);
                    int id_classif_esg = await _repository.InserirJustificativaEsg(justificativa);
                    int anexos = await _anexoRepository.InserirAnexoJustificativaEsg(justificativa.Anexos);
                    await _repository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = id_classif_esg,
                        Aprovacao = EStatusAprovacao.Pendente,
                        UsCriacao = justificativa.UsCriacao
                    });
                    // to-do enviar aprovacao pro email
                    return true;
                }, "Classificacao Inserido com sucesso"
            );
        }        
        public async Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            var justif = await _repository.ConsultarJustificativaEsgPorId(justificativa.IdJustifClassifEsg);
            var validacao = new ValidacaoJustificativaClassif()
            {
                IdEmpresa = justif.IdEmpresa,
                IdProjeto = justif.IdProjeto,
                Percentual = justif.PercentualKpi,
                IdSubClassif = justif.IdSubClassif,
                IdClassif = justif.IdClassif,
            };
            PayloadDTO percentualValido = await ValidarPercentualKpi(validacao);
            if (!percentualValido.Sucesso) return percentualValido;
            return await ExecutarTransacao(
                async () => await _repository.AlterarJustificativaEsg(justificativa)
                , "Classificacao alterada com sucesso!"
            );
        }
        public async Task<PayloadDTO> InserirAprovacao(int idClassifEsg, string statusAprovacao, string usuarioAprovacao)
        {
            var validacao = await ValidarAprovacao(idClassifEsg, statusAprovacao);
            if (!validacao.Sucesso) return validacao;
            await ExecutarTransacao(
                async () =>
                {
                    await _repository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = idClassifEsg,
                        Aprovacao = statusAprovacao,
                        UsCriacao = usuarioAprovacao
                    });
                    await _repository.AlterarStatusJustificativaEsg(new AlteracaoJustificativaClassifEsg()
                    {
                        IdJustifClassifEsg = idClassifEsg,
                        StatusAprovacao = statusAprovacao
                    });
                    return true;
                }, "");
            // enviar e-mail para o gestores
            return new PayloadDTO("Classificação aprovada com sucesso", true);
        }        
        public async Task<PayloadDTO> ExcluirClassificacao(int id, string usuarioExclusao)
        {
            return await ExecutarTransacao(
                async () => {
                    await _repository.RemoverClassificacao(id);
                    await _repository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        Aprovacao = EStatusAprovacao.Excluido,
                        IdJustifClassifEsg = id,
                        UsCriacao = usuarioExclusao
                    });
                    return true;    
                }
                , "Classificação excluida com sucesso!"
                );
        }

        #endregion

        #region [ Validacoes ]
        private async Task<PayloadDTO> ValidarPercentualKpi(ValidacaoJustificativaClassif validacao)
        {
            var justificativas = await _repository.ConsultarJustificativaEsg(new FiltroJustificativaClassifEsg()
            {
                IdEmpresa = validacao.IdEmpresa,
                IdProjeto = validacao.IdProjeto,
                DataClassif = validacao.DataClassif,
            });
            decimal totalPercentual = justificativas.Any() ? justificativas.Where(p => p.StatusAprovacao == EStatusAprovacao.Aprovado).Sum(p => p.PercentualKpi + validacao.Percentual) : 0m;
            return new PayloadDTO(string.Empty, totalPercentual <= 100, "Total dos percentuais de KPI passou dos 100%, favor ajustar!");
        }
        private async Task<PayloadDTO> ValidarAprovacao(int idClassifEsg, string statusAprovacao)
        {
            var classificacao = await _repository.ConsultarJustificativaEsgPorId(idClassifEsg);
            if (classificacao == null)
            {
                return new PayloadDTO("Classificação não existe!", false);
            }
            if (!_aprovacoes.Contains(statusAprovacao))
            {
                return new PayloadDTO("Tipo de aprovações permitidas A,P ou R", false);
            }
            var logsAprovacao = await _repository.ConsultarLogAprovacoesPorId(idClassifEsg);
            var pendentes = logsAprovacao.Where(p => p.Aprovacao == EStatusAprovacao.Pendente);
            var aprovados = logsAprovacao.Where(p => p.Aprovacao == EStatusAprovacao.Aprovado);
            var reprovados = logsAprovacao.Where(p => p.Aprovacao == EStatusAprovacao.Reprovado);
            if (pendentes.Any() && (aprovados.Any() || reprovados.Any()))
            {
                return new PayloadDTO("Classificação não pode ser aprovada ou reprovada.", false);
            }
            return new PayloadDTO(string.Empty, true);
        }
        private async Task<PayloadDTO> ValidarClassificacaoEsg(ValidacaoJustificativaClassif validacao)
        {
            if (validacao.Justificativa.Length < 16)
            {
                return new PayloadDTO("Texto para justificativa deve conter entre 15 e 200 caracteres!", false);
            }
            var justificativas = await _repository.ConsultarJustificativaEsg(new FiltroJustificativaClassifEsg()
            {
                IdEmpresa = validacao.IdEmpresa,
                IdProjeto = validacao.IdProjeto
            });
            var pendentes = justificativas.Count(p => p.StatusAprovacao == EStatusAprovacao.Pendente && p.IdClassif == validacao.IdClassif && p.IdSubClassif == validacao.IdSubClassif);
            var aprovados = justificativas.Count(p => p.StatusAprovacao == EStatusAprovacao.Aprovado && p.IdClassif == validacao.IdClassif && p.IdSubClassif == validacao.IdSubClassif);
            var reprovados = justificativas.Count(p => p.StatusAprovacao == EStatusAprovacao.Reprovado && p.IdClassif == validacao.IdClassif && p.IdSubClassif == validacao.IdSubClassif);
            if (pendentes > 0 || aprovados > 0)
                return new PayloadDTO("Classificação e Sub Classificação já existe para o projeto", false);
            return new PayloadDTO(string.Empty, true);
        }

        
        #endregion

    }
}
