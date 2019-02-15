using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataLibrary.DataAccess
{
	public class HttpWebRequestToCrm
	{
		private const string AuthServiceUri = @"/ServiceModel/AuthService.svc/Login";
		public static CookieContainer AuthCookie = new CookieContainer();

		// Вспомогательный класс для десериализации JSON-объекта из HTTP-ответа.
		public class ResponseStatus
		{
			public int Code { get; set; }
			public string Message { get; set; }
			public object Exception { get; set; }
			public object PasswordChangeUrl { get; set; }
			public object RedirectUrl { get; set; }
			public string Bpmcsrf { get; set; }
			public CookieContainer AuthCookie { get; set; }
		}

		public static ResponseStatus BpmAuthentificationResponse(string baseUri, string userName, string userPassword)
		{
			if (WebRequest.Create(baseUri + AuthServiceUri) is HttpWebRequest authRequest)
			{
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

				using (var response = (HttpWebResponse)authRequest.GetResponse())
				{
					using (var reader =
						new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
					{
						// Десериализация HTTP-ответа во вспомогательный объект.
						string responseText = reader.ReadToEnd();
						var status = new JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
						status.AuthCookie = AuthCookie;
						if (response.Cookies["BPMCSRF"] != null)
						{
							status.Bpmcsrf = response.Cookies["BPMCSRF"].Value;
						}

						return status;
					}
				}
			}
			else
				return null;
		}
	}
}
