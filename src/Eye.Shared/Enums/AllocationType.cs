﻿namespace Eye.Shared.Enums;

[Flags]
public enum AllocationType
{
    Commit = 0x1000,
    Reserve = 0x2000
}