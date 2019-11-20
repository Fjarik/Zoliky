import 'package:flutter/material.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';

class ZoliksDetailPage extends StatefulWidget {
  ZoliksDetailPage({Key key, this.list}) : super(key: key);

  final List<CustomZolik> list;

  @override
  _ZoliksDetailPageState createState() => _ZoliksDetailPageState(list);
}

class _ZoliksDetailPageState extends State<ZoliksDetailPage> {
  _ZoliksDetailPageState(List<CustomZolik> list) {
    _zoliks = list;
  }

  List<CustomZolik> _zoliks = List<CustomZolik>();

  @override
  Widget build(BuildContext context) {
    return ExpansionPanelList(
      expansionCallback: (int i, bool isExpanded) {
        this.setState(() {
          _zoliks[i].isExpanded = !isExpanded;
        });
      },
      children: _zoliks.map<ExpansionPanel>((CustomZolik z) {
        return ExpansionPanel(
          canTapOnHeader: true,
          headerBuilder: (BuildContext c, bool isExpanded) {
            return ListTile(
              title: Text(
                "${z.original.title} (${z.original.ownerName})",
                overflow: TextOverflow.ellipsis,
              ),
            );
          },
          isExpanded: z.isExpanded,
          body: ListTile(
            title: Text(z.original.originalOwnerName),
          ),
        );
      }).toList(),
    );
  }
}

class CustomZolik {
  bool isExpanded = false;

  Zolik original;

  CustomZolik(Zolik z) {
    original = z;
  }
}
