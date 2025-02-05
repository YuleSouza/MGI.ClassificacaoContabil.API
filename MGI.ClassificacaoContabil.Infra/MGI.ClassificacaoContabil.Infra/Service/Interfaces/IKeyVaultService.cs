namespace Infra.Service.Interfaces
{
    public interface IKeyVaultService
    {
        Task<string> ConsultarSegredo(string nome);
    }
}
