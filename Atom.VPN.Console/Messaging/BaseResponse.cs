namespace Atom.VPN.Console
{

    /// <summary>
    /// This is base class for all responses. It has sufficient properties to send all of responses, however if we want to add additional properties/fields
    /// then we can derive a class from it and add additional fields.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Set it to True to show that request has been processed successfully
        /// </summary>
        public bool IsOK { get; set; } = false;

        /// <summary>
        /// Type of Message or Request which was received in Request, return the same in response
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Copy it from request and return the same, it is very important don't miss it
        /// </summary>
        public string RequestId { get; set; }


        /// <summary>
        /// In case of any error this property has error/exception details
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// This property stores the result/return object of any operation/request. It can be null if we don't have result for a request
        /// </summary>
        public object Result { get; set; }

    }





}
