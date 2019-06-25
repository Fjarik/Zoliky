using MahApps.Metro.Controls;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;

namespace ZolikDistributor.Admin
{
	public partial class ZolikRemover : MetroWindow
	{
		private SharedApi.Models.User _me;
		private ReadOnlyCollection<Student> _allStudents;
		private List<Student> _availableStudents = new List<Student>();
		private List<Zolik> _studentsZoliks = new List<Zolik>();
		private Random _rnd = new Random();
		private Student _selectedStudent;
		private Zolik _selectedZolik;
		private bool _changed = false;

		public ZolikRemover(SharedApi.Models.User logged)
		{
			InitializeComponent();

			_me = logged;
		}

		private void ZolikRemover_OnInitialized(object sender, EventArgs e)
		{
			Refresh();
		}

		private void RebindUsers()
		{
			if (ListBoxStudents == null || _allStudents == null || _allStudents.Count < 1) {
				return;
			}

			ListBoxStudents.Items.Clear();

			_availableStudents = _allStudents.ToList();

			if (CBoxClasses.SelectedValue is ComboBoxItem citem && citem.Tag is int cid && cid >= 0) {
				_availableStudents.RemoveAll(x => x.ClassID != cid);
			}

			if (CheckOnlyJokers.IsChecked == true) {
				_availableStudents.RemoveAll(x => x.ZolikCount < 1);
			}

			//_availableStudents = _availableStudents.OrderBy(x => x.Class.Name).ThenBy(x => x.Name).ThenBy(x => x.Lastname).ToList();
			foreach (var u in _availableStudents) {
				//students.Add(u);
				ListBoxItem item = new ListBoxItem() {
					Content = $"{u.Name} {u.Lastname} ({u.ClassName}) ({u.ZolikCount})",
					Tag = u.ID,
					ToolTip = $"{u.ClassName}"
				};
				ListBoxStudents.Items.Add(item);
				//listBox_Students.Items.Add($"{u.Name} {u.Lastname} ({u.Class.Name}) ({u.Zoliky.Count})");
			}
			if (ListBoxStudents.Items.Count > 0) {
				ListBoxStudents.SelectedIndex = 0;
			} else {
				_selectedStudent = null;
				_selectedZolik = null;
				ListBoxZoliks.Items.Clear();
			}
		}

		private void Refresh()
		{
			ShowElements(false);
			GridLoading.Visibility = Visibility.Visible;
			GridMain.Visibility = Visibility.Hidden;
			//var users = await Task.Run(() => _mgr.Users.GetStudents());
			var students = Globals.Students;

			if (students == null || students.Count < 1) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Nezdařilo se načíst uživatele :(");
				Owner?.Focus();
				this.Close();
				return;
			}
			_allStudents = new ReadOnlyCollection<Student>(students);
			_availableStudents = _allStudents.ToList();
			RebindUsers();

			foreach (var c in Globals.Classes) {
				ComboBoxItem item = new ComboBoxItem() {
					Content = c.Name,
					ToolTip = "Filtrovat podle třídy",
					Tag = c.ID,
					Visibility = Visibility.Visible,
				};
				CBoxClasses.Items.Add(item);
			}
			GridMain.Visibility = Visibility.Visible;
			GridLoading.Visibility = Visibility.Hidden;
		}

		private async void listBox_Students_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(ListBoxStudents.SelectedItem is ListBoxItem item)) {
				_selectedStudent = null;
				return;
			}


			ListBoxZoliks.Items.Clear();
			_studentsZoliks.Clear();

			if (!(item.Tag is int id)) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Nezdařilo se vybrat uživatele");
				return;
			}

			_selectedStudent = _availableStudents.SingleOrDefault(x => x.ID == id);

			if (_selectedStudent == null) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Nezdařilo se vybrat uživatele");
				return;
			}
			var zc = new ZolikConnector(_me.Token);
			var a = await zc.GetUserZoliksAsync(_selectedStudent.ID, false);
			if (!a.Any()) {
				Xceed.Wpf.Toolkit.MessageBox.Show($"Něco se nepovedlo {_me.Name}!", "Chyba", MessageBoxButton.OK,
												  MessageBoxImage.Error);
				return;
			}
			_studentsZoliks = a;
			_studentsZoliks = _studentsZoliks.OrderByDescending(x => x.OwnerSince).ToList();
			foreach (var zol in _studentsZoliks) {
				ListBoxZoliks
					.Items.Add($"{zol.Title} – {zol.Type.GetDescription()}{(zol.IsLocked ? " (zamčený)" : "")}");
			}
		}

		private void listBox_Zoliks_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ListBoxZoliks.SelectedItem == null) {
				ShowElements(false);
				_selectedZolik = null;
				return;
			}
			ShowElements(true);
			_selectedZolik = _studentsZoliks[ListBoxZoliks.SelectedIndex];

			TxtType.Text = _selectedZolik.Type.GetDescription();
			TxtTitle.Text = _selectedZolik.Title;
			TxtSubject.Text = Globals.Subjects?
								  .FirstOrDefault(x => x.ID == _selectedZolik.SubjectID)?
								  .Name ??
							  "Počítačová grafika";
			TxtDate.Text = _selectedZolik.OwnerSince.ToString("dd.MM.yyyy");

			var originalOwner = Globals.Students?
									.FirstOrDefault(x => x.ID == _selectedZolik.OriginalOwnerID)?
									.FullName ??
								"Neznámý";
			TxtOriginalOwner.Text = originalOwner;


			Txtreason.Text = _selectedZolik.IsLocked ? _selectedZolik.Lock : "";

			if (_selectedZolik.Type == ZolikType.Normal) {
				GridikMaturitniOtazka.Width = new GridLength(0, GridUnitType.Star);
				RbMo.Focusable = false;
			} else {
				GridikMaturitniOtazka.Width = new GridLength(1, GridUnitType.Star);
				RbMo.Focusable = true;
			}
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			TextBoxHelper.SetWatermark(Txtreason,
									   $"Specifikujte důvod (min. 5 znaků)\n\nNapř.: Prohra sázky, špatná nálada, podvod v úkolu, ...");
		}

		private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			TextBoxHelper.SetWatermark(Txtreason,
									   $"Specifikujte důvod (min. 2 znaky)\n\nNapř.: Test č. {_rnd.Next(1, 13)}, úkol - 3D loga, ...");
		}

		private void ShowElements(bool show)
		{
			Visibility vis = Visibility.Hidden;
			if (show) {
				vis = Visibility.Visible;
			}

			TxtType.Visibility = vis;
			TxtTitle.Visibility = vis;
			TxtDate.Visibility = vis;
			TxtSubject.Visibility = vis;
			TxtOriginalOwner.Visibility = vis;

			BtnInfo.Visibility = vis;

			TxtReasonTitle.Visibility = vis;
			Txtreason.Visibility = vis;
			TxtRemoveTitle.Visibility = vis;

			GridigDuvod.Visibility = vis;

			BtnRemove.Visibility = vis;
		}

		private async void BtnRemove_Click(object sender, RoutedEventArgs e)
		{
			if (RbOther.IsChecked == true) {
				if (Txtreason.Text.Length < 5) {
					Xceed.Wpf.Toolkit.MessageBox
						 .Show("Specifikujte blíže důvod odebrání žolíka!\n\nNapř.: Prohra sázky, špatná nálada, podvod v úkolu, ...",
							   "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			} else {
				if (Txtreason.Text.Length < 2) {
					Xceed.Wpf.Toolkit.MessageBox
						 .Show($"Specifikujte blíže důvod odebrání žolíka!\n\nNapř.: Test č. {_rnd.Next(1, 13)}, úkol - 3D loga, ...",
							   "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}

			if (_selectedStudent == null || _selectedZolik == null) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Někde se stala chyba", "Chyba", MessageBoxButton.OK,
												  MessageBoxImage.Error);
				return;
			}
			string removalMessage = "";

			if (RbTest.IsChecked == true) {
				removalMessage = $"Odebráno za použití na test: {Txtreason.Text}";
			} else if (RbMo.IsChecked == true) {
				removalMessage = $"Odebráno za použití na maturitní otázku: {Txtreason.Text}";
			} else if (RbHomework.IsChecked == true) {
				removalMessage = $"Odebráno za použití na úkol: {Txtreason.Text}";
			} else if (RbOther.IsChecked == true) {
				removalMessage = $"Odebráno (dále specifikováno): {Txtreason.Text}";
			}

			var connector = new ZolikConnector(_me.Token);
			ZolikPackage zp = new ZolikPackage {
				FromID = _selectedStudent.ID,
				ToID = _me.ID,
				ZolikID = _selectedZolik.ID,
				Type = TransactionAssignment.ZerziRemoval,
				Message = removalMessage
			};
			var res = await connector.TransferAsync(zp);
			if (!res.IsSuccess) {
				Xceed.Wpf.Toolkit.MessageBox.Show($"Něco se nepovedlo {_me.Name}!", "Chyba", MessageBoxButton.OK,
												  MessageBoxImage.Error);
				return;
			}

			Xceed.Wpf.Toolkit.MessageBox.Show("Žolík byl úspěšně odebrán", "Úspěch", MessageBoxButton.OK,
											  MessageBoxImage.Information);
			_changed = true;
			Refresh();
		}

		private async void ZolikRemover_OnClosing(object sender, CancelEventArgs e)
		{
			if (this.Owner is MainWindow mw && _changed) {
				await mw.RefreshStatsAsync();
			}
		}

		private void CBoxClasses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RebindUsers();
		}

		private void CheckOnlyJokers_Changed(object sender, RoutedEventArgs e)
		{
			RebindUsers();
		}

		private void BtnInfo_OnClick(object sender, RoutedEventArgs e)
		{
			if (_selectedZolik == null) {
				return;
			}
			ZolikTransactions trans = new ZolikTransactions(_me, _selectedZolik);
			trans.ShowDialog();
		}
	}
}