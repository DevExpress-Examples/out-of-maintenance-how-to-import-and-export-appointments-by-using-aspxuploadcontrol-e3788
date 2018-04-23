using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;

public partial class _Default : System.Web.UI.Page {
    private List<int> lastInsertedIds = new List<int>();

    protected void Page_Load(object sender, EventArgs e) {
        if(!IsPostBack) {
            ASPxScheduler1.Start = new DateTime(2008, 7, 12);
        }
    }

    protected void ASPxScheduler1_AppointmentCollectionCleared(object sender, EventArgs e) {
        lastInsertedIds.Clear();
    }

    protected void ASPxScheduler1_AppointmentRowInserting(object sender, DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertingEventArgs e) {
        e.NewValues.Remove("ID");
    }

    protected void SqlDataSourceAppointments_Inserted(object sender, System.Web.UI.WebControls.SqlDataSourceStatusEventArgs e) {
        SqlConnection connection = (SqlConnection)e.Command.Connection;

        using(SqlCommand command = new SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", connection)) {
            lastInsertedIds.Add(Convert.ToInt32(command.ExecuteScalar()));
        }
    }

    protected void ASPxScheduler1_AppointmentRowInserted(object sender, DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertedEventArgs e) {
        int count = lastInsertedIds.Count;
        System.Diagnostics.Debug.Assert(count > 0);

        e.KeyFieldValue = lastInsertedIds[count - 1];
    }

    protected void ASPxScheduler1_AppointmentsInserted(object sender, DevExpress.XtraScheduler.PersistentObjectsEventArgs e) {
        int count = e.Objects.Count;
        System.Diagnostics.Debug.Assert(count == lastInsertedIds.Count);
        
        ASPxSchedulerStorage storage = (ASPxSchedulerStorage)sender;
        for (int i = 0; i < count; i++) {
            Appointment apt = (Appointment)e.Objects[i];
            int appointmentId = lastInsertedIds[i];
            storage.SetAppointmentId(apt, appointmentId);
        }

        lastInsertedIds.Clear();
    }

    protected void ASPxScheduler1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
        if (e.Menu.MenuId.Equals(SchedulerMenuItemId.DefaultMenu)) {
            e.Menu.Items[0].BeginGroup = true;

            MenuItem menuItemImport = new MenuItem("Import Appointments", "ImportAppointments");
            e.Menu.Items.Insert(0, menuItemImport);

            MenuItem menuItemExport = new MenuItem("Export Appointments", "ExportAppointments");
            e.Menu.Items.Insert(1, menuItemExport);

            e.Menu.ClientSideEvents.ItemClick = "function(s, e) { DefaultViewMenuHandler(s, e); }";
        }
    }

    protected void ASPxScheduler1_BeforeExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
        if (e.CommandId == "EXPORTAPT")
            e.Command = new ExportAppointmentCallbackCommand((ASPxScheduler)sender);
        else if (e.CommandId == "IMPRTAPT")
            e.Command = new ImportAppointmentsCallbackCommand((ASPxScheduler)sender);
    }

    protected void ASPxUploadControl1_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e) {
        ASPxUploadControl uploadControl = (ASPxUploadControl)sender;
        UploadedFile uploadedFile = uploadControl.UploadedFiles[0];
        
        if (!IsFileNameCorrect(uploadedFile.FileName)) {
            e.IsValid = false;
            e.ErrorText = "Incorrect file type";
            return;
        }

        if (uploadedFile.IsValid)
            Session["UploadedFile"] = GetBytes(uploadedFile.FileContent);
    }

    private bool IsFileNameCorrect(string fileName) {
        int length = fileName.Length;
        return fileName.Substring(length - 4, 4) == ".ics";
    }

    private byte[] GetBytes(Stream stream) {
        stream.Position = 0;
        byte[] buf = new byte[stream.Length];
        stream.Read(buf, 0, (int)stream.Length);
        return buf;
    }
}