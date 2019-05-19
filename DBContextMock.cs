using System;
using System.Collections.Generic;
using System.Text;
using AdvDictionaryServer.DBContext;
using AdvDictionaryServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;


namespace AdvDictionaryServerTests
{
    static class DBContextMock
    {
        static DBContextMock()
        {
            InitializeMock();
        }

        public static DictionaryDBContext dbcontextMock;
        private static void InitializeMock()
        {
            var options = new DbContextOptionsBuilder<DictionaryDBContext>()
                .UseInMemoryDatabase(databaseName: "AdvDictionary")
                .EnableSensitiveDataLogging()
                .Options;
            dbcontextMock = new DictionaryDBContext(options);
            //var userStore = Mock.Of<IUserStore<User>>();
            //var userManager = new Mock<UserManager<User>>(userStore, null, null, null, null, null, null, null, null);
            //UserManagerMock userManager = new UserManagerMock(dbcontextMock);

            List<ForeignWord> foreignWords = PrepareForeignWordMocks();
            List<NativePhrase> nativePhrases = PrepareNativePhrasesMocks();
            User user = PrepareUserMock("test@gmail.com");
            Language language = PrepareLanguageMock(user);
            List<WordPriority> wordPriorities = PrepareWordPrioritiesMocks(nativePhrases, foreignWords, language);

            dbcontextMock.NativePhrases.AddRange(nativePhrases);
            dbcontextMock.ForeignWords.AddRange(foreignWords);
            dbcontextMock.Users.Add(user);
            dbcontextMock.Languages.Add(language);
            dbcontextMock.WordPriorities.AddRange(wordPriorities);
            dbcontextMock.SaveChanges();
        }

        static private List<NativePhrase> PrepareNativePhrasesMocks()
        {
            return new List<NativePhrase>()
            {
                new NativePhrase() {Phrase = "риба"},
                new NativePhrase() {Phrase = "двері"},
                new NativePhrase() {Phrase = "дерево"},
                new NativePhrase() {Phrase = "дивний"},
                new NativePhrase() {Phrase = "чудернацький"}
            };
        }

        static private List<ForeignWord> PrepareForeignWordMocks()
        {
            return new List<ForeignWord>()
            {
                new ForeignWord() {Word = "fish"},
                new ForeignWord() {Word = "door"},
                new ForeignWord() {Word = "tree"},
                new ForeignWord() {Word = "weird"}
            };
        }

        static private List<WordPriority> PrepareWordPrioritiesMocks(List<NativePhrase> phrases, List<ForeignWord> words, Language language)
        {
            List<WordPriority> wordPriorities = new List<WordPriority>();
            for (int i = 0; i < words.Count; i++)
            {
                wordPriorities.Add(new WordPriority()
                {
                    ForeignWord = words[i],
                    NativePhrase = phrases[i],
                    Value = 0,
                    Language = language
                });
            }
            wordPriorities.Add(new WordPriority() { Value = 0, ForeignWord = words.Last(), NativePhrase = phrases.Last(), Language = language });

            return wordPriorities;
        }

        static private Language PrepareLanguageMock(User user)
        {

            return new Language() { Name = "english", User = user };
        }

        static private User PrepareUserMock(string email)
        {
            User user = new User { Email = email, UserName = email };
            return user;
        }

        static public DictionaryDBContext GetContextMock()
        {
            return dbcontextMock;
        }
    }
}
