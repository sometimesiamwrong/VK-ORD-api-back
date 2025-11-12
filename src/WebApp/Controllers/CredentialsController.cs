using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Extensions;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.Services.Interfaces;
using WebApp.Security;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CredentialsController : BaseController
    {
        private readonly IApiCredentialService _service;

        public CredentialsController(IApiCredentialService service)
        {
            _service = service;
        }

        [HttpGet("v1/{userId:guid}")]
        public async Task<List<ApiCredentialResponse>> Get(Guid userId, CancellationToken cancellationToken)
        {
            return await _service.GetAll(userId, cancellationToken);
        }

        [HttpGet("v1/{userId:guid}/{credentialPublicId:guid}")]
        public async Task<ApiCredentialResponse?> GetById(Guid userId, Guid credentialPublicId, CancellationToken cancellationToken)
        {
            return await _service.GetById(credentialPublicId, userId, cancellationToken);
        }

        [HttpPost("v1/")]
        public async Task<ApiCredentialResponse> Create([FromBody] CreateApiCredentialRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();
            return await _service.Create(request, userId, cancellationToken);
        }

        [HttpPut("v1/{id:guid}")]
        public Task<ApiCredentialResponse?> Update(Guid id, [FromBody] UpdateApiCredentialRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();
            return _service.Update(id, request, userId, cancellationToken);
        }

        [HttpDelete("v1/{id:guid}")]
        public Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetUserId();
            return _service.Delete(id, userId, cancellationToken);
        }
    }
}


