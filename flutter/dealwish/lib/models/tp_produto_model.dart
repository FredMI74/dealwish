class RetornoTpProduto {
  List<TpProduto> tpProdutoList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoTpProduto(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      for (Map value in dados['conteudo']) {
        TpProduto _tpProduto = TpProduto(value);
        tpProdutoList.add(_tpProduto);
      }
    }

    mensagem = dados['resultado']['mensagem'];
  }
}

class TpProduto {
  int id;
  String descricao;
  String preenchimento;
  var icone;

  TpProduto(Map _map) {
    id = _map['id'];
    descricao = _map['descricao'];
    preenchimento = _map['preenchimento'];
    final UriData data = Uri.parse(_map['icone']).data;
    icone = data.contentAsBytes();
  }
}
