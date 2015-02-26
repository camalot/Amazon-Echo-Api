using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmazonEchoApi {
	internal class HttpWebRequestBuilder {
		private HttpWebRequestBuilder(Uri uri) {
			Request = HttpWebRequest.CreateHttp(uri);
		}
		public static HttpWebRequestBuilder Build(Uri uri) {
			return new HttpWebRequestBuilder(uri);
		}

		public HttpWebRequestBuilder Setting<V>(Expression<Func<HttpWebRequest, V>> xpression, V value) {
			var memberSelectorExpression = xpression.Body as MemberExpression;
			if(memberSelectorExpression != null) {
				var property = memberSelectorExpression.Member as PropertyInfo;
				if(property != null) {
					property.SetValue(Request, value, null);
				}
			}
			return this;
		}

		public HttpWebRequestBuilder SetHeader(string name, string value) {
			if(Request.Headers.HasKeys() && Request.Headers.AllKeys.Contains(name)) {
				Request.Headers[name] = value;
			} else {
				Request.Headers.Add(name, value);
			}
			return this;
		}

		public HttpWebRequestBuilder SetHeader(string header) {
			var kvp = header.Split(new string[] { ":" }, 2,StringSplitOptions.RemoveEmptyEntries);
			return SetHeader(kvp[0], kvp[1]);
		}

		public HttpWebRequest Create() {
			return Request;
		}
		private HttpWebRequest Request { get; set; }
	}
}
