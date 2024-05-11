namespace Service.DTO.Classificacao
{
    public class ClassificacaoContabilDTO
    {
        public int? IdClassificacaoContabil { get; set; }
        public int? IdEmpresa { get; set; }
        public string? NomeEmpresa { get; set; }
        public string? Status { get; set; }
        public DateTime MesAnoInicio { get; set; }
        public DateTime MesAnoFim { get; set; }
        public IEnumerable<ClassificacaoProjetoDTO>? Projetos { get; set; } 
        public UsuarioDTO? Usuario { get; set; }
    }
}
