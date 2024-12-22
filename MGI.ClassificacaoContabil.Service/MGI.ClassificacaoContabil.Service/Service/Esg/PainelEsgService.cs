using DTO.Payload;
using MGI.ClassificacaoContabil.Service.DTO.Esg.Classificacao;
using Service.Helper;
using Microsoft.Extensions.Configuration;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class PainelEsgService : IPainelEsgService
    {
        private readonly IPainelEsgRepository _painelEsgRepository;
        private readonly IEsgAnexoRepository _esgAnexoRepository;
        private ITransactionHelper _transactionHelper;
        private List<char> _aprovacoes = new List<char> { 'P', 'A', 'R' };
        private IConfiguration _configuration;
        public PainelEsgService(IPainelEsgRepository painelEsgRepository
            , ITransactionHelper transactionHelper
            , IEsgAnexoRepository esgAnexoRepository
            , IConfiguration configuration)
        {
            _painelEsgRepository = painelEsgRepository;
            _transactionHelper = transactionHelper;
            _configuration = configuration;
            _esgAnexoRepository = esgAnexoRepository;
        }

        #region [ Classificação Esg]
        public async Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg()
        {
            return await _painelEsgRepository.ConsultarClassificacaoEsg();
        }
        public async Task<IEnumerable<SubClassificacaoEsgDTO>> ConsultarSubClassificacaoEsg(int idClassificacao)
        {            
            return await _painelEsgRepository.ConsultarSubClassificacaoEsg(idClassificacao);
        }

        #endregion
        public async Task<IEnumerable<ProjetoEsg>> ConsultarComboProjetosEsg(FiltroProjeto filtro)
        {
            return await _painelEsgRepository.ConsultarComboProjetosEsg(filtro);
        }        
        public async Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento()
        {
            return await _painelEsgRepository.ConsultarCalssifInvestimento();
        }
        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro)
        {
            return await _painelEsgRepository.ConsultarProjetosPainelEsg(filtro);
        }
        public async Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto()
        {
            return await _painelEsgRepository.ConsultarStatusProjeto();
        }
        public async Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa)
        {
            // to-do 
            /*
                validar percentual
                validar se não existe na tabela a mesma classificação e subclassificação
            */
            PayloadDTO percentualValido = await ValidarPercentualKpi(new ValidacaoJustificativaClassif()
            {
                IdEmpresa = justificativa.IdEmpresa,
                DataClassif = justificativa.DataClassif,
                IdProjeto = justificativa.IdProjeto,
                Percentual = justificativa.PercentualKpi
            });
            if (!percentualValido.Sucesso) return percentualValido;
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () =>
                {
                    justificativa.StatusAprovacao = 'P';
                    justificativa.DataClassif = new DateTime(justificativa.DataClassif.Year, justificativa.DataClassif.Month, 1);
                    int id_classif_esg = await _painelEsgRepository.InserirJustificativaEsg(justificativa);
                    int anexos = await _esgAnexoRepository.InserirAnexoJustificativaEsg(justificativa.Anexos);
                    await _painelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = id_classif_esg,
                        Aprovacao = 'P', // Pendente
                        UsCriacao = justificativa.UsCriacao
                    });
                    // to-do enviar aprovacao pro email
                    return true;
                }, "Classificacao Inserido com sucesso"
            );
        }

        private async Task<PayloadDTO> ValidarPercentualKpi(ValidacaoJustificativaClassif validacao)
        {
            var justificativas = await _painelEsgRepository.ConsultarJustificativaEsg(new FiltroJustificativaClassifEsg()
            {
                IdEmpresa = validacao.IdEmpresa,
                IdProjeto = validacao.IdProjeto,
                DataClassif = validacao.DataClassif,
            });
            decimal totalPercentual = justificativas.Any() ? justificativas.Sum(p => p.PercentualKpi + validacao.Percentual) : 0m;
            return new PayloadDTO(string.Empty, totalPercentual <= 100,"Total dos percentuais de KPI passou dos 100%, favor ajustar!");
        }
        public async Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            var justif = await _painelEsgRepository.ConsultarJustificativaEsgPorId(justificativa.IdJustifClassifEsg);
            PayloadDTO percentualValido = await ValidarPercentualKpi(new ValidacaoJustificativaClassif()
            {
                IdEmpresa = justif.IdEmpresa,
                DataClassif = justif.DataClassif,
                IdProjeto = justif.IdProjeto,
                Percentual = justificativa.PercentualKpi
            });
            if (!percentualValido.Sucesso) return percentualValido;
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _painelEsgRepository.AlterarJustificativaEsg(justificativa)
                , "Classificacao alterada com sucesso!"
            );
        }
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            var retorno = await _painelEsgRepository.ConsultarJustificativaEsg(filtro);            
            foreach (var item in retorno)
            {
                var aprovacoes = await _painelEsgRepository.ConsultarAprovacoesPorId(item.IdJustifClassifEsg);
                item.Logs = new List<AprovacaoClassifEsg>();
                item.Logs.AddRange(aprovacoes);
            }
            return retorno;
        }
        public async Task<PayloadDTO> InserirAprovacao(int idClassifEsg, char aprovacao, string usuarioAprovacao)
        {
            var validacao = await ValidarAprovacao(idClassifEsg, aprovacao);
            if (!validacao.Sucesso) return validacao;
            await _transactionHelper.ExecuteInTransactionAsync(
                async () =>
                {
                    await _painelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = idClassifEsg,
                        Aprovacao = aprovacao,
                        UsCriacao = usuarioAprovacao
                    });
                    await _painelEsgRepository.AlterarStatusJustificativaEsg(new AlteracaoJustificativaClassifEsg()
                    {
                        IdJustifClassifEsg = idClassifEsg,
                        StatusAprovacao = aprovacao
                    });
                    return true;
                }, "");
            // enviar e-mail para o gestores
            return new PayloadDTO("Classificação aprovada com sucesso", true);
        }
        private async Task<PayloadDTO> ValidarAprovacao(int idClassifEsg, char aprovacao)
        {
            var classificacao = await _painelEsgRepository.ConsultarAprovacoesPorId(idClassifEsg);
            if (!classificacao.Any())
            {
                return new PayloadDTO("Classificação não existe!", false);
            }
            if (!_aprovacoes.Contains(aprovacao))
            {
                return new PayloadDTO("Tipo de aprovações permitidas A,P ou R", false);
            }
            var logsAprovacao = await _painelEsgRepository.ConsultarAprovacoesPorId(idClassifEsg);
            var pendentes = logsAprovacao.Where(p => p.Aprovacao == 'P');
            var aprovados = logsAprovacao.Where(p => p.Aprovacao == 'A');
            var reprovados = logsAprovacao.Where(p => p.Aprovacao == 'R');
            if (pendentes.Any() && (aprovados.Any() || reprovados.Any()))
            {
                return new PayloadDTO("Classificação não pode ser aprovada ou reprovada.", false);
            }
            return new PayloadDTO(string.Empty, true);
        }
        public async Task<PayloadDTO> ExcluirClassificacao(int id, string usuarioExclusao)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => {
                    await _painelEsgRepository.ExcluirClassificacao(id);
                    await _painelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        Aprovacao = 'E',
                        IdJustifClassifEsg = id,
                        UsCriacao = usuarioExclusao
                    });
                    return true;    
                }
                , "Classificação excluida com sucesso!"
                );
        }
        
    }
}
