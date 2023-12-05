using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;


// ИСПОЛЬЗОВАЛ https://panel.whapi.cloud/
class WhatsAppBot
{
	private static readonly string accessToken = "EH9v9ztckuUjS7dEzucaC2MMFCqsS3CP"; // Ваш Access Token
	private static readonly HttpClient client = new HttpClient();
	private static string token = "";
	static async Task Main(string[] args)
	{
		Console.WriteLine("Запуск WhatsApp бота...");
		GetLastMessage();
		string instanceId = "qwe";
		// Создание нового Instance
		//else instanceId = await CreateInstance();

		// Отправка текстового сообщения
		//var messageResponse = await SendMessage("9041386602", "это чмо реально не работало из-за 8, а не 7 в номере)"); // Замените на ваш Instance ID и номер телефона
		//Console.WriteLine($"Ответ сервера при отправке сообщения: {messageResponse}");

		Console.WriteLine("Нажмите любую клавишу для выхода...");
		Console.ReadKey();
	}
	
	private static async void GetLastMessage()
	{
		string url = "https://webhook.site/ea9596bd-21af-4ff0-8c98-f84c0034861a";

		using (HttpClient client = new HttpClient())
		{
			try
			{
				
				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();
				Console.WriteLine(responseBody);
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
	}
	private static async Task<string> SendMessage(string phoneNumber, string message)
	{
		var options = new RestClientOptions($"https://gate.whapi.cloud/messages/text?token={accessToken}");
		var client = new RestClient(options);
		var request = new RestRequest("");
		request.AddHeader("accept", "application/json");
		request.AddJsonBody("{"+$"\"typing_time\":0,\"to\":\"7{phoneNumber}\",\"body\":\"{message}\""+"}", false);
		var response = await client.PostAsync(request);

		return response.Content;
	}
	private static async Task<string> GetMessage()
	{
		List<string> list = new List<string>();
		var options = new RestClientOptions($"https://gate.whapi.cloud/messages/list?token={accessToken}");
		var client = new RestClient(options);
		var request = new RestRequest("");
		request.AddHeader("accept", "application/json");
		var response = await client.GetAsync(request);
		var jsonParsed = JObject.Parse(response.Content);

		// Словарь для хранения последнего сообщения от каждого пользователя
		var lastMessages = new Dictionary<string, string>();

		foreach (var message in jsonParsed["messages"])
		{
			string body = message["text"]?["body"]?.ToString();
			string from = message["chat_id"]?.ToString();
			string fromMe = message["from_me"]?.ToString();

			if (fromMe != "True" && !string.IsNullOrEmpty(from))
			{
				if (!lastMessages.ContainsKey(from))
				{
					lastMessages[from] = body;
				}
			}
		}


		// Вывод последнего сообщение каждого пользователя
		foreach (var item in lastMessages)
		{
			Console.WriteLine($"Отправитель: {item.Key}, Последнее сообщение: {item.Value}");
		}
		return null;
	}

	public class CreateInstanceResponse
	{
		public string Status { get; set; }
		public string instance_id { get; set; }
	}
}