﻿namespace BusinessObject;

public abstract class BaseEntity<TEntityId> where TEntityId : notnull
{
    public TEntityId Id { get; set; } = default!;
}
