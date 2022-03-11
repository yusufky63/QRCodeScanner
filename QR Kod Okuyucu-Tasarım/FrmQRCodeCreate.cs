using System;

using System.Drawing;

using System.Windows.Forms;
using MessagingToolkit.QRCode.Codec;

namespace QR_Kod_Okuyucu_Tasarım
{
    public partial class FrmQRCodeCreate : Form
    {
        public FrmQRCodeCreate(){
            InitializeComponent();
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e){
            Close();
        }

        private QRCodeEncoder qrEncode = new QRCodeEncoder();
        private Image img;

        private void guna2ImageButton1_Click(object sender, EventArgs e){
            if (richTextBox1.Text == "")
            {
                MessageBox.Show("Lütfen Oluşturlacak QR Kod İçin Metin Girin !");
            }
            else
            {
                img = qrEncode.Encode(richTextBox1.Text);
                guna2PictureBox1.Image = img;
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e){
            if (guna2PictureBox1.Image == null) return;
            var svf = new SaveFileDialog();
            svf.Filter = "Resim Dosyası |*.jpg ,*.png";
            if (svf.ShowDialog() == DialogResult.OK)
            {
                guna2PictureBox1.Image.Save(svf.FileName);
                MessageBox.Show("Kaydedildi !","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            
        }

        private void guna2Button2_Click(object sender, EventArgs e){
            richTextBox1.Text = "";
        }
    }
}