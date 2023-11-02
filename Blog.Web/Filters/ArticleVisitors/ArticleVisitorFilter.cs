﻿using Blog.Data.UnitOfWorks;
using Blog.Entity.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Web.Filters.ArticleVisitors
{
    public class ArticleVisitorFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork unitOfWork;

        public ArticleVisitorFilter(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            List<Visitor> visitors = unitOfWork.GetRepository<Visitor>().GetAllAsync().Result;

            string getIp = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string getUserAgent = context.HttpContext.Request.Headers["User-Agent"];

            Visitor visitor = new(getIp, getUserAgent);

            if (visitors.Any(v => v.IpAddress == visitor.IpAddress))
                return next();

            else
            {
                unitOfWork.GetRepository<Visitor>().AddAsync(visitor);
                unitOfWork.Save();
            }
                return next();
        }
    }
}
