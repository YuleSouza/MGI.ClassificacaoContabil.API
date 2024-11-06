namespace Service.DTO.PainelClassificacao
{
    public class TotalizadorContabil
    {
        public int IdEmpresa { get; set; }
        public LancamentoContabilTotalDTO TotalEmpresa { get; set; }
        public IEnumerable<LancamentoContabilTotalDTO> TotalGrupoPrograma { get; set; }
        public IEnumerable<LancamentoContabilTotalDTO> TotalPrograma { get; set; }
        public IEnumerable<LancamentoContabilTotalDTO> TotalProjeto { get; set; }
        public IEnumerable<LancamentoContabilTotalDTO> TotalFase { get; set; }

    }
}
