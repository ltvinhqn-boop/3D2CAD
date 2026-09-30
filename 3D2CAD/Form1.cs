using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using comp = NXOpen.Assemblies.Component;
using NXOpen;
//using NXOpen.;
using NXOpen.UF;
using NXOpen.Assemblies;
using NXOpen.BlockStyler;
using NXOpen.Drawings;
using NXOpen.Annotations;
using System.Security.Cryptography;
//using MiniSnap;
namespace _3D2CAD
{

    public partial class Form_auto : Form
    {
        static UI theUI = UI.GetUI();

        TaggedObject[] objects;
        // Process[] processes = Process.GetProcessesByName("ugraf");
        // Sử dụng nxHwnd cho các tương tác với cửa sổ chính của NX12
        static Session theSession = Session.GetSession();
        private static UFSession theUFSession = UFSession.GetUFSession();
        Part workPart = theSession.Parts.Work;
        private List<comp> allComponents = new List<comp>();
        private List<string> listnamecomponents = new List<string>();
        List<listdrafting> Listsheets = new List<listdrafting>();
        List<listcomp> ListDatafilter = new List<listcomp>();
        List<listfile> Listfile = new List<listfile>();
        List<listfilter> ListFilter = new List<listfilter>();
        listfilter Editsel;
        listdrafting Editsel1;
        string cue = "Chọn đối tượng xuất bản vẽ";
        string title = "Select Component";
        Selection.SelectionScope scope = Selection.SelectionScope.AnyInAssembly;
        Selection.SelectionAction action = Selection.SelectionAction.ClearAndEnableSpecific;
        bool includeFeatures = false;
        bool keepHighlighted = false;
        // static Selection.MaskTriple lineMask = new Selection.MaskTriple(UFConstants.UF_component_type, 0, 0);
        static Selection.MaskTriple lineMask = new Selection.MaskTriple(UFConstants.UF_component_type, 0, 0);
        Selection.MaskTriple[] maskArray = { lineMask };
        List<string> ColorItems = new List<string>() { "Select Color..." };
        private List<int> LayerItems;
        List<System.Windows.Forms.Button> buttonList;
        int rowIndex1 = 1;
        int colIndex1 = 1;
        string dateTimeString;
        
        string Viewname = "Vtech";
   

        //----------------------------------------

        public Form_auto()
        {
            InitializeComponent();
            NXOpenUI.FormUtilities.SetApplicationIcon(this);
            NXOpenUI.FormUtilities.ReparentForm(this);



        }

        private void Form_auto_Load(object sender, EventArgs e)
        {
                             
            buttonList = new List<System.Windows.Forms.Button>() { btn_bottom, btn_right, btn_parent, btn_left, btn_vback, btn_iso, btn_top, btn_tri };

            listnamecomponents = Getlistnamecomponent.Getlist(workPart, allComponents);
            LayerItems = Getlistnamecomponent.Getlayers(workPart, allComponents);
            //----------------------------------------------------------------
            NXOpen.ModelingViewCollection viewableCollection = workPart.ModelingViews;
            foreach (NXOpen.View view in viewableCollection)
            {
                NXOpen.ModelingView modelingView = view as NXOpen.ModelingView;
                if (modelingView != null)
                {
                    cbb_baseview.Items.Add(modelingView.Name);
                }
            }
            //InsertTB(tb1, tb1.Lines.Length, listnamecomponents[3]);
            //---------gán thuộc tính cho datagridview1
            cl_check.DataPropertyName = "Check";
            cl_name.DataPropertyName = "OComp";
            cl_type.DataPropertyName = "Type";
            cl_Qli.DataPropertyName = "Qty";
            cl_select.DataPropertyName = "Sel";
            cl_Color.DataPropertyName = "Color";
            cl_layer.DataPropertyName = "Layer";
            //---------gán thuộc tính cho datagridview3
            cl_check2.DataPropertyName = "Check";
            cl_properties.DataPropertyName = "Prop";
            cl_operator.DataPropertyName = "Operator";
            cl_value.DataPropertyName = "Value";
            //--------------------------------------
            CL2_Sheetname.DataPropertyName = "Name";
            CL2_size.DataPropertyName = "Size";
            CL2_template.DataPropertyName = "Template";
            CL2_baseview.DataPropertyName = "BaseView";
            CL2_projection.DataPropertyName = "Dop";
            CL2_scale.DataPropertyName = "Scale";
            CL2_part.DataPropertyName = "Part";
            //----------------------------------
            SH_check.DataPropertyName = "Check";
            SH_name.DataPropertyName = "Name";
            SH_sheet.DataPropertyName = "Sheet";
            //----------------------------------
            comboBox1.SelectedIndex = 0;

            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            cbb_baseview.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            
            if (Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\VTECH_TEMPLATES"))
            {
                // Lấy tất cả các tệp tin trong thư mục
                string[] fileNames = Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\VTECH_TEMPLATES");

                // Lặp qua từng tên tệp tin và thêm vào ComboBox
                foreach (string fileName in fileNames)
                {
                    string fileNameOnly = Path.GetFileName(fileName);
                    cbb_temp.Items.Add(fileNameOnly);
                }

                //  label10.Text = comboBox2.SelectedItem.ToString().Replace(" ", "").Split('-')[0];
                //   label9.Text= cbb_temp.Items.IndexOf(comboBox2.SelectedItem.ToString().Replace(" ", "").Split('-')[0],StringComparison.OrdinalIgnoreCase).ToString();
                //int index = cbb_temp.Items.IndexOf(comboBox2.SelectedItem.ToString().Replace(" ","").Split('-')[0]);
                // cbb_temp.SelectedIndex = index;
                List<string> items = cbb_temp.Items.Cast<string>().ToList();
                CL2_template.Items.AddRange(items.ToArray());
                List<string> items2 = cbb_baseview.Items.Cast<string>().ToList();
                CL2_baseview.Items.AddRange(items2.ToArray());
            }
            comboBox2.SelectedIndex = 1;
            button3.Text = (checkBox2.Checked) ? "Create list" : "Select";
            cbb_prop.SelectedIndex = 1;
            RegistryHelper.LoadAllSettings("VTECH", this);
            if (tabControl1.SelectedIndex == 1)
            {
                btn_ok.Enabled = true;
            }
            else
            {
                btn_ok.Enabled = false;
            }
            byte[] key = new byte[24];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            textBox1.Text = (string)RegistryHelper.GetSetting("VTECHKEY", "PRODUCTKEY", "");
            HardwareInfo hardwareInfo = new HardwareInfo();
            string productID = hardwareInfo.GenerateProductID();
            textBox2.Text = productID;
            // string keynot = productID + "|" + "20251010";
            byte[] TripleDESKey = new byte[] { 0x07, 0x04, 0x03, 0x2f, 0x05, 0x06, 0x07, 0xef, 0x09, 0x0A, 0xff, 0x0C, 0x0D, 0x0E, 0x0F, 0x4a };
            byte[] iv = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };           
            
            bool check=false;
            string datetime;
            double datetimetrial = double.Parse(hardwareInfo.DecryptLicenseKey("FWh2Z+DY9WIaJerg8PJ9mw==", TripleDESKey, iv));
            checklicense(textBox1.Text, productID,out datetime,out check);
            if(!check)
            {
                
                if (CHECKTIME.CHECKLICENSE(datetimetrial))
                {
                    tabControl1.TabPages.Remove(tabPage1);
                    customTableLayoutPanel2.Show();
                    tableLayoutPanel10.Show();
                    label16.Text = "License expiration:" + " " + "Trial";
                    lb_thongbao.Text = "";
                }
                else
                {
                    RegistryHelper.SaveSetting("VTECHKEY", "PRODUCTKEY", "");
                    customTableLayoutPanel2.Hide();
                    tableLayoutPanel10.Hide();
                    label16.Text = "License expiration:" + " " + datetime;
                    lb_thongbao.Text = "Giấy phép đã hết hạn!";
                    tabControl1.SelectedIndex = 3;
                }
            }
            else
            {
               // tabControl1.TabPages.Add(tabPage1);
                customTableLayoutPanel2.Show();
                tableLayoutPanel10.Show();
                label16.Text = "License expiration:" + " " + datetime;
                lb_thongbao.Text = "";
            }
            label19.Text = textBox2.Text;
            
            // textBox1.Text = hardwareInfo.EncryptLicenseKey(keynot, TripleDESKey1);
            // label16.Text = hardwareInfo.DecryptLicenseKey("9ECKyqJMg1SHIhVNqTOHDSu1tj75dDmF8yxVkynnO/A=", TripleDESKey,iv);
            //  textBox1.Text = hardwareInfo.ConvertStringToSecureCode(productID);
            // textBox1.Text = hardwareInfo.EncodeString("toilaai");
            //  textBox2.Text = hardwareInfo.DecodeString(hardwareInfo.EncodeString("toilaai"));
            // textBox1.Text= hardwareInfo.EncryptLicenseKey("MAMAY|");
           // label24.Text = hardwareInfo.GetHDDSerialNumbers();
           // label1.Text = "Object type:";
        }
        private static void checklicense(string LicenseKey,string ProductId,out string datetime , out bool result)
        {
            try
            {
                byte[] TripleDESKey = new byte[] { 0x07, 0x04, 0x03, 0x2f, 0x05, 0x06, 0x07, 0xef, 0x09, 0x0A, 0xff, 0x0C, 0x0D, 0x0E, 0x0F, 0x4a };
                byte[] iv = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                result = false;
                HardwareInfo hardwareInfo = new HardwareInfo();
                string keydecryp = hardwareInfo.DecryptLicenseKey(LicenseKey, TripleDESKey, iv);
                string productID1 = keydecryp.Split('|')[0];
                double datetime1 = double.Parse(keydecryp.Split('|')[1]);
                if (productID1.Length == 29 && productID1 == ProductId && CHECKTIME.CHECKLICENSE(datetime1))
                {

                    // result = productID1 == ProductId && CHECKTIME.CHECKLICENSE(datetime1) ? true : false;
                    result = true;
                    datetime = datetime1.ToString().Insert(4, "/").Insert(7, "/");
                }
                else
                {
                   
                    result = false;
                    datetime = "2000/01/01";
                }
            }
            catch
            {
                result = false;
                datetime = "2000/01/01";
            }
         
        }
        private void AutoDrafting() 
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {


        }
        public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }





        private void tb1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //DialogResult dlr = MessageBox.Show("Bạn muốn xóa tất cả", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (dlr == DialogResult.Yes)
            //{

            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<listcomp> ListData = new List<listcomp>();
            if (checkBox2.Checked==false)
            {
                var selMgr = theUI.SelectionManager;
                // Selection.MaskTriple lineMask = new Selection.MaskTriple(0, 0, 0);
                // Selection.MaskTriple[] maskArray = { lineMask };
                Selection.Response response = selMgr.SelectTaggedObjects(cue, title, scope, action,
             includeFeatures, keepHighlighted, maskArray, out objects);

                //  int i = 0;

                if (response != Selection.Response.Cancel)
                {
                    // NXOpen.Assemblies.Component[] components = (NXOpen.Assemblies.Component[])workPart.ComponentAssembly.RootComponent.GetChildren();


                    foreach (TaggedObject obj in objects)
                    {
                        string typecomp = "";
                        NXOpen.Part part1;

                        NXOpen.Assemblies.Component comp = obj as NXOpen.Assemblies.Component;
                        part1 = comp.Prototype as NXOpen.Part;
                        if (comp.GetChildren().Count() == 0)
                        {
                            typecomp = "Part";

                        }
                        else
                        {
                            typecomp = "Assembly";


                        }

                        // Part part1 = NXOpen.Utilities.NXObjectManager.Get(obj.Tag) as Part;

                        //  NXOpen.Part  part1 = obj as Part;

                        if (comp != null && part1 != null)
                        {

                            if (comboBox1.SelectedItem.ToString() == "None")
                            {

                                // i++;
                                string componentName = part1.Name;
                                //   InsertTb.InsertTB(textBox2, textBox2.Lines.Length, componentName);



                                ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                                // InsertTB(tb1, tb1.Lines.Length, new listcomp(i.ToString(), componentName, "1", "1").No);
                            }
                            else if (comboBox1.SelectedItem.ToString() == "Assembly")
                            {

                                // i++;
                                string componentName = part1.Name;
                                if (comp.GetChildren().Count() > 0)
                                {
                                    typecomp = "Assembly";
                                    ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                                }


                            }
                            else
                            {

                                // i++;
                                string componentName = part1.Name;
                                if (comp.GetChildren().Count() == 0)
                                {
                                    typecomp = "Part";
                                    ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (comp comp in allComponents)
                {
                    string typecomp = "";
                    NXOpen.Part part1;

                    //  NXOpen.Assemblies.Component comp = obj as NXOpen.Assemblies.Component;
                    part1 = comp.Prototype as NXOpen.Part;
                    if (comp.GetChildren().Count() == 0)
                    {
                        typecomp = "Part";

                    }
                    else
                    {
                        typecomp = "Assembly";


                    }

                    // Part part1 = NXOpen.Utilities.NXObjectManager.Get(obj.Tag) as Part;

                    //  NXOpen.Part  part1 = obj as Part;

                    if (comp != null && part1 != null)
                    {

                        if (comboBox1.SelectedItem.ToString() == "None")
                        {

                            // i++;
                            string componentName = part1.Name;
                            //   InsertTb.InsertTB(textBox2, textBox2.Lines.Length, componentName);



                            ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                            // InsertTB(tb1, tb1.Lines.Length, new listcomp(i.ToString(), componentName, "1", "1").No);
                        }
                        else if (comboBox1.SelectedItem.ToString() == "Assembly")
                        {

                            // i++;
                            string componentName = part1.Name;
                            if (comp.GetChildren().Count() > 0)
                            {
                                typecomp = "Assembly";
                                ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                            }


                        }
                        else
                        {

                            // i++;
                            string componentName = part1.Name;
                            if (comp.GetChildren().Count() == 0)
                            {
                                typecomp = "Part";
                                ListData.Add(new listcomp(true, componentName, typecomp, Getlistnamecomponent.CountElements(listnamecomponents, componentName).ToString(), comboBox1.SelectedItem.ToString(), comp.Color.ToString(), comp.Layer.ToString(), part1));
                            }
                        }
                    }
                }
            }
            
                // dataGridView1.Rows.Clear();
                ListData = RemoveDuplicates(ListData, (x, y) => x.OComp == y.OComp);

            // ListDatafilter.AddRange(FilterComponent(ListFilter, ListData));
            ListDatafilter = (ListFilter.Count() > 0) ? ListData.Where(data => !FilterComponent(ListFilter, ListData).Any(removeData => removeData.OComp == data.OComp)).ToList() : ListData;

               // ListDatafilter = RemoveDuplicates(ListDatafilter, (x, y) => x.OComp == y.OComp);
            dataGridView1.DataSource = new List<listcomp>();
                dataGridView1.DataSource = ListDatafilter;
                int displayedRowCount = dataGridView1.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
                // dataGridView1.Refresh();
            }
            //UFSession ufSession = UFSession.GetUFSession();
            //UFUi ufUi = ufSession.Ui;

        
        //------Hàm lấy tất cả đối tượng
        private static void GetAllComponents(comp parentComponent, List<comp> allComponents)
        {
            allComponents.Add(parentComponent);

            comp[] childComponents = parentComponent.GetChildren();
            foreach (comp childComponent in childComponents)
            {
                GetAllComponents(childComponent, allComponents);
            }
        }
        //----------------------------------------------------------------------------Lop quan ly danh sach đoi tuong chon
        private class listdrafting
        {

            public string Name { get; set; }
            public string Size { get; set; }
            public string Template { get; set; }
            public string Dop { get; set; }
            public string BaseView { get; set; }
            public List<bool> Enable { get; set; }
            public string Scale { get; set; }
            public NXOpen.Part Part { get; set; }

            public listdrafting(string Name1, string Size1, string Template1, string BaseView1, string Dop1, List<bool> Enable1, string Scale1, NXOpen.Part Part1)
            {
                // No = No1;

                Name = Name1;
                Size = Size1;
                Template = Template1;
                BaseView = BaseView1;
                Dop = Dop1;
                Enable = Enable1;
                Scale = Scale1;
                Part = Part1;
            }
        }
        //---------------------------------------------------------------------------------
        //----------------------------------------------------------------------------Lop quan ly danh sach đoi tuong chon
        private class listcomp
        {
            public bool Check { get; set; }
            public string OComp { get; set; }
            public string Type { get; set; }
            public string Sel { get; set; }
            public string Qty { get; set; }
            public string Color { get; set; }
            public string Layer { get; set; }
            public NXOpen.Part Part { get; set; }

            public listcomp(bool check1, string OComp1, string Type1, string Qty1, string Sel1, string Color1, string Layer1, NXOpen.Part Part1)
            {
                // No = No1;
                Check = check1;
                OComp = OComp1;
                Type = Type1;
                Sel = Sel1;
                Qty = Qty1;
                Color = Color1;
                Layer = Layer1;
                Part = Part1;
            }
        }
        //---------------------------------------------------------------------------------
        //----------------------------------------------------------------------------Lop quan ly danh sach loc filter
        private class listfilter
        {
            public bool Check { get; set; }
            public string Prop { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }



            public listfilter(bool check1, string Prop1, string Operator1, string Value1)
            {
                // No = No1;
                Check = check1;
                Prop = Prop1;
                Operator = Operator1;
                Value = Value1;

            }
        }
        //---------------------------------------------------------------------------------
        private void button4_Click(object sender, EventArgs e)
        {

            ListDatafilter = new List<listcomp>();
            dataGridView1.DataSource = new List<listcomp>();
        }

        private void btn_sel_Click(object sender, EventArgs e)
        {

        }
        //----------------------------
        private List<listcomp> FilterComponent(List<listfilter> ListFiter1, List<listcomp> ListComp)
        {
            List<listcomp> result = new List<listcomp>();
            List<listcomp> listname = new List<listcomp>();
            List<listcomp> listcolor = new List<listcomp>();
            List<listcomp> listlayer = new List<listcomp>();
            if (ListFiter1.Count <= 0 || ListFiter1.All(filter => filter.Check == false))
            {
               
            }
            else
            {

                foreach (listfilter Filter1 in ListFiter1)
                {
                    if (Filter1.Check == true)
                    {
                        switch (Filter1.Operator)
                        {
                            case "= Equals":
                                switch (Filter1.Prop)
                                {
                                    case "Name":
                                        List<listcomp> filteredList1 = ListComp.Where(comp => comp.OComp.IndexOf(Filter1.Value, StringComparison.OrdinalIgnoreCase) < 0).ToList();
                                        listname.AddRange(filteredList1);
                                        break;
                                    case "Layer":
                                        List<listcomp> filteredList2 = ListComp.Where(comp => comp.Layer != Filter1.Value).ToList();
                                        listname.AddRange(filteredList2);
                                        break;
                                    case "Color":
                                        List<listcomp> filteredList3 = ListComp.Where(comp => comp.Color != Filter1.Value).ToList();
                                        listname.AddRange(filteredList3);
                                        break;
                                }

                                break;
                            case "<> Not Equal":
                                switch (Filter1.Prop)
                                {
                                    case "Name":
                                        List<listcomp> filteredList1 = ListComp.Where(comp => comp.OComp.IndexOf(Filter1.Value, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                                        listname.AddRange(filteredList1);
                                        break;
                                    case "Layer":
                                        List<listcomp> filteredList2 = ListComp.Where(comp => comp.Layer == Filter1.Value).ToList();
                                        listname.AddRange(filteredList2);
                                        break;
                                    case "Color":
                                        List<listcomp> filteredList3 = ListComp.Where(comp => comp.Color == Filter1.Value).ToList();
                                        listname.AddRange(filteredList3);
                                        break;
                                }
                                break;
                        }
                    }
                }
                result.AddRange(listname);
                result.AddRange(listcolor);
                result.AddRange(listlayer);
            }
            //  result= RemoveDuplicates(result, (x, y) => x.OComp == y.OComp);

            return result;
        }
        //------------------------------

        private void label2_Click(object sender, EventArgs e)
        {

        }
        //-----------------------------------hàm loại bỏ đối tượng trùng lặp
        public static List<T> RemoveDuplicates<T>(List<T> list, Func<T, T, bool> comparer)
        {
            List<T> distinctList = new List<T>();
            foreach (T item in list)
            {
                bool isDuplicate = distinctList.Any(x => comparer(x, item));
                if (!isDuplicate)
                {
                    distinctList.Add(item);
                }
            }
            return distinctList;

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbb_prop.SelectedItem != null)
            {
                switch (cbb_prop.SelectedItem.ToString())
                {
                    case "Color":
                        cbb_value.DropDownStyle = ComboBoxStyle.DropDownList;
                        cbb_value.Items.Clear();
                        cbb_value.Items.AddRange(ColorItems.ToArray());
                        cbb_ope.SelectedIndex = 0;
                        cbb_value.SelectedIndex = 0;
                        break;
                    case "Layer":
                        cbb_value.DropDownStyle = ComboBoxStyle.DropDownList;
                        cbb_value.Items.Clear();
                        cbb_value.Items.AddRange(LayerItems.ConvertAll(x => x.ToString()).ToArray());
                        cbb_ope.SelectedIndex = 0;
                        if (cbb_value.Items.Count > 0)
                        { cbb_value.SelectedIndex = 0; }
                        break;
                    case "Name":
                        cbb_value.DropDownStyle = ComboBoxStyle.Simple;
                        cbb_ope.SelectedIndex = 0;
                        break;

                }

            }
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (comboBox2.SelectedValue as string == "Select Color...")
            if (cbb_value.SelectedItem != null)
            {
                if (cbb_value.SelectedItem.ToString() == "Select Color...")
                {

                    PickColor thePickColor = null;
                    try
                    {
                        thePickColor = new PickColor();
                        // The following method shows the dialog immediately
                        thePickColor.Show();
                        cbb_value.Items.Insert(cbb_value.Items.Count - 1, thePickColor.Color_id());
                        cbb_value.SelectedIndex = cbb_value.Items.Count - 2;
                        ColorItems = cbb_value.Items.Cast<string>().ToList();

                        // textBox1.Text= thePickColor.
                    }
                    catch (Exception ex)
                    {
                        //---- Enter your exception handling code here -----
                        theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
                    }
                    finally
                    {
                        if (thePickColor != null)
                            thePickColor.Dispose();

                        thePickColor = null;

                    }
                }
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (cbb_prop.Text != "" && cbb_ope.Text != "" && cbb_value.Text != "")
            {
                ListFilter.Add(new listfilter(true, cbb_prop.Text, cbb_ope.Text, cbb_value.Text));
                ListFilter = RemoveDuplicates(ListFilter, (x, y) => x.Prop == y.Prop && x.Operator == y.Operator && x.Value == y.Value);
                dataGridView3.DataSource = new List<listfilter>();
                dataGridView3.DataSource = ListFilter;
                int displayedRowCount = dataGridView3.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
            }

        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            // Khai báo một danh sách các chỉ mục hàng
            List<int> selectedRows = new List<int>();

            // Lặp qua từng ô được chọn
            foreach (DataGridViewCell cell in dataGridView3.SelectedCells)
            {
                // Lấy chỉ mục hàng của ô
                int rowIndex = cell.RowIndex;

                // Kiểm tra xem chỉ mục hàng đã tồn tại trong danh sách chưa
                if (!selectedRows.Contains(rowIndex))
                {
                    // Thêm chỉ mục hàng vào danh sách
                    selectedRows.Add(rowIndex);
                }
            }
            if (selectedRows != null)
            {
                // Xóa các đối tượng từ danh sách dựa trên danh sách chỉ mục
                selectedRows.Sort(); // Sắp xếp danh sách chỉ mục theo thứ tự tăng dần
                for (int i = selectedRows.Count - 1; i >= 0; i--)
                {
                    int index = selectedRows[i];
                    ListFilter.RemoveAt(index);
                }

                dataGridView3.DataSource = new List<listfilter>();
                dataGridView3.DataSource = ListFilter;

                int displayedRowCount = dataGridView3.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
            }
        }

        private void dataGridView3_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int allowedColumnIndex = 1; // Chỉ mục của cột được cho phép chỉnh sửa

            if (e.ColumnIndex != allowedColumnIndex)
            {
                e.Cancel = true; // Hủy bỏ sự kiện chỉnh sửa của ô không phù hợp
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int allowedColumnIndex = 1; // Chỉ mục của cột được cho phép chỉnh sửa

            if (e.ColumnIndex != allowedColumnIndex)
            {
                e.Cancel = true; // Hủy bỏ sự kiện chỉnh sửa của ô không phù hợp
            }
        }

        private void cbb_prop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // int rowIndex = e.RowIndex; // Lấy vị trí hàng
        }

        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex; // Lấy vị trí hàng
            if (ListFilter.Count > 0)
            {
                Editsel = ListFilter[rowIndex];
                cbb_prop.SelectedItem = Editsel.Prop;
                cbb_ope.SelectedItem = Editsel.Operator;
                if (cbb_prop.SelectedItem == "Name")
                {
                    cbb_value.Text = Editsel.Value;
                }
                else
                {
                    cbb_value.SelectedItem = Editsel.Value;
                }
            }

        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (Editsel != null && ListFilter.IndexOf(Editsel) != -1)
            {
                ListFilter[ListFilter.IndexOf(Editsel)] = new listfilter(Editsel.Check, cbb_prop.Text, cbb_ope.Text, cbb_value.Text);
                dataGridView3.DataSource = new List<listfilter>();
                dataGridView3.DataSource = ListFilter;
                int displayedRowCount = dataGridView3.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
                Editsel = null;
            }
        }

        private void btn_checkall_Click(object sender, EventArgs e)
        {
            ListDatafilter.ForEach(item => item.Check = true);
            dataGridView1.DataSource = new List<listcomp>();
            dataGridView1.DataSource = ListDatafilter;
            int displayedRowCount = dataGridView1.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

        }

        private void btn_uncheck_Click(object sender, EventArgs e)
        {
            ListDatafilter.ForEach(item => item.Check = false);
            dataGridView1.DataSource = new List<listcomp>();
            dataGridView1.DataSource = ListDatafilter;
            int displayedRowCount = dataGridView1.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {/*
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "NX File|*.Prt";
            openFileDialog.Title = "Select file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                cbb_temp.SelectedText = openFileDialog.FileName;

            }*/
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedItem.ToString())
            {
                case "A0 - 841x1189":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "841";
                    textBox4.Text = "1189";
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    break;
                case "A1 - 594x841":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "594";
                    textBox4.Text = "841";
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    break;
                case "A2 - 420x594":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "420";
                    textBox4.Text = "594";
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    break;
                case "A3 - 297x420":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "297";
                    textBox4.Text = "420";
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    break;
                case "A4 - 210x297":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "210";
                    textBox4.Text = "297";
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    break;
                case "Custom size":
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    textBox3.Text = "297";
                    textBox4.Text = "420";
                    textBox3.ReadOnly = false;
                    textBox4.ReadOnly = false;
                    break;
            }
            int i = 0;
            //int index;
            foreach (var item in cbb_temp.Items)
            {
                if (cbb_temp.Items.Count == i)
                {
                    cbb_temp.SelectedIndex = 0;
                    break;
                }
                string itemString = item as string;
                if (itemString.Contains(comboBox2.SelectedItem.ToString().Replace(" ", "").Split('-')[0]))
                {
                    cbb_temp.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private static bool IsValidInput(string text)
        {
            // Kiểm tra ký tự nhập vào có phải là số, dấu ":" hoặc theo đúng thứ tự không
            return Regex.IsMatch(text, @"^\d*:\d*$");
        }

        private static bool IsValidFormat(string text)
        {
            // Kiểm tra định dạng tổng quát (số nguyên dương:dấu:số nguyên dương)
            return Regex.IsMatch(text, @"^\d+:\d+$");
        }

        private void comboBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            ComboBox comboBox3 = (ComboBox)sender;
            string text = comboBox3.Text + e.KeyChar;

            // Kiểm tra nếu ký tự nhập vào không phải là số, không phải là dấu ":" hoặc không theo đúng thứ tự
            if (!char.IsControl(e.KeyChar) && !IsValidInput(text))
            {
                e.Handled = true; // Không cho phép hiển thị ký tự đó trong ComboBox
            }
        }

        private void comboBox3_Validating(object sender, CancelEventArgs e)
        {
            ComboBox comboBox3 = (ComboBox)sender;
            string text = comboBox3.Text;

            // Kiểm tra nếu giá trị nhập vào không đúng định dạng
            if (!string.IsNullOrEmpty(text) && !IsValidFormat(text))
            {
                // MessageBox.Show("Vui lòng nhập theo định dạng số nguyên dương:dấu:số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true; // Hủy sự kiện để giữ lại trạng thái nhập liệu trước đó
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn chặn ký tự không phải số được hiển thị
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn chặn ký tự không phải số được hiển thị
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ListDatafilter.All(name => name.OComp != workPart.Name))
            {
                ListDatafilter.Insert(0, new listcomp(true, workPart.Name, "Assembly", "-", "-", "-", "-", workPart));

                dataGridView1.DataSource = new List<listcomp>();
                dataGridView1.DataSource = ListDatafilter;
                int displayedRowCount = dataGridView1.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
                dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.PaleGoldenrod;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btn_ok_Click(object sender, EventArgs e)
        { //int i = 0;
            List<NXOpen.Part> listpart = new List<NXOpen.Part>();
            //List<string> thislist = new List<string>();
            //foreach (listcomp li in ListDatafilter)
            //{
            //    listpart.Add(li.Part);
            //    thislist.Add(li.OComp);
            //}
            //string[] listop = thislist.ToArray();
            //string pathtemp = System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\VTECH_TEMPLATES", cbb_temp.SelectedItem.ToString());
            //---------------------------xóa sheet
            theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            DrawingSheet[] sheets = workPart.DrawingSheets.ToArray();
            foreach (DrawingSheet she in sheets)
            {
                var matchingItem = Listsheets.FirstOrDefault(item => item.Name == she.Name);
                if (matchingItem != null)
                {
                    DeleteSheet(true, she.Name, theSession, workPart);
                }
                //InsertTb.InsertTB(tb1, tb1.Lines.Length, she.Name);
               
            }
            foreach (listdrafting Listsheet1 in Listsheets)
            {
                
                int index;
                if (Listsheet1.Dop == "Third")
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
               // string standard1 = (checkBox3.Checked) ? "VTECH01" : "VTECH02";
              //  string standard2 = "VTECH02";
                //---------------------
                double H = Double.Parse(Listsheet1.Size.Replace(" ", "").Split('x', '-')[1]);
                double W = Double.Parse(Listsheet1.Size.Replace(" ", "").Split('x', '-')[2]);

                string pathtemp = System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\VTECH_TEMPLATES", Listsheet1.Template.ToString());
                // textBox1.Text = listop[0];
                CreateView.Create3View(theSession, workPart, Listsheet1.Name, pathtemp, Listsheet1.Part, H, W, Listsheet1.Scale, index, Listsheet1.BaseView, Listsheet1.Enable,!checkBox3.Checked,checkBox4.Checked);
            }
            theSession.ApplicationSwitchImmediate("UG_APP_MODELING");

            workPart.Drafting.ExitDraftingApplication();
        }

        private void comboBox4_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 0)
            {
                tableLayoutPanel3.Controls.Clear();
                tableLayoutPanel3.Controls.Add(buttonList[0], 1, 0);
                tableLayoutPanel3.Controls.Add(buttonList[1], 0, 1);
                tableLayoutPanel3.Controls.Add(buttonList[2], 1, 1);
                tableLayoutPanel3.Controls.Add(buttonList[3], 2, 1);
                tableLayoutPanel3.Controls.Add(buttonList[4], 3, 1);
                tableLayoutPanel3.Controls.Add(buttonList[5], 0, 2);
                tableLayoutPanel3.Controls.Add(buttonList[6], 1, 2);
                tableLayoutPanel3.Controls.Add(buttonList[7], 2, 2);
                // buttonList[0]
            }
            else
            {
                tableLayoutPanel3.Controls.Clear();
                tableLayoutPanel3.Controls.Add(buttonList[0], 1, 2);
                tableLayoutPanel3.Controls.Add(buttonList[1], 2, 1);
                tableLayoutPanel3.Controls.Add(buttonList[2], 1, 1);
                tableLayoutPanel3.Controls.Add(buttonList[3], 0, 1);
                tableLayoutPanel3.Controls.Add(buttonList[4], 3, 1);
                tableLayoutPanel3.Controls.Add(buttonList[5], 2, 0);
                tableLayoutPanel3.Controls.Add(buttonList[6], 1, 0);
                tableLayoutPanel3.Controls.Add(buttonList[7], 0, 0);
            }
        }

        private void btn_bottom_Click(object sender, EventArgs e)
        {
            if (btn_bottom.BackColor == Color.White)
            {
                btn_bottom.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_bottom.BackColor = Color.White;
            }
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (btn_right.BackColor == Color.White)
            {
                btn_right.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_right.BackColor = Color.White;
            }
        }

        private void btn_parent_Click(object sender, EventArgs e)
        {

            btn_parent.BackColor = Color.FromArgb(255, 224, 192);
           

        }

        private void btn_left_Click(object sender, EventArgs e)
        {
            if (btn_left.BackColor == Color.White)
            {
                btn_left.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_left.BackColor = Color.White;
            }
        }

        private void btn_vback_Click(object sender, EventArgs e)
        {
            if (btn_vback.BackColor == Color.White)
            {
                btn_vback.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_vback.BackColor = Color.White;
            }
           // checkBox1.Text = btn_vback.BackColor.ToString();
        }

        private void btn_iso_Click(object sender, EventArgs e)
        {
            if (btn_iso.BackColor == Color.White)
            {
                btn_iso.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_iso.BackColor = Color.White;
            }
        }

        private void btn_top_Click(object sender, EventArgs e)
        {
            if (btn_top.BackColor == Color.White)
            {
                btn_top.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_top.BackColor = Color.White;
            }
        }

        private void btn_tri_Click(object sender, EventArgs e)
        {
            if (btn_tri.BackColor == Color.White)
            {
                btn_tri.BackColor = Color.FromArgb(255, 224, 192);
            }
            else
            {
                btn_tri.BackColor = Color.White;
            }
        }

        private void btn_bottom_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_bottom, "Bottom");
        }

        private void btn_right_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_right, "Right");
        }

        private void btn_parent_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_parent, "Parent");
        }

        private void btn_left_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_left, "Left");
        }

        private void btn_vback_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_vback, "Back");
        }

        private void btn_iso_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_iso, "Isometric");
        }

        private void btn_top_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_top, "Top");
        }

        private void btn_tri_MouseHover(object sender, EventArgs e)
        {
            toolTip2.SetToolTip(btn_tri, "Trimetric");
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
                  //--------------------------------------------------------------------------------------------------------
            List<bool> enableview = new List<bool>();
            bool abc;
            foreach (System.Windows.Forms.Button btn1 in buttonList)
            {
                if (btn1.BackColor == Color.White)
                {
                    abc = false;
                    enableview.Add(abc);
                }
                else
                {
                    abc = true;
                    enableview.Add(abc);
                }
            }
            enableview.Insert(0, false);
            enableview.Insert(2, false);
            enableview.Insert(3, false);
            enableview.Insert(11, false);
            //---------------------------------------------------------------------------------------------------------
           

            
            
            //--------------------------------------------------------------------------------------------------------
            Listsheets = new List<listdrafting>();
            
            foreach (listcomp Listcomp1 in ListDatafilter)
            {
                

                if (Listcomp1.Check == true)
                {
                    listdrafting Lid1 = new listdrafting(Listcomp1.OComp, comboBox2.SelectedItem.ToString(), cbb_temp.SelectedItem.ToString(), cbb_baseview.SelectedItem.ToString(), comboBox4.SelectedItem.ToString(), enableview, comboBox3.SelectedItem.ToString(), Listcomp1.Part);
                    Listsheets.Add(Lid1);
                }
                
            }
            dataGridView2.DataSource = new List<listdrafting>();
            dataGridView2.DataSource = Listsheets;
            int displayedRowCount = dataGridView2.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView2.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thể chuyển đến tab kế tiếp hay không
            if (tabControl1.SelectedIndex < tabControl1.TabCount - 2)
            {
                tabControl1.SelectedIndex += 1; // Chuyển đến tab kế tiếp
            }
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thể quay lại tab trước hay không
            if (tabControl1.SelectedIndex > 0)
            {
                tabControl1.SelectedIndex -= 1; // Quay lại tab trước
            }
        }

        private void btn_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //-------------------------------------------------------------------------------
        //private List<string> LayersItems()
        //{
        //    NXOpen.Layer layer = workPart.Layers.CurrentLayer;

        //    Category category = layer.Category;
        //    Layer[] memberLayers = category.GetMemberLayers();
        //            A4 - 210x297
        //A3 - 297x420
        //A2 - 420x594
        //A1 - 594x841
        //A0 - 841x1189
        //  Custom size

        //}


        //----------------------------------------------------------------------------
        private static void DeleteSheet(bool delete, string sheetnamedel, Session theSession, Part workPart)
        {
            if (delete)
            {
                string aa = sheetnamedel + "@0";
                NXOpen.TaggedObject[] objects1 = new NXOpen.TaggedObject[1];
                NXOpen.View view1 = (NXOpen.View)workPart.Views.FindObject(aa);
                objects1[0] = view1;
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

           



        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    BeginInvoke(new Action(() =>
                    {

                        rowIndex1 = e.RowIndex;
                        colIndex1 = e.ColumnIndex;

                        switch (colIndex1)
                        {
                            case 1:
                                Listsheets.ForEach(item => item.Name = dataGridView2.Rows[rowIndex1].Cells[colIndex1].Value.ToString());
                                break;
                            case 2:
                                Listsheets.ForEach(item1 => item1.Size = dataGridView2.Rows[rowIndex1].Cells[colIndex1].Value.ToString());
                                break;
                            case 3:
                                Listsheets.ForEach(item2 => item2.Template = dataGridView2.Rows[rowIndex1].Cells[colIndex1].Value.ToString());
                                break;
                            case 4:
                                Listsheets.ForEach(item4 => item4.Dop = dataGridView2.Rows[rowIndex1].Cells[colIndex1].Value.ToString());
                                break;
                            case 5:
                                Listsheets.ForEach(item3 => item3.BaseView = dataGridView2.Rows[rowIndex1].Cells[colIndex1].Value.ToString());

                                break;

                        }
                        dataGridView2.DataSource = new List<listdrafting>();
                        dataGridView2.DataSource = Listsheets;
                        int displayedRowCount = dataGridView2.RowCount;
                        for (int i = 0; i < displayedRowCount; i++)
                        {
                            dataGridView2.Rows[i].Cells[0].Value = (i + 1).ToString();
                        }
                    }));
                }
            }
            catch { };

        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // int allowedColumnIndex = 1; // Chỉ mục của cột được cho phép chỉnh sửa

            if (e.ColumnIndex == 1 || e.ColumnIndex == 0)
            {
                e.Cancel = true; // Hủy bỏ sự kiện chỉnh sửa của ô không phù hợp
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Editsel1 != null && Listsheets.IndexOf(Editsel1) != -1)
            {
                List<bool> enableview = new List<bool>();
                bool abc;
                foreach (System.Windows.Forms.Button btn1 in buttonList)
                {
                    if (btn1.BackColor == Color.White)
                    {
                        abc = false;
                        enableview.Add(abc);
                    }
                    else
                    {
                        abc = true;
                        enableview.Add(abc);
                    }
                }
                enableview.Insert(0, false);
                enableview.Insert(2, false);
                enableview.Insert(3, false);
                enableview.Insert(11, false);

                Listsheets[Listsheets.IndexOf(Editsel1)] = new listdrafting(Editsel1.Name, comboBox2.SelectedItem.ToString(), cbb_temp.SelectedItem.ToString(), cbb_baseview.SelectedItem.ToString(), comboBox4.SelectedItem.ToString(), enableview, comboBox3.SelectedItem.ToString(), Editsel1.Part);
                dataGridView2.DataSource = new List<listdrafting>();
                dataGridView2.DataSource = Listsheets;
                int displayedRowCount = dataGridView2.RowCount;
                for (int i = 0; i < displayedRowCount; i++)
                {
                    dataGridView2.Rows[i].Cells[0].Value = (i + 1).ToString();
                }
                Editsel1 = null;
            }

        }

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
                       
           
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex; // Lấy vị trí hàng
            if (Listsheets.Count > 0)
            {
                Editsel1 = Listsheets[rowIndex];
                comboBox2.SelectedItem = Editsel1.Size;
                cbb_temp.SelectedItem = Editsel1.Template;
                comboBox4.SelectedItem = Editsel1.Dop;
                cbb_baseview.SelectedItem = Editsel1.BaseView;
                // List<Color> listcolor = new List<Color> { };
                List<Color> colorList = Editsel1.Enable.Select(value => value ? Color.FromArgb(255, 224, 192) : Color.White).ToList();
                buttonList[0].BackColor = colorList[1];
                buttonList[1].BackColor = colorList[4];
                buttonList[2].BackColor = colorList[5];
                buttonList[3].BackColor = colorList[6];
                buttonList[4].BackColor = colorList[7];
                buttonList[5].BackColor = colorList[8];
                buttonList[6].BackColor = colorList[9];
                buttonList[7].BackColor = colorList[10];

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button3.Text = (checkBox2.Checked) ? "Create list" : "Select";
        }

        private void button6_Click(object sender, EventArgs e)
        {
        //    DateTime now = DateTime.Now;
        //    dateTimeString = now.ToString("mm ss fff").Replace(" ", "");

        //    //----------------------------------------------------------------------------------------------------------

        //    //  sCreateCustomModellingView(workPart, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, "myview", 1);
        //    //  NXOpen.ViewableCollection viewableCollection = workPart.ModelingViews;
        //    LookAt.orientcustomtool();       
         
               
               
        //    Viewcustom = workPart.Views.SaveAs(workPart.ModelingViews.WorkView, dateTimeString, false, false);
            
       
            //cbb_baseview.Items.Add(dateTimeString);
            //cbb_baseview.SelectedItem = dateTimeString;
            //CL2_baseview.Items.Add(dateTimeString);
         //   Viewname = Viewcustom.Name;

            //int viewType1 = 0;
            ////1 = modeling view
            ////2 = drawing view
            ////other = error
            //theUFSession.Draw.AskDisplayState(out viewType1);
            //theUFSession.Draw.SetDisplayState(viewType1);
            //NXOpen.ModelingViewCollection viewableCollection = workPart.ModelingViews;
            //foreach (NXOpen.View view in viewableCollection)
            //{
            //    NXOpen.ModelingView modelingView = view as NXOpen.ModelingView;
            //    if (modelingView != null)
            //    {
            //        InsertTb.InsertTB(textBox1, textBox1.Lines.Length, modelingView.Name);
            //    }
            //}
            //   workPart.ModelingViews.WorkView.Orient
            //----------------------------------------------------------------------------------------------------------
        }

        private void Form_auto_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistryHelper.SaveAllSettings("VTECH", this);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                btn_ok.Enabled = true;
            }
            else
            {
                btn_ok.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the file output folder";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (cbb_output.Items.Count > 0)
                {
                    //textBox2.Text = folderBrowserDialog.SelectedPath;
                    cbb_output.Items[0] = folderBrowserDialog.SelectedPath;

                }
                else
                {
                    cbb_output.Items.Add(folderBrowserDialog.SelectedPath);
                }
                cbb_output.SelectedIndex = 0;
               
            }
        }
        private class listfile
        {
            public bool Check { get; set; }
            public string Name { get; set; }
            // public string Pathfile { get; set; }
            public DrawingSheet Sheet { get; set; }


            public listfile(bool check1, string Name1, DrawingSheet Sheet1)
            {
                // No = No1;
                Check = check1;
                Name = Name1;
                Sheet = Sheet1;
              //  Pathfile = Pathfile1;

            }
        }

        private void btn_insertlist_Click(object sender, EventArgs e)
        {
            
            DrawingSheet[] sheets = workPart.DrawingSheets.ToArray();
            foreach (DrawingSheet she in sheets)
            {
                listfile file = new listfile(true, she.Name, she);
                Listfile.Add(file);
                
               
            }
            dataGridView4.DataSource = new List<listcomp>();
            dataGridView4.DataSource = Listfile;
            int displayedRowCount = dataGridView4.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
            SH_sheet.Visible = false;
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
           
            if (Directory.Exists(cbb_output.Text))
            {
                string ex = comboBox5.Text;
                string scale;
                int i = 1;
                foreach (listfile lif in Listfile)
                {
                    DateTime now = DateTime.Now;
                    string dateTimeString = now.ToString(" HH mm ss fff").Replace(" ", "");
                    if (lif.Check)
                    {
                        var draftingViews = lif.Sheet.GetDraftingViews();
                        double exportScale = draftingViews[0].Style.General.Scale;
                        scale = exportScale + ":" + 1;
                        //foreach (DraftingView view in lif.Sheet.GetDraftingViews)
                        //{
                        //    DraftingView baseview = view;
                        //    label17.Text = view.Scale.ToString();
                        //    break; // Take the first view
                        //}

                        string outputfilename = Path.Combine(cbb_output.Text,i+"_"+ lif.Name +"."+ ex);
                       // outputfilename=(File.Exists(outputfilename)) ? Path.Combine(outputfilename.Spli, dateTimeString) : outputfilename;
                        EXPORTDWG.ExportDrawingSheetToDxf(lif.Sheet, outputfilename, workPart, (comboBox5.Text=="dwg")?true:false, scale);
                        i++;
                    }
                }
                DeleteZeroByteFiles(cbb_output.Text);


            }
            else
            {
                theUI.NXMessageBox.Show("Output Folder", NXMessageBox.DialogType.Error, "The directory does not exist");
            }
            SH_sheet.Visible = false;
        }
        public void DeleteZeroByteFiles(string folderPath)
        {
            // Lấy danh sách tất cả các tệp tin trong thư mục
            string[] files = Directory.GetFiles(folderPath);

            // Lặp qua từng tệp tin
            foreach (string file in files)
            {
                // Lấy thông tin về kích thước của tệp tin
                FileInfo fileInfo = new FileInfo(file);

                // Nếu kích thước của tệp tin là 0 byte
                if (fileInfo.Length == 0)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (extension == ".dwg" || extension == ".dxf")
                    {
                        // Xóa tệp tin
                        File.Delete(file);
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
            Listfile.ForEach(item => item.Check = true);
            dataGridView4.DataSource = new List<listcomp>();
            dataGridView4.DataSource = Listfile;
            int displayedRowCount = dataGridView4.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
            SH_sheet.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
            Listfile = new List<listfile>();
            dataGridView4.DataSource = new List<listfile>();
            SH_sheet.Visible = false;
        }
     
        private void button10_Click(object sender, EventArgs e)
        {
           
            Listfile.ForEach(item => item.Check = false);
            dataGridView4.DataSource = new List<listcomp>();
            dataGridView4.DataSource = Listfile;
            int displayedRowCount = dataGridView4.RowCount;
            for (int i = 0; i < displayedRowCount; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = (i + 1).ToString();
            }
            SH_sheet.Visible = false;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
           // customTableLayoutPanel2.Hide();
        }

        private void btn_ac_Click(object sender, EventArgs e)
        {
            HardwareInfo hardwareInfo = new HardwareInfo();
            string productID = hardwareInfo.GenerateProductID();
            bool check;
            string datetime1;
            checklicense(textBox1.Text, productID,out datetime1,out check);
            RegistryHelper.SaveSetting("VTECHKEY", "PRODUCTKEY", textBox1.Text);
            if (check)
            {
                RegistryHelper.SaveSetting("VTECHKEY", "PRODUCTKEY", textBox1.Text);
                // RegistryHelper.Clearproductid("VTECHKEY", "PRODUCTKEY");

                customTableLayoutPanel2.Show();
                tableLayoutPanel10.Show();
                label16.Text = "License expiration:" + " " + datetime1;
                lb_thongbao.Text = "";
            }
            else
            {
                RegistryHelper.SaveSetting("VTECHKEY", "PRODUCTKEY", "");
                customTableLayoutPanel2.Hide();
                tableLayoutPanel10.Hide();
                label16.Text = "License expiration:" + " " + datetime1;
                lb_thongbao.Text = "Giấy phép đã hết hạn!";
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = "https://zalo.me/0987.700.421";

            // Mở trình duyệt web với liên kết đó
            Process.Start(link);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở trình gửi thư điện tử với địa chỉ email đã được định sẵn
            string emailAddress = "autodraftings@gmail.com";
            string subject = "autodrafting!";
            string body = "Mua bản quyền phần mềm autodrafting";

            // Tạo URI để mở trình gửi thư điện tử
            string mailToUri = $"mailto:{emailAddress}?subject={subject}&body={body}";

            // Mở trình gửi thư điện tử
            Process.Start(mailToUri);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = "https://www.facebook.com/AutodraftingNX/";

            // Mở trình duyệt web với liên kết đó
            Process.Start(link);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //------------------------------------------------------------------------------------------------------------
        //public static void sCreateCustomModellingView(Part objPart, double X, double Y, double Z, double Xx, double Xy, double Xz,
        //                                                     double Yx, double Yy, double Yz, double Zx, double Zy, double Zz,
        //                                                     string sCustViewName, double dScale, string sDefaultRefView = "FRONT")
        //        {
        //            int viewType1 = 0;
        //            //1 = modeling view
        //            //2 = drawing view
        //            //other = error
        //            theUFSession.Draw.AskDisplayState(out  viewType1);

        //            //if drawing sheet shown, change to modeling view
        //            if (viewType1 == 2)
        //            {
        //                theUFSession.Draw.SetDisplayState(1);
        //            }

        //        NXOpen.View objView = null;
        //            Matrix3x3 rotMatrix = new Matrix3x3();

        //            rotMatrix.Xx = Xx;
        //            rotMatrix.Xy = Xy;
        //            rotMatrix.Xz = Xz;
        //            rotMatrix.Yx = Yx;
        //            rotMatrix.Yy = Yy;
        //            rotMatrix.Yz = Yz;
        //            rotMatrix.Zx = Zx;
        //            rotMatrix.Zy = Zy;
        //            rotMatrix.Zz = Zz;
        //            Point3d translation = new Point3d(X, Y, Z);

        //            if (objPart.ModelingViews.WorkView != null)
        //            {
        //                objPart.ModelingViews.WorkView.SetRotationTranslationScale(rotMatrix, translation, dScale);
        //                objView = objPart.Views.SaveAs(objPart.ModelingViews.WorkView, sCustViewName, false, false);

        //            }
        //            else
        //            {
        //                foreach (ModelingView objMdVw in objPart.ModelingViews)
        //                {
        //                    if (objMdVw.Name.ToUpper() == sDefaultRefView)
        //                    {
        //                        objMdVw.MakeWork();
        //                        objMdVw.SetRotationTranslationScale(rotMatrix, translation, dScale);
        //                        objPart.Views.SaveAs(objMdVw, sCustViewName, false, false);
        //                        break;
        //                    }
        //                }
        //            }

        //            //reset initial view state
        //            theUFSession.Draw.SetDisplayState(viewType1);
        //        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        //public static int GetUnloadOption1(string dummy)
        //{
        //    Unloads the image when the NX session terminates
        //            return NXOpen.Session.LibraryUnloadOption.AtTermination;
        //}


    }


}
