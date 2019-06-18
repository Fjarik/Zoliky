using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.AspNet.Identity;
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;
using Xceed.Wpf.AvalonDock.Themes;

namespace ZolikDistributor.Admin
{
	/// <summary>
	/// Interaction logic for RemoverNew.xaml
	/// </summary>
	public partial class RemoverNew : MetroWindow
	{
		private Random _rnd = new Random();
		private bool Changed { get; set; }
		private User LoggedUser { get; set; }
		private Grid[] Gridiky { get; set; }
		private int SelectedClassId { get; set; }
		private IStudent SelectedStudent { get; set; }
		private Action PreviousStep { get; set; }
		private List<IZolik> Zoliks { get; set; }
		private IZolik SelectedZolik { get; set; }
		private bool BackToStepFour { get; set; } = false;

		private string[] Types { get; set; } = {
			"After Effects",
			"Audition",
			"Cinema 4D",
			"Illustrator",
			"InDesign",
			"Photoshop",
			"Premiere",
		};

		private string[] Mos { get; set; } = {
			"Počítačová grafika",
			"Digitální fotografie",
			"Digitální video",
			"Kompozice",
			"Grafické formáty",
		};

#region Init

		public RemoverNew(User user)
		{
			this.Changed = false;
			this.LoggedUser = user;
			InitializeComponent();
		}

		private void RemoverNew_OnInitialized(object sender, EventArgs e)
		{
			this.Gridiky = new[] {
				GridLoading,
				GridClasses,
				GridGroups,
				GridStudents,
				GridZoliks,
				GridZolikDetail
			};
			foreach (var subject in Globals.Subjects) {
				var tag = subject.Shortcut == "Jiný" ? -1 : subject.ID;
				var item = new ComboBoxItem() {
					Content = subject.Shortcut,
					ToolTip = subject.Name,
					Tag = tag,
				};
				CmBoxSubjects.Items.Add(item);
			}
			foreach (var type in Types) {
				var item = new ComboBoxItem() {
					Content = type,
					Tag = type,
				};
				CmBoxTypes.Items.Add(item);
			}
			foreach (var mo in Mos) {
				var item = new ComboBoxItem() {
					Content = mo,
					Tag = mo,
				};
				CmBoxMo.Items.Add(item);
			}
			for (int i = 1; i <= 12; i++) {
				var value = $"M{i}";
				var item = new ComboBoxItem() {
					Content = value,
					Tag = value
				};
				CmBoxM.Items.Add(item);
			}
			CmBoxTypes.Items.Add(GetOtherCmBoxItem());
			CmBoxM.Items.Add(GetOtherCmBoxItem());
			CmBoxMo.Items.Add(GetOtherCmBoxItem());
			Select(CmBoxSubjects, Globals.Subjects.First().Shortcut);
			StepOne();
		}

		private ComboBoxItem GetOtherCmBoxItem() => new ComboBoxItem() {
			Content = "Jiné",
			Tag = -1,
			IsSelected = true
		};

#endregion

#region Events

		private void TileClass_OnClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Tile t) || !(t.Tag is int id)) {
				MessageBox.Show("Neplatná třída");
				return;
			}
			SelectedClassId = id;
			StepTwo();
		}

		private void ToggleStudentsWithZoliks_OnChecked(object sender, RoutedEventArgs e)
		{
			if (this.IsInitialized) {
				StepThree();
			}
		}

		private async void TileStudent_OnClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Tile t) || !(t.Tag is int id)) {
				MessageBox.Show("Neplatný student");
				return;
			}
			SelectedStudent = Globals.Students.FirstOrDefault(x => x.ID == id);
			if (SelectedStudent == null) {
				MessageBox.Show("Neplatný student");
				return;
			}
			await StepFourAsync();
		}

		private void TileZolik_OnClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Tile t) || !(t.Tag is int id)) {
				MessageBox.Show("Neplatný žolík");
				return;
			}
			SelectedZolik = this.Zoliks.FirstOrDefault(x => x.ID == id);
			if (SelectedZolik == null) {
				MessageBox.Show("Neplatný žolík");
				return;
			}
			StepFive();
		}

		private void RbTypes_OnChecked(object sender, RoutedEventArgs e)
		{
			RbCheckedChanged();
		}

		private void RbCheckedChanged()
		{
			if (!this.IsInitialized) {
				return;
			}
			if (RbOther.IsChecked == true) {
				TextBoxHelper.SetWatermark(TxtReason,
										   "Specifikujte důvod (min. 5 znaků)\n\nNapř.: Prohra sázky, špatná nálada, podvod v úkolu, ...");
			}
			if ((RbHomework.IsChecked == true || RbTest.IsChecked == true) && CmBoxesValid()) {
				return;
			}

			var i = _rnd.Next(1, 13);
			if (RbHomework.IsChecked == true) {
				TextBoxHelper.SetWatermark(TxtReason,
										   $"Specifikujte důvod (min. 2 znaky)\n\nNapř.: Úkol M{i}");
			}
			if (RbTest.IsChecked == true) {
				TextBoxHelper.SetWatermark(TxtReason,
										   $"Specifikujte důvod (min. 2 znaky)\n\nNapř.: Test M{i}");
			}
			CmBoxM.Visibility = Visibility.Visible;
			CmBoxTypes.Visibility = Visibility.Visible;
			CmBoxMo.Visibility = Visibility.Hidden;
			TxtReason.Opacity = 1;
			if (RbMo.IsChecked == true || RbOther.IsChecked == true) {
				SelectOther(CmBoxM);
				SelectOther(CmBoxTypes);
				CmBoxM.Visibility = Visibility.Hidden;
				CmBoxTypes.Visibility = Visibility.Hidden;
				if (RbMo.IsChecked == true) {
					SelectOther(CmBoxMo);
					CmBoxMo.Visibility = Visibility.Visible;
					TextBoxHelper.SetWatermark(TxtReason, "Vyplňte název maturitní otázky");
				}
			}
		}

		private void CmBoxMo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CheckCmBoxes(cmBoxes: CmBoxMo);
		}

		private void CmBoxTypes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CheckCmBoxes();
		}

		private void CheckCmBoxes(string watermark = "Prostor pro případnou poznámku")
		{
			CheckCmBoxes(watermark, CmBoxM, CmBoxTypes, CmBoxSubjects);
		}

		private void CheckCmBoxes(string watermark = "Prostor pro případnou poznámku", params ComboBox[] cmBoxes)
		{
			TxtReason.Opacity = 1;
			foreach (var box in cmBoxes) {
				if (!CmBoxValid(box)) {
					RbCheckedChanged();
					return;
				}
			}
			TextBoxHelper.SetWatermark(TxtReason, watermark);
			TxtReason.Opacity = 0.55;
		}

		private bool CmBoxesValid()
		{
			var cmBoxes = new[] {CmBoxM, CmBoxTypes, CmBoxSubjects};
			return cmBoxes.All(CmBoxValid);
		}

		private bool CmBoxValid(ComboBox box)
		{
			return !(box.SelectedItem is ComboBoxItem m && m.Tag is int id && id == -1);
		}

		private async void BtnRemove_Click(object sender, RoutedEventArgs e)
		{
			await StepSixAsync();
		}

		private async void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
		{
			await StepFourAsync();
		}

		private async void BtnRefreshStudents_OnClick(object sender, RoutedEventArgs e)
		{
			await RefreshStudentsAsync();
		}

		private void BtnInfo_OnClick(object sender, RoutedEventArgs e)
		{
			if (SelectedZolik == null) {
				return;
			}
			ZolikTransactions trans = new ZolikTransactions(LoggedUser, SelectedZolik);
			trans.ShowDialog();
		}

#endregion

#region Steps

		private void StepOne()
		{
			ShowGrid(GridLoading);
			TilesClasses.Children.Clear();
			foreach (var c in Globals.Classes) {
				var title = $"{c.Since.Year} - {c.Graduation.Year}";
				if (c.ID == 0) {
					title = "";
				}
				var tooltip = "Kliknutím vyberete třídu";
				var enabled = c.Enabled;
				if (!enabled) {
					tooltip = "Tato třída není aktivní";
				}
				if (!Globals.Students.Any(x => x.ClassID == c.ID && x.ZolikCount > 0)) {
					enabled = false;
					tooltip = "V této třídě nemá nikdo žolíka";
				}
				var tile = new Tile {
					Visibility = Visibility.Visible,
					Count = c.Name,
					Title = title,
					IsEnabled = true,
					ClickMode = ClickMode.Release,
					Cursor = enabled ? Cursors.Hand : Cursors.Help,
					Tag = c.ID,
					ToolTip = tooltip,
					Opacity = enabled ? 1 : 0.55
				};
				tile.Click += TileClass_OnClick;
				TilesClasses.Children.Add(tile);
			}
			this.PreviousStep = null;
			ShowGrid(GridClasses);
		}

		private void StepTwo()
		{
			if (PreviousStep == StepTwo) {
				StepOne();
				return;
			}
			this.PreviousStep = StepOne;
			//if (!SelectedClass.hasGroups) {

			StepThree();
			//}
			/*
			this.ShowGrid(GridSecond);
			 
			 
			foreach (var c in Globals.Classes) {
				var tile = new Tile {
					Visibility = Visibility.Visible,
					Count = c.Name,
					Title = "",
					IsEnabled = true,
					ClickMode = ClickMode.Release
				};
				tile.Click += TileClass_OnClick;
				TilesGroups.Children.Add(tile);
			}*/
		}

		private void StepThree()
		{
			this.PreviousStep = StepTwo;

			TilesStudents.Children.Clear();
			LblClass.Content = Globals.Classes.FirstOrDefault(x => x.ID == SelectedClassId)?.Name ?? "0.A";
			var students = Globals.Students.Where(x => x.ClassID == SelectedClassId);
			if (ChBoxStudentsWithZoliks.IsChecked == true) {
				students = students.Where(x => x.ZolikCount > 0);
			}

			foreach (var student in students) {
				var tile = new Tile {
					Visibility = Visibility.Visible,
					Count = student.ZolikCount.ToString(),
					Title = student.FullName,
					IsEnabled = true,
					Opacity = student.ZolikCount > 0 ? 1 : 0.55,
					ClickMode = ClickMode.Release,
					Tag = student.ID,
					Cursor = Cursors.Hand,
					ToolTip = "Kliknutím vyberete studenta"
				};
				tile.Click += TileStudent_OnClick;
				TilesStudents.Children.Add(tile);
			}

			this.ShowGrid(GridStudents);
		}

		private async Task StepFourAsync()
		{
			ShowGrid(GridLoading);
			this.PreviousStep = StepThree;
			this.BackToStepFour = false;
			LblStudentName.Content = SelectedStudent.FullName;
			var zc = new ZolikConnector(LoggedUser.Token);
			var res = await zc.GetUserZoliksAsync(SelectedStudent.ID, false);
			this.Zoliks = res.ToList<IZolik>();
			TilesZoliks.Children.Clear();
			foreach (var zolik in res) {
				var title = zolik.Title;
				if (title.Length > 22) {
					title = title.Substring(0, 22);
					title += "...";
				}
				var tile = new Tile {
					Visibility = Visibility.Visible,
					Count = zolik.Type.GetDescription().First().ToString(),
					Title = title,
					IsEnabled = zolik.Enabled,
					ClickMode = ClickMode.Release,
					Cursor = Cursors.Hand,
					Tag = zolik.ID,
					Opacity = zolik.IsLocked ? 0.75 : 1
				};
				tile.Click += TileZolik_OnClick;
				TilesZoliks.Children.Add(tile);
			}
			await Task.Delay(300);
			ShowGrid(GridZoliks);
		}

		private void StepFive()
		{
			this.BackToStepFour = true;

			LblTitle.Content = SelectedZolik.Title;
			LblType.Content = SelectedZolik.Type.GetDescription();
			LblDate.Content = SelectedZolik.OwnerSince.ToString("dd.MM.yyyy");
			LblTitle2.Content = SelectedZolik.Title;
			LblSubject.Content = Globals.Subjects?
									 .FirstOrDefault(x => x.ID == SelectedZolik.SubjectID)?
									 .Name ??
								 "Počítačová grafika";
			var originalOwner = Globals.Students?
									.FirstOrDefault(x => x.ID == SelectedZolik.OriginalOwnerID)?
									.FullName ??
								"Neznámý";
			LblOriginalOwner.Content = originalOwner;

			TxtReason.Text = "";
			TxtReason.ToolTip = null;
			TxtReason.Opacity = 1;
			SelectOther(CmBoxM);
			if (this.SelectedZolik.IsLocked) {
				var l = this.SelectedZolik.Lock.Trim();
				var lLower = l.ToLower();
				if (lLower.Contains("test")) {
					RbTest.IsChecked = true;
				} else if (lLower.Contains("úkol") || lLower.Contains("ukol")) {
					RbHomework.IsChecked = true;
				} else {
					RbOther.IsChecked = true;
				}
				SelectOther(CmBoxTypes);
				for (int i = 1; i <= 12; i++) {
					var type = $"m{i}";
					if (lLower.Contains(type)) {
						Select(CmBoxM, type);
					}
				}
				TxtReason.Text = l;
				TxtReason.ToolTip = "Student si přeje žolíka použít zadanou známku";
				TxtReason.Opacity = 0.75;
			}

			RbMo.Visibility = Visibility.Hidden;
			if (SelectedZolik.Type == ZolikType.Joker) {
				RbMo.Visibility = Visibility.Visible;
			}

			ShowGrid(GridZolikDetail);
		}

		private async Task StepSixAsync()
		{
			if (RbOther.IsChecked == true) {
				if (!(await CheckReasonAsync())) {
					return;
				}
			}

			// Opacity == 1 (+-0.1)
			if (Math.Abs(TxtReason.Opacity - 1) < 0.1 && !(await CheckReasonAsync())) {
				return;
			}
			ShowGrid(GridLoading);
			var msg = "Odebráno za použití na ";
			if (RbTest.IsChecked == true) {
				msg += "test";
			} else if (RbHomework.IsChecked == true) {
				msg += "úkol";
			} else if (RbMo.IsChecked == true) {
				msg += "maturitní otázku ";
			} else {
				msg += "něco jiného";
			}
			msg += ". ";

			if (RbMo.IsChecked == true) {
				if (CmBoxMo.SelectedItem is ComboBoxItem moI) {
					msg += $"{moI.Content}; ";
				}
			} else {
				if (CmBoxM.SelectedItem is ComboBoxItem mI) {
					msg += $"{mI.Content}; ";
				}
				if (CmBoxTypes.SelectedItem is ComboBoxItem typeI) {
					msg += $"Program: {typeI.Content}; ";
				}
			}

			if (CmBoxSubjects.SelectedItem is ComboBoxItem subjectI) {
				msg += $"Předmět: {subjectI.Content}; ";
			}

			if (!string.IsNullOrWhiteSpace(TxtReason.Text)) {
				msg += $"Poznámka: {TxtReason.Text}";
			}

			var zc = new ZolikConnector(LoggedUser.Token);
			var zp = new ZolikPackage() {
				FromID = SelectedStudent.ID,
				ToID = LoggedUser.ID,
				ZolikID = SelectedZolik.ID,
				Type = TransactionAssignment.ZerziRemoval,
				Message = msg
			};
			var res = await zc.TransferAsync(zp);

			if (!res.IsSuccess) {
				await this.ShowMessageAsync("Chyba", $"Něco se nepovedlo! Chyba: {res.GetStatusMessage()}");
				return;
			}

			await this.ShowMessageAsync("Úspěch", "Žolík byl úspěšně odebrán");
			this.Changed = true;
			Globals.ZolikCountMinus(SelectedStudent.ID);
			await StepFourAsync();
		}

		private Task ShowMessageAsync(string title, string msg)
		{
			var set = new MetroDialogSettings() {
				AffirmativeButtonText = "Ok",
				AnimateHide = false,
				AnimateShow = false,
				OwnerCanCloseWithDialog = false,
				ColorScheme = MetroDialogOptions.ColorScheme
			};
			return this.ShowMessageAsync(title, msg, settings: set);
		}

		private async Task<bool> CheckReasonAsync()
		{
			var reason = TxtReason?.Text?.Trim();
			if (string.IsNullOrWhiteSpace(reason) || reason.Length < 2) {
				await this.ShowMessageAsync("Chyba", "Specifikujte blíže důvod odebrání žolíka!");
				return false;
			}
			return true;
		}

		private async Task RefreshStudentsAsync()
		{
			await Globals.RefreshStudentsAsync(LoggedUser.Token, SelectedClassId);
			StepThree();
		}

#endregion

#region Back, ShowGrid, SelectOther

		private static void SelectOther(ComboBox cmBox)
		{
			Select(cmBox, "Jiné");
		}

		private static void Select(ComboBox cmBox, string value)
		{
			foreach (var item in cmBox.Items) {
				if (item is ComboBoxItem i &&
					string.Equals(i.Content.ToString(), value, StringComparison.CurrentCultureIgnoreCase)) {
					i.IsSelected = true;
				}
			}
		}

		private async void BtnBack_OnClick(object sender, RoutedEventArgs e)
		{
			if (BackToStepFour) {
				await StepFourAsync();
				return;
			}

			PreviousStep?.Invoke();
		}

		private void ShowGrid(Grid showGrid)
		{
			foreach (var gridik in Gridiky) {
				gridik.Visibility = Visibility.Hidden;
			}

			showGrid.Visibility = Visibility.Visible;
		}

#endregion

#region Closing

		private async void RemoverNew_OnClosing(object sender, CancelEventArgs e)
		{
			if (Owner != null) {
				Owner.Show();
				Owner.WindowState = WindowState.Normal;
				Owner.Focus();
			}
			if (this.Owner is MainWindow mw && Changed) {
				await mw.RefreshStatsAsync();
			}
		}

		private void RemoverNew_OnDeactivated(object sender, EventArgs e)
		{
			if (sender is Window w) {
				w.Topmost = true;
			}
		}

#endregion
	}
}