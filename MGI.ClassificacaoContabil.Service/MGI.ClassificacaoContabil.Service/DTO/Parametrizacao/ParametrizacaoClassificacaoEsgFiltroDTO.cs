namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoClassificacaoEsgFiltroDTO
    {
        public int IdParametrizacaoEsgExc { get; set; }
        public int IdCenario { get; set; }
        public string? NomeCenario { get; set; }
        public int IdEmpresa { get; set; }
        public string? NomeEmpresa { get; set; }
        public int IdGrupoPrograma { get; set; }
        public string? NomeGrupoPrograma { get; set; }
        public int IdPrograma { get; set; }
        public string? NomePrograma { get; set; }
        public int IdProjeto { get; set; }
        public string? NomeProjeto { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public string? NomeClassificacaoEsg { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
