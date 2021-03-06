namespace dw_webservice.Models
{
    public static class Constantes
    {
        public static string string_conexao = "";

        public const string RETORNO_PROCESSAMENTO_NF_CSV = "_retorno_processamento_nf.csv";
        public const string DESEJOS_CSV = "_desejos.csv";
        public const string RETORNO_PROCESSAMENTO_BOLETO_CSV = "_retorno_processamento_boleto.csv";
        public const string LOTEOFERTAS_CSV = "_loteofertas.csv";
        public const int ATIVO = 1;
        public const int INATIVO = 2;
        public const int BLOQUEADO = 3;
        public const int ENCERRADO = 4;
        public const int ABERTA = 8;
        public const int A_LIQUIDAR = 9;
        public const int LIQUIDADA = 10;
        public const int GERAR_REMESSA_NF = 12;
        public const int GERAR_COBRANCA = 13;
        public const string CSV = "C";
        public const string NOTA_FISCAL = "F";
        public const string BOLETO = "B";
        public const string PIX = "P";
        public const string NAO = "N";
        public const string SIM = "S";
        public const string FRONTOFFICE = "F";
        public const string BACKOFFICE = "B";
        public const string APLICATIVO = "A";
        public const string VISUALIZACAO_PRECO = "P";
        public const string VISUALIZACAO_EMPRESA_PRECO = "E";

        public const int ID_DEALWISH = 1;

        public const string MENSAGEM_CONSULTA = "Informe ao menos um critério de busca.";
        public const string MENSAGEM_INCLUSAO_ATUALIZACAO = "Campo(s) obrigatório(s): ";
    }
}

