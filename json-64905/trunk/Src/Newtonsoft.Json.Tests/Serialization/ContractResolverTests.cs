﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json4.Serialization;
using Newtonsoft.Json4.Tests.TestObjects;
using System.Reflection;

namespace Newtonsoft.Json4.Tests.Serialization
{
  public class DynamicContractResolver : DefaultContractResolver
  {
    private readonly char _startingWithChar;
    public DynamicContractResolver(char startingWithChar)
      : base(false)
    {
      _startingWithChar = startingWithChar;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

      // only serializer properties that start with the specified character
      properties =
        properties.Where(p => p.PropertyName.StartsWith(_startingWithChar.ToString())).ToList();

      return properties;
    }
  }

  public class Book
  {
    public string BookName { get; set; }
    public decimal BookPrice { get; set; }
    public string AuthorName { get; set; }
    public int AuthorAge { get; set; }
    public string AuthorCountry { get; set; }
  }

  public class IPersonContractResolver : DefaultContractResolver
  {
    protected override JsonContract CreateContract(Type objectType)
    {
      if (objectType == typeof(Employee))
        objectType = typeof(IPerson);

      return base.CreateContract(objectType);
    }
  }

  public class ContractResolverTests : TestFixtureBase
  {
    [Test]
    public void SerializeInterface()
    {
      Employee employee = new Employee
         {
           BirthDate = new DateTime(1977, 12, 30, 1, 1, 1, DateTimeKind.Utc),
           FirstName = "Maurice",
           LastName = "Moss",
           Department = "IT",
           JobTitle = "Support"
         };

      string iPersonJson = JsonConvert.SerializeObject(employee, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = new IPersonContractResolver() });

      Assert.AreEqual(@"{
  ""FirstName"": ""Maurice"",
  ""LastName"": ""Moss"",
  ""BirthDate"": ""\/Date(252291661000)\/""
}", iPersonJson);
    }

    [Test]
    public void SingleTypeWithMultipleContractResolvers()
    {
      Book book = new Book
                    {
                      BookName = "The Gathering Storm",
                      BookPrice = 16.19m,
                      AuthorName = "Brandon Sanderson",
                      AuthorAge = 34,
                      AuthorCountry = "United States of America"
                    };

      string startingWithA = JsonConvert.SerializeObject(book, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = new DynamicContractResolver('A') });

      // {
      //   "AuthorName": "Brandon Sanderson",
      //   "AuthorAge": 34,
      //   "AuthorCountry": "United States of America"
      // }

      string startingWithB = JsonConvert.SerializeObject(book, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = new DynamicContractResolver('B') });

      // {
      //   "BookName": "The Gathering Storm",
      //   "BookPrice": 16.19
      // }

      Assert.AreEqual(@"{
  ""AuthorName"": ""Brandon Sanderson"",
  ""AuthorAge"": 34,
  ""AuthorCountry"": ""United States of America""
}", startingWithA);

      Assert.AreEqual(@"{
  ""BookName"": ""The Gathering Storm"",
  ""BookPrice"": 16.19
}", startingWithB);
    }

    [Test]
    public void SerializeCompilerGeneratedMembers()
    {
      StructTest structTest = new StructTest
        {
          IntField = 1,
          IntProperty = 2,
          StringField = "Field",
          StringProperty = "Property"
        };

      DefaultContractResolver skipCompilerGeneratedResolver = new DefaultContractResolver
      {
        DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
      };

      string skipCompilerGeneratedJson = JsonConvert.SerializeObject(structTest, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = skipCompilerGeneratedResolver });

      Assert.AreEqual(@"{
  ""StringField"": ""Field"",
  ""IntField"": 1,
  ""StringProperty"": ""Property"",
  ""IntProperty"": 2
}", skipCompilerGeneratedJson);

      DefaultContractResolver includeCompilerGeneratedResolver = new DefaultContractResolver
      {
        DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
        SerializeCompilerGeneratedMembers = true
      };

      string includeCompilerGeneratedJson = JsonConvert.SerializeObject(structTest, Formatting.Indented,
        new JsonSerializerSettings { ContractResolver = includeCompilerGeneratedResolver });

      Assert.AreEqual(@"{
  ""StringField"": ""Field"",
  ""IntField"": 1,
  ""<StringProperty>k__BackingField"": ""Property"",
  ""<IntProperty>k__BackingField"": 2,
  ""StringProperty"": ""Property"",
  ""IntProperty"": 2
}", includeCompilerGeneratedJson);
    }
  }
}