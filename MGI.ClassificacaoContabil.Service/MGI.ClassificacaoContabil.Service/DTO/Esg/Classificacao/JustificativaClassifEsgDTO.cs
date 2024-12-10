namespace Service.DTO.Esg
{
    public class JustificativaClassifEsgDTO
    {
        public int IdJustifClassifEsg { get; set; }
        public int IdCatClassif { get; set; }
        public string DescricaoCategoria { get; set; }
        public int IdSubCatClassif { get; set; }
        public string DescricaoSubCategoria { get; set; }
        public string Justificativa { get; set; }
        public string StatusAprovacao { get; set; }
        public string DescricaoStatusAprovacao { get; set; }
        public bool ClassificacaoBloqueada { get; set; }
        public List<AprovacaoClassifEsg> Logs { get; set; }
    }
}
