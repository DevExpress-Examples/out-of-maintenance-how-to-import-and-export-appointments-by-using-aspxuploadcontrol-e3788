Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports DevExpress.Web.ASPxMenu
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxUploadControl
Imports DevExpress.XtraScheduler

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private lastInsertedIds As New List(Of Integer)()

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If (Not IsPostBack) Then
			ASPxScheduler1.Start = New DateTime(2008, 7, 12)
		End If
	End Sub

	Protected Sub ASPxScheduler1_AppointmentCollectionCleared(ByVal sender As Object, ByVal e As EventArgs)
		lastInsertedIds.Clear()
	End Sub

	Protected Sub ASPxScheduler1_AppointmentRowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertingEventArgs)
		e.NewValues.Remove("ID")
	End Sub

	Protected Sub SqlDataSourceAppointments_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.SqlDataSourceStatusEventArgs)
		Dim connection As SqlConnection = CType(e.Command.Connection, SqlConnection)

		Using command As New SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", connection)
			lastInsertedIds.Add(Convert.ToInt32(command.ExecuteScalar()))
		End Using
	End Sub

	Protected Sub ASPxScheduler1_AppointmentRowInserted(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertedEventArgs)
		Dim count As Integer = lastInsertedIds.Count
		System.Diagnostics.Debug.Assert(count > 0)

		e.KeyFieldValue = lastInsertedIds(count - 1)
	End Sub

	Protected Sub ASPxScheduler1_AppointmentsInserted(ByVal sender As Object, ByVal e As DevExpress.XtraScheduler.PersistentObjectsEventArgs)
		Dim count As Integer = e.Objects.Count
		System.Diagnostics.Debug.Assert(count = lastInsertedIds.Count)

		Dim storage As ASPxSchedulerStorage = CType(sender, ASPxSchedulerStorage)
		For i As Integer = 0 To count - 1
			Dim apt As Appointment = CType(e.Objects(i), Appointment)
			Dim appointmentId As Integer = lastInsertedIds(i)
			storage.SetAppointmentId(apt, appointmentId)
		Next i

		lastInsertedIds.Clear()
	End Sub

	Protected Sub ASPxScheduler1_PopupMenuShowing(ByVal sender As Object, ByVal e As PopupMenuShowingEventArgs)
		If e.Menu.Id.Equals(SchedulerMenuItemId.DefaultMenu) Then
			e.Menu.Items(0).BeginGroup = True

			Dim menuItemImport As New MenuItem("Import Appointments", "ImportAppointments")
			e.Menu.Items.Insert(0, menuItemImport)

			Dim menuItemExport As New MenuItem("Export Appointments", "ExportAppointments")
			e.Menu.Items.Insert(1, menuItemExport)

			e.Menu.ClientSideEvents.ItemClick = "function(s, e) { DefaultViewMenuHandler(s, e); }"
		End If
	End Sub

	Protected Sub ASPxScheduler1_BeforeExecuteCallbackCommand(ByVal sender As Object, ByVal e As SchedulerCallbackCommandEventArgs)
		If e.CommandId = "EXPORTAPT" Then
			e.Command = New ExportAppointmentCallbackCommand(CType(sender, ASPxScheduler))
		ElseIf e.CommandId = "IMPRTAPT" Then
			e.Command = New ImportAppointmentsCallbackCommand(CType(sender, ASPxScheduler))
		End If
	End Sub

	Protected Sub ASPxUploadControl1_FileUploadComplete(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxUploadControl.FileUploadCompleteEventArgs)
		Dim uploadControl As ASPxUploadControl = CType(sender, ASPxUploadControl)
		Dim uploadedFile As UploadedFile = uploadControl.UploadedFiles(0)

		If (Not IsFileNameCorrect(uploadedFile.FileName)) Then
			e.IsValid = False
			e.ErrorText = "Incorrect file type"
			Return
		End If

		If uploadedFile.IsValid Then
			Session("UploadedFile") = GetBytes(uploadedFile.FileContent)
		End If
	End Sub

	Private Function IsFileNameCorrect(ByVal fileName As String) As Boolean
		Dim length As Integer = fileName.Length
		Return fileName.Substring(length - 4, 4) = ".ics"
	End Function

	Private Function GetBytes(ByVal stream As Stream) As Byte()
		stream.Position = 0
		Dim buf(stream.Length - 1) As Byte
		stream.Read(buf, 0, CInt(Fix(stream.Length)))
		Return buf
	End Function
End Class