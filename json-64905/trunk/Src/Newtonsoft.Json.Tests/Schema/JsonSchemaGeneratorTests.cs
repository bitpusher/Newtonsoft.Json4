#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json4.Converters;
using Newtonsoft.Json4.Serialization;
using Newtonsoft.Json4.Tests.TestObjects;
using Newtonsoft.Json4.Utilities;
using NUnit.Framework;
using Newtonsoft.Json4.Schema;
using System.IO;
using System.Linq;
using Newtonsoft.Json4.Linq;
using System.Text;
using Extensions=Newtonsoft.Json4.Schema.Extensions;

namespace Newtonsoft.Json4.Tests.Schema
{
  public class JsonSchemaGeneratorTests : TestFixtureBase
  {
    [Test]
    public void Generate_GenericDictionary()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof (Dictionary<string, List<string>>));

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""type"": ""object"",
  ""additionalProperties"": {
    ""type"": [
      ""array"",
      ""null""
    ],
    ""items"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    }
  }
}", json);

      Dictionary<string, List<string>> value = new Dictionary<string, List<string>>
                                                 {
                                                   {"HasValue", new List<string>() { "first", "second", null }},
                                                   {"NoValue", null}
                                                 };

      string valueJson = JsonConvert.SerializeObject(value, Formatting.Indented);
      JObject o = JObject.Parse(valueJson);

      Assert.IsTrue(o.IsValid(schema));
    }

#if !PocketPC
    [Test]
    public void Generate_DefaultValueAttributeTestClass()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(DefaultValueAttributeTestClass));

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""description"": ""DefaultValueAttributeTestClass description!"",
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""TestField1"": {
      ""required"": true,
      ""type"": ""integer"",
      ""default"": 21
    },
    ""TestProperty1"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""TestProperty1Value""
    }
  }
}", json);
    }
#endif

    [Test]
    public void Generate_Person()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(Person));

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""id"": ""Person"",
  ""title"": ""Title!"",
  ""description"": ""JsonObjectAttribute description!"",
  ""type"": ""object"",
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""BirthDate"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastModified"": {
      ""required"": true,
      ""type"": ""string""
    }
  }
}", json);
    }

    [Test]
    public void Generate_UserNullable()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(UserNullable));

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""Id"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""FName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""LName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""RoleId"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""NullableRoleId"": {
      ""required"": true,
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""NullRoleId"": {
      ""required"": true,
      ""type"": [
        ""integer"",
        ""null""
      ]
    },
    ""Active"": {
      ""required"": true,
      ""type"": [
        ""boolean"",
        ""null""
      ]
    }
  }
}", json);
    }

    [Test]
    public void Generate_RequiredMembersClass()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(RequiredMembersClass));

      Assert.AreEqual(JsonSchemaType.String, schema.Properties["FirstName"].Type);
      Assert.AreEqual(JsonSchemaType.String | JsonSchemaType.Null, schema.Properties["MiddleName"].Type);
      Assert.AreEqual(JsonSchemaType.String | JsonSchemaType.Null, schema.Properties["LastName"].Type);
      Assert.AreEqual(JsonSchemaType.String, schema.Properties["BirthDate"].Type);
    }

    [Test]
    public void Generate_Store()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(Store));

      Assert.AreEqual(11, schema.Properties.Count);

      JsonSchema productArraySchema = schema.Properties["product"];
      JsonSchema productSchema = productArraySchema.Items[0];

      Assert.AreEqual(4, productSchema.Properties.Count);
    }

    [Test]
    public void MissingSchemaIdHandlingTest()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();

      JsonSchema schema = generator.Generate(typeof(Store));
      Assert.AreEqual(null, schema.Id);

      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      schema = generator.Generate(typeof (Store));
      Assert.AreEqual(typeof(Store).FullName, schema.Id);

      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseAssemblyQualifiedName;
      schema = generator.Generate(typeof(Store));
      Assert.AreEqual(typeof(Store).AssemblyQualifiedName, schema.Id);
    }

    [Test]
    public void Generate_NumberFormatInfo()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      JsonSchema schema = generator.Generate(typeof(NumberFormatInfo));

      string json = schema.ToString();

      Console.WriteLine(json);

      //      Assert.AreEqual(@"{
      //  ""type"": ""object"",
      //  ""additionalProperties"": {
      //    ""type"": ""array"",
      //    ""items"": {
      //      ""type"": ""string""
      //    }
      //  }
      //}", json);
    }

    [Test]
    [ExpectedException(typeof(Exception), ExpectedMessage = @"Unresolved circular reference for type 'Newtonsoft.Json4.Tests.TestObjects.CircularReferenceClass'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.")]
    public void CircularReferenceError()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.Generate(typeof(CircularReferenceClass));
    }

    [Test]
    public void CircularReferenceWithTypeNameId()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;

      JsonSchema schema = generator.Generate(typeof(CircularReferenceClass), true);

      Assert.AreEqual(JsonSchemaType.String, schema.Properties["Name"].Type);
      Assert.AreEqual(typeof(CircularReferenceClass).FullName, schema.Id);
      Assert.AreEqual(JsonSchemaType.Object | JsonSchemaType.Null, schema.Properties["Child"].Type);
      Assert.AreEqual(schema, schema.Properties["Child"]);
    }

    [Test]
    public void CircularReferenceWithExplicitId()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();

      JsonSchema schema = generator.Generate(typeof(CircularReferenceWithIdClass));

      Assert.AreEqual(JsonSchemaType.String | JsonSchemaType.Null, schema.Properties["Name"].Type);
      Assert.AreEqual("MyExplicitId", schema.Id);
      Assert.AreEqual(JsonSchemaType.Object | JsonSchemaType.Null, schema.Properties["Child"].Type);
      Assert.AreEqual(schema, schema.Properties["Child"]);
    }

    [Test]
    public void GenerateSchemaForType()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;

      JsonSchema schema = generator.Generate(typeof(Type));

      Assert.AreEqual(JsonSchemaType.String, schema.Type);

      string json = JsonConvert.SerializeObject(typeof(Version), Formatting.Indented);

      JValue v = new JValue(json);
      Assert.IsTrue(v.IsValid(schema));
    }

#if !SILVERLIGHT && !PocketPC
    [Test]
    public void GenerateSchemaForISerializable()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;

      JsonSchema schema = generator.Generate(typeof(Exception));

      Assert.AreEqual(JsonSchemaType.Object, schema.Type);
      Assert.AreEqual(true, schema.AllowAdditionalProperties);
      Assert.AreEqual(null, schema.Properties);
    }
#endif

    [Test]
    public void GenerateSchemaForDBNull()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;

      JsonSchema schema = generator.Generate(typeof(DBNull));

      Assert.AreEqual(JsonSchemaType.Null, schema.Type);
    }

    public class CustomDirectoryInfoMapper : DefaultContractResolver
    {
      public CustomDirectoryInfoMapper()
        : base(true)
      {
      }

      protected override JsonContract CreateContract(Type objectType)
      {
        if (objectType == typeof(DirectoryInfo))
          return base.CreateObjectContract(objectType);

        return base.CreateContract(objectType);
      }

      protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
      {
        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

        JsonPropertyCollection c = new JsonPropertyCollection(type);
        CollectionUtils.AddRange(c, (IEnumerable)properties.Where(m => m.PropertyName != "Root"));

        return c;
      }
    }

    [Test]
    public void GenerateSchemaForDirectoryInfo()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      generator.ContractResolver = new CustomDirectoryInfoMapper();

      JsonSchema schema = generator.Generate(typeof(DirectoryInfo), true);

      string json = schema.ToString();
      
      Assert.AreEqual(@"{
  ""id"": ""System.IO.DirectoryInfo"",
  ""required"": true,
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Parent"": {
      ""$ref"": ""System.IO.DirectoryInfo""
    },
    ""Exists"": {
      ""required"": true,
      ""type"": ""boolean""
    },
    ""FullName"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""Extension"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""CreationTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""CreationTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastAccessTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastAccessTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastWriteTime"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""LastWriteTimeUtc"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""Attributes"": {
      ""required"": true,
      ""type"": ""integer""
    }
  }
}", json);

      DirectoryInfo temp = new DirectoryInfo(@"c:\temp");

      JTokenWriter jsonWriter = new JTokenWriter();
      JsonSerializer serializer = new JsonSerializer();
      serializer.Converters.Add(new IsoDateTimeConverter());
      serializer.ContractResolver = new CustomDirectoryInfoMapper();
      serializer.Serialize(jsonWriter, temp);

      List<string> errors = new List<string>();
      jsonWriter.Token.Validate(schema, (sender, args) => errors.Add(args.Message));

      Assert.AreEqual(0, errors.Count);
    }

    [Test]
    public void GenerateSchemaCamelCase()
    {
      JsonSchemaGenerator generator = new JsonSchemaGenerator();
      generator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      generator.ContractResolver = new CamelCasePropertyNamesContractResolver();

      JsonSchema schema = generator.Generate(typeof (Version), true);

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""id"": ""System.Version"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""additionalProperties"": false,
  ""properties"": {
    ""major"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""minor"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""build"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""revision"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""majorRevision"": {
      ""required"": true,
      ""type"": ""integer""
    },
    ""minorRevision"": {
      ""required"": true,
      ""type"": ""integer""
    }
  }
}", json);
    }

    public enum SortTypeFlag
    {
      No = 0,
      Asc = 1,
      Desc = -1
    }

    public class X
    {
      public SortTypeFlag x;
    }

    [Test]
    public void GenerateSchemaWithNegativeEnum()
    {
      JsonSchemaGenerator jsonSchemaGenerator = new JsonSchemaGenerator();
      JsonSchema schema = jsonSchemaGenerator.Generate(typeof(X));

      string json = schema.ToString();

      Assert.AreEqual(@"{
  ""type"": ""object"",
  ""properties"": {
    ""x"": {
      ""required"": true,
      ""type"": ""integer"",
      ""enum"": [
        0,
        1,
        -1
      ],
      ""options"": [
        {
          ""value"": 0,
          ""label"": ""No""
        },
        {
          ""value"": 1,
          ""label"": ""Asc""
        },
        {
          ""value"": -1,
          ""label"": ""Desc""
        }
      ]
    }
  }
}", json);
    }

    [Test]
    public void CircularCollectionReferences()
    {
      Type type = typeof (Workspace);
      JsonSchemaGenerator jsonSchemaGenerator = new JsonSchemaGenerator();

      jsonSchemaGenerator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      JsonSchema jsonSchema = jsonSchemaGenerator.Generate(type);

      // should succeed
      Assert.IsNotNull(jsonSchema);
    }

    [Test]
    public void CircularReferenceWithMixedRequires()
    {
      JsonSchemaGenerator jsonSchemaGenerator = new JsonSchemaGenerator();

      jsonSchemaGenerator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      JsonSchema jsonSchema = jsonSchemaGenerator.Generate(typeof(CircularReferenceClass));
      string json = jsonSchema.ToString();

      Assert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json4.Tests.TestObjects.CircularReferenceClass"",
  ""type"": [
    ""object"",
    ""null""
  ],
  ""properties"": {
    ""Name"": {
      ""required"": true,
      ""type"": ""string""
    },
    ""Child"": {
      ""$ref"": ""Newtonsoft.Json4.Tests.TestObjects.CircularReferenceClass""
    }
  }
}", json);
    }

    [Test]
    public void JsonPropertyWithHandlingValues()
    {
      JsonSchemaGenerator jsonSchemaGenerator = new JsonSchemaGenerator();

      jsonSchemaGenerator.UndefinedSchemaIdHandling = UndefinedSchemaIdHandling.UseTypeName;
      JsonSchema jsonSchema = jsonSchemaGenerator.Generate(typeof(JsonPropertyWithHandlingValues));
      string json = jsonSchema.ToString();

      Assert.AreEqual(@"{
  ""id"": ""Newtonsoft.Json4.Tests.TestObjects.JsonPropertyWithHandlingValues"",
  ""required"": true,
  ""type"": [
    ""object"",
    ""null""
  ],
  ""properties"": {
    ""DefaultValueHandlingIgnoreProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingIncludeProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingPopulateProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""DefaultValueHandlingIgnoreAndPopulateProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ],
      ""default"": ""Default!""
    },
    ""NullValueHandlingIgnoreProperty"": {
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""NullValueHandlingIncludeProperty"": {
      ""required"": true,
      ""type"": [
        ""string"",
        ""null""
      ]
    },
    ""ReferenceLoopHandlingErrorProperty"": {
      ""$ref"": ""Newtonsoft.Json4.Tests.TestObjects.JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingIgnoreProperty"": {
      ""$ref"": ""Newtonsoft.Json4.Tests.TestObjects.JsonPropertyWithHandlingValues""
    },
    ""ReferenceLoopHandlingSerializeProperty"": {
      ""$ref"": ""Newtonsoft.Json4.Tests.TestObjects.JsonPropertyWithHandlingValues""
    }
  }
}", json);
    }
  }

  public class DMDSLBase
  {
    public String Comment;
  }

  public class Workspace : DMDSLBase
  {
    public ControlFlowItemCollection Jobs = new ControlFlowItemCollection();
  }

  public class ControlFlowItemBase : DMDSLBase
  {
    public String Name;
  }

  public class ControlFlowItem : ControlFlowItemBase//A Job
  {
    public TaskCollection Tasks = new TaskCollection();
    public ContainerCollection Containers = new ContainerCollection();
  }

  public class ControlFlowItemCollection : List<ControlFlowItem>
  {
  }

  public class Task : ControlFlowItemBase
  {
    public DataFlowTaskCollection DataFlowTasks = new DataFlowTaskCollection();
    public BulkInsertTaskCollection BulkInsertTask = new BulkInsertTaskCollection();
  }

  public class TaskCollection : List<Task>
  {
  }

  public class Container : ControlFlowItemBase
  {
    public ControlFlowItemCollection ContainerJobs = new ControlFlowItemCollection();
  }

  public class ContainerCollection : List<Container>
  {
  }

  public class DataFlowTask_DSL : ControlFlowItemBase
  {
  }

  public class DataFlowTaskCollection : List<DataFlowTask_DSL>
  {
  }

  public class SequenceContainer_DSL : Container
  {
  }

  public class BulkInsertTaskCollection : List<BulkInsertTask_DSL>
  {
  }

  public class BulkInsertTask_DSL
  {
  }
}