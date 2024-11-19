﻿using StateManagerLib.StateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib
{
    public interface IStateCollecion:ICollection<IStateBase>
    {
        /// <summary>
        /// プロパティを検索する深さ
        /// </summary>
        int FindPropertyNest { get; set; }
    }
}