using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{   
    public class GenerateCode
    {
        public static string GenerateRoleCode(string role)
        {
            string codePrefix = "VN";
            string roleCodePrefix = "";
            string year = DateTime.Now.Year.ToString();

            switch (role.ToLower())
            {
                case "customer":
                    roleCodePrefix = "CUS";
                    break;
                case "admin":
                    roleCodePrefix = "ADM";
                    break;
                case "partner":
                    roleCodePrefix = "PAR";
                    break;
                default:
                    throw new ArgumentException("Invalid role");
            }

            codePrefix += roleCodePrefix + year.Substring(2, 2);

            return $"{codePrefix}{Guid.NewGuid().ToString("N").Substring(0, 5)}";
        }

        public static string GenerateTableCode(string tableName)
        {
            string codePrefix = "VN";
            string year = DateTime.Now.Year.ToString();
            string tableCodePrefix = "";

            switch (tableName.ToLower())
            {
                case "schedule":
                    tableCodePrefix = "SCH";
                    break;
                case "partnertype":
                    tableCodePrefix = "PTP";
                    break;
                case "servicecategory":
                    tableCodePrefix = "SCC";
                    break;
                case "booking":
                    tableCodePrefix = "BOK";
                    break;
                case "paymentmethod":
                    tableCodePrefix = "PMT";
                    break;
                case "address":
                    tableCodePrefix = "ADR";
                    break;
                default:
                    throw new ArgumentException("Invalid table name");
            }

            codePrefix += tableCodePrefix + year.Substring(2, 2);

            return $"{codePrefix}{Guid.NewGuid().ToString("N").Substring(0, 5)}";
        }
    }
}
