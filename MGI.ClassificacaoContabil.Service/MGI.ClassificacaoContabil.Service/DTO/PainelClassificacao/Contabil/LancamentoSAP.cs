namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil
{
    public class LancamentoSAP
    {
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public decimal VlrReplan { get; set; }
        public decimal VlrOrcado { get; set; }
        public decimal RealizadoAcumulado { get; set; }
        public DateTime DtLancamentoSap { get; set; }
        public string Pep { get; set; }
        public string IdClassifContabil { get; set; }
        public decimal Variacao
        {
            get
            {
                return VlrOrcado - RealizadoAcumulado;
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
                return VlrOrcado == 0 ? 0 : Math.Round(Variacao / VlrOrcado * 100, 2);
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
                return VlrReplan - RealizadoAcumulado;
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
                return VlrReplan == 0 ? 0 : Math.Round(VariacaoReplan / VlrReplan * 100, 2);
            }
            set
            {
                PercentualVariacaoReplan = value;
            }
        }
        public string DescricaoLancamento { get; set; }
    }
}
