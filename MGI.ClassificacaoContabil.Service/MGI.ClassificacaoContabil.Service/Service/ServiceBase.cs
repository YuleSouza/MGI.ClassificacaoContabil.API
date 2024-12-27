using DTO.Payload;
using Service.Helper;

namespace Service.Base
{
    public abstract class ServiceBase
    {
        private readonly ITransactionHelper _transactionHelper;
        protected ServiceBase(ITransactionHelper transactionHelper)
        {
            _transactionHelper = transactionHelper;
        }

        public async Task<PayloadDTO> ExecutarTransacao(Func<Task<bool>> action, string mensagemSucesso)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(action, mensagemSucesso);
        }
        protected void SetPayload<T>(T payload) where T : class
        {
            _transactionHelper.SetPayload(payload);
        }
    }
}
