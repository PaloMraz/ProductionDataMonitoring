using PdmData;
using PdmWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PdmWebClient.Controllers
{
  [RoutePrefix(Constants.RoutePrefix)]
  public class UptimeMachineController : Controller
  {
    // Toto sa inkrementuje vždy, keď sa mení logika či už na klientovi (Client/UptimeMachine.ts)
    // alebo na serveri.
    private const int CurrentSystemVersion = 20;


    /// <summary>
    /// Na toto URL pôjde browser po štarte a na toto sa vykonáva aj auto-refresh z klienta pri zistení
    /// vyššej verzie.
    /// </summary>
    [Route(Constants.IndexRoute), HttpGet()]
    public async Task<ActionResult> IndexAsync()
    {
      await Task.Delay(1); // zatiaľ nemáme async calls tak to emulujeme takto...

      // Model pre parametrizáciu klientskeho skriptu.
      string basePollingUrl = this.GenerateContentUrl($"{Constants.RoutePrefix}/{Constants.GetValuesRoute}/");
      string hostIP = this.Request.UserHostAddress;
      string hostName = this.Request.UserHostName;

      var model = new UptimeMachineSetupModel(getValuesPollingUrl: basePollingUrl, tagId: hostIP, systemVersion: CurrentSystemVersion);
      return this.View("Index", model);
    }


    /// <summary>
    /// Toto volá klient periodicky cez Ajax.
    /// </summary>
    [Route(Constants.GetValuesRoute)]
    public async Task<ActionResult> GetValuesAsync(string tagId)
    {
      await Task.Delay(1); // zatiaľ nemáme async calls tak to emulujeme takto...

      // Vygenerujeme si nejaké random údaje - v reále to budeme čítať z DB.
      var random = new Random();
      const int DataPointCount = 50;
      const float MaxDataPointValue = 1000;
      var dataPoints = Enumerable.Range(0, DataPointCount - 1)
        .Select(i => new DataPoint<float>() { Time = DateTimeOffset.Now.AddMinutes(i - DataPointCount), Value = (float)(random.NextDouble() * MaxDataPointValue) });

      var data = new UptimeMachineData()
      {
        CurrentEffectivity = dataPoints.Last().Value,
        CurrentTime = DateTimeOffset.Now,
        TagId = tagId,
        EffectivityHistory = dataPoints
      };
      var model = new UptimeMachineDataModel(data: data, systemVersion: CurrentSystemVersion);
      
      // Vraciame explicitne JSON.
      return this.Json(model, JsonRequestBehavior.AllowGet);
    }


    /// <summary>
    /// Táto action metóda generuje AppCache manifest - FMI: https://developer.mozilla.org/en-US/docs/Web/HTML/Using_the_application_cache
    /// </summary>
    [Route(Constants.GetAppCacheManifestRoute), HttpGet()]
    public ActionResult GetAppCacheManifest()
    {  
      string manifestText = string.Format(
@"CACHE MANIFEST
# SystemVersion {0} - forces refreshing the app cache 
{1}
{2}
{3}

NETWORK:
*
",
        
        CurrentSystemVersion,
        this.GenerateContentUrl("bundles/jquery"), 
        this.GenerateContentUrl("Client/_generated.js?v=" + CurrentSystemVersion.ToString()), 
        this.GenerateContentUrl("Content/css"));

      return this.Content(manifestText, Constants.AppCacheManifestContentType, System.Text.Encoding.UTF8);
    }


    private string GenerateContentUrl(string path)
    {
      return UrlHelper.GenerateContentUrl("~/" + path, this.HttpContext);
    }


    private static class Constants
    {
      public const string RoutePrefix = "uptime-machine";
      public const string IndexRoute = "index";
      public const string GetValuesRoute = "get-values";
      public const string GetAppCacheManifestRoute = "app-cache-manifest";

      public const string AppCacheManifestContentType = "text/cache-manifest";
    }

  }
}