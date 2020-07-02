using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using CustManSvc.API.Common;

namespace CustManSvc.API.Filters
{
	public class ServiceException : TypeFilterAttribute
	{
		public ServiceException()
			: base(typeof(ExceptionFilterImpl))
		{}

		private class ExceptionFilterImpl : IExceptionFilter
		{
			private ILogger<ExceptionFilterImpl> _logger;
			private IWebHostEnvironment _env;

			public ExceptionFilterImpl(ILogger<ExceptionFilterImpl> logger, IWebHostEnvironment env)
			{
				_logger = logger;
				_env = env;
			}

			public void OnException(ExceptionContext context)
			{
				_logger.LogError(context.Exception, string.Empty);

				var se = new ServiceError(context.Exception.Message);
				if (_env.IsDevelopment())
				{
					se.DetailedMessage = context.Exception.StackTrace;
				}

				context.Result = new ObjectResult(se)
				{
					StatusCode = (int)HttpStatusCode.InternalServerError 
				};
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}
		}
	}
}