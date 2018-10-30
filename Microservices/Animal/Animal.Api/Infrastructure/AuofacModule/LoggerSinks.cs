using System;

namespace MicroServices.Animal.Api.Infrastructure.AuofacModule
{
	[Flags]
	public enum LoggerSinks
	{
		None = 0x0,
		RabbitMq = 0x1,
		RollingFile = 0x2,
		ColoredConsole = 0x4
	}
}