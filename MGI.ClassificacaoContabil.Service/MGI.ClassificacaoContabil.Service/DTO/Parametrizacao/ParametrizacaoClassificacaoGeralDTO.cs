namespace Service.DTO.Parametrizacao
{
    public class ParametrizacaoClassificacaoGeralDTO
    {
        public int IdParametrizacaoEsgGeral { get; set; }
        public int IdGrupoPrograma { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
