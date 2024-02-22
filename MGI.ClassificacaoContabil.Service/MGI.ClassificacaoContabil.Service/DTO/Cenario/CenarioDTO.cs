namespace Service.DTO.Cenario
{
    public class CenarioDTO
    {
        public int IdCenario { get; set; }
        public string? Nome { get; set; }
        public string? Status { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
