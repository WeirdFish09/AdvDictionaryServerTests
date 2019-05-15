using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using AdvDictionaryServer.DBContext;
using AdvDictionaryServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace AdvDictionaryServerTests
{
    public class DbAccessTests
    {
        //{

        DictionaryDBContext dbcontextMock;
        private void InitializeMock()
        {
            var options = new DbContextOptionsBuilder<AdvDictionaryServer.DBContext.DictionaryDBContext>()
                .UseInMemoryDatabase(databaseName : "AdvDictionary")
                .Options;
            dbcontextMock = new DictionaryDBContext(options);
            dbcontextMock.ForeignWords.AddRange(PrepareForeignWordMocks());
            dbcontextMock.NativePhrases.AddRange(PrepareNativePhrasesMocks());
            dbcontextMock.Languages.AddRange(PrepareLanguageMocks());
        }

        private List<NativePhrase> PrepareNativePhrasesMocks()
        {
            return new List<NativePhrase>()
            {
                new NativePhrase() {ID = 1, Phrase = "риба"},
                new NativePhrase() {ID = 2, Phrase = "двері"},
                new NativePhrase() {ID = 3, Phrase = "дерево"},
                new NativePhrase() {ID = 4, Phrase = "дивний"},
                new NativePhrase() {ID = 5, Phrase = "чудернацький"}
            };
        }

        private List<ForeignWord> PrepareForeignWordMocks()
        {
            return new List<ForeignWord>()
            {
                new ForeignWord() {ID = 1, Word = "fish"},
                new ForeignWord() {ID = 2, Word = "door"},
                new ForeignWord() {ID = 3, Word = "tree"},
                new ForeignWord() {ID = 4, Word = "weird"}
            };
        }

        private List<WordPriority> PrepareWordPrioritiesMocks()
        {
            
            return new List<WordPriority>(){};
        }

        private List<Language> PrepareLanguageMocks()
        {
            
            return new List<Language>(){new Language(){ID = 1, Name = "english"}};
        }

        [Fact]
        public void LoginReturnsAValidJWTToken()
        {
            var mock = new Mock<DictionaryDBContext>();
        }
    }
}
