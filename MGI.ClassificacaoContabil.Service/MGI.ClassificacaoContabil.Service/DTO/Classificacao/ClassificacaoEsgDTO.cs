namespace Service.DTO.Classificacao
{
    public class ClassificacaoEsgDTO
    {
        public int IdClassificacaoEsg { get; set; }
        public string? Nome { get; set; }
        public string? Status { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
