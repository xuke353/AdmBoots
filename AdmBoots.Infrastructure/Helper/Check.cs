using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdmBoots.Infrastructure.Extensions;

namespace AdmBoots.Infrastructure.Helper {
    public static class Check {

        public static T NotNull<T>(T value, string parameterName) {
            if (value == null) {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotNullOrEmpty(string value, string parameterName) {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }


        public static string NotNullOrWhiteSpace(string value, string parameterName) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName) {
            if (value.IsNullOrEmpty()) {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }
    }
}
