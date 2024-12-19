using DTO.Payload;
using Service.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.DTO.Esg;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class EsgAnexoService : IEsgAnexoService
    {
        private IConfiguration _configuration;
        private ITransactionHelper _transactionHelper;
        private readonly IPainelEsgRepository _painelEsgRepository;
        public EsgAnexoService(IConfiguration configuration
            , ITransactionHelper transactionHelper
            , IPainelEsgRepository painelEsgRepository)
        {
            configuration = _configuration;
            _transactionHelper = transactionHelper;
            _painelEsgRepository = painelEsgRepository;
        }
        public async Task<byte[]> ObterArquivo(string nomeArquivo)
        {
            string caminho = _configuration.GetSection("dir_anexo").Value;
            var filePath = Path.Combine(caminho, nomeArquivo);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Arquivo não encontrado.", nomeArquivo);
            }
            return File.ReadAllBytes(filePath);
        }
        public async Task<PayloadDTO> InserirAnexos(List<AnexoJustificaitvaClassifEsgDTO> anexos)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                    async () =>
                    {
                        await _painelEsgRepository.InserirAnexoJustificativaEsg(anexos);
                        return true;
                    }, "Anexos salvos com sucesso!"
                );
        }
        public async Task<PayloadDTO> SalvarAnexos(List<IFormFile> arquivos)
        {
            try
            {
                string? diretorio = _configuration.GetSection("dir_anexo").Value;
                foreach (var arquivo in arquivos)
                {
                    var filePath = Path.Combine(diretorio, arquivo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        arquivo.CopyTo(stream);                        
                    }
                }
                return new PayloadDTO("Arquivos salvos com sucesso", true);
            }
            catch (Exception ex)
            {
                return new PayloadDTO(string.Empty, false, "Ocorreu um erro ao salvar os arquivos.");
            }
        }
        public string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string> {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
        public async Task<PayloadDTO> ApagarArquivoAnexo(string arquivo)
        {
            try
            {
                string? diretorio = _configuration.GetSection("dir_anexo").Value;
                var filePath = Path.Combine(diretorio, arquivo);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    return new PayloadDTO("Arquivo não encontrado!", false);
                }                
                return new PayloadDTO("Anexo deletado com sucesso!", true);
            }
            catch (Exception ex)
            {
                return new PayloadDTO(string.Empty, false, "Ocorreu um erro ao salvar os arquivos.");
            }
        }
        public Task<PayloadDTO> ApagarAnexo(int id)
        {
            return _transactionHelper.ExecuteInTransactionAsync(
                    async () => await _painelEsgRepository.ApagarAnexo(id),"Registro apagado com sucesso!"
                );
        }
    }
}
