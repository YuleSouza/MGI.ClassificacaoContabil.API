using System.Drawing;

namespace Service.DTO
{
    public class UsuarioDTO
    {
        public DateTime? DataCriacao { get; set; }
        public string? UsuarioCriacao { get; set; }
        public DateTime? DataModificacao { get; set; }
        public string? UsuarioModificacao { get; set; }

        public string? Login {  get; set; }
        public string? Grupo {  get; set; }
        public string? Email { get; set; }

        public UsuarioDTO()
        {
            Login = string.Empty; 
            Grupo = string.Empty; 
            Email = string.Empty;
        }
    }
}
