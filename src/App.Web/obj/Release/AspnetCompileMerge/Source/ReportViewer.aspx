<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="LMS_Portal.UI.HRLoans.ReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="56000">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="rv" runat="server"
                Font-Names="Verdana" Font-Size="8pt"
                ShowBackButton="False" ShowCredentialPrompts="False"
                ShowDocumentMapButton="False" ShowFindControls="False"
                ShowPageNavigationControls="True" ShowParameterPrompts="False"
                ShowPromptAreaButton="False" ShowPrintButton="true" Height="500px" Width="100%" BackColor="White" BorderStyle="None" ZoomPercent="100">
            </rsweb:ReportViewer>
        </div>
    </form>

    <script type="text/javascript">
        $(document).ready(function () {
            //if ($("#rv_ctl09_ctl04_ctl00_Menu") != null &&
            //    $("#rv_ctl09_ctl04_ctl00_Menu") != undefined) {
            //    $("#rv_ctl09_ctl04_ctl00_Menu").append("<div class='NormalButton'><a id='rv_ctl09_ctl04_ctl00_Menu_csv_anchor_tag' style='font-family:Verdana;font-size:8pt;padding:14px 18px 14px 18px;display:block;white-space:nowrap;text-decoration:none;text-overflow:ellipsis;text-align:left;' class='ActiveLink' title='CSV' alt='CSV' href='javascript:void(0);'>CSV</a></div>");
            //}

            //$('#rv_ctl09_ctl04_ctl00_Menu_csv_anchor_tag').click(function () {
            //    alert("Testing. . .");
            //});

        });
    </script>
</body>
</html>
