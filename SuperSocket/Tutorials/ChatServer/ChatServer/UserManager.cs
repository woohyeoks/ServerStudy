using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    public class UserManager
    {
        int MaxUserCount;
        UInt64 UserSequenceNumber = 0;

        public void Init(int maxUserCount)
        {
            MaxUserCount = maxUserCount;
        }
    }
}
