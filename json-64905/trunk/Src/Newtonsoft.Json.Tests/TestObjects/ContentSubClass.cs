using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json4.Tests.TestObjects
{
  public class ContentSubClass : ContentBaseClass
  {
    public ContentSubClass() { }
    public ContentSubClass(string EasyIn)
    {
      SomeString = EasyIn;
    }

    public string SomeString { get; set; }
  }
}
