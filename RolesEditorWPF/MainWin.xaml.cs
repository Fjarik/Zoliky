using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess.Models;
using MahApps.Metro.Controls;

namespace RolesEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWin : MetroWindow
	{
		private readonly IList<CheckBox> _checkBoxes;

		public MainWin()
		{
			InitializeComponent();
			_checkBoxes = new List<CheckBox>() {
				ChBoxTeacher, ChBoxAdmin,
				ChBoxStudent, ChBoxTester,
				ChBoxPublic, ChBoxStudentFake,
				ChBoxSupport, ChBoxStudentHidden,
				ChBoxRobot, ChBoxDeveloper
			};
			BtnLoadUsers_Click(null, new RoutedEventArgs());
		}

		private void BtnLoadUsers_Click(object sender, RoutedEventArgs e)
		{
			ListUsers.Items.Clear();
			using (ZoliksEntities ent = new ZoliksEntities()) {
				foreach (var user in ent.Users
										.Include(x => x.Class)
										.Include(x => x.Roles)
										.OrderBy(x => x.Class.Name)
										.ThenBy(x => x.Name)) {
					ListUsers.Items.Add(new ListBoxItem() {
						Content = $"{user.FullName} ({user.Class?.Name})",
						Tag = user.ID
					});
				}
			}
			foreach (var checkBox in _checkBoxes) {
				checkBox.IsChecked = false;
			}
		}

		private async void ListUsers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ListUsers.SelectedIndex < 0 && ListUsers.SelectedItem == null) {
				return;
			}
			if (!(ListUsers.SelectedItem is ListBoxItem item) || !(item.Tag is int id)) {
				return;
			}
			using (ZoliksEntities ent = new ZoliksEntities()) {
				var u = await ent.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.ID == id);
				if (u == null) {
					return;
				}

				foreach (CheckBox checkBox in _checkBoxes) {
					if (int.TryParse(checkBox.Tag.ToString(), out int cid) && u.Roles.Any(x => x.ID == cid)) {
						checkBox.IsChecked = true;
					} else {
						checkBox.IsChecked = false;
					}
				}
			}
		}

		private async void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (ListUsers.SelectedIndex < 0 && ListUsers.SelectedItem == null) {
				return;
			}
			if (!(ListUsers.SelectedItem is ListBoxItem item) || !(item.Tag is int id)) {
				return;
			}

			IList<int> checkedIds = _checkBoxes.Where(x => x.IsChecked == true)
											   .Select(x => int.Parse(x.Tag.ToString()))
											   .ToList();
			IList<int> notCheckedIds = _checkBoxes.Where(x => x.IsChecked == false)
												  .Select(x => int.Parse(x.Tag.ToString()))
												  .ToList();

			using (ZoliksEntities ent = new ZoliksEntities()) {
				var u = await ent.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.ID == id);
				if (u == null) {
					return;
				}

				foreach (var roleId in notCheckedIds) {
					if (u.Roles.Any(x => x.ID == roleId)) {
						var role = ent.Roles.Find(roleId);
						u.Roles.Remove(role);
						ent.Entry(u).State = EntityState.Modified;
					}
				}

				foreach (int roleId in checkedIds) {
					if (u.Roles.All(x => x.ID != roleId)) {
						var role = await ent.Roles.FindAsync(roleId);
						u.Roles.Add(role);
						ent.Entry(u).State = EntityState.Modified;
					}
				}
				await ent.SaveChangesAsync();
			}
			BtnLoadUsers_Click(sender, e);
		}
	}
}