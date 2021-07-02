class RetornoGrpProduto
{
  List<GrpProduto> grpProdutoList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoGrpProduto(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      for (Map value in dados['conteudo']) {
        GrpProduto _grpProduto = GrpProduto(value);
        grpProdutoList.add(_grpProduto);
      }

    }

    mensagem = dados['resultado']['mensagem'];
  }

}

class GrpProduto
{
  int id;
  String descricao;
  String tipos_produtos;
  var icone;

  GrpProduto(Map _map){
    id = _map['id'];
    descricao = _map['descricao'];
    tipos_produtos = _map['tipos_produtos'];
    final UriData data = Uri.parse(_map['icone']).data;
    icone =  data.contentAsBytes();
  }
}
