namespace Service.DTO.PainelClassificacao
{
    public class LancamentoContabilTotalDTO
    {
        public int IdGrupoPrograma { get; set; }
        public decimal TotalBaseOrcamento { get; set; }
        public decimal TotalFormatoAcompanhamento { get; set; }
        public decimal Variacao
        {
            get
            {
                return TotalBaseOrcamento - TotalFormatoAcompanhamento;
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
                return TotalBaseOrcamento == 0 ? 0 : Math.Round(Variacao / TotalBaseOrcamento * 100, 2);
            }
            set
            {
                PercentualVariacao = value;
            }
        }

        public int IdPrograma { get; set; }
        public int IdProjeto { get; set; }
        public int IdSeqFase { get; set; }        
        
    }
}
