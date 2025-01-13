namespace DTO.Payload
{
    public sealed class PayloadDTO
    {
        public string? Mensagem { get; private set; }
        public bool Sucesso { get; private set; }
        public object? ObjetoRetorno { get; private set; }
        public string MensagemErro { get; private set; }
        public PayloadDTO(string mensagem, bool sucesso, string mensagemErro = "", object? objeto = null)
        {
            Mensagem = mensagem;
            Sucesso = sucesso;
            ObjetoRetorno = objeto;
            MensagemErro = mensagemErro;
        }
    }
}
