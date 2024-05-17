using Service.DTO.Empresa;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class PainelClassificacaoEsg
    {
        public CabecalhoEsg Cabecalho { get; set; }
        public IEnumerable<EmpresaDTO> Empresas { get; set; }
    }
}
