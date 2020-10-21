<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="EnergyMonitoringSystem.SignIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <style>
        body {
            background-color: #615f5d;
            width: 100vw;
            height: 100vh;
            font-family: sans-serif;
        }

        .loginContainer {
            width: 380px;
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%,-50%);
            background-color: white;
            padding: 1% 2% 1% 2%;
            border-radius: 8px;
        }

            .loginContainer h2 {
                margin: 10px 0px 10% 0px;
                text-align: center;
                color: #d13400;
            }

            .loginContainer input {
                margin-bottom: 50px;
                padding: 10px 0px;
                width: 100%;
                outline: none;
                border: none;
                /*border-bottom: 2px solid #ed8139;*/
                border-bottom: 2px solid #a1978d;
            }

        #loginBtn {
            width: 100%;
            border: 1px solid #d13400;
            color: #d13400;
            background-color: white;
            font-size: 20px;
            box-shadow: 5px 5px 5px rgba(1,1,1,0.2);
            border-radius: 20px;
            padding: 10px;
            outline:none;
        }

            #loginBtn:hover {
                background-color: #d13400;
                color: white;
                box-shadow: 3px 5px 10px rgba(1,1,1,0.3);
                font-size: 20px;
            }

        .loginContainer div {
            position: relative;
        }

            .loginContainer div label {
                position: absolute;
                top: 10px;
                left: 0px;
                pointer-events: none;
                color: #a1978d;
                transition: .5s;
                font-size: 15px;
                font-family: sans-serif;
                font-weight: 600;
            }

        .loginContainer input:focus ~ label, .loginContainer input:valid ~ label {
            top: -12px;
            left: 0px;
            font-size: 13px;
            color: #d13400;
            font-family: sans-serif;
            font-weight: 600;
        }

        .loginContainer input:focus, .loginContainer input:valid {
            border-bottom: 2px solid #d13400;
        }

        .userImage {
            position: relative;
            left: 42%;
            font-size: 50px;
            background-color: gray;
            border-radius: 30px;
            padding: 7px;
            margin-bottom: 5px;
        }

            .userImage i {
                color: white;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <%--  <div class="container">
            <div class="row text-center">--%>
        <%--  <div class="col-lg-12 col-sm-12 col-md-12">--%>
        <div class="loginContainer">
           <%-- <span class="userImage">
                <i class="glyphicon glyphicon-user"></i>
            </span>--%>
            <h2>Sign in</h2>
            <div>
                <input type="text" id="txtUsername" runat="server" required="required" />
                <label>Username</label>
            </div>
            <div>
                <input type="password" id="txtPassword" runat="server" required="required" />
                <label>Password</label>
            </div>
            <button type="button" id="loginBtn"  runat="server" onserverclick="loginBtn_ServerClick" >Login</button>
            <br />  <br />
            <div style="text-align:center">
            <span id="errorMsg" runat="server" visible="false" style="color:#bf0808;font-weight:500;">Invalid username or password.</span>
                </div>
        </div>
        <%--  </div>
            </div>
        </div>--%>
    </form>
    <script>
        function button_click(objTextBox, objBtnID) {
            if (window.event.keyCode == 13) {
                document.getElementById(objBtnID).focus();
                document.getElementById(objBtnID).click();
            }
        }
    </script>
</body>
</html>
