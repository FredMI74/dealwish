import 'package:dealwish/models/desejo_model.dart';
import 'package:dealwish/models/oferta_model.dart';
import 'package:dealwish/models/retorno_proc_model.dart';
import 'package:dealwish/ui/empresa.dart';
import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:dealwish/helpers/api_helper.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

const int ID_DEALWISH = 1;

class OfertaDetalhe extends StatefulWidget {
  @override
  _OfertaDetalheState createState() => _OfertaDetalheState();

  final Oferta oferta;
  final Desejo desejo;

  OfertaDetalhe({this.desejo, this.oferta});
}

class _OfertaDetalheState extends State<OfertaDetalhe> {
  ApiHelper api_helper = ApiHelper();

  @override
  void initState() {
    super.initState();

    _atualizarlidaOferta();
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
          title: Text("Oferta - " + widget.desejo.desc_tp_produto,
              style: TextStyle(color: Colors.white)),
        ),
        body: _ofertacard(),
        floatingActionButton: Opacity(
            opacity: 0.6,
            child: Column(
              verticalDirection: VerticalDirection.up,
              children: <Widget>[
                Transform.scale(
                    scale: widget.oferta.like_unlike == 'U'
                        ? 1.0
                        : 0.80,
                    child: FloatingActionButton(
                      onPressed: () {
                        _like_unlike('U');
                      },
                      child: Icon(Icons.thumb_down),
                      backgroundColor: widget.oferta.like_unlike == 'U' ? Colors.red : Colors.black12,
                      heroTag: null,
                    )),
                Padding(
                    padding: EdgeInsets.only(bottom: 10.0),
                    child: Transform.scale(
                        scale: widget.oferta.like_unlike == 'L'
                            ? 1.0
                            : 0.80,
                        child: FloatingActionButton(
                          onPressed: () {
                            _like_unlike('L');
                          },
                          child: Icon(Icons.thumb_up),
                          backgroundColor: widget.oferta.like_unlike == 'L' ? Colors.green: Colors.black12,
                          heroTag: null,
                        ))),
                Padding(
                    padding: EdgeInsets.only(bottom: 10.0),
                    child: Transform.scale(
                        scale: 0.80,
                        child: FloatingActionButton(
                          onPressed: () {
                            _showEmpresa(widget.oferta.id_empresa);
                          },
                          child: Icon(Icons.info_outline),
                          backgroundColor: Colors.blue,
                          heroTag: null,
                        ))),
              ],
            )));
  }

  Widget _oferta() {
    return Padding(
        padding: EdgeInsets.all(20.0),
        child: Column(
          children: <Widget>[
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                Container(
                  width: 60.0,
                  height: 80.0,
                  decoration: BoxDecoration(
                    shape: BoxShape.rectangle,
                    image:
                        DecorationImage(image: MemoryImage(widget.oferta.logo)),
                  ),
                ),
                Text(
                  " " + widget.oferta.fantasia,
                  style: TextStyle(fontSize: 30.0, fontWeight: FontWeight.bold),
                ),
              ],
            ),
            Container(
                padding: EdgeInsets.only(top: 20.0, bottom: 20.0),
                child: Text(
                  widget.oferta.valor,
                  style: TextStyle(fontSize: 26.0, fontWeight: FontWeight.bold),
                )),
            Divider(color: Colors.black,),
            Text(
              widget.oferta.descricao,
              style: TextStyle(fontSize: 20.0),
            ),
            Divider(color: Colors.black,),
            Container(
                padding: EdgeInsets.only(top: 20.0, bottom: 20.0),
                child: widget.oferta.id_empresa != ID_DEALWISH ? Text(
                  'Validade: ' + widget.oferta.validade,
                  style: TextStyle(
                      fontSize: 18.0,
                      fontWeight: FontWeight.bold,
                      fontStyle: FontStyle.italic),
                ) : Container()) ,
            ElevatedButton(
                style: ElevatedButton.styleFrom(
                  primary: Color.fromARGB(255, 255, 127, 0),
                  onPrimary: Colors.white,
                ),
                child: Text(
                  'Ir para Oferta',
                  style: TextStyle(
                      color: Colors.white,
                      fontSize: 20.0,
                      fontWeight: FontWeight.bold),
                ),
                onPressed: () {
                  _launchURL(widget.oferta.url);
                }),
          ],
        ));
  }

  Widget _ofertacard()
  {
   return  Padding(
       padding: EdgeInsets.all(5.0),
       child: Card(
           child: Padding(
               padding: EdgeInsets.all(10.0),
               child: Row(
                 children: <Widget>[
                   Expanded(
                     child: Container(
                       child: Padding(
                         padding: EdgeInsets.all(0.0),
                         child: _oferta(),
                       ),
                     ),
                   ),
                 ],
               )
           )
       )
   );
  }

  _launchURL(String url) async {
    if (await canLaunch(url)) {
      await launch(url);
    } else {
      throw 'Could not launch $url';
    }
  }

  Future<Null> _atualizarlidaOferta() async {
    if (widget.oferta.lida == 'N') {
      try {
        RetornoProcessamento retornoProcessamento =
            await api_helper.atualizarlidaOferta(widget.oferta.id);
      } catch (e) {
        _onFail();
      }
    }
  }

  void _showEmpresa(int id_empresa) async {
    final recContact = await Navigator.push(
        context,
        MaterialPageRoute(
            builder: (context) =>
                Empresa(id_empresa, widget.oferta.url)));
  }

  Future<Null> _like_unlike(String like_unlike) async {
    setState(() {
      widget.oferta.like_unlike = like_unlike;
    });

    try {
      RetornoProcessamento retornoProcessamento = await api_helper
          .atualizarlikeunlikeOferta(widget.oferta.id, like_unlike);
    } catch (e) {
      _onFail();
    }
  }

  void _onFail() {
    setState(() {
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text("Falha ao marcar Oferta como lida!"),
        backgroundColor: Colors.redAccent,
        duration: Duration(seconds: 2),
      ));
    });
  }
}
