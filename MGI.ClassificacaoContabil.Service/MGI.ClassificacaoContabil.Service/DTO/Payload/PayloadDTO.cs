namespace DTO.Payload
{
    public class PayloadDTO
    {
        public string? Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public object? ObjetoRetorno { get; set; }
        public string MensagemErro { get; set; }
        public PayloadDTO(string mensagem, bool sucesso, string mensagemErro = "", object? objeto = null)
        {
            Mensagem = mensagem;
            Sucesso = sucesso;
            ObjetoRetorno = objeto;
            MensagemErro = mensagemErro;
        }
    }
}
