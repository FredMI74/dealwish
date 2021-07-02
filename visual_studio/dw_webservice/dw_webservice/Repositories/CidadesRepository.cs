using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;


namespace dw_webservice.Repositories
{
    public class CidadesRepository
    {
        readonly IConfiguration _configuration;
        public CidadesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarCidadeAsync(int id = 0, string nome = "", string nome_exato = "", string uf = "")
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {

                if (id == 0 && string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(uf) && string.IsNullOrWhiteSpace(nome_exato))
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
                    var query = "select id, nome, uf from dealwish.cidades where ";
                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " id = :id";
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

                    query += " order by uf, nome ";

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