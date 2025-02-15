﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Text_Grab.Properties;
using Text_Grab.Utilities;

namespace Text_Grab.Controls
{
    /// <summary>
    /// Interaction logic for WordBorder.xaml
    /// </summary>
    public partial class WordBorder : UserControl
    {
        public bool IsSelected { get; set; } = false;

        public bool WasRegionSelected { get; set; } = false;

        public string Word { get; set; } = "";

        public int LineNumber { get; set; } = 0;

        public bool IsFromEditWindow { get; set; } = false;

        public WordBorder()
        {
            InitializeComponent();
        }

        public void Select()
        {
            IsSelected = true;
            this.BorderBrush = new SolidColorBrush(Colors.Yellow);
        }

        public void Deselect()
        {
            IsSelected = false;
            this.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 48, 142, 152));
        }

        private void WordBorderControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsSelected)
                Deselect();
            else
                Select();
        }

        private async void WordBorderControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Word);

            if (Settings.Default.ShowToast
                && IsFromEditWindow == false)
                NotificationUtilities.ShowToast(Word);

            if (IsFromEditWindow == true)
                WindowUtilities.AddTextToOpenWindow(Word);

            if (IsSelected)
            {
                await Task.Delay(100);
                Deselect();
            }
            else
            {
                await Task.Delay(100);
                Select();
            }
        }
    }
}
