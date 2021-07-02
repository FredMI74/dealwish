import 'package:intl/intl.dart';
import 'package:intl/date_symbol_data_file.dart';

class RetornoOferta {
  List<Oferta> ofertaList = List.empty(growable: true);
  bool erro;
  String mensagem = '';

  RetornoOferta(Map dados) {
    erro = dados['resultado']['erro'];

    if (!erro) {
      for (Map value in dados['conteudo']) {
        Oferta _oferta = Oferta(value);
        ofertaList.add(_oferta);
      }
    }

    mensagem = dados['resultado']['mensagem'];
  }
}

class Oferta {

  int id;
  int id_desejo;
  dynamic id_empresa;
  String fantasia;
  var logo;
  DateTime _validade;
  String validade;
  double _valor;
  String valor;
  String url;
  String descricao;
  String lida;
  String like_unlike;
  String destaque;

  Oferta(Map _map) {

    NumberFormat _formatReal = NumberFormat.currency(locale: 'pt_BR',name: 'BRL', symbol: 'R\$', decimalDigits: 2);
    var formatter = new DateFormat('dd/MM/yyyy');

    id = _map['id'];
    id_desejo = _map['id_desejo'];
    id_empresa = _map['id_empresa'];
    fantasia = _map['fantasia'];
    final UriData data = Uri.parse(_map['logo']).data;
    logo  =  data.contentAsBytes();
    _validade = DateTime.parse(_map['validade']);
    validade =  formatter.format(_validade);
    _valor = _map['valor'];
    valor = _formatReal.format(_valor);
    url = _map['url'];
    descricao = _map['descricao'];
    lida = _map['lida'];
    like_unlike = _map['like_unlike'];
    destaque = _map['destaque'];
  }
  Oferta.empty();
}
