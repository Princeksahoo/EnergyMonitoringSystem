<%@ Page Title="LiveData" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LiveData.aspx.cs" Inherits="EnergyMonitoringSystem.LiveData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="">
            <div class="" style="height:auto;border:2px solid #ece3e3;margin:auto;width:auto;display:flex;justify-content:flex-end;box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);border-radius:7px">
                <asp:UpdatePanel runat="server" ID="updFilter">
                    <ContentTemplate>                            
                        <table style="width: auto;vertical-align:middle;margin:5px;">
                            <tr>
                                <td style="width: auto;">
                                    <asp:Label runat="server" Text="Auto Refresh" CssClass=" selectlbl" Style="display: inline-block;vertical-align: middle;font-size: 18px;margin-right: 10px;
                                            line-height: 35px;" />
                                    <label class="switch">
                                        <asp:CheckBox runat="server" ID="cbAutoRefresh" AutoPostBack="True" OnCheckedChanged="cbAutoRefresh_CheckedChanged" meta:resourcekey="cbAutoRefreshResource1" />
                                        <span class="slider round"></span>
                                    </label>
                                </td>
                            </tr>
                        </table>
                        <asp:Timer ID="timerToAutoRefresh" runat="server" Enabled="False" OnTick="timerToAutoRefresh_Tick" ></asp:Timer>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
            <div class="" style="border: 2px solid #ece3e3; margin:10px auto 0 auto;">
                <div id="tblGrid" class="" style="overflow: auto; background-color: transparent; height: 100%;width:100%;">
                    <asp:GridView runat="server" ID="gvLiveData" AutoGenerateColumns="false" CssClass="table table-bordered cockpit headerFixerTable" AllowPaging="false" ShowHeader="true" ShowFooter="false" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Machine ID">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMacID" Text='<%# Eval("Machineid") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DateTime">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDateTime" Text='<%# Eval("LastArrival_TS") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="VLN-R">
                                <ItemTemplate>
                                    <asp:Label ID="lblVLN_R" runat="server" Text='<%# Eval("VLN_R") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="VLN-Y">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblVLN_Y" Text='<%# Eval("VLN_Y") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="VLN-B">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblVLN_B" Text='<%# Eval("VLN_B") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Y-amp.">
                                <ItemTemplate>
                                    <asp:Label ID="lblY_amp" runat="server" Text='<%#Eval("Y_AMP") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="R-amp.">
                                <ItemTemplate>
                                    <asp:Label ID="lblR_amp" runat="server" Text='<%#Eval("R_AMP") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="B-amp.">
                                <ItemTemplate>
                                    <asp:Label ID="lblB_amp" runat="server" Text='<%#Eval("B_AMP") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Power Factor (PF)">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblPowerFactor" Text='<%# Eval("PowerFactor") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Kw">
                                <ItemTemplate>
                                    <asp:Label ID="lblKw" runat="server" Text='<%# Eval("Kw") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="KwH">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblKwH" Text='<%# Eval("Kwh") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Last Arrival Time">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblLAT" Text='<%# Eval("LastArrival_TS") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <EmptyDataTemplate>
                            No records found.
                        </EmptyDataTemplate>
                        <EmptyDataRowStyle BackColor="White" ForeColor="Red" HorizontalAlign="Center" />
                        <HeaderStyle CssClass="HeaderCss" />
                        <RowStyle BackColor="White" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#DCDCDC" ForeColor="Black" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
    <style>
        
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
         .table tbody > tr > th {
            vertical-align: middle;
        }

        .table tbody> tr > td {
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
    <script>
        function resize() {
            debugger;
            var heights = window.innerHeight;
            document.getElementById("tblGrid").style.height = heights - 140 + "px";
        }
        $(document).ready(function () {
            resize();
            window.onresize = function () {
                this.resize();
            };
        });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            resize();
            window.onresize = function () {
                this.resize();
            };
        });
    </script>
</asp:Content>
