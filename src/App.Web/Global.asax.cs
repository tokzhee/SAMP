using App.Core.BusinessLogic;
using App.Core.Services;
using App.DataModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using App.Web.Models;
using App.Core.Utilities;

namespace App.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CacheData.Menuicons = MenuService.GetMenuIcons("Global", "Application_Start", "");
            CacheData.Mainmenus = MenuService.GetMainMenus("Global", "Application_Start", "");
            CacheData.Submenus = MenuService.GetSubMenus("Global", "Application_Start", "");
            CacheData.Rolemenus = MenuService.GetRoleMenus("Global", "Application_Start", "");
            CacheData.Useraccountauthenticationtypes = UserAccountAuthenticationTypeService.GetAll("Global", "Application_Start", "");
            CacheData.Useraccountstatus = UserAccountStatusService.GetAll("Global", "Application_Start", "");
            CacheData.Persontypes = PersonTypeService.GetAll("Global", "Application_Start", "");
            CacheData.Onboardingdocuments = OnboardingDocumentsService.GetAll("Global", "Application_Start", "");
            
            MvcHandler.DisableMvcResponseHeader = true;//this line is to hide mvc header
        }       
        void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {

            const string callerFormMethod = "Application_PostAuthenticateRequest";

            try
            {
                var authenticationCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authenticationCookie != null)
                {
                    var audituseraccessactivity = UserAccessActivityService.GetWithKey(authenticationCookie.Value, "", callerFormMethod, "");
                    if (audituseraccessactivity != null && audituseraccessactivity.key_expiration_date > DateTime.UtcNow.AddHours(1))
                    {
                        var authenticationTicket = FormsAuthentication.Decrypt(authenticationCookie.Value);
                        if (authenticationTicket != null && !authenticationTicket.Expired)
                        {

                            //
                            var user = JsonConvert.DeserializeObject<user>(authenticationTicket.UserData);
                            if (user == null)
                            {
                                return;
                            }

                            #region Added to Take Care of Incessant 15 Minutes Timeout

                            //
                            authenticationCookie.Expires = DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes")));
                            audituseraccessactivity.key_expiration_date = DateTime.UtcNow.AddHours(1).AddMinutes(Convert.ToDouble(ConfigurationUtility.GetAppSettingValue("SessionTimeOutInMinutes")));

                            //
                            HttpContext.Current.Response.Cookies.Set(authenticationCookie);
                            UserAccessActivityService.Update(audituseraccessactivity, "", "Application_PostAuthenticateRequest", "");

                            #endregion

                            var roles = RoleService.GetWithRoleId(Convert.ToString(user.role_id), "", callerFormMethod, "")?.role_name.Split('|');
                            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authenticationTicket), roles);

                            //
                            user = UsersService.GetWithUsername(audituseraccessactivity.username, "", callerFormMethod, "");
                            if (user != null && !user.status_id.Equals((int)UserAccountStatus.LoggedIn))
                            {
                                FormsAuthentication.SignOut();
                            }

                        }
                        else
                        {
                            //
                            var user = UsersService.GetWithUsername(audituseraccessactivity.username, "", callerFormMethod, "");
                            if (user != null)
                            {
                                user.status_id = (int)UserAccountStatus.Active;
                                UsersService.Update(user, "", callerFormMethod, "");
                            }
                            FormsAuthentication.SignOut();
                        }
                    }
                    else
                    {
                        //
                        var audituseraccessactivity_ = UserAccessActivityService.GetWithKey(authenticationCookie.Value, "", callerFormMethod, "");
                        if (audituseraccessactivity_ != null)
                        {
                            var user = UsersService.GetWithUsername(audituseraccessactivity_.username, "", callerFormMethod, "");
                            if (user != null)
                            {
                                user.status_id = (int)UserAccountStatus.Active;
                                UsersService.Update(user, "", callerFormMethod, "");
                            }
                        }

                        FormsAuthentication.SignOut();
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        void Session_Start(Object sender, EventArgs e)
        {
            try
            {
                //Application.Lock();
                //StaticDataModel.ActiveUsersCount += 1;
                //Application.UnLock();
            }
            catch (Exception)
            {
                throw;
            }
        }
        void Session_End(Object sender, EventArgs e)
        {
            try
            {
                //Application.Lock();
                //StaticDataModel.ActiveUsersCount -= 1;
                //Application.UnLock();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        void Application_Error(Object sender, EventArgs e)
        {
            //I want to send a mail 
            var exception = Server.GetLastError().GetBaseException();
            string err = "Error in: " + Request.Url.ToString() + ". Error Message:" + exception.Message.ToString();
        }
    }
}