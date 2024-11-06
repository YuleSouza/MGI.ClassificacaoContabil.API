using Service.DTO.Classificacao;
using Service.DTO.Empresa;

namespace Service.DTO.PainelClassificacao
{
    public class PainelClassificacaoContabilDTO
    {
        public IList<EmpresaDTO> Empresas { get; set; }       
        public IList<TotalizadorContabil> Totalizador { get; set; }
        public IList<ClassificacaoContabilMgpDTO> Cabecalho { get; set; }
    }
}
