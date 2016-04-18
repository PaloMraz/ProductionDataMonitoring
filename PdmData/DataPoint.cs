using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdmData
{
  public class DataPoint<T>
  {
    public DateTimeOffset Time { get; set; }
    public T Value { get; set; }
  }
}
