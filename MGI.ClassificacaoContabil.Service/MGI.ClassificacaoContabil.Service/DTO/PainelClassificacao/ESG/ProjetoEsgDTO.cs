namespace Service.DTO.PainelClassificacao
{
    public class ProjetoEsgDTO
    {
        public int IdProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public LancamentoTotalESG Total { get; set; }
        public IEnumerable<FaseEsgDTO> Fase {  get; set; }
    }
}
