using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dw_webservice.Repositories
{
    public class EmpresasRepository 
    {
        readonly IConfiguration _configuration;
        public EmpresasRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public async Task<object> ConsultarEmpresaAsync(int id = 0, string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string uf = "")
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                cnpj = utils.SomenteNumeros(cnpj);

                if (id == 0 && string.IsNullOrWhiteSpace(fantasia) && string.IsNullOrWhiteSpace(razao_social) && string.IsNullOrWhiteSpace(cnpj) && string.IsNullOrWhiteSpace(insc_est)
                    && string.IsNullOrWhiteSpace(url) && string.IsNullOrWhiteSpace(email_com) && string.IsNullOrWhiteSpace(email_sac) &&
                    string.IsNullOrWhiteSpace(fone_com) && string.IsNullOrWhiteSpace(fone_sac) && string.IsNullOrWhiteSpace(endereco) && string.IsNullOrWhiteSpace(numero) &&
                    string.IsNullOrWhiteSpace(complemento) && string.IsNullOrWhiteSpace(bairro) && string.IsNullOrWhiteSpace(cep) && string.IsNullOrWhiteSpace(endereco_cob) &&
                    string.IsNullOrWhiteSpace(numero_cob) && string.IsNullOrWhiteSpace(complemento_cob) && string.IsNullOrWhiteSpace(bairro_cob) && string.IsNullOrWhiteSpace(cep_cob) &&
                    id_cidade == 0 && id_cidade_cob == 0 && string.IsNullOrWhiteSpace(uf))
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
                    var query = "select e.id, e.fantasia, e.razao_social, e.cnpj, e.insc_est, e.url, e.email_com, e.email_sac, e.fone_com, e.fone_sac, " +
                        "e.endereco, e.numero, e.complemento, e.bairro, e.cep, e.endereco_cob, e.numero_cob, e.complemento_cob, e.bairro_cob, e.cep_cob, e.id_cidade, " +
                        "e.id_cidade_cob, c.nome nome_cidade, c.uf ,cb.nome nome_cidade_cob, cb.uf uf_cob, e.logo, e.id_qualificacao, q.descricao desc_qualificacao " +
                        "from dealwish.empresas e, dealwish.cidades c, dealwish.cidades cb, dealwish.qualificacoes q " +
                        "where e.id_cidade = c.id and e.id_cidade_cob = cb.id and e.id_qualificacao = q.id and (";
                    var tem_param = false;

                    if (id != 0)
                    {
                        query += " e.id = :id";
                        dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(fantasia))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.fantasia) like upper(:fantasia)";
                        dyParam.Add("fantasia", "%" + fantasia + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }


                    if (!string.IsNullOrWhiteSpace(razao_social))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.razao_social) like upper(:razao_social)";
                        dyParam.Add("razao_social", "%" + razao_social + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(cnpj))
                    {
                        query = query + (tem_param ? " and " : "") + " e.cnpj = :cnpj";
                        dyParam.Add("cnpj", cnpj, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(insc_est))
                    {
                        query = query + (tem_param ? " and " : "") + " e.insc_est = :insc_est";
                        dyParam.Add("insc_est", insc_est, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.url) like upper(:url)";
                        dyParam.Add("url", "%" + url + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(email_com))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.email_com) like upper(:email_com)";
                        dyParam.Add("email_com", "%" + email_com + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(email_sac))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.email_sac) like upper(:email_sac)";
                        dyParam.Add("email_sac", "%" + email_sac + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(fone_com))
                    {
                        query = query + (tem_param ? " and " : "") + " e.fone_com like fone_com";
                        dyParam.Add("fone_com", "%" + fone_com + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(fone_sac))
                    {
                        query = query + (tem_param ? " and " : "") + " e.fone_sac like :fone_sac";
                        dyParam.Add("fone_sac", "%" + fone_sac + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(endereco))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.endereco) like upper(:endereco)";
                        dyParam.Add("endereco", "%" + endereco + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(numero))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.numero) like upper(:numero)";
                        dyParam.Add("numero", "%" + numero + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(complemento))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.complemento) like upper(:complemento)";
                        dyParam.Add("complemento", "%" + complemento + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(bairro))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.bairro) like upper(:bairro)";
                        dyParam.Add("bairro", "%" + bairro + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(cep))
                    {
                        query = query + (tem_param ? " and " : "") + " e.cep = :cep";
                        dyParam.Add("cep", cep, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(endereco_cob))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.endereco_cob) like upper(:endereco_cob)";
                        dyParam.Add("endereco_cob", "%" + endereco_cob + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(numero_cob))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.numero_cob) like upper(:numero_cob)";
                        dyParam.Add("numero_cob", "%" + numero_cob + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(complemento_cob))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.complemento_cob) like upper(:complemento_cob)";
                        dyParam.Add("complemento_cob", "%" + complemento_cob + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(bairro_cob))
                    {
                        query = query + (tem_param ? " and " : "") + " upper(e.bairro_cob) like upper(:bairro_cob)";
                        dyParam.Add("bairro_cob", "%" + bairro_cob + "%", DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(cep_cob))
                    {
                        query = query + (tem_param ? " and " : "") + " e.cep_cob = :cep_cob";
                        dyParam.Add("cep_cob", cep_cob, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_cidade != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " e.id_cidade = :id_cidade";
                        dyParam.Add("id_cidade", id_cidade, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (id_cidade_cob != 0)
                    {
                        query = query + (tem_param ? " and " : "") + " e.id_cidade_cob = :id_cidade_cob";
                        dyParam.Add("id_cidade_cob", id_cidade_cob, DbType.Int32, ParameterDirection.Input);
                        tem_param = true;
                    }

                    if (!string.IsNullOrWhiteSpace(uf))
                    {
                        query = query + (tem_param ? " and " : "") + " c.uf = :uf";
                        dyParam.Add("uf", uf, DbType.String, ParameterDirection.Input);
                        tem_param = true;
                    }

                    query += ")";

                    result = await SqlMapper.QueryAsync(conn, query, param: dyParam, commandType: CommandType.Text);

                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> IncluirEmpresaAsync(string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string logo = "", int id_qualificacao = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                cnpj = utils.SomenteNumeros(cnpj);

                if (string.IsNullOrWhiteSpace(fantasia) || string.IsNullOrWhiteSpace(razao_social) || string.IsNullOrWhiteSpace(cnpj) || string.IsNullOrWhiteSpace(insc_est) ||
                    string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(email_com) || string.IsNullOrWhiteSpace(email_sac) || string.IsNullOrWhiteSpace(fone_com) ||
                    string.IsNullOrWhiteSpace(fone_sac) || string.IsNullOrWhiteSpace(endereco) || string.IsNullOrWhiteSpace(numero) || string.IsNullOrWhiteSpace(bairro) ||
                    string.IsNullOrWhiteSpace(cep) || string.IsNullOrWhiteSpace(endereco_cob) || string.IsNullOrWhiteSpace(numero_cob) || string.IsNullOrWhiteSpace(bairro_cob) ||
                    string.IsNullOrWhiteSpace(cep_cob) || id_cidade == 0 || id_cidade_cob == 0 || string.IsNullOrWhiteSpace(logo) || id_qualificacao < 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Nome Fantasia, Razão Social, CNPJ, IE, URL, e-mail Com., e-mail SAC, Fone Com., Fone SAC, Endereço, Número, Bairro, CEP, Endereço Cob., Número Cob., Bairro Cob., Cep. Cob., Cód. Cidade, Cód. Cidade Cob., Cód. Qualificação e Logo." }));
                }

                if (!utils.IsCnpj(cnpj))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "CNPJ inválido." }));
                }

                if (!utils.ValidarEmail(email_com))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail comercial inválido." }));
                }

                if (!utils.ValidarEmail(email_sac))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail SAC inválido." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "insert into dealwish.empresas (fantasia, razao_social, cnpj, insc_est, url, email_com, email_sac, " +
                                 "fone_com, fone_sac, endereco, numero, complemento, bairro, cep, endereco_cob, numero_cob, " +
                                 "complemento_cob, bairro_cob, cep_cob, id_cidade, id_cidade_cob, logo, id_qualificacao, id_usuario_log) values " +
                                 "(:fantasia, :razao_social, :cnpj, :insc_est, :url, :email_com, :email_sac, :fone_com, :fone_sac, " +
                                 ":endereco, :numero, :complemento, :bairro, :cep, :endereco_cob, :numero_cob, :complemento_cob, :bairro_cob, " +
                                 ":cep_cob, :id_cidade, :id_cidade_cob, :logo, :id_qualificacao, :id_usuario_login) returning id";

                    dyParam.Add("id", null, DbType.Int32, ParameterDirection.Output);
                    dyParam.Add("fantasia", fantasia, DbType.String, ParameterDirection.Input);
                    dyParam.Add("razao_social", razao_social, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cnpj", cnpj, DbType.String, ParameterDirection.Input);
                    dyParam.Add("insc_est", insc_est, DbType.String, ParameterDirection.Input);
                    dyParam.Add("url", url, DbType.String, ParameterDirection.Input);
                    dyParam.Add("email_com", email_com, DbType.String, ParameterDirection.Input);
                    dyParam.Add("email_sac", email_sac, DbType.String, ParameterDirection.Input);
                    dyParam.Add("fone_com", fone_com, DbType.String, ParameterDirection.Input);
                    dyParam.Add("fone_sac", fone_sac, DbType.String, ParameterDirection.Input);
                    dyParam.Add("endereco", endereco, DbType.String, ParameterDirection.Input);
                    dyParam.Add("numero", numero, DbType.String, ParameterDirection.Input);
                    dyParam.Add("complemento", complemento, DbType.String, ParameterDirection.Input);
                    dyParam.Add("bairro", bairro, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cep", cep, DbType.String, ParameterDirection.Input);
                    dyParam.Add("endereco_cob", endereco_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("numero_cob", numero_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("complemento_cob", complemento_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("bairro_cob", bairro_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cep_cob", cep_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_cidade", id_cidade, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_cidade_cob", id_cidade_cob, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("logo", logo, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_qualificacao", id_qualificacao, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    result = new { ID = dyParam.Get<int>("id") };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ExcluirEmpresaAsync(int id = 0, long id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                if (id == 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    transaction = conn.BeginTransaction();

                    var query = "update dealwish.empresas set id_usuario_log = :id_usuario_login where id = :id ";
                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "delete from dealwish.empresas where id = :id ";
                    linhasafetadas += await SqlMapper.ExecuteAsync(conn, query, dyParam, transaction, null, CommandType.Text);

                    transaction.Commit();

                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> AtualizarEmpresaAsync(int id = 0, string fantasia = "", string razao_social = "", string cnpj = "", string insc_est = "", string url = "", string email_com = "", string email_sac = "", string fone_com = "", string fone_sac = "", string endereco = "", string numero = "", string complemento = "", string bairro = "", string cep = "", string endereco_cob = "", string numero_cob = "", string complemento_cob = "", string bairro_cob = "", string cep_cob = "", int id_cidade = 0, int id_cidade_cob = 0, string logo = "", int id_qualificacao = 0, long id_usuario_login = 0)
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                cnpj = utils.SomenteNumeros(cnpj);

                if (string.IsNullOrWhiteSpace(fantasia) || string.IsNullOrWhiteSpace(razao_social) || string.IsNullOrWhiteSpace(cnpj) || string.IsNullOrWhiteSpace(insc_est) ||
                    string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(email_com) || string.IsNullOrWhiteSpace(email_sac) || string.IsNullOrWhiteSpace(fone_com) ||
                    string.IsNullOrWhiteSpace(fone_sac) || string.IsNullOrWhiteSpace(endereco) || string.IsNullOrWhiteSpace(numero) || string.IsNullOrWhiteSpace(bairro) ||
                    string.IsNullOrWhiteSpace(cep) || string.IsNullOrWhiteSpace(endereco_cob) || string.IsNullOrWhiteSpace(numero_cob) || string.IsNullOrWhiteSpace(bairro_cob) ||
                    string.IsNullOrWhiteSpace(cep_cob) || id_cidade == 0 || id_cidade_cob == 0 || string.IsNullOrWhiteSpace(logo) || id_qualificacao < 0)
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Nome Fantasia, Razão Social, CNPJ, IE, URL, e-mail Com., e-mail SAC, Fone Com., Fone SAC, Endereço, Número, Bairro, CEP, Endereço Cob., Número Cob., Bairro Cob., CEP Cob., Cód. Cidade, Cód. Cidade Cob., Cód. Qualificação e Logo." }));
                }

                if (!utils.IsCnpj(cnpj))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "CNPJ inválido." }));
                }

                if (!utils.ValidarEmail(email_com))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail comercial inválido." }));
                }

                if (!utils.ValidarEmail(email_sac))
                {
                    return (utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail SAC inválido." }));
                }

                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "update dealwish.empresas set fantasia = :fantasia, razao_social = :razao_social, cnpj = :cnpj, insc_est = :insc_est, " +
                                "url = :url, email_com = :email_com , email_sac = :email_sac, fone_com = :fone_com, fone_sac = :fone_sac, " +
                                "endereco = :endereco, numero = :numero, complemento = :complemento, bairro = :bairro, cep = :cep, " +
                                "endereco_cob = :endereco_cob, numero_cob = :numero_cob, complemento_cob = :complemento_cob, bairro_cob = :bairro_cob, " +
                                "cep_cob = :cep_cob, id_cidade = :id_cidade, id_cidade_cob = :id_cidade_cob, logo = :logo, " +
                                "id_qualificacao = :id_qualificacao, id_usuario_log = :id_usuario_login where id = :id ";

                    dyParam.Add("id", id, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("fantasia", fantasia, DbType.String, ParameterDirection.Input);
                    dyParam.Add("razao_social", razao_social, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cnpj", cnpj, DbType.String, ParameterDirection.Input);
                    dyParam.Add("insc_est", insc_est, DbType.String, ParameterDirection.Input);
                    dyParam.Add("url", url, DbType.String, ParameterDirection.Input);
                    dyParam.Add("email_com", email_com, DbType.String, ParameterDirection.Input);
                    dyParam.Add("email_sac", email_sac, DbType.String, ParameterDirection.Input);
                    dyParam.Add("fone_com", fone_com, DbType.String, ParameterDirection.Input);
                    dyParam.Add("fone_sac", fone_sac, DbType.String, ParameterDirection.Input);
                    dyParam.Add("endereco", endereco, DbType.String, ParameterDirection.Input);
                    dyParam.Add("numero", numero, DbType.String, ParameterDirection.Input);
                    dyParam.Add("complemento", complemento, DbType.String, ParameterDirection.Input);
                    dyParam.Add("bairro", bairro, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cep", cep, DbType.String, ParameterDirection.Input);
                    dyParam.Add("endereco_cob", endereco_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("numero_cob", numero_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("complemento_cob", complemento_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("bairro_cob", bairro_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("cep_cob", cep_cob, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_cidade", id_cidade, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_cidade_cob", id_cidade_cob, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("logo", logo, DbType.String, ParameterDirection.Input);
                    dyParam.Add("id_qualificacao", id_qualificacao, DbType.Int32, ParameterDirection.Input);
                    dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                    var linhasafetadas = await SqlMapper.ExecuteAsync(conn, query, dyParam, null, null, CommandType.Text);
                    result = new { linhasafetadas };
                }
            }
            catch (Exception ex)
            {
                return (utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
            }

            return (utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
        }

        public async Task<object> ConsultarQualificacaoAsync()
        {
            object result = null;
            Utils utils = new();

            using NpgsqlConnection conn = new(Constantes.string_conexao);

            try
            {
                var dyParam = new DynamicParameters();

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    var query = "select id, descricao from dealwish.qualificacoes order by id";
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