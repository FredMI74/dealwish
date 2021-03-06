import 'package:flutter/material.dart';

Widget hint(
    {String line1, String line2, double width, EdgeInsetsGeometry padding}) {
  return Padding(
      padding: padding ?? EdgeInsets.fromLTRB(0.0, 0.0, 0.0, 0.0),
      child: Container(
          width: width,
          child: Row(
            children: <Widget>[
              Material(
                borderRadius: BorderRadius.all(Radius.circular(10.0)),
                color: Colors.blue,
                child: Padding(
                    padding: EdgeInsets.all(10.0),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: <Widget>[
                        Text(line1,
                            style:
                                TextStyle(fontSize: 16.0, color: Colors.white)),
                        Text(line2,
                            style:
                                TextStyle(fontSize: 16.0, color: Colors.white)),
                      ],
                    )),
              ),
              Transform.translate(
                  offset: Offset(-6.0, 0.0),
                  child: RotationTransition(
                      turns: AlwaysStoppedAnimation(45 / 360),
                      child: Container(
                        alignment: Alignment.bottomRight,
                        color: Colors.blue,
                        width: 12.0,
                        height: 12.0,
                      )))
            ],
          )));
}
