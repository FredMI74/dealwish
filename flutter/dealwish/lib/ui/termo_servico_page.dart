import 'package:dealwish/helpers/api_helper.dart';
import 'package:flutter/material.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class TermoServicoPage extends StatefulWidget {
  @override
  _TermoServicoPageState createState() => _TermoServicoPageState();
}

class _TermoServicoPageState extends State<TermoServicoPage> {
  ApiHelper api_helper = ApiHelper();
  bool isLoading = false;
  String texto = '';

  @override
  void initState() {
    super.initState();

    setState(() {
      isLoading = true;
    });

    api_helper.consultarTermoServico().then((texto) {
      setState(() {
        isLoading = false;
        this.texto = texto;
      });
    }).catchError((e) {
      _onFail('Falha ao carregar termo de serviço.');
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      appBar: AppBar(
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          centerTitle: true,
          title:
              Text("Termo de Serviço", style: TextStyle(color: Colors.white))),
      body: Stack(
        children: <Widget>[
          SingleChildScrollView(
              padding: EdgeInsets.all(40.0), child: Text(texto, textAlign: TextAlign.justify)),
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
      ),
    );
    ;
  }

  void _onFail(String mensagem) {
    setState(() {
      isLoading = false;
      _scaffoldKey.currentState.showSnackBar(SnackBar(
        content: Text(mensagem),
        backgroundColor: Colors.redAccent,
        duration: Duration(seconds: 2),
      ));
    });
  }
}
