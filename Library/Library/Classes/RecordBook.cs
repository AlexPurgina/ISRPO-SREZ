using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class RecordBook
    {
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public Book book { get; set; }
        public string path
        {
            get
            {
                if (retDay > 7)
                {
                    return "../../icon/unnamed.png";
                }
                else
                {
                    return "../../icon/img_28615.png";
                }
            }
        }
        public int retDay
        {
            get
            {
                return (dateEnd - dateStart).Days;
            }
        }
    }
}
