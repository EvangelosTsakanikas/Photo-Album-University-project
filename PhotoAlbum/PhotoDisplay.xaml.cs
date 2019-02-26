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
    /// Interaction logic for PhotoDisplay.xaml
    /// </summary>
    public partial class PhotoDisplay : Window
    {
        private Album album;
        private int index;
        private Point origin;
        private Point start;
        private int counterMouseWheelUp = 0;
        private int counterMouseWheelDown = 0;
                              
        public PhotoDisplay(Album album, int index)
        {
            InitializeComponent();
            this.album = album;
            this.index = index;
            BitmapImage selectedImageBitmap = new BitmapImage(new Uri(album.Images[index].ImagePath));            
            slotForImage.Source = selectedImageBitmap;
        }

        private void LeftPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            if (index > 0)
            {
                index--;               
            }
            else
            {
                index = album.Images.Count - 1;
            }
            BitmapImage selectedImageBitmap = new BitmapImage(new Uri(album.Images[index].ImagePath));            
            slotForImage.Source = selectedImageBitmap;            
        }

        private void RightPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (index < album.Images.Count - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            BitmapImage selectedImageBitmap = new BitmapImage(new Uri(album.Images[index].ImagePath));
            slotForImage.Source = selectedImageBitmap;
        }

        private void slotForImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point point = e.MouseDevice.GetPosition(slotForImage);

            Matrix matrix = slotForImage.RenderTransform.Value;
            if (e.Delta > 0)
            {
                matrix.ScaleAtPrepend(1.1, 1.1, point.X, point.Y);
                counterMouseWheelUp++;
            }
            else
            {
                matrix.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, point.X, point.Y);
                counterMouseWheelDown++;
            }
            
            slotForImage.RenderTransform = new MatrixTransform(matrix);
        }

        private void slotForImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (slotForImage.IsMouseCaptured) return;
            slotForImage.CaptureMouse();

            start = e.GetPosition(borderForSlotForImage);
            origin.X = slotForImage.RenderTransform.Value.OffsetX;            
            origin.Y = slotForImage.RenderTransform.Value.OffsetY;
        }

        private void slotForImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            slotForImage.ReleaseMouseCapture();            
        }

        private void slotForImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!slotForImage.IsMouseCaptured) return;

            Point point = e.MouseDevice.GetPosition(borderForSlotForImage);
            Matrix matrix = slotForImage.RenderTransform.Value;

            matrix.OffsetX = origin.X + (point.X - start.X);
            matrix.OffsetY = origin.Y + (point.Y - start.Y);

            slotForImage.RenderTransform = new MatrixTransform(matrix);
        }

        private void CommandBindingRestoreImageZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            Matrix matrix = slotForImage.RenderTransform.Value; 
            int mouseWheelingDeference = counterMouseWheelUp - counterMouseWheelDown;
            
            if (mouseWheelingDeference >= 0)
            {
                for (int i = 0; i < mouseWheelingDeference; i++)
                {
                    matrix.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, borderForSlotForImage.ActualWidth / 2, borderForSlotForImage.ActualHeight / 2);
                }
            }
            else
            {
                for (int i = 0; i > mouseWheelingDeference; i--)
                {
                    matrix.ScaleAtPrepend(1.1, 1.1, borderForSlotForImage.ActualWidth / 2, borderForSlotForImage.ActualHeight / 2);
                }
            }
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            
            slotForImage.RenderTransform = new MatrixTransform(matrix);

            counterMouseWheelUp = 0;
            counterMouseWheelDown = 0;
        }

        private void CommandBindingRestoreImageZoom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

    }
}
