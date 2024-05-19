using Service.DTO.Projeto;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class ProgramaEsgDTO
    {
        public int IdPrograma { get; set; }
        public string? Nome { get; set; }
        public IEnumerable<LancamentoESG> LancamentoESG { get; set; }
        public LancamentoTotalESG TotalLancamento { get; set; }
        public IEnumerable<ProjetoDTO>? Projetos { get; set; }
    }
}
