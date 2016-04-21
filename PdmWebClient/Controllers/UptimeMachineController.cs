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
    private static int CurrentSystemVersion
    {
      get
      {
        string currentVersionString = System.Configuration.ConfigurationManager.AppSettings["CurrentSystemVersion"];
        int currentVersion;
        if (!int.TryParse(currentVersionString, out currentVersion))
        {
          currentVersion = 1;
        }
        return currentVersion;
      }
    }


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

      var model = new UptimeMachineSetupModel(
        getValuesPollingUrl: basePollingUrl, 
        tagId: hostIP, 
        systemVersion: CurrentSystemVersion,
        versionedCssUrl: this.GenerateVersionedCssUrl(),
        versionedJavaScriptUrl: this.GenerateVersionedJavaScriptUrl());

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
        this.GenerateVersionedCssUrl(),
        this.GenerateVersionedJavaScriptUrl());

      return this.Content(manifestText, Constants.AppCacheManifestContentType, System.Text.Encoding.UTF8);
    }


    private string GenerateVersionedCssUrl()
    {
      return this.GenerateVersionedContentUrl("Content/site.css");
    }


    private string GenerateVersionedJavaScriptUrl()
    {
      return this.GenerateVersionedContentUrl("Client/_generated.js");
    }


    private string GenerateContentUrl(string path)
    {
      return UrlHelper.GenerateContentUrl("~/" + path, this.HttpContext);
    }


    private string GenerateVersionedContentUrl(string path)
    {
      return this.GenerateContentUrl(path + "?v=" + CurrentSystemVersion.ToString());
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