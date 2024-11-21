namespace Service.DTO.PainelClassificacao
{
    public class LancamentoESG
    {
        public int IdClassificacaoEsg { get; set; }
        public decimal OrcadoAcumulado { get; set; }
        public decimal ValorReplan { get; set; }
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

        public decimal ValorTendencia { get; set; }

        public decimal VariacaoReplan
        {
            get
            {
                return ValorReplan - RealizadoAcumulado;
            }
            set
            {
                Variacao = value;
            }
        }
        public decimal PercentualVariacaoReplan
        {
            get
            {
                return ValorReplan == 0 ? 0 : Math.Round(Variacao / ValorReplan * 100, 2);
            }
            set
            {
                PercentualVariacao = value;
            }
        }

        public bool IndicadorReplan
        {
            get
            {
                return ValorReplan > RealizadoAcumulado;
            }
            set
            {
                Indicador = value;
            }
        }

        public decimal ValorCiclo { get; internal set; }
        public decimal ValorBaseOrcamento { get; set; }
        public decimal ValorFormatoAcompanhamento { get; set; }
    }
}
