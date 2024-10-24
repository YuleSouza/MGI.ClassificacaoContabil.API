using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class FaseEsgDTO
    {
        public int IdEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public DateTime DtLancamentoProjeto { get; set; }
        public string Nome { get; set; }
        public int FseSeq { get; set; }
        public string Pep { get; set; }
        public LancamentoESG Lancamentos { get; set; }
    }
}
