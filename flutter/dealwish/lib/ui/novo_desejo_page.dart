import 'package:dealwish/helpers/api_helper.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:dealwish/models/desejo_model.dart';
import 'package:flutter/services.dart';
import 'package:xml/xml.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class Atributo {
  String descricao;
  String valor;
  List<String> valores;

  Atributo(this.descricao);
}

class NovoDesejo extends StatefulWidget {
  final String desc_tp_produto;
  final String preenchimento;
  final int id_tp_produto;

  NovoDesejo({this.id_tp_produto, this.desc_tp_produto, this.preenchimento});

  @override
  _NovoDesejoState createState() => _NovoDesejoState();
}

class _NovoDesejoState extends State<NovoDesejo> {
  ApiHelper api_helper = ApiHelper();
  final _storage = new FlutterSecureStorage();

  bool isLoading = false;

  final _scaffoldKey = GlobalKey<ScaffoldState>();

  final _descricaoController = TextEditingController();

  List<Atributo> _atributos = [];

  @override
  void initState() {
    super.initState();
    _popularAtributos(widget.preenchimento);
  }

  @override
  Widget build(BuildContext context) {
    if (_atributos.length > 1) {
      Future.delayed(Duration.zero, () => _instrucaoPreenchimento(context));
    }
    return Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          centerTitle: true,
          title: Text("Desejo: " + widget.desc_tp_produto,
              style: TextStyle(color: Colors.white)),
        ),
        floatingActionButton: Opacity(
            opacity: 0.6,
            child: FloatingActionButton(
              onPressed: () {
                if (!isLoading) {
                  if (_descricao() != '') {
                    Desejo _desejo = Desejo.empty();
                    _desejo.id_tipo_produto = widget.id_tp_produto;
                    _desejo.descricao = _descricao();
                    _incluirDesejo(_desejo);
                  } else {
                    _avisoPreencher();
                  }
                }
              },
              child: Icon(Icons.save),
              backgroundColor: Color.fromARGB(255, 255, 127, 0),
            )),
        body: Stack(
          children: <Widget>[
            _descDesejo(),
            isLoading
                ? Container(
                    color: Colors.black.withOpacity(0.5),
                    child: Center(
                      child: CircularProgressIndicator(
                        valueColor: new AlwaysStoppedAnimation<Color>(
                            Color.fromARGB(255, 255, 127, 0)),
                      ),
                    ),
                  )
                : Container()
          ],
        ));
  }

  void _onFail() {
    setState(() {
      isLoading = false;
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text("Falha ao incluir o Desejo!"),
        backgroundColor: Colors.redAccent,
        duration: Duration(seconds: 2),
      ));
    });
  }

  void _avisoPreencher() {
    setState(() {
      isLoading = false;
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text("Preencha alguma informação."),
        backgroundColor: Colors.amber,
        duration: Duration(seconds: 2),
      ));
    });
  }

  String _descricao() {
    String _descricaoFinal = '';

    for (Atributo _atributo in _atributos) {
      if (_atributo.valor != _atributo.descricao &&
          _atributo.descricao != 'TextField')
        _descricaoFinal = _descricaoFinal + ' ' + _atributo.valor;
    }

    if (_descricaoController.text.isNotEmpty) {
      _descricaoFinal = _descricaoFinal + ' ' + _descricaoController.text;
    }
    return _descricaoFinal.trim();
  }

  void _incluirDesejo(Desejo desejo) async {
    setState(() {
      SystemChannels.textInput.invokeMethod('TextInput.hide');
      isLoading = true;
    });

    try {
      RetornoInclusao retornoInclusao =
          await api_helper.incluirDesejo(desejo);

      if (retornoInclusao.id > 0) {
        isLoading = false;
        Navigator.pushNamedAndRemoveUntil(
            context, '/desejos', ModalRoute.withName('/'));
      } else {
        _onFail();
      }
    } catch (e) {
      _onFail();
    }
  }

  _popularAtributos(String preenchimento) {
    XmlDocument document;
    bool _xmlvalido = true;
    try {
      document = XmlDocument.parse(preenchimento);
    } catch (e) {
      _xmlvalido = false;
    }
    if (_xmlvalido) {
      var _itensXML = document.findAllElements('atributo');

      for (var _item in _itensXML) {
        Atributo _novoAtributo = Atributo(_item.attributes[0].value);
        _novoAtributo.valores = [];
        _novoAtributo.valores.add(_item.attributes[0].value);
        _novoAtributo.valor = _item.attributes[0].value;
        for (var _valor in _item.findAllElements('valor')) {
          _novoAtributo.valores.add(_valor.text);
        }
        _atributos.add(_novoAtributo);
      }
    }
  }

  Widget _descDesejo() {
    return ListView.builder(
        padding: EdgeInsets.all(30.0),
        itemCount: _atributos.length,
        itemBuilder: (context, index) {
          return _atributoInput(context, index);
        });
  }

  Widget _atributoInput(BuildContext context, int index) {
    if (_atributos[index].valores.length > 2) {
      List<DropdownMenuItem<String>> _valores = new List.empty(growable: true);
      for (var _valor in _atributos[index].valores) {
        _valores
            .add(new DropdownMenuItem(value: _valor, child: new Text(_valor)));
      }

      return DropdownButton(
          isExpanded: true,
          value: _atributos[index].valor,
          items: _valores,
          onChanged: (String novoValor) {
            setState(() {
              _atributos[index].valor = novoValor;
            });
          });
    } else {
      return Column(children: <Widget>[
        Padding(
            padding: EdgeInsets.only(bottom: 10.0),
            child: TextField(
                controller: _descricaoController,
                decoration:
                    InputDecoration(labelText: _atributos[index].descricao),
                keyboardType: TextInputType.multiline,
                maxLines: 5)),
        Text(
          _atributos[index].valores[1],
          style: TextStyle(color: Colors.grey, fontSize: 14.0),
        ),
      ]);
      ;
    }
  }

  Future _instrucaoPreenchimento(BuildContext context) async {
    // flutter defined function
    bool lida = await _dicaLida();
    if (!lida) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          // return object of type Dialog
          return AlertDialog(
            title: Text("Dica"),
            content: Text("Não é necessário preeencher tudo."),
            actions: <Widget>[
              TextButton(
                child: new Text("OK, entendi"),
                onPressed: () {
                  _salvarDica();
                  Navigator.of(context).pop();
                },
              ),
            ],
          );
        },
      );
    }
  }

  Future _salvarDica() async {
    await _storage.write(key: 'dica_preenchimento', value: 'S');
  }

  Future<bool> _dicaLida() async {
    String lida;
    lida = await _storage.read(key: 'dica_preenchimento');
    return lida == 'S' ?? false;
  }
}
