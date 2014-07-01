using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Validation;
namespace CommonHelper
{
    public sealed class ValidationHelper
    {
        /// <summary>
        /// 验证实体
        /// </summary>
        /// <returns></returns>
        public static string Valid<T>(T model)
        {
            string ValidateMessage = string.Empty;

            var validationResults = Validation.Validate<T>(model);
            if (!validationResults.IsValid)
            {
                foreach (var result in validationResults)
                {
                    ValidateMessage += string.Format("{0}:{1}\n", result.Key, result.Message);
                }
            }

            return ValidateMessage;
        }
    }
}
