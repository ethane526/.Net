using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.TechCompanies;
using Sabio.Models.Requests.Friends;
using Sabio.Models.Requests.TechCompanies;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/techcompanies")]
    [ApiController]
    public class TechCompanyApiController : BaseApiController
    {
        private ITechCompanyService _service = null;
        
        public TechCompanyApiController(ITechCompanyService service,
            IFriendService friend,
            ILogger<TechCompanyApiController> logger) : base(logger)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ItemsResponse<TechCompany>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<TechCompany> list = _service.GetAll();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<TechCompany> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }


        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<TechCompany>> Get(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                TechCompany company = _service.Get(id);
                if (company == null)
                {
                    code = 404;
                    response = new ErrorResponse("Company Id not found");
                }
                else
                {
                    response = new ItemResponse<TechCompany> { Item = company };
                }
            }
            catch (SqlException argEx)
            {
                code = 500;
                response = new ErrorResponse($"SqlException Error: {argEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                code = 500;
                response = new ErrorResponse($"ArgumentException Error: {argEx.Message}");
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(TechCompanyAddRequest companyModel)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int result = _service.Add(companyModel);
                response = new ItemResponse<int>() { Item = result };
            }
            catch (SqlException argEx)
            {
                code = 500;
                response = new ErrorResponse($"SqlException Error: {argEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                code = 500;
                response = new ErrorResponse($"ArgumentException Error: {argEx.Message}");
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }


            return StatusCode(code, response);
        }

        

        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(TechCompanyUpdateRequest model)
        {
            
            int code = 200;
            BaseResponse response = null;
            try
            {
                _service.Update(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ItemResponse<int>> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }
    
    }
    
}
