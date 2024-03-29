﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json4.Tests.TestObjects
{
  public class HolderClass
  {
    public HolderClass() { }

    [Newtonsoft.Json4.JsonProperty(TypeNameHandling = Newtonsoft.Json4.TypeNameHandling.All)]
    public ContentBaseClass TestMember { get; set; }

    [Newtonsoft.Json4.JsonProperty(TypeNameHandling = Newtonsoft.Json4.TypeNameHandling.All)]
    public Dictionary<int, IList<ContentBaseClass>> AnotherTestMember { get; set; }

    public ContentBaseClass AThirdTestMember { get; set; }

  }
}
