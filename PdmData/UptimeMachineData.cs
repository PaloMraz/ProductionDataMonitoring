using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdmData
{
  public class UptimeMachineData
  {
    public string TagId { get; set; }
    public DateTimeOffset CurrentTime { get; set; }
    public float CurrentEffectivity { get; set; }
    public IEnumerable<DataPoint<float>> EffectivityHistory { get; set; }
  }
}