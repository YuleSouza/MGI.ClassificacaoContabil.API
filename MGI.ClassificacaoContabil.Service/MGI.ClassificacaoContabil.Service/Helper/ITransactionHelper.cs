using DTO.Payload;

namespace Service.Helper
{
    public interface ITransactionHelper
    {
        Task<PayloadDTO> ExecuteInTransactionAsync(Func<Task<bool>> action, string successMessage, string mensagemErro = "");
        void SetPayload<T>(T payload) where T : class;
    }
}
