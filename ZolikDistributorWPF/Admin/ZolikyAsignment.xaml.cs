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
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;

namespace ZolikDistributor.Admin
{
	public partial class ZolikyAsignment : MetroWindow
	{
		private ReadOnlyCollection<Student> _allStudents;
		private List<Student> _availableStudents = new List<Student>();
		private List<Student> _chosenUsers = new List<Student>();
		private SharedApi.Models.User _me;
		public bool Changed { get; set; }

		public ZolikyAsignment(SharedApi.Models.User ucitel)
		{
			_me = ucitel;
			Changed = false;
			InitializeComponent();
		}

		private void ZolikyAsignment_OnInitialized(object sender, EventArgs e)
		{
			ChangeLoading(true);
			var students = Globals.Students;
			this._allStudents = new ReadOnlyCollection<Student>(students);
			this._availableStudents = _allStudents.ToList();

			RebindUsers();

			foreach (var c in Globals.Classes) {
				var item = new ComboBoxItem() {
					Content = c.Name,
					ToolTip = "Filtrovat podle třídy",
					Tag = c.ID,
					Visibility = Visibility.Visible,
				};
				CBoxClasses.Items.Add(item);
			}
			foreach (var s in Globals.Subjects) {
				var item = new ComboBoxItem() {
					Content = s.Shortcut,
					ToolTip = s.Name,
					Tag = s.ID,
					Visibility = Visibility.Visible
				};
				CBoxSubjects.Items.Add(item);
			}
			ChangeLoading(false);
		}

		private void RebindUsers()
		{
			if (ListBoxAvailableUsers == null) {
				return;
			}
			ListBoxAvailableUsers.Items.Clear();
			CheckFilters();
			_availableStudents = _availableStudents
								 .OrderBy(x => x.ClassName).ThenBy(x => x.Name).ThenBy(x => x.Lastname).ToList();
			foreach (var u in _availableStudents) {
				//availableUsers.Add(u);
				if (_chosenUsers.Any(x => x.ID == u.ID)) {
					continue;
				}
				ListBoxItem item = new ListBoxItem() {
					Content = $"{u.Name} {u.Lastname} ({u.ClassName})",
					Tag = u.ID,
					ToolTip = $"{u.ClassName}"
				};
				ListBoxAvailableUsers.Items.Add(item);
			}
		}

		private void ChangeLoading(bool show)
		{
			if (show) {
				GridMain.Visibility = Visibility.Hidden;
				GridLoading.Visibility = Visibility.Visible;
			} else {
				GridMain.Visibility = Visibility.Visible;
				GridLoading.Visibility = Visibility.Hidden;
			}
		}

		private void BtnAddUser_Click(object sender, RoutedEventArgs e)
		{
			if (ListBoxAvailableUsers.SelectedItem is ListBoxItem item && item.Tag is int id) {
				ListBoxAvailableUsers.Items.Remove(item);
				ListBoxChosenUsers.Items.Add(item);

				var moving = _availableStudents.Find(x => x.ID == id);

				_chosenUsers.Add(moving);
				_availableStudents.Remove(moving);

				//	listBox_chosenUsers.Items.Add(listBox_availableUsers.SelectedIndex);
				//	chosenUsers.Add(students[listBox_availableUsers.SelectedIndex]);
				//	students.RemoveAt(listBox_availableUsers.SelectedIndex);
				//listBox_availableUsers.Items.RemoveAt(listBox_availableUsers.SelectedIndex);
			}
		} //adding users to list that will be assigned a Žolík

		private void BtnRemoveUser_Click(object sender, RoutedEventArgs e)
		{
			if (ListBoxChosenUsers.SelectedItem is ListBoxItem item && item.Tag is int id) {
				ListBoxChosenUsers.Items.Remove(item);


				var moving = _chosenUsers.Find(x => x.ID == id);


				_chosenUsers.Remove(moving);
				/*
				if (!(CBoxClasses.SelectedValue is ComboBoxItem citem && citem.Tag is int cid && cid < 1) || (cid == moving.ClassID)) {
					//	listBox_availableUsers.Items.Add(item);
				}
				*/
				if (_availableStudents.All(x => x.ID != moving.ID)) {
					_availableStudents.Add(moving);
				}


				RebindUsers();

				/*
				listBox_availableUsers.Items.Add(listBox_chosenUsers.SelectedItem);
				students.Add(chosenUsers[listBox_chosenUsers.SelectedIndex]);
				chosenUsers.RemoveAt(listBox_chosenUsers.SelectedIndex);

				listBox_chosenUsers.Items.RemoveAt(listBox_chosenUsers.SelectedIndex);
				*/
			}
		} //reverting users to list that won't be assigned a Žolík

		private async void BtnSendZoliks_Click(object sender, RoutedEventArgs e)
		{
			var text = TxtJokerDescription.Text;
			if (string.IsNullOrWhiteSpace(text) && _chosenUsers.Count < 1) {
				Xceed.Wpf.Toolkit.MessageBox
					 .Show("Vyplňte za co byl žolík udělen a vyberte minimálně jednoho studenta!",
						   "Chyba při odesílání", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (string.IsNullOrWhiteSpace(text)) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Vyplňte za co byl žolík udělen!", "Chyba při odesílání",
												  MessageBoxButton.OK, MessageBoxImage.Error);
				TxtJokerDescription.Focus();
				return;
			}
			text = text.Trim();
			if (_chosenUsers.Count < 1) {
				Xceed.Wpf.Toolkit.MessageBox.Show("Vyberte minimálně jednoho studenta!", "Chyba při odesílání",
												  MessageBoxButton.OK, MessageBoxImage.Error);
				ListBoxAvailableUsers.Focus();
				return;
			}

			MessageBoxResult mbr =
				Xceed.Wpf.Toolkit.MessageBox
					 .Show($"Opravdu si přejete odeslat žolíky vybraným studentům ({_chosenUsers.Count})?\n\nTato operace je nevratná!",
						   "Odesílání žolíků", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (mbr != MessageBoxResult.Yes) {
				return;
			}
			ChangeLoading(true);
			bool allowSplit = false;
			ZolikType type = ZolikType.Normal;
			if (RbJokerJoker.IsChecked == true) {
				type = ZolikType.Joker;
				allowSplit = ChBoxSplit.IsChecked == true;
			} else if (RbJokerBlack.IsChecked == true) {
				type = ZolikType.Black;
			}
			var subjectId = Ext.OtherSubjectId;
			if (CBoxSubjects.SelectedItem is ComboBoxItem i && i.Tag is int id && id > 0) {
				subjectId = id;
			}
			var connector = new ZolikConnector(_me.Token);
			foreach (var u in _chosenUsers) {
				// TODO: Opravit
				Xceed.Wpf.Toolkit.MessageBox.Show($"Aktuálně nefunkční", "Chyba", MessageBoxButton.OK,
												  MessageBoxImage.Error);
				ChangeLoading(false);

				var z = await connector.CreateAndTransferAsync(_me.ID,
															   u.ID,
															   type,
															   subjectId,
															   text,
															   allowSplit);
				if (!z.IsSuccess) {
					Xceed.Wpf.Toolkit.MessageBox.Show($"Něco se nepovedlo {_me.Name}!", "Chyba", MessageBoxButton.OK,
													  MessageBoxImage.Error);
					ChangeLoading(false);
					return;
				}
			}
			var res = MessageBox.Show("Odeslání proběhlo úspěšně! \n\nPřejete si přidělit další žolíky?", "Úspěch",
									  MessageBoxButton.YesNo, MessageBoxImage.Information);
			if (res == MessageBoxResult.Yes) {
				ZolikyAsignment za = new ZolikyAsignment(_me);
				if (this.Owner != null) {
					za.Owner = this.Owner;
					za.Changed = true;
				}
				za.Show();
			} else {
				if (this.Owner != null) {
					Changed = true;
					this.Owner.Show();
				}
			}
			this.Close();
		}

		private void CBoxClasses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_allStudents == null) {
				return;
			}
			if (!(CBoxClasses.SelectedValue is ComboBoxItem item) || !(item.Tag is int id) || id < 1) {
				_availableStudents = _allStudents.ToList();
				RebindUsers();
				return;
			}

			_availableStudents = _allStudents.Where(x => x.ClassID == id).ToList();
			RebindUsers();
		}

		private void CheckFilters()
		{
			if (CBoxClasses.SelectedValue is ComboBoxItem citem && citem.Tag is int cid && cid >= 0) {
				_availableStudents.RemoveAll(x => x.ClassID != cid);
			}

			string lname = TxtSearch.Text;
			if (!string.IsNullOrWhiteSpace(lname)) {
				lname = lname.Trim().ToLower();

				_availableStudents.RemoveAll(x => !x.Lastname.ToLower().Contains(lname));
			}
		}

		private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TxtSearch.Text)) {
				_availableStudents = _allStudents.ToList();
			}
			RebindUsers();
		}

		private async void ZolikyAsignment_OnClosing(object sender, CancelEventArgs e)
		{
			if (this.Owner is MainWindow mw && Changed) {
				await mw.RefreshStatsAsync();
			}
		}

		private void RbJokerJoker_OnChecked(object sender, RoutedEventArgs e)
		{
			ChBoxSplit.Visibility = RbJokerJoker.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
		}
	}
}