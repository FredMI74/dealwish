using System.Security.Cryptography;
using System.Text;

namespace dw_backoffice.Models
{
    public static class Constantes
    {
        //Sistema
        public const int ATIVO = 1;
        public const int BLOQUEADO = 3;
        public const int ABERTA = 8;
        public const int A_LIQUIDAR = 9;
        public const int LIQUIDADA = 10;
        public const int CANCELADA = 11;
        public const int GERAR_REMESSA = 12;
        public const string CSV = "C";
        public const string NOTA_FISCAL = "F";
        public const string BOLETO = "B";
        public const string PIX = "P";
        public const string NAO = "N";
        public const string SIM = "S";
        public const string FRONTOFFICE = "F";
        public const string BACKOFFICE = "B";
        public const string FATURA = "F";
        public const int ID_DEALWISH = 1;

        //Cookies
        public static string DW_BACKOFFICE = MD5Hash("dw_bko_backoffice");
        public static string TOKEN_USUARIO = MD5Hash("dw_bko_token_usuario");
        public static string TOKEN_JWT_USUARIO = MD5Hash("dw_bko_token_jwt_usuario");
        public static string LOGO = MD5Hash("dw_bko_logo");
        public static string FANTASIA = MD5Hash("dw_bko_fantasia");
        public static string ID_EMPRESA = MD5Hash("dw_bko_id_empresa");
        public static string ID_CIDADE_EMPRESA = MD5Hash("dw_bko_id_cidade_empresa");
        public static string NOME_USUARIO = MD5Hash("dw_bko_nome_usuario");
        public static string ID_USUARIO = MD5Hash("dw_bko_id_usuario");
        public static string EMAIL = MD5Hash("dw_bko_email");
        public static string GRUPO_PERMISSAO_BKC = MD5Hash("dw_bko_grp_prm_bkc");
        public static string GRUPO_PERMISSAO_BKF = MD5Hash("dw_bko_grp_prm_bkf");
        public static string GRUPO_PERMISSAO_BKA = MD5Hash("dw_bko_grp_prm_bka");
        public static string GRUPO_PERMISSAO_BKI = MD5Hash("dw_bko_grp_prm_bki");
        public static string GRUPO_PERMISSAO_TIN = MD5Hash("dw_bko_grp_prm_tin");
        public static string ULTIMA_CONSULTA = MD5Hash("dw_bko_ultima_consulta");
        public static string ULTIMA_CONSULTA_AUX = MD5Hash("dw_bko_ultima_consulta_aux");
        public static string TIPO_CONSULTA = MD5Hash("dw_bko_tipo_consulta");
        public static string TRUE = MD5Hash("dw_bko_true");
        public static string FALSE = MD5Hash("dw_bko_false");

        private static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }


}

