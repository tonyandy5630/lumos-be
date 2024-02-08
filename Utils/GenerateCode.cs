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

            return $"{codePrefix}{Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper()}";
        }

        public static string GenerateTableCode(string tableName)
        {
            string codePrefix = "";
            string year = DateTime.Now.Year.ToString();

            switch (tableName.ToLower())
            {
                case "schedule":
                    codePrefix = "SCH";
                    break;
                case "partnertype":
                    codePrefix = "PTP";
                    break;
                case "servicecategory":
                    codePrefix = "SCC";
                    break;
                case "booking":
                    codePrefix = "BOK";
                    break;
                case "paymentmethod":
                    codePrefix = "PMT";
                    break;
                case "address":
                    codePrefix = "ADR";
                    break;
                case "medicalreport":
                    codePrefix = "MDR";
                    break;
                default:
                    throw new ArgumentException("Invalid table name");
            }

            codePrefix += year.Substring(2, 2);

            return $"{codePrefix}{Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper()}";
        }
    }
}
