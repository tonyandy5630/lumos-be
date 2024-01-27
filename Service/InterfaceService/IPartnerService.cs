﻿using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.InterfaceService
{
    public interface IPartnerService
    {
        public Task<ApiResponse<PartnerService?>> GetPartnerServiceDetailAsync(int serviceId);
    }
}
