using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using NUnit;
using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using log4net.Config;
using log4net;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using Csv;
using System.Configuration;
using System.IO;
using System.Globalization;
using CsvHelper;

namespace Testinium
{
    class Program
    {
        
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            BasicConfigurator.Configure();

            ILog Logger = LogManager.GetLogger(typeof(Program));

            Random rnd = new Random();

            IWebDriver driver = new EdgeDriver();
            driver.Manage().Window.Maximize();
            Logger.Debug("Tarayıcı açıldı.");

            string homeUrl = "https://www.network.com.tr/";
            driver.Navigate().GoToUrl(homeUrl);
            Logger.Debug(homeUrl + " Adresine gidiliyor." );

            Logger.Debug("1 saniye bekliyor.");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();

            Logger.Info("Jean aranıyor.");
            IWebElement search = driver.FindElement(By.Id("search"));
            search.Click();
            search.SendKeys("jean");
            search.SendKeys(Keys.Return);


            Logger.Info("Daha fazlaya basılıyor. (Fakat onclick eventi yok.)");
            IWebElement showMore = driver.FindElement(By.XPath("(//button[contains(@class,'button -secondary')])[2]"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(showMore);
            actions.Perform();
            driver.FindElement(By.XPath("(//button[contains(@class,'button -secondary')])[2]")).Click();


            Thread.Sleep(TimeSpan.FromSeconds(2));
            Logger.Debug("İndirimli ürüne hover ediyor.");
            IWebElement discount = driver.FindElement(By.ClassName("product__discountPercent"));
            actions.MoveToElement(discount);
            actions.Perform();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Logger.Info("Rastgele beden seçiliyor.");
            //var size = new List<string>() { "(//label[@class='radio-box__label '])[1]", "(//label[@class='radio-box__label '])[2]", "(//label[@class='radio-box__label '])[3]", "(//label[@class='radio-box__label '])[4]", "(//label[@class='radio-box__label '])[5]", "(//label[@class='radio-box__label '])[6]" };
            //int random = rnd.Next(0, 3);
            //var randomSize = size[random].ToString();
            List<IWebElement> elementList = new List<IWebElement>();
            elementList.AddRange(driver.FindElements(By.XPath("(//label[@class='radio-box__label '])")));

            if (elementList.Count > 0)
            {

                elementList[0].Click();
            }
            //driver.FindElement(By.XPath(randomSize)).Click();

            
            Thread.Sleep(TimeSpan.FromSeconds(2));
            //driver.FindElement(By.LinkText("Sepete Git")).Click();
            //------ butonu bir türlü görmüyor ex atıyor
            Logger.Debug("Sepete git butonuna tıklanıyor.");
            driver.Navigate().GoToUrl(homeUrl + "cart");


            Thread.Sleep(TimeSpan.FromSeconds(4));
            string fiyat = driver.FindElement(By.XPath("(//div[@class='summaryItem__value'])[1]")).Text;
            string indirim = driver.FindElement(By.XPath("(//div[@class='summaryItem__value'])[4]")).Text;
            fiyat = fiyat.Substring(0, fiyat.Length - 2).Trim();
            indirim = indirim.Substring(0, indirim.Length - 2).Trim();

            if (System.Convert.ToDecimal(fiyat) > System.Convert.ToDecimal(indirim))
            {
                Logger.Info(" Eski fiyatı indirimli fiyatından büyük.");
            }
            else
            {
                Logger.Info(" Eski fiyatı indirimli fiyatından küçük.");
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.FindElement(By.XPath("(//button[contains(@class,'continueButton n-button')])[1]")).Click();
            Logger.Info("Devam et butonuna tıklanıyor.");

            //csvReader = new CsvReader(new FileReader(CSV_PATH));
            //string text = System.IO.File.ReadAllText(csvFile);

            using var streamReader = File.OpenText("data.csv");
            using var csvReader = new CsvHelper.CsvReader(streamReader, CultureInfo.InvariantCulture);
            csvReader.Read();
            var Eposta = csvReader.GetField(0).ToString();
            var Sifre = csvReader.GetField(1).ToString();
            

            Thread.Sleep(TimeSpan.FromSeconds(2));
            Logger.Info("Email ve şifre giriliyor.");
            driver.FindElement(By.Id("n-input-email")).SendKeys(Eposta);
            driver.FindElement(By.Id("n-input-password")).SendKeys(Sifre);
            driver.FindElement(By.XPath("//button[contains(@class,'n-button large')]")).Click();


            Thread.Sleep(TimeSpan.FromSeconds(1));
            Logger.Info("Network logosuna tıklanıyor.");
            driver.FindElement(By.ClassName("headerCheckout__logo__img")).Click();

            Thread.Sleep(TimeSpan.FromSeconds(1));
            Logger.Info("Çanta seçiliyor.");
            IWebElement canta = driver.FindElement(By.XPath("(//a[@class='o-header__navItem--link'])[2]"));
            actions.MoveToElement(canta);
            actions.Perform();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            driver.FindElement(By.XPath("(//a[@href='/canta-731'])[2]")).Click();


            Thread.Sleep(TimeSpan.FromSeconds(1));
            Logger.Info("Sepet açılıyor.");
            driver.FindElement(By.XPath("(//span[@class='header__basket--count'])[1]")).Click();


            Thread.Sleep(TimeSpan.FromSeconds(2));
            Logger.Info("Sepetteki ürün siliniyor.");
            driver.FindElement(By.XPath("//div[contains(@class,'header__basketProductBtn header__basketModal -remove')]")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            driver.FindElement(By.XPath("//button[contains(@class,'btn -black')]")).Click();


            Logger.Debug("Kapanmasına son 10 saniye.");
            Thread.Sleep(TimeSpan.FromSeconds(10));
            Logger.Info("Exiting browser.");
            driver.Quit();
        }
    }
}
