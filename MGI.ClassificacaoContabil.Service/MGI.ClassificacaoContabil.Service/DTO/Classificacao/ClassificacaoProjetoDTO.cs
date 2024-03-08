namespace Service.DTO.Classificacao
{
    public class ClassificacaoProjetoDTO
    {
        public int? IdClassificacaoContabilProjeto { get; set; }
        public int? IdClassificacaoContabil { get; set; }
        public int? IdProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public string? Status { get; set; }
        public DateTime MesAnoInicio { get; set; }
        public DateTime MesAnoFim { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
