using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmazonEchoApi;

namespace AmazonEcho {
	class Program {
		static void Main(string[] args) {
			try {
				var req = new AlexaRequest("my@email.address", "P@$$VV0rd!");
				var tasksTask = req.GetTasks();
				foreach(var item in tasksTask) {
					Console.WriteLine(item.Text);
				}
				Console.WriteLine("");
				Console.WriteLine("PRESS ENTER TO EXIT");
				Console.ReadLine();
			} catch(Exception ex) {
				Console.Error.WriteLine(ex.Message);
				Console.WriteLine("PRESS ENTER TO EXIT");
				Console.ReadLine();

			}
		}
	}
}
