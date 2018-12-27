using System;
using System.Linq;
using System.Web;
using ChineseAbs.Logic;
using System.Data;
using System.Web.Security;

namespace ChineseAbs.Menu
{
    public partial class CnabsMenu : System.Web.UI.UserControl
    {
        protected int isAccept = 0;
        protected string m_cnabsHost = string.Empty;
        protected string m_absmanagementHost = string.Empty;
        protected string m_deallabHost = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isLogin = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!Request.IsLocal)
            {
                m_cnabsHost = "https://cn-abs.com";
                m_absmanagementHost = "https://absmanager.cn-abs.com";
                m_deallabHost = "https://deallab.cn-abs.com";
            }

            this.hidLoginStats.Text = isLogin ? "1" : "0";
            
            if (!IsPostBack)
            {
                if (isLogin)
                {
                    string userName = System.Web.HttpContext.Current.User.Identity.Name;
                    string sessionName = string.Format("{0}_{1}", "agreement", userName);

                    if (Session[sessionName] != null && Session[sessionName].ToString() == "signed")
                    {
                        isAccept = 1;
                    }
                    else
                    {
                        var service = new WebAgreementService();
                        DataTable agreement = service.GetLatestWebAgreement();
                        if (agreement.Rows.Count == 0)
                        {
                            isAccept = 1;
                            Session[sessionName] = "signed";
                        }
                        else
                        {
                            double latestVersion = Convert.ToDouble(agreement.Rows[0]["version_id"]);
                            if (service.GetWebAgreementSignedStat(userName, latestVersion) == 1)
                            {
                                isAccept = 1;
                                Session[sessionName] = "signed";
                            }
                            else
                            {
                                this.agreementTitle.Text = agreement.AsEnumerable().FirstOrDefault().Field<string>("title");
                                this.protocol.Text = agreement.AsEnumerable().FirstOrDefault().Field<string>("agreement").Replace("<br />", "");
                            }
                        }
                    }
                    //organization.Visible = Roles.IsUserInRole("admin") || Roles.IsUserInRole("CompanyUser");   
                    /////// 需要修改，ulOrganization 已经改变
                    //if (Roles.IsUserInRole("admin") || Roles.IsUserInRole("CompanyUser") || Roles.IsUserInRole("Ticket"))
                    //    ulOrganization.Style.Value = "right:5%";

                    //organization.Visible = Roles.IsUserInRole("OrganizationUser");
                    this.adminRole.Visible = Roles.IsUserInRole("admin");
                    this.companyUser.Visible = Roles.IsUserInRole("CompanyUser");
                    this.liABSManagerConfig.Visible = Roles.IsUserInRole("CompanyUser");
                    //this.divTicketHandler.Visible = Roles.IsUserInRole("Ticket");

                    this.liReceive.Visible = Roles.IsUserInRole("Ticket");
                    this.liSelf.Visible = Roles.IsUserInRole("Ticket");
                    lbLogin.Text = new AdminService().GetUserInfo("username", userName).AsEnumerable().FirstOrDefault().Field<string>("name");
                    lbLogin.Attributes.Add("title", lbLogin.Text);
                    liLogin.Visible = false;

                    var avatarUrl = new UserService().GetAvatar(Context.User.Identity.Name);
                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        avatarUrl = "../Images/avatar/headerDefault.jpg";
                    }
                    else
                    {
                        if (avatarUrl.Contains("/Images/avatar"))
                        {
                            avatarUrl = ".." + avatarUrl;
                        }
                    }
                    avatar.ImageUrl = avatarUrl;
                    avatar.AlternateText = Context.User.Identity.Name;
                    personal.Visible = true;
                }
                else
                {
                    isAccept = 1;
                    liLogin.Visible = true;
                    personal.Visible = false;
                }
            }
            else
            {
                isAccept = 1;
            }
        }
    }
}