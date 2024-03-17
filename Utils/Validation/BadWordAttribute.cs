using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Validation
{
    public class BadWordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var badWords = new string[] {
                "sex",
                "xxx",
                "mại dâm",
                "bán dâm",
                "làm tình",
                "quan hệ",
                "làm chuyện ấy",
                "tình dục",
                "nứng",
                "lén lút",
                "nghiện tình dục",
                "dâm ô",
                "dâm dục",
                "khách mại dâm",
                "gái mại dâm",
                "thuê gái",
                "gái bán dâm",
                "cưỡng bức",
                "hiếp dâm",
                "thủ dâm",
                "mát xa",
                "happy ending",
                "bồi bút",
                "thú tính",
                "thú dâm",
                "thú cưng"
            };

            var text = value as string;
            if (text != null)
            {
                text = text.ToLower();
                foreach (var word in badWords)
                {
                    if (text.Contains(word.ToLower()))
                    {
                        return new ValidationResult("The text contains a forbidden word.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
