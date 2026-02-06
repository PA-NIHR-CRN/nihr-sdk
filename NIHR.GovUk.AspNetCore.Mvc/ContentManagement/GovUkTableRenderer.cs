using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NIHR.GovUk.AspNetCore.Mvc.ContentManagement
{
    public class GovUkTableRenderer : GovUkBaseRenderer
    {
        public GovUkTableRenderer(ContentRendererCollection rendererCollection) : base(rendererCollection)
        {
        }

        public override async Task<string> RenderAsync(IContent content)
        {
            Table table = content as Table;
            var tableTagbuilder = new TagBuilder("table");
            tableTagbuilder.AddCssClass("govuk-table");

            var rowCount = table.Content.Count;

            var headerTagbuilder = new TagBuilder("thead");
            headerTagbuilder.AddCssClass("govuk-table__head");

            var headerRowTagBuilder = new TagBuilder("tr");
            headerRowTagBuilder.AddCssClass("govuk-table__row");

            TableRow header = table.Content[0] as TableRow;
            foreach (TableHeader tableHeader in header.Content)
            {
                string headerContent = null;
                if (tableHeader.Content[0] is Paragraph && tableHeader.Content.Count == 1)
                {
                    Paragraph paragraph = tableHeader.Content[0] as Paragraph;
                    headerContent = await GetInnerHtmlAsync(paragraph.Content);
                }
                headerContent ??= await GetInnerHtmlAsync(tableHeader.Content);

                var headerItem = new TagBuilder("th");
                headerItem.AddCssClass("govuk-table__header");

                headerItem.InnerHtml.SetHtmlContent(headerContent);
                headerRowTagBuilder.InnerHtml.AppendHtml(headerItem);
            }

            headerTagbuilder.InnerHtml.AppendHtml(headerRowTagBuilder);
            tableTagbuilder.InnerHtml.AppendHtml(headerTagbuilder);

            var bodyTagBuilder = new TagBuilder("tbody");
            bodyTagBuilder.AddCssClass("govuk-table__body");

            for (int i = 1; i < rowCount; i++)
            {
                var rowTagBuilder = new TagBuilder("tr");
                rowTagBuilder.AddCssClass("govuk-table__row");

                TableRow tableRow = table.Content[i] as TableRow;
                foreach (TableCell tableCell in tableRow.Content)
                {
                    var tableitem = new TagBuilder("td");
                    tableitem.AddCssClass("govuk-table__cell");

                    tableitem.InnerHtml.SetHtmlContent(await GetInnerHtmlAsync(tableCell.Content));
                    rowTagBuilder.InnerHtml.AppendHtml(tableitem);
                }
                bodyTagBuilder.InnerHtml.AppendHtml(rowTagBuilder);
            }

            tableTagbuilder.InnerHtml.AppendHtml(bodyTagBuilder);

            return RenderTagBuilder(tableTagbuilder);
        }



        public override bool SupportsContent(IContent content)
        {
            return content is Table;
        }
    }
}