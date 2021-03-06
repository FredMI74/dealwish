using dw_frontoffice.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace dw_frontoffice
{
    public class Utils
    {

       public static void formataCabecalho(dynamic ViewBag, HttpRequest Request)
        {
            ViewBag.logado = !string.IsNullOrWhiteSpace(Request.Cookies[Constantes.TOKEN_USUARIO]);
            ViewBag.logo = Request.Cookies[Constantes.LOGO];
            ViewBag.fantasia = Request.Cookies[Constantes.FANTASIA];
            ViewBag.id_empresa = Request.Cookies[Constantes.ID_EMPRESA];
            ViewBag.nome_usuario = Request.Cookies[Constantes.NOME_USUARIO];
            ViewBag.fta = Request.Cookies[Constantes.GRUPO_PERMISSAO_FTA] == Constantes.TRUE;
            ViewBag.fto = Request.Cookies[Constantes.GRUPO_PERMISSAO_FTO] == Constantes.TRUE;
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
        }

        public static void ClearAllCookies(HttpContext Context)
        {
            Context.Response.Cookies.Delete(Constantes.DW_FRONTOFFICE);
            Context.Response.Cookies.Delete(Constantes.TOKEN_USUARIO);
            Context.Response.Cookies.Delete(Constantes.TOKEN_JWT_USUARIO);
            Context.Response.Cookies.Delete(Constantes.LOGO);
            Context.Response.Cookies.Delete(Constantes.FANTASIA);
            Context.Response.Cookies.Delete(Constantes.ID_EMPRESA);
            Context.Response.Cookies.Delete(Constantes.ID_CIDADE_EMPRESA);
            Context.Response.Cookies.Delete(Constantes.NOME_USUARIO);
            Context.Response.Cookies.Delete(Constantes.ID_USUARIO);
            Context.Response.Cookies.Delete(Constantes.EMAIL);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_TIN);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_FTA);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_FTO);
            Context.Response.Cookies.Delete(Constantes.GRUPO_PERMISSAO_TIN);
            Context.Response.Cookies.Delete(Constantes.ULTIMA_CONSULTA);
            Context.Response.Cookies.Delete(Constantes.ULTIMA_CONSULTA_AUX);
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
