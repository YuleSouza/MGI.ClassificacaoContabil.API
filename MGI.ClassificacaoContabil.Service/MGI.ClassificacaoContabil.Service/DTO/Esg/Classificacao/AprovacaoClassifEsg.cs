using Service.Enum;

namespace Service.DTO.Esg
{
    public class AprovacaoClassifEsg
    {
        public int IdAprovacao { get; set; }
        public int IdJustifClassifEsg { get; set; }
        public string Aprovacao { get; set; }
        public string UsCriacao { get; set; }
        public DateTime DtCriacao { get; set; }
        public string NomeUsuario { get; set; }
        public string MensagemLog
        {
            get
            {
                string texto = string.Empty;
                switch (Aprovacao)
                {
                    case EStatusAprovacao.Pendente:
                        {
                            texto = $"Criado em {DtCriacao.ToString("dd/MM/yyyy HH:mm")} por {NomeUsuario}";
                            break;
                        }
                    case EStatusAprovacao.Aprovado:
                        {
                            texto = $"Aprovado em {DtCriacao.ToString("dd/MM/yyyy HH:mm")} por {NomeUsuario}";
                            break;
                        }
                    case EStatusAprovacao.Reprovado:
                        {
                            texto = $"Reprovado em {DtCriacao.ToString("dd/MM/yyyy HH:mm")} por {NomeUsuario}";
                            break;
                        }
                    case EStatusAprovacao.Excluido:
                        {
                            texto = $"Excluído em {DtCriacao.ToString("dd/MM/yyyy HH:mm")} por {NomeUsuario}";
                            break;
                        }
                    default:
                        break;
                }
                return texto;
            }            
        }
    }

}
