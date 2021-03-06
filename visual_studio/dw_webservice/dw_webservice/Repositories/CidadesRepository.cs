using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;


namespace dw_webservice.Repositories
{
    public class CidadesRepository
    {
        IConfiguration configuration;
        public CidadesRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_cidade(int id = 0, string nome = "", string nome_exato = "", string uf = "")
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id == 0 && string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(uf) && string.IsNullOrWhiteSpace(nome_exato))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_CONSULTA }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select id, nome, uf from dealwish.cidades where ";
                        var tem_param = false;

                        if (id != 0)
                        {
                            query = query + " id = :id";
                            dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(nome))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(nome) like upper(:nome)";
                            dyParam.Add("nome", "%" + nome + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }


                        if (!string.IsNullOrWhiteSpace(nome_exato))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(nome) = upper(:nome_exato)";
                            dyParam.Add("nome_exato", nome_exato, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(uf))
                        {
                            query = query + (tem_param ? " and " : "") + " uf = upper(:uf)";
                            dyParam.Add("uf", uf, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        query = query + " order by uf, nome ";

                        result = SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.Text);

                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

    }
}