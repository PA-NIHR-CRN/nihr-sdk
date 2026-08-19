using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public record GovUkRadioModel (
    string Name,
    string Value,
    bool IsSelected,
    bool Autofocus,
    TagHelperContent InnerContent) : GovUkModelWithContent(InnerContent);