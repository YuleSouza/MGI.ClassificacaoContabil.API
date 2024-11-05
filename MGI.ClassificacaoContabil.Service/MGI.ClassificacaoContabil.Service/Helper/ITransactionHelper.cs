using DTO.Payload;

namespace MGI.ClassificacaoContabil.Service.Helper
{
    public interface ITransactionHelper
    {
        Task<PayloadDTO> ExecuteInTransactionAsync(Func<Task<bool>> action, string successMessage);
    }
}
