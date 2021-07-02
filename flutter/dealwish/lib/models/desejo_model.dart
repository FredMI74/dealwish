class RetornoDesejo {
  List<Desejo> desejoList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoDesejo(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      for (Map value in dados['conteudo']) {
        Desejo _desejo = Desejo(value);
        desejoList.add(_desejo);
      }
    }

    mensagem = dados['resultado']['mensagem'];
  }
}

class Desejo {
  int id;
  String descricao;
  int id_usuario;
  String nome_usuario;
  String email_usuario;
  var icone_tp_produto;
  int id_tipo_produto;
  String desc_tp_produto;
  int id_situacao;
  String desc_situacao;
  int qtd_ofertas;

  Desejo(Map _map) {
    id = _map['id'];
    descricao = _map['descricao'];
    id_usuario = _map['id_usuario'];
    nome_usuario = _map['nome_usuario'];
    email_usuario = _map['email_usuario'];
    final UriData data = Uri.parse(_map['icone_tp_produto']).data;
    icone_tp_produto =  data.contentAsBytes();
    id_tipo_produto = _map['id_tipo_produto'];
    desc_tp_produto = _map['desc_tp_produto'];
    id_situacao = _map['id_situacao'];
    desc_situacao = _map['desc_situacao'];
    qtd_ofertas = _map['qtd_ofertas'];
  }
  Desejo.empty();
}
