﻿using BussinessObject;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class HistoryLogRepo:IHistoryLogRepo
    {
        public HistoryLogRepo(LumosDBContext context) { }
    }
}
