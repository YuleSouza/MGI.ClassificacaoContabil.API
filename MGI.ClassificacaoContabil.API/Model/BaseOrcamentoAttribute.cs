using System.ComponentModel.DataAnnotations;

namespace MGI.ClassificacaoContabil.API.Model
{
    public class BaseOrcamentoAttribute : ValidationAttribute
    {
        private string[] baseOrcamentoValores = new string[] {"O","R","J","2","P","1"};
        protected override ValidationResult? IsValid(object valor, ValidationContext validationContext)
        {
            if (valor == null)
            {
                return new ValidationResult("O valor da propriedade BaseOrcamento deve ser O,R,J,2,P,1");
            }
            bool valorValido = baseOrcamentoValores.Any(p => p == (string)valor);
            if (valorValido) 
            { 
                return ValidationResult.Success;
            }
            return new ValidationResult(@"O valor da propriedade BaseOrcamento deve ser O,R,J,2,P,1");
        }
    }
}
