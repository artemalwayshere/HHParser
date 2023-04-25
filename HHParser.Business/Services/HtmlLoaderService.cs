using HHParser.Business.Models;
using HtmlAgilityPack;

namespace HHParser.Business.Services
{
    public class HtmlLoaderService
    {
        public async void HtmlLoadAndParse()
        {
            List<JobMessage> strings = new List<JobMessage>();
            var url = "https://ekaterinburg.hh.ru/search/vacancy?no_magic=true&L_save_area=true&text=.net&excluded_text=&salary=&currency_code=RUR&experience=between1And3&schedule=remote&order_by=relevance&search_period=1&items_on_page=50";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            List<HtmlNode> myParagraphs = doc.DocumentNode.SelectNodes("//a[@class='serp-item__title']").ToList();

            foreach (var node in myParagraphs) 
            { 
                strings.Add(new JobMessage()
                {
                    Title = node.InnerText,
                    JobUrl = node.Attributes["href"].Value,
                });
            }

            Console.WriteLine("Complete request");
        }

    }
}
