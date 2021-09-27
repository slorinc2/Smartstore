﻿using System;
using Smartstore.Forums.Domain;
using Smartstore.Web.Modelling;
using Smartstore.Web.Models.Customers;

namespace Smartstore.Forums.Models.Public
{
    public partial class PublicForumPostModel : EntityModelBase
    {
        public int ForumTopicId { get; set; }
        public string ForumTopicSlug { get; set; }
        public string ForumTopicSubject { get; set; }
        public int CurrentTopicPage { get; set; }

        public string FormattedText { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool IsForumModerator { get; set; }
        public bool IsGuest { get; set; }
        public bool HasCustomerProfile { get; set; }
        public CustomerAvatarModel Avatar { get; set; }

        public ForumModerationPermits ModerationPermits { get; set; }
        public bool CanEditPost
        {
            get => ModerationPermits.HasFlag(ForumModerationPermits.CanEditPost);
        }
        public bool CanDeletePost
        {
            get => ModerationPermits.HasFlag(ForumModerationPermits.CanDeletePost);
        }
        public bool CanCreatePrivateMessages
        {
            get => ModerationPermits.HasFlag(ForumModerationPermits.CanCreatePrivateMessages);
        }

        public string PostCreatedOnStr { get; set; }
        public bool Published { get; set; }

        public bool ShowCustomersPostCount { get; set; }
        public int ForumPostCount { get; set; }

        public bool ShowCustomersJoinDate { get; set; }
        public DateTime CustomerJoinDate { get; set; }

        public bool ShowCustomersLocation { get; set; }
        public string CustomerLocation { get; set; }

        public bool SignaturesEnabled { get; set; }
        public string FormattedSignature { get; set; }

        public bool CanVote { get; set; }
        public bool Vote { get; set; }
        public int VoteCount { get; set; }
    }
}