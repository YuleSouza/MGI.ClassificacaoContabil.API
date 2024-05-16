using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace Service.DTO.Projeto
{
    public class ProjetoDTO
    {
        public int CodProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public LancamentoSAP? LancamentoSAPIntangivel { get; set; }
        public LancamentoSAP? LancamentoSAPImobilizado { get; set; }
        public LancamentoSAP? LancamentoSAPProvisao { get; set; }
        public LancamentoContabilTotalDTO? TotalLancamentoSAP { get; set; }
    }
}
