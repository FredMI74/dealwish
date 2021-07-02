class RetornoCidade {
  List<Cidade> cidadeList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoCidade(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      for (Map value in dados['conteudo']) {
        Cidade _cidade = Cidade(value);
        cidadeList.add(_cidade);
      }
    }

    mensagem = dados['resultado']['mensagem'];
  }
}

class Cidade {
  int id;
  String nome;
  String uf;

  Cidade(Map _map) {
    id = _map['id'];
    nome = _map['nome'];
    uf = _map['uf'];
  }
}
