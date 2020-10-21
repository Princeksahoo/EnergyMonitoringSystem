<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EnergyReportPage.aspx.cs" Inherits="EnergyMonitoringSystem.EnergyReportPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="Scripts/DateTimePicker/moment.js"></script>
    <script src="Scripts/DateTimePicker/bootstrap-datetimepicker.js"></script>
    <link href="Scripts/DateTimePicker/bootstrap-datetimepicker.css" rel="stylesheet" />
    <script src="Scripts/bootstrap-multiselect.js"></script>
    <link href="Scripts/bootstrap-multiselect.css" rel="stylesheet" />

<%--    <%: Styles.Render("~/bundles/datecss") %>
    <%: Scripts.Render("~/bundles/datejs") %>
    <%: Styles.Render("~/bundles/datetimepickercss") %>
    <%: Scripts.Render("~/bundles/datetimepickerjs") %>
    <%: Styles.Render("~/bundles/multiselectcss") %>
    <%: Scripts.Render("~/bundles/multiselectjs") %>--%>
    <style>
        #tblfilter tr td {
            vertical-align: middle;
        }

        #tblfilter tbody tr:nth-child(odd) {
            background-color: #DCDCDC;
        }

        #tblfilter tbody tr:nth-child(even) {
            background-color: #DCDCDC;
        }

        .multiselect-container {
            height: 300px;
            overflow-x: auto;
        }

        .multiselect-selected-text {
            padding-right: 181px;
        }

        .open > .dropdown-menu {
            display: block;
            left: auto;
        }

        .multiselect .dropdown-toggle {
            width: 50%;
        }

        .cssclass {
            min-width: 350px;
        }
    </style>
    <div class="row" style="text-align: center; color: red;">
        <asp:Label ID="lblMessages" EnableViewState="False" runat="server" Style="font-weight: bold; font-family: Calibri;" meta:resourcekey="lblMessagesResource1"></asp:Label>
        <asp:HiddenField ID="width" runat="server" />
        <asp:HiddenField ID="height" runat="server" />
    </div>
    <div class="row">
        <div class="col-md-12" style="margin: auto;">
            <h1 class="text-center login-title commontd" style="color: #fc3503;">Energy Report</h1>
            <div class="account-wall">
                <div class="col-md-12">
                    <div class="form-signin">
                        <asp:UpdatePanel ID="upadetPanalReport" runat="server">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnGenerate" />
                            </Triggers>
                            <ContentTemplate>
                                <table id="tblfilter" class="table table-bordered table-striped" style="margin: auto; width: auto;box-shadow: 0 6px 10px 0 rgba(0, 0, 0, 0.2), 0 8px 22px 0 rgba(0, 0, 0, 0.19);border-radius:7px;">
                                    <tr id="trFromDate" runat="server">
                                        <td runat="server">
                                            <b>From Date</b> </td>
                                        <td class="input-group" runat="server">
                                            <div class="input-group-addon">
                                                <i class="glyphicon glyphicon-calendar"></i>
                                            </div>
                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control date" placeholder="DD-MM-YYYY" MaxLength="15" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:TextBox ID="txtFromDateTime" runat="server" CssClass="form-control dateTime" placeholder="DD-MM-YYYY HH:mm:ss" MaxLength="15" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:TextBox ID="txtFromMonth" runat="server" CssClass="form-control month" placeholder="MMM-YYYY" MaxLength="15" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr id="trToDate" runat="server">
                                        <td runat="server">
                                            <b>To Date</b> </td>
                                        <td class="input-group" runat="server">
                                            <div class="input-group-addon">
                                                <i class="glyphicon glyphicon-calendar"></i>
                                            </div>
                                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control date" placeholder="DD-MM-YYYY" MaxLength="15" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:TextBox ID="txtToDateTime" runat="server" CssClass="form-control dateTime" placeholder="DD-MM-YYYY HH:mm:ss" MaxLength="15" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:TextBox ID="txtToMonth" runat="server" CssClass="form-control month" placeholder="MMM-YYYY" MaxLength="15" meta:resourcekey="txtFromDateResource1" AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr id="trPlant" runat="server">
                                        <td runat="server">
                                            <b>Plant ID</b>
                                        </td>
                                        <td runat="server">
                                            <asp:DropDownList ID="ddlPlantId" runat="server" OnSelectedIndexChanged="ddlPlantId_SelectedIndexChanged" CssClass="select form-control loadData cssclass" AutoPostBack="True">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr id="trMachine" runat="server">
                                        <td runat="server">
                                            <b>Machine ID</b> </td>
                                        <td runat="server">
                                            <%--<asp:DropDownList ID="ddlMachineId" runat="server" CssClass="  form-control cssclass" AutoPostBack="True">
                                                 <asp:ListItem Value="All">All</asp:ListItem>
                                            </asp:DropDownList>--%>
                                            <asp:ListBox ID="ddlMachineIDs" runat="server" SelectionMode="Multiple" ToolTip="Machine!" ClientIDMode="Static"></asp:ListBox>
                                        </td>
                                    </tr>
                                    <tr id="trFormat" runat="server">
                                        <td runat="server">
                                            <b>Format</b>
                                        </td>
                                        <td runat="server">
                                            <asp:DropDownList ID="ddlFormat" runat="server" OnSelectedIndexChanged="ddlFormat_SelectedIndexChanged" CssClass="select form-control loadData cssclass" AutoPostBack="True">
                                                <asp:ListItem Value="Shift" meta:resourcekey="ListItemResourceType2">Shift</asp:ListItem>
                                                <asp:ListItem Value="Day" meta:resourcekey="ListItemResourceType1">Day</asp:ListItem>
                                                <asp:ListItem Value="Month" meta:resourcekey="ListItemResourceType3">Month</asp:ListItem>
                                                <asp:ListItem Value="TimeConsolidated" meta:resourcekey="ListItemResourceType4">Time Consolidated</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trShift" runat="server">
                                        <td runat="server">
                                            <b>Shift</b>
                                        </td>
                                        <td runat="server">
                                            <asp:DropDownList ID="ddlShift" runat="server" CssClass="select form-control cssclass" AutoPostBack="True" Enabled="false">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trReportType" runat="server">
                                        <td runat="server">
                                            <b>Report Type</b>
                                        </td>
                                        <td runat="server">
                                            <asp:DropDownList ID="ddlReportType" runat="server" CssClass="select form-control cssclass" AutoPostBack="True">
                                                <asp:ListItem Value="Format - I" meta:resourcekey="ListItemResourceFormat1">Format - I</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: center;">
                                            <asp:Button runat="server" ID="btnGenerate" Text="Generate" OnClick="btnGenerate_Click" CssClass="btn btn-primary" meta:resourcekey="btnGenerateResource1" />
                                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-primary" meta:resourcekey="btnCancelResource1" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        const _MS_PER_DAY = 1000 * 60 * 60 * 24;
        $(document).ready(function () {
            $('[id$=ddlMachineIDs]').multiselect({
                includeSelectAllOption: true
            });

            $(".date").datetimepicker({
                format: 'DD-MM-YYYY',
                useCurrent: false,
                locale: 'en-US'
            });
            $(".dateTime").datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss',
                useCurrent: false,
                locale: 'en-US'
            });
            $(".month").datetimepicker({
                format: 'MMM-YYYY',
                useCurrent: false,
                locale: 'en-US'
            });
            //$(".month").datepicker({
            //    viewMode: "months",
            //    minViewMode: "months",
            //    format: 'mmm-yyyy',
            //    todayHighlight: true,
            //    autoclose: true,
            //    language: 'en-US',
            // });
            $("[id$=width]").val($(window).width());
            $("[id$=height]").val($(window).height());
            //$("[id$=btnGenerate").click(function () {
            //    $.blockUI({ message: '<img src="Images/ajax-loader.gif" />' });
            //});
            
        });

        function messageSuccess(msg) {
            debugger;
            Command: toastr["success"](msg)
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        }
        function messageWarning(msg) {
            debugger;
            Command: toastr["error"](msg)
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "2000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        }
        function messageOk() {
            debugger;
            Command: toastr["success"]("Report Generated")
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        }
        function messageNotOk() {
            debugger;
            Command: toastr["error"]("Try Again!")
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "2000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"

            }
        }
        function messageNodata() {
            debugger;
            Command: toastr["error"]("Try Again!No Data Found.")
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "2000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"

            }
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            $("[id$=width]").val($(window).width());
            $("[id$=height]").val($(window).height());

            $('[id$=ddlMachineIDs]').multiselect({
                includeSelectAllOption: true
            });
            $(".date").datetimepicker({
                format: 'DD-MM-YYYY',
                useCurrent: false,
                locale: 'en-US'
            });
            $(".dateTime").datetimepicker({
                format: 'DD-MM-YYYY HH:mm:ss',
                useCurrent: false,
                locale: 'en-US'
            });
            $(".month").datetimepicker({
                format: 'MMM-YYYY',
                useCurrent: false,
                locale: 'en-US'
            });
            //$(".month").datepicker({
            //    viewMode: "months",
            //    minViewMode: "months",
            //    format: 'mmm-yyyy',
            //    todayHighlight: true,
            //    autoclose: true,
            //    language: 'en-US',
            //});
            //$("[id$=btnGenerate").click(function () {
            //    $.blockUI({ message: '<img src="Images/ajax-loader.gif" />' });
            //});
            function messageSuccess(msg) {
                debugger;
                Command: toastr["success"](msg)
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
            }
            function messageWarning(msg) {
                debugger;
                Command: toastr["error"](msg)
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "2000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
            }
            function messageOk() {
                debugger;
                Command: toastr["success"]("Report Generated")
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
            }

            function messageNotOk() {
                debugger;
                Command: toastr["error"]("Try Again!")
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "2000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"

                }
            }
            function messageNodata() {
                debugger;
                Command: toastr["error"]("Try Again!No Data Found.")
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "2000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"

                }
            }


        });
    </script>

</asp:Content>
