using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class LoggerList
    {
        public int indexNum { get; set; }
        public DateTime InputTime { get; set; }
        public String Comment { get; set; }


        public LoggerList(int indexNum, DateTime InputTime, String Comment)
        {
            this.indexNum = indexNum;
            this.InputTime = InputTime;
            this.Comment = Comment;
        }
    }
}
