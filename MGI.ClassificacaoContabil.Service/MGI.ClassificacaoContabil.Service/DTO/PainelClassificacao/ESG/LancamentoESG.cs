namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class LancamentoESG
    {
        public int IdClassificacaoEsg { get; set; }
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

        public bool Indicador
        {
            get
            {
                return OrcadoAcumulado > RealizadoAcumulado;
            }
            set
            {
                Indicador = value;
            }
        }
    }
}
