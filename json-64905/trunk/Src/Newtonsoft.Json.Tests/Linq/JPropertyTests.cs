﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json4.Linq;
using NUnit.Framework;
using System.IO;

namespace Newtonsoft.Json4.Tests.Linq
{
  public class JPropertyTests : TestFixtureBase
  {
    [Test]
    public void NullValue()
    {
      JProperty p = new JProperty("TestProperty", null);
      Assert.IsNotNull(p.Value);
      Assert.AreEqual(JTokenType.Null, p.Value.Type);
      Assert.AreEqual(p, p.Value.Parent);

      p.Value = null;
      Assert.IsNotNull(p.Value);
      Assert.AreEqual(JTokenType.Null, p.Value.Type);
      Assert.AreEqual(p, p.Value.Parent);
    }

#if !SILVERLIGHT
    [Test]
    public void ListChanged()
    {
      JProperty p = new JProperty("TestProperty", null);
      IBindingList l = p;

      ListChangedType? listChangedType = null;
      int? index = null;

      l.ListChanged += (sender, args) =>
      {
        listChangedType = args.ListChangedType;
        index = args.NewIndex;
      };

      p.Value = 1;

      Assert.AreEqual(ListChangedType.ItemChanged, listChangedType.Value);
      Assert.AreEqual(0, index.Value); 
    }
#endif

    [Test]
    public void IListCount()
    {
      JProperty p = new JProperty("TestProperty", null);
      IList l = p;

      Assert.AreEqual(1, l.Count);
    }

    [Test]
    [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot add or remove items from Newtonsoft.Json4.Linq.JProperty.")]
    public void IListClear()
    {
      JProperty p = new JProperty("TestProperty", null);
      IList l = p;

      l.Clear();
    }

    [Test]
    [ExpectedException(typeof(Exception), ExpectedMessage = "Newtonsoft.Json4.Linq.JProperty cannot have multiple values.")]
    public void IListAdd()
    {
      JProperty p = new JProperty("TestProperty", null);
      IList l = p;

      l.Add(null);
    }

    [Test]
    [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot add or remove items from Newtonsoft.Json4.Linq.JProperty.")]
    public void IListRemove()
    {
      JProperty p = new JProperty("TestProperty", null);
      IList l = p;

      l.Remove(p.Value);
    }

    [Test]
    public void Load()
    {
      JsonReader reader = new JsonTextReader(new StringReader("{'propertyname':['value1']}"));
      reader.Read();

      Assert.AreEqual(JsonToken.StartObject, reader.TokenType);
      reader.Read();

      JProperty property = JProperty.Load(reader);
      Assert.AreEqual("propertyname", property.Name);
      Assert.IsTrue(JToken.DeepEquals(JArray.Parse("['value1']"), property.Value));

      Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

      reader = new JsonTextReader(new StringReader("{'propertyname':null}"));
      reader.Read();

      Assert.AreEqual(JsonToken.StartObject, reader.TokenType);
      reader.Read();

      property = JProperty.Load(reader);
      Assert.AreEqual("propertyname", property.Name);
      Assert.IsTrue(JToken.DeepEquals(new JValue(null, JTokenType.Null), property.Value));

      Assert.AreEqual(JsonToken.EndObject, reader.TokenType);
    }

    [Test]
    public void MultiContentConstructor()
    {
      JProperty p = new JProperty("error", new List<string> { "one", "two" });
      JArray a = (JArray) p.Value;

      Assert.AreEqual(a.Count, 2);
      Assert.AreEqual("one", (string)a[0]);
      Assert.AreEqual("two", (string)a[1]);
    }

    [Test]
    [ExpectedException(typeof(Exception), ExpectedMessage = "Newtonsoft.Json4.Linq.JProperty cannot have multiple values.")]
    public void IListGenericAdd()
    {
      IList<JToken> t = new JProperty("error", new List<string> { "one", "two" });
      t.Add(1);
      t.Add(2);
    }
  }
}
