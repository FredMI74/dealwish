import 'package:dealwish/helpers/animator.dart';
import 'package:dealwish/ui/novo_desejo_tipo_page.dart';
import 'package:flutter/material.dart';
import 'package:dealwish/helpers/api_helper.dart';
import 'package:dealwish/models/grp_produto_model.dart';

final _scaffoldKey = GlobalKey<ScaffoldState>();

class NovoDesejoGrupo extends StatefulWidget {
  @override
  _NovoDesejoGrupoState createState() => _NovoDesejoGrupoState();
}

class _NovoDesejoGrupoState extends State<NovoDesejoGrupo> {
  ApiHelper api_helper = ApiHelper();

  List<GrpProduto> grupos = List.empty(growable: true);
  bool isLoading;

  @override
  void initState() {
    super.initState();

    setState(() {
      isLoading = true;
    });

    api_helper.consultarGrpPrdutos().then((list) {
      setState(() {
        isLoading = false;
        grupos = list;
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
        title: Text("Escolha um Departamento",
            style: TextStyle(color: Colors.white)),
      ),
      body: _grupos(),
    );
  }

  Widget _grupos() {
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
          itemCount: grupos.length,
          itemBuilder: (context, index) {
            return WidgetAnimator(_grupoCard(context, index));
          });
    }
  }

  Widget _grupoCard(BuildContext context, int index) {
    return GestureDetector(
      child: Card(
        child: Padding(
          padding: EdgeInsets.fromLTRB(14.0, 16.0, 14.0, 16.0),
          child: Row(
            children: <Widget>[
              Container(
                width: 58.0,
                height: 58.0,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  image:
                      DecorationImage(image: MemoryImage(grupos[index].icone)),
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
                          grupos[index].descricao,
                          style: TextStyle(
                              fontSize: 22.0, fontWeight: FontWeight.bold),
                        ),
                        Text(
                          grupos[index].tipos_produtos,
                          style: TextStyle(fontSize: 16.0, color: Colors.grey),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
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
        _showNovoDesejoTipoProduto(grpproduto: grupos[index]);
      },
    );
  }

  void _showNovoDesejoTipoProduto({GrpProduto grpproduto}) async {
    final recContact = await Navigator.push(
        context,
        MaterialPageRoute(
            builder: (context) =>
                NovoDesejoTipo(id_grp_produto: grpproduto.id)));
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
