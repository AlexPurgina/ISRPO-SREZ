using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Reader
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public byte[] photo { get; set; }
        public string fullName
        {
            get
            {
                return lastName + " " + firstName.FirstOrDefault() + ". " + middleName.FirstOrDefault() + ".";
            }
        }
        public string FIO
        {
            get
            {
                return lastName + " " + firstName + " " + middleName;
            }
        }
    }
}
