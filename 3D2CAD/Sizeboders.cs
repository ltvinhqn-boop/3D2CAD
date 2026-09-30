using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NXOpen;
//using NXOpen.;
using NXOpen.UF;
using NXOpen.Assemblies;
using NXOpen.BlockStyler;
using NXOpen.Drawings;
using NXOpen.Annotations;

namespace _3D2CAD
{
    class Sizeboders
    {
        static public List<double> GetRatioScale(Session theSession, Part workPart, NXOpen.NXObject[] objects1, double boudaryWidth,double boudaryHeight)
        {
            UFSession ufSession = UFSession.GetUFSession();
            List<double> ratio = new List<double>();
            List<double> WidthMax = new List<double>();
            List<double> HeightMax = new List<double>();
            foreach (NXOpen.NXObject obj in objects1)
            {
                // objects1.Count();
                NXOpen.TaggedObject[] objects11 = new NXOpen.TaggedObject[1];
                
               
                if (obj is NXOpen.Drawings.ProjectedView)
                {
                    NXOpen.Drawings.ProjectedView sectionView = (NXOpen.Drawings.ProjectedView)obj;

                    // Lấy thẻ của section view
                    Tag viewTag = sectionView.Tag;
                   
                    // Khai báo mảng để lưu kích thước giới hạn (xmin, ymin, xmax, ymax)
                    double[] viewBorders = new double[4];

                    // Gọi hàm UF_DRAW_ask_view_borders để lấy kích thước giới hạn
                    ufSession.Draw.AskViewBorders(viewTag, viewBorders);

                    // Tính toán width và height từ kích thước giới hạn
                    double xmin = viewBorders[0];
                    double ymin = viewBorders[1];
                    double xmax = viewBorders[2];
                    double ymax = viewBorders[3];
                    double width = xmax - xmin;
                    double height = ymax - ymin;
                    WidthMax.Add(width);
                    HeightMax.Add(height);
                    // Sử dụng width và height
                    // ...
                    //objects1[0] = sectionView;
                    objects11[0] = sectionView;

                    theSession.UpdateManager.AddObjectsToDeleteList(objects11);
                    theSession.UpdateManager.DoUpdate(theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete"));
                }
                else if (obj is NXOpen.Drawings.BaseView)
                {
                    NXOpen.Drawings.BaseView baseView = (NXOpen.Drawings.BaseView)obj;

                    // Lấy thẻ của section view
                    Tag viewTag = baseView.Tag;
                   
                    // Khai báo mảng để lưu kích thước giới hạn (xmin, ymin, xmax, ymax)
                    double[] viewBorders = new double[4];

                    // Gọi hàm UF_DRAW_ask_view_borders để lấy kích thước giới hạn
                    ufSession.Draw.AskViewBorders(viewTag, viewBorders);

                    // Tính toán width và height từ kích thước giới hạn
                    double xmin = viewBorders[0];
                    double ymin = viewBorders[1];
                    double xmax = viewBorders[2];
                    double ymax = viewBorders[3];
                    double width = xmax - xmin;
                    double height = ymax - ymin;

                    WidthMax.Add(width);
                    HeightMax.Add(height);
                    objects11[0] = baseView;


                    theSession.UpdateManager.AddObjectsToDeleteList(objects11);
                    theSession.UpdateManager.DoUpdate(theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete"));

                }
                               
            }
            if (WidthMax.Count() > 0 && HeightMax.Count() > 0)
            {
                double W1 = boudaryWidth / WidthMax.Max();
                double H1 = boudaryHeight / HeightMax.Max();
                if (W1 < H1)
                {
                    ratio = scalefit(W1);
                }
                else
                {
                    ratio = scalefit(H1);
                }
            }
            else
            {
                ratio = new List<double> { 1.0, 1.0 };
            }

            return ratio;
        }


        //-----------------------------------------------------------------------------------------------------
        public static List<double> scalefit(double cbb2)
        {
            List<double> scales = new List<double>();

          
                double dividend;
                double divisor;

                GetFraction(cbb2, out dividend, out divisor);
                scales.Add(dividend);
                scales.Add(divisor);
            
            return scales;
        }
        ///---------------------------------------------
        public static void GetFraction(double number, out double numerator1, out double denominator1)
        {
            // Chuyển đổi số double thành phân số tối giản
            double epsilon = 1e-6; // Độ chính xác epsilon để so sánh với số thực

            // Xác định tử số ban đầu và mẫu số ban đầu
           int numerator = 1;
           int denominator = 1;

            // Tìm ước số chung lớn nhất (GCD) của tử số và mẫu số
            double factor = 1.0;
            double error = Math.Abs(factor - number);

            for (int i = 1; i <= 1000; i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    double currentFactor = (double)i / j;
                    double currentError = Math.Abs(currentFactor - number);

                    if (currentError < error)
                    {
                        factor = currentFactor;
                        error = currentError;
                        numerator = i;
                        denominator = j;
                    }

                    if (currentError < epsilon)
                    {
                        // Đạt được độ chính xác đủ, thoát vòng lặp
                        break;
                    }
                }
            }

            // Đơn giản hóa phân số bằng cách chia tử số và mẫu số cho GCD
            int gcd = GetGCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            if (numerator < denominator)
            {
                // Tính giá trị mới của tử số và mẫu số
                numerator1 = 1.0;
                denominator1 = Math.Ceiling(((double)denominator / (double)numerator) * 2) / 2; 
            }
            else
            {
                denominator1 = 1.0;
                numerator1 = Math.Floor(((double)numerator / (double)denominator) * 2) / 2;
                
            }

        }

        public static int GetGCD(int a, int b)
        {
            // Thuật toán Euclid để tìm ước số chung lớn nhất (GCD)
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        public static double RoundToNearestHalf(double value)
        {
            double roundedValue = Math.Round(value * 2) / 2;
            return roundedValue;
        }
        //-----------------------------------------------------------------------------------------------------
    }
}
