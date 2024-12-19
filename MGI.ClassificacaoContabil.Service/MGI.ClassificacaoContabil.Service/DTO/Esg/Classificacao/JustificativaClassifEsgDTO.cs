namespace Service.DTO.Esg
{
    public class JustificativaClassifEsgDTO
    {
        public int IdEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public int IdJustifClassifEsg { get; set; }
        public int IdClassif { get; set; }
        public string DescricaoClassif { get; set; }
        public int IdSubClassif { get; set; }
        public string DescricaoSubClassif { get; set; }
        public string Justificativa { get; set; }
        public string StatusAprovacao { get; set; }
        public string DescricaoStatusAprovacao { get; set; }
        public bool ClassificacaoBloqueada { get; set; }
        public List<AprovacaoClassifEsg> Logs { get; set; }
        public string Usuario { get; set; }
        public decimal PercentualKpi { get; set; }
    }
}
