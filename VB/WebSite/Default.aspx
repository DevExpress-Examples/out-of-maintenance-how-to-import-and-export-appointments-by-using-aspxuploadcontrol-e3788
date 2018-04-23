<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register assembly="DevExpress.Web.ASPxScheduler.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxScheduler" tagprefix="dxwschs" %>
<%@ Register assembly="DevExpress.XtraScheduler.v14.1.Core, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraScheduler" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxUploadControl" tagprefix="dxu" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" tagPrefix="dx" namespace="DevExpress.Web.ASPxEditors" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function DefaultViewMenuHandler(s, e) {
            if (e.item.GetItemCount() <= 0) {
                if (e.item.name == "ExportAppointments") {
                    scheduler.SendPostBack("EXPORTAPT|");
                }
                else if (e.item.name == "ImportAppointments") {
                    if (uploader.GetText()) {
                        uploader.UploadFile();
                    }
                    else {
                        alert('Please choose *.ics file with appointments prior to executing this command.');
                    }
                }
                else {
                    scheduler.RaiseCallback("MNUVIEW|" + e.item.name);
                }
            }
        }

        function Uploader_TextChanged() {
            alert("The source file has been selected. Choose 'Import Appointments' scheduler's context menu item to upload file and import appointments.");
        }

        function Uploader_FileUploadStart() {
            scheduler.ShowLoadingPanel();
        }

        function Uploader_FileUploadComplete() {
            scheduler.HideLoadingPanel();
            scheduler.RaiseCallback("IMPRTAPT|");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td valign="top" style="padding-top: 5px;">
                    <dx:ASPxLabel ID="lblSelect" runat="server" Text="Select iCalendar file: " />
                </td>
                <td valign="top">
                    <dxu:ASPxUploadControl ID="ASPxUploadControl1" runat="server" ClientInstanceName="uploader" 
                        OnFileUploadComplete="ASPxUploadControl1_FileUploadComplete">
                        <ValidationSettings MaxFileSize="1048576" AllowedFileExtensions=".ics"/>
                        <ClientSideEvents TextChanged="function(s, e) { Uploader_TextChanged(); }"
                            FileUploadComplete="function(s, e) { Uploader_FileUploadComplete(); }"
                        FileUploadStart="function(s, e) { Uploader_FileUploadStart(); }" />
                    </dxu:ASPxUploadControl>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <dx:ASPxLabel ID="lblMaxFileSize" runat="server" Text="Maximum file size: 1Mb" Font-Size="8pt" />
                </td>
            </tr>
        </table>
        <p/>
        <dxwschs:ASPxScheduler ID="ASPxScheduler1" runat="server" ClientInstanceName="scheduler"
            AppointmentDataSourceID="SqlDataSourceAppointments" 
            ResourceDataSourceID="SqlDataSourceResources"
            onappointmentrowinserted="ASPxScheduler1_AppointmentRowInserted" 
            onappointmentrowinserting="ASPxScheduler1_AppointmentRowInserting" 
            onappointmentsinserted="ASPxScheduler1_AppointmentsInserted" 
            onpopupmenushowing="ASPxScheduler1_PopupMenuShowing"
            onbeforeexecutecallbackcommand="ASPxScheduler1_BeforeExecuteCallbackCommand" 
            onappointmentcollectioncleared="ASPxScheduler1_AppointmentCollectionCleared">
            <Storage>
                <Appointments ResourceSharing="True">
                    <Mappings 
                        AppointmentId="ID" 
                        AllDay="AllDay" 
                        Description="Description" 
                        End="EndTime" 
                        Label="Label" 
                        Location="Location" 
                        RecurrenceInfo="RecurrenceInfo" 
                        ReminderInfo="ReminderInfo" 
                        ResourceId="CarId" 
                        Start="StartTime" 
                        Status="Status" 
                        Subject="Subject" 
                        Type="EventType" />
                </Appointments>
                <Resources>
                    <Mappings 
                        ResourceId="ID"  
                        Caption="Model" />
                </Resources>
            </Storage>
        </dxwschs:ASPxScheduler>

        <asp:SqlDataSource ID="SqlDataSourceResources" runat="server" 
            ConnectionString="<%$ ConnectionStrings:CarsXtraSchedulingConnectionString %>" 
            SelectCommand="SELECT [ID], [Model] FROM [Cars]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAppointments" runat="server" 
            ConnectionString="<%$ ConnectionStrings:CarsXtraSchedulingConnectionString %>" 
            DeleteCommand="DELETE FROM [CarScheduling] WHERE [ID] = @ID" 
            InsertCommand="INSERT INTO [CarScheduling] ([CarId], [UserId], [Status], [Subject], [Description], [Label], [StartTime], [EndTime], [Location], [AllDay], [EventType], [RecurrenceInfo], [ReminderInfo], [Price], [ContactInfo]) VALUES (@CarId, @UserId, @Status, @Subject, @Description, @Label, @StartTime, @EndTime, @Location, @AllDay, @EventType, @RecurrenceInfo, @ReminderInfo, @Price, @ContactInfo)" 
            SelectCommand="SELECT * FROM [CarScheduling]" 
            UpdateCommand="UPDATE [CarScheduling] SET [CarId] = @CarId, [UserId] = @UserId, [Status] = @Status, [Subject] = @Subject, [Description] = @Description, [Label] = @Label, [StartTime] = @StartTime, [EndTime] = @EndTime, [Location] = @Location, [AllDay] = @AllDay, [EventType] = @EventType, [RecurrenceInfo] = @RecurrenceInfo, [ReminderInfo] = @ReminderInfo, [Price] = @Price, [ContactInfo] = @ContactInfo WHERE [ID] = @ID" 
            oninserted="SqlDataSourceAppointments_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="ID" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="ID" Type="Int32" />
                <asp:Parameter Name="CarId" Type="String" />
                <asp:Parameter Name="UserId" Type="Int32" />
                <asp:Parameter Name="Status" Type="Int32" />
                <asp:Parameter Name="Subject" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Label" Type="Int32" />
                <asp:Parameter Name="StartTime" Type="DateTime" />
                <asp:Parameter Name="EndTime" Type="DateTime" />
                <asp:Parameter Name="Location" Type="String" />
                <asp:Parameter Name="AllDay" Type="Boolean" />
                <asp:Parameter Name="EventType" Type="Int32" />
                <asp:Parameter Name="RecurrenceInfo" Type="String" />
                <asp:Parameter Name="ReminderInfo" Type="String" />
                <asp:Parameter Name="Price" Type="Decimal" />
                <asp:Parameter Name="ContactInfo" Type="String" />
            </UpdateParameters>
            <InsertParameters>
                <asp:Parameter Name="CarId" Type="String" />
                <asp:Parameter Name="UserId" Type="Int32" />
                <asp:Parameter Name="Status" Type="Int32" />
                <asp:Parameter Name="Subject" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Label" Type="Int32" />
                <asp:Parameter Name="StartTime" Type="DateTime" />
                <asp:Parameter Name="EndTime" Type="DateTime" />
                <asp:Parameter Name="Location" Type="String" />
                <asp:Parameter Name="AllDay" Type="Boolean" />
                <asp:Parameter Name="EventType" Type="Int32" />
                <asp:Parameter Name="RecurrenceInfo" Type="String" />
                <asp:Parameter Name="ReminderInfo" Type="String" />
                <asp:Parameter Name="Price" Type="Decimal" />
                <asp:Parameter Name="ContactInfo" Type="String" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>