using System.Collections.Generic;

namespace Newtonsoft.Json4.Tests.TestObjects
{
  public class Container
  {
    public IList<Product> In { get; set; }
    public IList<Product> Out { get; set; }
  }
}