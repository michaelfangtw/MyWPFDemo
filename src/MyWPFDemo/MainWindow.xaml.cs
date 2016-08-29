using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyEntity;

namespace MyWPFDemo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    /// 
        
    public partial class MainWindow : Window
    {
       
     
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            int sendCount = SendMessageAll();
            ShowMessage("發送筆數:{0}", sendCount);
        }

        void ShowMessage(string msg,params object[] arg)
        {
            string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            txtMsg.Text = string.Format(now+ " "+msg+"\r\n", arg) + txtMsg.Text;
        }
        public int SendMessageAll()
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                count++;
                Console.Write("message sent,#{0}, time:{1} \r\n", i + 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                SendMessage();
            }
            return count;
        }

        private static bool SendMessage()
        {
            MessageQueue myQueue;
            string path = "";
            //local private
            path = ".\\private$\\MyQueue"; //本機
            //path = @"FormatName:Direct=OS:fij-nb\private$\MyQueue"; //遠端主機名稱
            //path = @"FormatName:Direct=TCP:192.168.0.103\private$\MyQueue";//遠端主機IP
            if (path.StartsWith(@".\"))// path=@".\Private$\MyQueue"
            {
                if (MessageQueue.Exists(path))
                {
                    myQueue = new MessageQueue(path);
                }
                else
                {
                    myQueue = MessageQueue.Create(path);
                }
            }
            else
            {
                myQueue = new MessageQueue(path);
            }


            List<Customer> customerList = new List<Customer>();
            customerList.Add(new Customer { CustomerID = "1", CustomerName = "Mike", Birthday = DateTime.Parse("1973/07/27"), Email = "yingchih.fang@gmail.com", CreateTime = DateTime.Now });
            customerList.Add(new Customer { CustomerID = "2", CustomerName = "Dan", Birthday = DateTime.Parse("1973/08/19"), Email = "danise0819@gmai.com", CreateTime = DateTime.Now });

            Message MyMessage = new Message();
            //set formatter for Message
            MyMessage.Formatter = new BinaryMessageFormatter();
            MyMessage.Body = customerList;
            MyMessage.Label = "customerList," + DateTime.Now.ToString("yyyyMMddHHmmss");
            MyMessage.Priority = MessagePriority.High;
            myQueue.Send(MyMessage);
            Console.WriteLine("path:{0}", path);

            return true;
        }

        public int ReceiveMessage()
        {
            MessageQueue myMessageQueue;
            string path = "";
            path = ".\\private$\\MyQueue"; //本機
            //path = @"FormatName:Direct=OS:fij-nb\private$\MyQueue"; //遠端主機名稱
            //path = @"FormatName:Direct=TCP:192.168.0.103\private$\MyQueue";//遠端主機IP
            myMessageQueue = new MessageQueue(path);
            myMessageQueue.Formatter = new BinaryMessageFormatter();
            int messageCount = 0;
            List<Customer> customerListAll = new List<Customer>();
            try
            { 
                Message[] messages = myMessageQueue.GetAllMessages();

                Console.WriteLine("Receive MessageQueue from {0}", path);
                Console.WriteLine("messages size:{0}", messages.Length);
                foreach (Message message in messages)
                {
                    messageCount++;
                    Message MyMessage = myMessageQueue.Receive();
                    List<Customer> customerList = (List<Customer>)MyMessage.Body;
                    customerListAll.AddRange(customerList);
                    Console.WriteLine("Message #{0}", messageCount);
                    Console.WriteLine("customerList.count:{0}", customerList.Count);
                    int count = 0;
                    foreach (Customer customer in customerList)
                    {
                        Console.WriteLine("Customer[{0}],CustomerID:{1},CustomerName:{2},CreateDate:{3} \r\n", count, customer.CustomerID, customer.CustomerName, customer.CreateTime.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                        count++;
                    }
                    Console.WriteLine("");
                }
            }catch(Exception ex)
            {
                Console.WriteLine("error:", ex.ToString());
                ShowMessage("error=" + ex.ToString());
            }
            dgMessage.ItemsSource = customerListAll;
            ShowMessage("count=" + messageCount.ToString());
          
            return messageCount;
        }
        private void btnReceive_Click(object sender, RoutedEventArgs e)
        {
            int receiveCount = ReceiveMessage();
            ShowMessage("接收筆數:{0}", receiveCount);
        }
    }
}
