﻿using System;
using System.Collections.Generic;
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

namespace PhotoAlbum
{
    /// <summary>
    /// Interaction logic for AlbumNameInputDialog.xaml
    /// </summary>
    public partial class AlbumNameInputDialog : Window
    {
        public AlbumNameInputDialog()
        {
            InitializeComponent();                    
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            textAnswer.SelectAll();
            textAnswer.Focus();
        }

        private void dialogOkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get { return textAnswer.Text; }
        }
        
    }
}
