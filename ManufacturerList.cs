using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WpfApp1
{

    public class ManufacturerList
    {

        public int indexNum { get; set; }
        public String Name { get; set; }
        public String Purchase { get; set; }

        public String Comment { get; set; }

        public ManufacturerList(int indexNum, String Name, String Purchase,String Comment)
        {
            this.indexNum = indexNum;
            this.Name = Name;
            this.Purchase = Purchase;
            this.Comment = Comment;
        }


    }
}
