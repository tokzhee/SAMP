using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Utilities;
using App.Database.Access;
using App.DataModels.Models;

namespace App.Core.Services
{
    public static class MenuService
    {
        //public static menuicon GetMenuIconWithId(string menuIconId, string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    menuicon menuicon = null;

        //    try
        //    {
        //        using (var db = new Data())
        //        {
        //            menuicon = db.Get<menuicon>(menuIconId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return menuicon;
        //}
        //public static List<mainmenu> GetActiveMainMenuWithRoleId(string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    List<mainmenu> list = null;

        //    try
        //    {
        //        using (var db = new Data())
        //        {
        //            list = (List<mainmenu>)db.QueryList<mainmenu>("select distinct a.id, a.display_name, a.menu_icon_id, a.active_flag, a.arrangement_order from main_menu A, sub_menu B, role_menu C where A.id = B.main_menu_id and B.id = C.menu_sub_id and a.active_flag = 'True' and c.role_id = @RoleId order by a.arrangement_order", new { RoleId = roleId });
        //            if (list != null)
        //            {
        //                foreach (var mainMenu in list)
        //                {
        //                    mainMenu.menuicon = db.Get<menuicon>(mainMenu.menu_icon_id);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return list;
        //}
        //public static List<submenu> GetActiveSubMenuWithMainMenuIdAndRoleId(string mainMenuId, string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    List<submenu> list = null;

        //    try
        //    {
        //        using (var db = new Data())
        //        {
        //            list = (List<submenu>)db.QueryList<submenu>("select A.* from sub_menu A, role_menu B where A.id = B.menu_sub_id and A.main_menu_id = @main_menu_id and B.role_id = @role_id and A.active_flag = 'True' and A.display_flag = 'True' order by A.arrangement_order", new { main_menu_id = mainMenuId, role_id = roleId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return list;
        //}
        //public static List<submenu> GetActiveSubMenuWithRoleId(string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    List<submenu> list = null;

        //    try
        //    {
        //        using (var db = new Data())
        //        {
        //            list = (List<submenu>)db.QueryList<submenu>("select A.* from sub_menu A, role_menu B where A.id = B.menu_sub_id and B.role_id = @role_id order by display_name", new { role_id = roleId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return list;
        //}
        //public static List<submenu> GetActiveSubMenu(string callerFormName, string callerFormMethod, string callerIpAddress)
        //{
        //    List<submenu> list = null;

        //    try
        //    {
        //        using (var db = new Data())
        //        {
        //            list = (List<submenu>)db.QueryList<submenu>("select distinct * from sub_menu where active_flag = 'True' order by display_name");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
        //    }

        //    return list;
        //}
        
        public static List<menuicon> GetMenuIcons(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<menuicon> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<menuicon>)db.GetList<menuicon>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<mainmenu> GetMainMenus(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<mainmenu> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<mainmenu>)db.GetList<mainmenu>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<submenu> GetSubMenus(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<submenu> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<submenu>)db.GetList<submenu>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<rolemenu> GetRoleMenus(string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<rolemenu> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<rolemenu>)db.GetList<rolemenu>();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static List<rolemenu> GetRoleMenusWithRoleId(string roleId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            List<rolemenu> list = null;

            try
            {
                using (var db = new Data())
                {
                    list = (List<rolemenu>)db.GetList<rolemenu>("where role_id = @role_id", new { role_id = roleId });
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return list;
        }
        public static bool IsMenuAssigned(string roleId, string subMenuId, string callerFormName, string callerFormMethod, string callerIpAddress)
        {
            bool result = false;

            try
            {
                using (var db = new Data())
                {
                    var submenus = db.QueryList<submenu>("select A.* from sub_menu A, role_menu B where A.id = B.menu_sub_id and B.menu_sub_id = @menu_sub_id and B.role_id = @role_id", new { menu_sub_id = subMenuId, role_id = roleId });
                    if (submenus != null && submenus.Count() > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }
        public static long AssignMenu(string username, string roleId, string subMenuId, bool isSelected, string callerFormName, string callerFormMethod, string callerIpAddress, string callerMacAddress)
        {
            long result = 0;

            try
            {
                using (var db = new Data())
                {
                    var rolemenu = db.Query<rolemenu>("select * from role_menu where role_id = @role_id and menu_sub_id = @menu_sub_id", new { role_id = roleId, menu_sub_id = subMenuId });
                    if (isSelected)
                    {
                        //it not exists, add
                        if (rolemenu == null)
                        {
                            rolemenu = new rolemenu
                            {
                                role_id = Convert.ToInt64(roleId),
                                menu_sub_id = Convert.ToInt64(subMenuId),
                                created_on = DateTime.UtcNow.AddHours(1),
                                created_by = username
                            };

                            result = db.Insert(rolemenu);
                            rolemenu.id = result;
                        }
                    }
                    else
                    {
                        //if exist, remove
                        if (rolemenu != null)
                        {
                            db.Delete(rolemenu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogError(callerFormName, callerFormMethod, callerIpAddress, ex);
            }

            return result;
        }

    }
}
