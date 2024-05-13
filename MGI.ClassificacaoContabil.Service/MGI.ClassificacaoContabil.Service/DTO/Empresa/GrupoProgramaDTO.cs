using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Empresa
{
    public class GrupoProgramaDTO
    {
        public int CodGrupoPrograma { get; set; }
        public string? Nome { get; set; }        
        public LancamentoContabilDTO? Lancamento { get; set; }
        public IEnumerable<ProgramaDTO>? Programas { get; set; }
    }
}
