using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using QR_Kod_Okuyucu_Tasarım.Properties;
using ZXing;

namespace QR_Kod_Okuyucu_Tasarım
{


    public partial class FrmMain : Form
    {
        private FilterInfoCollection filterInfoCollection;
        private VideoCaptureDevice videoCaptureDevice;
        private bool Baslat_Durdur;


        public string stoped = "Durduruldu...";
        public string scaning = "Taranıyor...";



        private Process process = new Process();

        public FrmMain(){
            InitializeComponent();
        }

        private void M_baslatDurdur(bool _true_false){
            if (Baslat_Durdur == _true_false) videoCaptureDevice.Stop();
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e){
            M_baslatDurdur(true);
            Application.Exit();
        }
       

        
        private void Form1_Load(object sender, EventArgs e){
            label1.Text = "Tarama Yapmak İçin Başlata Tıklayın !";
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo f in filterInfoCollection) c1.Items.Add(f.Name);
        }

        private void guna2Button4_Click(object sender, EventArgs e){
            if (Baslat_Durdur)
            {
                Stoped();
                b1.Image = Resources.icons8_pause_48px;
                Baslat_Durdur = false;
                videoCaptureDevice.Stop();
            }

            var frmQRCodeCreate = new FrmQRCodeCreate();
            frmQRCodeCreate.Show();
        }

        private void guna2Button6_Click(object sender, EventArgs e){
            timer1.Start();
            r1.Text = "";
        }


        private void guna2ImageButton3_Click(object sender, EventArgs e){
            WindowState = FormWindowState.Minimized; //Formu minimize eder
        }

        //BAŞLAT BUTONU
        private void guna2Button1_Click(object sender, EventArgs e){
            if (Baslat_Durdur == false)
            {
                if (c1.Text == "") //Kameranın Secili Olup olmadıgını Kontrol eder.
                {
                    c1.Focus();
                    MessageBox.Show("Lütfen Kullanılacak Aygıtı Seçin !");
                }
                else
                {
                    Scanning();
                    b1.Image = Resources.icons8_stop_48px;

                    M_baslatDurdur(true);
                    Baslat_Durdur = true;
                    videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[c1.SelectedIndex].MonikerString);
                    videoCaptureDevice.NewFrame += V_NewFrame;
                    videoCaptureDevice.Start();
                    timer1.Start();
                }
            }
            else
            {
                Stoped();
                b1.Image = Resources.icons8_pause_48px;
                Baslat_Durdur = false;
                videoCaptureDevice.Stop();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) //Yakala butonu
        {
            if (p1.Image==null) return; //Görüntün Olup olmadıgını kontrol eder.
           
            if (c1.Text == "") //aygıtın secili olup olmadıgını kontrol eder.
            {
                MessageBox.Show("Lütfen Kullanılacak Aygıtı Seçin !");
            }
            else
            {
                M_baslatDurdur(true); //Ekran görüntüsü alırken kamerayı Durdurur
                var saveImage = new SaveFileDialog {Filter = "Resim | *.jpg*.png"};
                var r = saveImage.ShowDialog();

                if (r == DialogResult.OK) p1.Image.Save(saveImage.FileName);

                if (Baslat_Durdur) videoCaptureDevice.Start(); //Ekran görüntüsü alındıktan sonra kamerayı Başlatır
            }
        }

        private void V_NewFrame(object sender, NewFrameEventArgs eventArgs){
            try
            {
                p1.Image = (Bitmap) eventArgs.Frame.Clone();
            }
            catch (Exception)
            {
                MessageBox.Show("Hata");
            }

        }

        private void timer1_Tick(object sender, EventArgs e){
            if (p1.Image != null)
            {
                var br = new BarcodeReader();
                try
                {
                    var result = br.Decode((Bitmap) p1.Image);
                    if (result == null) return;
                    r1.Text = result.ToString();
                    timer1.Stop();
                }
                catch (Exception)
                {
                    MessageBox.Show("Bilinmeyen Hata !");
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e){
            Stoped();

            b1.Image = Resources.icons8_pause_48px;
            if (Baslat_Durdur) videoCaptureDevice.Stop();

            var openFileDialog = new OpenFileDialog();
            timer1.Start();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                p1.Load(openFileDialog.FileName);
            }
            else
            {
                Scanning();
                if (Baslat_Durdur) videoCaptureDevice.Start();
            }
        }

       

        private void guna2ImageButton1_Click(object sender, EventArgs e){
            timer1.Start();
            r1.Text = "";
        }

        private void guna2ImageButton4_Click(object sender, EventArgs e){
            
            var frmInfo = new FrmInfo();
            frmInfo.Show();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e){
            label1.Text = r1.Text != ""
                ? "Yeniden Tarama İçin \"Yenile\" Butonuna Basınız !"
                : scaning;
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e){
            try
            {
                process = Process.Start("msedge.exe", e.LinkText);
            }
            catch (Exception)
            {
                process = Process.Start("chrome.exe", e.LinkText);
            }
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void b5_Click(object sender, EventArgs e)
        {
            string copy = r1.Text;
            Clipboard.SetDataObject((object)copy, false);
        }
        void Stoped()
        {
            label1.Text = stoped;
            b1.Text = stoped;
        }
        void Scanning()
        {
            label1.Text = scaning;
            b1.Text = scaning;
        }

    }
}