import 'package:dealwish/ui/novo_desejo_page.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/models/tp_produto_model.dart';
import 'package:dealwish/helpers/animator.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class NovoDesejoTipo extends StatefulWidget {
  final int id_grp_produto;

  NovoDesejoTipo({this.id_grp_produto});

  @override
  _NovoDesejoTipoState createState() => _NovoDesejoTipoState();
}

class _NovoDesejoTipoState extends State<NovoDesejoTipo> {
  ApiHelper api_helper = ApiHelper();

  List<TpProduto> tipos = List.empty(growable: true);
  bool isLoading;

  @override
  void initState() {
    super.initState();

    setState(() {
      isLoading = true;
    });

    api_helper.consultarTpPrdutos(widget.id_grp_produto).then((list) {
      setState(() {
        isLoading = false;
        tipos = list;
      });
    }).catchError((e) {
      _onFail('Falha ao carregar lista.');
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
        title: Text("Escolha um Produto", style: TextStyle(color: Colors.white)),
      ),
      body: _tipos(),
    );
  }

  Widget _tipos() {
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
          itemCount: tipos.length,
          itemBuilder: (context, index) {
            return WidgetAnimator(_grupoCard(context, index));
          });
    }
  }

  Widget _grupoCard(BuildContext context, int index) {
    return GestureDetector(
      child: Card(
        child: Padding(
          padding: EdgeInsets.all(10.0),
          child: Row(
            children: <Widget>[
              Container(
                width: 58.0,
                height: 58.0,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  image:
                  DecorationImage(image: MemoryImage(tipos[index].icone)),
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
                          tipos[index].descricao,
                          style: TextStyle(
                              fontSize: 22.0, fontWeight: FontWeight.bold),
                        ),
                      ],
                    ),
                  ),
                ),
              )
            ],
          ),
        ),
      ),
      onTap: () {
        _showNovoDesejo(tppproduto: tipos[index]);
      },
    );
  }

  void _showNovoDesejo({TpProduto tppproduto}) async {
    final recContact = await Navigator.push(
        context,
        MaterialPageRoute(
            builder: (context) => NovoDesejo(
                id_tp_produto: tppproduto.id,
                desc_tp_produto: tppproduto.descricao,
                preenchimento: tppproduto.preenchimento)));
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
