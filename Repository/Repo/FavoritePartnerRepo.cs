using DataAccessLayer.DBContext;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class FavoritePartnerRepo:IFavoritePartnerRepo
    {
        public FavoritePartnerRepo(LumosDBContext) { }
    }
}
