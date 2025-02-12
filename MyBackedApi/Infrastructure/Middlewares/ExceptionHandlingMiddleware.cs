using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using Infrastructure.Base;
using Infrastructure.Exceptions;

namespace Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ResourceMissingException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (OperationNotAllowedException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (ResourceRequiredException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (ResourceAlreadyExistsException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (WrongInputException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (PhoneNumberAlreadyExistsException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (EmailAlreadyExistsException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (InvalidPhoneNumberException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex);
            }
            catch (UnauthenticatedException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message, ex);
            }
            catch (UnauthorizedException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message, ex);
            }
            catch (AmazonS3Exception ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message, ex);
            }
            catch (AmazonServiceException ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message, ex);
            }
            catch (Exception ex)
            {
                await RespondToExceptionAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error", ex);
            }
        }

        private static Task RespondToExceptionAsync(HttpContext context, HttpStatusCode failureStatusCode, string message, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)failureStatusCode;

            var response = new BaseResponseError();
            response.Message = message;
            response.Error = exception.GetType().Name;
            response.Timestamp = DateTime.UtcNow;

            context.Response.Headers.Append("X-Error-Message", response.Message);
            context.Response.Headers.Append("X-Error-Type", response.Error);
            context.Response.Headers.Append("X-Error-StackTrace", exception.StackTrace?.Substring(0, Math.Min(exception.StackTrace.Length, 100)) ?? "");

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
}