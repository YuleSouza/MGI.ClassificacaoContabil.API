namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoClassificacaoEsgDTO
    {
        public int? IdParametrizacaoEsgExc { get; set; }
        public int IdCenario { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdGrupoPrograma { get; set; }
        public int? IdPrograma { get; set; }
        public int? IdProjeto { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
