using DTO.Payload;

namespace Service.Helper
{
    public interface ITransactionHelper
    {
        Task<PayloadDTO> ExecuteInTransactionAsync(Func<Task<bool>> action, string successMessage);
    }
}
