using System;
using NXOpen;
using NXOpen.UF;
using MiniSnap;
namespace _3D2CAD
{
    public class LookAt
    {
        static Session theSession = Session.GetSession();
        static UFSession theUFSession = UFSession.GetUFSession();
        static Part workPart = theSession.Parts.Work;

        public static Matrix3x3  orientcustomtool() // Program entry point
        {
            Matrix3x3 result = new Matrix3x3 { };
            Face face1 = MySelect.SelectAPlanarFace("Select a planar face");
            if (face1 != null)
            {
                UFModl ufmodl = theUFSession.Modl;

                //Get surface normal pointing out of the solid. : dir
                int faceType;
                double[] facePt = new double[3];
                double[] faceDir = new double[3];
                double[] bbox = new double[6];
                double faceRadius;
                double faceRadData;
                int normDirection;
                ufmodl.AskFaceData(face1.Tag, out faceType, facePt, faceDir, bbox, out faceRadius, out faceRadData, out normDirection);
                Vector dir = faceDir;

                //Get the range of the surface parameters.
                double[] uvminmax = new double[4];
                ufmodl.AskFaceUvMinmax(face1.Tag, uvminmax);
                double umin = uvminmax[0];
                double vmin = uvminmax[2];

                //Get u,v vector of the surface.
                NXOpen.UF.ModlSrfValue surfaceValue = new NXOpen.UF.ModlSrfValue();
                int mode = NXOpen.UF.UFConstants.UF_MODL_EVAL_UNIT_NORMAL;
                ufmodl.EvaluateFace(face1.Tag, mode, new double[] { umin, vmin }, out surfaceValue);
                Vector u = surfaceValue.srf_du;

                //review v-direction so that u,v,dir make up right-handed c-sys.
                Vector v = Vector.Cross(dir, u);

                // "ufmodl.AskFaceProps" does not return correct u,v direction. IR has been issued already.
                //"surfaceValue.srf_unormal" does not always point out of the solid.

                //Get orientation of the work view.
                Orientation oriview = new Orientation(workPart.ModelingViews.WorkView.Matrix);

                //among 4 c-sys (u,v),(v,-u),(-u,-v),(-v,u), select the nearest one to the view. : oriface
                double a0 = u * oriview.AxisX + v * oriview.AxisY;
                double a1 = v * oriview.AxisX - u * oriview.AxisY;
                double a2 = -u * oriview.AxisX - v * oriview.AxisY;
                double a3 = -v * oriview.AxisX + u * oriview.AxisY;
                Orientation oriface;
                if ((a0 > a1) && (a0 > a2) && (a0 > a3))
                    oriface = new Orientation(u, v);
                else if ((a1 > a2) && (a1 > a3))
                    oriface = new Orientation(v, -u);
                else if (a2 > a3)
                    oriface = new Orientation(-u, -v);
                else
                    oriface = new Orientation(-v, u);

                workPart.ModelingViews.WorkView.Orient(oriface);
                result = oriface;
               
            }
            return result;
        }

        public static int GetUnloadOption(string dummy)
        {
            return (int)Session.LibraryUnloadOption.Immediately;
        }
    }

    public class MySelect
    {
        public static Face SelectAPlanarFace(string prompt)
        {
            Selection.MaskTriple[] masks = new Selection.MaskTriple[1];
            masks[0].Type = UFConstants.UF_solid_type;
            masks[0].SolidBodySubtype = UFConstants.UF_UI_SEL_FEATURE_PLANAR_FACE;
            TaggedObject selObj = null;
            Point3d cursor;
            Selection.Response resp = UI.GetUI().SelectionManager.SelectTaggedObject(
            prompt, //message
            "Select a planar face", //title
            Selection.SelectionScope.AnyInAssembly, //scope
            Selection.SelectionAction.ClearAndEnableSpecific, //action
            false, //includeFeatures
            false, //keepHighlighted
            masks, //maskArray
            out selObj,
            out cursor);
            return (Face)selObj;
        }
    }
}