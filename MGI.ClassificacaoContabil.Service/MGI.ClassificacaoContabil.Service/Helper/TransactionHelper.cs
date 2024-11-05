using DTO.Payload;
using Infra.Interface;
using MGI.ClassificacaoContabil.Service.Helper;

public class TransactionHelper : ITransactionHelper
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionHelper(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PayloadDTO> ExecuteInTransactionAsync(Func<Task<bool>> action, string successMessage)
    {
        using (IUnitOfWork unitOfWork = _unitOfWork)
        {
            try
            {
                unitOfWork.BeginTransaction();
                bool result = await action();
                unitOfWork.Commit();
                return new PayloadDTO(successMessage, result, string.Empty);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }
    }
}
