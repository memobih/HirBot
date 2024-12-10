using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ResponseHandler.Consts
{
    public class CommonErrorCodes : IErrorCodes
    {
        public static readonly CommonErrorCodes NULL = new CommonErrorCodes("NULL", CommonErrorCode.NULL);
        public static readonly CommonErrorCodes FORBIDDEN = new CommonErrorCodes("FORBIDDEN", CommonErrorCode.FORBIDDEN);
        public static readonly CommonErrorCodes UN_AUTHORIZED = new CommonErrorCodes("UN_AUTHORIZED", CommonErrorCode.UN_AUTHORIZED);
        public static readonly CommonErrorCodes OPERATION_FAILED = new CommonErrorCodes("OPERATION_FAILED", CommonErrorCode.OPERATION_FAILED);
        public static readonly CommonErrorCodes INVALID_INPUT = new CommonErrorCodes("INVALID_INPUT", CommonErrorCode.INVALID_INPUT);
        public static readonly CommonErrorCodes FAILED_TO_SAVE_DATA = new CommonErrorCodes("FAILED_TO_SAVE_DATA", CommonErrorCode.FAILED_TO_SAVE_DATA);
        public static readonly CommonErrorCodes SERVER_ERROR = new CommonErrorCodes("SERVER_ERROR", CommonErrorCode.SERVER_ERROR);
        public static readonly CommonErrorCodes FAILED_TO_SEND_CODE = new CommonErrorCodes("FAILED_TO_SEND_CODE", CommonErrorCode.FAILED_TO_SEND_CODE);
        public static readonly CommonErrorCodes INVALID_EMAIL_OR_PASSWORD = new CommonErrorCodes("INVALID_EMAIL_OR_PASSWORD", CommonErrorCode.INVALID_EMAIL_OR_PASSWORD);
        public static readonly CommonErrorCodes EMAIL_NOT_CONFIRMED = new CommonErrorCodes("EMAIL_NOT_CONFIRMED", CommonErrorCode.EMAIL_NOT_CONFIRMED);
        public static readonly CommonErrorCodes INVALID_REFRESH_TOKEN = new CommonErrorCodes("INVALID_REFRESH_TOKEN", CommonErrorCode.INVALID_REFRESH_TOKEN);
        public static readonly CommonErrorCodes RESOURCE_NOT_FOUND = new CommonErrorCodes("RESOURCE_NOT_FOUND", CommonErrorCode.RESOURCE_NOT_FOUND);
        public static readonly CommonErrorCodes NotAuthorized = new CommonErrorCodes("NotAuthorized", CommonErrorCode.NotAuthorized);
        public static readonly CommonErrorCodes NOT_FOUND = new CommonErrorCodes("NOT_FOUND", CommonErrorCode.NOT_FOUND);

        private CommonErrorCodes(string value, CommonErrorCode code)
        {
            Value = value;
            Code = (int)code;
        }
        public CommonErrorCodes()
        {
        }
        public string Value { get; set; }
        public int Code { get; set; }
    

}
    public enum CommonErrorCode
    {
    NULL=000,
        FORBIDDEN = 0001,
        UN_AUTHORIZED = 0002,
        OPERATION_FAILED = 0003,
        INVALID_INPUT = 0004,
        FAILED_TO_SAVE_DATA = 0005,
        SERVER_ERROR = 0006,
        FAILED_TO_SEND_CODE = 0007,
        INVALID_EMAIL_OR_PASSWORD = 0008,
        EMAIL_NOT_CONFIRMED = 0009,
        INVALID_REFRESH_TOKEN = 0010,
        RESOURCE_NOT_FOUND = 0011,
        NotAuthorized = 0012,
           NOT_FOUND = 0013,
    

}
}
