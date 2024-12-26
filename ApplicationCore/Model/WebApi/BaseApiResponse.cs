using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Model.WebApi
{
    public class BaseApiResponse
    {
        public BaseApiResponse()
        {
        }

        public BaseApiResponse(object body)
        {
            IsSuccess = true;
            Body = body;
            ErrMsg= string.Empty;
        }


        public bool IsSuccess {  get; set; }
        public object Body { get; set; }
        public string ErrMsg {  get; set; }
    }
}
