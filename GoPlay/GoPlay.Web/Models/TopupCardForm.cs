using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GoPlay.Core;

namespace GoPlay.Web.Models
{
    public class TopupCardForm : IValidatableObject
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string cardNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string cardPassword { get; set; }
        public string submit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var api = GoPlayApi.Instance;
            var card = api.GetTopUpCard(this.cardNumber, cardPassword);
            if (!card.HasData)
            {
                yield return new ValidationResult(Resources.Resources.Card_Number_Password_combination_does_not_match, new[] { "cardNumber" });
            }
            if (card.HasData)
            {
                if (card.Data.used_at.HasValue)
                {
                    yield return new ValidationResult(Resources.Resources.The_card_has_already_been_used, new[] { "cardNumber" });
                }
                var now = DateTime.UtcNow;
                if (card.Data.validity_date < now)
                {
                    yield return new ValidationResult(Resources.Resources.The_card_has_already_expired, new[] { "cardNumber" });
                }
            }
        }
    }
}