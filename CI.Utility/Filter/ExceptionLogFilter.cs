using CI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CI.Data;
using Microsoft.AspNetCore.Mvc;

namespace CI.Utility.Filter
{
    public class ExceptionLogFilter: ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext context)
        {
            var db = context.HttpContext.RequestServices.GetService<CIDBContext>();
            ExceptionModel exception = new ExceptionModel();
            exception.ExceptionMessage = (context.Exception.InnerException == null) ? context.Exception.Message : context.Exception.InnerException.Message;
            exception.StackTrace = context.Exception.StackTrace;
            exception.ControllerName = context.RouteData.Values["controller"].ToString();
            exception.ActionName = context.RouteData.Values["action"].ToString();
            exception.DateofException = DateTime.Now;
            exception.UserName = "jc699907@gmail.com";
            //db.Connection.Close();
            //db.Connection.Open();
            db.ChangeTracker.Clear();
            db.Exceptions.Add(exception);
            db.SaveChanges();            
            context.Result = new BadRequestResult();
            context.ExceptionHandled = true;
            base.OnException(context);
        }
    }
}
