﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lewisstupidthingy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Segment> segmentOptions = new List<Segment>();
		List<ListBoxItem> document = new List<ListBoxItem>();
		FileIO fileIO = new FileIO();

		public MainWindow()
		{
			InitializeComponent();
			DocOptions.ItemsSource = segmentOptions;
			DocOptions.MouseDoubleClick += new MouseButtonEventHandler(AddSegment_Click);
			//DocContents.ItemsSource = document;

			//read the config file

			//string segName1 = "sample segment";
			//string sampleSeg1 = "go to [@planet a, planet b] and do [@thing a, thing b]";

			Dictionary<string, string> rawSegments = loadFile();

			foreach(var kvp in rawSegments)
			{
				segmentOptions.Add(new Segment(kvp.Key, kvp.Value));
			}
		}

		private Dictionary<string, string> loadFile()
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
			foreach(ListBoxItem lbi in document)
			{
				try
				{
					j++;
					string line = j.ToString() + ". ";
					StackPanel panel = lbi.Content as StackPanel;
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
					lines.Add(line);
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
			StackPanel stackPanel = new StackPanel();
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
					comboBox.ItemsSource = items;
					stackPanel.Children.Add(comboBox);
				}
				//subsegment is a textblock
				else if (subsegment[0] == '~')
				{
					TextBox textBox = new TextBox();
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
			newItem.Content = stackPanel;
			newItem.MouseDoubleClick += new MouseButtonEventHandler(documentContent_DoubleClick);
			return newItem;
		}

		private void AddSegment_Click(object sender, RoutedEventArgs e)
		{
			//no segment selected return nothing
			if (DocOptions.SelectedIndex == -1) return;

			ListBoxItem newItem = ParseSegment(DocOptions.SelectedItem as Segment);
			document.Add(newItem);
			documentContent.Items.Add(newItem);
			//documentContents.Children.Add(stackPanel);
		}

		private void documentContent_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				document.Remove(sender as ListBoxItem);
				documentContent.Items.Remove(sender);
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
			//TODO
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
	}
}
