using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using DataLibrary.DataAccess;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
	[DataContract]
	public class ContactList
	{
		[DataMember (Name = "GetAllContactsInformationResult")] public List<Contact> ContactListContainer { get; set; }
	}
	[DataContract]
	public class Contact
	{
		[DataMember] public Guid Id { get; set; }
		[DataMember] public string Name { get; set; }
	}

	public class ValuesController : ApiController
	{
		// Строка адреса BPMonline сервиса OData.
		private const string BaseUri = "http://10.11.12.5:8093";
		private const string NewUri = "http://10.11.12.5:8093/0/rest/CgrGetNumberOfPerformancesService/GetNumberOfPerformances";

		private const string getUri = "http://10.11.12.5:8093/0/rest/CgrGetContacts/GetAllContactsInformation";

		public static void Test(string BaseUri, string userName, string userPassword)
		{
			HttpWebRequestToCrm newRequest = new HttpWebRequestToCrm();
			var status = HttpWebRequestToCrm.BpmAuthentificationResponse(BaseUri, "Supervisor", "Supervisor");

			// Проверка статуса аутентификации.
			if (status != null)
			{
				if (status.Code == 0)
				{
					if (WebRequest.Create(NewUri) is HttpWebRequest dataRequest)
					{
						dataRequest.Method = "GET";
						// Определение типа контента запроса.
						dataRequest.ContentType = "application/json";
						// Включение использования cookie в запросе.
						dataRequest.CookieContainer = status.AuthCookie;
						dataRequest.Headers["BPMCSRF"] = status.Bpmcsrf;
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

						using (var response = (HttpWebResponse) dataRequest.GetResponse())
						{
							using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
							{
								// Десериализация HTTP-ответа во вспомогательный объект.
								string responseText = reader.ReadToEnd();
								var status2 = new JavaScriptSerializer().Deserialize<ContactList>(responseText);
							}
						}
					}

					return;
				}
			}
			return;
		}

		public static void Test2(string userName, string userPassword)
		{
			HttpWebRequestToCrm newRequest = new HttpWebRequestToCrm();
			var status = HttpWebRequestToCrm.BpmAuthentificationResponse(BaseUri, "Supervisor", "Supervisor");

			// Проверка статуса аутентификации.
			if (status != null)
			{
				if (status.Code == 0)
				{
					if (WebRequest.Create(getUri) is HttpWebRequest dataRequest)
					{
						dataRequest.Method = "GET";
						dataRequest.ContentType = "application/json";
						dataRequest.CookieContainer = status.AuthCookie;
						dataRequest.Headers["BPMCSRF"] = status.Bpmcsrf;

						status = null;

						using (var response = (HttpWebResponse)dataRequest.GetResponse())
						{
							using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
							{
								using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader)reader))
								{
									JsonSerializer serializer = new JsonSerializer();
									var getContactList = serializer.Deserialize<ContactList>((JsonReader)jsonTextReader);
								}
							}
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
			Test2("Supervisor", "Supervisor");
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
