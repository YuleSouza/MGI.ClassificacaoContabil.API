namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoClassificacaoGeralDTO
    {
        public int IdParametrizacaoEsgGeral { get; set; }
        public int IdClassificacaoEsg { get; set; }
        public string? NomeClassificacaoEsg { get; set; }
        public int IdGrupoPrograma { get; set; }
        public string? NomeGrupoPrograma { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
