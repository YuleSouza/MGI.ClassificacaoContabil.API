namespace Service.DTO.PainelClassificacao
{
    public class ProgramaEsgDTO
    {
        public int IdPrograma { get; set; }
        public string? Nome { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public IEnumerable<ProjetoEsgDTO>? Projetos { get; set; }
        public LancamentoTotalESG Total {  get; set; }
        public string NomeClassificacaoEsg { get; set; }
    }
}
