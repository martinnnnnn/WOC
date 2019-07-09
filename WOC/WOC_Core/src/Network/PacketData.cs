using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WOC_Battle;

namespace WOC_Core
{
    public abstract class IPacketData { public Guid id = Guid.NewGuid(); }

    public class PD_Validate : IPacketData
    {
        public Guid validationId;
        public bool isValid;
        public string errorMessage;

        public PD_Validate()
        {
        }

        public PD_Validate(Guid valId, string message = "")
        {
            validationId = valId;
            isValid = string.IsNullOrEmpty(message);
            errorMessage = message;
        }

        public PD_Validate(Guid valId, bool valid, string message = "")
        {
            isValid = valid;
            errorMessage = message;
        }
    }

    public class PD_Shutdown : IPacketData {}

    public class PD_Chat : IPacketData
    {
        public string senderName;
        public string message;
    }

    public class PD_PlayerAdd : IPacketData
    {
        
    }
}
