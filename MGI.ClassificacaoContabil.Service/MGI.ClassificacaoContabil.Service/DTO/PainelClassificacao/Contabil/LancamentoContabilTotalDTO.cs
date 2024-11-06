namespace Service.DTO.PainelClassificacao
{
    public class LancamentoContabilTotalDTO
    {
        public int IdGrupoPrograma { get; set; }
        public decimal TotalOrcado { get; set; }
        public decimal TotalRealizado { get; set; }        
        public decimal TotalReplan {  get; set; }
        public decimal Variacao
        {
            get
            {
                return TotalOrcado - TotalRealizado;
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
                return TotalOrcado == 0 ? 0 : Math.Round(Variacao / TotalOrcado * 100, 2);
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
                return TotalReplan - TotalRealizado;
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
                return TotalReplan == 0 ? 0 : Math.Round(Variacao / TotalReplan * 100, 2);
            }
            set
            {
                PercentualVariacaoReplan = value;
            }
        }

        public int IdPrograma { get; set; }
        public int IdProjeto { get; set; }
        public int IdSeqFase { get; set; }
        public decimal TotalPrevisto { get; set; }
    }
}
