using PdmData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdmContracts
{
  public interface IUptimeMachineDataRepository
  {
    Task<UptimeMachineData> GetUptimeMachineDataAsync(string tagId); 
  }
}
