using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

//     MaxNetTcpBinding is a derived class of NetTcpBinding that size all the message and reader quotas to int.MaxValue
//     MaxChannelFactory<T> is a derived class of ChannelFactory<T> that sets the MaxItemsInObjectGraph to int.MaxValue for each endpoint operation

/*****************************************************************************/
/****                        Sample Calling Scheme                        ****/
/*****************************************************************************/
//public RackTypes GetRackTypes()
//{
//    var binding = new MaxNetTcpBinding();
//    var uri = new System.Uri("net.tcp://svcpd.cgi.int/E1ReferenceService/E1ReferenceService.ReferenceService.svc");
//    var endpoint = new EndpointAddress(uri, EndpointIdentity.CreateSpnIdentity("CGIServices/SVCPD.cgi.int"));

//    var channelFactory = new MaxChannelFactory<IReferenceService>(binding, endpoint);
//    var client = channelFactory.CreateChannel();

//    return client.GetRackTypes("HRIG", null);
//}
/*****************************************************************************/
/****                                                                     ****/
/*****************************************************************************/

namespace Cardinal.Components.WcfClient
{
    public class MaxChannelFactory<T> : ChannelFactory<T>
    {

        public MaxChannelFactory(Binding binding) :
            base(binding) { }

        public MaxChannelFactory(ServiceEndpoint endpoint) :
            base(endpoint) { }

        public MaxChannelFactory(string endpointConfigurationName) :
            base(endpointConfigurationName) { }

        public MaxChannelFactory(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress) { }

        public MaxChannelFactory(Binding binding, string remoteAddress) :
            base(binding, remoteAddress) { }

        public MaxChannelFactory(string endpointConfigurationName,
            EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress) { }

        protected override void OnOpening()
        {
            foreach (var operation in Endpoint.Contract.Operations)
            {
                var behavior =
                    operation.Behaviors.
                        Find<DataContractSerializerOperationBehavior>();
                if (behavior != null)
                {
                    behavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }
            base.OnOpening();
        }

    }
    /*********************************************************************/
    public class MaxNetTcpBinding : NetTcpBinding
    {
        public MaxNetTcpBinding()
        {
            MaxReceivedMessageSize = int.MaxValue;
            MaxBufferPoolSize = int.MaxValue;
            MaxBufferSize = int.MaxValue;
            ReaderQuotas.MaxDepth = int.MaxValue;
            ReaderQuotas.MaxStringContentLength = int.MaxValue;
            ReaderQuotas.MaxArrayLength = int.MaxValue;
            ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
        }
    }
}

