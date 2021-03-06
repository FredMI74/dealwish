using Dapper;
using dw_webservice.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace dw_webservice.Repositories
{
    public class UsuariosRepository 
    {
        IConfiguration configuration;
        public UsuariosRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration GetConfiguration()
        {
            return configuration;
        }

        public object Consultar_usuario(int id = 0, string email = "", string nome = "", string cpf = "", string aplicativo = "",
                                        string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, string uf = "", 
                                        int id_empresa = 0, string origem = "")
        {
            object result = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    cpf = Utils.SomenteNumeros(cpf);

                    if ((id == 0 && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(cpf) &&
                        string.IsNullOrWhiteSpace(aplicativo) && string.IsNullOrWhiteSpace(retaguarda) && string.IsNullOrWhiteSpace(empresa) && string.IsNullOrWhiteSpace(uf) &&
                        id_situacao == 0 && id_cidade_ap == 0 && id_empresa == 0) || (string.IsNullOrWhiteSpace(origem) || (origem == Constantes.FRONTOFFICE && id_empresa == 0)))
                    {
                        return (Utils.FormataRetorno("", new
                        {
                            Erro = true,
                            Mensagem = Constantes.MENSAGEM_CONSULTA
                        })); ;
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select u.id, u.email, u.nome, u.data_nasc, u.cpf, u.aplicativo, u.retaguarda, u.empresa, u.id_situacao, u.id_cidade_ap, " +
                                    "case   when u.id_cidade_ap is null then  ''  else c.nome||'/'||c.uf end nome_cidade_ap, s.descricao desc_situacao, u.id_empresa, e.razao_social " +
                                    "from dealwish.usuarios u " +
                                    "inner join dealwish.situacoes s on u.id_situacao = s.id " +
                                    "left join dealwish.cidades c on u.id_cidade_ap = c.id " +
                                    "left join dealwish.empresas e on u.id_empresa = e.id " +
                                    "where ( ";
                        var tem_param = false;

                        if (id != 0)
                        {
                            query = query + " u.id = :id";
                            dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(u.email) like upper(:email)";
                            dyParam.Add("email", "%" + email + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(nome))
                        {
                            query = query + (tem_param ? " and " : "") + " upper(u.nome) like upper(:nome)";
                            dyParam.Add("nome", "%" + nome + "%", DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(cpf))
                        {
                            query = query + (tem_param ? " and " : "") + " u.cpf = upper(:cpf)";
                            dyParam.Add("cpf", cpf, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(aplicativo))
                        {
                            query = query + (tem_param ? " and " : "") + " u.aplicativo = upper(:aplicativo)";
                            dyParam.Add("aplicativo", aplicativo, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(retaguarda))
                        {
                            query = query + (tem_param ? " and " : "") + " u.retaguarda = upper(:retaguarda)";
                            dyParam.Add("retaguarda", retaguarda, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(empresa))
                        {
                            query = query + (tem_param ? " and " : "") + " u.empresa = upper(:empresa)";
                            dyParam.Add("empresa", empresa, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_situacao != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " u.id_situacao = :id_situacao";
                            dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_cidade_ap != 0)
                        {
                            query = query + (tem_param ? " and " : "") + " u.id_cidade_ap = :id_cidade_ap";
                            dyParam.Add("id_cidade_ap", id_cidade_ap, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (!string.IsNullOrWhiteSpace(uf))
                        {
                            query = query + (tem_param ? " and " : "") + " c.uf = :uf";
                            dyParam.Add("uf", uf, DbType.String, ParameterDirection.Input);
                            tem_param = true;
                        }

                        if (id_empresa != 0 && origem != Constantes.FRONTOFFICE)
                        {
                            query = query + (tem_param ? " and " : "") + " u.id_empresa = :id_empresa";
                            dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                            tem_param = true;
                        }

                        query = query + " ) ";

                        if (origem == Constantes.FRONTOFFICE) //Somente usuários da Empresa
                        {
                            query = query + " and ( u.id_empresa = :id_empresa )";
                            dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        }

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

        public object Incluir_usuario(string email = "", string senha1 = "", string senha2 = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                      string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, int id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    cpf = Utils.SomenteNumeros(cpf);

                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(senha1) || string.IsNullOrWhiteSpace(senha2) ||
                        string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(aplicativo) || string.IsNullOrWhiteSpace(retaguarda) ||
                        string.IsNullOrWhiteSpace(empresa) || id_situacao == 0 || string.IsNullOrWhiteSpace(data_nasc))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "e-mail, Nome, Senha 1, Senha 2, CPF, Aplicativo, Retaguarda, Empresae Cód. Situação. Opcionalmente Cód. Cidade App ou Cód. Empresa." }));
                    }

                    if (senha1 != senha2)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Senha e confirmação de senha são diferentes." }));
                    }

                    if (!Utils.IsCpf(cpf))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "CPF inválido." }));
                    }

                    DateTime v_data_nasc = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(data_nasc))
                    {
                        try
                        {
                            v_data_nasc = DateTime.Parse(data_nasc);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de nascimento inválida." }));
                        }
                        if (v_data_nasc.AddYears(18) > DateTime.Today)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Menor de 18 anos" }));
                        }
                    }

                    email = email.ToLower();
                    if (!Utils.ValidarEmail(email))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail inválido." }));
                    }

                    if (aplicativo == "N")
                    {
                        id_cidade_ap = 0;
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        transaction = conn.BeginTransaction();

                        var query = "select count(1) email_valido from dealwish.usuarios where email = :email";
                        dyParam.Add("email", email, DbType.String, ParameterDirection.Input);
                        var resultado = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                        if (resultado.email_valido > 0)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail já cadastrado." }));
                        }

                        query = "select count(1) cpf_valido from dealwish.usuarios where cpf = :cpf";
                        dyParam.Add("cpf", cpf, DbType.String, ParameterDirection.Input);
                        resultado = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                        if (resultado.cpf_valido > 0)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "CPF já cadastrado." }));
                        }

                        query = "insert into dealwish.usuarios (email, nome, senha, cpf, aplicativo, retaguarda, empresa, id_situacao, id_cidade_ap, id_empresa, id_usuario_log, data_nasc) " +
                                     "values (:email, :nome, :senha, :cpf, :aplicativo, :retaguarda, :empresa, :id_situacao, :id_cidade_ap, :id_empresa, :id_usuario_login, :data_nasc) returning id";

                        dyParam.Add("id", null, DbType.Int64, ParameterDirection.Output);
                        dyParam.Add("senha", Utils.getHash(senha1), DbType.String, ParameterDirection.Input);
                        dyParam.Add("nome", nome, DbType.String, ParameterDirection.Input);
                        dyParam.Add("aplicativo", aplicativo, DbType.String, ParameterDirection.Input);
                        dyParam.Add("retaguarda", retaguarda, DbType.String, ParameterDirection.Input);
                        dyParam.Add("empresa", empresa, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("data_nasc", v_data_nasc, DbType.Date, ParameterDirection.Input);
                        if (id_usuario_login == 0)
                        {
                            dyParam.Add("id_usuario_login", null, DbType.Int64, ParameterDirection.Input);
                        }
                        else
                        {
                            dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                        }
                        if (id_cidade_ap == 0)
                        {
                            dyParam.Add("id_cidade_ap", null, DbType.Int32, ParameterDirection.Input);
                        }
                        else
                        {
                            dyParam.Add("id_cidade_ap", id_cidade_ap, DbType.Int32, ParameterDirection.Input);
                        }
                        if (id_empresa == 0)
                        {
                            dyParam.Add("id_empresa", null, DbType.Int32, ParameterDirection.Input);
                        }
                        else
                        {
                            dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        }

                        SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        if (aplicativo == "S")
                        {
                            query = "insert into dealwish.grp_usr (id_usuario, id_grp_permissao, id_usuario_log) " +
                                       "values (:id_usuario, (select id from dealwish.grp_permissoes where codigo = 'app'), :id_usuario_login)";
                            var dyParam1 = new DynamicParameters();
                            dyParam1.Add("id_usuario", dyParam.Get<long>("id"), DbType.Int32, ParameterDirection.Input);
                            if (id_usuario_login == 0)
                            {
                                dyParam1.Add("id_usuario_login", dyParam.Get<long>("id"), DbType.Int32, ParameterDirection.Input);
                            }
                            else
                            {
                                dyParam1.Add("id_usuario_login", id_usuario_login, DbType.Int32, ParameterDirection.Input);
                            }
                            SqlMapper.Execute(conn, query, dyParam1, transaction, null, CommandType.Text);
                        }

                        transaction.Commit();

                        result = new { ID = dyParam.Get<long>("id") };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Excluir_usuario(int id = 0, int id_usuario_login = 0, string origem = "")
        {
            object result = null;
            IDbTransaction transaction = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                    }

                    if (id == id_usuario_login && origem != Constantes.APLICATIVO)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Não é permitido excluir o próprio usuário." }));
                    }


                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        transaction = conn.BeginTransaction();

                        var query = "update dealwish.usuarios set id_usuario_log = :id_usuario_login where id = :id ";
                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "update dealwish.desejos set id_usuario = 0 where id_usuario = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "update dealwish.ofertas set id_usuario_log = 0 where id_usuario_log = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "delete from dealwish.tokens where id_usuario = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "delete from dealwish.grp_usr where id_usuario = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        query = "delete from dealwish.usuarios where id = :id ";
                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        transaction.Commit();

                        result = new { linhasafetadas };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Atualizar_usuario(int id = 0, string email = "", string nome = "", string data_nasc = "", string cpf = "", string aplicativo = "",
                                 string retaguarda = "", string empresa = "", int id_situacao = 0, int id_cidade_ap = 0, int id_empresa = 0, int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    cpf = Utils.SomenteNumeros(cpf);

                    if (id == 0 || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(aplicativo) ||
                        string.IsNullOrWhiteSpace(retaguarda) || string.IsNullOrWhiteSpace(empresa) || id_situacao == 0 || (id_cidade_ap == 0 && string.IsNullOrWhiteSpace(aplicativo)) ||
                        string.IsNullOrWhiteSpace(data_nasc) || (id_empresa == 0 && string.IsNullOrWhiteSpace(empresa)))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código, e-mail, Nome, CPF, Aplicativo, Retaguarda, Empresa, Cód. Situação. Opcionalmente Cód. Cidade App ou Cód. Empresa." }));
                    }

                    DateTime v_data_nasc = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(data_nasc))
                    {
                        try
                        {
                            v_data_nasc = DateTime.Parse(data_nasc);
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Data de nascimento inválida." }));
                        }
                        if (v_data_nasc.AddYears(18) > DateTime.Today)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Menor de 18 anos" }));
                        }
                    }

                    if (!Utils.IsCpf(cpf))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "CPF inválido." }));
                    }

                    if (!Utils.ValidarEmail(email))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail inválido." }));
                    }

                    if (aplicativo == "N")
                    {
                        id_cidade_ap = 0;
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {

                        var query = "select count(1) email_valido from dealwish.usuarios where email = :email and id <> :id";
                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("email", email, DbType.String, ParameterDirection.Input);
                        var resultado = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                        if (resultado.email_valido > 0)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "e-mail já cadastrado." }));
                        }

                        query = "select count(1) cpf_valido from dealwish.usuarios where cpf = :cpf and id <> :id";
                        dyParam.Add("cpf", cpf, DbType.String, ParameterDirection.Input);
                        resultado = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                        if (resultado.cpf_valido > 0)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "CPF já cadastrado." }));
                        }

                        query = "update dealwish.usuarios set email = :email, nome = :nome, data_nasc = :data_nasc, cpf = :cpf, aplicativo = :aplicativo, " +
                                "retaguarda = :retaguarda, empresa = :empresa, id_situacao = :id_situacao, id_cidade_ap = :id_cidade_ap, " +
                                "id_empresa = :id_empresa, id_usuario_log = :id_usuario_login where id  = :id ";

                        dyParam.Add("nome", nome, DbType.String, ParameterDirection.Input);
                        dyParam.Add("data_nasc", v_data_nasc, DbType.Date, ParameterDirection.Input);
                        dyParam.Add("aplicativo", aplicativo, DbType.String, ParameterDirection.Input);
                        dyParam.Add("retaguarda", retaguarda, DbType.String, ParameterDirection.Input);
                        dyParam.Add("empresa", empresa, DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_situacao", id_situacao, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                        if (id_cidade_ap != 0)
                        {
                            dyParam.Add("id_cidade_ap", id_cidade_ap, DbType.Int32, ParameterDirection.Input);
                        }
                        else
                        {
                            dyParam.Add("id_cidade_ap", null, DbType.Int32, ParameterDirection.Input);
                        }
                        if (id_empresa != 0)
                        {
                            dyParam.Add("id_empresa", id_empresa, DbType.Int32, ParameterDirection.Input);
                        }
                        else
                        {
                            dyParam.Add("id_empresa", null, DbType.Int32, ParameterDirection.Input);
                        }

                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);
                        result = new { linhasafetadas };
                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Reiniciar_senha(string email = "", string cpf ="", int id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "e-mail." }));
                    }

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string nome = "";
                    long id_usuario = 0;
                    var query = "";

                    if (conn.State == ConnectionState.Open)
                    {
                        if (string.IsNullOrWhiteSpace(cpf))
                        {
                            cpf = "0";
                        }
                        else
                        {
                            cpf = Utils.SomenteNumeros(cpf);
                        }
                        

                        query = "select id, email, nome from dealwish.usuarios where email = lower(:email) and (:cpf = '0' or cpf = :cpf)";

                        var dyParamL = new DynamicParameters();
                        dyParamL.Add("email", email, DbType.String, ParameterDirection.Input);
                        dyParamL.Add("cpf", cpf, DbType.String, ParameterDirection.Input);

                        try
                        {
                            var usuario = SqlMapper.QuerySingle(conn, query, param: dyParamL, commandType: CommandType.Text);
                            result = usuario;
                            email = usuario.email;
                            nome = usuario.nome;
                            id_usuario = usuario.id;
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário não localizado." }));
                        }

                    }

                    transaction = conn.BeginTransaction();

                    string nova_senha = Utils.GeneratePassword(10);

                    query = "update dealwish.usuarios set senha = :senha, id_usuario_log = :id_usuario_login where id = :id_usuario ";

                    var dyParam = new DynamicParameters();
                    dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);
                    dyParam.Add("senha", Utils.getHash(nova_senha), DbType.String, ParameterDirection.Input);
                    if (id_usuario_login == 0)
                    {
                        dyParam.Add("id_usuario_login", null, DbType.Int64, ParameterDirection.Input);
                    }
                    else
                    {
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                    }

                    var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                    query = "delete from dealwish.tokens where id_usuario = :id_usuario";
                    linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                    Utils.EnviarEmail(email, "Nova senha Dealwish", nome + ", sua nova senha é: " + nova_senha, configuration);

                    transaction.Commit();

                    result = new { linhasafetadas, nova_senha };

                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Trocar_senha(string email = "", string senha_atual = "", string senha_nova = "", string senha_nova_conf = "", int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha_atual) || string.IsNullOrWhiteSpace(senha_nova) || string.IsNullOrWhiteSpace(senha_nova_conf))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "e-mail, Senha Atual, Senha Nova, Conf. Nova Senha." }));
                    }

                    if (senha_nova != senha_nova_conf)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Senha e confirmação da senha não conferem." }));
                    }

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    var query = "";
                    var senha_valida = "";

                    if (conn.State == ConnectionState.Open)
                    {

                        query = "select senha from dealwish.usuarios where email = lower(:email) ";

                        var dyParam1 = new DynamicParameters();
                        dyParam1.Add("email", email, DbType.String, ParameterDirection.Input);

                        try
                        {
                            var usuario = SqlMapper.QuerySingle(conn, query, param: dyParam1, commandType: CommandType.Text);
                            senha_valida = usuario.senha;
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário não localizado ou senha inválida." }));
                        }

                        if (senha_valida != Utils.getHash(senha_atual))
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário não localizado ou senha inválida." }));
                        }

                        query = "update dealwish.usuarios set senha = :senha, id_usuario_log = :id_usuario_login where email = lower(:email) ";

                        var dyParam = new DynamicParameters();
                        dyParam.Add("email", email, DbType.String, ParameterDirection.Input);
                        dyParam.Add("senha", Utils.getHash(senha_nova), DbType.String, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);
                        result = new { linhasafetadas };

                    }

                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Login_usuario(string email = "", string senha = "", string origem = "", string token_app = "")
        {
            string senha_valida = "";
            string nome = "";
            long id = 0;
            long id_cidade_ap = 0;
            long id_empresa = 0;
            long id_situacao = 0;
            decimal id_situacao_contrato = 0;
            DateTime data_nasc = DateTime.Today;
            string cpf = "";
            string grp_permissoes = "";
            string cidade = "";
            string uf = "";
            dynamic num_falhas_login = 0;
            bool login_app = !string.IsNullOrWhiteSpace(token_app);
            IDbTransaction transaction = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                object result;
                try
                {
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário e Senha inválidos." }));
                    }

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select u.id, u.senha, u.id_situacao, u.nome, u.data_nasc, u.cpf, u.id_cidade_ap, u.id_empresa, coalesce(t.num_falhas_login,t.num_falhas_login,0) num_falhas_login, c.nome cidade, c.uf, " +
                                    "(select string_agg(codigo, ',' order by descricao) from dealwish.grp_permissoes gp, dealwish.grp_usr gu where gu.id_grp_permissao = gp.id and gu.id_usuario = u.id) grp_permissoes, " +
                                    "(select id_situacao from dealwish.empresas e, dealwish.contratos ct where e.id = ct.id_empresa and ct.id_situacao = :ativo and e.id = u.id_empresa) id_situacao_contrato " +
                                    "from dealwish.usuarios u " +
                                    "left join dealwish.cidades c on (u.id_cidade_ap = c.id) " +
                                    "left join dealwish.tokens t on (u.id = t.id_usuario) " +
                                    "where u.email = lower(:email)";

                        var dyParam = new DynamicParameters();
                        dyParam.Add("email", email, DbType.String, ParameterDirection.Input);
                        dyParam.Add("ativo", Constantes.ATIVO, DbType.Int32, ParameterDirection.Input);

                        try
                        {
                            var usuario = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
                            senha_valida = usuario.senha;
                            nome = usuario.nome;
                            id_situacao = usuario.id_situacao;
                            id = usuario.id;
                            id_cidade_ap = usuario.id_cidade_ap == null ? 0 : usuario.id_cidade_ap;
                            cidade = usuario.cidade;
                            uf = usuario.uf;
                            data_nasc = usuario.data_nasc;
                            cpf = usuario.cpf;
                            id_empresa = usuario.id_empresa == null ? 0 : usuario.id_empresa;
                            id_situacao_contrato = usuario.id_situacao_contrato == null ? 0 : usuario.id_situacao_contrato;
                            grp_permissoes = usuario.grp_permissoes;
                            num_falhas_login = usuario.num_falhas_login;
                        }
                        catch (Exception)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário e Senha inválidos." }));
                        }

                    }

                    if (!grp_permissoes.Contains("ti"))
                    {
                        if ((origem == Constantes.BACKOFFICE && !grp_permissoes.Contains("bk")) ||
                            (origem == Constantes.FRONTOFFICE && !grp_permissoes.Contains("ft")))
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário sem permissão de acesso" }));
                        }
                    }

                    if (num_falhas_login >= int.Parse(ConfigRepository.Consultar_valor_config("num_max_tentativas_login")))
                    {
                        if (login_app)
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário bloqueado, utilize 'esqueci minha senha'" }));
                        }
                        else
                        {
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário bloqueado, solicite a reinicialização da senha." }));
                        }

                    }

                    if (id_situacao != Constantes.ATIVO)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário não está ativo. Contate o suporte." }));
                    }

                    if (id_empresa != 0 && id_situacao_contrato != Constantes.ATIVO && !login_app)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Contrato da Empresa Bloqueado ou Encerrado." }));
                    }

                    try
                    {
                        transaction = conn.BeginTransaction();

                        if (senha_valida == Utils.getHash(senha))
                        {
                            string token = Utils.getHash(senha_valida + DateTime.Now.ToString() + DateTime.Now.ToString());

                            var tokenHandler = new JwtSecurityTokenHandler();
                            var key = System.Text.Encoding.ASCII.GetBytes(configuration.GetSection("Chaves").GetSection("ChaveJWT").Value);

                            ClaimsIdentity _clainIdentity = new ClaimsIdentity();
                            _clainIdentity.AddClaim(new Claim(ClaimTypes.Name, id.ToString()));
                            foreach (string roles in grp_permissoes.Split(',')) { 
                                _clainIdentity.AddClaim(new Claim(ClaimTypes.Role, roles)); 
                            };

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = _clainIdentity,
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                            };
                            var tokenJwt = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

                            var query = "update dealwish.tokens set token = :token, val_token = current_timestamp + interval '30 min' , token_app = :token_app, num_falhas_login = 0 " +
                                        "where id_usuario = :id_usuario  ";

                            var dyParam = new DynamicParameters();
                            dyParam.Add("id_usuario", id, DbType.Int64, ParameterDirection.Input);
                            dyParam.Add("token", token, DbType.String, ParameterDirection.Input);
                            if (!string.IsNullOrWhiteSpace(token_app))
                            {
                                dyParam.Add("token_app", token_app, DbType.String, ParameterDirection.Input);
                            }
                            else
                            {
                                dyParam.Add("token_app", null, DbType.String, ParameterDirection.Input);
                            }

                            var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                            if (linhasafetadas == 0)
                            {
                                query = "insert into dealwish.tokens (id_usuario, token, val_token, token_app, num_falhas_login) values " +
                                        "(:id_usuario, :token,  current_timestamp + interval '30 min' , :token_app, 0)";
                                SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);
                            }

                            transaction.Commit();

                            result = new { id, token, tokenJwt, nome, id_situacao, data_nasc, cpf, id_cidade_ap, cidade, uf, id_empresa, grp_permissoes };
                        }
                        else
                        {
                            var query = "update dealwish.tokens set num_falhas_login = num_falhas_login + 1 " +
                                        "where id_usuario = :id_Usuario";

                            var dyParam = new DynamicParameters();
                            dyParam.Add("id_usuario", id, DbType.Int64, ParameterDirection.Input);
                            var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                            if (linhasafetadas == 0)
                            {
                                query = "insert into dealwish.tokens (id_usuario, num_falhas_login) values (:id_usuario, 1)";
                                SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);
                            }

                            transaction.Commit();

                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Usuário e Senha inválidos." }));
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                    }

                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Incluir_grp_permissao_usuario(int id_usuario = 0, int id_grp_permissao = 0,  int id_usuario_login = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id_usuario == 0 || id_grp_permissao == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Cód. Usuário e Cód. Grupo Permissão." }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "insert into dealwish.grp_usr (id_usuario, id_grp_permissao, id_usuario_log) values (:id_usuario, :id_grp_permissao, :id_usuario_login) returning id";

                        dyParam.Add("id", null, DbType.Int64, ParameterDirection.Output);
                        dyParam.Add("id_grp_permissao", id_grp_permissao, DbType.Int32, ParameterDirection.Input);
                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);

                        SqlMapper.Execute(conn, query, dyParam, null, null, CommandType.Text);
                        result = new { ID = dyParam.Get<long>("id") };
                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Excluir_grp_permissao_usuario(int id = 0, int id_usuario_login = 0)
        {
            object result = null;
            IDbTransaction transaction = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    if (id == 0)
                    {
                        return (Utils.FormataRetorno("", new { Erro = true, Mensagem = Constantes.MENSAGEM_INCLUSAO_ATUALIZACAO + "Código." }));
                    }

                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        transaction = conn.BeginTransaction();

                        var query = "update dealwish.grp_usr set id_usuario_log = :id_usuario_login where id = :id and " +
                                    "id_usuario not in (select gu.id_usuario from dealwish.grp_permissoes gp, dealwish.grp_usr gu where gp.codigo in ('tin','bka','fta') " +
                                                       "and gu.id_grp_permissao = gp.id and gu.id_usuario = :id_usuario_login and gu.id = :id)";
                        dyParam.Add("id", id, DbType.Int64, ParameterDirection.Input);
                        dyParam.Add("id_usuario_login", id_usuario_login, DbType.Int64, ParameterDirection.Input);
                        var linhasafetadas = SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        if (linhasafetadas == 0)
                        {
                            transaction.Rollback();
                            return (Utils.FormataRetorno("", new { Erro = true, Mensagem = "Não é permitido excluir a própria permissão de administrador." }));
                        }

                        query = "delete from dealwish.grp_usr where id = :id ";

                        linhasafetadas = linhasafetadas + SqlMapper.Execute(conn, query, dyParam, transaction, null, CommandType.Text);

                        transaction.Commit();

                        result = new { linhasafetadas };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));
            }
        }

        public object Consultar_permissoes_usuario(int id_usuario = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id_usuario == 0)
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
                        var query = "select distinct pg.id_permissao, p.descricao descricaco_permissao, p.codigo codigo_permissao " +
                                    "from dealwish.grp_usr g, dealwish.perm_grp pg, dealwish.permissoes p " +
                                    "where g.id_grp_permissao = pg.id_grp_permissao and p.id = pg.id_permissao and g.id_usuario = :id_usuario";

                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);

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

        public object Consultar_grp_permissoes_usuario(int id_usuario = 0)
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (id_usuario == 0)
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
                        var query = "select g.id, g.id_grp_permissao, gp.descricao descricao_grp_permissao " +
                                     "from dealwish.grp_usr g, dealwish.grp_permissoes gp " +
                                     "where g.id_grp_permissao = gp.id " +
                                     "and g.id_usuario = :id_usuario";

                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);

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

        public object Consultar_termo_servico()
        {
            object result = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select ts.texto from dealwish.termo_servico ts " +
                                    "where  ts.corrente = 'S'";

                        result = SqlMapper.Query(conn, query, commandType: CommandType.Text);

                    }
                }
                catch (Exception ex)
                {
                    return (Utils.FormataRetorno("", new { Erro = true, Mensagem = ex.Message }));
                }

                return (Utils.FormataRetorno(result, new { Erro = false, Mensagem = "" }));

            }
        }


        public static string Consultar_lista_perm_usuario(int id_usuario = 0)
        {
            dynamic result = "";

            using (NpgsqlConnection conn = new NpgsqlConnection(Constantes.string_conexao))
            {

                try
                {
                    var dyParam = new DynamicParameters();

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "select string_agg(codigo, ';' order by descricao) permissoes " +
                                    "from dealwish.grp_permissoes gp, dealwish.grp_usr gu " +
                                    "where gu.id_grp_permissao = gp.id and gu.id_usuario = :id_usuario";

                        dyParam.Add("id_usuario", id_usuario, DbType.Int64, ParameterDirection.Input);

                        result = SqlMapper.QuerySingle(conn, query, param: dyParam, commandType: CommandType.Text);
        
                    }
                }
                catch (Exception)
                {
                    return "";
                }

                return result.permissoes;
            }
        }
    }

}