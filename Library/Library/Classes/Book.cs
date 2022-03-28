using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Book
    {
        public string authorTitle { get { return $"{author} {title}"; } }
        public string title { get; set; }
        public string author { get; set; }
        public string genre { get; set; }
        public string subGenre { get; set; }
        public string height { get; set; }
        public string publisher { get; set; }
        public string publisherEx
        {
            get
            {
                return publisher.Length == 0 ? "не указано" : publisher;
            }
        }
    }

}
