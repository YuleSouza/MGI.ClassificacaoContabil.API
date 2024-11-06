namespace Service.DTO.PainelClassificacao
{
    public class EmpresaEsgDTO
    {
        public int IdEmpresa { get; set; }
        public string? Nome { get; set; }
        //public IEnumerable<LancamentoESG> LancamentoESG { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public LancamentoTotalESG Total { get; set; }
        public IEnumerable<GrupoProgramaEsgDTO> GrupoPrograma { get; set; }
    }
}
