﻿using System.Collections.Generic;

namespace ClientCore.CnCNet5
{
    public class PrivateMessageInfo
    {
        public string UserName { get; set; }
        public List<MessageInfo> Messages = new List<MessageInfo>();
        public List<bool> IsSelfSent = new List<bool>();
    }
}
