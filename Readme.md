<!-- default file list -->
*Files to look at*:

* [ImportExportCommands.cs](./CS/WebSite/App_Code/ImportExportCommands.cs) (VB: [ImportExportCommands.vb](./VB/WebSite/App_Code/ImportExportCommands.vb))
* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# How to import and export appointments by using ASPxUploadControl


<p>This example combines the approach shown in the following demo modules into one simple application:</p><p><a href="http://demos.devexpress.com/ASPxSchedulerDemos/iCalendar/iCalendarImport.aspx"><u>iCalendar - iCalendar Import</u></a><br />
<a href="http://demos.devexpress.com/ASPxSchedulerDemos/iCalendar/iCalendarExport.aspx"><u>iCalendar - iCalendar Export</u></a></p><p>Special <strong>ImportAppointmentsCallbackCommand </strong>and <strong>ExportAppointmentsCallbackCommand</strong> <a href="http://documentation.devexpress.com/#AspNet/CustomDocument5462"><u>callback commands</u></a> are used to import and export appointments. Execution of these commands is initiated form the customized context menu items (see the <a href="https://www.devexpress.com/Support/Center/p/E291">How to change default menu items and actions in the popup menu</a> example to learn more on this subject).</p><p>Note the manner in which Ids of the last inserted appointments are handled. If you compare the approach from the <a href="http://documentation.devexpress.com/#AspNet/CustomDocument3844"><u>How to: Bind an ASPxScheduler to Data at Design Time</u></a> help section (see step #14) with the approach from this example, you will see that the later is more general because it maintains a list of Ids. This implementation is obligatory in import/export scenarios (see <a href="https://www.devexpress.com/Support/Center/p/B184873"> iCalendar Import Demo - incorrect AccessRowInsertionProvider class implementation results in an unhandled exception</a>).</p><p>Prior to running this example, it is required to register a CarsXtraScheduling database on your local SQL Server instance. You can download the corresponding SQL scripts from the <a href="https://www.devexpress.com/Support/Center/p/E215">How to bind ASPxScheduler to MS SQL Server database</a> example.</p>

<br/>


