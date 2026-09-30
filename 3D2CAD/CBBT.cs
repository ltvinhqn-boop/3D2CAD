using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DWG2PDF
{
    public partial class CBBT : UserControl
    {
        
        public CBBT()
        {
            InitializeComponent();
          
        }
       
        public string NameRight
        {
            get { return bt_Right.Name; }
            set { bt_Right.Name = value; }
        }
        public string NameIso
        {
            get { return bt_Iso.Name; }
            set { bt_Iso.Name = value; }
        }
        public string NameRemove
        {
            get { return bt_rm.Name; }
            set { bt_rm.Name = value; }
        }
        public string NameBottom
        {
            get { return bt_Bottom.Name; }
            set { bt_Bottom.Name = value; }
        }
        public string NameFront
        {
            get { return bt_Front.Name; }
            set { bt_Front.Name = value; }
        }
        public string TextNumber
        {
            get { return tb_cbbt3.Text; }
            set { tb_cbbt3.Text = value; }
        }
        public string TextDrawing
        {
            get { return tb_cbbt2.Text; }
            set { tb_cbbt2.Text = value; }
        }
        public Color ColorButtonRight
        {
            get { return bt_Right.ForeColor; }
            set { bt_Right.ForeColor = value; }
        }

        public Color ColorButtonIso
        {
            get { return bt_Iso.ForeColor; }
            set { bt_Iso.ForeColor = value; }
        }

        public Color ColorButtonBottom
        {
            get { return bt_Bottom.ForeColor; }
            set { bt_Bottom.ForeColor = value; }
        }
        public Color ColorButtonFront
        {
            get { return bt_Front.ForeColor; }
            set { bt_Front.ForeColor = value; }
        }
        private void bt_la1_Click(object sender, EventArgs e)
        {
        
            OnBtla1Clicked();
        }
    

        public void bt_rm_Click(object sender, EventArgs e)
        {
            OnBtrmClicked();
        }

   

        private void bt_la1_MouseEnter(object sender, EventArgs e)
        {
            bt_Iso.BorderColor = Color.FromArgb(55, 55, 55);
            bt_Iso.BackColor = Color.FromArgb(55, 55, 55);
            bt_Iso.BackgroundColor = Color.FromArgb(55, 55, 55);
        }

        private void bt_la1_MouseLeave(object sender, EventArgs e)
        {
            bt_Iso.BorderColor = Color.FromArgb(50, 50, 50);
            bt_Iso.BackColor = Color.FromArgb(50, 50, 50);
            bt_Iso.BackgroundColor = Color.FromArgb(50, 50, 50);
        }

        private void bt_rm_MouseEnter(object sender, EventArgs e)
        {
            bt_rm.BorderColor = Color.FromArgb(55, 55, 55);
            bt_rm.BackColor = Color.FromArgb(55, 55, 55);
            bt_rm.BackgroundColor = Color.FromArgb(55, 55, 55);
            OnBtrmMouseEnter();
        }

        private void bt_rm_MouseLeave(object sender, EventArgs e)
        {
            bt_rm.BorderColor = Color.FromArgb(50, 50, 50);
            bt_rm.BackColor = Color.FromArgb(50, 50, 50);
            bt_rm.BackgroundColor = Color.FromArgb(50, 50, 50);
            OnBtrmMouseLeave();
        }


        

        public event EventHandler BtMd1Clicked;
        public event EventHandler Btla1Clicked;
        public event EventHandler BtrmClicked;
        public event EventHandler BtprintClicked;
        public event EventHandler BtFrontClicked;
        public event EventHandler Textbox1Clicked;
        public event EventHandler Textbox2Clicked;
        public event EventHandler Btrm_MouseLeave;
        public event EventHandler Btrm_MouseEnter;

        // Khai báo các sự kiện cho các button khác (nếu có)
        private void OnBtrmMouseLeave()
        {
            Btrm_MouseLeave?.Invoke(this, EventArgs.Empty);
        }
       
        private void OnBtrmMouseEnter()
        {
            Btrm_MouseEnter?.Invoke(this, EventArgs.Empty);
        }
        private void OnBtMd1Clicked()
        {
            BtMd1Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnBtla1Clicked()
        {
            Btla1Clicked?.Invoke(this, EventArgs.Empty);
        }
        private void OnBtrmClicked()
        {
            BtrmClicked?.Invoke(this, EventArgs.Empty);
        }
        private void OnBtprintClicked()
        {
            BtprintClicked?.Invoke(this, EventArgs.Empty);
        }
        private void OnBtFrontClicked()
        {
            BtFrontClicked?.Invoke(this, EventArgs.Empty);
        }
        private void OnTextbox1Clicked()
        {
           Textbox1Clicked?.Invoke(this, EventArgs.Empty);
        }
        private void OnTextbox2Clicked()
        {
            Textbox2Clicked?.Invoke(this, EventArgs.Empty);
        }
        private void bt_mdel_Click(object sender, EventArgs e)
        {
          //  vb9bool1 = !vb9bool1;
          //  if (vb9bool1 == true)
          //  {
         //       bt_mdel.ForeColor = Color.Yellow;
         //   }
           // else
          //  {
          //      bt_mdel.ForeColor = Color.White;
         //   }
            OnBtMd1Clicked();
        }

        private void bt_mdel_MouseEnter(object sender, EventArgs e)
        {
            bt_Right.BorderColor = Color.FromArgb(55, 55, 55);
            bt_Right.BackColor = Color.FromArgb(55, 55, 55);
            bt_Right.BackgroundColor = Color.FromArgb(55, 55, 55);
        }

        private void bt_mdel_MouseLeave(object sender, EventArgs e)
        {

            bt_Right.BorderColor = Color.FromArgb(50, 50, 50);
            bt_Right.BackColor = Color.FromArgb(50, 50, 50);
            bt_Right.BackgroundColor = Color.FromArgb(50, 50, 50);
        }

        private void bt_print_Click(object sender, EventArgs e)
        {
            //vb11bool = !vb11bool;
            //if (vb11bool == true)
            //{
            //    bt_print.ForeColor = Color.Yellow;
            //}
            //else
            //{
            //    bt_print.ForeColor = Color.White;
            //}
            OnBtprintClicked();
        }

        private void bt_print_MouseEnter(object sender, EventArgs e)
        {
            bt_Bottom.BorderColor = Color.FromArgb(55, 55, 55);
            bt_Bottom.BackColor = Color.FromArgb(55, 55, 55);
            bt_Bottom.BackgroundColor = Color.FromArgb(55, 55, 55);
        }

        private void bt_print_MouseLeave(object sender, EventArgs e)
        {
            bt_Bottom.BorderColor = Color.FromArgb(50, 50, 50);
            bt_Bottom.BackColor = Color.FromArgb(50, 50, 50);
            bt_Bottom.BackgroundColor = Color.FromArgb(50, 50, 50);
        }

        private void tb_cbbt2_Click(object sender, EventArgs e)
        {
            OnTextbox1Clicked();
        }

        private void tb_cbbt3_Click(object sender, EventArgs e)
        {
            OnTextbox2Clicked();
        }

        private void tb_cbbt3_MouseEnter(object sender, EventArgs e)
        {
            tb_cbbt3.ForeColor = Color.Yellow;
        }

        private void tb_cbbt3_MouseLeave(object sender, EventArgs e)
        {
            tb_cbbt3.ForeColor = Color.White;
        }

        private void tb_cbbt2_MouseEnter(object sender, EventArgs e)
        {
            tb_cbbt2.ForeColor = Color.Yellow;
        }

        private void tb_cbbt2_MouseLeave(object sender, EventArgs e)
        {
            tb_cbbt2.ForeColor = Color.White;
        }

        private void bt_print_MouseHover(object sender, EventArgs e)
        {
            if (bt_Bottom.ForeColor == Color.Yellow)
            {
                toolTip2.SetToolTip(bt_Bottom, "Tắt hình chiếu bằng");
            }
            else
            {
                toolTip2.SetToolTip(bt_Bottom, "Bật hình chiếu bằng");
            }
        }

        private void bt_mdel_MouseHover(object sender, EventArgs e)
        {
            if (bt_Right.ForeColor == Color.Yellow)
            {
                toolTip2.SetToolTip(bt_Right, "Tắt hình chiếu cạnh");
            }
            else
            {
                toolTip2.SetToolTip(bt_Right, "Bật hình chiếu cạnh");
            }
        }

        private void bt_la1_MouseHover(object sender, EventArgs e)
        {
            if (bt_Iso.ForeColor == Color.Yellow)
            {
                toolTip2.SetToolTip(bt_Iso, "Tắt hình chiếu 3D");
            }
            else
            {
                toolTip2.SetToolTip(bt_Iso, "Bật hình chiếu 3D");
            }
        }

        private void bt_rm_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(bt_rm, "Xóa Component");
        }

        private void bt_Front_Click(object sender, EventArgs e)
        {
            OnBtprintClicked();
        }

        private void bt_Front_MouseEnter(object sender, EventArgs e)
        {

            bt_Front.BorderColor = Color.FromArgb(55, 55, 55);
            bt_Front.BackColor = Color.FromArgb(55, 55, 55);
            bt_Front.BackgroundColor = Color.FromArgb(55, 55, 55);
        }

        private void bt_Front_MouseLeave(object sender, EventArgs e)
        {
            bt_Front.BorderColor = Color.FromArgb(50, 50, 50);
            bt_Front.BackColor = Color.FromArgb(50, 50, 50);
            bt_Front.BackgroundColor = Color.FromArgb(50, 50, 50);
        }

        private void bt_Front_MouseHover(object sender, EventArgs e)
        {
            if (bt_Front.ForeColor == Color.Yellow)
            {
                toolTip2.SetToolTip(bt_Front, "Tắt hình chiếu đứng");
            }
            else
            {
                toolTip2.SetToolTip(bt_Front, "Bật hình chiếu đứng");
            }
        }
    }
}
