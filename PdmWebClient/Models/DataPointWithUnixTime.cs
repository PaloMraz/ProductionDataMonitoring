using PdmData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdmWebClient.Models
{
  public class DataPointWithUnixTime<T> : DataPoint<T>
  {
    public DataPointWithUnixTime()
    { }


    public DataPointWithUnixTime(DataPoint<T> other)
    {
      this.Time = other.Time;
      this.Value = other.Value;
    }


    public long TimeUnixMilliseconds { get { return this.Time.ToUnixTimeMilliseconds(); } }
  }
}