using System;
using Gsdk.User;
using Gsdk.Err;
using Grpc.Core;
using Google.Protobuf.Collections;
using Google.Protobuf;
using System.Windows;
using System.Collections.Generic;

namespace Biogenesis_WPF.biostar.services
{
    class UserSvc
    {
        private User.UserClient userClient;

        public UserSvc(Channel channel)
        {
            userClient = new User.UserClient(channel);
        }

        public ByteString GetPINHash(string pin)
        {
            var request = new GetPINHashRequest { PIN = pin };
            GetPINHashResponse response;

            try
            {
                response = userClient.GetPINHash(request);    
            }
            catch (RpcException e)
            {
                throw;
            }

            return response.HashVal;
        }
        public RepeatedField<UserHdr> GetList(uint deviceID)
        {
            var request = new GetListRequest { DeviceID = deviceID };

            try
            {
                var response = userClient.GetList(request);

                return response.Hdrs;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the user list {0}: {1}", deviceID, e);
                throw;
            }
        }

        public RepeatedField<UserInfo> GetUser(uint deviceID, List<string> userIDs)
        {
            var request = new GetRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                var response = userClient.Get(request);

                return response.Users;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the user information {0}: {1}", deviceID, e);
                throw;
            }
        }

        public void Enroll(uint deviceID, UserInfo[] users, bool overwrite=false)
        {
            var request = new EnrollRequest { DeviceID = deviceID };
            request.Users.AddRange(users);
            request.Overwrite = overwrite;

            try
            {
             userClient.Enroll(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot enroll users {0}: {1}", deviceID, e);
                throw;
            }
        }

        public RepeatedField<ErrorResponse> EnrollMulti(uint[] deviceIDs, UserInfo[] users, bool overwrite = false)
        {
            var request = new EnrollMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);
            request.Users.AddRange(users);
            request.Overwrite = overwrite;

            EnrollMultiResponse response = new EnrollMultiResponse();

            try
            {
                response = userClient.EnrollMulti(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Enroll multi errores: {0}", e);
                return response.DeviceErrors;
            }

            return response.DeviceErrors;
        }

        public void Delete(uint deviceID, string[] userIDs)
        {
            var request = new DeleteRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                userClient.Delete(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot delete users {0}: {1}", deviceID, e);
                throw;
            }
        }

        public RepeatedField<ErrorResponse> DeleteMulti(uint[] deviceIDs, string[] userIDs)
        {
            var request = new DeleteMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);
            request.UserIDs.AddRange(userIDs);

            DeleteMultiResponse response = new DeleteMultiResponse();

            try
            {
                response = userClient.DeleteMulti(request);
            }
            catch (RpcException e)
            {
                MessageBox.Show ($"Delete multi errores: {e}");
                return response.DeviceErrors;
            }
            return response.DeviceErrors;
        }

        public void SetCard(uint deviceID, UserCard[] userCards)
        {
            var request = new SetCardRequest { DeviceID = deviceID };
            request.UserCards.AddRange(userCards);

            try
            {
                userClient.SetCard(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set cards {0}: {1}", deviceID, e);
                throw;
            }
        }

        public void SetFinger(uint deviceID, UserFinger[] userFingers)
        {
            var request = new SetFingerRequest { DeviceID = deviceID };
            request.UserFingers.AddRange(userFingers);

            try
            {
                userClient.SetFinger(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set fingers {0}: {1}", deviceID, e);
                throw;
            }
        }

        public void SetFace(uint deviceID, UserFace[] userFaces)
        {
            var request = new SetFaceRequest { DeviceID = deviceID };
            request.UserFaces.AddRange(userFaces);

            try
            {
                userClient.SetFace(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set faces {0}: {1}", deviceID, e);
                throw;
            }
        }

        public void SetAccessGroup(uint deviceID, UserAccessGroup[] groups)
        {
            var request = new SetAccessGroupRequest { DeviceID = deviceID };
            request.UserAccessGroups.AddRange(groups);

            try
            {
                userClient.SetAccessGroup(request);
            }
            catch (RpcException e)
            {
                MessageBox.Show ($"Cannot set access groups {deviceID}: {e}");
                throw;
            }
        }

        public RepeatedField<UserAccessGroup> GetAccessGroup(uint deviceID, List<string> userIDs)
        {
            var request = new GetAccessGroupRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                var response = userClient.GetAccessGroup(request);
                return response.UserAccessGroups;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get access groups {0}: {1}", deviceID, e);
                throw;
            }
        }
    }
}