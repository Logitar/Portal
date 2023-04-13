using MediatR;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Commands;

public record MigrateDatabase : IRequest;
