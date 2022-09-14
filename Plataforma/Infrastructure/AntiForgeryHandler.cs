using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.Tasks;
using BstHelpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Plataforma.Infrastructure;

public sealed class AntiForgeryHandler {
    private readonly RequestDelegate _next;
    private readonly IAntiforgery _antiForgery;

    public AntiForgeryHandler(RequestDelegate next, IAntiforgery antiForgery) {
        _next = next;
        _antiForgery = antiForgery;
    }

    public async Task Invoke(HttpContext context) {
        var validAntiForgery = true;
        var validateAntiForgery = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata?
            .Any(m => m is ValidateAntiForgeryTokenAttribute) ?? false;
        if (validateAntiForgery && HttpMethods.IsPost(context.Request.Method)) {
            try {
                validAntiForgery = await _antiForgery.IsRequestValidAsync(context);
            } catch (Exception e) {
                e.LogString();
                validAntiForgery = false;
            }
        }
        if (!validAntiForgery) {
            try {
                await _next(context);
            } catch {/*Ignored*/ }
            context.Response.Redirect(context.Request.GetDisplayUrl());
        } else {
            await _next(context);
        }

    }

}