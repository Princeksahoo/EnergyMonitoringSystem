<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="EnergyMonitoringSystem.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%--<script src="Scripts/jquery-3.3.1.js"></script>--%>
    <div class="container-fluid">
        <div class="row">
            <div  class="col-lg-6 col-md-6 col-sm-6" style="">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <fieldset class="masterFS" id="divGridSettings">
                            <legend>Grid Column Settings</legend>
                            <asp:GridView runat="server" ID="gvGridColumnSettings" CssClass="gridSetting" AutoGenerateColumns="false" ShowHeader="false" CellSpacing="15">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblGridColumn" Text='<%# Eval("ColumnName") %>' CssClass=" selectlbl" Style="display: inline-block; vertical-align: middle; font-size: 20px; margin-right: 10px;" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtColumnText" CssClass="form-control" Style="text-align: center; background-color: white;" Text='<%# Bind("ColumnText") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <label class="switch">
                                                <asp:CheckBox runat="server" ID="cbVisibility" Checked='<%# Eval("Visibility") %>' meta:resourcekey="cbAutoRefreshResource1" />
                                                <span class="slider round"></span>
                                            </label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <div style="display: flex; justify-content: center; align-content: center; margin-top: 10px;position:absolute;bottom:10px;left:44%;">
                                <asp:Button runat="server" ID="btnUpdateColVisibility" Text="SAVE" OnClick="btnUpdateColVisibility_Click" CssClass="btn btn-primary" />
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <fieldset class="masterFS" id="divTargetSettings">
                            <legend>Target Settings</legend>
                            <fieldset class="masterFS"  >
                                <legend>Search</legend>
                                <div style="display: flex; justify-content: center; align-content: center;">
                                    <asp:Label runat="server" Text="Plant" Style="font-size: 18px; vertical-align: middle; margin-right: 5px; line-height: 30px;" />
                                    <asp:DropDownList runat="server" ID="ddlPlant" CssClass="form-control input-sm" Style="width: 30%; margin-right: 5px;" />
                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" Style="height: 30px;" />
                                </div>
                            </fieldset>
                            <div class="divTargetSetting">
                                <asp:GridView runat="server" ID="gvTargetSettings" CssClass="TargetSetting" AutoGenerateColumns="false" OnRowDataBound="gvTargetSettings_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Machine ID">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblMachineId" Text='<%# Eval("Machineid") %>' CssClass=" selectlbl" Style="display: inline-block; vertical-align: middle; font-size: inherit; margin-right: 10px;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Target (KwH)">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtKwh" CssClass="form-control input-sm" Style="text-align: center; background-color: white;" Text='<%# Bind("Target") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div style="display: flex; justify-content: center; align-content: center; margin-top: 10px;position:absolute;bottom:10px;left:44%;">
                                <asp:Button runat="server" ID="btnUpdateTarget" Text="SAVE" OnClick="btnUpdateTarget_Click" CssClass="btn btn-primary" />
                            </div>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <style>
        fieldset {
            /*border: 1px solid #2B7B78;*/
            padding: 0px;
            border-radius: 4px;
            width: auto;
            border: 2px solid #ece3e3;
            /*box-shadow: 2px 2px 8px 2px #efe7e7;*/
        }

        .masterFS {
            padding: 0 5px 10px 5px;
            position:relative;
            box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
            border-radius:7px;
        }

        legend {
            text-align: center;
            color: white;
            display: block;
            width: auto;
            padding: 0;
            margin-bottom: 5px;
            line-height: inherit;
            border-bottom: transparent;
            color: black;
            font-size: 21px;
            font-weight: bold;
        }


        .gridSetting {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0 15px;
            border: none;
        }

            .gridSetting tr td {
                border: 2px solid #ece3e3;
                border-right: hidden;
                border-left: hidden;
                width: 33.3%;
            }

        .divTargetSetting {
            overflow: auto;
            padding: 2px;
            margin-top: 5px;
            border: 2px solid #ece3e3;

        }

        .TargetSetting {
            width: 100%;
        }

            .TargetSetting tr th {
                position: sticky;
                top: -1px;
                z-index: 1;
                background-color: #2e6886;
                color: white;
                min-width: 150px;
                width: 50%;
                text-align: center;
                background-color: #2E6886 !important;
                height: 50px;
                font-size: 18px;
            }

            .TargetSetting tr td {
                height: 30px;
                width: 50%;
                text-align: center;
                background-color: #BACADE;
                padding: 3px;
                font-weight: bold;
            }

        .switch {
            position: relative;
            display: inline-block;
            vertical-align: middle;
            width: 50px;
            height: 30px;
            float: right;
            margin: 5px;
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
    <script>
        
        function resize() {
            debugger;
            var heights = window.innerHeight;
            var widths = window.innerWidth;
            document.getElementById("divGridSettings").style.height = heights - 80 + "px";
            document.getElementById("divTargetSettings").style.height = heights -80 + "px";
            document.getElementsByClassName("divTargetSetting")[0].style.height = heights -250 + "px";
            //document.getElementById("divHourwiseTable").style.height = heights * 0.8 + "px";
            //document.getElementById("divAllDownsGrid").style.height = heights + "px";
        }
        function messageGridOk() {
            debugger;
            Command: toastr["success"](" Grid Settings Saved.")
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

        function messageGridNotOk() {
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
        function messageTargetOk() {
            debugger;
            Command: toastr["success"](" Machine Target Settings Saved.")
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

        function messageTargetNotOk() {
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
        $(document).ready(function () {
            resize();
            window.onresize = function () {
                resize();
            };
        });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            resize();
            window.onresize = function () {
                resize();
            };
            function messageGridOk() {
                debugger;
                Command: toastr["success"]("Grid Settings Saved.")
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

            function messageGridNotOk() {
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
            function messageTargetOk() {
                debugger;
                Command: toastr["success"](" Machine Target Settings Saved.")
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

            function messageTargetNotOk() {
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
        })
    </script>
</asp:Content>
