using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public record GovUkRadiosModel (GovUkRadioSize Size, GovUkRadioLayout Layout,
    TagHelperContent InnerContent) : GovUkModelWithContent(InnerContent);