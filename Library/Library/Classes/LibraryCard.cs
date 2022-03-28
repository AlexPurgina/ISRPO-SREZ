using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class LibraryCard
    {
        public Reader Reader { get; set; }
        public List<RecordBook> Records { get; set; }
        public bool IsActice { get; set; } = true;
    }
}
