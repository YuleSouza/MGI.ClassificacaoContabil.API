using DTO.Payload;
using Infra.Service;
using Infra.Service.Interfaces;
using Service.Base;
using Service.DTO.Combos;
using Service.DTO.Esg;
using Service.DTO.Esg.Email;
using Service.DTO.Filtros;
using Service.Enum;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Interface.Usuario;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class PainelEsgService : ServiceBase, IPainelEsgService
    {
        private readonly IPainelEsgRepository _repository;
        private readonly IEsgAnexoRepository _anexoRepository;
        private readonly IEsgAprovadorRepository _aprovadorRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;
        private List<string> _aprovacoes = new List<string> { EStatusAprovacao.Pendente, EStatusAprovacao.Aprovado, EStatusAprovacao.Reprovado };
        public PainelEsgService(IPainelEsgRepository painelEsgRepository
            , ITransactionHelper transactionHelper
            , IEsgAnexoRepository esgAnexoRepository
            , IEsgAprovadorRepository esgAprovadorRepository
            , IUsuarioService usuarioService
            , IEmailService emailService
            ) : base(transactionHelper)
        {
            _repository = painelEsgRepository;
            _anexoRepository = esgAnexoRepository;
            _aprovadorRepository = esgAprovadorRepository;
            _usuarioService = usuarioService;
            _emailService = emailService;
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
            return await _repository.ConsultarProjetosEsg(filtro);
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
                Justificativa = justificativa.Justificativa
            };
            PayloadDTO classificacaoValida = await ValidarClassificacaoEsg(validacao);
            if (!classificacaoValida.Sucesso) return classificacaoValida;
            PayloadDTO percentualValido = await ValidarPercentualKpi(validacao);
            if (!percentualValido.Sucesso) return percentualValido;
            int id_classif_esg = 0;
            var retorno = await ExecutarTransacao(
                async () =>
                {
                    justificativa.StatusAprovacao = EStatusAprovacao.Pendente;
                    justificativa.DataClassif = new DateTime(justificativa.DataClassif.Year, justificativa.DataClassif.Month, 1);
                    id_classif_esg = await _repository.InserirJustificativaEsg(justificativa);
                    int anexos = await _anexoRepository.InserirAnexoJustificativaEsg(justificativa.Anexos);
                    await _repository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = id_classif_esg,
                        Aprovacao = EStatusAprovacao.Pendente,
                        UsCriacao = justificativa.UsCriacao
                    });                    
                    return true;
                }, "Classificacao Inserido com sucesso"
            );
            
            var dadosEmail = await _repository.ConsultarDadosEmail(id_classif_esg);
            dadosEmail.UsuarioCripto = justificativa.UsuarioCripto;
            await EnviarEmail(dadosEmail);
            return retorno;
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
                , "Classificação alterada com sucesso!"
            );
        }
        public async Task<PayloadDTO> InserirAprovacao(int idClassifEsg, string statusAprovacao, string usuarioAprovacao)
        {
            var validacao = await ValidarAprovacao(idClassifEsg, statusAprovacao, usuarioAprovacao);
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
        private async Task<PayloadDTO> ValidarAprovacao(int idClassifEsg, string statusAprovacao, string usuario)
        {
            if (! await _usuarioService.EhUmUsuarioSustentabilidade(usuario))
            {
                return new PayloadDTO("Usuário não tem permissão para aprovar!", false);
            }
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

        public async Task<PayloadDTO> EnviarEmail(EmailAprovacaoDTO email)
        {
            try
            {
                var usuarios = await _aprovadorRepository.ConsultarUsuariosSustentabilidade();
                string emails = string.Join(';', usuarios.Select(p => p.Email).ToArray());
                await _emailService.EnviarEmailAsync(new Infra.DTO.EmailAprovacaoDTO()
                {
                    EmailDestinatario = "andre.silva@partner.elo.inf.br;andretdswork@gmail.com",
                    IdProjeto = email.IdProjeto,
                    NomeGestor = email.NomeGestor,
                    NomePatrocinador = email.NomePatrocinador,
                    NomeProjeto = email.NomeProjeto,
                    Usuario = email.Usuario,
                    PercentualKPI = email.PercentualKPI,
                    NomeClassificacao = email.NomeClassificacao,
                    NomeSubClassificacao = email.NomeSubClassificacao,
                    UsuarioCripto = email.UsuarioCripto
                    
                });
                return new PayloadDTO("Email Enviado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new PayloadDTO(string.Empty, false, ex.Message);
            }
        }

    }
}
