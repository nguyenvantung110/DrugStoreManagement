using System.Text;

namespace drug_store_api.mappings.Exceptions
{
    public class BaseException : ApplicationException
    {
        /// <summary>
        /// ExceptionId
        /// </summary>
        public string? ExceptionId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public new string? Message { get; set; }

        /// <summary>
        /// Params
        /// </summary>
        public object[]? Params { get; set; }

        /// <summary>
        /// Inner Exeption
        /// </summary>
        public new Exception? InnerException { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class
        /// </summary>
        public BaseException()
        {
            this.ExceptionId = string.Empty;
            this.Message = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="exceptionId">例外ID.</param>
        /// <param name="param"> message param. </param>
        public BaseException(string exceptionId, params object[]? param)
        {
            this.ExceptionId = exceptionId;
            this.Params = param == null || param.Length == 0 ? null : param;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="exceptionId">Id of exception.</param>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="param">params for error message.</param>
        public BaseException(string exceptionId, Exception innerException, params object[]? param)
        {
            this.ExceptionId = exceptionId;
            this.InnerException = innerException;
            this.Params = param == null || param.Length == 0 ? null : param;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            StringBuilder strMessage = new();
            Exception? innerException = InnerException;
            for (int i = 0; i < 5; i++)
            {
                if (innerException != null)
                {
                    strMessage.AppendLine(innerException.ToString());
                    strMessage.Append(Environment.NewLine);
                    innerException = innerException.InnerException;
                }
                else
                {
                    break;
                }
            }

            return strMessage.ToString();
        }
    }
}
