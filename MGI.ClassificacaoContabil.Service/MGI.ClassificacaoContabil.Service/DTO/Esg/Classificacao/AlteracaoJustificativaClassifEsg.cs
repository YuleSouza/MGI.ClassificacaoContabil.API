namespace Service.DTO.Esg
{
    public class AlteracaoJustificativaClassifEsg
    {
        public int IdJustifClassifEsg { get; set; }
        public string Justificativa { get; set; }
        public string UsAlteracao { get; set; }
        public char? StatusAprovacao { get; set; }
        public int PercentualKpi { get; set; }
    }
}
