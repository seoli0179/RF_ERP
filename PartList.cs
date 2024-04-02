using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PartList
    {

        public Boolean Part_DataSheet_Exists_Check { get; set; }
        public int indexNum { get; set; }
        public String Name { get; set; }
        public String Category { get; set; }
        public int Stock { get; set; }
        public int Size { get; set; }
        public Double Volume { get; set; }
        public String Description { get; set; }
        public String Manufacturer { get; set; }
        public String Purchase { get; set; }
        public String Room { get; set; }
        public int Position { get; set; }
        public String Comment { get; set; }

        public PartList(int indexNum, String Name, String Category, int Size, Double Volume, int Stock, String Description, String Manufacturer, String Purchase,String Room, int Position, String Comment)
        {
            this.indexNum = indexNum;
            this.Name = Name;
            this.Category = Category;
            this.Size = Size;
            this.Volume = Volume;
            this.Stock = Stock;
            this.Description = Description;
            this.Manufacturer = Manufacturer;
            this.Purchase = Purchase;
            this.Room = Room;
            this.Position = Position;
            this.Comment = Comment;
        }
    }
}
