import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikDetailPage.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';

class ZolikDetailState extends State<ZolikDetailPage> {
  ZolikDetailState(Zolik z) {
    zolik = z;
  }

  Zolik zolik;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Padding(
        padding: EdgeInsets.all(10),
        child: FutureBuilder(
          builder: (BuildContext ctx, AsyncSnapshot a) {
            if (a.connectionState != ConnectionState.done) {}

            return Container(
              child: Row(
                children: <Widget>[
                  Text("Test"),
                  Text("Test"),
                  Column(
                    children: <Widget>[
                      Text("Test"),
                      Text("Test"),
                      Text("Test"),
                      Text("Test"),
                    ],
                  ),
                  Text("Test"),
                  Text("Test"),
                ],
              ),
            );
          },
        ),
      ),
    );
  }
}
