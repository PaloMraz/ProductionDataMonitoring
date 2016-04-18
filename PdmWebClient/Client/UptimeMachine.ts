/// <reference path="../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../scripts/typings/flot/jquery.flot.d.ts" />
module App {

  export class UptimeMachineService {
    private _setIntervalHandle: number;

    public constructor(
      private _getValuesPollingUrl: string,
      private _tagId: string,
      private _systemVersion: number,
      private _displayElement: JQuery) {

      $.ajaxSettings.cache = false;

      this._setIntervalHandle = setInterval(() => this.onTimer(), 5000);
    }


    private onTimer() {
      $.getJSON(this._getValuesPollingUrl + "?tagId=" + this._tagId).then(
        (data) => this.processData(data),
        (xhr: JQueryXHR, textStatus: string, error: any) => this.processError(xhr, textStatus, error));
    }


    private processData(data: any) {
      if (data.SystemVersion > this._systemVersion) {
        clearInterval(this._setIntervalHandle);
        this._displayElement.text(`Detected new client version ${data.SystemVersion}, reloading...`);
        setInterval(() => window.location.reload(true), 2000);
      } else {
        let currentDate = new Date(data.CurrentTimeUnixMilliseconds);
        let currentEffectivityValue: number = data.CurrentEffectivity;
        this._displayElement.text(
          `Current Date: ${currentDate.toLocaleDateString()},
          time: ${currentDate.toLocaleTimeString()},
          Value: ${currentEffectivityValue}`);
      }
    }


    private processError(xhr: JQueryXHR, textStatus: string, errorThrown: any) {
      console.log(xhr);
      this._displayElement.text(`Communication error: textStatus=${textStatus}, error: ${errorThrown}, retrying...`);
    }
  }
}