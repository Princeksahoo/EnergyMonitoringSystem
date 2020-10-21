<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="EnergyMonitoringSystem.Dashboard" %>
<asp:Content ID="MainContainer" ContentPlaceHolderID="MainContent" runat="server">
    <%--    <script src="Scripts/jquery-3.3.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <link href="Content/bootstrap.css" rel="stylesheet" />--%>
<%--    <script src="Scripts/HighChart/highcharts.js"></script>--%>
    <script src="Scripts/DateTimePicker/moment.js"></script>
    <script src="Scripts/DateTimePicker/bootstrap-datetimepicker.js"></script>
    <link href="Scripts/DateTimePicker/bootstrap-datetimepicker.css" rel="stylesheet" />
    <script src="Scripts/JavaScriptUIBlocker.js"></script>
    <div class="container-fluid">
        <div class="">
            <div id="divOverlay" class="overlay">
                <div id="divProcess" class="process">
                    <asp:Image runat="server" ImageUrl="~/Images/ajax-loader.gif" /></div>
            </div>
            <asp:UpdatePanel runat="server" ID="updFilter">
                <ContentTemplate>
                    <div style="height: 49px; width: 8%; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); float: right; border: 2px solid #ece3e3; text-align:center;border-radius:7px;">
                        <div Style="  margin-right: 10px;height:35%;box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2), 0 2px 5px 0 rgba(0, 0, 0, 0.19);width:100%;font-weight:bold;background-color:#EDEEF5;border-radius:3px ">
                            <asp:Label runat="server" Text="Hide Graphs" CssClass=" selectlbl" style="font-size: 11.5px;line-height:100%;" />
                        </div>
                         
                        <span style="height: auto; cursor: pointer; color: black;line-height:27px; " id="uncheckedBtn" onclick="ShowHideGraph(this);" title="Hide Graphs"><i class="glyphicon glyphicon-unchecked" style="font-size: 24px; vertical-align: middle;"></i></span>
                        <span style="height: auto; cursor: pointer; color: black;line-height:27px; display: none; " id="checkedBtn" onclick="ShowHideGraph(this);" title="Hide Graphs"><i class="glyphicon glyphicon-check" style="font-size: 24px; vertical-align: middle"></i></span>
                    </div>
                    <div class="" style="height: auto; border: 2px solid #ece3e3; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); width: 91%;border-radius:7px;">
                        <table style="" class="masterFS">
                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Date" ID="lblDate" CssClass=" selectlbl" Style=" margin-right: 10px;font-size: 16px;line-height: 35px;" />
                                </td>
                                <td class="input-group" runat="server">
                                    <div class="input-group-addon">
                                        <i class="glyphicon glyphicon-calendar"></i>
                                    </div>
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control date" placeholder="DD-MM-YYYY" MaxLength="15" Style="                                            min-width: 130px;
                                            max-width: 160px;
                                            margin: 0 10px 0 0;" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label runat="server" Text="Shift" ID="lblShift" CssClass=" selectlbl" Style="margin: 0 10px 0 0; font-size: 16px; line-height: 35px;" />
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlShift" CssClass="form-control specInputField" Style="margin: 0 10px 0 0;" />
                                </td>
                                <td>
                                    <asp:Button runat="server" ID="btnProcess" ClientIDMode="Static" Text="Process" OnClick="btnProcess_Click" CssClass="Btns" Style="margin-left: 10px;"  />
                                </td>

                            </tr>
                        </table>
                        <table style="width: auto; display: inline-block; vertical-align: middle; margin: 5px; float: right;">
                            <tr>
                                <td style="                                        width: auto;
                                        float: right;
                                ">
                                    <asp:Label runat="server" Text="Auto Refresh" CssClass=" selectlbl" Style="display: inline-block; vertical-align: middle; font-size: 18px; margin-right: 10px; line-height: 35px;" />
                                    <label class="switch">
                                        <asp:CheckBox runat="server" ID="cbAutoRefresh" AutoPostBack="True" OnCheckedChanged="cbAutoRefresh_CheckedChanged" meta:resourcekey="cbAutoRefreshResource1" />
                                        <span class="slider round"></span>
                                    </label>
                                </td>
                            </tr>
                        </table>
                        <asp:Timer ID="timerToAutoRefresh" runat="server" Enabled="False" OnTick="timerToAutoRefresh_Tick"></asp:Timer>
                    </div>
                    <div id="divDashboard" style="border: 2px solid #ece3e3; margin: 10px auto 0 auto;">
                        <div id="tblGrid" class="" style="overflow: auto; background-color: transparent; height: 50%; width: 100%;">
                            <asp:GridView runat="server" ID="gvDashboard" AutoGenerateColumns="false" AllowSorting="true" CssClass="table table-bordered cockpit headerFixerTable" AllowPaging="false" ShowHeader="true" ShowFooter="false" ShowHeaderWhenEmpty="true" OnRowDataBound="gvDashboard_RowDataBound">
                                <EmptyDataTemplate>
                                    No records found.
                                </EmptyDataTemplate>
                                <EmptyDataRowStyle BackColor="White" ForeColor="Red" HorizontalAlign="Center" />
                                <HeaderStyle CssClass="HeaderCss" />
                                <RowStyle BackColor="White" ForeColor="Black" />
                                <AlternatingRowStyle BackColor="#DCDCDC" ForeColor="Black" />
                            </asp:GridView>
                        </div>
                        <div id="divCharts" style="height: 50%; width: 100%; padding: 3px 0;">
                            <div id="divEnergy" style="background-color: transparent; height: 100%; margin: 0 0 0 0; width: 60%; display: inline-block; border: 1px solid black;">
                            </div>
                            <div id="divSummery" style="background-color: transparent; height: 100%; padding: 0; width: 40%; display: inline-block; float: right; border: 1px solid black; text-align: center;">
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="modal fade" id="HousewiseMonitoringDiv" role="dialog" style="min-width: 800px; height: auto;">
        <div id="divHourwiseModal" class="modal-dialog  modal-dialog-centered" style="border-radius: 25%;">
            <div  id="divHourwiseMonitor" class="modal-content" style="border: 2px solid #5D7B9D">
                <div class="modal-header" style="background-color: #EDEEF5; padding: 5px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);">
                    <button type="button" class="close" data-dismiss="modal" style="font-size: 32px;vertical-align:middle;">&times;</button>
                    <img src="Images/iconApp.ico" height="30" style="padding: 1px; display: inline-block;" />
                    <h4 class="modal-title" style="color: black; display: inline-block; vertical-align: middle;">Hourwise Energy Cockpit</h4>

                </div>               
                <div class="modal-body" style="padding-top: 0;height:auto;">
                    <div style="display:flex;justify-content:space-between;">
                        <asp:Label ID="lblShiftnDate" EnableViewState="False" runat="server" Style="font-weight: bold; font-family: Calibri; color: #fc3503; font-size: 21px;width:32%;" Text="20-12-2019(Shift- First)"></asp:Label>
                        <asp:Label ID="lblHourwiseHeader" EnableViewState="False" runat="server" Style="font-weight: bold; font-family: Calibri; color: #fc3503; font-size: 21px;width:32%;" Text="Hourwise Energy Cockpit For - FANUC"></asp:Label>
                        <div style="width:32%;text-align:right;">
                            <asp:Label ID="Label1" EnableViewState="False" runat="server" Style="font-weight: bold; font-family: Calibri; color: #fc3503; font-size: 21px;" Text="Hide Graphs"></asp:Label>
                            <span style="height: auto; cursor: pointer; color: black;line-height:30px;text-align:right;vertical-align:top;" id="uncheckedHourBtn" onclick="ShowHideHourwiseGraph(this);" title="Hide Graphs"><i class="glyphicon glyphicon-unchecked" style="font-size: 21px; vertical-align: sub;"></i></span>
                            <span style="height: auto; cursor: pointer; color: black; display: none;line-height:30px;text-align:right;vertical-align:top;" id="checkedHourBtn" onclick="ShowHideHourwiseGraph(this);" title="Hide Graphs"><i class="glyphicon glyphicon-check" style="font-size: 21px; vertical-align: sub"></i></span>
                        </div>
                        
                    </div>
                    <div id="divHourwise" style="border: 2px solid #ece3e3; margin: 5px auto 0 auto; width: 100%;box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);">
                        <div id="divHourwiseTable" style="height: 50%; overflow: auto; background-color: transparent; width: 100%; ">
                            <table id="tblHourMonitoring" class="table table-bordered cockpit headerFixerTable">
                                <thead class="HeaderCss">
                                    <tr>
                                        <th>Hour</th>
                                        <th>VLN-R  (Min.\Max.)</th>
                                        <th>VLN-Y  (Min.\Max.)</th>
                                        <th>VLN-B  (Min.\Max.)</th>
                                        <th>Power Factor (PF)</th>
                                        <th>Production Time</th>
                                        <th>Production Count</th>
                                        <th>Energy (KwH)</th>
                                        <th>Cost (INR)</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div id="divHourwiseCharts" style="height: 50%; width: 100%; padding: 3px 0;">
                            <div id="divHourwiseEnergy" style="background-color: transparent; height: 100%; margin: 0 0 0 0; width: 60%; display: inline-block; border: 1px solid black;">
                            </div>
                            <div id="divHourwiseSummary" style="background-color: transparent; height: 100%; padding: 0; width: 40%; display: inline-block; float: right; border: 1px solid black; text-align: center;">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        var checked = false;
        function resize() {
            debugger;
            var heights = window.innerHeight;
            var widths = window.innerWidth;
            document.getElementById("divDashboard").style.height = heights - 140 + "px";
            document.getElementById("divHourwiseMonitor").style.height = heights * 0.9 + "px";
            document.getElementById("divHourwiseModal").style.width = widths * 0.8 + "px";
            document.getElementById("divHourwiseModal").style.height = heights * 0.9 + "px";
            document.getElementById("divHourwise").style.height = heights * 0.7 + "px";

        }
        function ShowHideProcessIndicator() {
            debugger;
            if ($("#divOverlay").css("display")=="none") {
                $("#divOverlay").css("display", "block");
            }
            else {
                $("#divOverlay").css("display", "none") ;
            }
        }
        function ShowHideGraph(obj) {
            debugger;
            if (obj == document.getElementById("uncheckedBtn")) {
                document.getElementById("uncheckedBtn").style.display = "none";
                document.getElementById("checkedBtn").style.display = "inline";
                HIdeGraphs();
                checked = true;
            }
            else {
                document.getElementById("checkedBtn").style.display = "none";
                document.getElementById("uncheckedBtn").style.display = "inline";
                ShowGraphs();
                checked = false;
            }

        }
        function ShowHideHourwiseGraph(obj) {
            debugger;
            if (obj == document.getElementById("uncheckedHourBtn")) {
                document.getElementById("uncheckedHourBtn").style.display = "none";
                document.getElementById("checkedHourBtn").style.display = "inline";
                document.getElementById("divHourwiseCharts").style.display = "none";
                document.getElementById("divHourwiseTable").style.height = "100%";
            }
            else {
                document.getElementById("checkedHourBtn").style.display = "none";
                document.getElementById("uncheckedHourBtn").style.display = "inline";
                document.getElementById("divHourwiseCharts").style.display = "block";
                document.getElementById("divHourwiseTable").style.height = "50%";
            }
        }
        function HIdeGraphs() {
            debugger;
            document.getElementById("divCharts").style.display = "none";
            document.getElementById("tblGrid").style.height = "100%";
        }
        function ShowGraphs() {
            document.getElementById("divCharts").style.display = "block";
            document.getElementById("tblGrid").style.height = "50%";
        }
        
        function BtnProcessClick() {
            debugger;      
            var fromDate = $("[id$=txtFromDate]").val();
            var selectedShift = $("[id$=ddlShift]").val();
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "Dashboard.aspx/GetEnergyBarData",
                data: '{fromDate:"' + fromDate + '", selectedShift:"' + selectedShift + '"}',
                dataType: "json",
                success: EnergyBarChart,
                error: function (Result) {
                    alert("Error");
                }
            });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "Dashboard.aspx/GetSummeryPieData",
                data: '{fromDate:"' + fromDate + '", selectedShift:"' + selectedShift + '"}',
                dataType: "json",
                success: SummeryPieChart,
                error: function (Result) {
                    alert("Error");
                }
            });
            //$.blockUI({ message: '<img src="Images/ajax-loader.gif" />' });
            
        }

        function EnergyBarChart(data) {
            Highcharts.chart('divEnergy', {
                chart: {
                    type: 'column'
                },
                title: {
                    text: data.d.TitleEnergy
                },
                xAxis: {
                    categories: data.d.EnergyCategories,
                    crosshair: true
                },
                yAxis: {
                    title: {
                        text: data.d.yAxisTitleEnergy
                    }
                },
                tooltip: {
                    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                        '<td style="padding:0"><b>{point.y:.1f} KwH</b></td></tr>',
                    footerFormat: '</table>',
                    shared: true,
                    useHTML: true
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.05,
                        borderWidth: 0
                    }
                },
                credits: {
                    enabled: false
                },
                series: data.d.seriesEnergy
            });
        }

        function SummeryPieChart(data) {
            debugger;
            Highcharts.chart('divSummery', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                legend: {
                    align: 'right',
                    layout: 'vertical',
                    verticalAlign: 'top',
                    x: 15,
                    y: 30
                },
                credits: {
                    enabled: false
                },
                title: {
                    text: 'Summary'
                },
                tooltip: {
                    formatter: function () {
                        var val = this.point.positive ? this.y : this.y * (-1);
                        return "<b>" + this.point.name + "</b><br><b>" + this.series.name + ': </b>' + val + 'KwH';
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false,
                            formatter: function () {
                                return this.point.positive ? this.y : this.y * (-1);
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    name: data.d.name,
                    colorByPoint: true,
                    data: data.d.data
                }]
            });
            $.unblockUI({});
        }

        function OpenHourwiseMonitoring() {
            $('[id*=HousewiseMonitoringDiv]').modal('show');
        }

        function BindHourwiseDataToTable(data) {
            debugger;

            var itmData = data.d;
            $("#tblHourMonitoring tr:gt(0)").each(function () {
                $(this).remove();
            });
            var strb;
            if (itmData.length > 0) {
                for (var i = 0; i < itmData.length; i++) {
                    strb = strb + '<tr><td>' + itmData[i].ShiftHourID + '</td><td>' + itmData[i].Volt1 + '</td><td>' + itmData[i].Volt2 + '</td><td>' + itmData[i].Volt3 + '</td><td>' + itmData[i].PF + '</td><td>' + itmData[i].UtilisedTime + '</td><td>' + itmData[i].Components + '</td><td>' + itmData[i].Energy + '</td><td>' + itmData[i].Cost + '</td></tr>'
                }
            }
            $("#tblHourMonitoring").append(strb);
        }

        function PlotHourwiseEnergyChart(data) {
            Highcharts.chart('divHourwiseEnergy', {
                chart: {
                    type: 'column'
                },
                title: {
                    text: data.d.TitleEnergy
                },
                xAxis: {
                    categories: data.d.EnergyCategories,
                    crosshair: true
                },
                yAxis: {
                    title: {
                        text: data.d.yAxisTitleEnergy
                    }
                },
                tooltip: {
                    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                        '<td style="padding:0"><b>{point.y:.1f} KwH</b></td></tr>',
                    footerFormat: '</table>',
                    shared: true,
                    useHTML: true
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.05,
                        borderWidth: 0
                    }
                },
                credits: {
                    enabled: false
                },
                series: data.d.seriesEnergy
            });
        }

        function PlotHourwiseSummary(data) {
            Highcharts.chart('divHourwiseSummary', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                credits: {
                    enabled: false
                },
                title: {
                    text: 'Summary'
                },
                tooltip: {
                    formatter: function () {
                        var val = this.point.positive ? this.y : this.y * (-1);
                        return "<b>" + this.point.name + "</b><br><b>" + this.series.name + ': </b>' + val + 'KwH';
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false,
                            formatter: function () {
                                return this.point.positive ? this.y : this.y * (-1);
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    name: data.d.name,
                    colorByPoint: true,
                    data: data.d.data
                }]
            });
        }

        $(document).ready(function () {
            resize();
            window.onresize = function () {
                resize();
            };
            $('.date').datetimepicker({
                format: 'DD-MM-YYYY',
                locale: 'en-US'
            });
            $("#btnProcess").click(function () {
                debugger;
                $.blockUI({ message: '<img src="Images/ajax-loader.gif" />' });
            });
            $("[id*=gvDashboard]").find("[id*=hlHourwise]").click(function () {
                debugger;
                var machine = $(this).text();
                var date = $('[id$=txtFromDate]').val();
                var shift = $('[id$=ddlShift]').val();
                $("[id$=lblShiftnDate]").html(date + " (Shift - " + shift + ")");
                $("[id$=lblHourwiseHeader]").html("Hourwise Energy Cockpit For - " + machine);
                $("#tblHourMonitoring tr:gt(0)").each(function () {
                    $(this).remove();
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseMonitoring",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: BindHourwiseDataToTable,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseEnergy",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: PlotHourwiseEnergyChart,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseSummary",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: PlotHourwiseSummary,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                OpenHourwiseMonitoring();
            });
            //BtnProcessClick();
           
        });
       
       
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            resize();
            window.onresize = function () {
                resize();
            };
            $('.date').datetimepicker({
                format: 'DD-MM-YYYY',
                locale: 'en-US'
            });
            $("#btnProcess").click(function () {
                debugger;
                $.blockUI({ message: '<img src="Images/ajax-loader.gif" />' });
            });
            $("[id*=gvDashboard]").find("[id*=hlHourwise]").click(function () {
                debugger;
                var machine = $(this).text();
                var date = $('[id$=txtFromDate]').val();
                var shift = $('[id$=ddlShift]').val();
                $("[id$=lblShiftnDate]").html(date + " (Shift - " + shift + ")");
                $("[id$=lblHourwiseHeader]").html("Hourwise Energy Cockpit For - " + machine);
                $("#tblHourMonitoring tr:gt(0)").each(function () {
                    $(this).remove();
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseMonitoring",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: BindHourwiseDataToTable,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseEnergy",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: PlotHourwiseEnergyChart,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                $.ajax({
                    type: "POST",
                    url: "Dashboard.aspx/GetHourwiseSummary",
                    contentType: "application/json; charset=utf-8",
                    data: '{machine:"' + machine + '", date:"' + date + '", shift:"' + shift + '"}',
                    dataType: "json",
                    success: PlotHourwiseSummary,
                    error: function (Result) {
                        alert("Error");
                    }
                });
                OpenHourwiseMonitoring();
            });
            debugger;
            if (checked) {
                ShowHideGraph(document.getElementById("uncheckedBtn"));
            }
            else {
                ShowHideGraph(document.getElementById("checkedBtn"));
            }
        });


    </script>
    <style>
        .overlay {
            position: fixed;
            display: none;
            width: 100%;
            height: 100%;
            top: 0;
            left: 0;
            right: -1px;
            bottom: 0;
            background-color: rgba(0,0,0,0.5);
            z-index: 9999;
            cursor: pointer;
        }

        .process {
            position: absolute;
            padding: 0;
            margin: 0;
            width: 30%;
            top: 50%;
            left: 50%;
            text-align: center;
            color: #000;
            border: 3px solid #aaa;
            background-color: #fff;
            transform: translate(-50%,-50%);
            -ms-transform: translate(-50%,-50%);
        }
        fieldset {
            /*border: 1px solid #2B7B78;*/
            /*box-shadow: 2px 2px 8px 2px #efe7e7;*/
        }

        a {
            cursor: pointer;
            text-decoration: underline;
            font-weight: bold;
        }
        .highcharts-root{
            width:100%;
            height:100%;
        }
        .masterFS {
            margin: 5px;
            padding: 0 5px 10px 5px;
            width: auto;
            display: inline-block;
            vertical-align: middle;
            padding: 0px;
            border-radius: 4px;
            width: auto;
            /*border: 1px solid #ece3e3;*/
            height: auto;
        }

            .masterFS tr td {
                width: auto;
            }

        #selectShift > div {
            display: flex;
        }

        #selectShift {
            max-width: 90%;
            margin: 0% auto;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
            grid-gap: 20px;
        }
        /*table tr td{
            width:auto;
            max-width:200px;
            margin-right:10px;
        }*/
        legend {
            text-align: left;
            color: white;
            display: block;
            width: auto;
            padding: 0;
            margin-bottom: 5px;
            line-height: inherit;
            border-bottom: transparent;
            color: black;
            font-size: 15px;
            font-weight: bold;
        }

        .specInputField {
            min-width: 160px;
            max-width: 200px;
        }

        .selectlbl {
            color: black;
        }

        .headerFixerTable tr th {
            position: sticky;
            top: -1px;
            z-index: 1;
            background-color: #2e6886;
            color: white;
            min-width: 150px;
            text-align: center;
        }

        .table tr:nth-child(odd) {
            background-color: #DCDCDC;
        }

        .table tr:nth-child(even) {
            background-color: white;
        }

        .table tbody > tr > th {
            vertical-align: middle;
        }

        .table tbody > tr > td {
            text-align: center;
            vertical-align: middle;
            height: 30px;
        }

        .table > tr > td {
            text-align: center;
            vertical-align: middle;
            height: 30px;
        }

        .HeaderCss th {
            color: white;
            background-color: #2E6886 !important;
            height: 50px;
            vertical-align: inherit;
        }

        .switch {
            position: relative;
            display: inline-block;
            vertical-align: middle;
            width: 50px;
            height: 30px;
            float: right;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            -webkit-transition: .4s;
            transition: .4s;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 22px;
                width: 22px;
                left: 3px;
                bottom: 3px;
                background-color: white;
                -webkit-transition: .4s;
                transition: .4s;
            }

        input:checked + .slider {
            background-color: #2196F3;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(23px);
            -ms-transform: translateX(23px);
            transform: translateX(23px);
        }

        /* Rounded sliders */
        .slider.round {
            border-radius: 30px;
        }

            .slider.round:before {
                border-radius: 50%;
            }
    </style>
</asp:Content>
