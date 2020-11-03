﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    public class PKHandler
    {
        protected MainServer ServerNetwork;
        protected UserManager UserMgr = null;

        public void Init(MainServer serverNetwork, UserManager userMgr)
        {
            ServerNetwork = serverNetwork;
            UserMgr = userMgr;
        }
    }
}
