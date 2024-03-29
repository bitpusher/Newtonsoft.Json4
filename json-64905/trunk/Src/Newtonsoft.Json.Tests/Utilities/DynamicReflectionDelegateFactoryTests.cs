﻿#region License
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

#if !PocketPC && !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Newtonsoft.Json4.Tests.Linq;
using NUnit.Framework;
using Newtonsoft.Json4.Utilities;
using Newtonsoft.Json4.Tests.TestObjects;
using Newtonsoft.Json4.Tests.Serialization;

namespace Newtonsoft.Json4.Tests.Utilities
{
  public class DynamicReflectionDelegateFactoryTests : TestFixtureBase
  {
    [Test]
    [ExpectedException(typeof(InvalidCastException), ExpectedMessage = "Unable to cast object of type 'Newtonsoft.Json4.Tests.TestObjects.Person' to type 'Newtonsoft.Json4.Tests.TestObjects.Movie'.")]
    public void CreateGetWithBadObjectTarget()
    {
      Person p = new Person();
      p.Name = "Hi";

      Func<object, object> setter = DynamicReflectionDelegateFactory.Instance.CreateGet<object>(typeof(Movie).GetProperty("Name"));

      setter(p);
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException), ExpectedMessage = "Unable to cast object of type 'Newtonsoft.Json4.Tests.TestObjects.Person' to type 'Newtonsoft.Json4.Tests.TestObjects.Movie'.")]
    public void CreateSetWithBadObjectTarget()
    {
      Person p = new Person();
      Movie m = new Movie();

      Action<object, object> setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(typeof(Movie).GetProperty("Name"));

      setter(m, "Hi");

      Assert.AreEqual(m.Name, "Hi");

      setter(p, "Hi");
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException), ExpectedMessage = "Specified cast is not valid.")]
    public void CreateSetWithBadTarget()
    {
      object structTest = new StructTest();

      Action<object, object> setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(typeof(StructTest).GetProperty("StringProperty"));

      setter(structTest, "Hi");

      Assert.AreEqual("Hi", ((StructTest)structTest).StringProperty);

      setter(new TimeSpan(), "Hi");
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException), ExpectedMessage = "Unable to cast object of type 'System.Version' to type 'System.String'.")]
    public void CreateSetWithBadObjectValue()
    {
      Movie m = new Movie();

      Action<object, object> setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(typeof(Movie).GetProperty("Name"));

      setter(m, new Version());
    }

    [Test]
    public void CreateStaticMethodCall()
    {
      MethodInfo castMethodInfo = typeof(JsonSerializerTest.DictionaryKey).GetMethod("op_Implicit", new[] { typeof(string) });

      Assert.IsNotNull(castMethodInfo);

      MethodCall<object, object> call = DynamicReflectionDelegateFactory.Instance.CreateMethodCall<object>(castMethodInfo);

      object result = call(null, "First!");
      Assert.IsNotNull(result);

      JsonSerializerTest.DictionaryKey key = (JsonSerializerTest.DictionaryKey) result;
      Assert.AreEqual("First!", key.Value);
    }

    //[Test]
    //public void sdfsdf()
    //{
    //  string name = "MyAssembly";
    //  string filename = name + ".dll";

    //  AssemblyName s = new AssemblyName(name);

    //  AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(s, AssemblyBuilderAccess.RunAndSave);
    //  ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(filename, filename);
    //  TypeBuilder typeBuilder = moduleBuilder.DefineType("MyType", TypeAttributes.Class, typeof(object));
    //  MethodBuilder methodBuilder = typeBuilder.DefineMethod("TestSet", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new[] { typeof(object), typeof(object) });

    //  DynamicReflectionDelegateFactory.GenerateCreateSetFieldIL(typeof(ClassWithGuid).GetField("GuidField"), methodBuilder.GetILGenerator());
    //  typeBuilder.CreateType();
    //  assemblyBuilder.Save(filename);
    //}

    //[Test]
    //public void sdfsdf1()
    //{
    //  string name = "MyAssembly1";
    //  string filename = name + ".dll";

    //  AssemblyName s = new AssemblyName(name);

    //  AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(s, AssemblyBuilderAccess.RunAndSave);
    //  ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(filename, filename);
    //  TypeBuilder typeBuilder = moduleBuilder.DefineType("MyType", TypeAttributes.Class, typeof(object));
    //  MethodBuilder methodBuilder = typeBuilder.DefineMethod("TestSet", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new[] { typeof(object), typeof(object) });

    //  DynamicReflectionDelegateFactory.GenerateCreateSetPropertyIL(typeof(Car).GetProperty("Model"), methodBuilder.GetILGenerator());
    //  typeBuilder.CreateType();
    //  assemblyBuilder.Save(filename);
    //}
  }
}
#endif