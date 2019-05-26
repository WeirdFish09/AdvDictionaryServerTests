using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AdvDictionaryServer.Controllers;
using AdvDictionaryServer.DBContext;
using AdvDictionaryServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.CSharp;

namespace AdvDictionaryServerTests
{
    public class DbAccessTests
    {

        [Fact]
        public async void RegisterReturnsAValidJWTTokenOnValidInput()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
                User user = dbcontextMock.Users.Single();
                FakeUserManager userManager = new FakeUserManager(user);
                FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

                var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
                controller.ControllerContext = new ControllerContext() { HttpContext = context };
                var result = await controller.Register(new RegisterModel { Email = "test@gmail.com", Password = "testpwd" });
                LoginRegisterResponse response = (LoginRegisterResponse)result.Value;
                Assert.NotNull(response.Token);
        }

        [Fact]
        public async void LoginReturnsAValidJWTTokenOnValidInput()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.Single();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            IActionResult result = await controller.Login(new LoginModel { Email = "test@gmail.com", Password = "testpwd" });
            Assert.True(result is JsonResult);
            LoginRegisterResponse response = (LoginRegisterResponse)((JsonResult)result).Value;
            Assert.NotNull(response.Token);
        }

        [Theory]
        [InlineData("test2@gmail.com","fish")]
        [InlineData("test@gmail.com","testpwd2")]
        public async void LoginReturnsAnErrorOnInValidInput(string email, string password)
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.Where(u => u.Email == email).SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            IActionResult result = await controller.Login(new LoginModel { Email = email, Password = password });
            Assert.False(result is JsonResult);
        }

        [Fact]
        public async void GetTranslationsReturnsTranslationsOnValidInput ()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            List<WordPrioritiesJSON> expectedWordPriorities = new List<WordPrioritiesJSON>()
            {
                new WordPrioritiesJSON()
                {
                    Phrase = new NativePhraseJson() {Phrase = "риба"},
                    Word = new ForeignWordJSON(){Word = "fish"},
                    Language = new LanguageJSON(){Name = "english"},
                    Value = 0
                }
            };

            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.Single();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetTranslations(new GetTranslations { Language = "english", Word = "fish" });
            List<WordPrioritiesJSON> recievedWordPriorities = (List<WordPrioritiesJSON>)result.Value;

            Assert.True(recievedWordPriorities.Count>0);
            Assert.Equal(expectedWordPriorities,recievedWordPriorities);
        }

        [Theory]
        [InlineData(5,10)]
        public async void GetWordsPrioritiesReturnsEmptyCollectionIfOffsetIsTooLarge(int amount, int offset)
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetWordsPriorities(new GetWordPrioritiesModel {Language="english",Amount = amount, Offset = offset});
            Assert.False(((List<WordPrioritiesJSON>)result.Value).Any());
        }

        [Fact]
        public async void GetWordsPrioritiesReturnsCollectionOnValidInput()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            List<WordPrioritiesJSON> expectedWordPriorities = new List<WordPrioritiesJSON>(){
                new WordPrioritiesJSON()
                {
                    Language = new LanguageJSON() {Name =  "english"},
                    Phrase = new NativePhraseJson() {Phrase = "риба"},
                    Word = new ForeignWordJSON(){Word = "fish"},
                    Value = 0
                },
                new WordPrioritiesJSON()
                {
                    Language = new LanguageJSON() {Name = "english"},
                    Phrase = new NativePhraseJson() {Phrase = "двері"},
                    Word = new ForeignWordJSON(){Word = "door"},
                    Value = 0
                }
            };

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetWordsPriorities(new GetWordPrioritiesModel { Language = "english", Amount = 2, Offset = 0 });
            List<WordPrioritiesJSON> recievedWordPriorities = (List<WordPrioritiesJSON>)result.Value;
            Assert.Equal(expectedWordPriorities, recievedWordPriorities);
        }

        [Fact]
        public async void GetWordPrioritiesCountReturnsCorrectNumber()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            int expectedAmount = 5;

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetWordPrioritiesCount(new LanguageInputModel { Name = "english"});
            int recievedamount = (int)result.Value;
            Assert.Equal(expectedAmount, recievedamount);
        }

        [Theory]
        [InlineData("spanish")]
        public async void GetWordPrioritiesCountReturnsZeroOnWrongInput(string languageName)
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            int expectedAmount = 0;

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetWordPrioritiesCount(new LanguageInputModel { Name = languageName });
            int recievedamount = (int)result.Value;
            Assert.Equal(expectedAmount, recievedamount);
        }

        [Fact]
        public async void GetLanguagesReturnsCollectionOfLanguages()
        {
            DictionaryDBContext dbcontextMock = DBContextMock.GetContextMock();
            var context = new DefaultHttpContext();
            User user = dbcontextMock.Users.SingleOrDefault();
            FakeUserManager userManager = new FakeUserManager(user);
            FakeSignInManager signInManager = new FakeSignInManager(user, "testpwd");

            List<LanguageJSON> expectedLanguages = new List<LanguageJSON>()
            {
                new LanguageJSON(){Name = "english"}
            };

            var controller = new JSONDataController(userManager, signInManager, dbcontextMock);
            controller.ControllerContext = new ControllerContext() { HttpContext = context };
            JsonResult result = await controller.GetLanguages();
            List<LanguageJSON> recievedLanguages = (List<LanguageJSON>)result.Value;
            Assert.Equal(expectedLanguages, recievedLanguages);
        }
    }
}
