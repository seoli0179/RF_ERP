using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class CategoryList
    {
        public int indexNum { get; set; }
        public String Name { get; set; }

        public CategoryList(int indexNum, String Name )
        {
            this.indexNum = indexNum;
            this.Name = Name;
        }
    }
}
