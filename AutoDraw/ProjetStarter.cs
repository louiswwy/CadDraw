using System;
using System.Collections.Generic;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

[assembly: ExtensionApplication(typeof(AutoDraw.InitProject))]
[assembly: CommandClass(typeof(AutoDraw.ProjetStarter))]
namespace AutoDraw
{
    public class ProjetStarter//:IExtensionApplication
    {
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        MainInterface form;
        [CommandMethod("AutoDrawer")]
        public void starter()
        {
            if (form == null || form.IsDisposed)
            {
                ed.WriteMessage("AutoDrawer 功能已载入.");
                form = new MainInterface();
                Application.ShowModelessDialog(form);  //显示非模态对话框 
                //Application.ShowModalDialog(form);  //显示非模态对话框 

            }

            else
            {

                form.Activate();
            }
        }

        /*public void Initialize()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            

        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }*/
    }

    public class InitProject : Autodesk.AutoCAD.Runtime.IExtensionApplication
    {
        Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        public void Initialize()
        {
            string dTime = DateTime.Now.ToLongDateString();
            ed.WriteMessage(dTime+": 功能已载入.");
        }

        public void Terminate()
        {

        }
    }
}
