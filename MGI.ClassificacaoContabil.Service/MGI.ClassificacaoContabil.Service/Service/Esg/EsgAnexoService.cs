using DTO.Payload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Base;
using Service.DTO.Esg;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class EsgAnexoService : ServiceBase, IEsgAnexoService
    {
        private IConfiguration _configuration;        
        private readonly IEsgAnexoRepository _esgAnexoRepository;
        public EsgAnexoService(IConfiguration configuration
            , ITransactionHelper transactionHelper
            , IEsgAnexoRepository esgAnexoRepository) : base(transactionHelper)
        {
            _configuration = configuration;
            _esgAnexoRepository = esgAnexoRepository;
        }
        public async Task<byte[]> ObterAnexo(int idAnexo)
        {
            var anexo = await _esgAnexoRepository.ConsultarAnexoiPorId(idAnexo);
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
            return await ExecutarTransacao(
                    async () =>
                    {
                        foreach (var anexo in anexos)
                        {
                            string prefixo = ObterPrefixoAnexo(anexo.IdProjeto);
                            anexo.NomeAnexo = $"{prefixo}{anexo.NomeAnexo}";
                        }
                        await _esgAnexoRepository.InserirAnexoJustificativaEsg(anexos);
                        return true;
                    }, "Anexos salvos com sucesso!"
                );
        }
        public async Task<PayloadDTO> SalvarAnexo(IFormFile arquivo, int idProjeto)
        {
            string? diretorio = _configuration.GetSection("dir_anexo").Value;                
            var filePath = Path.Combine(diretorio, $"{ObterPrefixoAnexo(idProjeto)}{arquivo.FileName}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                arquivo.CopyTo(stream);
            }
            return new PayloadDTO("Arquivos salvos com sucesso", true);
        }
        private string ObterPrefixoAnexo(int idProjeto)
        {
            return $"PRJ_{idProjeto}_MET_";
        }
        public async Task<(string extensao, string nomeArquico)> GetContentType(int idAnexo)
        {
            var anexo = await _esgAnexoRepository.ConsultarAnexoiPorId(idAnexo);
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
        public Task<PayloadDTO> ExcluirAnexo(int id)
        {
            return ExecutarTransacao(
                    async () => 
                    {
                        var anexo = await _esgAnexoRepository.ConsultarAnexoiPorId(id);
                        await ExcluirArquivoAnexo(anexo.NomeAnexo);
                        await _esgAnexoRepository.RemoverAnexo(id);
                        return true;
                        
                    },"Registro apagado com sucesso!"
                );
        }
        private async Task<PayloadDTO> ExcluirArquivoAnexo(string nomeArquivo)
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
            return await _esgAnexoRepository.ConsultarAnexos(idJustifClassif);
        }
    }
}
