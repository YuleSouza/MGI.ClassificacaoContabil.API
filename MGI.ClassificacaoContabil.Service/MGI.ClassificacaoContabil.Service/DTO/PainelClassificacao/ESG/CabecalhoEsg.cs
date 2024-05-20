using Service.DTO.Classificacao;

namespace MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG
{
    public class CabecalhoEsg
    {
        public CabecalhoEsg()
        {
            ClassificacoesEsg = new List<ClassificacaoEsgDTO>();
        }
        public IList<ClassificacaoEsgDTO> ClassificacoesEsg {  get; set; }
    }
}
