using Service.DTO.PainelClassificacao;
using Service.DTO.Projeto;

namespace Service.DTO.Empresa
{
    public class ProgramaDTO
    {
        public int CodPrograma { get; set; }
        public string? Nome { get; set; }
        public int IdClassifContabil { get; set; }
        public LancamentoContabilDTO? Lancamentos { get; set; }
        public IEnumerable<ProjetoDTO>? Projetos { get; set; }
    }
}
