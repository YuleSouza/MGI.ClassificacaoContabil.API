using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Projeto
{
    public class ProjetoDTO
    {
        public int CodProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public IEnumerable<LancamentoSAP>? LancamentoSAPIntangivel { get; set; }
        public IEnumerable<LancamentoSAP>? LancamentoSAPImobilizado { get; set; }
        public IEnumerable<LancamentoSAP>? LancamentoSAPProvisao { get; set; }
        public IEnumerable<LancamentoContabilTotalDTO>? TotalLancamentoSAP { get; set; }
    }
}
