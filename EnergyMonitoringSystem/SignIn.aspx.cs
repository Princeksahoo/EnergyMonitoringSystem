using EnergyMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class SignIn : System.Web.UI.Page
    {
        UserDetails userDetails = new UserDetails();
        protected void Page_Load(object sender, EventArgs e)
        {
            SetDefaultButton(txtPassword, loginBtn);
        }

        private void SetDefaultButton(HtmlInputPassword txtPassword, HtmlButton loginBtn)
        {
            this.txtPassword.Attributes.Add("onkeypress", "button_click(this,'" + this.loginBtn.ClientID + "')");
        }

        protected void loginBtn_ServerClick(object sender, EventArgs e)
        {
            if (ValidateUser())
            {
                if (txtPassword.Value.Equals(userDetails.Password, StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    errorMsg.Visible = true;
                }
            }
            else
            {
                errorMsg.InnerText = "You are not an Admin";
                errorMsg.Visible = true;
            }

        }

        private bool ValidateUser()
        {
            userDetails = DataBaseAccess.GetEmployeeDetails(txtUsername.Value);
            return userDetails.IsAdmin;
        }
    }
    public class UserDetails
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

    }
}