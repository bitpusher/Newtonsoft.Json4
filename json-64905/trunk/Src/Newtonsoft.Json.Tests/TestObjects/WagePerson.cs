using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json4.Tests.TestObjects
{
  public class WagePerson : Person
  {
    [JsonProperty]
    public decimal HourlyWage { get; set; }
  }
}