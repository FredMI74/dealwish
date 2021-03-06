class RetornoIndicadores
{
  List<Indicador> indicadoresList = List();
  bool erro;
  String mensagem = '';

  RetornoIndicadores(Map dados)
  {
    erro = dados['resultado']['erro'];

    if(!erro)
    {
      for (Map value in dados['conteudo']) {
        Indicador _indicador = Indicador(value);
        indicadoresList.add(_indicador);
      }

    }

    mensagem = dados['resultado']['mensagem'];
  }

}

class Indicador
{
  String indicador;
  int valor;

  Indicador(Map _map){
    indicador = _map['indicador'];
    valor = _map['valor'];
  }
}
