namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoCenarioDTO
    {
        public int IdParametrizacaoCenario { get; set; }
        public int IdClassificacaoContabil { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public int IdCenario { get; set; }
        public string? Status { get; set; }
        public string? NomeCenario {  get; set; }
        public string? NomeClassifEsg { get; set; }
        public string? NomeClassifContabil { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
