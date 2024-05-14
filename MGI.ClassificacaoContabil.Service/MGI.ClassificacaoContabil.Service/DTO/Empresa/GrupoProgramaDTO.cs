using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Empresa
{
    public class GrupoProgramaDTO
    {
        public int CodGrupoPrograma { get; set; }
        public string? Nome { get; set; }
        public LancamentoContabilDTO? LancamentoIntangivel { get; set; }
        public LancamentoContabilDTO? LancamentoImobilizado { get; set; }
        public LancamentoContabilDTO? LancamentoProvisao { get; set; }
        public LancamentoContabilTotalDTO? TotalLancamento { get; set; }
        public IEnumerable<ProgramaDTO>? Programas { get; set; }
    }
}
