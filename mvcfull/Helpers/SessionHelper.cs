using System;
using System.Web;

namespace mvc_full.Helpers
{
    public static class SessionHelper
    {
        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current.Session["MusteriId"] != null;
        }

        public static int? GetCurrentUserId()
        {
            if (HttpContext.Current.Session["MusteriId"] != null)
            {
                return (int)HttpContext.Current.Session["MusteriId"];
            }
            return null;
        }

        public static string GetCurrentUserName()
        {
            if (HttpContext.Current.Session["MusteriAd"] != null)
            {
                return HttpContext.Current.Session["MusteriAd"].ToString();
            }
            return "Misafir";
        }

        public static void SetUserSession(int musteriId, string musteriAd)
        {
            HttpContext.Current.Session["MusteriId"] = musteriId;
            HttpContext.Current.Session["MusteriAd"] = musteriAd;
        }

        public static void ClearUserSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
