<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="EnergyMonitoringSystem.Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="text-align: center; width: 100%;">
                <tr>
                    <td>&nbsp;
                    </td>
                    <td style="height: 300px; vertical-align: bottom;">
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; width: 50%;">&nbsp;
                    </td>
                    <td style="text-align: left;" colspan="2">
                        <asp:Timer ID="Timer1" runat="server" Interval="1000" OnTick="Timer1_Tick">
                        </asp:Timer>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; width: 60%;">You are being logged out,kindly do not close this window...
                    </td>
                    <td style="text-align: left;" colspan="2">
                        <asp:Image ID="imgValidator" runat="server" ImageUrl="~/Images/ajax-loader (5).gif" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>

</html>
