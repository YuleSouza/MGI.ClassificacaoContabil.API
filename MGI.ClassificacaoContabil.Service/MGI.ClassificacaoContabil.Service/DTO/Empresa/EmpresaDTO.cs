using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Empresa
{
    public class EmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public string? Nome { get; set; }
        public LancamentoContabilDTO Lancamento { get; set; }
        public IEnumerable<GrupoProgramaDTO> GrupoPrograma { get; set; }
    }
}
