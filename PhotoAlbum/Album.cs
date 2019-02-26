using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoAlbum
{
    public class Album
    {
        public Album()
        {
            this.Images = new ObservableCollection<ImageMember>();
        }

        public string Name { get; set; }

        public ObservableCollection<ImageMember> Images { get; set; }
    }
}
