using Service.DTO.Empresa;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class EmpresaEsgDTO
    {
        public int IdEmpresa { get; set; }
        public string? Nome { get; set; }
        //public IEnumerable<LancamentoESG> LancamentoESG { get; set; }
        public LancamentoESG LancamentoESG { get; set; }
        public LancamentoTotalESG Total { get; set; }
        public IEnumerable<GrupoProgramaEsgDTO> GrupoPrograma { get; set; }
    }
}
