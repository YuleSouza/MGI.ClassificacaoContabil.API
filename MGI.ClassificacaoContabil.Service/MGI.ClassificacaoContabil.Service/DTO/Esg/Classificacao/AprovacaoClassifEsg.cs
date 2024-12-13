namespace Service.DTO.Esg
{
    public class AprovacaoClassifEsg
    {
        public int IdAprovacao { get; set; }
        public int IdJustifClassifEsg { get; set; }
        public char Aprovacao { get; set; }
        public string UsCriacao { get; set; }
        public DateTime DtCriacao { get; set; }
        public string MensagemLog
        {
            get
            {
                string texto = string.Empty;
                switch (Aprovacao)
                {
                    case 'P':
                        {
                            texto = $"Criado em {DtCriacao.ToString("dd/MM/yyyy")} por {UsCriacao}";
                            break;
                        }
                    case 'A':
                        {
                            texto = $"Aprovado em {DtCriacao.ToString("dd/MM/yyyy")} por {UsCriacao}";
                            break;
                        }
                    case 'R':
                        {
                            texto = $"Reprovado em {DtCriacao.ToString("dd/MM/yyyy")} por {UsCriacao}";
                            break;
                        }
                    case 'E':
                        {
                            texto = $"Excluído em {DtCriacao.ToString("dd/MM/yyyy")} por {UsCriacao}";
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
