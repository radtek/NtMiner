﻿using NTMiner.AppSetting;
using NTMiner.ServiceContracts;
using NTMiner;
using System;
using System.Linq;

namespace NTMiner.Services {
    public class AppSettingServiceImpl : IAppSettingService {
        public GetAppSettingResponse GetAppSetting(Guid messageId, string key) {
            try {
                IAppSetting data = HostRoot.Current.AppSettingSet.GetAppSetting(key);
                return GetAppSettingResponse.Ok(messageId, AppSettingData.Create(data));
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingResponse>(messageId, e.Message);
            }
        }

        public GetAppSettingsResponse GetAppSettings(Guid messageId, string[] keys) {
            try {
                var data = HostRoot.Current.AppSettingSet.GetAppSettings(keys);
                return GetAppSettingsResponse.Ok(messageId, data.Select(a => AppSettingData.Create(a)).ToList());
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError<GetAppSettingsResponse>(messageId, e.Message);
            }
        }

        public ResponseBase SetAppSetting(SetAppSettingRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                Global.Execute(new SetAppSettingCommand(request.Data));
                Global.WriteDevLine($"{request.Data.Key} {request.Data.Value}");
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        public void Dispose() {
            // nothing need to do
        }
    }
}
