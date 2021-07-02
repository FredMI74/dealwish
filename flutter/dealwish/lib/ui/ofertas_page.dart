import 'dart:async';
import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/models/desejo_model.dart';
import 'package:dealwish/models/oferta_model.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:dealwish/ui/oferta_detalhe_page.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/helpers/animator.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class Ofertas extends StatefulWidget {
  @override
  _OfertasState createState() => _OfertasState();

  final Desejo desejo;

  Ofertas({this.desejo});
}

class _OfertasState extends State<Ofertas> {
  ApiHelper api_helper = ApiHelper();

  List<Oferta> ofertas = List.empty(growable: true);
  bool isLoading = true;

  @override
  void initState() {
    super.initState();

    _atualizarOfertas();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
          centerTitle: true,
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          title: Text("Ofertas - " + widget.desejo.desc_tp_produto,
              style: TextStyle(color: Colors.white)),
        ),
        body: RefreshIndicator(
            child: _ofertas(),
            onRefresh: _atualizarOfertas,
            color: Color.fromARGB(255, 255, 127, 0)),
        floatingActionButton: Opacity(
            opacity: 0.6,
            child: Column(
              verticalDirection: VerticalDirection.up,
              children: <Widget>[
                FloatingActionButton(
                  onPressed: () {
                    _confirmarExclusao();
                  },
                  child: Icon(Icons.delete),
                  backgroundColor: !isLoading
                      ? Color.fromARGB(255, 255, 127, 0)
                      : Colors.black12,
                  heroTag: null,
                ),
                Padding(
                    padding: EdgeInsets.only(bottom: 10.0),
                    child: FloatingActionButton(
                      onPressed: () {
                        _confirmarAtendido();
                      },
                      child: Icon(Icons.check_circle),
                      backgroundColor:
                          !isLoading && widget.desejo.qtd_ofertas > 0
                              ? Color.fromARGB(255, 255, 127, 0)
                              : Colors.black12,
                      heroTag: null,
                    )),
              ],
            )));
  }

  Widget _ofertas() {
    if (isLoading) {
      return Container(
          child: Center(
        child: CircularProgressIndicator(
          valueColor: new AlwaysStoppedAnimation<Color>(
              Color.fromARGB(255, 255, 127, 0)),
        ),
      ));
    } else {
      return ListView.builder(
          padding: EdgeInsets.all(10.0),
          itemCount: _calclLength(ofertas.length),
          itemBuilder: (context, index) {
            if (ofertas.length > 0) {
              if (index == 0) {
                return Column(
                  children: <Widget>[
                    _desejoCard(widget.desejo),
                    _ofertaCard(context, index)
                  ],
                );
              } else {
                return WidgetAnimator(_ofertaCard(context, index));
              }
            } else {
              return Column(
                children: <Widget>[_desejoCard(widget.desejo), _aguardeCard()],
              );
            }
          });
    }
  }

  Widget _ofertaCard(BuildContext context, int index) {
    return GestureDetector(
      child: Card(
        child: Padding(
          padding: EdgeInsets.all(10.0),
          child: Stack(children: <Widget>[
            Row(
              children: <Widget>[
                Container(
                  width: 80.0,
                  height: 80.0,
                  decoration: BoxDecoration(
                    shape: BoxShape.rectangle,
                    image: DecorationImage(
                        image: MemoryImage(ofertas[index].logo)),
                  ),
                ),
                Expanded(
                  child: Container(
                    child: Padding(
                      padding: EdgeInsets.only(left: 10.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: <Widget>[
                          Text(
                            ofertas[index].fantasia,
                            style: TextStyle(
                                fontSize: 24.0, fontWeight: FontWeight.bold),
                          ),
                          Text(ofertas[index].valor,
                              style: TextStyle(fontSize: 22.0)),
                          Text(ofertas[index].descricao,
                              style:
                                  TextStyle(fontSize: 16.0, color: Colors.grey),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis),
                        ],
                      ),
                    ),
                  ),
                )
              ],
            ),
            ofertas[index].destaque == SIM
                ? Container(
                    alignment: AlignmentDirectional.topEnd,
                    child: Icon(Icons.star, color: Colors.amber))
                : Container()
          ]),
        ),
      ),
      onTap: () {
        _showOferta(desejo: widget.desejo, oferta: ofertas[index]);
      },
    );
  }

  Widget _aguardeCard() {
    return Card(
        child: Padding(
            padding: EdgeInsets.all(10.0),
            child: Row(
              children: <Widget>[
                Expanded(
                  child: Container(
                    child: Padding(
                      padding: EdgeInsets.only(left: 20.0),
                      child: Text(
                        "Aguarde, em breve você receberá ofertas para o seu desejo.",
                        style: TextStyle(fontSize: 16.0, color: Colors.grey),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ),
                ),
              ],
            )));
  }

  Widget _desejoCard(Desejo desejo) {
    return Card(
        color: Color.fromARGB(255, 230, 230, 230),
        child: Padding(
            padding: EdgeInsets.all(10.0),
            child: Row(
              children: <Widget>[
                Expanded(
                  child: Container(
                    child: Padding(
                      padding: EdgeInsets.only(left: 20.0),
                      child: Text(
                        desejo.descricao,
                        style: TextStyle(fontSize: 22.0),
                        maxLines: 5,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ),
                ),
                Container(
                  width: 40.0,
                  height: 40.0,
                  decoration: BoxDecoration(
                    shape: BoxShape.rectangle,
                    image: DecorationImage(
                        image: MemoryImage(desejo.icone_tp_produto)),
                  ),
                ),
              ],
            )));
  }

  void _showOferta({Desejo desejo, Oferta oferta}) async {
    final recContact = await Navigator.push(
        context,
        MaterialPageRoute(
            builder: (context) =>
                OfertaDetalhe(desejo: desejo, oferta: oferta)));
  }

  int _calclLength(int length) {
    if (length == 0) {
      return 1;
    } else {
      return length;
    }
  }

  Future<Null> _atualizarOfertas() {
    return api_helper.consultarOfertas(widget.desejo.id).then((list) {
      setState(() {
        isLoading = false;
        ofertas = list;
      });
    }).catchError((e) {
      _onFail(mensagem : 'Falha ao listar ofertas.');
    });
  }

  void _confirmarExclusao() {
    // flutter defined function
    if (!isLoading) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          // return object of type Dialog
          return AlertDialog(
            title: Text("Desejo não realizado?"),
            content: Text("O seu desejo será excluído."),
            actions: <Widget>[
              // usually buttons at the bottom of the dialog
              TextButton(
                child: new Text("Cancelar"),
                onPressed: () {
                  Navigator.of(context).pop();
                },
              ),
              TextButton(
                child: new Text("OK"),
                onPressed: () {
                  widget.desejo.id_situacao = 7; // Não Realizado
                  _atualizarsituacaoDesejo(widget.desejo);
                  Navigator.of(context).pop();
                },
              ),
            ],
          );
        },
      );
    }
  }

  void _atualizarsituacaoDesejo(Desejo desejo) async {
    setState(() {
      isLoading = true;
    });

    try {
      RetornoProcessamento retornoProcessamento =
          await api_helper.atualizarsituacaoDesejo(desejo);

      if (retornoProcessamento.linhasafetadas > 0) {
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

  void _onFail({String mensagem = ''}) {
    setState(() {
      isLoading = false;
      if (mensagem == '') {
        mensagem = "Falha ao atualizar o Desejo!";
      }
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text("Falha ao atualizar o Desejo!"),
        backgroundColor: Colors.redAccent,
        duration: Duration(seconds: 2),
      ));
    });
  }

  void _confirmarAtendido() {
    // flutter defined function
    if (!isLoading && widget.desejo.qtd_ofertas > 0) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          // return object of type Dialog
          return AlertDialog(
            title: Text("Desejo atendido?"),
            content: Text("O seu desejo foi realizado."),
            actions: <Widget>[
              // usually buttons at the bottom of the dialog
              TextButton(
                child: new Text("Cancelar"),
                onPressed: () {
                  Navigator.of(context).pop();
                },
              ),
              TextButton(
                child: new Text("OK"),
                onPressed: () {
                  widget.desejo.id_situacao = 6; // Realizado
                  _atualizarsituacaoDesejo(widget.desejo);
                  Navigator.of(context).pop();
                },
              ),
            ],
          );
        },
      );
    }
  }
}
