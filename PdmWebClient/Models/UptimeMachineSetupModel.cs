using PdmData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PdmWebClient.Models
{
  public class UptimeMachineSetupModel
  {

    public UptimeMachineSetupModel(string getValuesPollingUrl, string tagId, int systemVersion)
    {
      this.GetValuesPollingUrl = getValuesPollingUrl;
      this.TagId = tagId;
      this.SystemVersion = systemVersion;
    }


    public string TagId { get; private set; }


    public string GetValuesPollingUrl { get; private set; }

    public int SystemVersion { get; private set; }
  }
}