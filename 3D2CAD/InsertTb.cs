using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace _3D2CAD
{
    class InsertTb
    {
        public static void InsertTB(TextBox textBox1, int n, string data)
        {
            // Kiểm tra xem n có hợp lệ hay không
            if (n < 0 || n > textBox1.Lines.Length + 1)
            {
                // Nếu không, hiển thị thông báo lỗi
                MessageBox.Show("Invalid line number");
            }
            else
            {
                // Nếu có, tạo một danh sách mới chứa các dòng của textbox
                List<string> lines = new List<string>(textBox1.Lines);

                // Chèn dữ liệu vào vị trí thứ n của danh sách
                lines.Insert(n, data);

                // Cập nhật các dòng của textbox bằng danh sách mới
                textBox1.Lines = lines.ToArray();
            }
        }
    }
}
