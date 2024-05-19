namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class ProjetoEsgDTO
    {
        public int IdProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public IEnumerable<LancamentoESG> Lancamentos { get; set; }
        public LancamentoTotalESG Total { get; set; }
    }
}
