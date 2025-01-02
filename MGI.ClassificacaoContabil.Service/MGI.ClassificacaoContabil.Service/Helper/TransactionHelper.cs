using DTO.Payload;
using Infra.Interface;

namespace Service.Helper
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private object? _objetoRetorno = null;

        public TransactionHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void SetPayload<T>(T payload) where T : class
        {
            _objetoRetorno = payload;
        }
        public async Task<PayloadDTO> ExecuteInTransactionAsync(Func<Task<bool>> action, string successMessage, string mensagemErro = "")
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool result = await action();
                    if (!result)
                    {
                        unitOfWork.Rollback();
                        return new PayloadDTO(string.Empty, result, mensagemErro);
                    }
                    unitOfWork.Commit();
                    return new PayloadDTO(successMessage, result, string.Empty, _objetoRetorno);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }
    }
}
