using NIHR.GovUk.AspNetCore.Mvc.Models;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.Views;

public static class GovUkStylingHelper
{
    public static string GetFieldSetLegendSizeClass(GovUkHeadingSize size)
        => size switch
        {
            GovUkHeadingSize.Small => "govuk-fieldset__legend--s",
            GovUkHeadingSize.Medium => "govuk-fieldset__legend--m",
            GovUkHeadingSize.Large => "govuk-fieldset__legend--l",
            GovUkHeadingSize.ExtraLarge => "govuk-fieldset__legend--xl",
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

    public static string? GetTagColourClass(GovUkTagColour colour) =>
        colour switch
        {
            GovUkTagColour.Default => null,
            GovUkTagColour.Grey => "govuk-tag--grey",
            GovUkTagColour.Green => "govuk-tag--green",
            GovUkTagColour.Teal => "govuk-tag--teal",
            GovUkTagColour.Blue => "govuk-tag--blue",
            GovUkTagColour.Purple => "govuk-tag--purple",
            GovUkTagColour.Magenta => "govuk-tag--magenta",
            GovUkTagColour.Red => "govuk-tag--red",
            GovUkTagColour.Orange => "govuk-tag--orange",
            GovUkTagColour.Yellow => "govuk-tag--yellow",
            _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, null)
        };

    public static string GetRadioLayoutClass(GovUkRadioLayout layout) =>
        layout switch
        {
            GovUkRadioLayout.Default => string.Empty,
            GovUkRadioLayout.Inline => "govuk-radios--inline",
            _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, null)
        };
}