using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using Service.DTO.Projeto;

namespace Service.DTO.Empresa
{
    public class ProgramaDTO
    {
        public int CodPrograma { get; set; }
        public string? Nome { get; set; }
        public LancamentoContabilDTO? LancamentoIntangivel { get; set; }
        public LancamentoContabilDTO? LancamentoImobilizado { get; set; }
        public LancamentoContabilDTO? LancamentoProvisao { get; set; }
        public LancamentoContabilTotalDTO? TotalLancamento { get; set; }
        public IEnumerable<ProjetoDTO>? Projetos { get; set; }
    }
}
