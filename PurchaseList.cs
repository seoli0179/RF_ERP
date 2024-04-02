using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PurchaseList
    {
        public int indexNum { get; set; }
        public String Name { get; set; }
        public String Human { get; set; }
        public String Mail { get; set; }
        public String Phone { get; set; }
        public String Payment { get; set; }
        public String Comment { get; set; }
        public PurchaseList(int indexNum, String Name, String Human, String Mail, String Phone,String Payment, String Comment)
        {
            this.indexNum = indexNum;
            this.Name = Name;
            this.Human = Human;
            this.Mail = Mail;
            this.Phone = Phone;
            this.Payment = Payment;
            this.Comment = Comment;
        }
    }
}
