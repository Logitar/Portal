using MediatR;

namespace Logitar.Portal.Application.Seeding.Commands;

public record SeedPortalCommand : INotification; // TODO(fpion): Seeding vs Startup User Interface?
