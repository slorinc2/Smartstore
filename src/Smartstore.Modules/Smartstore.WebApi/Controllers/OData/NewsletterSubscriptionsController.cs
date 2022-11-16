﻿using Smartstore.Core.Messaging;

namespace Smartstore.Web.Api.Controllers.OData
{
    /// <summary>
    /// The endpoint for operations on NewsLetterSubscription entity.
    /// </summary>
    public class NewsletterSubscriptionsController : WebApiController<NewsletterSubscription>
    {
        private readonly Lazy<IMessageFactory> _messageFactory;

        public NewsletterSubscriptionsController(Lazy<IMessageFactory> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        [HttpGet, ApiQueryable]
        [Permission(Permissions.Promotion.Newsletter.Read)]
        public IQueryable<NewsletterSubscription> Get()
        {
            return Entities.AsNoTracking();
        }

        [HttpGet, ApiQueryable]
        [Permission(Permissions.Promotion.Newsletter.Read)]
        public SingleResult<NewsletterSubscription> Get(int key)
        {
            return GetById(key);
        }

        // INFO: there is no insert permission because a subscription is always created by customer.

        [HttpPost]
        [Permission(Permissions.Promotion.Newsletter.Update)]
        public async Task<IActionResult> Post([FromBody] NewsletterSubscription entity)
        {
            if (!entity.Email.IsEmail())
            {
                return BadRequest($"Provided email address {entity.Email} is invalid.");
            }

            return await PostAsync(entity);
        }

        [HttpPut]
        [Permission(Permissions.Promotion.Newsletter.Update)]
        public Task<IActionResult> Put(int key, Delta<NewsletterSubscription> model)
        {
            return PutAsync(key, model);
        }

        [HttpPatch]
        [Permission(Permissions.Promotion.Newsletter.Update)]
        public Task<IActionResult> Patch(int key, Delta<NewsletterSubscription> model)
        {
            return PatchAsync(key, model);
        }

        [HttpDelete]
        [Permission(Permissions.Promotion.Newsletter.Delete)]
        public Task<IActionResult> Delete(int key)
        {
            return DeleteAsync(key);
        }

        #region Actions and functions

        /// <summary>
        /// Activates a newsletter subscription.
        /// </summary>
        /// <remarks>Also sends an activation message to the related email address.</remarks>
        [HttpPost("NewsletterSubscriptions({key})/Subscribe")]
        [Produces(Json)]
        [ProducesResponseType(typeof(NewsletterSubscription), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        public Task<IActionResult> Subscribe(int key)
        {
            return SubscribeInternal(key, true);
        }

        /// <summary>
        /// Deactivates a newsletter subscription.
        /// </summary>
        /// <remarks>Also sends a deactivation message to the related email address.</remarks>
        [HttpPost("NewsletterSubscriptions({key})/Unsubscribe")]
        [Produces(Json)]
        [ProducesResponseType(typeof(NewsletterSubscription), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        public Task<IActionResult> Unsubscribe(int key)
        {
            return SubscribeInternal(key, false);
        }

        private async Task<IActionResult> SubscribeInternal(int key, bool subscribe)
        {
            try
            {
                var entity = await GetByIdNotNull(key);

                if (subscribe && !entity.Active)
                {
                    entity.Active = true;
                    await _messageFactory.Value.SendNewsletterSubscriptionActivationMessageAsync(entity, entity.WorkingLanguageId);
                    await Db.SaveChangesAsync();
                }
                else if (!subscribe && entity.Active)
                {
                    entity.Active = false;
                    await _messageFactory.Value.SendNewsletterSubscriptionDeactivationMessageAsync(entity, entity.WorkingLanguageId);
                    await Db.SaveChangesAsync();
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        #endregion
    }
}