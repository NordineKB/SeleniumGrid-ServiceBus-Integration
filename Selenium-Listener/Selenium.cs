using OpenQA.Selenium.Remote;
using System;
using System.Threading.Tasks;

namespace Selenium.Listener
{
	class Selenium
	{
		public async Task UseSeleniumDriver()
		{
			DesiredCapabilities caps = new DesiredCapabilities();
			caps.SetCapability("platformVersion", "81.0");
			caps.SetCapability("browserName", "chrome");

			using (var driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), caps))
			{
				// Do Selenium Stuff

				Console.WriteLine($"{driver.SessionId}");
				driver.Quit();
			}
		}
	}
}
