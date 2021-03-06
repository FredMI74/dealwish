using System.Security.Cryptography;
using System.Text;

namespace dw_frontoffice.Models
{
    public static class Constantes
    {
        //Sistema
        public const int ATIVO = 1;
        public const int BLOQUEADO = 3;
        public const int ABERTA = 8;
        public const int A_LIQUIDAR = 9;
        public const int LIQUIDADA = 10;
        public const int GERAR_REMESSA = 13;
        public const string CSV = "C";
        public const string NOTA_FISCAL = "F";
        public const string BOLETO = "B";
        public const string NAO = "N";
        public const string SIM = "S";
        public const string FRONTOFFICE = "F";
        public const string BACKOFFICE = "B";
        public const string FATURA = "F";
        public const string DESEJO = "D";

        //Cookies
        public static string DW_FRONTOFFICE = MD5Hash("dw_fto_frontoffice");
        public static string TOKEN_USUARIO = MD5Hash("dw_fto_token_usuario");
        public static string TOKEN_JWT_USUARIO = MD5Hash("dw_fto_token_jwt_usuario");
        public static string LOGO = MD5Hash("dw_fto_logo");
        public static string FANTASIA = MD5Hash("dw_fto_fantasia");
        public static string ID_EMPRESA = MD5Hash("dw_fto_id_empresa");
        public static string ID_CIDADE_EMPRESA = MD5Hash("dw_fto_id_cidade_empresa");
        public static string NOME_USUARIO = MD5Hash("dw_fto_nome_usuario");
        public static string ID_USUARIO = MD5Hash("dw_fto_id_usuario");
        public static string EMAIL = MD5Hash("dw_fto_email");
        public static string GRUPO_PERMISSAO_TIN = MD5Hash("dw_fto_grp_prm_tin");
        public static string GRUPO_PERMISSAO_FTA = MD5Hash("dw_fto_grp_prm_fta");
        public static string GRUPO_PERMISSAO_FTO = MD5Hash("dw_fto_grp_prm_fto");
        public static string ULTIMA_CONSULTA = MD5Hash("dw_fto_ultima_consulta");
        public static string ULTIMA_CONSULTA_AUX = MD5Hash("dw_fto_ultima_consulta_aux");
        public static string TRUE = MD5Hash("dw_fto_true");
        public static string FALSE = MD5Hash("dw_fto_false");

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

