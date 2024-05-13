namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil
{
    public class LancamentoSAP
    {
        public string DescricaoLancSap {  get; set; }
        public int IdTipoClassificacao { get; set; }
        public decimal OrcadoAcumulado { get; set; }
        public decimal RealizadoAcumulado { get; set; }
        public decimal Variacao
        {
            get
            {
                return OrcadoAcumulado - RealizadoAcumulado;
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
        public string DescricaoLancamento { get; set; }
    }
}
