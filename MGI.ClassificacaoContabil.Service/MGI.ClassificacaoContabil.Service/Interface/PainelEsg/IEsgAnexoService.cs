using DTO.Payload;
using Microsoft.AspNetCore.Http;
using Service.DTO.Esg;

namespace Service.Interface.PainelEsg
{
    public interface IEsgAnexoService
    {
        Task<byte[]> ObterArquivo(string nomeArquivo);
        Task<PayloadDTO> SalvarAnexos(List<IFormFile> arquivos);
        Task<PayloadDTO> InserirAnexos(List<AnexoJustificaitvaClassifEsgDTO> anexos);
        string GetContentType(string path);        
        Task<PayloadDTO> ApagarAnexo(int id);
        Task<IEnumerable<AnexoJustificaitvaClassifEsgDTO>> ConsultarAnexos(int idJustifClassif);
    }
}
