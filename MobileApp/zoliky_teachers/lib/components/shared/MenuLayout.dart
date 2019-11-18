import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/pages/Administration/DashboardPage.dart';
import 'package:zoliky_teachers/pages/App/SettingsPage.dart';
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

  final List<DrawerMenuItem> _menuItems = [
    DrawerMenuItem(
      icon: Icons.dashboard,
      page: Pages.dashboard,
      title: "Přehled",
    ),
    DrawerMenuItem(
      icon: FontAwesomeIcons.users,
      page: Pages.other,
      title: "Studenti",
    ),
    DrawerMenuItem(
      icon: Icons.apps,
      page: Pages.other,
      title: "Žolíci",
    ),
    DrawerMenuItem(
      icon: FontAwesomeIcons.school,
      page: Pages.other,
      title: "Třídy",
    ),
  ];

  Pages _currentPageEnum;

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
    this._currentPageEnum = Pages.dashboard;
  }

  void _changePage(Pages page, String title) {
    Widget p;
    Navigator.pop(context);
    switch (page) {
      case Pages.loginPage:
        _logOut();
        return;
      case Pages.dashboard:
        p = DashboardPage(analytics: analytics, observer: observer);
        break;
      case Pages.settings:
        p = SettingsPage(analytics: analytics, observer: observer);
        break;
      case Pages.other:
        _showSnackbar("Ještě neimplementováno");
        return;
    }
    _currentPageEnum = page;
    _routePage(p, title);
  }

  void _routePage(Widget page, String title) {
    if (page == null || page.runtimeType == _currentPage.runtimeType) {
      return;
    }
    setState(() {
      _pageTitle = title;
      _currentPage = page;
    });
  }

  void _showSnackbar(String content) {
    var snack = SnackBar(
      content: Text(content),
      backgroundColor: Colors.red[700],
      duration: Duration(seconds: 2),
    );
    _key.currentState.showSnackBar(snack);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _key,
      appBar: _getAppBar(),
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 500),
        child: _currentPage,
      ),
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
    return ListView.builder(
      padding: EdgeInsets.zero,
      itemCount: _menuItems.length,
      shrinkWrap: true,
      itemBuilder: (BuildContext ctnx, int index) {
        return _getMenuItem(_menuItems[index]);
      },
    );
  }

  Widget _getMenuItem(DrawerMenuItem item) {
    return ListTile(
      title: Text(
        item.title,
      ),
      leading: Icon(item.icon),
      selected: item.page == _currentPageEnum,
      onTap: () {
        _changePage(item.page, item.title);
      },
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
          onTap: _logOut,
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
              _getMenuItem(
                DrawerMenuItem(
                  title: "Nastavení",
                  icon: Icons.settings,
                  page: Pages.settings,
                ),
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
      leading: IconButton(
        icon: Icon(
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
}

class DrawerMenuItem {
  final String title;
  final Pages page;
  final IconData icon;

  DrawerMenuItem({this.title, this.page, this.icon});
}
