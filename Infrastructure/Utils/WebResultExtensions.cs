using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Utils
{
    public static class WebResultExtensions
    {
        /*

        public static Task<ActionResult> ToCreatedResult<TResult>(this Task<TResult> resultTask, ControllerBase ctrl, string route, Func<TResult, object> routeArgsCreator)
        {
            Func<Task<ActionResult>> createResult = async () =>
            {
                var targetObject = await resultTask;
                var routeArgs = routeArgsCreator(targetObject);
                return new CreatedAtRouteResult(route, routeArgs, targetObject);
            };
//            return await cr

//            return stream.ToResultInternal(ctrl, createResult);
        }
        
        public static Task<IHttpActionResult> ToContentResult<TResult>(this IObservable<TResult> stream,
            ApiController ctrl)
        {
            return stream.ToResultInternal(ctrl, CreateContentResult);
        }

        public static IObservable<TViewModel> ToViewModelsWithCount<TResult, TViewModel>(this IObservable<ICollection<TResult>> stream, Func<ICollection<TResult>, TViewModel> convertFunc)
        {
            return stream.Select(convertFunc);
        }

        public static async Task<IHttpActionResult> ToContentResult<TResult>(this Task<TResult> task, ApiController ctrl, SynchronizationContext ctx, Func<TResult, object> convertFunc = null)
        {
            return await Observable.StartAsync(() => task)
                .Select(p => convertFunc != null ? convertFunc(p) : p)
                .ObserveOn(ctx).ToContentResult(ctrl);
        }


        public static Task<IHttpActionResult> ToFileResult(this IObservable<byte[]> stream, ApiController ctrl)
        {
            return stream.ToResultInternal(ctrl, CreateFileResult);
        }

        public static Task<IHttpActionResult> ToImageResult(this IObservable<byte[]> stream, ApiController ctrl)
        {
            return stream.ToResultInternal(ctrl, CreateImageResult);
        }

        private static IHttpActionResult CreateFileResult(byte[] result, ApiController ctrl)
        {
            return new FileResult(result);
        }

        private static IHttpActionResult CreateImageResult(byte[] result, ApiController ctrl)
        {
            return new ImageResult(result);
        }

        private static IHttpActionResult CreateContentResult<TResult>(TResult result, ApiController ctrl)
        {
            return new NegotiatedContentResult<TResult>(HttpStatusCode.OK, result, ctrl);
        }
        
        private static async Task<ActionResult> ToResultInternal<TResult>(ControllerBase ctrl, Func<Task<ActionResult>> resultProvider)
        {
            try
            {
                var actionResult = await resultProvider();
                return actionResult;
            }
            catch (UnauthorizedAccessException err)
            {
                return Task.FromResult(new UnauthorizedObjectResult(err));
            }

                        LogException(error);
                        if (HandleUnauthorizedException(ctrl, error, source)) return;
                        if (HandleDbEntityUpdateException(ctrl, error, source)) return;
                        if (HandleArgumentNullException(ctrl, error, source)) return;
                        if (HandleValidationException(ctrl, error, source)) return;
                        if (HandleForbiddenAccessResponse(ctrl, error, source)) return;
                        if (HandleBadRequestException(ctrl, error, source)) return;
                        if (!_isDevelopment)
                        {
                            HandleGeneralException(ctrl, error, source);
                            return;
                        }
                        source.SetException(error);
                    },
                    () => { source.TrySetResult(new StatusCodeResult(HttpStatusCode.OK, ctrl)); }
                );

            return await source.Task;
        }
        
        private static void LogException(Exception error)
        {
            _logger.Error(error, error.Message);
        }

        private static void HandleGeneralException(ApiController ctrl, Exception e, TaskCompletionSource<IHttpActionResult> source)
        {
            source.TrySetResult(new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = e.Message,
                Content = new StringContent(e.Message)
            }));
        }

        private static bool HandleUnauthorizedException(ApiController ctrl, Exception e, TaskCompletionSource<IHttpActionResult> source)
        {
            var unauthorizedException = e as UnauthorizedAccessException;
            if (unauthorizedException != null)
            {

                return true;
            }
            return false;
        }

        private static bool HandleBadRequestException(ApiController ctrl, Exception e, TaskCompletionSource<IHttpActionResult> source)
        {
            if (e is BadRequestException)
            {
                var badRequestException = e as BadRequestException;
                source.TrySetResult(new ResponseMessageResult(new HttpResponseMessage(badRequestException.StatusCode)
                {
                    ReasonPhrase = badRequestException.Message,
                    Content = new StringContent(badRequestException.Message)
                }));
                return true;
            }
            else if (e is NotSupportedException)
            {
                source.TrySetResult(new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = e.Message,
                    Content = new StringContent(e.Message)
                }));
                return true;
            }
            return false;
        }

        private static bool HandleDbEntityUpdateException(ApiController ctrl, Exception e,
            TaskCompletionSource<IHttpActionResult> source)
        {
            var dbUpdateError = e as DbUpdateConcurrencyException;
            if (dbUpdateError != null)
            {
                source.TrySetResult(new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Entity requested for update does not exist in database.",
                    Content = new StringContent("Entity requested for update does not exist in database.")
                }));
                return true;
            }

            return false;
        }

        private static bool HandleArgumentNullException(ApiController ctrl, Exception e,
            TaskCompletionSource<IHttpActionResult> source)
        {
            var operationError = e as ArgumentNullException;
            if (operationError != null)
            {
                source.TrySetResult(new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Parameter type requested for query doesn't exist on queried entity.",
                    Content = new StringContent("Parameter type requested for query doesn't exist on queried entity.")
                }));
                return true;
            }

            return false;
        }

        private static bool HandleValidationException(ApiController ctrl, Exception e,
            TaskCompletionSource<IHttpActionResult> source)
        {
            var validationError = e as ValidationException;
            if (validationError != null)
            {
                var response = new ValidationErrorResponse()
                {
                    Message = e.Message,
                    Details = validationError.Errors.Select(p => p.ErrorMessage).ToArray()
                };
                source.TrySetResult(
                    new NegotiatedContentResult<ValidationErrorResponse>((HttpStatusCode)422, response, ctrl));
                return true;
            }

            return false;
        }

        private static bool HandleForbiddenAccessResponse(ApiController ctrl, Exception e,
            TaskCompletionSource<IHttpActionResult> source)
        {
            var validationError = e as ForbiddenAccessException;
            if (validationError != null)
            {
                var response = new ValidationErrorResponse()
                {
                    Message = "Forbidden access",
                    Details = new string[] { $"Cannot access {ctrl.Request.RequestUri} - permission denied" }
                };
                source.TrySetResult(
                    new NegotiatedContentResult<ValidationErrorResponse>(HttpStatusCode.Forbidden, response, ctrl));
                return true;
            }

            return false;
        }

        public class ValidationErrorResponse
        {
            public string Message { get; set; }
            public string[] Details { get; set; }
        }
    }

    public class FileResult : IHttpActionResult
    {
        public readonly byte[] _data;
        public FileResult(byte[] data)
        {
            _data = data;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            // 'using' here breaks the task
            Stream decryptedStream = new MemoryStream(_data);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(decryptedStream)
            };

            var contentType = MediaTypeNames.Application.Zip;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return Task.FromResult(response);
        }
    }

    public class ImageResult : IHttpActionResult
    {
        public readonly byte[] _data;
        public ImageResult(byte[] data)
        {
            _data = data;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            // 'using' here breaks the task
            Stream decryptedStream = new MemoryStream(_data);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(decryptedStream)
            };

            var contentType = MediaTypeNames.Image.Jpeg;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return Task.FromResult(response);
        }
    */
    }
}
