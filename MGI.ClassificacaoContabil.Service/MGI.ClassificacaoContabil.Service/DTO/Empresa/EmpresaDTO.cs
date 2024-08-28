using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;

namespace Service.DTO.Empresa
{
    public class EmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public string? Nome { get; set; }
        public LancamentoContabilDTO LancamentoIntangivel { get; set; }
        public LancamentoContabilDTO LancamentoImobilizado { get; set; }
        public LancamentoContabilDTO LancamentoProvisao { get; set; }
        public LancamentoContabilTotalDTO TotalLancamento { get; set; }
        public IEnumerable<GrupoProgramaDTO> GrupoPrograma { get; set; }
        public IEnumerable<LancamentoESG> LancamentoESG { get; set; }
        public LancamentoContabilDTO LancamentoSoftware { get; internal set; }
    }
}
