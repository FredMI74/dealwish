import 'dart:async';
import 'package:dw_dashboard/models/indicadores.dart';
import 'package:flutter/material.dart';
import 'package:dw_dashboard/helpers/api_helper.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class IndicadoresPage extends StatefulWidget {
  @override
  _IndicadoresPage createState() => _IndicadoresPage();
}

class _IndicadoresPage extends State<IndicadoresPage> {
  ApiHelper api_helper = ApiHelper();

  bool isLoading = true;

  @override
  void initState() {
    super.initState();

    _atualizaIndicadores();

    Timer.periodic(Duration(seconds: 60), (timer) {
      _atualizaIndicadores();
    });
  }

  List<Indicador> indicadores = List();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
          iconTheme: IconThemeData(
            color: Colors.white, //change your color here
          ),
          centerTitle: true,
          title: Column(
            children: <Widget>[
              Container(
                padding: EdgeInsets.only(bottom: 6.0),
                child: Text("Dealwish",
                    style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: 20.0)),
              ),
              Text("Dealwish - Indicadores", style: TextStyle(color: Colors.white))
            ],
          ),
        ),
        body: Stack(
          children: <Widget>[
            Image.asset(
              "images/fundo.png",
              fit: BoxFit.fitHeight,
              height: 1000.0,
            ),
            RefreshIndicator(
                onRefresh: _atualizaIndicadores,
                child: _indicadores(),
                color: Color.fromARGB(255, 255, 127, 0))
          ],
        ));
  }

  Widget _indicadores() {
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
          itemCount: indicadores.length,
          itemBuilder: (context, index) {
            return _indicadorCard(context, index);
          });
    }
  }

  Widget _indicadorCard(BuildContext context, int index) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(40.0),
        child: Row(
          children: <Widget>[
            Row(
              children: <Widget>[
                Container(width: 600.0, child: Text(
                    indicadores[index].indicador.toString() + ":" ,
                    style: TextStyle(fontSize: 100.0))),
                Text(
                    indicadores[index].valor.toString(),
                    style: TextStyle(fontSize: 100.0))
              ],
            )
          ],
        ),
      ),
    );
  }

  Future<Null> _atualizaIndicadores() {
    try {
    return api_helper.consultarIndicadores().then((list) {
      setState(() {
        isLoading = false;
        indicadores = list;
      });
    });
    } catch (e) {
      setState(() {
        isLoading = false;
      });
      _onFail("Falha ao entrar" + e.toString());
    }
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
