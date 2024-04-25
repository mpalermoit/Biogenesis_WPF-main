using Gsdk.Time;
using Grpc.Core;
using Google.Protobuf;
using System;
using System.Windows;
using System.Collections.Generic;

namespace Biogenesis_WPF.biostar.services
{
    class TimeSvc
    {
        private Time.TimeClient timeClient;

        public TimeSvc(Channel channel)
        {
            timeClient = new Time.TimeClient(channel);
        }

        public ulong Get(uint deviceID)
        {
            var request = new GetRequest { DeviceID = deviceID };
            var response = timeClient.Get(request);

            return response.GMTTime;
        }

        public TimeConfig GetConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = timeClient.GetConfig(request);

            return response.Config;
        }

        public void Set(uint deviceID, ulong gmtTime)
        {
            var request = new SetRequest { DeviceID = deviceID, GMTTime = gmtTime };
            SetResponse response = new SetResponse();

            try
            {
                response = timeClient.Set(request);
            }
            catch (RpcException e)
            {
                MessageBox.Show("Error: " + e);
                throw;
            }
        }

        public void SetConfig(uint deviceID, TimeConfig timeConfig)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = timeConfig };
            SetConfigResponse response = new SetConfigResponse();

            try
            {
                response = timeClient.SetConfig(request);
            }
            catch (RpcException e)
            {
                MessageBox.Show("Error: " + e);
                throw;
            }
        }

        public void SetMulti(List<uint> deviceIDs, ulong gmtTime)
        {
            var request = new SetMultiRequest { GMTTime = gmtTime };
            request.DeviceIDs.AddRange(deviceIDs);
            SetMultiResponse response = new SetMultiResponse();

            try
            {
                response = timeClient.SetMulti(request);
            }
            catch (RpcException e)
            {

                throw;
            }
        }

        public void SetConfigMulti(List<uint> deviceIDs, TimeConfig timeConfig)
        {
            var request = new SetConfigMultiRequest { Config = timeConfig };
            request.DeviceIDs.AddRange(deviceIDs);
            SetConfigMultiResponse response = new SetConfigMultiResponse();

            try
            {
                response = timeClient.SetConfigMulti(request);
            }
            catch (RpcException e)
            {

                throw;
            }
        }

    }
}