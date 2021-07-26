using System.IO;
using System.Net;
using System.Text;
using AngleSharp.Html.Parser;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoRuCalendar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        private HtmlWebViewSource GetSourseHtml()
        {
            string html = @"https://www.roller.ru/forum/pokatushki.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(html);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251));
            string silehtml = sr.ReadToEnd();
            sr.Close();

            //Parse the stream
            var parser = new HtmlParser();
            var document = parser.ParseDocument(silehtml);

            string newhtml = "<html>\n" +
                "<style>\n" +
                "        body{font-family:Arial,Sans-serif;font-size:12px;color:#666666;}\n" +
                "        h1{font-size:20px;color:#E87400;text-align: center;}\n" +
                "        .month{font-size:14px;font-weight:bold;color:#666666;padding-top:5px;padding-bottom:20px;text-align: center;}\n" +
                "        .day{font-size:12px;font-weight:bold;color:#666666;}\n" +
                "        .holiday{font-size:12px;font-weight:bold;color:#E87400;}\n" +
                "        .event a, .event a:visited{color:#6c7f03;}\n" +
                "    </style>\n" +
                "<body><h1>Календарь покатушек</h1>\n";
            newhtml += "<table border=\"0\" cellpadding=\"5\" cellspacing=\"0\"><tbody>\n";

            foreach (var element in document.QuerySelectorAll("div[class='content']"))
            {
                var subdocument = parser.ParseDocument(element.InnerHtml);
                bool isadd = true;
                foreach (var subelement in subdocument.QuerySelectorAll("tr"))
                {
                    if (subelement.InnerHtml.ToUpper().Contains("ДАЛЕКОЕ БУДУЩЕЕ")
                        || subelement.InnerHtml.ToUpper().Contains("ПРОШЕДШИЕ ПОКАТУШКИ")
                        || subelement.InnerHtml.ToUpper().Contains("ДОБАВИТЬ ПОКАТУШКУ"))
                        isadd = false;

                    if (isadd)
                    {
                        string curhtml = subelement.InnerHtml.Replace("/forum/viewtopic.php?t=", "https://www.roller.ru/forum/viewtopic.php?t=");

                        curhtml = curhtml.Replace("/ понедельник</div>", "/ Пн</div>");
                        curhtml = curhtml.Replace("/ вторник</div>", "/ Вт</div>");
                        curhtml = curhtml.Replace("/ среда</div>", "/ Ср</div>");
                        curhtml = curhtml.Replace("/ четверг</div>", "/ Чт</div>");
                        curhtml = curhtml.Replace("/ пятница</div>", "/ Пт</div>");
                        curhtml = curhtml.Replace("/ суббота</div>", "/ Сб</div>");
                        curhtml = curhtml.Replace("/ воскресенье</div>", "/ Вс</div>");

                        newhtml += $"<tr valign=\"top\">{curhtml}</tr>\n";
                    }
                }

            }
            newhtml += "</tbody></table></body></html>";
            var localhtml = new HtmlWebViewSource();
            localhtml.Html = newhtml;

            return localhtml;
        }

        public CalendarPage()
        {
            webView = new WebView
            {
                Source = GetSourseHtml(),
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            ToolbarItems.Add(new ToolbarItem("Refresh", "", () => { webView.Source = GetSourseHtml(); }));
            ToolbarItems.Add(new ToolbarItem("Back", "", () => { webView.GoBack(); }));

            //webView.Navigating += WebView_Navigating;

            this.Content = new StackLayout { Children = { webView } };
        }

        //private void WebView_Navigating(object sender, WebNavigatingEventArgs e) { e.Cancel = true; }
    }
}