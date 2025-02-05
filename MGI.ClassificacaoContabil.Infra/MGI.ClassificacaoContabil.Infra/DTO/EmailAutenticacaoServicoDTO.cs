namespace MGI.ClassificacaoContabil.Infra.DTO
{
    public class EmailAutenticacaoServicoDTO
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string[] Scopes { get; set; }
    }
}
