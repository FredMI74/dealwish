namespace dw_frontoffice.Models
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
