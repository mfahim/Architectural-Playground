using System;
using System.Collections.Specialized;
using Nlis.Standard.CommonPackages.Apis.Constants;

namespace MicroServices.Animal.Api.Common.Cqrs
{
	public class BaseCommand
	{
		public BaseCommand(long? RequestId, long AccountId, string ClientId, bool IsCreateCommand = true)
		{
			this.IsCreateCommand = IsCreateCommand;
			this.RequestId = RequestId;
			this.AccountId = AccountId;
			this.ClientId = ClientId;
			CommandBag = new NameValueCollection();
		}

		public long? RequestId { get; }
		public long AccountId { get; }

		// the client id (from the auth token)
		public string ClientId { get; }

		// post/put http verbs
		public bool IsCreateCommand { get; }

		// todo : not sure if it's a good idea to retain some PK-Ids in the bag
		protected NameValueCollection CommandBag { get; }

		public RuleResultModel[] RuleResults { get; private set; }

		public bool IsBridgeRequest => string.Equals(ClientId,
			ClientIds.BridgeClientId, StringComparison.OrdinalIgnoreCase);

		public void AddToCommandBag(string key, string value)
		{
			CommandBag.Add(key.ToLower(), value.ToLower());
		}

		public string GetFromCommandBag(string key)
		{
			if (CommandBag.GetValues(key.ToLower()) != null)
				return CommandBag[key.ToLower()];
			return string.Empty;
		}

		public void AppendRuleResults(RuleResultModel[] ruleResultModels)
		{
			RuleResults = ruleResultModels;
		}
	}
}