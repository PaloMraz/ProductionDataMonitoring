/// <reference path="../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../scripts/typings/flot/jquery.flot.d.ts" />
var App;
(function (App) {
    var UptimeMachineService = (function () {
        function UptimeMachineService(_getValuesPollingUrl, _tagId, _systemVersion, _displayElement) {
            var _this = this;
            this._getValuesPollingUrl = _getValuesPollingUrl;
            this._tagId = _tagId;
            this._systemVersion = _systemVersion;
            this._displayElement = _displayElement;
            $.ajaxSettings.cache = false;
            this._setIntervalHandle = setInterval(function () { return _this.onTimer(); }, 2000);
        }
        UptimeMachineService.prototype.onTimer = function () {
            var _this = this;
            $.getJSON(this._getValuesPollingUrl + "?tagId=" + this._tagId).then(function (data) { return _this.processData(data); }, function (xhr, textStatus, error) { return _this.processError(xhr, textStatus, error); });
        };
        UptimeMachineService.prototype.processData = function (data) {
            if (data.SystemVersion > this._systemVersion) {
                clearInterval(this._setIntervalHandle);
                this._displayElement.text("Detected new client version " + data.SystemVersion + ", reloading...");
                setInterval(function () { return window.location.reload(true); }, 2000);
            }
            else {
                var currentDate = new Date(data.CurrentTimeUnixMilliseconds);
                var currentEffectivityValue = data.CurrentEffectivity;
                this._displayElement.text("CURRENT DATE: " + currentDate.toLocaleDateString() + ",\n          AND TIME: " + currentDate.toLocaleTimeString() + ",\n          CURRENT VALUE: " + currentEffectivityValue);
            }
        };
        UptimeMachineService.prototype.processError = function (xhr, textStatus, errorThrown) {
            console.log(xhr);
            this._displayElement.text("Communication error: textStatus=" + textStatus + ", error: " + errorThrown + ", retrying...");
        };
        return UptimeMachineService;
    }());
    App.UptimeMachineService = UptimeMachineService;
})(App || (App = {}));
//# sourceMappingURL=_generated.js.map