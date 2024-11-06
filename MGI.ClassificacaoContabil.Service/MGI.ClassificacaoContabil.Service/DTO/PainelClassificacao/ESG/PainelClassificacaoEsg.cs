namespace Service.DTO.PainelClassificacao
{
    public class PainelClassificacaoEsg
    {
        public CabecalhoEsg Cabecalho { get; set; }
        public IEnumerable<EmpresaEsgDTO> Empresas { get; set; }
    }
}
