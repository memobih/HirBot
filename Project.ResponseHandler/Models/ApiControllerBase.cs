using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ResponseHandler.Models
{
    public class ApiControllerBase : ControllerBase
    {
        protected ActionResult ProcessResponse(ResponseType errorCode, string erroMessage = "")
        {
            return StatusCode((int)errorCode, new { code = CommonErrorCodes.NULL.Code, erroMessage });
        }
    
        protected ActionResult ProcessResponse<T>(APIOperationResponse<T> response)
        {
            return response.StatusCode== (int)ResponseType.Success ? Ok(response.Data) : StatusCode((int)response.StatusCode, new { code = response.Code.Code, value = response.Code.Value, response.Message });
        }
      


    }
}
