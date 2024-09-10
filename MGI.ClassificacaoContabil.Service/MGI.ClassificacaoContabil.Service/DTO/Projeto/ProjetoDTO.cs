using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Projeto
{
    public class ProjetoDTO
    {
        public int CodProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public LancamentoContabilDTO? Lancamentos { get; set; }
        public LancamentoContabilTotalDTO? TotalLancamento { get; set; }
        public IEnumerable<FaseContabilDTO>? Fase { get; set; }
    }
}
