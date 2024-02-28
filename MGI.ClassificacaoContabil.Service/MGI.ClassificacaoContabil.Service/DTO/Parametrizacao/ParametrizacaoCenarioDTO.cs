namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoCenarioDTO
    {
        public int IdParametrizacaoCenario { get; set; }
        public int IdClassificacaoContabil { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public int IdCenarioClassificacaoContabil { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
