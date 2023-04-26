using HHParser.Business.Models;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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
            var tempMessages = new List<JobMessage>();

            while (!stoppingToken.IsCancellationRequested)
            {

                var url = "https://ekaterinburg.hh.ru/search/vacancy?no_magic=true&L_save_area=true&text=.Net&excluded_text=%D0%BC%D0%B5%D0%BD%D0%B5%D0%B4%D0%B6%D0%B5%D1%80%2C+%D0%B0%D0%B4%D0%BC%D0%B8%D0%BD%D0%B8%D1%81%D1%82%D1%80%D0%B0%D1%82%D0%BE%D1%80%2C+%D1%8E%D1%80%D0%B8%D1%81%D1%82&salary=&currency_code=RUR&experience=doesNotMatter&schedule=remote&order_by=publication_time&search_period=1&items_on_page=20";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var aTags = doc.DocumentNode.SelectNodes("//a[@class='serp-item__title']");


                List<string> vacacyHrefs = new List<string>();

                var messages = new List<JobMessage>();

                foreach (var page in aTags)
                {
                    var href = page.Attributes["href"].Value;
                    doc = web.Load(href);
                    JobMessage message = new JobMessage();

                    foreach (var item in doc.DocumentNode.SelectNodes("//p[@class='vacancy-creation-time-redesigned']/span"))
                    {
                        message.PublishDate = DateTime.Parse(item.InnerText);
                    }
                    message.Title = page.InnerText;
                    message.JobUrl = href;
                    messages.Add(message);
                }

                if (tempMessages.Count == 0)
                {
                    await botClient.SendTextMessageAsync(chatId, $"Самая новая на данный моент вакансия \n {messages[0].Title} \n Дата публикации - {messages[0].PublishDate} \n {messages[0].JobUrl} \n");
                    tempMessages = messages;
                    _logger.LogInformation("Первый запрос: {time}", DateTimeOffset.Now);
                }

                else if (!messages.SequenceEqual(tempMessages))
                {
                    foreach (var item in messages)
                    {
                        if (!tempMessages.Contains(item))
                        {
                            await Console.Out.WriteLineAsync(item.Title);
                            await botClient.SendTextMessageAsync(chatId, $"Новая вакансия!!!! \n {item.Title} \n Дата публикации - {item.PublishDate}\n {item.JobUrl}");
                        }
                    }
                    tempMessages = messages;

                    _logger.LogInformation("Появилась новая вакансия: {time}", DateTimeOffset.Now);
                }
                else _logger.LogInformation("Изменений нет: {time}", DateTimeOffset.Now);
                await Task.Delay(3600000, stoppingToken);
            }
        }
    }
}