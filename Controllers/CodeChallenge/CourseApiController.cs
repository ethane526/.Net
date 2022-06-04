using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Build.Framework;
using Sabio.Services.CodeChallenge;
using Sabio.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Sabio.Web.Models.Responses;
using Sabio.Models.Requests.CodeChallenge;
using Sabio.Models.Domain.CodeChallenge;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers.CodeChallenge
{
    [Route("api/courses")]
    [ApiController]
    public class CourseApiController : BaseApiController
    {
        private ICourseService _service = null; 
        public CourseApiController(ICourseService service, ILogger<CourseApiController> logger) : base(logger)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(CourseAddRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int result = _service.AddCourse(model);
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
        public ActionResult<ItemResponse<int>> Update(CourseUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateCourse(model);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }

        [HttpDelete("students/{id:int}")]
        public ActionResult<ItemResponse<int>> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.DeleteStudent(id);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Course>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Course user = _service.GetCourseById(id);
                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse($"User {id} not found.");
                }
                else
                {
                    response = new ItemResponse<Course> { Item = user };
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

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Course>>> GetByPage(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Course> paged = _service.GetCoursesByPage(pageIndex, pageSize);
                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Course>> { Item = paged };
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
    }
}
