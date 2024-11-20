namespace Service.DTO.PainelClassificacao
{
    public class LancamentoContabilDTO
    {
        public int IdTipoClassificacao {  get; set; }
        public int IdClassifContabil { get; set; }
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

        public string DescricaoLancSap { get; set; }
        public object NomeTipoClassificacao { get; internal set; }
        
    }
}
