using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using comp = NXOpen.Assemblies.Component;
using NXOpen;
using NXOpen.UF;
using NXOpen.Assemblies;

namespace _3D2CAD
{
    class Getlistnamecomponent
    {

        public static List<string> Getlist(Part workPart, List<comp> allComponents)
        {
            List<string> listnamecomponents = new List<string>();
           
                // Lấy một thành phần trong tệp hiện tại, ví dụ: thành phần đầu tiên
                comp rootComponent = workPart.ComponentAssembly.RootComponent;
            if (rootComponent != null)
            { 
                GetAllComponents(rootComponent, allComponents);
            
                foreach (comp cpp in allComponents)
                {
                    NXOpen.Part part1 = cpp.Prototype as NXOpen.Part;
                    if (part1 == null)
                    { continue; }
                    string componentName = part1.Name;

                    listnamecomponents.Add(componentName);
                    // InsertTB(tb1, tb1.Lines.Length, componentName);
                }
            }
            else
            {
                listnamecomponents.Add(workPart.Name);
            }
            
            return listnamecomponents;

        }
        public static List<int> Getlayers(Part workPart, List<comp> allComponents)
        {
            
            List<int> listlayers1 = new List<int>();
          
                // Lấy một thành phần trong tệp hiện tại, ví dụ: thành phần đầu tiên
                comp rootComponent = workPart.ComponentAssembly.RootComponent;
            if (rootComponent != null)
            {
                // Lấy tất cả các thành phần bao gồm cả các thành phần con

                GetAllComponents(rootComponent, allComponents);
           
                foreach (comp cpp in allComponents)
            {
                int componentName = cpp.Layer;
                listlayers1.Add(componentName);
                // InsertTB(tb1, tb1.Lines.Length, componentName);
            }
            }
          
            List<int> listlayers =  listlayers1.Distinct().ToList();
            return listlayers;

        }
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
        public static int CountElements(List<string> listnamecomponents, string part)
        {
           
                int count = 0;
                foreach (string component in listnamecomponents)
                {
                    if (component == part)
                    {
                        count++;
                    }
                }
              
            
            return count;
        
       }

}
}
