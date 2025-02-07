namespace MGI.ClassificacaoContabil.Infra.DTO
{
    public class EmailAutenticacaoServicoDTO
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string SenderEmail { get; set; }
        public string Uri { get; set; }
        public string TemplateDirectory { get; set; }
        public string UrlAprovacao {  get; set; }
    }
}
