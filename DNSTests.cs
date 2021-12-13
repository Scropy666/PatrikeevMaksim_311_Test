using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace AutoTests
{
    public class DNSTests
    {
        WebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.dns-shop.ru/");
            driver.FindElement(By.CssSelector(".btn.btn-additional")).Click();

        }

        [Test]
        public void TestPriceFilter()
        {


            driver.FindElement(By.XPath("//*[contains(text(), 'Смартфоны и гаджеты')]")).Click();
            driver.FindElement(By.XPath("//span[text() = 'Смартфоны и гаджеты']")).Click();
            driver.FindElement(By.XPath("//span[text() = 'Смартфоны']")).Click();



            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector(".ui-list-controls__content.ui-list-controls__content_custom.left-filters__radio-list")));
            Actions actions = new Actions(driver);
            actions.SendKeys(Keys.PageDown).Build().Perform();

            driver.FindElement(By.XPath("//input[@placeholder = 'от 2999']")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//input[@placeholder = 'от 2999']")));
            driver.FindElement(By.XPath("//input[@placeholder = 'от 2999']")).Clear();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//input[@placeholder = 'от 2999']")));
            driver.FindElement(By.XPath("//input[@placeholder = 'от 2999']")).SendKeys("30000");

            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//input[@placeholder = 'до 169999']")));
            driver.FindElement(By.XPath("//input[@placeholder = 'до 169999']")).Clear();
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(x => driver.FindElement(By.XPath("//input[@placeholder = 'до 169999']")));
            driver.FindElement(By.XPath("//input[@placeholder = 'до 169999']")).SendKeys("50000");


            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.XPath("//*[@data-role='filters-submit']")).Enabled);

            driver.FindElement(By.XPath("//*[@class='apply-filters-float-btn']")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
            .Until(x => driver.FindElements(By.CssSelector(".catalog-preloader__spin")).Count > 0);

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
            .Until(x => driver.FindElements(By.CssSelector(".catalog-preloader__spin")).Count == 0);
            var webPrices = driver.FindElements(By.CssSelector(".product-buy__prev"));
            if (driver.FindElements(By.CssSelector(".product-buy__price.product-buy__price_active")).Any()) webPrices = driver.FindElements(By.CssSelector(".product-buy__prev"));

            int[] actualPrices = webPrices.Select(webPrice => Int32.Parse(Regex.Replace(webPrice.Text, @" \₽?", ""))).ToArray();
            actualPrices.ToList().ForEach(price => Assert.IsTrue(price >= 30000 && price <= 50000));



        }

        [Test]
        public void TestTooltipText()
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(Keys.PageDown);
            driver.FindElement(By.CssSelector("body")).SendKeys(Keys.PageDown);
            new WebDriverWait(driver, TimeSpan.FromSeconds(20))
                 .Until(x => driver.FindElements(By.CssSelector("button-ui_white button-ui_icon")).Count == 0);

            var firstButtonFavorites = driver.FindElement(By.CssSelector("button.button-ui.button-ui_white.button-ui_icon.wishlist-btn"));
            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                 .Until(x => driver.FindElements(By.CssSelector("button.button-ui.button-ui_white.button-ui_icon.wishlist-btn")).Count > 0);
            new Actions(driver).MoveToElement(firstButtonFavorites).Build().Perform();

            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                 .Until(x => driver.FindElement(By.CssSelector("span.tooltip-ui.tooltip-ui_top")).Displayed);

            Assert.AreEqual(driver.FindElement(By.CssSelector("span.tooltip-ui.tooltip-ui_top")).Text.Trim(), "Добавить в избранное", "Uncorrect text in tooltip.");
        }

        [Test]
        public void TestInputInvalidPhoneNumber()
        {
            driver.FindElement(By.XPath("//*[contains(text(), 'Войти')]")).Click();
            driver.FindElement(By.CssSelector(".base-ui-input-row__input.base-ui-input-row__input_with-icon")).SendKeys("");
            driver.FindElement(By.CssSelector(".base-ui-button.base-ui-button_brand.base-ui-button_big-flexible-width")).Click();
            Assert.IsTrue(driver.FindElements(By.CssSelector(".error-message-block.form-entry-or-registry__error")).Any(),
            "Error 'Заполните поле' is enabled when phone number input is empty");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}




