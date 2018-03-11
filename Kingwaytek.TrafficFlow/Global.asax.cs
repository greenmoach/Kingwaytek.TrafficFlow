using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;

namespace Kingwaytek.TrafficFlow
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            MvcHandler.DisableMvcResponseHeader = true;
        }

        #region   ELMAH_Error_Mail 

        void ErrorLog_Filtering(object sender, Elmah.ExceptionFilterEventArgs e)
        {
            FilterError404(e);
            var exception = e.Exception.GetBaseException();

            if (exception is HttpRequestValidationException)
            {
                e.Dismiss();
            }

        }

        void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var exception = e.Exception.GetBaseException();

            FilterError404(e);

            if (exception is System.IO.FileNotFoundException || exception is HttpRequestValidationException || exception is HttpException)
            {
                e.Dismiss();
            }
        }

        void ErrorMail_Mailing(object sender, Elmah.ErrorMailEventArgs e)
        {
            var exception = e.Error.Exception;

            if (exception is NotImplementedException)
            {
                e.Mail.Priority = System.Net.Mail.MailPriority.High;
                e.Mail.Subject = "嚴重錯誤";
                e.Mail.CC.Add(System.Configuration.ConfigurationManager.AppSettings["elmahMail"]);
            }
        }

        private void FilterError404(ExceptionFilterEventArgs e)
        {
            if (e.Exception.GetBaseException() is HttpException)
            {
                HttpException ex = (HttpException)e.Exception.GetBaseException();

                if (ex.GetHttpCode() == 404)
                {
                    e.Dismiss();
                }
            }
        }

        #endregion

    }
}