namespace dw_backoffice.Models
{
    public class ProcessarRegistro : Base
    {
        public class Dados 
        {
            public int Linhasafetadas { get; set; }
        }

        public Dados Conteudo { get; set; }

    }
}
