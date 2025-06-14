﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Abstractions.Persistence;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(bool autoSaveEnable = true, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for database context
/// </summary>
public interface IDatabaseContext;
