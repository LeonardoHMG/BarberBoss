using PdfSharp.Fonts;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;

public class BillingReportFontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        throw new NotImplementedException();
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        return new FontResolverInfo(familyName);
    }
}
