using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class SituacoesRepository 
    {
        readonly IConfiguration _configuration;
        public SituacoesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarSituacaoAsync(int id = 0, string descricao = "", string contratos = "", string usuarios = "", string desejos = "", string faturas = "", string produtos = "", string ofertas = "", string origem = "")
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0 && string.IsNullOrWhiteSpace(descricao) && string.IsNullOrWhiteSpace(contratos) && string.IsNullOrWhiteSpace(usuarios) &&
                    string.IsNullOrWhiteSpace(desejos) && string.IsNullOrWhiteSpace(faturas) && string.IsNullOrWhiteSpace(produtos) && string.IsNullOrWhiteSpace(ofertas))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "select id, descricao, contratos, usuarios, desejos, faturas from dealwish.situacoes where ";
                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(descricao))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(descricao) like upper(:descricao)";
                        dyParam.Add("descricao", "%" + descricao + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(contratos))
                    {
                        query = query + (tem_param ? " and " : "") + " contratos = upper(:contratos)";
                        dyParam.Add("contratos", contratos, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(usuarios))
                    {
                        query = query + (tem_param ? " and " : "") + " usuarios = upper(:usuarios)";
                        dyParam.Add("usuarios", usuarios, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(desejos))
                    {
                        query = query + (tem_param ? " and " : "") + " desejos = upper(:desejos)";
                        dyParam.Add("desejos", desejos, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(faturas))
                    {
                        query = query + (tem_param ? " and " : "") + " faturas = upper(:faturas)";
                        dyParam.Add("faturas", faturas, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(produtos))
                    {
                        query = query + (tem_param ? " and " : "") + " produtos = upper(:produtos)";
                        dyParam.Add("produtos", produtos, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(ofertas))
                    {
                        query = query + (tem_param ? " and " : "") + " ofertas = upper(:ofertas)";
                        dyParam.Add("ofertas", ofertas, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (origem == Constantes.FRONTOFFICE)
                    {
                        query = query + " and  (id <> " + Constantes.BLOQUEADO.ToString() + ") ";
                        query = query + " and  (id <> " + Constantes.GERAR_COBRANCA.ToString() + ") ";
                        query = query + " and  (id <> " + Constantes.GERAR_REMESSA_NF.ToString() + ") ";
                    }

                    query += " order by descricao";

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }
    }
}