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
            _configuration = configuration;
            _transactionHelper = transactionHelper;
            _painelEsgRepository = painelEsgRepository;
        }
        public async Task<byte[]> ObterAnexo(int idAnexo)
        {
            var anexo = await _painelEsgRepository.ConsultarAnexoiPorId(idAnexo);
            string caminho = _configuration.GetSection("dir_anexo").Value;
            var filePath = Path.Combine(caminho, anexo.NomeAnexo);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Arquivo não encontrado.", anexo.NomeAnexo);
            }
            return File.ReadAllBytes(filePath);
        }
        public async Task<PayloadDTO> InserirAnexos(List<AnexoJustificaitvaClassifEsgDTO> anexos)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                    async () =>
                    {
                        foreach (var anexo in anexos)
                        {
                            string prefixo = ObterPrefixoAnexo(anexo.IdProjeto);
                            anexo.NomeAnexo = $"{prefixo}{anexo.NomeAnexo}";
                        }
                        await _painelEsgRepository.InserirAnexoJustificativaEsg(anexos);
                        return true;
                    }, "Anexos salvos com sucesso!"
                );
        }
        public async Task<PayloadDTO> SalvarAnexos(List<IFormFile> arquivos, int idProjeto)
        {
            try
            {
                string? diretorio = _configuration.GetSection("dir_anexo").Value;
                foreach (var arquivo in arquivos)
                {
                    var filePath = Path.Combine(diretorio, $"{ObterPrefixoAnexo(idProjeto)}{arquivo.FileName}");
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
        private string ObterPrefixoAnexo(int idProjeto)
        {
            return $"PRJ_{idProjeto}_MET_";
        }
        public async Task<(string extensao, string nomeArquico)> GetContentType(int idAnexo)
        {
            var anexo = await _painelEsgRepository.ConsultarAnexoiPorId(idAnexo);
            var types = GetMimeTypes();
            var ext = Path.GetExtension(anexo.NomeAnexo).ToLowerInvariant();
            return (types[ext], anexo.NomeAnexo);
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
        public Task<PayloadDTO> ApagarAnexo(int id)
        {
            return _transactionHelper.ExecuteInTransactionAsync(
                    async () => 
                    {
                        var anexo = await _painelEsgRepository.ConsultarAnexoiPorId(id);
                        await ApagarArquivoAnexo(anexo.NomeAnexo);
                        await _painelEsgRepository.ApagarAnexo(id);
                        return true;
                        
                    },"Registro apagado com sucesso!"
                );
        }
        private async Task<PayloadDTO> ApagarArquivoAnexo(string nomeArquivo)
        {
            try
            {
                string? diretorio = _configuration.GetSection("dir_anexo").Value;
                var filePath = Path.Combine(diretorio, nomeArquivo);
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
        public async Task<IEnumerable<AnexoJustificaitvaClassifEsgDTO>> ConsultarAnexos(int idJustifClassif)
        {
            return await _painelEsgRepository.ConsultarAnexos(idJustifClassif);
        }
    }
}
