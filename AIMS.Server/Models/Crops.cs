﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AIMS.Server.Models;

public partial class Crops
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<FarmRecords> FarmRecords { get; set; } = new List<FarmRecords>();

    public virtual ICollection<Prices> Prices { get; set; } = new List<Prices>();

    public virtual Users User { get; set; }
}