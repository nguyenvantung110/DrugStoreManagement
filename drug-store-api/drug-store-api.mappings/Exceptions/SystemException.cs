namespace drug_store_api.mappings.Exceptions
{
    public class SystemException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemException"/> class.
        /// </summary>
        /// <param name="exceptionId">Id of exeption.</param>
        /// <param name="param">params for error message.</param>
        public SystemException(string exceptionId, params object[]? param)
            : base(exceptionId, param)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemException"/> class.
        /// </summary>
        /// <param name="exceptionId">Id of exeption.</param>
        /// <param name="innerException">InnerExeption.</param>
        /// <param name="param">params for error message.</param>
        public SystemException(string exceptionId, Exception innerException, params object[]? param)
            : base(exceptionId, innerException, param)
        {
        }
    }
}
