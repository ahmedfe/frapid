﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Frapid.ApplicationState.Cache;
using Frapid.Account.DTO;
using Frapid.Messaging;
using Frapid.Messaging.DTO;
using Frapid.Messaging.Helpers;

namespace Frapid.Account.Messaging
{
    public class SignUpEmail
    {
        private readonly Registration _registration;
        private readonly string _registrationId;

        public SignUpEmail(Registration registration, string registrationId)
        {
            this._registration = registration;
            this._registrationId = registrationId;
        }

        private string GetTemplate()
        {
            var path = "~/Catalogs/{catalog}/Areas/Frapid.Account/EmailTemplates/account-verification.html";
            path = path.Replace("{catalog}", AppUsers.GetCatalog());
            path = HostingEnvironment.MapPath(path);

            if (!File.Exists(path))
            {
                return string.Empty;
            }

            return path != null ? File.ReadAllText(path, Encoding.UTF8) : string.Empty;
        }

        private string ParseTemplate(string template)
        {
            var siteUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var link = siteUrl + "/account/sign-up/confirm?token=" + this._registrationId;

            var parsed = template.Replace("{{Name}}", this._registration.Name);
            parsed = parsed.Replace("{{EmailAddress}}", this._registration.Email);
            parsed = parsed.Replace("{{VerificationLink}}", link);
            parsed = parsed.Replace("{{SiteUrl}}", siteUrl);

            return parsed;
        }

        private EmailQueue GetEmail(string catalog, Registration model, string subject, string message)
        {
            Config config = new Config(catalog);

            return new EmailQueue
            {
                AddedOn = DateTime.Now,
                FromName = model.Name,
                ReplyTo = model.Email,
                Subject = subject,
                Message = message,
                SendTo = config.FromEmail
            };
        }

        public async Task SendAsync()
        {
            var template = this.GetTemplate();
            var parsed = this.ParseTemplate(template);
            var subject = "Confirm Your Registration at " + HttpContext.Current.Request.Url.Authority;

            var catalog = AppUsers.GetCatalog();
            var email = this.GetEmail(catalog, this._registration, subject, parsed);
            var queue = new MailQueueManager(catalog, email);
            queue.Add();
            await queue.ProcessMailQueueAsync(EmailProcessor.GetDefault());
        }
    }
}