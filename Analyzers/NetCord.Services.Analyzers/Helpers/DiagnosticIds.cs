using System;
using System.Collections.Generic;
using System.Text;

namespace NetCord.Services.Analyzers.Helpers;
internal static class DiagnosticIds
{
    public const string CommandModuleShouldBeValidRuleId = "NCS0001";
    public const string TypeContainingSlashCommandShouldBeCommandModuleRuleId = "NCS0002";
    public const string SlashCommandClassMustHaveSubSlashCommandMethodsRuleId = "NCS0003";
}
