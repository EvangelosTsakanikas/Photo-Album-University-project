using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PhotoAlbum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Album> albums = new List<Album>();
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private int albumIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            initializeOpenFileDialogProperties();
            createAlbumsFromReadFile();
        }

        private void initializeOpenFileDialogProperties()
        {            
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image Files (*.jpg)|*.jpg|" +
                                    "Image Files (*.png)|*.png|" +
                                    "All Files (*.*)|*.*";           
        }

        private void createAlbumsFromReadFile()
        {
            string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string photosFilePath = myDocPath + "/Albums.txt";

            if (File.Exists(photosFilePath))
            {
                string deSerializedObject = File.ReadAllText(photosFilePath);

                albums = JsonConvert.DeserializeObject(deSerializedObject, typeof(List<Album>)) as List<Album>;

                albumIndex = albums.Count - 1;

                if (albumIndex > -1)
                {
                    textBoxForEmptyUniformGrid.Visibility = Visibility.Collapsed;
                }

                updateTreeView();
                updateUniformGrid();
            }           
        }

        private void createAlbumButton_Click(object sender, RoutedEventArgs e)
        {                            
            try
            {
                tryToCreateAlbum();
            }
            catch(Exception)
            {
                MessageBox.Show("One or more images does not have a valid format \n \t \t OR \n it is not supported by BitmapImage.",
                                "Error", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);                
            }                               
        }

        private void tryToCreateAlbum()
        {
            AlbumNameInputDialog albumNameInputDialog = new AlbumNameInputDialog();
            Album album = new Album();          

            if (albumNameInputDialog.ShowDialog() == true)
            {
                textBoxForEmptyUniformGrid.Visibility = Visibility.Collapsed;
                
                album.Name = albumNameInputDialog.Answer;
                updateAlbum(album, albumIndex + 1);
                albumIndex++;

                albums.Add(album);

                updateTreeView();
                updateUniformGrid();
            }
        }

        private void updateAlbum(Album album, int parentAlbum)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                int i = 0;
                string[] safeFileNames = openFileDialog.SafeFileNames;
                foreach (string path in openFileDialog.FileNames)
                {                                       
                    album.Images.Add(new ImageMember()
                    {
                        Name = safeFileNames[i],                         
                        ImagePath = path,                      
                        indexOfParentAlbum = parentAlbum
                    });
                    i++;
                }
            }
        }
        
        private void updateTreeView()
        {
            treeViewForAlbums.ItemsSource = albums;
            treeViewForAlbums.Items.Refresh();           
        }

        private void updateUniformGrid()
        {
            listBoxForUniformGrid.ItemsSource = albums;
            listBoxForUniformGrid.Items.Refresh();
        }

        private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            listBoxForUniformGrid.Items.Refresh();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImagesWindow imagesWindow = new ImagesWindow(albums[listBoxForUniformGrid.SelectedIndex]);            
            
            imagesWindow.ShowDialog();
            updateTreeView();
            updateUniformGrid();            
        }

        private void CommandBindingDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (checkIfTargetCanBeDeletedFromUniformGrid() || checkIfTargetCanBeDeletedFromTreeView())
            {
                e.CanExecute = true;
            }
        }

        private bool checkIfTargetCanBeDeletedFromUniformGrid()
        {
            if (listBoxForUniformGrid.SelectedIndex >= 0)
            {
                return true;
            }
            return false;
        }

        private bool checkIfTargetCanBeDeletedFromTreeView()
        {
            if (treeViewForAlbums.SelectedItem != null)
            {
                return true;
            }
            return false;
        }

        private void CommandBindingDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            if (checkIfTargetCanBeDeletedFromUniformGrid())
            {
                deleteAlbumFromUniformGrid();
                return;
            }
            if (checkIfTargetCanBeDeletedFromTreeView())
            {
                deletePhotoFromTreeView();
                return;
            }
        }

        private void deleteAlbumFromUniformGrid()
        {
            int indexToBeDeleted = listBoxForUniformGrid.SelectedIndex;            
            MessageBoxResult messageBoxResult = createMessageBoxResultForDeletion("album");

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                decreaseParentIndexOfRestAlbums(indexToBeDeleted);
                albums.RemoveAt(indexToBeDeleted);
                albumIndex--;
                updateTreeView();
                updateUniformGrid();
            }

            if (albumIndex < 0)
            {
                textBoxForEmptyUniformGrid.Visibility = Visibility.Visible;
            }
        }

        private void decreaseParentIndexOfRestAlbums(int indexOfParentAlbumToBeDecreased)
        {
            for (int i = indexOfParentAlbumToBeDecreased; i < albums.Count; i++)
            {
                for (int j = 0; j < albums[i].Images.Count; j++)
                {
                    albums[i].Images[j].indexOfParentAlbum--;
                }
            }
        }

        private void deletePhotoFromTreeView()
        {
            if (treeViewForAlbums.SelectedItem.ToString().Equals("PhotoAlbum.Album"))
            {
                return;
            }
             
            ImageMember selectedImageMember = (ImageMember)treeViewForAlbums.SelectedItem;
            int indexOfParentAlbum = selectedImageMember.indexOfParentAlbum;           

            for (int i = 0; i < albums[indexOfParentAlbum].Images.Count; i++)
            {
                if (albums[indexOfParentAlbum].Images[i].Name.Equals(selectedImageMember.Name))
                {
                    MessageBoxResult messageBoxResult = createMessageBoxResultForDeletion("photo");

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        albums[indexOfParentAlbum].Images.RemoveAt(i);
                        updateUniformGrid();
                        break;
                    }
                }
            }
        }                              

        private MessageBoxResult createMessageBoxResultForDeletion(string choiceOfUser)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this " + choiceOfUser + "?", "Sure?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);
            return result;
        }

        private void CommandBindingAddToAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int albumsTargetedIndexToAddPhotos = listBoxForUniformGrid.SelectedIndex;
            
            if ((uniformGridForAlbums.Children.Count > 0) && (albumsTargetedIndexToAddPhotos >= 0))
            {
                updateAlbum(albums[albumsTargetedIndexToAddPhotos], albumsTargetedIndexToAddPhotos);               
            }                     
        }

        private void CommandBindingAddToAlbum_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowPhotoDetails_Click(object sender, RoutedEventArgs e)
        {
            ImageMember selectedImageMember = (ImageMember)treeViewForAlbums.SelectedItem;
            BitmapImage bitmapImage = new BitmapImage(new Uri(selectedImageMember.ImagePath));
            var imageType = selectedImageMember.Name.Substring(selectedImageMember.Name.LastIndexOf('.') + 1);            

            MessageBox.Show("Path : " + bitmapImage.UriSource + "\n" + "\n" +
                            "Name : " + selectedImageMember.Name + "\n" +
                            "Type : " + imageType + "\n" +
                            "Dimensions : " + bitmapImage.PixelWidth + " x " + bitmapImage.PixelHeight + "\n" +
                            "Width : " + bitmapImage.PixelWidth + "\n" +
                            "Height : " + bitmapImage.PixelHeight,                            
                            "Details", MessageBoxButton.OK, MessageBoxImage.Information);

           
        }

        private void selectTreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = visualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }       

        static TreeViewItem visualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save the changes in a Desktop file for later?", "Do you?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string serializedObject = JsonConvert.SerializeObject(albums);
                string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                File.WriteAllText(myDocPath + "/Albums.txt", serializedObject);
            }           
        }
    }
}
