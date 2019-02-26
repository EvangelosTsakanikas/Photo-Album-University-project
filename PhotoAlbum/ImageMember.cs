using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoAlbum
{
    public class ImageMember
    {
        public string Name { get; set; }

        public string ImagePath { get; set; }    
        
        public int indexOfParentAlbum { get; set; }   
    }
}
