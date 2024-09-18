using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Empresa
{
    public class GrupoProgramaDTO
    {
        public int IdGrupoPrograma { get; set; }
        public string? Nome { get; set; }
        public int IdClassifContabil { get; set; }
        public LancamentoContabilDTO? Lancamentos { get; set; }
        public IEnumerable<ProgramaDTO>? Programas { get; set; }
    }
}
