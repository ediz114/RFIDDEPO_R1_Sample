using RFIDDepo.DesktopReader;
using RFIDDepo.DesktopReader.interfaces;
using RFIDDepo.DesktopReader.utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Policy;
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

namespace RFIDDEPO_R1_Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UHFAPI uhf = null;
        delegate void GetRemotelyIPCallback(string remoteip);
        GetRemotelyIPCallback RemotelyIPCallback;
        IAutoReceive iAutoReceive = null;
        private const int max = 1024 * 1024;
        private byte[] uhfOriginalData = new byte[max];
        private int wIndex = 0;
        private int rIndex = 0;
        private bool isRuning = true;
        private bool isOpen = false;
        private Thread threadEPC = null;
        List<EpcInfo> epcList = new List<EpcInfo>();

        public MainWindow()
        {
            InitializeComponent();
            #region UHF Reader
            uhf = UHFAPI.getInstance();
            BTN_Connect.IsEnabled = true;
            BTN_Start.IsEnabled = false;
            BTN_Stop.IsEnabled = false;
            #endregion
        }

        TagViewModel ekk1;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.dataGrid.ItemsSource = epcList;

        }

        private void StartEPC()
        {

            new Thread(new ThreadStart(delegate { ReadEPC(); })).Start();

        }


        string last;

        private async void UpdataEPC(string epc, string tid, string rssi, string count, string ant, string user)
        {
            bool[] exist = new bool[1];
            int index = CheckUtils.getInsertIndex(epcList, epc, exist);
            if (exist[0])
            {
                //lvEPC.Items[index].SubItems["TID"].Text = tid;
                //lvEPC.Items[index].SubItems["USER"].Text = user;
                //lvEPC.Items[index].SubItems["RSSI"].Text = rssi;
                //lvEPC.Items[index].SubItems["COUNT"].Text = (int.Parse(lvEPC.Items[index].SubItems["COUNT"].Text) + int.Parse(count)).ToString();
                //lvEPC.Items[index].SubItems["ANT"].Text = ant;

                var ee = ((EpcInfo)dataGrid.Items.GetItemAt(index));
                ee.Count++;
                dataGrid.Items.Refresh();

                //dataGrid.Items.Ge(index, ee);
            }
            else
            {
                //total++;
                //ListViewItem lv = new ListViewItem();
                //lv.Text = (index + 1).ToString();
                //ListViewItem.ListViewSubItem itemEPC = new ListViewItem.ListViewSubItem();
                //itemEPC.Name = "EPC";
                //itemEPC.Text = epc;
                //lv.SubItems.Add(itemEPC);

                //ListViewItem.ListViewSubItem itemTid = new ListViewItem.ListViewSubItem();
                //itemTid.Name = "TID";
                //itemTid.Text = tid;
                //lv.SubItems.Add(itemTid);

                //ListViewItem.ListViewSubItem itemUser = new ListViewItem.ListViewSubItem();
                //itemUser.Name = "USER";
                //itemUser.Text = user;
                //lv.SubItems.Add(itemUser);

                //ListViewItem.ListViewSubItem itemRssi = new ListViewItem.ListViewSubItem();
                //itemRssi.Name = "RSSI";
                //itemRssi.Text = rssi;
                //lv.SubItems.Add(itemRssi);

                //ListViewItem.ListViewSubItem itemCount = new ListViewItem.ListViewSubItem();
                //itemCount.Name = "COUNT";
                //itemCount.Text = count;
                //lv.SubItems.Add(itemCount);

                //ListViewItem.ListViewSubItem itemAnt = new ListViewItem.ListViewSubItem();
                //itemAnt.Name = "ANT";
                //itemAnt.Text = ant;
                //lv.SubItems.Add(itemAnt);

                //lvEPC.Items.Insert(index, lv);// Add(lv);



                this.Title = epc;
                last = epc;


                var ee = new EpcInfo(epc, int.Parse(count), DataConvert.HexStringToByteArray(epc));
                epcList.Insert(index, ee);



                dataGrid.Items.Insert(index, ee);



            }

        }









        delegate void SetTextCallback(string epc, string tid, string rssi, string count, string ant, string user);
        SetTextCallback setTextCallback;
        private void ReadEPC()
        {
            try
            {
                //beginTime = System.Environment.TickCount;
                while (true)
                {
                    UHFTAGInfo info = uhf.uhfGetReceived();
                    if (info != null)
                    {
                        this.Dispatcher.BeginInvoke(setTextCallback, new object[] { info.Epc, info.Tid, info.Rssi, "1", info.Ant, info.User });
                    }
                    else
                    {
                        if (isRuning)
                        {
                            Thread.Sleep(5);
                        }
                        else
                        {
                            break;
                        }
                    }
                }



            }
            catch (Exception ex)
            {

            }

        }

        private bool Connect()
        {



            if (uhf.Inventory())
            {
                StartEPC();
            }
            else
            {

            }



            return true;
        }

        private void BTN_Connect_USB_Click(object sender, RoutedEventArgs e)
        {
            setTextCallback = new SetTextCallback(UpdataEPC);

            var result = uhf.OpenUsb();

            if (result)
            {


                Btn_Ekle.IsEnabled = true;
                Btn_Ekle_Copy.IsEnabled = true;

                BTN_Start.IsEnabled = true;

            }
            else
            {

            }
        }
        private void BTN_Start_Click(object sender, RoutedEventArgs e)
        {
            isRuning = true;
            Connect();

            BTN_Start.IsEnabled = false;
            BTN_Stop.IsEnabled = true;
        }

        private void BTN_Stop_Click(object sender, RoutedEventArgs e)
        {
            bool reuslt = uhf.StopGet();

            isRuning = false;
            BTN_Stop.IsEnabled = false;
            BTN_Start.IsEnabled = true;


            epcList = new List<EpcInfo>();

        }

        private void Btn_Ekle1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Ekle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
