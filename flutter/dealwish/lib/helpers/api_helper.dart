import 'dart:async';
import 'dart:convert';
import 'package:dealwish/main.dart';
import 'package:dealwish/models/cidade_model.dart';
import 'package:dealwish/models/empresa_model.dart';
import 'package:dealwish/models/oferta_model.dart';
import 'package:dealwish/models/termo_servico_model.dart';
import 'package:dealwish/models/ultima_atualizacao_produtos.dart';
import 'package:http/http.dart' as http;
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:dealwish/models/usuario_model.dart';
import 'package:dealwish/models/grp_produto_model.dart';
import 'package:dealwish/models/tp_produto_model.dart';
import 'package:dealwish/models/desejo_model.dart';
import 'package:localstorage/localstorage.dart';
import 'dart:io';

const URLDW = '';
const TOKEN_ANONIMO = '';
const TEMPO_VIDA_TOKEN = 25;
const ATIVO = "1";
const APLICATIVO = "A";
const SIM = "S";
const NAO = "N";
const TIMEOUT = 20;

class ApiHelper {
  // Singleton
  static final ApiHelper _instance = ApiHelper.internal();
  LocalStorage storage;

  factory ApiHelper() => _instance;

  ApiHelper.internal(){
    storage = LocalStorage('delawish.json');
  }

  DateTime _ultima_chamada;

  Future<Usuario> loginUsuario(String email, String senha, String token_app) async {

    _ultima_chamada = DateTime.now();
    Codec<String, String> stringToBase64 = utf8.fuse(base64);

    var _login = new Map();
    _login['dados'] = stringToBase64.encode(email) + ';' + stringToBase64.encode(senha);
    _login['token_app'] = token_app;
    _login['token'] = TOKEN_ANONIMO;

    final response =
        await http.post(URLDW + '/api/login_usuario', body: _login).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoLogin _dados = RetornoLogin(json.decode(response.body));

      usuario.token = _dados.token;
      usuario.nome = _dados.nome;
      usuario.id = _dados.id;
      usuario.email = email;
      usuario.senha1 = senha;
      usuario.id = _dados.id;
      usuario.id_cidade_ap = _dados.id_cidade_ap;
      usuario.cidade = _dados.cidade;
      usuario.uf = _dados.uf;
      usuario.data_nasc = _dados.data_nasc;
      usuario.cpf = _dados.cpf;
      usuario.token_app = token_app;
      usuario.token_jwt = _dados.token_jwt;
      usuario.mensagem = _dados.mensagem;

      return usuario;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoInclusao> reinicarSenha(String email, String cpf) async {

    _ultima_chamada = DateTime.now();

    var _login = new Map();
    _login['email'] = email;
    _login['cpf'] = cpf;
    _login['token'] = TOKEN_ANONIMO;

    final response =
        await http.post(URLDW + '/api/reiniciar_senha', body: _login).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoInclusao _dados = RetornoInclusao(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarGrpPrdutos() async {

    await renovartoken();

    var _grpProdutos = new Map();

    _grpProdutos['token'] = usuario.token;

      final response = await http.post(
          URLDW + '/api/consultar_ultima_atualizacao_produtos',
          headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
          body: _grpProdutos).timeout(
          Duration(seconds: TIMEOUT),
          onTimeout: () {
            throw Exception('Timeout.');
          });

      if (response.statusCode == 200) {
        UltimaAtualizacaoProdutos _dados = UltimaAtualizacaoProdutos(json.decode(response.body));

        if (_dados.erro) {
          throw Exception('Erro ao buscar os dados.');
        } else {
          if (storage.getItem('ultima_atualizacao_produtos') == null ||
              storage.getItem('ultima_atualizacao_produtos') != _dados.ultima_atualizacao_produtos) {
            await storage.clear();
            await storage.setItem('ultima_atualizacao_produtos', _dados.ultima_atualizacao_produtos);
          }
        }
      } else {
        throw Exception('Erro crítico.');
      }


    if(storage.getItem('grpproduto') == null)
    {
      final response = await http.post(
          URLDW + '/api/consultar_todos_grp_produto',
          headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
          body: _grpProdutos).timeout(
          Duration(seconds: TIMEOUT),
          onTimeout: () {
            throw Exception('Timeout.');
          });

      if (response.statusCode == 200) {
        RetornoGrpProduto _dados = RetornoGrpProduto(json.decode(response.body));
        if (_dados.erro) {
          throw Exception('Erro ao buscar os dados.');
        } else {
          await storage.setItem('grpproduto', response.body);
          return _dados.grpProdutoList;
        }
      } else {
        throw Exception('Erro crítico.');
      }
    } else {
      RetornoGrpProduto _dados = RetornoGrpProduto(json.decode(storage.getItem('grpproduto')));
      return _dados.grpProdutoList;
    }
  }

  Future<List> consultarTpPrdutos(int id_grp_prod) async {

    var _tpProdutos = new Map();
    _tpProdutos['id_grp_prod'] = id_grp_prod.toString();
    _tpProdutos['id_situacao'] = ATIVO.toString();
    _tpProdutos['token'] = usuario.token;

    String _keyTpProduto = 'tpproduto' + id_grp_prod.toString();

    if (storage.getItem(_keyTpProduto) == null) {

      await renovartoken();

      final response =
      await http.post(URLDW + '/api/consultar_tp_produto',
          headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
          body: _tpProdutos).timeout(
          Duration(seconds: TIMEOUT),
          onTimeout: () {
            throw Exception('Timeout.');
          });

      if (response.statusCode == 200) {
        RetornoTpProduto _dados = RetornoTpProduto(json.decode(response.body));
        if (_dados.erro) {
          throw Exception('Erro ao buscar os dados.');
        } else {
          await storage.setItem(_keyTpProduto, response.body);
          return _dados.tpProdutoList;
        }
      } else {
        throw Exception('Erro crítico.');
      }
    } else {
      RetornoTpProduto _dados = RetornoTpProduto(json.decode(storage.getItem(_keyTpProduto)));
      return _dados.tpProdutoList;
    }
  }

  Future<RetornoInclusao> incluirDesejo(Desejo desejo) async {

    await renovartoken();

    var _desejo = new Map();
    _desejo['descricao'] = desejo.descricao;
    _desejo['id_usuario'] = usuario.id.toString();
    _desejo['id_tipo_produto'] = desejo.id_tipo_produto.toString();
    _desejo['id_situacao'] = ATIVO.toString();
    _desejo['token'] = usuario.token;

    final response =
        await http.post(URLDW + '/api/incluir_desejo',
            headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
            body: _desejo).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoInclusao _dados = RetornoInclusao(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarDesejos(int id_situacao) async {

    await renovartoken();

    var _desejos = new Map();
    _desejos['id_usuario'] = usuario.id.toString();
    _desejos['id_situacao'] = id_situacao.toString();
    _desejos['token'] = usuario.token;

    final response =
        await http.post(URLDW + '/api/consultar_desejo',
            headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
            body: _desejos).timeout(
    Duration(seconds: TIMEOUT),
    onTimeout: () {
      throw Exception('Timeout.');
    });

    if (response.statusCode == 200) {
      RetornoDesejo _dados = RetornoDesejo(json.decode(response.body));

      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.desejoList;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarOfertas(int id_desejo) async {

    await renovartoken();

    var _ofertas = new Map();
    _ofertas['id_desejo'] = id_desejo.toString();
    _ofertas['origem'] = APLICATIVO; //Aplicativo
    _ofertas['token'] = usuario.token;

    final response =
        await http.post(URLDW + '/api/consultar_oferta',
            headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
            body: _ofertas).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoOferta _dados = RetornoOferta(json.decode(response.body));
      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.ofertaList;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoProcessamento> atualizarsituacaoDesejo(Desejo desejo) async {

    await renovartoken();

    var _desejo = new Map();
    _desejo['id'] = desejo.id.toString();
    _desejo['id_situacao'] = desejo.id_situacao.toString();
    _desejo['token'] = usuario.token;

    final response = await http.post(URLDW + '/api/atualizar_situacao_desejo',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _desejo).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados =
          RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarCidades({String uf = '', int id = 0, String nome = ''}) async {

    var _cidades = new Map();
    if (uf != '') {
      _cidades['uf'] = uf;
    }
    if (id != 0) {
      _cidades['id'] = id.toString();
    }
    if (nome != '') {
      _cidades['nome_exato'] = nome;
    }
    _cidades['token'] = TOKEN_ANONIMO;

    final response =
    await http.post(URLDW + '/api/consultar_cidade', body: _cidades).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoCidade _dados = RetornoCidade(json.decode(response.body));
      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.cidadeList;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoInclusao> incluirUsuario(Usuario usuario) async {

    var _usuario = new Map();
    _usuario['email'] = usuario.email;
    _usuario['senha1'] = usuario.senha1;
    _usuario['senha2'] = usuario.senha2;
    _usuario['nome'] = usuario.nome;
    _usuario['data_nasc'] = usuario.data_nasc.toString();
    _usuario['cpf'] = usuario.cpf;
    _usuario['aplicativo'] = SIM;
    _usuario['retaguarda'] = NAO;
    _usuario['empresa'] = NAO;
    _usuario['id_situacao'] = ATIVO; //Ativo
    _usuario['id_cidade_ap'] = usuario.id_cidade_ap.toString();
    _usuario['token'] = TOKEN_ANONIMO;

    final response =
        await http.post(URLDW + '/api/incluir_usuario', body: _usuario).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoInclusao _dados = RetornoInclusao(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoProcessamento> atualizarUsuario(Usuario usuario) async {

    await renovartoken();

    var _usuario = new Map();
    _usuario['id'] = usuario.id.toString();
    _usuario['email'] = usuario.email;
    _usuario['nome'] = usuario.nome;
    _usuario['data_nasc'] = usuario.data_nasc.toString();
    _usuario['cpf'] = usuario.cpf;
    _usuario['aplicativo'] = SIM;
    _usuario['retaguarda'] = NAO;
    _usuario['empresa'] = NAO;
    _usuario['id_situacao'] = ATIVO; //Ativo
    _usuario['id_cidade_ap'] = usuario.id_cidade_ap.toString();
    _usuario['token'] = usuario.token;

    final response =
    await http.post(URLDW + '/api/atualizar_usuario',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _usuario).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados = RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoProcessamento> excluirUsuario(int id) async {

    await renovartoken();

    var _usuario = new Map();
    _usuario['id'] = id.toString();
    _usuario['origem'] = APLICATIVO;
    _usuario['token'] = usuario.token;

    final response =
    await http.post(URLDW + '/api/excluir_usuario',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _usuario).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados = RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<String> consultarTermoServico() async {

    var _termo = new Map();
    _termo['token'] = TOKEN_ANONIMO;

    final response =
        await http.post(URLDW + '/api/consultar_termo_servico', body: _termo).timeout(
            Duration(seconds: TIMEOUT),
            onTimeout: () {
              throw Exception('Timeout.');
            });

    if (response.statusCode == 200) {
      RetornoTermoServico _dados =
          RetornoTermoServico(json.decode(response.body));
      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.texto;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoProcessamento> trocarSenha(String email, String senha_atual, String senha_nova, String senha_nova_conf) async {

    await renovartoken();

    var _trocar = new Map();
    _trocar['email'] = email;
    _trocar['senha_atual'] = senha_atual;
    _trocar['senha_nova'] = senha_nova;
    _trocar['senha_nova_conf'] = senha_nova_conf;
    _trocar['token'] = usuario.token;

    final response = await http.post(URLDW + '/api/trocar_senha',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _trocar).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados =
      RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future renovartoken() async {

    if  (usuario.token.isNotEmpty)
    {
      Duration difference = DateTime.now().difference(_ultima_chamada);
      if (difference.inMinutes >= TEMPO_VIDA_TOKEN)
      {
        await loginUsuario(usuario.email, usuario.senha1, usuario.token_app);
      }
    }
    _ultima_chamada = DateTime.now();
  }

  Future<RetornoProcessamento> atualizarlidaOferta(int id_oferta) async {

    await renovartoken();

    var _oferta = new Map();
    _oferta['id'] = id_oferta.toString();
    _oferta['token'] = usuario.token;

    final response = await http.post(URLDW + '/api/atualizar_lida_oferta',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _oferta).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados =
      RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<RetornoProcessamento> atualizarlikeunlikeOferta(int id_oferta, String like_unlike) async {

    await renovartoken();

    var _oferta = new Map();
    _oferta['id'] = id_oferta.toString();
    _oferta['like_unlike'] = like_unlike;
    _oferta['token'] = usuario.token;

    final response = await http.post(URLDW + '/api/atualizar_like_unlike_oferta',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _oferta).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoProcessamento _dados =
      RetornoProcessamento(json.decode(response.body));

      return _dados;
    } else {
      throw Exception('Erro crítico.');
    }
  }

  Future<List> consultarEmpresas(int id_empresa) async {

    await renovartoken();

    var _empresas = new Map();
    _empresas['id'] = id_empresa.toString();
    _empresas['token'] = usuario.token;

    final response =
    await http.post(URLDW + '/api/consultar_empresa',
        headers: {HttpHeaders.authorizationHeader: "Bearer " + usuario.token_jwt},
        body: _empresas).timeout(
        Duration(seconds: TIMEOUT),
        onTimeout: () {
          throw Exception('Timeout.');
        });

    if (response.statusCode == 200) {
      RetornoEmpresa _dados = RetornoEmpresa(json.decode(response.body));
      if (_dados.erro) {
        throw Exception('Erro ao buscar os dados.');
      } else {
        return _dados.empresaList;
      }
    } else {
      throw Exception('Erro crítico.');
    }
  }


}
