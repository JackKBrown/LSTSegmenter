using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace lewisstupidthingy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string DRAG_SYMBOL = "☰  ";
		private string BASE_COLOUR = "#FFFFFF";
		private string ACCENT_COLOUR = "#DDDDDD";
		public ObservableCollection<Segment> SegmentOptions { get; set; } = new ObservableCollection<Segment>();
		public List<ListBoxItem> Document { get; set; } = new List<ListBoxItem>();
		FileIO fileIO = new FileIO();
		BrushConverter bc = new BrushConverter();

		public MainWindow()
		{
			InitializeComponent();
			DocOptions.ItemsSource = SegmentOptions;
			DocOptions.MouseDoubleClick += new MouseButtonEventHandler(AddSegment_Click);

			LoadDoctions();

		}

		private void LoadDoctions()
		{
			SegmentOptions.Clear();

			List<string> rawSegments = loadFile();

			foreach (var rs in rawSegments)
			{
				SegmentOptions.Add(new Segment(rs));
				Console.WriteLine(rs);
			}
		}

		private List<string> loadFile()
		{
			string path = "segments.txt";
			if (!File.Exists(path))
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.FileName = "Document"; // Default file name
				dlg.DefaultExt = ".txt"; // Default file extension
				dlg.Filter = "Text file (.txt) |*.txt"; // Filter files by extension
				Nullable<bool> result = dlg.ShowDialog();

				// Process save file dialog box results
				if (result == true)
				{
					path = dlg.FileName;
				}
				else
				{
					MessageBox.Show("need to select a configuration file to use this application");
					Close();
				}
			}

			return fileIO.parseInput(path);
		}


		private void saveFile(string[] lines)
		{
			// Configure save file dialog box
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = "Document"; // Default file name
			dlg.DefaultExt = ".txt"; // Default file extension
			dlg.AddExtension = true; //always add an extension
			dlg.Filter = "Text file (.txt) |*.txt|comma seperated value (.csv)|*.csv"; // Filter files by extension

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process save file dialog box results
			if (result == true)
			{
				// Save document
				string filename = dlg.FileName;
				Console.WriteLine(filename);
				fileIO.WriteOutput(filename, lines);
			}
		}

		private string[] ParseDocument()
		{
			List<string> lines = new List<string>();
			int j = 0;
			foreach(ListBoxItem lbi in Document)
			{
				try
				{
					string line = "";
					StackPanel panel = lbi.Content as StackPanel;
					//check the panel resources to see if it should be numbered or not
					if (panel.FindResource("numbered").ToString() == "true")
					{
						j++;
						line += j.ToString() + ". ";
					}
					foreach (UIElement element in panel.Children)
					{
						if(element.GetType() == typeof(ComboBox))
						{
							ComboBox combo = element as ComboBox;
							if (combo.SelectedIndex != -1) line += combo.SelectedItem.ToString();
						}
						else if (element.GetType() == typeof(TextBox))
						{
							TextBox tb = element as TextBox;
							line += tb.Text;
						}
						else if (element.GetType() == typeof(TextBlock))
						{
							TextBlock tb = element as TextBlock;
							line += tb.Text;
						}
						else
						{
							MessageBox.Show("error parsing the document to save");
							return new string[0];
						}
					}
					lines.Add(line.Replace(DRAG_SYMBOL, ""));
				}
				catch(Exception exc)
				{
					MessageBox.Show("shit there was an error, good luck mate you're on your own");
					Console.WriteLine(exc.Message);
				}
			}

			return lines.ToArray();
		}

		private ListBoxItem ParseSegment(Segment seg)
		{
			ListBoxItem newItem = new ListBoxItem();
			//drag properties
			newItem.AllowDrop = true;
			newItem.Drop += new DragEventHandler(DocItem_Drop);
			newItem.DragEnter += new DragEventHandler(DocItem_DragEnter);
			newItem.DragLeave += new DragEventHandler(DocItem_DragLeave);

			//containing stackpanel
			StackPanel stackPanel = new StackPanel();
			stackPanel.Height = 24;

			//drag symbol
			TextBlock dragBlock = new TextBlock();
			dragBlock.Text = DRAG_SYMBOL;
			dragBlock.MouseMove += new MouseEventHandler(DocItem_MouseMove);
			stackPanel.Children.Add(dragBlock);

			//numbering item
			if (seg.numbered)
			{
				stackPanel.Resources.Add("numbered", "true");
			}
			else
			{
				stackPanel.Resources.Add("numbered", "false");
			}

			stackPanel.Orientation = Orientation.Horizontal;
			foreach(string subsegment in seg.subSegments)
			{
				//skip empty subsegments
				if (subsegment.Length == 0) continue;
				
				//subsegment is a dropdown box
				if (subsegment[0] == '@')
				{
					string[] items = subsegment.Split(new char[] { '@', ',' });
					List<string> comboOptions = new List<string>();
					for(int i = 0; i< items.Length; i++)
					{
						if (string.IsNullOrWhiteSpace(items[i])) continue;
						comboOptions.Add(items[i].Trim());
					}
					ComboBox comboBox = new ComboBox();
					comboBox.ItemsSource = comboOptions;
					stackPanel.Children.Add(comboBox);
				}
				//subsegment is a textblock
				else if (subsegment[0] == '~')
				{
					TextBox textBox = new TextBox();
					textBox.MinWidth = 30;
					textBox.Text = subsegment.Replace("~", ""); ;
					stackPanel.Children.Add(textBox);
				}
				//default to textblock
				else
				{
					TextBlock textBlock = new TextBlock();
					textBlock.Text = subsegment;
					stackPanel.Children.Add(textBlock);
				}
			}
			newItem.MouseRightButtonUp += new MouseButtonEventHandler(DocumentContentRemove_Click);
			newItem.Content = stackPanel;
			
			return newItem;
		}

		private void renderDocument()
		{
			documentContent.Items.Clear();
			for(int i = 0; i<Document.Count; ++i)
			{
				if (i % 2 == 0) Document[i].Background = (Brush)bc.ConvertFrom(ACCENT_COLOUR);
				else Document[i].Background = (Brush)bc.ConvertFrom(BASE_COLOUR);
				documentContent.Items.Add(Document[i]);
			}
		}

		//========================================================================
		//user interaction functions
		//========================================================================

		private void AddSegment_Click(object sender, RoutedEventArgs e)
		{
			//no segment selected return nothing
			if (DocOptions.SelectedIndex == -1) return;

			ListBoxItem newItem = ParseSegment(DocOptions.SelectedItem as Segment);
			Document.Add(newItem);
			if (!(Document.Count%2==0)) newItem.Background = (Brush)bc.ConvertFrom(ACCENT_COLOUR);
			//don't need to rerender here can just add item to the list
			documentContent.Items.Add(newItem);
		}

		private void DocItem_MouseMove(object sender, MouseEventArgs e)
		{
			TextBlock tb = sender as TextBlock;
			ListBoxItem lbi = LogicalTreeHelper.GetParent(LogicalTreeHelper.GetParent(tb)) as ListBoxItem;
			
			if (lbi != null && e.LeftButton == MouseButtonState.Pressed)
			{
				int index = Document.IndexOf(lbi);
				DragDrop.DoDragDrop(lbi, index.ToString(), DragDropEffects.Move);
			}
		}

		private void DocItem_DragEnter(object sender, DragEventArgs e)
		{
			ListBoxItem lbi = sender as ListBoxItem;
		}

		private void DocItem_DragLeave(object sender, DragEventArgs e)
		{
			ListBoxItem lbi = sender as ListBoxItem;
		}

		private void DocItem_Drop(object sender, DragEventArgs e)
		{
			try
			{
				int indexDragged = int.Parse(e.Data.GetData(DataFormats.StringFormat) as string);
				ListBoxItem lbiDragged = Document[indexDragged];
				ListBoxItem lbiDropped = sender as ListBoxItem;
				int indexDropped = Document.IndexOf(lbiDropped);
				if (indexDragged > indexDropped)
				{
					Document.RemoveAt(indexDragged);
					Document.Insert(indexDropped + 1, lbiDragged);
				}
				else if (indexDragged < indexDropped)
				{
					Document.Insert(indexDropped + 1, lbiDragged);
					Document.RemoveAt(indexDragged);
				}
				renderDocument();
			}
			catch(Exception exc)
			{
				Console.WriteLine(exc.Message);
				Console.WriteLine("do not recognise");
			}
		}

		private void DocumentContentRemove_Click(object sender, MouseButtonEventArgs e)
		{
			TextBox tb = e.OriginalSource as TextBox;
			Console.WriteLine(tb==null);
			Console.WriteLine(e.OriginalSource.GetType());
			Console.WriteLine(typeof(TextBox));
			//this is clearly not the way this should be done but I'm really struggling to find any documentation so fuck this shit gonna come back and do it correctly later
			//TODO fix this when it isn't 2am 
			if (e.OriginalSource.GetType().ToString() == "System.Windows.Controls.TextBoxView")
			{
				Console.WriteLine("textbox");
				return;
			}
			try
			{
				Document.Remove(sender as ListBoxItem);
				documentContent.Items.Remove(sender);
				renderDocument();
			}
			catch(Exception exc)
			{
				Console.WriteLine("error trying to remove object");
				Console.WriteLine(exc.Message);
				MessageBox.Show("shit there was an error");
			}

		}

		//==========================================================================
		//COMMANDS and MENU
		//==========================================================================

		private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			//don't really need this 
			e.CanExecute = true;
		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Document.Clear();
			documentContent.Items.Clear();
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string[] lines = ParseDocument();
			saveFile(lines);
		}

		private void Clipboard_Click(object sender, RoutedEventArgs e)
		{
			string[] lines = ParseDocument();
			string concatLines = "";
			foreach(string line in lines)
			{
				concatLines += line + "\n";
			}
			System.Windows.Clipboard.SetText(concatLines);
		}

		private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Document.Count != 0;
		}

		private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string[] lines = ParseDocument();
			string concatLines = "";
			foreach (string line in lines)
			{
				concatLines += line + "\n";
			}
			System.Windows.Clipboard.SetText(concatLines);
		}

		private void ReloadClick(object sender, RoutedEventArgs e)
		{
			LoadDoctions();
		}
	}
}
