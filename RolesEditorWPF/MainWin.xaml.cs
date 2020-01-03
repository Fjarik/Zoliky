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
using MahApps.Metro.Controls.Dialogs;
using SharedLibrary.Shared;

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
				ChBoxRobot, ChBoxDeveloper,
				ChBoxManager
			};
		}

		private async void BtnLoadUsers_Click(object sender, RoutedEventArgs e)
		{
			await LoadStudents();
		}

		private async Task LoadStudents()
		{
			var controller = await this.ShowProgressAsync("Prosíme, čekejte", "Probíhá načítání");
			ListUsers.Items.Clear();
			using (var ent = new ZoliksEntities()) {
				var users = await ent.Users
									 .Include(x => x.Class)
									 .Include(x => x.Roles)
									 .Where(x => x.ClassID == null ||
												 x.Class.Enabled)
									 .OrderBy(x => x.Class.Name)
									 .ThenBy(x => x.Name)
									 .ToListAsync();
				foreach (var user in users) {
					var school = user.SchoolName.Substring(0, 5);
					var color = Brushes.Black;
					if (user.IsInRole(UserRoles.Tester)) {
						color = Brushes.Green;
					}
					if (!user.IsEnabled) {
						color = Brushes.Red;
					}
					var cl = "";
					if (user.Class != null) {
						cl = $"({user.ClassName})";
					}
					ListUsers.Items.Add(new ListBoxItem() {
						Content = $"{user.FullName} - {school} {cl}",
						Tag = user.ID,
						Foreground = color
					});
				}
			}
			foreach (var checkBox in _checkBoxes) {
				checkBox.IsChecked = false;
			}
			await controller.CloseAsync();
		}

		private async void ListUsers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ListUsers.SelectedIndex < 0 && ListUsers.SelectedItem == null) {
				return;
			}
			if (!(ListUsers.SelectedItem is ListBoxItem item) || !(item.Tag is int id)) {
				return;
			}
			using (var ent = new ZoliksEntities()) {
				var u = await ent.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.ID == id);
				if (u == null) {
					return;
				}

				foreach (var checkBox in _checkBoxes) {
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

			var controller = await this.ShowProgressAsync("Prosíme, čekejte", "Probíhá ukládání", false);
			IList<int> checkedIds = _checkBoxes.Where(x => x.IsChecked == true)
											   .Select(x => int.Parse(x.Tag.ToString()))
											   .ToList();
			IList<int> notCheckedIds = _checkBoxes.Where(x => x.IsChecked == false)
												  .Select(x => int.Parse(x.Tag.ToString()))
												  .ToList();

			using (var ent = new ZoliksEntities()) {
				var u = await ent.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.ID == id);
				if (u == null) {
					await controller.CloseAsync();
					return;
				}

				foreach (var roleId in notCheckedIds) {
					if (u.Roles.Any(x => x.ID == roleId)) {
						var role = await ent.Roles.FindAsync(roleId);
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
			await controller.CloseAsync();
			await LoadStudents();
		}

		private async void MainWin_OnLoaded(object sender, RoutedEventArgs e)
		{
			await LoadStudents();
		}
	}
}