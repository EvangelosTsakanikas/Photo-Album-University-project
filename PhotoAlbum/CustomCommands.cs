using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoAlbum
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand AddToAlbum = new RoutedUICommand("Add",
                                                                                "AddToAlbum",
                                                                                typeof(CustomCommands),
                                                                                new InputGestureCollection()
                                                                                {
                                                                                    new KeyGesture(Key.O, ModifierKeys.Control)
                                                                                });

        public static readonly RoutedUICommand RestoreImageZoom = new RoutedUICommand("Restore Zoom",
                                                                                      "RestoreImageZoom",
                                                                                       typeof(CustomCommands),
                                                                                       new InputGestureCollection()
                                                                                       {
                                                                                           new KeyGesture(Key.R, ModifierKeys.Control)
                                                                                       });       
    }
}
