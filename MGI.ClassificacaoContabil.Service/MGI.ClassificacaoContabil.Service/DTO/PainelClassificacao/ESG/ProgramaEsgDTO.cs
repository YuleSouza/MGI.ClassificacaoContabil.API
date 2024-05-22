using Service.DTO.Projeto;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class ProgramaEsgDTO
    {
        public int IdPrograma { get; set; }
        public string? Nome { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public IEnumerable<ProjetoEsgDTO>? Projetos { get; set; }
        public LancamentoTotalESG Total {  get; set; }
    }
}
