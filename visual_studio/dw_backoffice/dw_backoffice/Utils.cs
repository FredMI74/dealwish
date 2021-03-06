using dw_backoffice.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dw_backoffice
{
    public class Utils
    {

        public static void formataCabecalho(dynamic ViewBag, HttpRequest Request)
        {
            ViewBag.logado = !string.IsNullOrWhiteSpace(Request.Cookies[Constantes.TOKEN_USUARIO]);
            ViewBag.nome_usuario = Request.Cookies[Constantes.NOME_USUARIO];
            ViewBag.bkc = Request.Cookies[Constantes.GRUPO_PERMISSAO_BKC] == Constantes.TRUE;
            ViewBag.bkf = Request.Cookies[Constantes.GRUPO_PERMISSAO_BKF] == Constantes.TRUE;
            ViewBag.bka = Request.Cookies[Constantes.GRUPO_PERMISSAO_BKA] == Constantes.TRUE;
            ViewBag.bki = Request.Cookies[Constantes.GRUPO_PERMISSAO_BKI] == Constantes.TRUE;
            ViewBag.tin = Request.Cookies[Constantes.GRUPO_PERMISSAO_TIN] == Constantes.TRUE;
        }

        public static void SetCookie(string cookie, string valor, HttpContext Context)
        {
            if (!string.IsNullOrWhiteSpace(Context.Request.Cookies[cookie]))
            {
                Context.Response.Cookies.Delete(cookie);
            }
            Context.Response.Cookies.Append(cookie, valor);
        }

        public static void SetCookie(string cookie, int valor, HttpContext Context)
        {
            string _valor = valor.ToString();
            if (!string.IsNullOrWhiteSpace(Context.Request.Cookies[cookie]))
            {
                Context.Response.Cookies.Delete(cookie);
            }
            Context.Response.Cookies.Append(cookie, _valor);

            string teste = "";
            teste = Context.Request.Cookies[cookie];
            teste = "";
        }

        public static void ClearAllCookies(HttpContext Context)
        {
            Context.Response.Cookies.Delete(Constantes.DW_BACKOFFICE);
            Context.Response.Cookies.Delete(Constantes.TOKEN_USUARIO);
            Context.Response.Cookies.Delete(Constantes.TOKEN_JWT_USUARIO);
            Context.Response.Cookies.Delete(Constantes.LOGO);
            Context.Response.Cookies.Delete(Constantes.FANTASIA);
            Context.Response.Cookies.Delete(Constantes.ID_EMPRESA);
            Context.Response.Cookies.Delete(Constantes.ID_CIDADE_EMPRESA);
            Context.Response.Cookies.Delete(Constantes.NOME_USUARIO);
            Context.Response.Cookies.Delete(Constantes.ID_USUARIO);
            Context.Response.Cookies.Delete(Constantes.EMAIL);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_BKC);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_BKF);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_BKA);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_BKI);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_TIN);
            Context.Response.Cookies.Delete(Constantes.ULTIMA_CONSULTA);
            Context.Response.Cookies.Delete(Constantes.ULTIMA_CONSULTA_AUX);
            Context.Response.Cookies.Delete(Constantes.TIPO_CONSULTA);
        }

        public static string FormatCPF(string sender)
        {
            string response = sender.Trim();
            if (response.Length == 11)
            {
                response = response.Insert(9, "-");
                response = response.Insert(6, ".");
                response = response.Insert(3, ".");
            }
            return response;
        }

        public static string FormatCNPJ(string sender)
        {
            string response = sender.Trim();
            if (response.Length == 14)
            {
                response = response.Insert(12, "-");
                response = response.Insert(8, "/");
                response = response.Insert(5, ".");
                response = response.Insert(3, ".");
            }
            return response;
        }
    }
}
