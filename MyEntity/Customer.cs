using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEntity
{
    [Serializable]  //必須要可序列化
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }

        public DateTime CreateTime { get; set; }
    }

}
