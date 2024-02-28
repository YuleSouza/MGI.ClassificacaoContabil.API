namespace Service.DTO.Classificacao
{
    public class ClassificacaoContabilDTO
    {
        public int? IdClassificacaoContabil { get; set; }
        public string? Nome { get; set; }
        public int? IdEmpresa { get; set; }
        public int IdProjeto { get; set; }
        public string? Status { get; set; }
        public DateTime MesAnoInicio { get; set; }
        public DateTime MesAnoFim { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
