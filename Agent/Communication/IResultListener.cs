﻿using Agent.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Communication
{
    public interface IResultListener
    {
        bool HandleResult(Result result);
    }
}
