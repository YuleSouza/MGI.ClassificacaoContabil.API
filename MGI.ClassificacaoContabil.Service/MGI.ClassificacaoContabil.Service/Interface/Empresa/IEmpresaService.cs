using DTO.Payload;

namespace Service.Interface.Empresa
{
    public interface IEmpresaService
    {
        Task<PayloadDTO> ConsultarEmpresa();
    }
}
