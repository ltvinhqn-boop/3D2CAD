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
    class CreateView
    {
        private static UFSession ufSession = UFSession.GetUFSession();
        public static void Create3View(Session theSession, Part workPart, string thislist,string Path,NXOpen.Part listpart,double Height, double length,string scale,System.Int32 dop,string parentview,List<bool> EnableView, bool hidden,bool check3dhiddenline)
        {
            UFSession ufSession = UFSession.GetUFSession();
            //theSession.ApplicationSwitchImmediate("UG_APP_MODELING");
            //NXOpen.Session theSession = NXOpen.Session.GetSession();
            // NXOpen.Part workPart = theSession.Parts.Work;
            NXOpen.Part displayPart = theSession.Parts.Display;
            //-----------------------------------------------------------------------------------------
            int rows = 3;
            int columns = 4;

            // Chuyển đổi danh sách bool thành mảng 2 chiều
            bool[,] boolArray = new bool[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int index = i * columns + j;
                    boolArray[i, j] = EnableView[index];
                }
            }

            // Tìm số phần tử true lớn nhất trong hàng và cột
            int maxRowTrueCount = Enumerable.Range(0, rows)
                .Select(row => Enumerable.Range(0, columns).Count(col => boolArray[row, col]))
                .Max();

            int maxColumnTrueCount = Enumerable.Range(0, columns)
                .Select(col => Enumerable.Range(0, rows).Count(row => boolArray[row, col]))
                .Max();
            //----------------------------------------------------------------------------------------------------
           

            // Lấy đường dẫn đầy đủ của Part
            string partFileName = workPart.FullPath;

            // Lấy tên tệp tin của Part
            string partFileNameWithoutPath = System.IO.Path.GetFileName(partFileName);

            // Lấy đường dẫn của Component bằng cách loại bỏ tên tệp tin khỏi đường dẫn của Part
            string componentPath = partFileName.Replace(partFileNameWithoutPath, "");


            // ---------------------------------------------------xóa sheet hiện hữu
            
           
            //-------------------------------------------------
            
                //---------------------LOADSTANDART
                //try
                //{
                //    NXOpen.Preferences.LoadDraftingStandardBuilder loadDraftingStandardBuilder1;

                //    // load standard
                //    loadDraftingStandardBuilder1 = workPart.Preferences.DraftingPreference.CreateLoadDraftingStandardBuilder();
                //    loadDraftingStandardBuilder1.Level = NXOpen.Preferences.LoadDraftingStandardBuilder.LoadLevel.User;
                
                //    loadDraftingStandardBuilder1.Name = Standard;
                
                //    NXOpen.NXObject nXObject1;
                //    nXObject1 = loadDraftingStandardBuilder1.Commit();


                //    loadDraftingStandardBuilder1.Destroy();
                //}
                //catch { }
                //---------------------------------------------------
                string A = thislist + ".prt";

                //---------------------------------------------------------------tạo sheet
                NXOpen.Drawings.DraftingDrawingSheet nullNXOpen_Drawings_DraftingDrawingSheet = null;
                NXOpen.Drawings.DrawingSheetBuilder drawingSheetBuilder1 = workPart.DrawingSheets.DrawingSheetBuilder(nullNXOpen_Drawings_DraftingDrawingSheet);
                drawingSheetBuilder1.Option = NXOpen.Drawings.DrawingSheetBuilder.SheetOption.CustomSize;
                // drawingSheetBuilder1.Option = NXOpen.Drawings.DrawingSheetBuilder.SheetOption.UseTemplate;
                 drawingSheetBuilder1.ProjectionAngle = (NXOpen.Drawings.DrawingSheetBuilder.SheetProjectionAngle)dop; //NXOpen.Drawings.DrawingSheetBuilder.SheetProjectionAngle.First;
                // drawingSheetBuilder1.MetricSheetTemplateLocation = path;
                
                drawingSheetBuilder1.Height = Height;
                drawingSheetBuilder1.Length = length;
                 double Height1 = drawingSheetBuilder1.Height;
                 double Length1 = drawingSheetBuilder1.Length;
                 double bouw = (drawingSheetBuilder1.Length - 2*40.0-(maxColumnTrueCount-1)*5.0) / maxColumnTrueCount;
                double bouh = (drawingSheetBuilder1.Height - 2 * 40.0 - (maxRowTrueCount - 1) * 5.0) / maxRowTrueCount;
                drawingSheetBuilder1.StandardMetricScale = NXOpen.Drawings.DrawingSheetBuilder.SheetStandardMetricScale.Custom;
                drawingSheetBuilder1.ScaleNumerator = double.Parse(scale.Split(':')[0]);
                drawingSheetBuilder1.ScaleDenominator= double.Parse(scale.Split(':')[1]);

                drawingSheetBuilder1.Name = thislist;
                NXOpen.NXObject nxObject1 = drawingSheetBuilder1.Commit();
                drawingSheetBuilder1.Destroy();
                //------------------------insert khung tên
                if (File.Exists(Path))
                 {
                    double b =  double.Parse(scale.Split(':')[0])/ double.Parse(scale.Split(':')[1]);
                    ImportPart(theSession, workPart, Path, b);
                }
            //-------------------------------------------------------------------------------------
            // Kiểm tra xem phần làm việc hiện tại có phải là môi trường drafting hay không
            //// tắt bật hidden line
            NXOpen.Drafting.PreferencesBuilder preferencesBuilder1;
            preferencesBuilder1 = workPart.SettingsManager.CreatePreferencesBuilder();
            preferencesBuilder1.ViewWorkflow.DisplayBorders = false;
            preferencesBuilder1.ViewStyle.ViewStyleHiddenLines.Font = NXOpen.Preferences.Font.Dashed;
            preferencesBuilder1.ViewStyle.ViewStyleHiddenLines.EdgesHiddenByEdges = true;

            preferencesBuilder1.ViewStyle.ViewStyleHiddenLines.ReferenceEdgesOnly = true;
            preferencesBuilder1.ViewStyle.ViewStyleHiddenLines.EdgesHiddenByEdges = hidden;

            preferencesBuilder1.ViewStyle.ViewStyleHiddenLines.ReferenceEdgesOnly = hidden;

            NXOpen.NXObject nXObject111;
            nXObject111 = preferencesBuilder1.Commit();
            preferencesBuilder1.Destroy();

            //----------------------------------------------------------------
            NXOpen.Drawings.ViewCreationWizardBuilder viewCreationWizardBuilder1;
                viewCreationWizardBuilder1 = workPart.DraftingViews.CreateViewCreationWizardBuilder();
                viewCreationWizardBuilder1.AutoScale = true;
            //iewScaleBuilder.Type.
            // viewCreationWizardBuilder1.ViewBoundary = NXOpen.Drawings.ViewCreationWizardBuilder.ViewBoundaryOption.Manual;
        



            //  viewCreationWizardBuilder1.OrientViewTool
            viewCreationWizardBuilder1.MultipleViewPlacement.ViewPlacementCenter.Associative = true;

                viewCreationWizardBuilder1.MultipleViewPlacement.ViewPlacementFirstCorner.Associative = true;

                viewCreationWizardBuilder1.MultipleViewPlacement.ViewPlacementSecondCorner.Associative = true;

                viewCreationWizardBuilder1.AssociativeAlignment = true;

                viewCreationWizardBuilder1.BaseView = parentview;

                viewCreationWizardBuilder1.SpecialBaseView = false;
                viewCreationWizardBuilder1.ViewBoundary = NXOpen.Drawings.ViewCreationWizardBuilder.ViewBoundaryOption.Automatic;
                // buttonList = new List<System.Windows.Forms.Button>() {btn_bottom,btn_right,btn_parent,btn_left,btn_vback,btn_iso,btn_top,btn_tri };
                viewCreationWizardBuilder1.BottomView = EnableView[1];

                viewCreationWizardBuilder1.RightView = EnableView[4];
                viewCreationWizardBuilder1.LeftView = EnableView[6];
                viewCreationWizardBuilder1.BackView = EnableView[7];
                viewCreationWizardBuilder1.IsometricView = EnableView[8];
                viewCreationWizardBuilder1.TopView = EnableView[9];
                viewCreationWizardBuilder1.TrimetricView = EnableView[10];
            // viewCreationWizardBuilder1.RightView = EnableView[1];
            // NXOpen.Drawings.ViewCreationWizardBuilder.OrientViewTool.
       
            //  viewCreationWizardBuilder1.o

            NXOpen.Part part1 = listpart;
               //ool loadStatus2;
                //loadStatus2 = part1.IsFullyLoaded;
                part1.LoadThisPartFully();//load part( rất quan trọng)

                viewCreationWizardBuilder1.Part = part1;
              

                viewCreationWizardBuilder1.ViewStyle.ViewStyleBase.Part = part1;
               // NXOpen.Drawings.ViewBoundaryBuilder

                viewCreationWizardBuilder1.ViewStyle.ViewStyleBase.PartName = System.IO.Path.Combine(componentPath, A);

                
                viewCreationWizardBuilder1.PlacementOption = NXOpen.Drawings.ViewCreationWizardBuilder.Option.Automatic;

                viewCreationWizardBuilder1.MultipleViewPlacement.OptionType = NXOpen.Drawings.MultipleViewPlacementBuilder.Option.Center;

                // ----------------------------------------------
                //   Dialog Begin View Creation Wizard
                // ----------------------------------------------
                double scale1 = viewCreationWizardBuilder1.ViewScale.Numerator;
                double scale2 = viewCreationWizardBuilder1.ViewScale.Denominator;

                NXOpen.NXObject nXxObject1;
                 nXxObject1 = viewCreationWizardBuilder1.Commit();
                 NXOpen.NXObject[] objects1;
                objects1 = viewCreationWizardBuilder1.GetCommittedObjects();


                viewCreationWizardBuilder1.Destroy();
                
                List<double> ratio = Sizeboders.GetRatioScale(theSession, workPart, objects1, bouw, bouh);
            // InsertTb.InsertTB(tb2, tb1.Lines.Length, ratio[0] * scale1 + ":" + ratio[1] * scale2 + "-" + scale1 + ":" + scale2);
            //  InsertTb.InsertTB(tb2, tb1.Lines.Length, );

            //-----------------------------------------------------------------------------------------------test
            // load standard
            //try
            //{
            //    NXOpen.Preferences.LoadDraftingStandardBuilder loadDraftingStandardBuilder2;
            //    loadDraftingStandardBuilder2 = workPart.Preferences.DraftingPreference.CreateLoadDraftingStandardBuilder();
            //    loadDraftingStandardBuilder2.Level = NXOpen.Preferences.LoadDraftingStandardBuilder.LoadLevel.User;
            //    // loadDraftingStandardBuilder1.
            //    loadDraftingStandardBuilder2.Name = Standard2;
            //    NXOpen.NXObject nXObject12;
            //    nXObject12 = loadDraftingStandardBuilder2.Commit();
            //    //-------------

            //    // boundingBox = workPart.BoundingBox;

            //    loadDraftingStandardBuilder2.Destroy();
            //}
            //catch { }
            //// tắt bật hidden line
            NXOpen.Drafting.PreferencesBuilder preferencesBuilder2;
            preferencesBuilder2 = workPart.SettingsManager.CreatePreferencesBuilder();
            preferencesBuilder2.ViewWorkflow.DisplayBorders = false;        
            preferencesBuilder2.ViewStyle.ViewStyleHiddenLines.EdgesHiddenByEdges = hidden;

            preferencesBuilder2.ViewStyle.ViewStyleHiddenLines.ReferenceEdgesOnly = hidden;

            NXOpen.NXObject nXObject1112;
            nXObject1112 = preferencesBuilder2.Commit();
            preferencesBuilder2.Destroy();
            //-----------------------------------------------------------
            NXOpen.Drawings.ViewCreationWizardBuilder viewCreationWizardBuilder13;
                viewCreationWizardBuilder13 = workPart.DraftingViews.CreateViewCreationWizardBuilder();
                viewCreationWizardBuilder13.AutoScale = true;
                //iewScaleBuilder.Type.
                // viewCreationWizardBuilder1.ViewBoundary = NXOpen.Drawings.ViewCreationWizardBuilder.ViewBoundaryOption.Manual;

                viewCreationWizardBuilder13.MultipleViewPlacement.ViewPlacementCenter.Associative = true;

                viewCreationWizardBuilder13.MultipleViewPlacement.ViewPlacementFirstCorner.Associative = true;

                viewCreationWizardBuilder13.MultipleViewPlacement.ViewPlacementSecondCorner.Associative = true;

                viewCreationWizardBuilder13.AssociativeAlignment = true;

                viewCreationWizardBuilder13.BaseView = parentview;

                viewCreationWizardBuilder13.SpecialBaseView =false;
                viewCreationWizardBuilder13.ViewBoundary = NXOpen.Drawings.ViewCreationWizardBuilder.ViewBoundaryOption.Manual;

                viewCreationWizardBuilder13.BottomView = EnableView[1];

                viewCreationWizardBuilder13.RightView = EnableView[4];
                viewCreationWizardBuilder13.LeftView = EnableView[6];
                viewCreationWizardBuilder13.BackView = EnableView[7];
                viewCreationWizardBuilder13.IsometricView = EnableView[8];
                viewCreationWizardBuilder13.TopView = EnableView[9];
                viewCreationWizardBuilder13.TrimetricView = EnableView[10];
          
            viewCreationWizardBuilder13.MarginToBorder = 40.0;

                viewCreationWizardBuilder13.MarginBetweenViews = 5.0;
                viewCreationWizardBuilder13.ViewScale.Numerator = ratio[0]*scale1;
                viewCreationWizardBuilder13.ViewScale.Denominator = ratio[1]*scale2;



                NXOpen.Part part13 = listpart;

                viewCreationWizardBuilder13.Part = part13;
             

                viewCreationWizardBuilder13.ViewStyle.ViewStyleBase.Part = part13;
                // NXOpen.Drawings.ViewBoundaryBuilder

                viewCreationWizardBuilder13.ViewStyle.ViewStyleBase.PartName = System.IO.Path.Combine(componentPath, A);

                viewCreationWizardBuilder13.PlacementOption = NXOpen.Drawings.ViewCreationWizardBuilder.Option.Automatic;
           // NXOpen.Point3d point1 = new NXOpen.Point3d(Length1/2, Height1/2+20, 0.0);
          //  viewCreationWizardBuilder13.MultipleViewPlacement.ViewPlacementCenter.Placement.SetValue(null, workPart.Views.WorkView, point1);

             viewCreationWizardBuilder13.MultipleViewPlacement.OptionType = NXOpen.Drawings.MultipleViewPlacementBuilder.Option.Center;

            // ----------------------------------------------
            //   Dialog Begin View Creation Wizard
            // ----------------------------------------------


            NXOpen.NXObject nXxObject13;
                nXxObject13 = viewCreationWizardBuilder13.Commit();

                NXOpen.NXObject[] objects13;
                objects13 = viewCreationWizardBuilder13.GetCommittedObjects();


                viewCreationWizardBuilder13.Destroy();
            //-------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------


            //------------------------------------------------------------------------------------------------test
            if (!hidden)
            {
                List<DraftingView> draftingViews = new List<DraftingView>();
                foreach (NXOpen.NXObject obj in objects13)
                {
                    if (obj is DraftingView)
                    {
                        DrawingSheet drawingSheet1 = (DrawingSheet)nxObject1;
                        string name = ((DraftingView)obj).Name;
                        if (name.Contains("Trimetric") || name.Contains("Isometric"))
                        {
                            NXOpen.View[] views1 = new NXOpen.View[1];
                            views1[0] = (DraftingView)obj;
                            NXOpen.Drawings.EditViewSettingsBuilder editViewSettingsBuilder1;
                            editViewSettingsBuilder1 = workPart.SettingsManager.CreateDrawingEditViewSettingsBuilder(views1);
                            editViewSettingsBuilder1.ViewStyle.ViewStyleHiddenLines.EdgesHiddenByEdges = !check3dhiddenline;

                            editViewSettingsBuilder1.ViewStyle.ViewStyleHiddenLines.ReferenceEdgesOnly = !check3dhiddenline;
                           
                            NXOpen.NXObject nXObject134;
                            nXObject134 = editViewSettingsBuilder1.Commit();
                            editViewSettingsBuilder1.Destroy();
                        }
                    }
                }
            }



            //-----------------------------------------------------------------------------------------------------------------------------------------------
            //theSession.ApplicationSwitchImmediate("UG_APP_MODELING");

            //workPart.Drafting.ExitDraftingApplication();


        }
        //---------------------------------
        //   public static int GetUnloadOption(string dummy) { return (int)NXOpen.Session.LibraryUnloadOption.Immediately; }
        private static void ImportPart(Session theSession, Part workPart,string Path, double scale)
        {
           

            NXOpen.PartImporter partImporter1;
            partImporter1 = workPart.ImportManager.CreatePartImporter();

            partImporter1.FileName = Path;

            partImporter1.Scale = 1.0;

            partImporter1.CreateNamedGroup = false;

            partImporter1.ImportViews = false;

            partImporter1.ImportCamObjects = false;

            partImporter1.LayerOption = NXOpen.PartImporter.LayerOptionType.Work;

            partImporter1.DestinationCoordinateSystemSpecification = NXOpen.PartImporter.DestinationCoordinateSystemSpecificationType.Work;

            NXOpen.Matrix3x3 element1 = new NXOpen.Matrix3x3();
            element1.Xx = 1.0;
            element1.Xy = 0.0;
            element1.Xz = 0.0;
            element1.Yx = 0.0;
            element1.Yy = 1.0;
            element1.Yz = 0.0;
            element1.Zx = 0.0;
            element1.Zy = 0.0;
            element1.Zz = 1.0;
            NXOpen.NXMatrix nXMatrix1;
            nXMatrix1 = workPart.NXMatrices.Create(element1);

            partImporter1.DestinationCoordinateSystem = nXMatrix1;

            NXOpen.Point3d destinationPoint1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            partImporter1.DestinationPoint = destinationPoint1;          

            NXOpen.NXObject nXObject1;
            nXObject1 = partImporter1.Commit();

          

            partImporter1.Destroy();
        }
        //public static void CreateDiametricDimension(Session theSession, Part workPart, DrawingSheet drawingSheet, DraftingView draftingView)
        //{
        //    DisplayableObject[] displayableObjects = draftingView.AskVisibleObjects();
        //    foreach (var obj in displayableObjects)
        //    {
        //        if (obj.GetType() == typeof(Centerline3d))
        //        {
        //            NXOpen.TaggedObject[] objects1 = new NXOpen.TaggedObject[1];
        //            NXOpen.Annotations.Centerline3d centerline3d1 = (NXOpen.Annotations.Centerline3d)obj;
        //            objects1[0] = centerline3d1;
        //            int nErrs1;
        //            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);

        //        }
        //        else
        //        {
        //            int type;
        //            int subtype;

        //            ufSession.Obj.AskTypeAndSubtype(obj.Tag, out type, out subtype);
        //            if (type == UFConstants.)
        //            {
        //                NXOpen.DisplayableObject[] objects1 = new NXOpen.DisplayableObject[1];
        //                objects1[0] = obj;
        //                theSession.DisplayManager.BlankObjects(objects1);

        //                workPart.Views.WorkView.FitAfterShowOrHide(NXOpen.View.ShowOrHideType.HideOnly);
        //            }
        //        }
        //        // }
        //    }
        //}
        //------------------------
        //public static void CreateDiametricDimension(Session theSession, Part workPart, DrawingSheet drawingSheet, DraftingView draftingView)
        //{
        //    DisplayableObject[] displayableObjects = draftingView.AskVisibleObjects();
        //   // draftingView.Scale
        //    foreach (var obj in displayableObjects)
        //    {
        //        if (obj.GetType() == typeof(Centerline3d))
        //        {
        //            NXOpen.TaggedObject[] objects1 = new NXOpen.TaggedObject[1];
        //            NXOpen.Annotations.Centerline3d centerline3d1 = (NXOpen.Annotations.Centerline3d)obj;
        //            objects1[0] = centerline3d1;
        //            int nErrs1;
        //            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);
        //        }
        //        else
        //        {
        //            int type;
        //            int subtype;
        //            NXOpen.Tag dimension;
        //            ufSession.Obj.AskTypeAndSubtype(obj.Tag, out type, out subtype);
        //            if (type == UFConstants.UF_VIEW_HIDDEN_EDGES_DASHED)
        //            {
        //                UFDrf.Object ufDrfObj = new UFDrf.Object();
        //                UFDrf.Text text = new UFDrf.Text();
        //                ufDrfObj.object_tag = obj.Tag;
        //                ufDrfObj.object_view_tag = draftingView.Tag;
        //                ufDrfObj.object_assoc_type = UFDrf.AssocType.Intersection;
        //                double[] dimOrigin = new double[3];
        //                dimOrigin[0] = 50.0;
        //                dimOrigin[1] = 100.0;
        //                dimOrigin[2] = 0.0;
        //                ufSession.Drf.CreateDiameterDim(ref ufDrfObj, ref text, dimOrigin, out dimension);
        //                workPart.Views.WorkView.FitAfterShowOrHide(NXOpen.View.ShowOrHideType.HideOnly);
        //            }
        //        }
        //        // }
        //    }
        //}
        //------------------------------------------
        public class DoublePair
        {
            public double Value1 { get; set; }
            public double Value2 { get; set; }
            public DoublePair(double vl1, double vl2)
            {
                Value1 = vl1;
                Value2 = vl2;
            }
        }
    }
}
