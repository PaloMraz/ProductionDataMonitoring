using PdmData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PdmWebClient.Models
{
  public class UptimeMachineDataModel
  {

    public UptimeMachineDataModel(UptimeMachineData data, int systemVersion)
    {
      this.SystemVersion = systemVersion;
      this.TagId = data.TagId;
      this.CurrentEffectivity = data.CurrentEffectivity;
      this.CurrentTime = data.CurrentTime;
      this.EffectivityHistory = new List<DataPointWithUnixTime<float>>(
        data.EffectivityHistory.Select(item => new DataPointWithUnixTime<float>(item)));
    }


    public string TagId { get; private set; }


    public DateTimeOffset CurrentTime { get; private set; }


    public long CurrentTimeUnixMilliseconds
    {
      get { return this.CurrentTime.ToUnixTimeMilliseconds(); }
    }


    public float CurrentEffectivity { get; private set; }


    public IEnumerable<DataPointWithUnixTime<float>> EffectivityHistory { get; private set; }


    public int SystemVersion { get; private set; }
  }
}