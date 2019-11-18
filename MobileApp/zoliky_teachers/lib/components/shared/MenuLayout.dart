import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/pages/Administration/DashboardPage.dart';
import 'package:zoliky_teachers/pages/shared/MenuLayoutPage.dart';
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/enums/Pages.dart';

class MenuLayoutState extends State<MenuLayoutPage> {
  MenuLayoutState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();

  bool _showMenuItems = true;
  Widget _homePage;
  Widget _currentPage;
  String _pageTitle;

  User get _logged => Singleton().user;

  _logOut() async {
    var prefs = await SharedPreferences.getInstance();
    await prefs.remove(SettingKeys.lastToken);
    Route r = MaterialPageRoute(
        builder: (context) => LoginPage(
              analytics: this.analytics,
              observer: this.observer,
              autoLogin: false,
            ));
    await Navigator.pushReplacement(context, r);
  }

  @override
  void initState() {
    super.initState();

    this._homePage = DashboardPage(analytics: analytics, observer: observer);

    this._pageTitle = "Přehled";
    this._currentPage = _homePage;
  }

  void _changePage(Pages page, String title) {
    Widget p;
    switch (page) {
      case Pages.loginPage:
        _logOut();
        return;
      case Pages.dashboard:
        p = DashboardPage(analytics: analytics, observer: observer);
        break;
      case Pages.settings:
        break;
    }
    _routePage(p, title);
  }

  void _routePage(Widget page, String title) {
    Navigator.pop(context);
    if (page == null || page.runtimeType == _currentPage.runtimeType) {
      return;
    }
    setState(() {
      _pageTitle = title;
      _currentPage = page;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _key,
      appBar: _getAppBar(),
      body: _currentPage,
      drawer: Drawer(
        child: Column(
          children: <Widget>[
            _drawerUser(),
            Expanded(
              child:
                  (_showMenuItems ? _drawerMenuList() : _drawerAccountList()),
            ),
            _drawerBottom(),
          ],
        ),
      ),
    );
  }

  Widget _drawerUser() {
    return UserAccountsDrawerHeader(
      accountName: Text(
        _logged.fullName,
      ),
      accountEmail: Text(
        _logged.email,
        style: TextStyle(
          color: Colors.white60,
        ),
      ),
      arrowColor: Colors.white,
      currentAccountPicture: CircleAvatar(
        backgroundColor: Colors.white,
        child: ClipOval(
          child: Image.memory(
            _logged.getProfileImage(),
            fit: BoxFit.fill,
          ),
        ),
      ),
      onDetailsPressed: () => {
        this.setState(() {
          _showMenuItems = !_showMenuItems;
        })
      },
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            Color(0xFF2196f3),
            Color(0xFF40c4ff),
          ],
        ),
      ),
    );
  }

  Widget _drawerMenuList() {
    return ListView(
      padding: EdgeInsets.zero,
      children: <Widget>[
        ListTile(
          title: Text(
            "Přehled",
          ),
          leading: Icon(Icons.dashboard),
          onTap: () {
            _changePage(Pages.dashboard, "Přehled");
          },
        ),
        ListTile(
          title: Text(
            "Studenti",
          ),
          leading: Icon(FontAwesomeIcons.users),
          onTap: null,
        ),
        ListTile(
          title: Text(
            "Žolíci",
          ),
          leading: Icon(Icons.apps),
          onTap: null,
        ),
        ListTile(
          title: Text(
            "Třídy",
          ),
          leading: Icon(FontAwesomeIcons.school),
          onTap: null,
        ),
      ],
    );
  }

  Widget _drawerAccountList() {
    return ListView(
      padding: EdgeInsets.zero,
      children: <Widget>[
        ListTile(
          title: Text(
            "Odhlásit se",
          ),
          leading: Icon(Icons.exit_to_app),
          onTap: null,
        ),
      ],
    );
  }

  Widget _drawerBottom() {
    return Container(
      child: Align(
        alignment: FractionalOffset.bottomCenter,
        child: Container(
          child: Column(
            children: <Widget>[
              ListTile(
                title: Text(
                  "Nastavení",
                ),
                leading: Icon(Icons.settings),
                onTap: _logOut,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _getAppBar() {
    return AppBar(
      elevation: 2,
      backgroundColor: Colors.white,
      title: Text(
        _pageTitle,
        style: TextStyle(
          color: Colors.black,
          fontWeight: FontWeight.w700,
          fontSize: 30.0,
        ),
      ),
      leading: new IconButton(
        icon: new Icon(
          Icons.menu,
          color: Colors.black,
        ),
        onPressed: () => _key.currentState.openDrawer(),
      ),
      actions: <Widget>[
        Tooltip(
          message: "Odhlásit se",
          child: IconButton(
            onPressed: _logOut,
            icon: Icon(
              Icons.exit_to_app,
              color: Colors.black,
            ),
            color: Colors.black,
          ),
        ),
      ],
    );
  }

  Widget _oldAppBar() {
    return Positioned(
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.only(
            bottomLeft: Radius.circular(16),
            bottomRight: Radius.circular(16),
          ),
          gradient: LinearGradient(
            colors: [
              Colors.blueAccent,
              Colors.lightBlueAccent,
            ],
          ),
        ),
        height: 85,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          children: <Widget>[
            Padding(
              padding: EdgeInsets.all(10),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: <Widget>[
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Text(
                          _logged.fullName,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            fontWeight: FontWeight.w700,
                            fontSize: 20,
                            color: Colors.white,
                          ),
                        ),
                        Text(
                          _logged.schoolName,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 17,
                            color: Colors.white,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Tooltip(
                    message: "Odhlásit se",
                    child: IconButton(
                      onPressed: _logOut,
                      icon: Icon(
                        Icons.exit_to_app,
                        color: Colors.white,
                      ),
                      color: Colors.white,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
