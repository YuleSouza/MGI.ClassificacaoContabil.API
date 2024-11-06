using Service.DTO.PainelClassificacao;

namespace Service.DTO.Projeto
{
    public class ProjetoDTO
    {
        public int CodProjeto { get; set; }
        public string? Nome { get; set; }
        public int IdClassifContabil { get; set; }
        public LancamentoContabilDTO? Lancamentos { get; set; }
        public IEnumerable<FaseContabilDTO>? Fase { get; set; }
    }
}
