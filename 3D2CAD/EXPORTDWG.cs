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
    class EXPORTDWG
    {
        static Session theSession = Session.GetSession();
        private static UFSession theUFSession = UFSession.GetUFSession();
        Part workPart = theSession.Parts.Work;
        // Export Specified Drawing Sheet to ACAD/DWG Format
        public static int ExportDrawingSheetToDxf(DrawingSheet aSheet, string aOutputFileName, Part aInputPart, bool ex,string scale)
        {
           

            string dxfSettingsDir = theSession.GetEnvironmentVariableValue("DXFDWG_DIR");
            string DxfdwgDir = theSession.GetEnvironmentVariableValue("DXFDWG_DIR");

            if (String.IsNullOrEmpty(dxfSettingsDir))
            {
               // NXMessageBox.Show("DXF settings directory not found; journal exiting.",
                   // "DXF settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            if (String.IsNullOrEmpty(DxfdwgDir))
            {
               // MessageBox.Show("Dxfdwg settings directory not found; journal exiting.",
                  //  "Dxfdwg settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            DxfdwgCreator dxfdwgCreator1;
            dxfdwgCreator1 = theSession.DexManager.CreateDxfdwgCreator();

            dxfdwgCreator1.SettingsFile = Path.Combine(dxfSettingsDir, "dxfdwg.def");
            dxfdwgCreator1.InputFile = aInputPart.FullPath;
            dxfdwgCreator1.OutputFile = aOutputFileName;
            

            // 
            dxfdwgCreator1.AutoCADRevision = DxfdwgCreator.AutoCADRevisionOptions.R2000;
            dxfdwgCreator1.ExportData = DxfdwgCreator.ExportDataOption.Drawing;
            dxfdwgCreator1.OutputFileType= (ex)?DxfdwgCreator.OutputFileTypeOption.Dwg: DxfdwgCreator.OutputFileTypeOption.Dxf;
            dxfdwgCreator1.ExportScaleOption = NXOpen.DxfdwgCreator.ExportScaleOptions.BaseView;

            dxfdwgCreator1.ExportScaleValue = scale;
            dxfdwgCreator1.ObjectTypes.Curves = true;
            dxfdwgCreator1.ObjectTypes.Annotations = true;
            dxfdwgCreator1.ObjectTypes.Structures = true;
            dxfdwgCreator1.FlattenAssembly = false;
            dxfdwgCreator1.ViewEditMode = false;
            dxfdwgCreator1.FileSaveFlag = false;
            dxfdwgCreator1.LayerMask = "1-256";
            dxfdwgCreator1.DrawingList = aSheet.Name;
            dxfdwgCreator1.ViewList = "TOP,FRONT,RIGHT,BACK,BOTTOM,LEFT,TFR-ISO,TFR-TRI";

            NXObject nXObject1;
            nXObject1 = dxfdwgCreator1.Commit();
            dxfdwgCreator1.Destroy();

            // presume no other errors
            return 0;
        }
        public static bool IsFileWritable(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while checking the file writability: " + ex.Message);
                return false;
            }
        }
    }
}
