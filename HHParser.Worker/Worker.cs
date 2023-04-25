using HHParser.Business.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HHParser.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string apiKey = "";
        private readonly ChatId chatId = -1001950048988;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var botClient = new TelegramBotClient(apiKey);
            var me = await botClient.GetMeAsync();

            List<JobMessage> jobs = new List<JobMessage>();

            while (!stoppingToken.IsCancellationRequested)
            {

                //var url = "https://ekaterinburg.hh.ru/search/vacancy?no_magic=true&L_save_area=true&text=.net&excluded_text=&salary=&currency_code=RUR&experience=between1And3&schedule=remote&order_by=relevance&search_period=1&items_on_page=20";
                var url = "https://ekaterinburg.hh.ru/search/vacancy?enable_snippets=true&experience=between1And3&no_magic=true&ored_clusters=true&schedule=remote&text=.net&search_period=0";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                List<HtmlNode> myParagraphs = doc.DocumentNode.SelectNodes("//a[@class='serp-item__title']").ToList();

                List<JobMessage> newJobs = new List<JobMessage>();

                foreach (var node in myParagraphs)
                {
                    //string url1 = node.Attributes["href"].Value;
                    //string id = url1.Split('/')[4].Split('?')[0];вфвфывф

                    newJobs.Add(new JobMessage()
                    {
                        Title = node.InnerText,
                        JobUrl = node.Attributes["href"].Value
                        //UniqId = id
                    });
                }


                if (jobs.Count == 0)
                {
                    await botClient.SendTextMessageAsync(chatId, $"{newJobs[0].Title} \n {newJobs[0].JobUrl} \n Вакансия при перезапуске.");
                    jobs = newJobs;
                    _logger.LogInformation("Первый запрос: {time}", DateTimeOffset.Now);
                }

                else if (jobs.Count > 0 && jobs.SequenceEqual(newJobs))
                { 
                    var diffArr = newJobs.Except(jobs);

                    foreach (var item in diffArr)
                    {
                        await botClient.SendTextMessageAsync(chatId, $"Новая вакансия!!!! \n {item.Title} \n {item.JobUrl}");
                    }
                    jobs = newJobs;

                    _logger.LogInformation("Появилась новая вакансия: {time}", DateTimeOffset.Now);
                }
                else _logger.LogInformation("Изменений нет: {time}", DateTimeOffset.Now);
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}