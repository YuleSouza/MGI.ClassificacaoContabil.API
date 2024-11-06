using Service.DTO.PainelClassificacao;

namespace Service.DTO.Empresa
{
    public class EmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public string? Nome { get; set; }
        public int IdClassifContabil { get; set; }
        public LancamentoContabilDTO Lancamentos { get; set; }
        public IEnumerable<GrupoProgramaDTO> GrupoPrograma { get; set; }
    }
}
