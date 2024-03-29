﻿using System;
using System.Collections.Generic;
#if !SILVERLIGHT && !PocketPC && !NET20
using System.Data.Linq;
#endif
#if !SILVERLIGHT
using System.Data.SqlTypes;
#endif
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json4.Bson;
using Newtonsoft.Json4.Converters;
using Newtonsoft.Json4.Utilities;
using NUnit.Framework;
using Newtonsoft.Json4.Tests.TestObjects;

namespace Newtonsoft.Json4.Tests.Converters
{
  public class RegexConverterTests : TestFixtureBase
  {
    public class RegexTestClass
    {
      public Regex Regex { get; set; }
    }

    [Test]
    public void SerializeToText()
    {
      Regex regex = new Regex("abc", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

      string json = JsonConvert.SerializeObject(regex, Formatting.Indented, new RegexConverter());

      Assert.AreEqual(@"{
  ""Pattern"": ""abc"",
  ""Options"": 513
}", json);
    }

    [Test]
    public void SerializeToBson()
    {
      Regex regex = new Regex("abc", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

      MemoryStream ms = new MemoryStream();
      BsonWriter writer = new BsonWriter(ms);
      JsonSerializer serializer = new JsonSerializer();
      serializer.Converters.Add(new RegexConverter());

      serializer.Serialize(writer, new RegexTestClass { Regex = regex });

      string expected = "13-00-00-00-0B-52-65-67-65-78-00-61-62-63-00-69-75-00-00";
      string bson = MiscellaneousUtils.BytesToHex(ms.ToArray());

      Assert.AreEqual(expected, bson);
    }

    [Test]
    public void DeserializeFromText()
    {
      string json = @"{
  ""Pattern"": ""abc"",
  ""Options"": 513
}";

      Regex newRegex = JsonConvert.DeserializeObject<Regex>(json, new RegexConverter());
      Assert.AreEqual("abc", newRegex.ToString());
      Assert.AreEqual(RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, newRegex.Options);
    }

    [Test]
    public void DeserializeFromBson()
    {
      MemoryStream ms = new MemoryStream(MiscellaneousUtils.HexToBytes("13-00-00-00-0B-52-65-67-65-78-00-61-62-63-00-69-75-00-00"));
      BsonReader reader = new BsonReader(ms);
      JsonSerializer serializer = new JsonSerializer();
      serializer.Converters.Add(new RegexConverter());

      RegexTestClass c = serializer.Deserialize<RegexTestClass>(reader);

      Assert.AreEqual("abc", c.Regex.ToString());
      Assert.AreEqual(RegexOptions.IgnoreCase, c.Regex.Options);
    }

    [Test]
    public void ConvertEmptyRegexBson()
    {
      Regex regex = new Regex(string.Empty);

      MemoryStream ms = new MemoryStream();
      BsonWriter writer = new BsonWriter(ms);
      JsonSerializer serializer = new JsonSerializer();
      serializer.Converters.Add(new RegexConverter());

      serializer.Serialize(writer, new RegexTestClass { Regex = regex });

      ms.Seek(0, SeekOrigin.Begin);
      BsonReader reader = new BsonReader(ms);
      serializer.Converters.Add(new RegexConverter());

      RegexTestClass c = serializer.Deserialize<RegexTestClass>(reader);

      Assert.AreEqual("", c.Regex.ToString());
      Assert.AreEqual(RegexOptions.None, c.Regex.Options);
    }

    [Test]
    public void ConvertRegexWithAllOptionsBson()
    {
      Regex regex = new Regex(
        "/",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

      MemoryStream ms = new MemoryStream();
      BsonWriter writer = new BsonWriter(ms);
      JsonSerializer serializer = new JsonSerializer();
      serializer.Converters.Add(new RegexConverter());

      serializer.Serialize(writer, new RegexTestClass { Regex = regex });

      string expected = "14-00-00-00-0B-52-65-67-65-78-00-2F-00-69-6D-73-75-78-00-00";
      string bson = MiscellaneousUtils.BytesToHex(ms.ToArray());

      Assert.AreEqual(expected, bson);

      ms.Seek(0, SeekOrigin.Begin);
      BsonReader reader = new BsonReader(ms);
      serializer.Converters.Add(new RegexConverter());

      RegexTestClass c = serializer.Deserialize<RegexTestClass>(reader);

      Assert.AreEqual("/", c.Regex.ToString());
      Assert.AreEqual(RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.ExplicitCapture, c.Regex.Options);
    }

    [Test]
    public void ConvertEmptyRegexJson()
    {
      Regex regex = new Regex("");

      string json = JsonConvert.SerializeObject(new RegexTestClass { Regex = regex }, Formatting.Indented, new RegexConverter());

      Assert.AreEqual(@"{
  ""Regex"": {
    ""Pattern"": """",
    ""Options"": 0
  }
}", json);

      RegexTestClass newRegex = JsonConvert.DeserializeObject<RegexTestClass>(json, new RegexConverter());
      Assert.AreEqual("", newRegex.Regex.ToString());
      Assert.AreEqual(RegexOptions.None, newRegex.Regex.Options);
    }
  }
}