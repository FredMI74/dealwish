using System;

namespace dw_webservice.Models
{
    public class Fatura
    {
        public int Id { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int Id_empresa { get; set; }
        public string Nosso_numero { get; set; }
        public double Valor { get; set; }
        public DateTime Data_vct { get; set; }
        public DateTime Data_pg { get; set; }
        public double Multa { get; set; }
        public double Juros { get; set; }
        public int Qtd_ofertas { get; set; }
        public double Id_situacao { get; set; }
        public string Razao_social { get; set; }
        public string Desc_situacao { get; set; }
        public string Pix { get; set; }
    }
}
