using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Camalot.Common.Serialization;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Camalot.Common.Extensions;
using System.Text.RegularExpressions;
using Camalot.Common.Net;

namespace AmazonEchoApi {
	public class AlexaRequest {
		private sealed class Defaults {
			public const string USERAGENT = "Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.13) Gecko/20101206 Ubuntu/10.10 (maverick) Firefox/3.6.13";
			public const string REFERER = "http://echo.amazon.com/spa/index.html";
			public const string ORIGIN = "http://echo.amazon.com";

			public const string LOGIN_FORM_ID = "ap_signin_form";
		}
		private sealed class Urls {
			public const string BASE = "https://pitangui.amazon.com";
			public const string WEBSOCKET_FORMAT = "wss://dp-gw-na-js.amazon.com:443/?x-amz-device-type={0}&x-amz-device-serial={1}";
			public const string CARDS = "/api/cards?limit=50";
			public const string TASKS = "/api/todos?type=TASK&size=100";
			public const string TASK_PUT_FORMAT = "/api/todos/{0}";
			public const string SHOPPING = "/api/todos?type=SHOPPING_ITEM&size=100";
			public const string DEVICES = "/api/device?cache=true";
			public const string GET_MEDIA_STATE_FORMAT = "/api/media/state?deviceSerialNumber={0}&deviceType={1}";
			public const string BLUETOOTH = "/api/bluetooth?cached=true";
			public const string MUSIC_ACCOUNTS = "/api/music-account-details";
			public const string PING = "/api/ping";
			public const string LOGOUT = "/logout";
			public const string FEATURE_ACCESS_FORMAT = "/api/featureaccess/{0}";
			public const string PROVIDER_CONTENTTYPE_CAPABILITIES = "/api/media/provider-contenttype-capabilities";

			public const string SET_MEDIA_STATE_FORMAT = "/api/media/state/{0}";
		}

		private sealed class MediaStates {
			public const string STOP = "stop";
			public const string PLAY = "play";
			public const string VOLUME = "volume";
			public const string REPEAT_ALL = "repeat-all";
			public const string SHUFFLE = "shuffle";
			public const string NEXT_TRACK = "next";
			public const string PREVIOUS_TRACK = "previous";
		}

		private sealed class Features {
			private const string FEATURE_AMBER = "AMBER_FEATURE";
			private const string FEATURE_ALERT = "FEATURE_ALERT_FEATURE";
			private const string FEATURE_TRAFFIC = "TRAFFIC_FEATURE";
			private const string FEATURE_PHOENIX = "PHOENIX_FEATURE";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AlexaRequest"/> class.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		public AlexaRequest(string username, string password) {
			Username = username.Require();
			Password = password.Require();
			Cookies = new CookieContainer();

			ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
												System.Security.Cryptography.X509Certificates.X509Chain chain,
												System.Net.Security.SslPolicyErrors sslPolicyErrors) {
				return true; // **** Always accept certificate
			};
		}

		private string Username { get; set; }
		private string Password { get; set; }
		private CookieContainer Cookies { get; set; }

		/// <summary>
		/// Gets the tasks.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TaskItem> GetTasks() {
			if(Login()) {
				return DeserializeStream<TaskItemCollection>(Urls.TASKS).Values;
			}
			return null;
		}
		/// <summary>
		/// Gets the shopping items.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TaskItem> GetShoppingItems() {
			if(Login()) {
				return DeserializeStream<TaskItemCollection>(Urls.SHOPPING).Values;
			}
			return null;
		}

		/// <summary>
		/// Determines whether the specified feature is supported.
		/// </summary>
		/// <param name="feature">The feature.</param>
		/// <returns></returns>
		public bool HasSupport(string feature) {
			if(Login()) {
				var result = DeserializeStream<FeatureAccessResult>(Urls.FEATURE_ACCESS_FORMAT.With(feature.Require()));
				return result.HasAccess;
			}
			return false;
		}

		/// <summary>
		/// Updates the task item.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		public TaskItem UpdateTaskItem(TaskItem task) {
			if(Login()) {
				var result = Put<TaskItem>(task, Urls.TASK_PUT_FORMAT.With(task.Id));
				return result;
			}
			return null;
		}


		/// <summary>
		/// Plays the specified device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public bool Play(EchoDevice device) {
			if(Login()) {
				var result = Post<MediaStatePayload>(new MediaStatePayload {
					DeviceSerialNumber = device.SerialNumber,
					DeviceType = device.Type,
					NewMediaVolume = null
				}, Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.PLAY));
				return result != null;
			}
			return false;
		}

		/// <summary>
		/// Plays the specified device.
		/// </summary>
		/// <param name="deviceSerialNumber">The device serial number.</param>
		/// <param name="deviceType">Type of the device.</param>
		/// <returns></returns>
		public bool Play(string deviceSerialNumber, string deviceType) {
			return Play(new EchoDevice {
				Type = deviceType.Require(),
				SerialNumber = deviceSerialNumber.Require()
			});
		}

		/// <summary>
		/// Stops the specified device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public bool Stop(EchoDevice device) {
			if(Login()) {
				var result = Post<MediaStatePayload>(new MediaStatePayload {
					DeviceSerialNumber = device.SerialNumber,
					DeviceType = device.Type,
					NewMediaVolume = null
				}, Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.STOP));
				return result != null;
			}
			return false;
		}

		/// <summary>
		/// Stops the specified device.
		/// </summary>
		/// <param name="deviceSerialNumber">The device serial number.</param>
		/// <param name="deviceType">Type of the device.</param>
		/// <returns></returns>
		public bool Stop(string deviceSerialNumber, string deviceType) {
			return Stop(new EchoDevice {
				Type = deviceType.Require(),
				SerialNumber = deviceSerialNumber.Require()
			});
		}

		/// <summary>
		/// Sets the volume on the specified device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <param name="volume">The volume.</param>
		/// <returns></returns>
		public bool Volume(EchoDevice device, int volume) {
			if(Login()) {
				var options = Options(Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.VOLUME));
				if(options == HttpStatusCode.OK) {
					var result = Post<MediaStatePayload>(new MediaStatePayload {
						DeviceSerialNumber = device.SerialNumber,
						DeviceType = device.Type,
						NewMediaVolume = volume.RequireBetween(-1, 101)
					}, Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.VOLUME));
					return result != null;
				}
			}
			return false;
		}

		/// <summary>
		/// Sets the volume on the specified device.
		/// </summary>
		/// <param name="deviceSerialNumber">The device serial number.</param>
		/// <param name="deviceType">Type of the device.</param>
		/// <param name="volume">The volume.</param>
		/// <returns></returns>
		public bool Volume(string deviceSerialNumber, string deviceType, int volume) {
			return Volume(new EchoDevice {
				Type = deviceType.Require(),
				SerialNumber = deviceSerialNumber.Require()
			}, volume);
		}

		/// <summary>
		/// Mutes the specified device.
		/// </summary>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		public bool Mute(EchoDevice device) {
			if(Login()) {
				var options = Options(Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.VOLUME));
				if(options == HttpStatusCode.OK) {
					var result = Post<MediaStatePayload>(new MediaStatePayload {
						DeviceSerialNumber = device.SerialNumber,
						DeviceType = device.Type,
						NewMediaVolume = 0
					}, Urls.SET_MEDIA_STATE_FORMAT.With(MediaStates.VOLUME));
					return result != null;
				}
			}
			return false;
		}

		/// <summary>
		/// Mutes the specified device serial number.
		/// </summary>
		/// <param name="deviceSerialNumber">The device serial number.</param>
		/// <param name="deviceType">Type of the device.</param>
		/// <returns></returns>
		public bool Mute(string deviceSerialNumber, string deviceType) {
			return Mute(new EchoDevice {
				Type = deviceType.Require(),
				SerialNumber = deviceSerialNumber.Require()
			});
		}

		/// <summary>
		/// Logout of Echo service.
		/// </summary>
		/// <returns></returns>
		public bool Logout() {
			return GetDataStream(Urls.LOGOUT) != null;
		}

		/// <summary>
		/// Login to the Echo service.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.Net.WebException"></exception>
		public bool Login() {

			// this needs to check if we need to log in, or if we are already logged in.

			var document = new HtmlDocument {
				OptionFixNestedTags = true
			};

			document.LoadHtml(Get(""));
			var form = document.GetElementbyId(Defaults.LOGIN_FORM_ID).Require();
			var action = form.Attributes.FirstOrDefault(x => x.Name == "action");
			if(action == null || string.IsNullOrWhiteSpace(action.Value)) {
				return false;
			}
			var urlAction = action.Value;
			var hiddens = form.SelectNodes("//input[@type=\"hidden\"]");
			var kvps = new List<KeyValuePair<string, string>>();
			kvps.Add(new KeyValuePair<string, string>("email", Username));
			kvps.Add(new KeyValuePair<string, string>("password", Password));
			kvps.Add(new KeyValuePair<string, string>("create", "0"));

			foreach(var item in hiddens) {
				kvps.Add(new KeyValuePair<string, string>(item.Attributes["name"].Value, item.Attributes["value"].Value));
			}


			var postData = new StringBuilder();
			foreach(var item in kvps) {
				postData.AppendFormat("{0}={1}&", item.Key, item.Value);
			}

			if(postData.Length > 0) {
				postData = postData.Remove(postData.Length - 1, 1);
			}
			var dataString = postData.ToString();
			var dataBytes = Encoding.UTF8.GetBytes(dataString);

			var req = HttpWebRequestBuilder.Build(new Uri(urlAction))
				.Setting(x => x.UserAgent, Defaults.USERAGENT)
				.Setting(x => x.Referer, Urls.BASE)
				.Setting(x => x.CookieContainer, Cookies)
				.Setting(x => x.Method, HttpMethods.POST)
				.Setting(x => x.ContentType, "application/x-www-form-urlencoded")
				.Setting(x => x.ContentLength, dataBytes.Length)
				.SetHeader("Origin", Defaults.ORIGIN)
				.Create();

			using(var rstream = req.GetRequestStream()) {
				rstream.Write(dataBytes, 0, dataBytes.Length);
			}

			var resp = req.GetResponse() as HttpWebResponse;
			var statusCode = resp.StatusCode;
			if(statusCode == HttpStatusCode.OK) {
				return true;
			}

			// resetting the container starts the session over.
			Cookies = new CookieContainer();
			throw new WebException(resp.StatusDescription, (WebExceptionStatus)statusCode);
		}


		private T DeserializeStream<T>(string url) {
			using(var stream = GetDataStream(Urls.SHOPPING)) {
				if(stream != null) {
					using(var sr = new StreamReader(stream)) {
						using(var jr = new JsonTextReader(sr)) {
							var result = JsonSerializationBuilder.Build().Create().Deserialize<T>(jr);
							return result;
						}
					}
				}
			}
			return default(T);
		}

		private T Put<T>(T payload, string url) {
			return SendPayload<T>(url, HttpMethods.PUT, payload);
		}

		private T Post<T>(T payload, string url) {
			return SendPayload<T>(url, HttpMethods.POST, payload);
		}

		private HttpStatusCode Options(string url) {
			var req = HttpWebRequestBuilder.Build(new Uri(Urls.BASE + url))
					.Setting(x => x.UserAgent, Defaults.USERAGENT)
					.Setting(x => x.Referer, Defaults.REFERER)
					.Setting(x => x.CookieContainer, Cookies)
					.Setting(x => x.Method, HttpMethods.OPTIONS)
					.SetHeader("Access-Control-Request-Method", "POST")
					.SetHeader("Access-Control-Request-Headers", "accept, csrf, content-type")
					.SetHeader("Origin", Defaults.ORIGIN)
					.Create();
			var resp = req.GetResponse() as HttpWebResponse;
			return resp.StatusCode;
		}

		private T SendPayload<T>(string url, string method, T payload) {
			try {
				var sb = new StringBuilder();
				// this finds the csrf cookie. if it exists, it will be added as a header
				var csrf = FindCookie("csrf");

				foreach(Cookie f in Cookies.GetCookies(new Uri(Defaults.ORIGIN))) {
					Console.WriteLine("{0}:{1}",f.Name,f.Value);
				}

				var req = HttpWebRequestBuilder.Build(new Uri(Urls.BASE + url))
					.Setting(x => x.UserAgent, Defaults.USERAGENT)
					.Setting(x => x.Referer, Defaults.REFERER)
					.Setting(x => x.CookieContainer, Cookies)
					.Setting(x => x.Accept, "application/json, text/javascript, */*; q=0.01")
					.Setting(x => x.ContentType, "application/json")
					.Setting(x => x.Method, method.Require())
					.SetHeader("Origin", Defaults.ORIGIN)
					.SetHeader("csrf", csrf)
					.Create();

				var serializer = JsonSerializationBuilder.Build().Create();

				using(var serializedDataStream = new MemoryStream()) {
					using(var streamWriter = new StreamWriter(serializedDataStream)) {
						serializer.Serialize(streamWriter, payload);
						streamWriter.Flush();
						// move to start
						serializedDataStream.Position = 0;
						using(var rs = req.GetRequestStream()) {
							serializedDataStream.CopyTo(rs);
						}

						var resp = req.GetResponse() as HttpWebResponse;
						if(resp.StatusCode != HttpStatusCode.OK) {
							throw new WebException(resp.StatusDescription, (WebExceptionStatus)resp.StatusCode);
						}
					}
				}
				return payload;
			} catch(WebException wex) {
				Console.Error.WriteLine(wex.Message);
			}
			return default(T);
		}

		private string FindCookie(string key) {
			var cookies = Cookies.GetCookies(new Uri(Urls.BASE));
			if(cookies != null && cookies[key] != null) {
				return cookies[key].Value;
			} else {
				return null;
			}
		}

		private string Get(string url) {
			var sb = new StringBuilder();
			var stream = GetDataStream(url);
			if(stream != null) {
				using(var sr = new StreamReader(stream)) {
					while(!sr.EndOfStream) {
						sb.AppendLine(sr.ReadLine());
					}
				}
			}
			return sb.ToString();
		}

		private Stream GetDataStream(string url) {
			try {
				var sb = new StringBuilder();

				var req = HttpWebRequestBuilder.Build(new Uri(Urls.BASE + url))
					.Setting(x => x.UserAgent, Defaults.USERAGENT)
					.Setting(x => x.Referer, Defaults.REFERER)
					.Setting(x => x.CookieContainer, Cookies)
					.Setting(x => x.Method, HttpMethods.GET)
					.SetHeader("Origin", Defaults.ORIGIN)
					.Create();
				var response = req.GetResponse() as HttpWebResponse;
				if(response.StatusCode == HttpStatusCode.OK) {
					return response.GetResponseStream();
				}

			} catch(WebException wex) {
				Console.Error.WriteLine(wex.Message);
			}
			return null;
		}

	}
}
