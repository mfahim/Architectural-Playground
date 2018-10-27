using System;
using MicroServices.Animal.Api.Features.Animal.Controller;

namespace MicroServices.Animal.Api.Features.Animal
{
	public class AnimalPayload
	{
		private string _deviceIdentifier;
		public DateTime TransactionDate { get; set; }
		public int? CurrentPropertyIdentifierId { get; set; }
		public string DeviceCompositeKey { get; set; } // the device's business key : (nlisId=3abcj)

		// either nlisId or rfid
		public string DeviceIdentifier
		{
			get
			{
				if (!string.IsNullOrEmpty(DeviceCompositeKey))
					return DeviceCompositeKey.GetDeviceIdentifier();
				return _deviceIdentifier;
			}
			set => _deviceIdentifier = value;
		}

		public string ExclusionReason { get; set; }
	}
}