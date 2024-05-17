namespace DTO.Payload 
{ 
    public class PayloadGeneric<T> where T : class
    {
        public string? Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public T? ObjetoRetorno { get; set; }
        public string MensagemErro { get; set; }
        public PayloadGeneric(string mensagem, bool sucesso, string mensagemErro = "", T? objeto = null)
        {
            Mensagem = mensagem;
            Sucesso = sucesso;
            ObjetoRetorno = objeto;
            MensagemErro = mensagemErro;
        }
    }
}
