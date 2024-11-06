namespace Service.DTO.PainelClassificacao
{
    public class LancamentoContabilDTO
    {
        public int IdTipoClassificacao {  get; set; }
        public int IdClassifContabil { get; set; }
        public int IdClassificacaoEsg { get; set; }
	    public decimal OrcadoAcumulado { get; set; }
	    public decimal RealizadoAcumulado { get; set; }
        public decimal ValorCiclo { get; set; }
        public decimal ValorTendencia { get; set; }
        public decimal ValorReplan {  get; set; }
        public decimal ValorTotalRealizado
        {
            get 
            { 
                return RealizadoAcumulado + ValorCiclo + ValorTendencia;
            }
        }
	    public decimal Variacao 
        { 
            get
            {
                return OrcadoAcumulado - ValorTotalRealizado;
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
                return OrcadoAcumulado == 0 ? 0 : Math.Round(Variacao / OrcadoAcumulado * 100, 2);
            }
            set
            {
                PercentualVariacao = value;
            }
        }

        public decimal VariacaoReplan
        {
            get
            {
                return OrcadoAcumulado - ValorTotalRealizado;
            }
            set
            {
                VariacaoReplan = value;
            }
        }
        public decimal PercentualVariacaoReplan
        {
            get
            {
                return OrcadoAcumulado == 0 ? 0 : Math.Round(VariacaoReplan / OrcadoAcumulado * 100, 2);
            }
            set
            {
                PercentualVariacaoReplan = value;
            }
        }

        public string DescricaoLancSap { get; set; }
        public object NomeTipoClassificacao { get; internal set; }
    }
}
