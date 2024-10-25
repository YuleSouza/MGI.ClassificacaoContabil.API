using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
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
