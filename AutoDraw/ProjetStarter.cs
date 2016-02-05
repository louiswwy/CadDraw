using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(AutoDraw.InitProject))]
[assembly: CommandClass(typeof(AutoDraw.ProjetStarter))]
namespace AutoDraw
{
    public class ProjetStarter//:IExtensionApplication
    {
        //Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        MainInterface form;
        [CommandMethod("AutoDrawer")]
        public void starter()
        {
            if (form == null || form.IsDisposed)
            {
                //ed.WriteMessage("AutoDrawer 功能已载入.");
                form = new MainInterface();
                try
                {
                    Application.ShowModelessDialog(form);  //显示非模态对话框 
                }
                catch (System.Exception ee)
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("发生错误！请联系维护人员！\n" + ee.ToString());
                }
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

            //Application.DocumentManager.DocumentLockModeChanged += new DocumentLockModeChangedEventHandler(callback_DocumentManager_DocumentLockModeChanged);
        }

        /*private void callback_DocumentManager_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }*/

        public void Terminate()

        {

        }
    }
}
