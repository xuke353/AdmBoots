using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Infrastructure.CustomExceptions {

    [Serializable]
    public class AdmDbConcurrencyException : Exception {

        public AdmDbConcurrencyException() {
        }

        public AdmDbConcurrencyException(string message)
            : base(message) {
        }

        public AdmDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
