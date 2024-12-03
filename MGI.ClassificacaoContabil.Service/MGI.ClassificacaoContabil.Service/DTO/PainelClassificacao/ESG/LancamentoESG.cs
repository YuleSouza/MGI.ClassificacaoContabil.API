namespace Service.DTO.PainelClassificacao
{
    public class LancamentoESG
    {
        public int IdClassificacaoEsg { get; set; }
        public string NomeClassificacaoEsg { get; set; }
        public decimal ValorBaseOrcamento { get; set; }
        public decimal ValorFormatoAcompanhamento { get; set; }
        public decimal Variacao
        {
            get
            {
                return ValorBaseOrcamento - ValorFormatoAcompanhamento;
            }
            set
            {
                Variacao = value;
            }
        }
        public decimal PercentualVariacao
        {
            get
            {
                return ValorBaseOrcamento == 0 ? 0 : Math.Round(Variacao / ValorBaseOrcamento * 100, 2);
            }
            set
            {
                PercentualVariacao = value;
            }
        }
        public bool Indicador
        {
            get
            {
                return ValorBaseOrcamento > ValorFormatoAcompanhamento;
            }
            set
            {
                Indicador = value;
            }
        }
    }
}
