#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace TagView
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //Tag Parameters
            TagMode tmode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrient = TagOrientation.Horizontal;

            List<BuiltInCategory> cats = new List<BuiltInCategory>();
            cats.Add(BuiltInCategory.OST_Windows);
            cats.Add(BuiltInCategory.OST_Doors);

            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(cats);

            IList<Element> tElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();

            try
            {
                using (Transaction trans = new Transaction(doc, "Tag Elements"))
                {
                    trans.Start();

                    //Tag Elements
                    foreach (Element ele in tElements)
                    {
                        Reference refe = new Reference(ele);
                        LocationPoint loc = ele.Location as LocationPoint;
                        XYZ point = loc.Point;
                        IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, refe, false, tmode, tOrient, point);
                    }

                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}

