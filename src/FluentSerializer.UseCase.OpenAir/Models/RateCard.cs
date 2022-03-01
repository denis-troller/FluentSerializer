﻿using System;
using FluentSerializer.UseCase.OpenAir.Models.Base;

namespace FluentSerializer.UseCase.OpenAir.Models;

internal sealed class RateCard : OpenAirEntity
{
	public string? Name { get; set; }
	public DateTime? LastUpdate { get; set; }
}