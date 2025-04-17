using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.UI.Messaging
{
    public class StatusUpdateMessage : ValueChangedMessage<string>
    {
        public StatusUpdateMessage(string message) : base(message) { }

    }
}
