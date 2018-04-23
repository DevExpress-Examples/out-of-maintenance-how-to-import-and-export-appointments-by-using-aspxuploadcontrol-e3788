using System.IO;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler.iCalendar;
using DevExpress.XtraScheduler;

public class ExportAppointmentCallbackCommand : MenuViewCallbackCommand {
    public ExportAppointmentCallbackCommand(ASPxScheduler control)
        : base(control) {
    }

    protected override void ExecuteCore() {
        //PostCalendarFile(Control.SelectedAppointments);
        PostCalendarFile(Control.Storage.Appointments.Items);
    }

    void PostCalendarFile(AppointmentBaseCollection appointments) {
        iCalendarExporter exporter = new iCalendarExporter(Control.Storage, appointments);
        
        if (appointments.Count == 0)
            return;
        
        MemoryStream memoryStream = new MemoryStream();

        exporter.Export(memoryStream);
        memoryStream.WriteTo(Control.Page.Response.OutputStream);
        Control.Page.Response.ContentType = "text/calendar";
        Control.Page.Response.AddHeader("Content-Disposition", "attachment; filename=appointment.ics");
        Control.Page.Response.End();
    }
}

public class ImportAppointmentsCallbackCommand : MenuViewCallbackCommand {
    public ImportAppointmentsCallbackCommand(ASPxScheduler control)
        : base(control) {
    }

    protected override void ExecuteCore() {
        Stream stream = GetStream();

        if (stream != null) {
            Control.Storage.Appointments.Clear();

            iCalendarImporter importer = new iCalendarImporter(Control.Storage);
            importer.Import(stream);
        }
    }

    private Stream GetStream() {
        byte[] buf = Control.Page.Session["UploadedFile"] as byte[];
        Stream stream = new MemoryStream(buf);
        return stream;
    }
}