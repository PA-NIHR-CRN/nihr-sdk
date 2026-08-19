using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.Models;

public record GovUkAccordionModel (
    TagHelperContent InnerContent) : GovUkModelWithContent(InnerContent);