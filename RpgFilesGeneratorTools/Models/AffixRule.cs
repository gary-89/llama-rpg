﻿using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Models;

internal sealed record AffixRule(
    IReadOnlyList<string> ItemTypes,
    int Tier,
    bool IsRare,
    bool IsElite,
    int ItemLevel,
    int Frequency,
    string Modifier1,
    string Modifier1Min,
    string Modifier1Max);
