using ClosedXML.Excel;

namespace Cotisations.Excel;

public static class XlCellExtensions
{
    public static void AddLongComment(this IXLCell cell, string comment)
    {
        cell.CreateComment().AddText(comment);
        cell.GetComment().Style
            .Size.SetHeight(50)
            .Size.SetWidth(100);
    }

    public static void AddShortComment(this IXLCell cell, string comment)
    {
        cell.CreateComment().AddText(comment);
        cell.GetComment().Style.Size.SetAutomaticSize();
    }

    public static IXLCell WithImportantValue(this IXLCell cell, XLCellValue value)
    {
        cell.Value = value;
        cell.Style.Font.SetBold();
        return cell;
    }

    public static IXLCell WithImportantFormula(this IXLCell cell, string formula)
    {
        cell.FormulaA1 = formula;
        cell.Style.Font.SetBold();
        return cell;
    }

    public static IXLCell WithLargeComment(this IXLCell cell, string comment)
    {
        cell.AddLongComment(comment);
        return cell;
    }

    public static IXLCell WithShortComment(this IXLCell cell, string comment)
    {
        cell.AddShortComment(comment);
        return cell;
    }

    public static IXLCell WithPercentFormat(this IXLCell cell)
    {
        cell.Style.NumberFormat.SetFormat("0.0#%");
        return cell;
    }

    public static IXLCell WithPlainNumberFormat(this IXLCell cell)
    {
        cell.Style.NumberFormat.SetFormat("###0");
        return cell;
    }
}