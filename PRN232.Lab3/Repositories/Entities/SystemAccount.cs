﻿using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class SystemAccount : BaseEntity<int>
{
    //public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Role { get; set; }

    public bool? IsActive { get; set; }
}
