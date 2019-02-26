using System;
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
    /// Interaction logic for ImagesWindow.xaml
    /// </summary>
    public partial class ImagesWindow : Window
    {
        private Album album;

        public ImagesWindow(Album album)
        {
            InitializeComponent();
            this.album = album;
            ListBoxForPhotos.ItemsSource = album.Images;
        }

        private void CommandBindingDeletePhoto_Executed(object sender, ExecutedRoutedEventArgs e)
        {           
            int indexToBeDeleted = ListBoxForPhotos.SelectedIndex;
            MessageBoxResult result;

            if (indexToBeDeleted >= 0)
            {
                result = MessageBox.Show("Are you sure you want to delete this photo?", "Sure?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    album.Images.RemoveAt(indexToBeDeleted);
                    updateListBoxForPhotos();                 
                }
            }
            if (album.Images.Count == 0)
            {
                this.Close();
            }
        }     

        private void CommandBindingDeletePhoto_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void updateListBoxForPhotos()
        {
            ListBoxForPhotos.ItemsSource = album.Images;
            ListBoxForPhotos.Items.Refresh();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
            PhotoDisplay photoDisplay = new PhotoDisplay(album, ListBoxForPhotos.SelectedIndex);
            photoDisplay.ShowDialog();
        }        
    }
}
