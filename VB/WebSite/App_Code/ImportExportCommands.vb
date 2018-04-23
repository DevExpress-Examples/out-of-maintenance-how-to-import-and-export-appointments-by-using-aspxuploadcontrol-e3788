Imports System.IO
Imports DevExpress.Web
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxScheduler.Internal
Imports DevExpress.XtraScheduler.iCalendar
Imports DevExpress.XtraScheduler

Public Class ExportAppointmentCallbackCommand
    Inherits MenuViewCallbackCommand

    Public Sub New(ByVal control As ASPxScheduler)
        MyBase.New(control)
    End Sub

    Protected Overrides Sub ExecuteCore()
        'PostCalendarFile(Control.SelectedAppointments);
        PostCalendarFile(Control.Storage.Appointments.Items)
    End Sub

    Private Sub PostCalendarFile(ByVal appointments As AppointmentBaseCollection)
        Dim exporter As New iCalendarExporter(Control.Storage, appointments)

        If appointments.Count = 0 Then
            Return
        End If

        Dim memoryStream As New MemoryStream()

        exporter.Export(memoryStream)
        memoryStream.WriteTo(Control.Page.Response.OutputStream)
        Control.Page.Response.ContentType = "text/calendar"
        Control.Page.Response.AddHeader("Content-Disposition", "attachment; filename=appointment.ics")
        Control.Page.Response.End()
    End Sub
End Class

Public Class ImportAppointmentsCallbackCommand
    Inherits MenuViewCallbackCommand

    Public Sub New(ByVal control As ASPxScheduler)
        MyBase.New(control)
    End Sub

    Protected Overrides Sub ExecuteCore()
        Dim stream As Stream = GetStream()

        If stream IsNot Nothing Then
            Control.Storage.Appointments.Clear()

            Dim importer As New iCalendarImporter(Control.Storage)
            importer.Import(stream)
        End If
    End Sub

    Private Function GetStream() As Stream
        Dim buf() As Byte = TryCast(Control.Page.Session("UploadedFile"), Byte())
        Dim stream As Stream = New MemoryStream(buf)
        Return stream
    End Function
End Class