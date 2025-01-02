using DTO.Payload;
using Microsoft.AspNetCore.Http;
using Service.DTO.Esg;

namespace Service.Interface.PainelEsg
{
    public interface IEsgAnexoService
    {
        Task<byte[]> ObterAnexo(int idAnexo);
        Task<PayloadDTO> SalvarAnexo(IFormFile arquivo, int idProjeto);
        Task<PayloadDTO> InserirAnexos(List<AnexoJustificaitvaClassifEsgDTO> anexos);
        Task<(string extensao, string nomeArquico)> GetContentType(int idAnexo);
        Task<PayloadDTO> ExcluirAnexo(int id);
        Task<IEnumerable<AnexoJustificaitvaClassifEsgDTO>> ConsultarAnexos(int idsJustifClassif);
    }
}
