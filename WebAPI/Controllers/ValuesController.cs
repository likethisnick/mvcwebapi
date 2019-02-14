using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace WebAPI.Controllers
{
	// Вспомогательный класс для десериализации JSON-объекта из HTTP-ответа.
	class ResponseStatus
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public object Exception { get; set; }
		public object PasswordChangeUrl { get; set; }
		public object RedirectUrl { get; set; }
	}
	class ResponcePerformances
	{
		public string GetNumberOfPerformancesResult { get; set; }
	}

	public class ValuesController : ApiController
	{
		// Строка адреса BPMonline сервиса OData.
		private const string baseUri = "http://10.11.12.5:8093";
		private const string authServiceUri = baseUri + @"/ServiceModel/AuthService.svc/Login";
		private const string newUri = "http://10.11.12.5:8093/0/rest/CgrGetNumberOfPerformancesService/GetNumberOfPerformances";
		public static CookieContainer AuthCookie = new CookieContainer();
		public static string Bpmcsrf;

		public static void test(string userName, string userPassword)
		{
			var authRequest = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
			authRequest.Method = "POST";
			// Определение типа контента запроса.
			authRequest.ContentType = "application/json";
			// Включение использования cookie в запросе.
			authRequest.CookieContainer = AuthCookie;
			// Помещение в тело запроса учетной информации пользователя.
			using (var requestStream = authRequest.GetRequestStream())
			{
				using (var writer = new StreamWriter(requestStream))
				{
					writer.Write(@"{
                    ""UserName"":""" + userName + @""",
                    ""UserPassword"":""" + userPassword + @"""
                    }");
				}
			}
			
			// Вспомогательный объект, в который будут десериализованы данные HTTP-ответа.
			ResponseStatus status = null;
			// Получение ответа от сервера. Если аутентификация проходит успешно, в свойство AuthCookie будут
			// помещены cookie, которые могут быть использованы для последующих запросов.

			
			using (var response = (HttpWebResponse)authRequest.GetResponse())
			{
				
				if (response.Cookies["BPMCSRF"] != null)
				{
					Bpmcsrf = response.Cookies["BPMCSRF"].Value;
				}

				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					// Десериализация HTTP-ответа во вспомогательный объект.
					string responseText = reader.ReadToEnd();
					status = new JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);

				}

			}

			// Проверка статуса аутентификации.
			if (status != null)
			{
				// Успешная аутентификация.
				if (status.Code == 0)
				{

					var dataRequest = HttpWebRequest.Create(newUri) as HttpWebRequest;
					dataRequest.Method = "POST";
					// Определение типа контента запроса.
					dataRequest.ContentType = "application/json";
					// Включение использования cookie в запросе.
					dataRequest.CookieContainer = AuthCookie;
					dataRequest.Headers["BPMCSRF"] = Bpmcsrf;
					// Помещение в тело запроса учетной информации пользователя.
					using (var requestStream = dataRequest.GetRequestStream())
					{
						using (var writer = new StreamWriter(requestStream))
						{
							writer.Write(@"{
		                    ""programCode"":""" + "007" + @"""
		                    }");
						}
					}

					// Вспомогательный объект, в который будут десериализованы данные HTTP-ответа.
					status = null;
					// Получение ответа от сервера. Если аутентификация проходит успешно, в свойство AuthCookie будут
					// помещены cookie, которые могут быть использованы для последующих запросов.
					using (var response = (HttpWebResponse)dataRequest.GetResponse())
					{
						using (var reader = new StreamReader(response.GetResponseStream()))
						{
							// Десериализация HTTP-ответа во вспомогательный объект.
							string responseText = reader.ReadToEnd();
							var status2 = new JavaScriptSerializer().Deserialize<ResponcePerformances>(responseText);
						}

					}

					return;
				}
			}
			return;
		}

		// GET api/values
			public IEnumerable<string> Get()
		{
			test("Supervisor", "Supervisor");
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}
